using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using Installer.Abstractions.Model;
using Installer.Abstractions.Packaging;
using Installer.Core.Packaging;
using Installer.Core.Security;

namespace Installer.Cli
{
    /// <summary>
    /// 打包命令行。现状的打包**只能点 GUI**，所以每次发版都是手工操作、无法审计、无法重现。
    /// 本命令让打包进 CI。
    /// </summary>
    internal static class Program
    {
        private static int Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            if (args.Length == 0 || IsHelp(args[0]))
            {
                PrintUsage();
                return args.Length == 0 ? 2 : 0;
            }

            try
            {
                switch (args[0].ToLowerInvariant())
                {
                    case "pack":
                        return Pack(args.Skip(1).ToArray());
                    case "verify":
                        return Verify(args.Skip(1).ToArray());
                    case "inspect":
                        return Inspect(args.Skip(1).ToArray());
                    case "extract":
                        return Extract(args.Skip(1).ToArray());
                    case "diff":
                        return Diff(args.Skip(1).ToArray());
                    case "legacy-inspect":
                        return LegacyInspect(args.Skip(1).ToArray());
                    case "legacy-import":
                        return LegacyImport(args.Skip(1).ToArray());
                    case "version":
                        Console.WriteLine("installer-cli " + PackageBuilder.PackerVersion);
                        return 0;
                    default:
                        Console.Error.WriteLine("未知命令：" + args[0]);
                        PrintUsage();
                        return 2;
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine();
                Console.Error.WriteLine("失败：" + ex.Message);
                if (ex is PackageBuildException || ex is InvalidDataException || ex is NotSupportedException)
                {
                    return 1;
                }

                Console.Error.WriteLine(ex.ToString());
                return 1;
            }
        }

        private static bool IsHelp(string s)
        {
            return s == "-h" || s == "--help" || s == "/?" || s == "help";
        }

