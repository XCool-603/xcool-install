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
    }

    // ─────────────────────────────────────────────────────────────────
    // 1. 预检
}
