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
    /// 写**标准**卸载键，让产品出现在「控制面板 → 程序和功能」。
    /// 现状写的是私有的 <c>HKCU\SOFTWARE\WayMark\&lt;产品&gt;</c>，且值名拼成
    /// <c>UnInstallationString</c>、指向根本不存在的 <c>UnInstallation.exe</c>。
    /// </summary>
    public sealed class UninstallInfoStep : IInstallStep
    {
        /// <inheritdoc />
        public string Id
        {
            get { return "uninstall-info"; }
        }

        /// <inheritdoc />
        public string MessageKey
        {
            get { return "Install.Registering"; }
        }

        /// <inheritdoc />
        public bool NeedsPayload
        {
            get { return false; }
        }

        /// <inheritdoc />
        public void Execute(InstallContext ctx)
        {
            var hive = StandardSteps.HiveFor(ctx.Manifest);
            var subKey = StandardSteps.UninstallKeyPath(ctx);
            var registry = ctx.Platform.Registry;

            // 先记一条"创建子键"，这样回滚能把整个键删掉
            if (!registry.SubKeyExists(hive, subKey))
            {
                ctx.Journal.Record(new JournalEntry
                {
                    StepId = Id,
                    Op = JournalOps.CreateSubKey,
                    Hive = InstallEngine.HiveName(hive),
                    SubKey = subKey,
                    ExistedBefore = false,
                });
            }

            var name = StandardSteps.ProductName(ctx);
            var entry = StandardSteps.EntryPointFullPath(ctx);
            var uninstaller = Path.Combine(ctx.InstallDir, InstallRecord.UninstallerFileName);
            var estimatedKb = (int)Math.Min(int.MaxValue, ctx.Manifest.Files.Sum(f => f.Size) / 1024);

            Set(ctx, hive, subKey, "DisplayName", name);
            Set(ctx, hive, subKey, "DisplayVersion", ctx.Manifest.Product.Version ?? "1.0.0.0");

            var publisher = ctx.Manifest.Product.Publisher != null && ctx.Manifest.Product.Publisher.Count > 0
                ? StandardSteps.Localized(ctx.Manifest.Product.Publisher, ctx, null)
                : null;
            if (!string.IsNullOrEmpty(publisher))
            {
                Set(ctx, hive, subKey, "Publisher", publisher);
            }

            if (!string.IsNullOrEmpty(entry))
            {
                Set(ctx, hive, subKey, "DisplayIcon", "\"" + entry + "\"");
            }

            Set(ctx, hive, subKey, "InstallLocation", ctx.InstallDir);

            // 把 productCode 写进命令行：这样即使安装记录丢了，卸载器也知道该删哪一条卸载键
            var code = ctx.Manifest.ProductCode;
            var codeArg = string.IsNullOrWhiteSpace(code) ? string.Empty : " --product-code " + code;

            Set(ctx, hive, subKey, "UninstallString",
                "\"" + uninstaller + "\" --uninstall" + codeArg);
            Set(ctx, hive, subKey, "QuietUninstallString",
                "\"" + uninstaller + "\" --uninstall --silent" + codeArg);
            Set(ctx, hive, subKey, "NoModify", "1");
            Set(ctx, hive, subKey, "NoRepair", "1");

            if (!string.IsNullOrEmpty(ctx.Manifest.Product.Url))
            {
                Set(ctx, hive, subKey, "URLInfoAbout", ctx.Manifest.Product.Url);
            }

            registry.SetDword(hive, subKey, "EstimatedSize", estimatedKb);
            ctx.Log.Info("已写标准卸载键：" + InstallEngine.HiveName(hive) + "\\" + subKey);
        }

        private void Set(InstallContext ctx, RegistryRoot hive, string subKey, string name, string value)
        {
            JournalWriter.SetRegistryValue(ctx, Id, hive, subKey, name, value, false);
        }
    }

    // ─────────────────────────────────────────────────────────────────
    // 7. 部署卸载器
}
