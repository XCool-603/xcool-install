using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Installer.Abstractions.Model;
using Installer.Abstractions.Packaging;
using Installer.Core.Common;
using Installer.Core.Pe;
using Installer.Core.Security;

namespace Installer.Core.Packaging
{
    /// <summary>源目录里的一个文件。</summary>
    public sealed class SourceFile
    {
        /// <summary>相对源目录的路径（反斜杠分隔）。</summary>
        public string RelativePath { get; set; }

        /// <summary>磁盘上的完整路径。</summary>
        public string FullPath { get; set; }

        /// <summary>字节数。</summary>
        public long Length { get; set; }

        /// <summary>SHA-256（小写十六进制）。</summary>
        public string Sha256 { get; set; }
    }

    /// <summary>校验结果。</summary>
    public sealed class PackageVerifyResult
    {
        /// <summary>是否是本程序的容器。</summary>
        public bool IsContainer { get; set; }

        /// <summary>容器索引。</summary>
        public ContainerInfo Info { get; set; }

        /// <summary>payload 哈希是否匹配。</summary>
        public bool PayloadHashOk { get; set; }

        /// <summary>stub 哈希是否匹配。</summary>
        public bool StubHashOk { get; set; }

        /// <summary>manifest 是否解析成功。</summary>
        public bool ManifestOk { get; set; }

        /// <summary>解析出的清单。</summary>
        public InstallerManifest Manifest { get; set; }

        /// <summary>清单里的文件条目数。</summary>
        public int EntryCount { get; set; }

        /// <summary>payload 里实际的 zip 条目数。</summary>
        public int ZipEntryCount { get; set; }

        /// <summary>清单里的文件是否都能在 payload 里找到。</summary>
        public bool AllFilesPresent { get; set; }

        /// <summary>缺失的文件（相对路径）。</summary>
        public List<string> MissingFiles { get; set; } = new List<string>();

        /// <summary>诊断消息。</summary>
        public List<string> Messages { get; set; } = new List<string>();

        /// <summary>整体是否通过。</summary>
        public bool Ok
        {
            get { return IsContainer && PayloadHashOk && StubHashOk && ManifestOk && AllFilesPresent; }
        }
    }

    /// <summary>
    /// 打包流水线。九个阶段，每个阶段是纯函数式的输入→输出，可单测、可重跑。
    ///
    /// <code>
    /// Validate → Collect → Hash → PatchIcon → Compose → Sign → Verify → Done
    /// </code>
    ///
    /// <see cref="BuildStage.Verify"/> 是这套实现里最要紧的一步：它用**运行时的同一个
    /// <see cref="ContainerFormat"/>** 把刚打出来的包回读一遍。
    /// 因此"打出来的包装不了"在打包阶段就会失败，而不是等发到用户手上。
    /// </summary>
    public sealed class PackageBuilder
    {
        /// <summary>打包器版本，写入清单的 <c>build.packerVersion</c>。</summary>
        public const string PackerVersion = "2.0.0";

        private const int CopyBufferSize = 1 << 20; // 1 MB

        private static readonly DateTimeOffset ZipTimeMin = new DateTimeOffset(1980, 1, 1, 0, 0, 0, TimeSpan.Zero);
        private static readonly DateTimeOffset ZipTimeMax = new DateTimeOffset(2107, 12, 31, 23, 59, 58, TimeSpan.Zero);

        /// <summary>同步打包。</summary>
        public PackageBuildResult Build(
            PackageBuildOptions options,
            IProgress<BuildProgress> progress = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return BuildAsync(options, progress, cancellationToken).GetAwaiter().GetResult();
        }

