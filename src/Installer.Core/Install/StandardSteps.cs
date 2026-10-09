using System;
using System.Collections.Generic;
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
    /// <summary>标准安装步骤管线。</summary>
    public static class StandardSteps
    {
        /// <summary>按依赖顺序返回默认的步骤列表。</summary>
        public static IList<IInstallStep> CreateDefault()
        {
            return new List<IInstallStep>
            {
                new PreflightStep(),
                new KillRunningProcessesStep(),
                new ExtractPayloadStep(),
                new ComRegistrationStep(),
                new ShortcutsStep(),
                new AutostartStep(),
                new CleanupOrphanUninstallKeysStep(),
                new UninstallInfoStep(),
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
    // ─────────────────────────────────────────────────────────────────

    /// <summary>
    /// 安装前预检：路径合法性、磁盘空间、是否已装更高版本。
    /// 现状**完全没有**这些检查（无磁盘空间检查，版本比较用 <c>Int32.Parse(v.Replace(".",""))</c>）。
    /// </summary>
    public sealed class PreflightStep : IInstallStep
    {
        /// <inheritdoc />
        public string Id
        {
            get { return "preflight"; }
        }

        /// <inheritdoc />
        public string MessageKey
        {
            get { return "Install.Preflight"; }
        }

        /// <inheritdoc />
        public bool NeedsPayload
        {
            get { return false; }
        }

        /// <inheritdoc />
        public void Execute(InstallContext ctx)
        {
            var dir = Path.GetFullPath(ctx.InstallDir);

            // 不允许往盘根、Windows 目录、System32 里装
            var root = Path.GetPathRoot(dir);
            if (string.Equals(dir.TrimEnd('\\'), root.TrimEnd('\\'), StringComparison.OrdinalIgnoreCase))
            {
                throw new InstallException(Id, "安装目录不能是驱动器根目录：" + dir);
            }

            var windows = ctx.Platform.Environment.GetFolder(SpecialFolderKind.System);
            if (!string.IsNullOrEmpty(windows) && SafePath.IsWithin(windows, dir))
            {
                throw new InstallException(Id, "安装目录不能位于系统目录内：" + dir);
            }

            // 磁盘空间：解压后 + payload 缓存，按 2 倍文件总量估算
            var need = 0L;
            foreach (var f in ctx.Manifest.Files)
            {
                need += f.Size;
            }

            need = need * 2 + (16L * 1024 * 1024);
            var free = ctx.Platform.Environment.GetAvailableFreeSpace(Path.GetPathRoot(dir) ?? dir);
            if (free >= 0 && free < need)
            {
                throw new InstallException(Id, string.Format(CultureInfo.InvariantCulture,
                    "磁盘空间不足：需要约 {0:N0} 字节，可用 {1:N0} 字节。", need, free));
            }

            ctx.Log.Info(string.Format(CultureInfo.InvariantCulture,
                "预检通过：目标目录 {0}，需要约 {1:N0} 字节，可用 {2:N0} 字节。", dir, need, free));

            // 是否已安装（读旧记录做版本比较，用 System.Version 而不是字符串拼接）
            var recordPath = InstallRecord.GetPath(dir);
            if (File.Exists(recordPath))
            {
                InstallRecord old = null;
                try
                {
                    old = JsonConvert.DeserializeObject<InstallRecord>(File.ReadAllText(recordPath, Encoding.UTF8));
                }
                catch (JsonException ex)
                {
                    ctx.Log.Warn("旧的安装记录无法解析，将按全新安装处理：" + ex.Message);
                }

                if (old != null && !string.IsNullOrEmpty(old.ProductVersion))
                {
                    var cmp = CompareVersions(old.ProductVersion, ctx.Manifest.Product.Version);
                    if (cmp > 0 && ctx.Mode == InstallMode.Install)
                    {
                        throw new InstallException(Id, string.Format(CultureInfo.InvariantCulture,
                            "已安装更高版本（{0} > {1}）。如需降级请先卸载。",
                            old.ProductVersion, ctx.Manifest.Product.Version));
                    }

                    ctx.Log.Info("检测到已安装版本 " + old.ProductVersion + "，本次 " +
                                 ctx.Manifest.Product.Version + "（比较结果 " + cmp + "）。");
                }
            }

            // 建目录（journal 会记录，失败时撤销）
            JournalWriter.EnsureDirectory(ctx, Id, dir, null);
        }

        /// <summary>用 <see cref="Version"/> 比较，位数不同也能正确处理（4.16.5 vs 4.16.05）。</summary>
        public static int CompareVersions(string a, string b)
        {
            Version va, vb;
            if (!Version.TryParse(Normalize(a), out va) || !Version.TryParse(Normalize(b), out vb))
            {
                return string.CompareOrdinal(a ?? string.Empty, b ?? string.Empty);
            }

            return va.CompareTo(vb);
        }

        private static string Normalize(string v)
        {
            if (string.IsNullOrWhiteSpace(v))
            {
                return "0.0";
            }

            return v.Trim();
        }
    }

    // ─────────────────────────────────────────────────────────────────
    // 2. 结束正在运行的程序
    // ─────────────────────────────────────────────────────────────────

    /// <summary>结束同名进程，避免文件占用。按 **exe 名**匹配（现状用产品显示名，基本匹配不到）。</summary>
    public sealed class KillRunningProcessesStep : IInstallStep
    {
        /// <inheritdoc />
        public string Id
        {
            get { return "kill"; }
        }

        /// <inheritdoc />
        public string MessageKey
        {
            get { return "Install.KillRunning"; }
        }

        /// <inheritdoc />
        public bool NeedsPayload
        {
            get { return false; }
        }

        /// <inheritdoc />
        public void Execute(InstallContext ctx)
        {
            var exe = ctx.Manifest.EntryPoint;
            if (string.IsNullOrEmpty(exe))
            {
                return;
            }

            var name = Path.GetFileNameWithoutExtension(exe);
            if (string.IsNullOrEmpty(name))
            {
                return;
            }

            if (!ctx.Platform.Processes.GetRunningProcessNames()
                    .Any(n => string.Equals(n, name, StringComparison.OrdinalIgnoreCase)))
            {
                return;
            }

            ctx.Log.Info("检测到正在运行的进程：" + name + "，尝试结束。");
            var killed = ctx.Platform.Processes.Kill(name);
            ctx.Log.Info("已结束 " + killed + " 个进程。");

            if (killed == 0)
            {
                throw new InstallException(Id, "无法结束正在运行的程序 \"" + name + "\"，请手动关闭后重试。");
            }
        }
    }

    // ─────────────────────────────────────────────────────────────────
    // 3. 解压 payload
    // ─────────────────────────────────────────────────────────────────

    /// <summary>
    /// 解压 payload 到安装目录。**所有落盘路径都走 <see cref="SafePath"/>** —— 这是 Zip Slip 的唯一防线。
    /// </summary>
    public sealed class ExtractPayloadStep : IInstallStep
    {
        /// <inheritdoc />
        public string Id
        {
            get { return "extract"; }
        }

        /// <inheritdoc />
        public string MessageKey
        {
            get { return "Install.Extracting"; }
        }

        /// <inheritdoc />
        public bool NeedsPayload
        {
            get { return true; }
        }

        /// <inheritdoc />
        public void Execute(InstallContext ctx)
        {
            if (ctx.Payload == null)
            {
                throw new InstallException(Id, "缺少 payload。");
            }

            var extracted = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            // 修复模式：内容与清单哈希一致的文件直接跳过，只补缺失/损坏的
            Dictionary<string, string> expectedHashes = null;
            var repaired = 0;
            if (ctx.Mode == InstallMode.Repair)
            {
                expectedHashes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                foreach (var f in ctx.Manifest.Files)
                {
                    if (!string.IsNullOrEmpty(f.Sha256))
                    {
                        expectedHashes[f.Path] = f.Sha256;
                    }
                }
            }

            using (var zip = new ZipArchive(ctx.Payload, ZipArchiveMode.Read, true))
            {
                foreach (var entry in zip.Entries)
                {
                    ctx.CancellationToken.ThrowIfCancellationRequested();

                    var name = entry.FullName;

                    // 目录条目
                    if (name.EndsWith("/", StringComparison.Ordinal))
                    {
                        var dirRel = SafePath.NormalizeRelative(name.TrimEnd('/'));
                        if (dirRel.Length == 0)
                        {
                            continue;
                        }

                        var dir = SafePath.ResolveWithin(ctx.InstallDir, dirRel);
                        JournalWriter.EnsureDirectory(ctx, Id, dir, null);
                        continue;
                    }

                    var rel = SafePath.NormalizeRelative(name);
                    var target = SafePath.ResolveWithin(ctx.InstallDir, rel);

                    // 修复模式：文件还在且哈希一致 → 不动它
                    if (expectedHashes != null && File.Exists(target))
                    {
                        string expected;
                        if (expectedHashes.TryGetValue(rel, out expected)
                            && string.Equals(Hashing.ComputeFile(target), expected, StringComparison.OrdinalIgnoreCase))
                        {
                            extracted.Add(rel);
                            ctx.InstalledFiles.Add(rel);
                            continue;
                        }

                        repaired++;
                        ctx.Log.Info("修复：文件缺失或已损坏，将重新写入 —— " + rel);
                    }

                    var parent = Path.GetDirectoryName(target);
                    if (!string.IsNullOrEmpty(parent))
                    {
                        JournalWriter.EnsureDirectory(ctx, Id, parent, null);
                    }

                    if (ctx.DryRun)
                    {
                        extracted.Add(rel);
                        continue;
                    }

                    JournalWriter.WriteFile(ctx, Id, target, p =>
                    {
                        using (var src = entry.Open())
                        using (var dst = File.Create(p))
                        {
                            src.CopyTo(dst);
                        }
                    });

                    extracted.Add(rel);
                    ctx.InstalledFiles.Add(rel);
                }
            }

            // 清单里的文件必须全部落到磁盘 —— 现状的 catch{} 会让"安装成功但文件缺失"成为可能
            var missing = ctx.Manifest.Files
                .Where(f => !extracted.Contains(f.Path))
                .Select(f => f.Path)
                .ToList();

            if (missing.Count > 0)
            {
                throw new InstallException(Id, "payload 中缺少 " + missing.Count + " 个清单声明的文件，例如：" +
                                               string.Join(", ", missing.Take(5).ToArray()));
            }

            ctx.Log.Info("已解压 " + extracted.Count + " 个条目。");
        }
    }

    // ─────────────────────────────────────────────────────────────────
    // 3.5 COM 注册
    // ─────────────────────────────────────────────────────────────────

    /// <summary>
    /// 注册清单里声明的 COM 组件。
    ///
    /// 现状（<c>FrmInstallation.cs:486-492</c>）先把 DLL 复制到 <c>System32</c> 再 regsvr32，
    /// 参数不加引号，`Verb="runas"` 与 `UseShellExecute=false` 冲突，异常被吞后仍打印"注册成功"。
    /// 这里不复制到 System32，参数加引号，失败**必须**报错。
    /// </summary>
    public sealed class ComRegistrationStep : IInstallStep
    {
        /// <inheritdoc />
        public string Id
        {
            get { return "com"; }
        }

        /// <inheritdoc />
        public string MessageKey
        {
            get { return "Install.Com"; }
        }

        /// <inheritdoc />
        public bool NeedsPayload
        {
            get { return false; }
        }

        /// <inheritdoc />
        public void Execute(InstallContext ctx)
        {
            var specs = ComRegistrar.ResolveAll(ctx.InstallDir, ctx.Manifest);
            if (specs.Count == 0)
            {
                return;
            }

            if (ctx.DryRun)
            {
                ctx.Log.Info("[试运行] 将注册 " + specs.Count + " 个 COM 组件。");
                return;
            }

            foreach (var spec in specs)
            {
                ctx.CancellationToken.ThrowIfCancellationRequested();

                // 先记 journal 再注册：即使 DllRegisterServer 写了一半才失败，回滚也会反注册
                ctx.Journal.Record(new JournalEntry
                {
                    StepId = Id,
                    Op = JournalOps.RegisterCom,
                    Target = spec.Path,
                    Extra = spec.Mode,
                });

                var result = ComRegistrar.Apply(spec.Path, spec.Mode, true);
                if (!result.Success)
                {
                    throw new InstallException(Id, "注册 COM 组件失败：" + spec.Path + "\r\n" + result.Message);
                }

                ctx.Log.Info("已注册 COM：" + spec.Path + "（" + spec.Mode + "）");
            }
        }
    }

    // ─────────────────────────────────────────────────────────────────
    // 4. 快捷方式
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
            if (string.Equals(location, "Desktop", StringComparison.OrdinalIgnoreCase))
            {
                return ctx.Platform.Environment.GetFolder(SpecialFolderKind.Desktop);
            }

            if (string.Equals(location, "StartMenu", StringComparison.OrdinalIgnoreCase))
            {
                return ctx.Platform.Environment.GetFolder(SpecialFolderKind.StartMenu);
            }

            if (string.Equals(location, "Startup", StringComparison.OrdinalIgnoreCase))
            {
                return ctx.Platform.Environment.GetFolder(SpecialFolderKind.Startup);
            }

            return null;
        }
    }

    // ─────────────────────────────────────────────────────────────────
    // 5. 开机启动
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
            var uninstaller = Path.Combine(ctx.InstallDir, "Uninstall.exe");
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
    // ─────────────────────────────────────────────────────────────────

    /// <summary>
    /// 释放卸载器到安装目录。
    ///
    /// 只拷贝**自己文件的前 <see cref="InstallContext.StubLength"/> 字节**（即 stub 部分），
    /// 而不是整个安装包 —— 否则 Uninstall.exe 会白白背上几十 MB 的 payload。
    /// </summary>
    public sealed class DeployUninstallerStep : IInstallStep
    {
        /// <inheritdoc />
        public string Id
        {
            get { return "deploy-uninstaller"; }
        }

        /// <inheritdoc />
        public string MessageKey
        {
            get { return "Install.DeployUninstaller"; }
        }

        /// <inheritdoc />
        public bool NeedsPayload
        {
            get { return false; }
        }

        /// <inheritdoc />
        public void Execute(InstallContext ctx)
        {
            var source = ctx.CurrentExecutablePath;
            if (string.IsNullOrEmpty(source) || !File.Exists(source))
            {
                source = Assembly.GetEntryAssembly() != null ? Assembly.GetEntryAssembly().Location : null;
            }

            if (string.IsNullOrEmpty(source) || !File.Exists(source))
            {
                ctx.Log.Warn("找不到安装器自身路径，跳过卸载器部署。");
                return;
            }

            var dest = Path.Combine(ctx.InstallDir, "Uninstall.exe");
            var stubLength = ctx.StubLength > 0 ? ctx.StubLength : new FileInfo(source).Length;

            if (ctx.DryRun)
            {
                return;
            }

            JournalWriter.WriteFile(ctx, Id, dest, p =>
            {
                using (var input = new FileStream(source, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var output = File.Create(p))
                {
                    var buffer = new byte[1 << 20];
                    long remaining = stubLength;
                    while (remaining > 0)
                    {
                        var want = (int)Math.Min(buffer.Length, remaining);
                        var read = input.Read(buffer, 0, want);
                        if (read <= 0)
                        {
                            break;
                        }

                        output.Write(buffer, 0, read);
                        remaining -= read;
                    }
                }
            });

            ctx.Log.Info("已部署卸载器：" + dest + "（" + stubLength + " 字节）");
        }
    }

    // ─────────────────────────────────────────────────────────────────
    // 8. 写安装记录
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
    // ─────────────────────────────────────────────────────────────────

    /// <summary>安装完成后启动入口程序（带存在性检查与异常处理）。</summary>
    public sealed class RunAfterInstallStep : IInstallStep
    {
        /// <inheritdoc />
        public string Id
        {
            get { return "run-after-install"; }
        }

        /// <inheritdoc />
        public string MessageKey
        {
            get { return "Install.Launching"; }
        }

        /// <inheritdoc />
        public bool NeedsPayload
        {
            get { return false; }
        }

        /// <inheritdoc />
        public void Execute(InstallContext ctx)
        {
            if (!ctx.Manifest.RunAfterInstall)
            {
                return;
            }

            var entry = StandardSteps.EntryPointFullPath(ctx);
            if (string.IsNullOrEmpty(entry) || !File.Exists(entry))
            {
                ctx.Log.Warn("清单要求安装后启动，但入口程序不存在：" + entry);
                return;
            }

            if (ctx.DryRun)
            {
                return;
            }

            try
            {
                ctx.Platform.Processes.Run(entry, string.Empty, false, out _);
                ctx.Log.Info("已启动：" + entry);
            }
            catch (Exception ex)
            {
                // 启动失败不该让整个安装失败 —— 文件已经装好了
                ctx.Log.Warn("启动入口程序失败（不影响安装）：" + ex.Message);
            }
        }
    }
}
