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
    /// 安装前预检：路径合法性、磁盘空间、是否已装更高版本。
    /// 现状**完全没有**这些检查（无磁盘空间检查，版本比较用 <c>Int32.Parse(v.Replace(".",""))</c>）。
    /// </summary>
    public sealed class PreflightStep : IInstallStep
    {
        /// <inheritdoc />
        public string Id
        {
            get { return "preflight"; }
        }

        /// <inheritdoc />
        public string MessageKey
        {
            get { return "Install.Preflight"; }
        }

        /// <inheritdoc />
        public bool NeedsPayload
        {
            get { return false; }
        }

        /// <inheritdoc />
        public void Execute(InstallContext ctx)
        {
            // 第一件事：看看上一次安装是不是被中断了。
            // 必须放在最前面 —— 后面的检查都可能被残留状态干扰。
            RecoverInterruptedInstall(ctx);

            var dir = Path.GetFullPath(ctx.InstallDir);

            // 不允许往盘根、Windows 目录、System32 里装
            var root = Path.GetPathRoot(dir);
            if (string.Equals(dir.TrimEnd('\\'), root.TrimEnd('\\'), StringComparison.OrdinalIgnoreCase))
            {
                throw new InstallException(Id, "安装目录不能是驱动器根目录：" + dir);
            }

            // 盘符都不存在：要么是外置盘没插，要么是制作机上配的路径照搬过来了。
            // 必须在这里说清楚 —— 否则会一路失败到"解压"那一步，报一句看不懂的系统错误。
            if (!string.IsNullOrEmpty(root) && !Directory.Exists(root))
            {
                throw new InstallException(Id,
                    "安装位置所在的磁盘不存在：" + root + "\r\n请换一个安装位置后重试。");
            }

            var windows = ctx.Platform.Environment.GetFolder(SpecialFolderKind.System);
            if (!string.IsNullOrEmpty(windows) && SafePath.IsWithin(windows, dir))
            {
                throw new InstallException(Id, "安装目录不能位于系统目录内：" + dir);
            }

            // 磁盘空间：解压后 + payload 缓存，按 2 倍文件总量估算
            var need = 0L;
            foreach (var f in ctx.Manifest.Files)
            {
                need += f.Size;
            }

            need = need * 2 + (16L * 1024 * 1024);
            var free = ctx.Platform.Environment.GetAvailableFreeSpace(Path.GetPathRoot(dir) ?? dir);
            if (free >= 0 && free < need)
            {
                throw new InstallException(Id, string.Format(CultureInfo.InvariantCulture,
                    "磁盘空间不足：需要约 {0:N0} 字节，可用 {1:N0} 字节。", need, free));
            }

            ctx.Log.Info(string.Format(CultureInfo.InvariantCulture,
                "预检通过：目标目录 {0}，需要约 {1:N0} 字节，可用 {2:N0} 字节。", dir, need, free));

            // 是否已安装（读旧记录做版本比较，用 System.Version 而不是字符串拼接）
            var recordPath = InstallRecord.GetPath(dir);
            if (File.Exists(recordPath))
            {
                InstallRecord old = null;
                try
                {
                    old = JsonConvert.DeserializeObject<InstallRecord>(File.ReadAllText(recordPath, Encoding.UTF8));
                }
                catch (JsonException ex)
                {
                    ctx.Log.Warn("旧的安装记录无法解析，将按全新安装处理：" + ex.Message);
                }

                if (old != null && !string.IsNullOrEmpty(old.ProductVersion))
                {
                    var cmp = CompareVersions(old.ProductVersion, ctx.Manifest.Product.Version);
                    if (cmp > 0 && ctx.Mode == InstallMode.Install && !ctx.AllowDowngrade)
                    {
                        throw new InstallException(Id, string.Format(CultureInfo.InvariantCulture,
                            "已安装更高版本（{0} > {1}）。如需降级请先卸载。",
                            old.ProductVersion, ctx.Manifest.Product.Version));
                    }

                    ctx.Log.Info("检测到已安装版本 " + old.ProductVersion + "，本次 " +
                                 ctx.Manifest.Product.Version + "（比较结果 " + cmp + "）。");
                }
            }

            // 建目录（journal 会记录，失败时撤销）
            JournalWriter.EnsureDirectory(ctx, Id, dir, null);
        }

        /// <summary>
        /// 回滚**上一次被中断**的安装。
        ///
        /// 为什么需要：写前日志 + 逆序 undo 只在「本次安装中途失败」时生效。
        /// 如果进程被杀、断电、或用户在任务管理器里结束进程，日志文件与
        /// <c>.rollback</c> 备份目录就留在磁盘上没人管 —— 已经写进去的文件 /
        /// 注册表值 / 快捷方式成了孤儿，用户下次装同一个产品还会被这些残留干扰。
        ///
        /// 这里在安装开始前把上一次的账结掉，复用的是 <see cref="InstallEngine.Undo"/>，
        /// 所以「中断恢复」与「失败回滚」的行为完全一致。
        /// </summary>
        private void RecoverInterruptedInstall(InstallContext ctx)
        {
            if (ctx.Journal == null)
            {
                return;
            }

            var journalPath = ctx.Journal.Path;
            if (string.IsNullOrEmpty(journalPath) || !File.Exists(journalPath))
            {
                return;   // 没有残留 —— 绝大多数安装都走这一条
            }

            List<JournalEntry> stale;
            try
            {
                stale = InstallJournal.LoadPending(journalPath);
            }
            catch (IOException ex)
            {
                ctx.Log.Warn("发现残留的安装日志但读不出来，已忽略：" + ex.Message);
                return;
            }

            if (stale == null || stale.Count == 0)
            {
                // 日志在、内容空 —— 说明第一条记录写下去之前就被杀了，直接删掉
                TryDeleteFile(journalPath, ctx);
                ctx.Log.Info("清理了一个空的残留安装日志。");
                return;
            }

            ctx.Log.Warn(string.Format(CultureInfo.InvariantCulture,
                "检测到上次安装被中断，正在按日志回滚 {0} 条记录……", stale.Count));

            var undone = 0;
            var failed = 0;
            for (var i = stale.Count - 1; i >= 0; i--)
            {
                try
                {
                    InstallEngine.Undo(ctx, stale[i]);
                    undone++;
                }
                catch (Exception ex)
                {
                    // 单条撤销失败**不该**让本次安装失败 ——
                    // 用户现在是想装，不是想回滚。记下来继续。
                    failed++;
                    ctx.Log.Warn("回滚残留记录失败（" + stale[i].Op + " " + stale[i].Target + "）：" + ex.Message);
                }
            }

            ctx.Log.Info(string.Format(CultureInfo.InvariantCulture,
                "中断安装回滚完成：成功 {0} 条，失败 {1} 条。", undone, failed));

            // 账结完了就把日志与备份目录删掉，否则下次安装还会再见到它们
            TryDeleteFile(journalPath, ctx);
            TryDeleteDirectory(ctx.Journal.BackupDir, ctx);
        }

        private static void TryDeleteFile(string path, InstallContext ctx)
        {
            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
            catch (IOException ex)
            {
                ctx.Log.Warn("删除残留日志失败：" + ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                ctx.Log.Warn("删除残留日志失败：" + ex.Message);
            }
        }

        private static void TryDeleteDirectory(string path, InstallContext ctx)
        {
            if (string.IsNullOrEmpty(path) || !Directory.Exists(path))
            {
                return;
            }

            try
            {
                Directory.Delete(path, true);
            }
            catch (IOException ex)
            {
                ctx.Log.Warn("删除残留备份目录失败：" + ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                ctx.Log.Warn("删除残留备份目录失败：" + ex.Message);
            }
        }

        /// <summary>用 <see cref="Version"/> 比较，位数不同也能正确处理（4.16.5 vs 4.16.05）。</summary>
        public static int CompareVersions(string a, string b)
        {
            Version va, vb;
            if (!Version.TryParse(Normalize(a), out va) || !Version.TryParse(Normalize(b), out vb))
            {
                return string.CompareOrdinal(a ?? string.Empty, b ?? string.Empty);
            }

            return va.CompareTo(vb);
        }

        private static string Normalize(string v)
        {
            if (string.IsNullOrWhiteSpace(v))
            {
                return "0.0";
            }

            return v.Trim();
        }
    }

    // ─────────────────────────────────────────────────────────────────
    // 2. 结束正在运行的程序
}
