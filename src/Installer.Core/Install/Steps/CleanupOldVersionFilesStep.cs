using System;
using System.Collections.Generic;
using System.IO;
using Installer.Abstractions.Install;
using Installer.Abstractions.Model;
using Installer.Core.Security;
using Installer.Core.Uninstall;

namespace Installer.Core.Install
{
    /// <summary>
    /// 升级安装时，删掉"上一版装过、这一版没有了"的文件。
    ///
    /// **为什么必须有**：同一个 productCode 覆盖安装时，旧版本独有的文件不会自动消失。
    /// 实测（v1 有 <c>V1Only.dll</c>、v2 里删掉它）：升级完安装目录里
    /// <c>V1Only.dll</c> 还在。这类残留会让程序在运行时加载到**旧 DLL**，
    /// 症状是"装完第二版反而更坏"——而且极难排查，因为文件确实"装上了"。
    ///
    /// **安全边界**（这条步骤会删文件，所以边界必须说死）：
    /// ① 只删上一版 <c>.install-record.json</c> 里**明确列出**的文件；
    /// ② 只删**安装目录内**的（走 <see cref="SafePath.ResolveWithin"/>，越界一律跳过）；
    /// ③ 用户自己往安装目录里放的文件不在记录里，不会被碰；
    /// ④ 删除前先备份进 <c>.rollback</c> 并写日志 —— 安装失败回滚时原样还原；
    /// ⑤ 删不掉（被占用）只记警告、跳过，绝不让安装失败。
    /// </summary>
    public sealed class CleanupOldVersionFilesStep : IInstallStep
    {
        /// <inheritdoc />
        public string Id
        {
            get { return "cleanup-old-files"; }
        }

        /// <inheritdoc />
        public string MessageKey
        {
            get { return "Install.CleanupOldFiles"; }
        }

        /// <inheritdoc />
        public bool NeedsPayload
        {
            get { return false; }
        }

        /// <inheritdoc />
        public void Execute(InstallContext ctx)
        {
            if (ctx.DryRun)
            {
                return;
            }

            var record = UninstallEngine.LoadRecord(ctx.InstallDir);
            if (record == null || record.Files == null || record.Files.Count == 0)
            {
                // 首次安装，没有"上一版"可言
                return;
            }

            var keep = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (ctx.Manifest.Files != null)
            {
                foreach (var f in ctx.Manifest.Files)
                {
                    if (!string.IsNullOrEmpty(f.Path))
                    {
                        keep.Add(Normalize(f.Path));
                    }
                }
            }

            var removed = 0;

            foreach (var relative in record.Files)
            {
                var key = Normalize(relative);
                if (string.IsNullOrEmpty(key) || keep.Contains(key))
                {
                    continue;
                }

                string full;
                try
                {
                    full = SafePath.ResolveWithin(ctx.InstallDir, relative);
                }
                catch (Exception)
                {
                    // 越界、非法字符……一律不碰
                    ctx.Log.Warn("跳过可疑的旧文件路径：" + relative);
                    continue;
                }

                if (!File.Exists(full))
                {
                    continue;
                }

                try
                {
                    // 先备份再记日志：回滚时 ExistedBefore=true 会从 BackupPath 还原
                    var backup = ctx.Journal.BackupFile(full);

                    ctx.Journal.Record(new JournalEntry
                    {
                        StepId = Id,
                        Op = JournalOps.WriteFile,
                        Target = full,
                        ExistedBefore = true,
                        BackupPath = backup,
                    });

                    File.Delete(full);
                    removed++;
                    ctx.Log.Info("清理上一版残留文件：" + relative);
                }
                catch (Exception ex)
                {
                    // 删不掉（比如被某个进程占用）不该让安装失败 ——
                    // 留着最多是多一个旧文件，装不上是硬伤。
                    ctx.Log.Warn("清理残留文件失败（已跳过）：" + relative + " —— " + ex.Message);
                }
            }

            if (removed > 0)
            {
                ctx.Log.Info("共清理 " + removed + " 个上一版残留文件");
            }
        }

        /// <summary>路径比较前先统一分隔符，避免 <c>a/b.dll</c> 与 <c>a\b.dll</c> 被当成两个文件。</summary>
        private static string Normalize(string path)
        {
            return string.IsNullOrEmpty(path)
                ? path
                : path.Replace('/', '\\').TrimStart('\\');
        }
    }
}
