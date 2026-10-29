using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Installer.Abstractions.Install;
using Installer.Abstractions.Model;
using Installer.Abstractions.Platform;
using Installer.Core.Security;
using Installer.Core.Uninstall;

namespace Installer.Core.Install
{
    // ─────────────────────────────────────────────────────────────────

    /// <summary>
    /// 在「开始菜单」里放一个**卸载**快捷方式。
    ///
    /// 用户的诉求：卸载只能去「设置 → 应用」，太绕 —— 希望开始菜单里就能点。
    /// 这也是 Windows 上的惯例（Chrome / VS Code / 微信 / ToDesk 都把卸载入口放在程序菜单里）。
    ///
    /// **必须放进「产品文件夹」**（<c>开始菜单\程序\&lt;产品名&gt;\</c>）：
    /// 实测把「卸载 X.lnk」直接平铺在 <c>Programs\</c> 根上时，文件确实生成了，
    /// 但 Windows 的「所有应用」列表（AppsFolder，也就是开始菜单里看到的那个列表）
    /// **根本不收录它** —— 用户反馈的"目录里有、菜单里没有"就是这件事。
    /// 同样的两个链接放进产品文件夹里就都能显示；这也是微信 / ToDesk / iVMS 的做法。
    /// App 快捷方式也一起挪进产品文件夹（见 <see cref="ShortcutsStep"/>），
    /// 所以用户点开产品文件夹就能同时看到「程序」和「卸载」。
    ///
    /// 其它几个刻意的地方：
    /// <list type="bullet">
    /// <item>目标写绝对路径的 <c>Uninstall.exe</c>，并带 <c>--uninstall --product-code</c>：
    /// 这样即便安装记录丢了，卸载器也能靠 product-code 正确定位自己的卸载项。</item>
    /// <item>目录跟着 scope 走（所有用户 → 公共开始菜单）。</item>
    /// <item>走 <see cref="JournalWriter.CreateShortcut"/>，所以会进
    /// <c>ctx.CreatedShortcuts</c> → 写进安装记录 → **卸载时被一起删掉**。</item>
    /// <item>顺手删掉**上一版**平铺在开始菜单根上的链接（升级后不留重复条目）。</item>
    /// </list>
    /// </summary>
    public sealed class UninstallShortcutStep : IInstallStep
    {
        /// <summary>卸载器相对安装目录的文件名（单一事实来源在 <see cref="InstallRecord"/>）。</summary>
        internal const string UninstallerFileName = InstallRecord.UninstallerFileName;

        /// <inheritdoc />
        public string Id
        {
            get { return "uninstall-shortcut"; }
        }

        /// <inheritdoc />
        public string MessageKey
        {
            get { return "Install.UninstallShortcut"; }
        }

        /// <inheritdoc />
        public bool NeedsPayload
        {
            get { return false; }
        }

        /// <inheritdoc />
        public void Execute(InstallContext ctx)
        {
            // 必须先有卸载器：这一步排在 deploy-uninstaller 之后，
            // 但修复安装/降级场景下可能没走到那一步 —— 文件不存在就别建死链接。
            var target = Path.Combine(ctx.InstallDir, UninstallerFileName);
            if (!File.Exists(target))
            {
                ctx.Log.Warn("没有找到卸载器，跳过开始菜单里的卸载快捷方式：" + target);
                return;
            }

            var folder = StandardSteps.StartMenuProductFolder(ctx);
            if (string.IsNullOrEmpty(folder))
            {
                ctx.Log.Warn("取不到开始菜单目录，跳过卸载快捷方式。");
                return;
            }

            JournalWriter.EnsureDirectory(ctx, Id, folder, null);

            // 显示名就叫「卸载」—— 和**老安装器**（D:\Worker\杂项\安装包制作和卸载）一致：
            // 它一直是在开始菜单的产品文件夹里放 `<产品名>.lnk` + `卸载.lnk`，多年没出问题。
            var displayName = StandardSteps.LocalizedOrDefault(ctx, "Shortcut.Uninstall", "卸载");

            // 开始菜单**必须指向另一个文件**（卸载.exe），不能指向卸载键登记的那个 Uninstall.exe ——
            // Windows 会把"卸载键里的 exe"当卸载程序隐藏起来（实测：指 Uninstall.exe 就不显示，
            // 指向同内容的另一份立刻显示；把卸载键也改成指那份，它又消失）。
            var menuTarget = EnsureMenuUninstaller(ctx, target);

            var linkPath = Path.Combine(folder, SafePath.SanitizeFileName(displayName) + ".lnk");
            var arguments = ("--uninstall --product-code " + (ctx.Manifest.ProductCode ?? string.Empty)).Trim();

            JournalWriter.CreateShortcut(ctx, Id, linkPath, menuTarget, arguments,
                ctx.InstallDir, displayName, menuTarget);

            ctx.Log.Info("已在开始菜单创建卸载快捷方式：" + linkPath);

            RemoveStaleShortcutLinks(ctx, Id, folder);
        }

