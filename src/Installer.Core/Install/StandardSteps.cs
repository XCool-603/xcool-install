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
    public static class StandardSteps
    {
        /// <summary>按依赖顺序返回默认的步骤列表。</summary>
        public static IList<IInstallStep> CreateDefault()
        {
            return new List<IInstallStep>
            {
                new PreflightStep(),
                new KillRunningProcessesStep(),
                // 先清掉上一版独有、这一版已经没有的文件 ——
                // 放在解压**之前**：既让"覆盖安装"真的变成新版本的文件集，
                // 也能在磁盘紧张时先腾出空间。
                new CleanupOldVersionFilesStep(),
                new ExtractPayloadStep(),
                new ComRegistrationStep(),
                new ShortcutsStep(),
                new AutostartStep(),
                new CleanupOrphanUninstallKeysStep(),
                new UninstallInfoStep(),
                // 老版安装器的私有键（HKCU\SOFTWARE\WayMark\<名字>）兼容垫片。
                // 放在标准卸载键之后：那批按老约定读版本号的程序靠它才能拿到值。
                new LegacyCompatStep(),
                new DeployUninstallerStep(),
                // 卸载器落地之后，才好在开始菜单里给它建快捷方式。
                // 以前卸载只能去「设置 → 应用」，用户觉得太绕（实测反馈）。
                new UninstallShortcutStep(),
                new WriteInstallRecordStep(),
                new RunAfterInstallStep(),
            };
        }

        // ─────────────────────────────────────────────────────────────
        // 共用小工具
        // ─────────────────────────────────────────────────────────────

        internal static RegistryRoot HiveFor(InstallerManifest m)
        {
            return string.Equals(m.Scope, InstallScopes.PerMachine, StringComparison.OrdinalIgnoreCase)
                ? RegistryRoot.LocalMachine
                : RegistryRoot.CurrentUser;
        }

        internal static string Localized(LocalizedText text, InstallContext ctx, string fallback)
        {
            if (text == null || text.Count == 0)
            {
                return fallback;
            }

            var table = ctx.Text;
            if (table != null)
            {
                return table.Resolve(text, fallback);
            }

            foreach (var kv in text)
            {
                return kv.Value;
            }

            return fallback;
        }

        internal static string ProductName(InstallContext ctx)
        {
            return StandardSteps.Localized(ctx.Manifest.Product.Name, ctx, ctx.Manifest.ProductCode);
        }

        /// <summary>
        /// 取一条**界面文案**（<c>strings</c> 表里的 key），找不到就用 <paramref name="fallback"/>。
        /// 与 <see cref="Localized"/> 的区别：那个解的是清单里的多语言字段，
        /// 这个解的是向导文案表（用于"卸载 {0}"这种带占位符的模板）。
        /// </summary>
        internal static string LocalizedOrDefault(InstallContext ctx, string key, string fallback)
        {
            var table = ctx.Text;
            if (table != null)
            {
                var text = table.Get(key);
                if (!string.IsNullOrEmpty(text))
                {
                    return text;
                }
            }

            return fallback;
        }

        /// <summary>
        /// 厂商名 —— 安装目录的厂商层级、以及卸载后回收空的厂商目录都要用它。
        /// 没填厂商时返回空串（= 不建厂商层级）。
        /// </summary>
        internal static string VendorName(InstallContext ctx)
        {
            if (ctx.Manifest.Product == null || ctx.Manifest.Product.Publisher == null)
            {
                return string.Empty;
            }

            var name = StandardSteps.Localized(ctx.Manifest.Product.Publisher, ctx, string.Empty);
            return InstallPaths.SanitizeOrEmpty(name);
        }

        internal static string EntryPointFullPath(InstallContext ctx)
        {
            if (string.IsNullOrEmpty(ctx.Manifest.EntryPoint))
            {
                return null;
            }

            return SafePath.ResolveWithin(ctx.InstallDir, ctx.Manifest.EntryPoint);
        }

        internal static string UninstallKeyPath(InstallContext ctx)
        {
            return @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\" + ctx.Manifest.ProductCode;
        }

        /// <summary>
        /// 开始菜单里本产品专用的文件夹：<c>&lt;开始菜单&gt;\&lt;产品名&gt;\</c>。
        ///
        /// **为什么不再把快捷方式平铺在开始菜单根上**（实测结论）：
        /// 把「卸载 X.lnk」直接放在 <c>…\Start Menu\Programs\</c> 根目录时，
        /// Windows 的「所有应用」列表（AppsFolder）**不会**收录它 —— 文件明明在那儿，
        /// 开始菜单里就是没有（用户反馈"目录里有、菜单里没有"）。
        /// 同样的两个链接放进 <c>Programs\&lt;产品名&gt;\</c> 里就都能显示，
        /// 这也是微信 / ToDesk / iVMS 等程序的做法。
        /// </summary>
        /// <param name="ctx">安装上下文。</param>
        /// <returns>产品文件夹（绝对路径）；取不到开始菜单目录时返回 null。</returns>
        internal static string StartMenuProductFolder(InstallContext ctx)
        {
            var root = ShortcutFolder(ctx, "StartMenu");
            if (string.IsNullOrEmpty(root))
            {
                return null;
            }

            return Path.Combine(root, SafePath.SanitizeFileName(ProductName(ctx)));
        }

        /// <summary>
        /// 快捷方式该放哪个目录。
        ///
        /// 「所有用户」安装必须落到**公共**桌面 / 开始菜单 / 启动，否则快捷方式只出现在
        /// "装它的那个账号"里：别的账号看不到，而提权时若用的是另一个管理员账号，
        /// 连装它的人都看不到（用户反馈过"勾了桌面快捷方式，装完桌面上没有"）。
        ///
        /// 抽到这里是因为有两处要用：清单里的快捷方式（<see cref="ShortcutsStep"/>）
        /// 和自动创建的「卸载」快捷方式（<see cref="UninstallShortcutStep"/>）。
        /// </summary>
        /// <param name="ctx">安装上下文。</param>
        /// <param name="location">Desktop / StartMenu / Startup。</param>
        internal static string ShortcutFolder(InstallContext ctx, string location)
        {
            var allUsers = string.Equals(ctx.Manifest.Scope, InstallScopes.PerMachine,
                StringComparison.OrdinalIgnoreCase);

            if (string.Equals(location, "Desktop", StringComparison.OrdinalIgnoreCase))
            {
                return Pick(ctx, allUsers, SpecialFolderKind.CommonDesktop, SpecialFolderKind.Desktop);
            }

            if (string.Equals(location, "StartMenu", StringComparison.OrdinalIgnoreCase))
            {
                return Pick(ctx, allUsers, SpecialFolderKind.CommonStartMenu, SpecialFolderKind.StartMenu);
            }

            if (string.Equals(location, "Startup", StringComparison.OrdinalIgnoreCase))
            {
                return Pick(ctx, allUsers, SpecialFolderKind.CommonStartup, SpecialFolderKind.Startup);
            }

            return null;
        }

        /// <summary>
        /// 取"首选目录"，取不到（老平台实现不认新枚举 / 路径为空）就退回备选目录。
        /// 宁可按当前用户装，也不能"什么都不建"。
        /// </summary>
        private static string Pick(InstallContext ctx, bool allUsers,
                                   SpecialFolderKind common, SpecialFolderKind perUser)
        {
            if (!allUsers)
            {
                return ctx.Platform.Environment.GetFolder(perUser);
            }

            string dir = null;
            try
            {
                dir = ctx.Platform.Environment.GetFolder(common);
            }
            catch (Exception)
            {
                dir = null;
            }

            if (string.IsNullOrEmpty(dir) || !Directory.Exists(dir))
            {
                ctx.Log.Warn("取不到公共目录（" + common + "），退回当前用户目录 ——" +
                             "该快捷方式只有当前账号看得到。");
                return ctx.Platform.Environment.GetFolder(perUser);
            }

            return dir;
        }
    }

    // ─────────────────────────────────────────────────────────────────
    // 1. 预检
}
