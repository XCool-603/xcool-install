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
    /// 清掉"同一个安装目录、但 productCode 不同"的残留卸载键。
    ///
    /// **为什么必须做**：制作端以前每次打开都生成新的 productCode，同一个产品装三次
    /// 就在「程序和功能」里留下三条记录 —— 而且它们指向同一个目录和同一个卸载器。
    /// 卸掉其中一条后安装记录就没了，剩下的**再也卸不掉**（用户实际遇到的问题）。
    ///
    /// 这里在写自己那条之前先把孤儿清掉，所以重复安装最终只会剩一条。
    /// </summary>
    public sealed class CleanupOrphanUninstallKeysStep : IInstallStep
    {
        /// <inheritdoc />
        public string Id
        {
            get { return "cleanup-orphans"; }
        }

        /// <inheritdoc />
        public string MessageKey
        {
            get { return "Install.CleanupOrphans"; }
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

            var orphans = UninstallKeys.FindOrphans(
                ctx.Platform.Registry, ctx.InstallDir, ctx.Manifest.ProductCode);

            if (orphans.Count == 0)
            {
                return;
            }

            foreach (var orphan in orphans)
            {
                try
                {
                    ctx.Platform.Registry.DeleteSubKeyTree(orphan.Hive, orphan.SubKey);
                    ctx.Log.Info(string.Format(CultureInfo.InvariantCulture,
                        "已清理残留卸载项：{0}（键 {1}，显示名 {2}）",
                        InstallEngine.HiveName(orphan.Hive) + "\\" + orphan.SubKey,
                        orphan.KeyName,
                        orphan.DisplayName));
                }
                catch (Exception ex)
                {
                    // 清不掉不算失败 —— 不能因为一条老记录就阻断安装
                    ctx.Log.Warn("清理残留卸载项失败（已跳过）：" + orphan.SubKey + " —— " + ex.Message);
                }
            }
        }
    }

    // ─────────────────────────────────────────────────────────────────
    // 6. 标准卸载键
}