        private static void PrintUsage()
        {
            Console.WriteLine(@"installer-cli " + PackageBuilder.PackerVersion + @" —— 安装包打包工具

用法：
  installer-cli pack    --project <app.wmpkg.json> --source <目录> --stub <stub.exe> --out <Setup.exe>
                        [--icon <app.ico>] [--build-time <ISO8601>] [--no-icon]
                        [--sign --signtool <path> [--sha1 <证书指纹>] [--ts <时间戳URL>]]
  installer-cli verify  <Setup.exe>
  installer-cli inspect <Setup.exe>
  installer-cli extract <Setup.exe> --out <目录>
  installer-cli diff    <A.exe> <B.exe>
  installer-cli legacy-inspect <目录或 install.resources>
  installer-cli legacy-import  <目录或 install.resources> --out <app.wmpkg.json> [--source <源目录>]
  installer-cli version

说明：
  --project     工程文件，内容就是 InstallerManifest 的 JSON（files/build 会被打包时覆盖）
  --source      待打包目录，其内容会成为安装后的文件树
  --stub        模板 exe（Installer.Runtime.exe），将被放在产物最前面
  --out         产物路径（单文件 exe）
  --icon        写入产物的图标；省略则保留 stub 自带图标
  --build-time  构建时间戳（ISO-8601）。**固定它 + 固定输入 = 可重现的字节**
  --sign        打包后调用 signtool 签名（必须在图标补丁之后）

退出码：0 成功 / 1 失败 / 2 用法错误");
        }

        // ─────────────────────────────────────────────────────────────
        // pack
        // ─────────────────────────────────────────────────────────────

        private static int Pack(string[] args)
        {
            var map = ParseArgs(args, "project", "source", "stub", "out", "icon", "build-time",
                "signtool", "sha1", "ts");
            var flags = ParseFlags(args, "no-icon", "sign");

            var projectPath = Require(map, "project");

            if (!File.Exists(projectPath))
            {
                throw new PackageBuildException("工程文件不存在：" + projectPath);
            }

            // 工程文件支持两种形状：裸清单 / { manifest, build }
            var project = ProjectSerializer.Load(projectPath);
            var manifest = project.Manifest;
            var build = project.Build ?? new ProjectBuildSettings();

            // 命令行优先，其次工程文件里的 build 段
            var sourceDir = Pick(map, "source", build.SourceDir);
            var stubPath = Pick(map, "stub", build.StubPath);
            var outPath = Pick(map, "out", build.OutputPath);
            var iconPath = Pick(map, "icon", build.IconPath);

            if (string.IsNullOrWhiteSpace(sourceDir))
            {
                throw new PackageBuildException("缺少源目录：请给 --source，或在工程文件的 build.sourceDir 里指定。");
            }

            if (string.IsNullOrWhiteSpace(stubPath))
            {
                throw new PackageBuildException("缺少 stub：请给 --stub，或在工程文件的 build.stubPath 里指定。");
            }

            if (string.IsNullOrWhiteSpace(outPath))
            {
                throw new PackageBuildException("缺少输出路径：请给 --out，或在工程文件的 build.outputPath 里指定。");
            }

            // 工程里勾了要注册的 COM 文件 → 写进清单
            if (build.ComFiles != null && build.ComFiles.Count > 0)
            {
                manifest.Com = new System.Collections.Generic.List<ComSpec>();
                foreach (var f in build.ComFiles)
                {
                    manifest.Com.Add(new ComSpec { Path = f, Mode = ComRegistrationModes.RegistrationFree });
                }
            }

            DateTimeOffset? buildTime = null;
            string bt;
            if (map.TryGetValue("build-time", out bt) && !string.IsNullOrWhiteSpace(bt))
            {
                buildTime = DateTimeOffset.Parse(bt, CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);
            }
            else if (!string.IsNullOrWhiteSpace(build.Timestamp))
            {
                buildTime = DateTimeOffset.Parse(build.Timestamp, CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);
            }
            else if (manifest.Build != null && !string.IsNullOrWhiteSpace(manifest.Build.BuiltAtUtc))
            {
                buildTime = DateTimeOffset.Parse(manifest.Build.BuiltAtUtc, CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);
            }

            var options = new PackageBuildOptions
            {
                Manifest = manifest,
                SourceDir = sourceDir,
                StubPath = stubPath,
                OutputPath = outPath,
                IconPath = iconPath,
                SkipIconPatch = flags.Contains("no-icon") || string.IsNullOrWhiteSpace(iconPath),
                BuildTimestamp = buildTime,
                PackerVersion = PackageBuilder.PackerVersion,
            };

            if (flags.Contains("sign"))
            {
                options.SignToolPath = map.ContainsKey("signtool") ? map["signtool"] : SignToolRunner.FindSignTool();
                options.SignCertificateThumbprint = map.ContainsKey("sha1") ? map["sha1"] : null;
                options.SignTimestampUrl = map.ContainsKey("ts") ? map["ts"] : null;
            }

            Console.WriteLine("打包中…");
            var progress = new Progress<BuildProgress>(p =>
            {
                Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "  [{0,3}%] {1,-8} {2}",
                    p.Percent, p.Stage, p.Message));
            });

            var result = new PackageBuilder().Build(options, progress);