        /// <summary>
        /// 准备开始菜单入口指向的那份卸载器（<see cref="InstallRecord.MenuUninstallerFileName"/>）。
        ///
        /// 和 <see cref="InstallRecord.UninstallerFileName"/> 同内容、不同文件：一个是"卸载键登记的名字"，
        /// 一个是"开始菜单里显示的名字"。**不能用硬链接**（同一个 File ID 会被一起判定成卸载程序）。
        /// 每次安装重写一遍，顺便把更早实验版留下的 <c>AppUninstall.exe</c> 清掉。
        /// </summary>
        private string EnsureMenuUninstaller(InstallContext ctx, string uninstallerPath)
        {
            // 清掉更早实验版留下的名字
            foreach (var stale in new[] { "AppUninstall.exe" })
            {
                var stalePath = Path.Combine(ctx.InstallDir, stale);
                if (File.Exists(stalePath))
                {
                    try
                    {
                        JournalWriter.DeleteFile(ctx, Id, stalePath);
                    }
                    catch (Exception ex)
                    {
                        ctx.Log.Warn("清理旧入口文件失败（已跳过）：" + stalePath + " —— " + ex.Message);
                    }
                }
            }

            var menu = Path.Combine(ctx.InstallDir, InstallRecord.MenuUninstallerFileName);
            try
            {
                JournalWriter.WriteFile(ctx, Id, menu, p =>
                {
                    if (File.Exists(p))
                    {
                        File.Delete(p);
                    }

                    File.Copy(uninstallerPath, p, true);
                });
            }
            catch (Exception ex)
            {
                // 退回 Uninstall.exe：功能能用，只是开始菜单里可能不显示
                ctx.Log.Warn("创建开始菜单用的卸载器失败，退回 Uninstall.exe：" + ex.Message);
                return uninstallerPath;
            }

            return menu;
        }


        /// <summary>
        /// 删掉**上一版建过、这一版不再建**的快捷方式。
        ///
        /// 典型场景（两个都是实测踩到的）：
        /// <list type="bullet">
        /// <item>上一版把链接平铺在 <c>Programs\</c> 根上，这一版改到产品文件夹里 —— 根上那条还留着，
        /// 而且它本来就不显示，看着像"什么都没变"；</item>
        /// <item>上一版的卸载入口叫「卸载 X」，这一版改叫「X 卸载」—— 不清掉旧的，
        /// 开始菜单里显示的还是旧那条，用户以为"升级了跟之前没区别"。</item>
        /// </list>
        ///
        /// 判断依据是**安装记录**：只删本产品上一条记录里登记过、而这次没有重新创建的链接。
        /// 用户自己的条目不在记录里，永远不会被这条逻辑碰到。
        /// </summary>
        private static void RemoveStaleShortcutLinks(InstallContext ctx, string stepId, string productFolder)
        {
            InstallRecord record;
            try
            {
                record = UninstallEngine.LoadRecord(ctx.InstallDir);
            }
            catch (Exception ex)
            {
                ctx.Log.Warn("读旧安装记录失败，跳过旧快捷方式清理：" + ex.Message);
                return;
            }

            if (record == null || record.Shortcuts == null)
            {
                return;
            }

            var keep = new HashSet<string>(ctx.CreatedShortcuts, StringComparer.OrdinalIgnoreCase);

            foreach (var path in record.Shortcuts)
            {
                if (string.IsNullOrWhiteSpace(path) || keep.Contains(path) || !File.Exists(path))
                {
                    continue;
                }

                // 只处理"我们自己的地盘"：开始菜单/桌面根，或本产品的开始菜单文件夹
                var parent = Path.GetDirectoryName(path);
                if (!IsDirectlyInStartMenuRoot(ctx, parent) &&
                    !IsDirectlyOnDesktop(ctx, parent) &&
                    !string.Equals(parent, productFolder, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                try
                {
                    if (JournalWriter.DeleteFile(ctx, stepId, path))
                    {
                        ctx.Log.Info("已移除上一版遗留的快捷方式：" + path);
                    }
                }
                catch (Exception ex)
                {
                    ctx.Log.Warn("移除旧快捷方式失败（已跳过）：" + path + " —— " + ex.Message);
                }
            }
        }

        /// <summary>这个目录是不是开始菜单根（用户或公共）。</summary>
        private static bool IsDirectlyInStartMenuRoot(InstallContext ctx, string dir)
        {
            return MatchesFolder(ctx, dir, SpecialFolderKind.StartMenu, SpecialFolderKind.CommonStartMenu);
        }

        /// <summary>这个目录是不是桌面（用户或公共）。</summary>
        private static bool IsDirectlyOnDesktop(InstallContext ctx, string dir)
        {
            return MatchesFolder(ctx, dir, SpecialFolderKind.Desktop, SpecialFolderKind.CommonDesktop);
        }

        private static bool MatchesFolder(InstallContext ctx, string dir, params SpecialFolderKind[] kinds)
        {
            if (string.IsNullOrEmpty(dir))
            {
                return false;
            }

            foreach (var kind in kinds)
            {
                string root;
                try
                {
                    root = ctx.Platform.Environment.GetFolder(kind);
                }
                catch (Exception)
                {
                    continue;
                }

                if (!string.IsNullOrEmpty(root) &&
                    string.Equals(dir.TrimEnd('\\'), root.TrimEnd('\\'), StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
