using System;
using System.Collections.Generic;
using System.IO;
using Installer.Abstractions.Install;
using Installer.Abstractions.Model;
using Installer.Abstractions.Platform;
using Installer.Core.Common;

namespace Installer.Core.Install
{
    // ─────────────────────────────────────────────────────────────────

    /// <summary>
    /// 写**老版安装器的私有键**作为兼容垫片：<c>HKCU\SOFTWARE\WayMark\&lt;名字&gt;</c>。
    ///
    /// **为什么需要它**：用户已经有一批程序是按老约定读版本号的 ——
    ///
    /// <code>
    /// var productName = Path.GetFileNameWithoutExtension(assembly.Location);
    /// using (var k = Registry.CurrentUser.OpenSubKey("SOFTWARE\\WayMark\\" + productName, false))
    /// {
    ///     if (k == null) return string.Empty;
    ///     return k.GetValue("DisplayVersion", "").ToString();
    /// }
    /// </code>
    ///
    /// 新版安装器只写**标准**卸载键（<c>...\CurrentVersion\Uninstall\{ProductCode}</c>），
    /// 那些程序在新装的机器上就永远读到空串 —— 用户反馈「获取版本号拿不到」正是这个。
    /// 这里把老键补上，让那批程序**一行代码都不用改**。
    ///
    /// **子键名写两个**（相同则只写一个）：
    /// ① 产品名 —— 老安装器的原始约定（<c>FrmInstallation.cs:578</c>）；
    /// ② 入口 exe 的文件名 —— 上面那段代码实际用的名字（<c>assembly.Location</c> 去掉扩展名）。
    /// 两者常常相同，但「产品名 = 照相馆 / exe = PhotoStudio.exe」时只写一个就会漏。
    ///
    /// **固定写 HKCU**：读它的代码是 <c>Registry.CurrentUser</c>，写进 HKLM 那边根本读不到。
    /// 代价是 perMachine 安装时也落在当前用户下 —— 与老安装器的行为一致。
    /// </summary>
    public sealed class LegacyCompatStep : IInstallStep
    {
        /// <summary>老安装器硬编码的厂商键（<c>项目分析报告.md</c> L616：换厂商须改代码）。</summary>
        internal const string VendorKey = @"SOFTWARE\WayMark";

        /// <inheritdoc />
        public string Id
        {
            get { return "legacy-waymark"; }
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
            var productName = StandardSteps.ProductName(ctx);
            var entry = StandardSteps.EntryPointFullPath(ctx);
            var version = ctx.Manifest.Product.Version ?? string.Empty;

            var names = new List<string>(KeyNames(productName, entry));
            if (names.Count == 0)
            {
                ctx.Log.Warn("没有产品名也没有入口程序，跳过老键兼容写入。");
                return;
            }

            if (ctx.DryRun)
            {
                return;
            }

            var hive = RegistryRoot.CurrentUser;

            var publisher = ctx.Manifest.Product.Publisher != null
                            && ctx.Manifest.Product.Publisher.Count > 0
                ? StandardSteps.Localized(ctx.Manifest.Product.Publisher, ctx, null)
                : null;

            var uninstaller = Path.Combine(ctx.InstallDir, "Uninstall.exe");

            foreach (var name in names)
            {
                var subKey = VendorKey + "\\" + name;

                // 值集合与老安装器保持一致（项目分析报告.md L280）：
                // InstallPath / DisplayIcon / DisplayName / DisplayVersion /
                // Publisher / UnInstallationString / URLInfoAbout
                JournalWriter.SetRegistryValue(ctx, Id, hive, subKey, "DisplayName", productName, false);
                JournalWriter.SetRegistryValue(ctx, Id, hive, subKey, "DisplayVersion", version, false);

                if (!string.IsNullOrEmpty(ctx.InstallDir))
                {
                    JournalWriter.SetRegistryValue(ctx, Id, hive, subKey, "InstallPath", ctx.InstallDir, false);
                }

                if (!string.IsNullOrEmpty(entry))
                {
                    JournalWriter.SetRegistryValue(ctx, Id, hive, subKey, "DisplayIcon", entry, false);
                }

                if (!string.IsNullOrEmpty(publisher))
                {
                    JournalWriter.SetRegistryValue(ctx, Id, hive, subKey, "Publisher", publisher, false);
                }

                // 老安装器把值名拼成了 UnInstallationString（大写 I），这里照抄以保持兼容，
                // 但指向**真实存在**的卸载器 —— 老实现指向的是根本不存在的 UnInstallation.exe。
                JournalWriter.SetRegistryValue(ctx, Id, hive, subKey, "UnInstallationString",
                    "\"" + uninstaller + "\" --uninstall", false);

                if (!string.IsNullOrEmpty(ctx.Manifest.Product.Url))
                {
                    JournalWriter.SetRegistryValue(ctx, Id, hive, subKey, "URLInfoAbout",
                        ctx.Manifest.Product.Url, false);
                }

                ctx.LegacyRegistryKeys.Add(InstallEngine.HiveName(hive) + "\\" + subKey);
                ctx.Log.Info("已写老键兼容项：" + InstallEngine.HiveName(hive) + "\\" + subKey +
                             "（DisplayVersion=" + version + "）");
            }
        }

        /// <summary>要写的子键名：产品名 + 入口 exe 文件名（去重，忽略大小写）。</summary>
        private static IEnumerable<string> KeyNames(string productName, string entryFullPath)
        {
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            var product = (productName ?? string.Empty).Trim();
            if (product.Length > 0 && seen.Add(product))
            {
                yield return product;
            }

            if (!string.IsNullOrEmpty(entryFullPath))
            {
                var exe = Path.GetFileNameWithoutExtension(entryFullPath);
                exe = (exe ?? string.Empty).Trim();
                if (exe.Length > 0 && seen.Add(exe))
                {
                    yield return exe;
                }
            }
        }
    }

    // ─────────────────────────────────────────────────────────────────
}