        /// <summary>异步打包。</summary>
        public async Task<PackageBuildResult> BuildAsync(
            PackageBuildOptions options,
            IProgress<BuildProgress> progress = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var result = new PackageBuildResult();
            var totalSw = Stopwatch.StartNew();

            // ── Stage 1：校验 ────────────────────────────────────────────
            var sw = Stopwatch.StartNew();
            Report(progress, BuildStage.Validate, "校验打包参数…", 0);
            Validate(options);
            var timestamp = options.BuildTimestamp ?? DateTimeOffset.UtcNow;
            result.BuildTimestamp = timestamp;
            result.StageMilliseconds[BuildStage.Validate] = sw.ElapsedMilliseconds;

            var manifest = options.Manifest;
            var outputFull = Path.GetFullPath(options.OutputPath);
            var sourceFull = Path.GetFullPath(options.SourceDir);
            var tempPath = outputFull + ".tmp";

            if (File.Exists(tempPath))
            {
                File.Delete(tempPath);
            }

            try
            {
                // ── Stage 2：收集 ────────────────────────────────────────
                sw = Stopwatch.StartNew();
                Report(progress, BuildStage.Collect, "扫描源目录…", 5);
                var files = CollectFiles(sourceFull, outputFull, cancellationToken);
                var directories = CollectEmptyDirectories(sourceFull);
                Report(progress, BuildStage.Collect,
                    string.Format(CultureInfo.InvariantCulture, "扫描完成：{0} 个文件，{1} 个空目录", files.Count, directories.Count),
                    10);
                result.StageMilliseconds[BuildStage.Collect] = sw.ElapsedMilliseconds;

                // ── Stage 3：哈希 ────────────────────────────────────────
                sw = Stopwatch.StartNew();
                for (var i = 0; i < files.Count; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var f = files[i];
                    f.Sha256 = Hashing.ComputeFile(f.FullPath);
                    if (i % 16 == 0 || i == files.Count - 1)
                    {
                        var pct = 10 + (int)(20.0 * (i + 1) / files.Count);
                        Report(progress, BuildStage.Hash,
                            string.Format(CultureInfo.InvariantCulture, "计算哈希 {0}/{1}：{2}", i + 1, files.Count, f.RelativePath),
                            pct);
                    }
                }

                result.StageMilliseconds[BuildStage.Hash] = sw.ElapsedMilliseconds;

                // ── Stage 4：给 stub 补图标（在追加 payload 之前）─────────
                sw = Stopwatch.StartNew();
                var stubBytes = File.ReadAllBytes(options.StubPath);
                if (!options.SkipIconPatch && !string.IsNullOrEmpty(options.IconPath))
                {
                    Report(progress, BuildStage.PatchIcon, "正在替换安装包图标…", 32);
                    var icoBytes = File.ReadAllBytes(options.IconPath);
                    stubBytes = PeIconPatcher.ReplaceIcon(stubBytes, icoBytes);
                    result.IconPatched = true;
                }

                result.StageMilliseconds[BuildStage.PatchIcon] = sw.ElapsedMilliseconds;

                // ── Stage 5：组装 ────────────────────────────────────────
                sw = Stopwatch.StartNew();
                Report(progress, BuildStage.Compose, "正在组装容器…", 35);

                long payloadOffset;
                long payloadLength;
                long manifestOffset;
                byte[] manifestBytes;

                using (var output = new FileStream(tempPath, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.None, CopyBufferSize))
                {
                    // ① stub
                    output.Write(stubBytes, 0, stubBytes.Length);

                    // ② payload（流式压缩，不落地中间文件）
                    // 必须套 OffsetWriteStream：ZipArchive 在 Create 模式下会把**绝对**流位置
                    // 写进 EOCD，直接挂在偏移后的 FileStream 上会打出只能在容器起点读的坏 zip。
                    payloadOffset = output.Position;
                    using (var region = new OffsetWriteStream(output, payloadOffset))
                    {
                        await WritePayloadAsync(region, sourceFull, files, directories, timestamp, options.CompressionLevel, progress, cancellationToken)
                            .ConfigureAwait(false);
                        payloadLength = region.WrittenLength;
                    }

                    output.Seek(payloadOffset + payloadLength, SeekOrigin.Begin);

                    // 清单在 payload 之后才写，因为 build.payloadBytes 需要 payloadLength
                    manifest.Files = files
                        .Select(f => new FileEntry { Path = f.RelativePath, Size = f.Length, Sha256 = f.Sha256 })
                        .ToList();
                    manifest.Directories = directories;
                    manifest.Build = new BuildInfo
                    {
                        BuiltAtUtc = timestamp.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture),
                        PackerVersion = string.IsNullOrEmpty(options.PackerVersion) ? PackerVersion : options.PackerVersion,
                        EntryCount = files.Count,
                        PayloadBytes = payloadLength,
                    };

                    ManifestSerializer.Validate(manifest);
                    manifestBytes = ManifestSerializer.SerializeToUtf8Bytes(manifest);

                    // ③ manifest
                    manifestOffset = output.Position;
                    output.Write(manifestBytes, 0, manifestBytes.Length);
                    var endPosition = output.Position;

                    // ④ footer
                    // 注意：ComputeRegionBytes 会把流定位到 payload 区间末尾，
                    // 所以必须先算哈希、再 Seek 回文件末尾，否则 footer 会被写在
                    // manifest 上面（这个 bug 就是被 Stage 7 的自校验抓出来的）。
                    var payloadHash = Hashing.ComputeRegionBytes(output, payloadOffset, payloadLength);
                    output.Seek(endPosition, SeekOrigin.Begin);

                    var info = new ContainerInfo
                    {
                        FormatVersion = ContainerFormatSpec.FormatVersion,
                        Flags = ContainerFlags.None,
                        PayloadOffset = payloadOffset,
                        PayloadLength = payloadLength,
                        ManifestOffset = manifestOffset,
                        ManifestLength = manifestBytes.Length,
                        PayloadSha256 = payloadHash,
                        StubSha256 = Hashing.Sha256Raw(stubBytes),
                        FileLength = endPosition + ContainerFormatSpec.FooterSize,
                    };

                    Report(progress, BuildStage.Compose, "正在写入容器索引…", 85);
                    ContainerFormat.WriteFooter(output, info);
                    output.Flush(true);

                    result.StubBytes = payloadOffset;
                    result.PayloadBytes = payloadLength;
                    result.ManifestBytes = manifestBytes.Length;
                    result.PayloadSha256 = Hashing.ToHex(info.PayloadSha256);
                }

                result.StageMilliseconds[BuildStage.Compose] = sw.ElapsedMilliseconds;

                // ── Stage 6：签名（必须在图标补丁之后）────────────────────
                sw = Stopwatch.StartNew();
                if (!string.IsNullOrEmpty(options.SignToolPath))
                {
                    Report(progress, BuildStage.Sign, "正在数字签名…", 90);
                    SignToolRunner.Sign(tempPath, options);
                }

                result.StageMilliseconds[BuildStage.Sign] = sw.ElapsedMilliseconds;

                // ── Stage 7：自校验 ──────────────────────────────────────
                sw = Stopwatch.StartNew();
                Report(progress, BuildStage.Verify, "回读自校验…", 95);
                var verify = VerifyPackage(tempPath);
                if (!verify.Ok)
                {
                    throw new PackageBuildException(
                        "打包自校验失败：" + string.Join("；", verify.Messages.ToArray()));
                }

                result.StageMilliseconds[BuildStage.Verify] = sw.ElapsedMilliseconds;

                // ── 原子替换 ────────────────────────────────────────────
                var parent = Path.GetDirectoryName(outputFull);
                if (!string.IsNullOrEmpty(parent) && !Directory.Exists(parent))
                {
                    Directory.CreateDirectory(parent);
                }

                if (File.Exists(outputFull))
                {
                    File.Delete(outputFull);
                }

                File.Move(tempPath, outputFull);

                result.OutputPath = outputFull;
                result.OutputBytes = new FileInfo(outputFull).Length;
                result.OutputSha256 = Hashing.ComputeFile(outputFull);
                result.EntryCount = files.Count;

                result.StageMilliseconds[BuildStage.Done] = totalSw.ElapsedMilliseconds;
                Report(progress, BuildStage.Done,
                    string.Format(CultureInfo.InvariantCulture, "完成：{0}（{1:N0} 字节）", Path.GetFileName(outputFull), result.OutputBytes),
                    100);
                return result;
            }
            finally
            {
                if (File.Exists(tempPath))
                {
                    try
                    {
                        File.Delete(tempPath);
                    }
                    catch (IOException)
                    {
                        // 清理失败不掩盖真正的异常
                    }
                }
            }
        }

