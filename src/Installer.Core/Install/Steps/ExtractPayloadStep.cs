using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using Installer.Abstractions.Install;
using Installer.Abstractions.Model;
using Installer.Abstractions.Platform;
using Installer.Core.Common;
using Installer.Core.Packaging;
using Installer.Core.Platform;
using Installer.Core.Security;
using Newtonsoft.Json;

namespace Installer.Core.Install
{
    // ─────────────────────────────────────────────────────────────────

    /// <summary>
    /// 解压 payload 到安装目录。**所有落盘路径都走 <see cref="SafePath"/>** —— 这是 Zip Slip 的唯一防线。
    /// </summary>
    public sealed class ExtractPayloadStep : IInstallStep
    {
        /// <inheritdoc />
        public string Id
        {
            get { return "extract"; }
        }

        /// <inheritdoc />
        public string MessageKey
        {
            get { return "Install.Extracting"; }
        }

        /// <inheritdoc />
        public bool NeedsPayload
        {
            get { return true; }
        }

        /// <inheritdoc />
        public void Execute(InstallContext ctx)
        {
            if (ctx.Payload == null)
            {
                throw new InstallException(Id, "缺少 payload。");
            }

            var extracted = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            // 修复模式：内容与清单哈希一致的文件直接跳过，只补缺失/损坏的
            Dictionary<string, string> expectedHashes = null;
            var repaired = 0;
            if (ctx.Mode == InstallMode.Repair)
            {
                expectedHashes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                foreach (var f in ctx.Manifest.Files)
                {
                    if (!string.IsNullOrEmpty(f.Sha256))
                    {
                        expectedHashes[f.Path] = f.Sha256;
                    }
                }
            }

            using (var zip = new ZipArchive(ctx.Payload, ZipArchiveMode.Read, true))
            {
                // 逐文件进度：界面上的「已解压文件 12 / 199」「正在写入 bin\App.exe」靠它。
                var totalFiles = zip.Entries.Count(e => !e.FullName.EndsWith("/", StringComparison.Ordinal));
                long totalBytes = zip.Entries
                    .Where(e => !e.FullName.EndsWith("/", StringComparison.Ordinal))
                    .Sum(e => e.Length);
                var doneFiles = 0;
                long doneBytes = 0;

                foreach (var entry in zip.Entries)
                {
                    ctx.CancellationToken.ThrowIfCancellationRequested();

                    var name = entry.FullName;

                    // 目录条目
                    if (name.EndsWith("/", StringComparison.Ordinal))
                    {
                        var dirRel = SafePath.NormalizeRelative(name.TrimEnd('/'));
                        if (dirRel.Length == 0)
                        {
                            continue;
                        }

                        var dir = SafePath.ResolveWithin(ctx.InstallDir, dirRel);
                        JournalWriter.EnsureDirectory(ctx, Id, dir, null);
                        continue;
                    }

                    var rel = SafePath.NormalizeRelative(name);
                    var target = SafePath.ResolveWithin(ctx.InstallDir, rel);

                    // 修复模式：文件还在且哈希一致 → 不动它
                    if (expectedHashes != null && File.Exists(target))
                    {
                        string expected;
                        if (expectedHashes.TryGetValue(rel, out expected)
                            && string.Equals(Hashing.ComputeFile(target), expected, StringComparison.OrdinalIgnoreCase))
                        {
                            extracted.Add(rel);
                            ctx.InstalledFiles.Add(rel);
                            continue;
                        }

                        repaired++;
                        ctx.Log.Info("修复：文件缺失或已损坏，将重新写入 —— " + rel);
                    }

                    var parent = Path.GetDirectoryName(target);
                    if (!string.IsNullOrEmpty(parent))
                    {
                        JournalWriter.EnsureDirectory(ctx, Id, parent, null);
                    }

                    if (ctx.DryRun)
                    {
                        extracted.Add(rel);
                        continue;
                    }

                    JournalWriter.WriteFile(ctx, Id, target, p =>
                    {
                        using (var src = entry.Open())
                        using (var dst = File.Create(p))
                        {
                            src.CopyTo(dst);
                        }
                    });

                    extracted.Add(rel);
                    ctx.InstalledFiles.Add(rel);

                    doneFiles++;
                    doneBytes += entry.Length;
                    ReportFile(ctx, rel, doneFiles, totalFiles, doneBytes, totalBytes);
                }
            }

            // 清单里的文件必须全部落到磁盘 —— 现状的 catch{} 会让"安装成功但文件缺失"成为可能
            var missing = ctx.Manifest.Files
                .Where(f => !extracted.Contains(f.Path))
                .Select(f => f.Path)
                .ToList();

            if (missing.Count > 0)
            {
                throw new InstallException(Id, "payload 中缺少 " + missing.Count + " 个清单声明的文件，例如：" +
                                               string.Join(", ", missing.Take(5).ToArray()));
            }

            ctx.Log.Info("已解压 " + extracted.Count + " 个条目。");
        }

        /// <summary>
        /// 上报"正在写哪个文件"。
        ///
        /// <c>Percent</c> 故意给 -1：进度条按**步骤**推进（14 步各占一段），
        /// 这里再塞一个 0–100 会把进度条拽回去。界面用
        /// <c>StepIndex / FilesDone / BytesTotal</c> 自己算条内进度。
        /// </summary>
        private static void ReportFile(InstallContext ctx, string rel, int doneFiles, int totalFiles,
                                       long doneBytes, long totalBytes)
        {
            var progress = ctx.Progress;
            if (progress == null)
            {
                return;
            }

            progress.Report(new InstallProgress
            {
                StepId = "extract",
                Message = "正在解压 " + rel,
                CurrentFile = rel,
                FilesDone = doneFiles,
                FilesTotal = totalFiles,
                BytesDone = doneBytes,
                BytesTotal = totalBytes,
                Percent = -1,
            });
        }
    }

    // ─────────────────────────────────────────────────────────────────
    // 3.5 COM 注册
}
