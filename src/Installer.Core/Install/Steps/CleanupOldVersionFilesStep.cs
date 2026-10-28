using System;
using System.Collections.Generic;
using System.IO;
using Installer.Abstractions.Install;
using Installer.Abstractions.Model;
using Installer.Core.Common;
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
    /// ④ **只删哈希仍与安装时一致的**：文件被改动过就说明里面可能已经是用户数据
    ///    （典型：v1 带的种子数据库 <c>Data\app.db</c>，v2 不再带它，但里面早就是真实数据），
    ///    这种情况一律保留；
    /// ⑤ 删除前先备份进 <c>.rollback</c> 并写日志 —— 安装失败回滚时原样还原；
    /// ⑥ 删不掉（被占用）只记警告、跳过，绝不让安装失败。
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

                // **最后一道闸**：只删"还是我们装进去的那份"。
                //
                // 安装记录里有每个文件的 SHA-256。如果用户（或程序运行时）改过它，
                // 说明里面可能已经是用户数据 —— 绝不能删。
                // 典型场景：v1 的 payload 带一个种子数据库 Data\app.db，
                // v2 不再带它；此时 app.db 里早就是用户的真实数据了。
                var expected = GetExpectedHash(record, key);
                if (!string.IsNullOrEmpty(expected))
                {
                    string actual;
                    try
                    {
                        actual = Hashing.ComputeFile(full);
                    }
                    catch (Exception ex)
                    {
                        ctx.Log.Warn("读不出旧文件哈希，跳过清理：" + relative + " —— " + ex.Message);
                        continue;
                    }

                    if (!string.Equals(actual, expected, StringComparison.OrdinalIgnoreCase))
                    {
                        ctx.Log.Warn("上一版文件已被改动过（可能是用户数据），保留不动：" + relative);
                        continue;
                    }
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

                    StripReadOnly(full);
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

        /// <summary>
        /// 从安装记录里取该文件安装时的 SHA-256。
        /// 取不到（老记录没写 hashes）时返回 null —— 调用方按"没有依据"处理。
        /// </summary>
        private static string GetExpectedHash(InstallRecord record, string normalizedRelative)
        {
            if (record.FileHashes == null || record.FileHashes.Count == 0)
            {
                return null;
            }

            string value;
            if (record.FileHashes.TryGetValue(normalizedRelative, out value))
            {
                return value;
            }

            // 记录里可能用的是正斜杠或不同大小写，宽松再试一遍
            foreach (var kv in record.FileHashes)
            {
                if (string.Equals(Normalize(kv.Key), normalizedRelative, StringComparison.OrdinalIgnoreCase))
                {
                    return kv.Value;
                }
            }

            return null;
        }

        /// <summary>路径比较前先统一分隔符，避免 <c>a/b.dll</c> 与 <c>a\b.dll</c> 被当成两个文件。</summary>
        private static string Normalize(string path)
        {
            return string.IsNullOrEmpty(path)
                ? path
                : path.Replace('/', '\\').TrimStart('\\');
        }

        private static void StripReadOnly(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    var attrs = File.GetAttributes(path);
                    if ((attrs & FileAttributes.ReadOnly) != 0)
                    {
                        File.SetAttributes(path, attrs & ~FileAttributes.ReadOnly);
                    }
                }
            }
            catch
            {
            }
        }
    }
}