        // ─────────────────────────────────────────────────────────────
        // Stage 1
        // ─────────────────────────────────────────────────────────────

        private static void Validate(PackageBuildOptions options)
        {
            if (options == null)
            {
                throw new PackageBuildException("打包选项为 null。");
            }

            if (options.ExternalPayload)
            {
                throw new NotSupportedException(
                    "外置 payload（差分更新用）尚未实现 —— 该能力已在容器格式里预留 flags 位。");
            }

            if (options.Manifest == null)
            {
                throw new PackageBuildException("必须提供清单（options.Manifest）。");
            }

            if (string.IsNullOrWhiteSpace(options.SourceDir))
            {
                throw new PackageBuildException("必须提供源目录（options.SourceDir）。");
            }

            if (!Directory.Exists(options.SourceDir))
            {
                throw new PackageBuildException("源目录不存在：" + options.SourceDir);
            }

            if (string.IsNullOrWhiteSpace(options.StubPath))
            {
                throw new PackageBuildException("必须提供 stub 路径（options.StubPath）。");
            }

            if (!File.Exists(options.StubPath))
            {
                throw new PackageBuildException("stub 不存在：" + options.StubPath);
            }

            var stubHead = new byte[2];
            using (var fs = File.OpenRead(options.StubPath))
            {
                if (fs.Read(stubHead, 0, 2) != 2 || stubHead[0] != (byte)'M' || stubHead[1] != (byte)'Z')
                {
                    throw new PackageBuildException("stub 不是有效的 PE 文件（缺少 MZ 头）：" + options.StubPath);
                }
            }

            // 提前检查体积：太小的"stub"根本不是可执行文件，等到容器布局校验才报错会很难定位
            var stubSize = new FileInfo(options.StubPath).Length;
            if (stubSize < ContainerFormatSpec.MinimumStubSize)
            {
                throw new PackageBuildException(string.Format(CultureInfo.InvariantCulture,
                    "stub 太小（{0:N0} 字节，至少需要 {1:N0}）—— 请确认指定的是 Installer.Runtime.exe 而不是别的东西：{2}",
                    stubSize, ContainerFormatSpec.MinimumStubSize, options.StubPath));
            }

            if (string.IsNullOrWhiteSpace(options.OutputPath))
            {
                throw new PackageBuildException("必须提供输出路径（options.OutputPath）。");
            }

            if (!string.IsNullOrEmpty(options.IconPath) && !File.Exists(options.IconPath))
            {
                throw new PackageBuildException("图标文件不存在：" + options.IconPath);
            }

            // 清单本身（此时 files 还没扫描，所以不要求非空）
            ManifestSerializer.Validate(options.Manifest, false);
        }

