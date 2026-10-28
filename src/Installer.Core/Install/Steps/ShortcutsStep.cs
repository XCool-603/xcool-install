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

    /// <summary>按清单创建快捷方式。制作端勾选什么，这里就建什么。</summary>
    public sealed class ShortcutsStep : IInstallStep
    {
        /// <inheritdoc />
        public string Id
        {
            get { return "shortcuts"; }
        }

        /// <inheritdoc />
        public string MessageKey
        {
            get { return "Install.Shortcuts"; }
        }

        /// <inheritdoc />
        public bool NeedsPayload
        {
            get { return false; }
        }

        /// <inheritdoc />
        public void Execute(InstallContext ctx)
        {
            if (ctx.Manifest.Shortcuts == null || ctx.Manifest.Shortcuts.Count == 0)
            {
                return;
            }

            var entry = StandardSteps.EntryPointFullPath(ctx);

            foreach (var spec in ctx.Manifest.Shortcuts)
            {
                var folder = ResolveFolder(ctx, spec.Location);
                if (folder == null)
                {
                    ctx.Log.Warn("未知的快捷方式位置，跳过：" + spec.Location);
                    continue;
                }

                var displayName = StandardSteps.Localized(spec.Name, ctx, StandardSteps.ProductName(ctx));
                var linkPath = Path.Combine(folder, SafePath.SanitizeFileName(displayName) + ".lnk");

                var target = string.IsNullOrEmpty(spec.Target)
                    ? entry
                    : SafePath.ResolveWithin(ctx.InstallDir, spec.Target);

                if (string.IsNullOrEmpty(target))
                {
                    ctx.Log.Warn("快捷方式 " + displayName + " 没有目标，跳过。");
                    continue;
                }

                var working = string.IsNullOrEmpty(spec.WorkingDirectory)
                    ? ctx.InstallDir
                    : SafePath.ResolveWithin(ctx.InstallDir, spec.WorkingDirectory);

                JournalWriter.CreateShortcut(ctx, Id, linkPath, target, spec.Arguments ?? string.Empty,
                    working, displayName, target);

                ctx.Log.Info("已创建快捷方式：" + linkPath);
            }
        }

        private static string ResolveFolder(InstallContext ctx, string location)
        {
            var isMachine = string.Equals(ctx.Manifest.Scope, InstallScopes.PerMachine, StringComparison.OrdinalIgnoreCase);

            if (string.Equals(location, "Desktop", StringComparison.OrdinalIgnoreCase))
            {
                return ctx.Platform.Environment.GetFolder(isMachine
                    ? SpecialFolderKind.CommonDesktop
                    : SpecialFolderKind.Desktop);
            }

            if (string.Equals(location, "StartMenu", StringComparison.OrdinalIgnoreCase))
            {
                return ctx.Platform.Environment.GetFolder(isMachine
                    ? SpecialFolderKind.CommonStartMenu
                    : SpecialFolderKind.StartMenu);
            }

            if (string.Equals(location, "Startup", StringComparison.OrdinalIgnoreCase))
            {
                return ctx.Platform.Environment.GetFolder(isMachine
                    ? SpecialFolderKind.CommonStartup
                    : SpecialFolderKind.Startup);
            }

            return null;
        }
    }

    // ─────────────────────────────────────────────────────────────────
    // 5. 开机启动
}
