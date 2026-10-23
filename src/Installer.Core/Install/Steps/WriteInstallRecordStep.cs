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

    /// <summary>写安装记录 —— 卸载端按它精确删除，而不是"把目录全删了"。</summary>
    public sealed class WriteInstallRecordStep : IInstallStep
    {
        /// <inheritdoc />
        public string Id
        {
            get { return "install-record"; }
        }

        /// <inheritdoc />
        public string MessageKey
        {
            get { return "Install.WriteRecord"; }
        }

        /// <inheritdoc />
        public bool NeedsPayload
        {
            get { return false; }
        }

        /// <inheritdoc />
        public void Execute(InstallContext ctx)
        {
            var record = new InstallRecord
            {
                SchemaVersion = 1,
                ProductCode = ctx.Manifest.ProductCode,
                ProductName = StandardSteps.ProductName(ctx),
                ProductVersion = ctx.Manifest.Product.Version,
                Scope = ctx.Manifest.Scope,
                InstallDir = ctx.InstallDir,
                SandboxRoot = ctx.SandboxRoot,
                InstalledAtUtc = DateTimeOffset.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture),
                EntryPoint = ctx.Manifest.EntryPoint,
                Files = new List<string>(ctx.InstalledFiles),
                FileHashes = ctx.Manifest.Files.ToDictionary(
                    f => f.Path, f => f.Sha256 ?? string.Empty, StringComparer.OrdinalIgnoreCase),
                Directories = new List<string>(ctx.Manifest.Directories ?? new List<string>()),
                Shortcuts = new List<string>(ctx.CreatedShortcuts),
                RegistryValues = new List<RegistryValueRecord>(ctx.WrittenRegistryValues),
                // 老约定的兼容键 —— 卸载时要整棵删掉，否则留一堆空键
                LegacyKeys = new List<string>(ctx.LegacyRegistryKeys),
                UninstallKeyPath = InstallEngine.HiveName(StandardSteps.HiveFor(ctx.Manifest)) + "\\" +
                                   StandardSteps.UninstallKeyPath(ctx),
                UninstallerPath = Path.Combine(ctx.InstallDir, "Uninstall.exe"),
                Com = ComRegistrar.ResolveAll(ctx.InstallDir, ctx.Manifest),
            };

            if (ctx.Manifest.Autostart)
            {
                record.AutostartKeyPath = InstallEngine.HiveName(StandardSteps.HiveFor(ctx.Manifest)) + "\\" + AutostartStep.RunKey;
                record.AutostartValueName = StandardSteps.ProductName(ctx);
            }

            var path = InstallRecord.GetPath(ctx.InstallDir);
            var json = JsonConvert.SerializeObject(record, Formatting.Indented);

            if (ctx.DryRun)
            {
                ctx.Log.Info("[试运行] 将写入安装记录：" + path);
                return;
            }

            JournalWriter.WriteFile(ctx, Id, path, p => File.WriteAllText(p, json, new UTF8Encoding(false)));
            ctx.Log.Info("已写安装记录：" + path + "（" + record.Files.Count + " 个文件）");
        }
    }

    // ─────────────────────────────────────────────────────────────────
    // 9. 安装后启动
}