            Console.WriteLine();
            Console.WriteLine("完成。");
            Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "  产物      : {0}", result.OutputPath));
            Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "  大小      : {0:N0} 字节（stub {1:N0} + payload {2:N0} + manifest {3:N0} + footer {4}）",
                result.OutputBytes, result.StubBytes, result.PayloadBytes, result.ManifestBytes, ContainerFormatSpec.FooterSize));
            Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "  文件条目  : {0}", result.EntryCount));
            Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "  图标补丁  : {0}", result.IconPatched ? "已应用" : "未应用"));
            Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "  构建时间  : {0:o}", result.BuildTimestamp));
            Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "  payload哈希: {0}", result.PayloadSha256));
            Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "  产物哈希  : {0}", result.OutputSha256));
            return 0;
        }

        // ─────────────────────────────────────────────────────────────
        // verify / inspect
        // ─────────────────────────────────────────────────────────────

        private static int Verify(string[] args)
        {
            if (args.Length < 1)
            {
                throw new PackageBuildException("用法：installer-cli verify <Setup.exe>");
            }

            var r = PackageBuilder.VerifyPackage(args[0]);
            foreach (var m in r.Messages)
            {
                Console.WriteLine("  " + m);
            }

            Console.WriteLine();
            Console.WriteLine(r.Ok ? "校验通过。" : "校验未通过。");
            return r.Ok ? 0 : 1;
        }

        private static int Inspect(string[] args)
        {
            if (args.Length < 1)
            {
                throw new PackageBuildException("用法：installer-cli inspect <Setup.exe>");
            }

            using (var fs = File.OpenRead(args[0]))
            {
                var info = ContainerFormat.TryRead(fs);
                if (info == null)
                {
                    Console.Error.WriteLine("不是本程序的安装包容器。");
                    return 1;
                }

                Console.WriteLine("容器");
                Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "  formatVersion : {0}", info.FormatVersion));
                Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "  flags         : {0}", info.Flags));
                Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "  fileLength    : {0:N0}", info.FileLength));
                Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "  stubLength    : {0:N0}", info.StubLength));
                Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "  payload       : offset={0:N0} length={1:N0}", info.PayloadOffset, info.PayloadLength));
                Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "  manifest      : offset={0:N0} length={1:N0}", info.ManifestOffset, info.ManifestLength));
                Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "  payloadSha256 : {0}", Core.Common.Hashing.ToHex(info.PayloadSha256)));
                Console.WriteLine();
                Console.WriteLine("清单 (manifest.json)");
                Console.WriteLine(ContainerFormat.ReadManifestJson(fs, info));
            }

            return 0;
        }

        // ─────────────────────────────────────────────────────────────
        // extract
        // ─────────────────────────────────────────────────────────────

        private static int Extract(string[] args)
        {
            var map = ParseArgs(args, "out");
            if (args.Length < 1)
            {
                throw new PackageBuildException("用法：installer-cli extract <Setup.exe> --out <目录>");
            }

            var package = args[0];
            var outDir = Require(map, "out");
            Directory.CreateDirectory(outDir);

            using (var fs = File.OpenRead(package))
            {
                var info = ContainerFormat.Read(fs);

                // manifest.json
                var manifestJson = ContainerFormat.ReadManifestJson(fs, info);
                File.WriteAllText(Path.Combine(outDir, "manifest.json"), manifestJson, new UTF8Encoding(false));

                // payload
                var payloadDir = Path.Combine(outDir, "payload");
                Directory.CreateDirectory(payloadDir);

                var count = 0;
                using (var payload = ContainerFormat.OpenPayload(fs, info))
                using (var zip = new ZipArchive(payload, ZipArchiveMode.Read))
                {
                    foreach (var entry in zip.Entries)
                    {
                        var name = entry.FullName;
                        if (name.EndsWith("/", StringComparison.Ordinal))
                        {
                            Directory.CreateDirectory(SafePath.ResolveWithin(payloadDir, name.TrimEnd('/')));
                            continue;
                        }

                        // 所有落盘路径都走 SafePath —— Zip Slip 在这里被挡住
                        var target = SafePath.ResolveWithin(payloadDir, name);
                        var dir = Path.GetDirectoryName(target);
                        if (!string.IsNullOrEmpty(dir))
                        {
                            Directory.CreateDirectory(dir);
                        }

                        using (var src = entry.Open())
                        using (var dst = File.Create(target))
                        {
                            src.CopyTo(dst);
                        }

                        count++;
                    }
                }

                Console.WriteLine(string.Format(CultureInfo.InvariantCulture,
                    "已解出 manifest.json 与 {0} 个文件到 {1}", count, Path.GetFullPath(outDir)));
            }

            return 0;
        }

        // ─────────────────────────────────────────────────────────────
        // diff
        // ─────────────────────────────────────────────────────────────

        private static int Diff(string[] args)
        {
            if (args.Length < 2)
            {
                throw new PackageBuildException("用法：installer-cli diff <A.exe> <B.exe>");
            }

            var a = LoadManifestFromPackage(args[0]);
            var b = LoadManifestFromPackage(args[1]);

            var am = a.Files.ToDictionary(f => f.Path, f => f, StringComparer.OrdinalIgnoreCase);
            var bm = b.Files.ToDictionary(f => f.Path, f => f, StringComparer.OrdinalIgnoreCase);

            var added = bm.Keys.Where(k => !am.ContainsKey(k)).OrderBy(k => k, StringComparer.Ordinal).ToList();
            var removed = am.Keys.Where(k => !bm.ContainsKey(k)).OrderBy(k => k, StringComparer.Ordinal).ToList();
            var changed = am.Keys
                .Where(k => bm.ContainsKey(k) && !string.Equals(am[k].Sha256, bm[k].Sha256, StringComparison.OrdinalIgnoreCase))
                .OrderBy(k => k, StringComparer.Ordinal)
                .ToList();

            Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "A: {0}  版本 {1}  {2} 个文件  构建 {3}",
                Path.GetFileName(args[0]), a.Product.Version, a.Files.Count,
                a.Build != null ? a.Build.BuiltAtUtc : "?"));
            Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "B: {0}  版本 {1}  {2} 个文件  构建 {3}",
                Path.GetFileName(args[1]), b.Product.Version, b.Files.Count,
                b.Build != null ? b.Build.BuiltAtUtc : "?"));
            Console.WriteLine();

            PrintGroup("新增 (+)", added);
            PrintGroup("删除 (-)", removed);
            PrintGroup("变更 (~)", changed);

            var unchanged = am.Keys.Count(k => bm.ContainsKey(k) && !changed.Contains(k));
            Console.WriteLine();
            Console.WriteLine(string.Format(CultureInfo.InvariantCulture,
                "合计：新增 {0}，删除 {1}，变更 {2}，未变 {3}",
                added.Count, removed.Count, changed.Count, unchanged));

            var fileDiff = added.Count + removed.Count + changed.Count;

            // 文件清单完全一致时，把元数据差异作为**信息**打出来，但不当作差异。
            // 理由：差分更新只关心文件；改一个构建时间戳不该让 CI 判定"包变了"。
            if (fileDiff == 0)
            {
                var metaDiff = new List<string>();
                if (a.Product.Version != b.Product.Version)
                {
                    metaDiff.Add("product.version " + a.Product.Version + " → " + b.Product.Version);
                }

                if (a.ProductCode != b.ProductCode)
                {
                    metaDiff.Add("productCode " + a.ProductCode + " → " + b.ProductCode);
                }

                var ta = a.Build != null ? a.Build.BuiltAtUtc : null;
                var tb = b.Build != null ? b.Build.BuiltAtUtc : null;
                if (ta != tb)
                {
                    metaDiff.Add("build.builtAtUtc " + ta + " → " + tb);
                }

                if (metaDiff.Count == 0)
                {
                    Console.WriteLine("两个包的文件清单与元数据都相同。");
                }
                else
                {
                    Console.WriteLine("文件清单相同；仅元数据不同（不计为差异）：");
                    foreach (var m in metaDiff)
                    {
                        Console.WriteLine("    " + m);
                    }
                }
            }

            return fileDiff == 0 ? 0 : 1;
        }

        private static void PrintGroup(string title, List<string> items)
        {
            Console.WriteLine(title + " " + items.Count);
            foreach (var i in items)
            {
                Console.WriteLine("    " + i);
            }
        }

        private static InstallerManifest LoadManifestFromPackage(string path)
        {
            using (var fs = File.OpenRead(path))
            {
                var info = ContainerFormat.Read(fs);
                return ContainerFormat.ReadManifest(fs, info);
            }
        }

        // ─────────────────────────────────────────────────────────────
        // legacy（v1 旧包）
        // ─────────────────────────────────────────────────────────────

        private static int LegacyInspect(string[] args)
        {
            if (args.Length < 1)
            {
                throw new PackageBuildException("用法：installer-cli legacy-inspect <目录或 install.resources>");
            }

            var pkg = LegacyV1Reader.Read(args[0]);

            Console.WriteLine("v1 包：" + pkg.ResourcesPath);
            Console.WriteLine();
            Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "  产品名     : {0}", pkg.ProductName));
            Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "  公司       : {0}", pkg.CompanyName));
            Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "  版本       : {0}", pkg.ProductVersion));
            Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "  可执行文件 : {0}", pkg.ProductExeName));
            Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "  网址       : {0}", pkg.CompanyUrl));
            Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "  默认安装到 : {0}", pkg.InstallationPath));
            Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "  需注册文件 : {0}",
                pkg.RegditFiles.Count == 0 ? "（无）" : string.Join(", ", pkg.RegditFiles.ToArray())));
            Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "  负载       : {0}",
                pkg.PayloadZip == null ? "（无）" : pkg.PayloadZip.Length.ToString("N0", CultureInfo.InvariantCulture) + " 字节"));

            var entries = LegacyV1Reader.ListPayloadEntries(pkg);
            if (entries.Count > 0)
            {
                Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "  负载条目   : {0} 个", entries.Count));
                foreach (var e in entries.Take(10))
                {
                    Console.WriteLine("      " + e);
                }

                if (entries.Count > 10)
                {
                    Console.WriteLine("      …（其余 " + (entries.Count - 10) + " 个）");
                }
            }

            Console.WriteLine();
            Console.WriteLine("原始键：");
            foreach (var kv in pkg.RawKeys.OrderBy(k => k.Key, StringComparer.Ordinal))
            {
                Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "  {0,-24} = {1}", kv.Key, kv.Value));
            }

            foreach (var w in pkg.Warnings)
            {
                Console.WriteLine();
                Console.WriteLine("⚠ " + w);
            }

            return 0;
        }

        private static int LegacyImport(string[] args)
        {
            var map = ParseArgs(args, "out", "source");
            if (args.Length < 1)
            {
                throw new PackageBuildException(
                    "用法：installer-cli legacy-import <目录或 install.resources> --out <app.wmpkg.json> [--source <源目录>]");
            }

            var outPath = Require(map, "out");
            var sourceDir = map.ContainsKey("source") ? map["source"] : null;

            var pkg = LegacyV1Reader.Read(args[0]);
            var project = LegacyV1Reader.ToProject(pkg, sourceDir);

            ProjectSerializer.Save(outPath, project);

            Console.WriteLine("已把 v1 包转换成新工程：" + Path.GetFullPath(outPath));
            Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "  产品名 : {0}", project.Manifest.Product.Name["zh-Hans"]));
            Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "  版本   : {0}", project.Manifest.Product.Version));
            Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "  productCode : {0}（**新生成**，与旧包无关）",
                project.Manifest.ProductCode));

            if (string.IsNullOrWhiteSpace(sourceDir))
            {
                Console.WriteLine();
                Console.WriteLine("⚠ 没有给 --source：新工程还没有文件清单，请用制作端指定待打包目录后重新打包。");
            }

            foreach (var w in pkg.Warnings)
            {
                Console.WriteLine("⚠ " + w);
            }

            return 0;
        }

        // ─────────────────────────────────────────────────────────────
        // 参数解析
        // ─────────────────────────────────────────────────────────────

        private static Dictionary<string, string> ParseArgs(string[] args, params string[] known)
        {
            var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            for (var i = 0; i < args.Length; i++)
            {
                var a = args[i];
                if (!a.StartsWith("--", StringComparison.Ordinal))
                {
                    continue;
                }

                var key = a.Substring(2);
                if (Array.IndexOf(known, key) < 0)
                {
                    continue;
                }

                if (i + 1 < args.Length && !args[i + 1].StartsWith("--", StringComparison.Ordinal))
                {
                    map[key] = args[i + 1];
                    i++;
                }
                else
                {
                    map[key] = "";
                }
            }

            return map;
        }

        private static HashSet<string> ParseFlags(string[] args, params string[] known)
        {
            var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var a in args)
            {
                if (!a.StartsWith("--", StringComparison.Ordinal))
                {
                    continue;
                }

                var key = a.Substring(2);
                if (Array.IndexOf(known, key) >= 0)
                {
                    set.Add(key);
                }
            }

            return set;
        }

        private static string Require(Dictionary<string, string> map, string key)
        {
            string v;
            if (!map.TryGetValue(key, out v) || string.IsNullOrWhiteSpace(v))
            {
                throw new PackageBuildException("缺少必填参数 --" + key);
            }

            return v;
        }

        /// <summary>命令行优先，其次工程文件里的值。</summary>
        private static string Pick(Dictionary<string, string> map, string key, string fallback)
        {
            string v;
            if (map.TryGetValue(key, out v) && !string.IsNullOrWhiteSpace(v))
            {
                return v;
            }

            return string.IsNullOrWhiteSpace(fallback) ? null : fallback;
        }
    }
}
