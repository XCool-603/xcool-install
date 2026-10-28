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

        /// <summary>
        /// 比较两个版本号：<paramref name="left"/> 小于 <paramref name="right"/> 返回负数，相等返回 0，大于返回正数。
        ///
        /// 用途：判定这次到底是**升级**（Update）、**修复**（Repair）还是**降级**（Downgrade）。
        ///
        /// 规则：
        /// ① 按点分段，逐段比数字（**"1.10" &gt; "1.9"**，不是字符串比较）；
        /// ② 缺的段当 0 —— 所以 "1.0" 与 "1.0.0" **相等**（同一个版本，应当判为修复而不是升级）；
        /// ③ 某段不是纯数字（如 "1.2.3-beta"）时，该段按不区分大小写字符串比；
        /// ④ 空值视为"最小版本"：没装过 &lt; 装过任何版本。
        /// </summary>
        public static int CompareVersions(string left, string right)
        {
            var a = (left ?? string.Empty).Trim();
            var b = (right ?? string.Empty).Trim();

            if (a.Length == 0 && b.Length == 0)
            {
                return 0;
            }

            if (a.Length == 0)
            {
                return -1;
            }

            if (b.Length == 0)
            {
                return 1;
            }

            var pa = a.Split('.');
            var pb = b.Split('.');
            var count = Math.Max(pa.Length, pb.Length);

            for (var i = 0; i < count; i++)
            {
                var sa = i < pa.Length ? pa[i].Trim() : "0";
                var sb = i < pb.Length ? pb[i].Trim() : "0";

                long na, nb;
                var isNumberA = long.TryParse(sa, NumberStyles.Integer, CultureInfo.InvariantCulture, out na);
                var isNumberB = long.TryParse(sb, NumberStyles.Integer, CultureInfo.InvariantCulture, out nb);

                if (isNumberA && isNumberB)
                {
                    if (na != nb)
                    {
                        return na < nb ? -1 : 1;
                    }

                    continue;
                }

                // 有一段不是纯数字（预发布标记之类）—— 退回字符串比较，但仍逐段比
                var text = string.Compare(sa, sb, StringComparison.OrdinalIgnoreCase);
                if (text != 0)
                {
                    return text < 0 ? -1 : 1;
                }
            }

            return 0;
        }
    }
}
