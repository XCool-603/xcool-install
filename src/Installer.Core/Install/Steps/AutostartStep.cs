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
    /// 开机启动。**与 scope 一致**：perUser 写 HKCU，perMachine 写 HKLM。
    /// 现状无论什么情况都写 HKLM（<c>FrmInstallation.cs:646</c>），而卸载端从不删它。
    /// </summary>
    public sealed class AutostartStep : IInstallStep
    {
        internal const string RunKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";

        /// <inheritdoc />
        public string Id
        {
            get { return "autostart"; }
        }

        /// <inheritdoc />
        public string MessageKey
        {
            get { return "Install.Autostart"; }
        }

        /// <inheritdoc />
        public bool NeedsPayload
        {
            get { return false; }
        }

        /// <inheritdoc />
        public void Execute(InstallContext ctx)
        {
            if (!ctx.Manifest.Autostart)
            {
                return;
            }

            var entry = StandardSteps.EntryPointFullPath(ctx);
            if (string.IsNullOrEmpty(entry))
            {
                ctx.Log.Warn("清单要求开机启动，但没有 entryPoint，跳过。");
                return;
            }

            var hive = StandardSteps.HiveFor(ctx.Manifest);
            var name = StandardSteps.ProductName(ctx);

            JournalWriter.SetRegistryValue(ctx, Id, hive, RunKey, name, "\"" + entry + "\"", false);
            ctx.Log.Info("已注册开机启动：" + name + " → " + entry);
        }
    }

    // ─────────────────────────────────────────────────────────────────
    // 5.5 清理残留的卸载键
}