        // ─────────────────────────────────────────────────────────────
        // Stage 2
        // ─────────────────────────────────────────────────────────────

        private static List<SourceFile> CollectFiles(string sourceFull, string outputFull, CancellationToken ct)
        {
            var result = new List<SourceFile>();
            var outputDir = Path.GetDirectoryName(outputFull);
            var outputName = Path.GetFileName(outputFull);

            foreach (var path in Directory.EnumerateFiles(sourceFull, "*", SearchOption.AllDirectories))
            {
                ct.ThrowIfCancellationRequested();

                var full = Path.GetFullPath(path);

                // 不要把产物自己打进包里
                if (string.Equals(full, outputFull, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (!string.IsNullOrEmpty(outputDir) &&
                    string.Equals(full, Path.Combine(outputDir, outputName + ".tmp"), StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                var rel = SafePath.NormalizeRelative(full.Substring(sourceFull.Length).TrimStart('\\', '/'));
                SafePath.ValidateEntryName(rel);

                result.Add(new SourceFile
                {
                    RelativePath = rel,
                    FullPath = full,
                    Length = new FileInfo(full).Length,
                });
            }

            // ordinal 排序 → 可重现
            result.Sort((a, b) => string.CompareOrdinal(a.RelativePath, b.RelativePath));
            return result;
        }

        /// <summary>
        /// 收集**真正为空**的目录（既没有文件，也没有子目录）。
        ///
        /// 只记"真空目录"而不是"不含文件的目录"：
        /// 父目录会由子目录的创建隐式带出来，重复记录只会让清单变吵。
        /// 现状的 <c>ZipHelper.ZipSetp</c> 不为目录生成 zip 条目，空目录会被静默丢掉 —— 这里补上。
        /// </summary>
        private static List<string> CollectEmptyDirectories(string sourceFull)
        {
            var empties = new List<string>();

            foreach (var dir in Directory.EnumerateDirectories(sourceFull, "*", SearchOption.AllDirectories))
            {
                if (Directory.GetFileSystemEntries(dir).Length != 0)
                {
                    continue;
                }

                var rel = SafePath.NormalizeRelative(dir.Substring(sourceFull.Length).TrimStart('\\', '/'));
                SafePath.ValidateEntryName(rel);
                empties.Add(rel);
            }

            empties.Sort(string.CompareOrdinal);
            return empties;
        }

        // ─────────────────────────────────────────────────────────────
        // Stage 5
        // ─────────────────────────────────────────────────────────────

        private static async Task WritePayloadAsync(
            Stream output,
            string sourceFull,
            List<SourceFile> files,
            List<string> directories,
            DateTimeOffset timestamp,
            CompressionLevel level,
            IProgress<BuildProgress> progress,
            CancellationToken ct)
        {
            var zipTime = ClampZipTimestamp(timestamp);
            var buffer = new byte[CopyBufferSize];

            // entryNameEncoding = null → 条目名用 UTF-8，非 ASCII 时自动置语言编码标志位。
            // 这正是现状 ZipConstants.cs:434（按区域 ANSI 代码页）那个跨语言乱码问题的根治办法。
            using (var zip = new ZipArchive(output, ZipArchiveMode.Create, true, null))
            {
                for (var i = 0; i < directories.Count; i++)
                {
                    ct.ThrowIfCancellationRequested();
                    var entry = zip.CreateEntry(SafePath.ToZipEntryName(directories[i]) + "/", level);
                    entry.LastWriteTime = zipTime;
                }

                for (var i = 0; i < files.Count; i++)
                {
                    ct.ThrowIfCancellationRequested();
                    var f = files[i];
                    var entry = zip.CreateEntry(SafePath.ToZipEntryName(f.RelativePath), level);
                    entry.LastWriteTime = zipTime;

                    using (var entryStream = entry.Open())
                    using (var input = new FileStream(f.FullPath, FileMode.Open, FileAccess.Read, FileShare.Read, CopyBufferSize))
                    {
                        int read;
                        while ((read = await input.ReadAsync(buffer, 0, buffer.Length, ct).ConfigureAwait(false)) > 0)
                        {
                            await entryStream.WriteAsync(buffer, 0, read, ct).ConfigureAwait(false);
                        }
                    }

                    if (i % 16 == 0 || i == files.Count - 1)
                    {
                        var pct = 35 + (int)(45.0 * (i + 1) / Math.Max(1, files.Count));
                        Report(progress, BuildStage.Compose,
                            string.Format(CultureInfo.InvariantCulture, "压缩 {0}/{1}：{2}", i + 1, files.Count, f.RelativePath),
                            pct);
                    }
                }
            }
        }

        private static DateTimeOffset ClampZipTimestamp(DateTimeOffset ts)
        {
            if (ts < ZipTimeMin)
            {
                return ZipTimeMin;
            }

            if (ts > ZipTimeMax)
            {
                return ZipTimeMax;
            }

            return ts;
        }

        // ─────────────────────────────────────────────────────────────
        // Stage 7：可复用的校验（CLI 的 verify 命令也用它）
        // ─────────────────────────────────────────────────────────────

        /// <summary>校验一个安装包容器：footer、payload 哈希、清单、清单与 payload 的一致性。</summary>
        public static PackageVerifyResult VerifyPackage(string path)
        {
            var r = new PackageVerifyResult();

            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, CopyBufferSize))
            {
                ContainerInfo info;
                try
                {
                    info = ContainerFormat.TryRead(fs);
                }
                catch (Exception ex)
                {
                    r.Messages.Add("读取容器失败：" + ex.Message);
                    return r;
                }

                if (info == null)
                {
                    r.Messages.Add("目标文件不是本程序的安装包容器。");
                    return r;
                }

                r.IsContainer = true;
                r.Info = info;
                r.Messages.Add(string.Format(CultureInfo.InvariantCulture,
                    "容器 v{0}：stub={1:N0}B payload={2:N0}B manifest={3:N0}B 总计={4:N0}B",
                    info.FormatVersion, info.StubLength, info.PayloadLength, info.ManifestLength, info.FileLength));

                try
                {
                    r.PayloadHashOk = ContainerFormat.VerifyPayloadHash(fs, info);
                }
                catch (Exception ex)
                {
                    r.Messages.Add("校验 payload 哈希时出错：" + ex.Message);
                }

                if (!r.PayloadHashOk)
                {
                    r.Messages.Add("payload SHA-256 与 footer 记录不一致 —— 文件被篡改或损坏。");
                }

                try
                {
                    r.StubHashOk = ContainerFormat.VerifyStubHash(fs, info);
                }
                catch (Exception ex)
                {
                    r.Messages.Add("校验 stub 哈希时出错：" + ex.Message);
                }

                if (!r.StubHashOk)
                {
                    r.Messages.Add("stub（安装器代码区）SHA-256 与 footer 记录不一致 —— 文件被篡改或损坏。");
                }

                try
                {
                    r.Manifest = ContainerFormat.ReadManifest(fs, info);
                    r.ManifestOk = true;
                    r.EntryCount = r.Manifest.Files.Count;
                    r.Messages.Add(string.Format(CultureInfo.InvariantCulture,
                        "清单：{0} {1}（{2} 个文件，scope={3}）",
                        r.Manifest.Product.Name.Count > 0 ? FirstValue(r.Manifest.Product.Name) : "?",
                        r.Manifest.Product.Version, r.EntryCount, r.Manifest.Scope));
                }
                catch (Exception ex)
                {
                    r.Messages.Add("解析清单失败：" + ex.Message);
                }

                if (!r.ManifestOk)
                {
                    return r;
                }

                // 清单与 payload 必须一致
                try
                {
                    var present = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                    using (var payload = ContainerFormat.OpenPayload(fs, info))
                    using (var zip = new ZipArchive(payload, ZipArchiveMode.Read))
                    {
                        r.ZipEntryCount = zip.Entries.Count;
                        foreach (var e in zip.Entries)
                        {
                            present.Add(e.FullName.Replace('/', '\\').TrimEnd('\\'));
                        }
                    }

                    foreach (var f in r.Manifest.Files)
                    {
                        if (!present.Contains(f.Path))
                        {
                            r.MissingFiles.Add(f.Path);
                        }
                    }

                    r.AllFilesPresent = r.MissingFiles.Count == 0;
                    if (!r.AllFilesPresent)
                    {
                        r.Messages.Add("清单里有 " + r.MissingFiles.Count + " 个文件在 payload 中找不到。");
                    }
                    else
                    {
                        r.Messages.Add(string.Format(CultureInfo.InvariantCulture,
                            "payload 含 {0} 个 zip 条目，清单里的 {1} 个文件全部命中。",
                            r.ZipEntryCount, r.EntryCount));
                    }
                }
                catch (Exception ex)
                {
                    r.Messages.Add("读取 payload 失败：" + ex.Message);
                }
            }

            return r;
        }

        private static string FirstValue(LocalizedText text)
        {
            foreach (var kv in text)
            {
                return kv.Value;
            }

            return string.Empty;
        }

        private static void Report(IProgress<BuildProgress> progress, BuildStage stage, string message, int percent)
        {
            if (progress != null)
            {
                progress.Report(new BuildProgress(stage, message, percent));
            }
        }
    }
}
