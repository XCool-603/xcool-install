using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Installer.Abstractions.Install;
using Installer.Abstractions.Model;
using Installer.Abstractions.Packaging;
using Installer.Abstractions.Platform;
using Installer.Core.Common;
using Installer.Core.Install;
using Installer.Core.Packaging;
using Installer.Core.Platform;
using Installer.Core.Uninstall;
using Newtonsoft.Json;

namespace Installer.Runtime
{
    /// <summary>
    /// 安装端宿主。
    ///
    /// 职责很薄：解析参数 → 组装 <see cref="InstallContext"/> → 交给
    /// <see cref="InstallEngine"/>。所有业务逻辑都在 Core 里，所以可单测。
    /// </summary>
    internal static class InstallHost
    {
        private const string UsageText =
            "Installer.Runtime —— 安装包\n" +
            "\n" +
            "安装：\n" +
            "  --target <目录>    安装目录（默认取清单里的 defaultInstallDir）\n" +
            "  --silent           不弹界面\n" +
            "  --culture <语言>   界面语言，如 en / zh-Hans（默认跟随系统）\n" +
            "  --result <文件>    把结果写成 JSON\n" +
            "  --sandbox <目录>   沙箱模式：快捷方式/注册表全部重定向到该目录，不碰真实系统\n" +
            "  --dry-run          只走流程不落盘\n" +
            "  --verify-only      只校验容器与哈希\n" +
            "  --elevated         内部用：表示已经提过权，避免无限重启\n" +
            "\n" +
            "卸载：\n" +
            "  --uninstall        按安装记录卸载（由控制面板调用）\n";

        public static int Run(string[] args)
        {
            try
            {
                Console.OutputEncoding = Encoding.UTF8;
            }
            catch (IOException)
            {
            }

            var options = ParseOptions(args);

            try
            {
                if (options.RenderUiDir != null)
                {
                    return RenderUi(options);
                }

                if (options.CleanupDir != null)
                {
                    return RunCleanup(options);
                }

                if (options.Uninstall)
                {
                    return RunUninstall(options, args);
                }

                return RunInstall(options, args);
            }
            catch (Exception ex)
            {
                WriteResult(options.ResultPath, new { ok = false, error = ex.Message, type = ex.GetType().Name });
                return Report(options, false, ex.Message);
            }
        }

        // ─────────────────────────────────────────────────────────────
        // 安装
        // ─────────────────────────────────────────────────────────────

        private static int RunInstall(RuntimeOptions options, string[] args)
        {
            var selfPath = GetSelfPath();

            using (var self = new FileStream(selfPath, FileMode.Open, FileAccess.Read, FileShare.Read, 1 << 20))
            {
                var info = ContainerFormat.TryRead(self);
                if (info == null)
                {
                    throw new InvalidDataException(
                        "这个 exe 里没有本程序的安装包容器。\r\n" +
                        "它可能是一个尚未打包的 stub 模板，或者是旧版（v1）安装包。");
                }

                if (!ContainerFormat.VerifyStubHash(self, info))
                {
                    throw new InvalidDataException("stub 校验失败：安装器代码区被篡改，已中止。");
                }

                if (!ContainerFormat.VerifyPayloadHash(self, info))
                {
                    throw new InvalidDataException("payload 校验失败：SHA-256 与容器记录不一致，文件可能已损坏或被篡改。");
                }

                var manifest = ContainerFormat.ReadManifest(self, info);

                if (options.VerifyOnly)
                {
                    WriteResult(options.ResultPath, new
                    {
                        ok = true,
                        mode = "verify-only",
                        formatVersion = info.FormatVersion,
                        stubBytes = info.StubLength,
                        payloadBytes = info.PayloadLength,
                        payloadSha256 = Hashing.ToHex(info.PayloadSha256),
                        entryCount = manifest.Files.Count,
                    });

                    return Report(options, true, string.Format(CultureInfo.InvariantCulture,
                        "校验通过。\r\n容器 v{0}，stub {1:N0} 字节，payload {2:N0} 字节，{3} 个文件。",
                        info.FormatVersion, info.StubLength, info.PayloadLength, manifest.Files.Count));
                }

                // 语言先解析出来：安装目录名、快捷方式名、卸载显示名都要按它取
                var text = new StringTable(manifest.Strings,
                    options.Culture ?? StringTable.DetectRequestedCulture(), manifest.DefaultCulture);
                var productName = text.Resolve(manifest.Product.Name, manifest.ProductCode);

                var installDir = ResolveInstallDir(options, manifest, CreatePlatform(options), productName);

                // 「所有用户」要写 HKLM，而 stub 声明的是 asInvoker —— 不主动提权就必然被拒。
                // 这里在**进向导之前**就把自己重启成管理员，用户只需要点一次 UAC。
                if (NeedsElevation(manifest) && !options.AlreadyElevated && !IsElevated())
                {
                    return RelaunchElevated(options, args);
                }

                // 非静默 → 走向导界面
                if (!options.Silent)
                {
                    return RunWizard(options, selfPath, manifest, installDir);
                }

                var journalPath = Path.Combine(installDir, ".install-journal.jsonl");
                var backupDir = Path.Combine(installDir, ".rollback");

                var logger = CreateLogger(options, installDir);
                var platform = CreatePlatform(options, logger);

                var context = new InstallContext
                {
                    Manifest = manifest,
                    Mode = options.Repair ? InstallMode.Repair : InstallMode.Install,
                    InstallDir = installDir,
                    Payload = ContainerFormat.OpenPayload(self, info),
                    StubLength = info.StubLength,
                    CurrentExecutablePath = selfPath,
                    Platform = platform,
                    Journal = new InstallJournal(journalPath, backupDir),
                    Log = logger,
                    Strings = manifest.Strings,
                    Text = text,
                    DryRun = options.DryRun,
                    SandboxRoot = options.SandboxRoot,
                    CancellationToken = System.Threading.CancellationToken.None,
                };

                logger.Info("=== 开始安装 " + manifest.ProductCode + " → " + installDir + " ===");
                logger.Info("平台：" + (options.SandboxRoot != null ? "沙箱 " + options.SandboxRoot : "真实系统") +
                            "；语言：" + text.Culture);

                var progress = options.Silent
                    ? new Progress<InstallProgress>(p => Console.WriteLine(
                        string.Format(CultureInfo.InvariantCulture, "  [{0,3}] {1}", p.Percent, p.Message)))
                    : null;

                var engine = new InstallEngine();
                var run = engine.Run(context, progress);

                WriteResult(options.ResultPath, new
                {
                    ok = run.Success,
                    mode = "install",
                    installDir = installDir,
                    sandbox = options.SandboxRoot,
                    productCode = manifest.ProductCode,
                    productName = productName,
                    productVersion = manifest.Product.Version,
                    scope = manifest.Scope,
                    culture = text.Culture,
                    entryCount = manifest.Files.Count,
                    filesInstalled = context.InstalledFiles.Count,
                    shortcutsCreated = context.CreatedShortcuts.Count,
                    registryValuesWritten = context.WrittenRegistryValues.Count,
                    stepsExecuted = run.StepsExecuted,
                    stepCount = run.StepCount,
                    rolledBack = run.RolledBack,
                    failedStep = run.FailedStepId,
                    rollbackIssues = run.RollbackIssues,
                    error = run.ErrorMessage,
                    payloadSha256 = Hashing.ToHex(info.PayloadSha256),
                });

                if (!run.Success)
                {
                    return Report(options, false,
                        "安装失败（步骤 " + run.FailedStepId + "）：" + run.ErrorMessage +
                        (run.RolledBack ? "\r\n已回滚到安装前状态。" : string.Empty));
                }

                return Report(options, true, string.Format(CultureInfo.InvariantCulture,
                    "安装完成。\r\n目录：{0}\r\n文件：{1} 个，快捷方式：{2} 个\r\n日志：{3}",
                    installDir, context.InstalledFiles.Count, context.CreatedShortcuts.Count, logger is FileLogger ? "见安装目录" : "-"));
            }
        }

        // ─────────────────────────────────────────────────────────────
        // 卸载
        // ─────────────────────────────────────────────────────────────

        private static int RunUninstall(RuntimeOptions options, string[] args)
        {
            var selfPath = GetSelfPath();
            var installDir = Path.GetDirectoryName(selfPath);

            var record = UninstallEngine.LoadRecord(installDir);

            // 安装记录丢了（比如之前卸载到一半、或者被重复安装覆盖过）——
            // 不能直接报错退出，否则用户在控制面板里点"卸载"就永远没反应。
            if (record == null)
            {
                return RunDegradedUninstall(options, selfPath, installDir);
            }

            // 卸载 HKLM 里的东西同样需要管理员
            if (!options.AlreadyElevated && !IsElevated() && NeedsElevationForUninstall(record))
            {
                return RelaunchElevated(options, args);
            }

            // 沙箱根：命令行优先，其次用安装记录里记下的 ——
            // 否则沙箱安装会被当成真实安装去动真实注册表。
            if (string.IsNullOrEmpty(options.SandboxRoot))
            {
                options.SandboxRoot = record.SandboxRoot;
            }

            var logger = CreateLogger(options, installDir);
            var platform = CreatePlatform(options, logger);

            // 非静默 → 走向导界面
            if (!options.Silent)
            {
                return RunUninstallWizard(options, selfPath, record, installDir);
            }

            logger.Info("=== 开始卸载 " + record.ProductCode + " ← " + record.InstallDir + " ===");

            var progress = options.Silent
                ? new Progress<InstallProgress>(p => Console.WriteLine(
                    string.Format(CultureInfo.InvariantCulture, "  [{0,3}] {1}", p.Percent, p.Message)))
                : null;

            var result = new UninstallEngine().Run(record, platform, progress);

            WriteResult(options.ResultPath, new
            {
                ok = result.Success,
                mode = "uninstall",
                installDir = record.InstallDir,
                productCode = record.ProductCode,
                deletedFiles = result.DeletedFiles,
                deletedShortcuts = result.DeletedShortcuts,
                modifiedFiles = result.ModifiedFiles,
                issues = result.Issues,
                requiresRestart = result.RequiresRestart,
            });

            // 卸载器自己还在安装目录里，删不掉自己 —— 复制到 %TEMP% 让副本收尾
            if (result.Success)
            {
                try
                {
                    ScheduleSelfCleanup(record.InstallDir, selfPath);
                }
                catch (Exception ex)
                {
                    logger.Warn("安排自删除失败（安装目录可能残留 Uninstall.exe）：" + ex.Message);
                }
            }

            return Report(options, result.Success, string.Format(CultureInfo.InvariantCulture,
                "卸载完成。\r\n删除文件：{0} 个，快捷方式：{1} 个" +
                (result.ModifiedFiles.Count > 0 ? "\r\n注意：有 {2} 个文件在安装后被修改过，已按记录删除。" : string.Empty) +
                (result.Issues.Count > 0 ? "\r\n问题 {3} 条，详见结果文件。" : string.Empty),
                result.DeletedFiles, result.DeletedShortcuts, result.ModifiedFiles.Count, result.Issues.Count));
        }

        /// <summary>
        /// 降级卸载：安装记录已经不存在了，但仍然要把这个产品从系统里清干净。
        ///
        /// 定位方式改为"卸载键里写的安装目录"——也就是卸载器自己所在的目录。
        /// 这样控制面板里那些**指向同一个目录的多余条目**会一次性全部清掉，
        /// 而不是像以前那样点一下没反应。
        /// </summary>
        private static int RunDegradedUninstall(RuntimeOptions options, string selfPath, string installDir)
        {
            var logger = CreateLogger(options, installDir);
            var platform = CreatePlatform(options, logger);

            logger.Warn("找不到安装记录（" + InstallRecord.GetPath(installDir) + "），进入降级卸载。");

            if (!options.Silent)
            {
                var name = FindDisplayName(platform, installDir, options.ProductCode) ?? "这个程序";
                var answer = System.Windows.Forms.MessageBox.Show(
                    "找不到安装记录，但会把「" + name + "」从系统中移除：\r\n\r\n" +
                    "• 清掉控制面板里的卸载项\r\n" +
                    "• 删掉快捷方式\r\n" +
                    "• 删掉安装目录里的残留文件\r\n\r\n" +
                    "（你自己放进安装目录的文件会保留）\r\n\r\n继续吗？",
                    "卸载确认", System.Windows.Forms.MessageBoxButtons.OKCancel,
                    System.Windows.Forms.MessageBoxIcon.Warning);

                if (answer != System.Windows.Forms.DialogResult.OK)
                {
                    return 1;
                }
            }

            var progress = options.Silent
                ? new Progress<InstallProgress>(p => Console.WriteLine(
                    string.Format(CultureInfo.InvariantCulture, "  [{0,3}] {1}", p.Percent, p.Message)))
                : null;

            var result = new UninstallEngine().RunWithoutRecord(
                installDir, platform, options.ProductCode, progress);

            WriteResult(options.ResultPath, new
            {
                ok = result.Success,
                mode = "uninstall-degraded",
                installDir = installDir,
                productCode = options.ProductCode,
                deletedFiles = result.DeletedFiles,
                deletedShortcuts = result.DeletedShortcuts,
                issues = result.Issues,
                requiresRestart = result.RequiresRestart,
            });

            if (result.Success)
            {
                try
                {
                    ScheduleSelfCleanup(installDir, selfPath);
                }
                catch (Exception ex)
                {
                    logger.Warn("安排自删除失败：" + ex.Message);
                }
            }

            return Report(options, result.Success, string.Format(CultureInfo.InvariantCulture,
                "已清理完成（没有安装记录，按卸载键定位）。\r\n" +
                "删除文件：{0} 个，快捷方式：{1} 个" +
                (result.Issues.Count > 0 ? "\r\n问题 {2} 条，详见结果文件。" : string.Empty),
                result.DeletedFiles, result.DeletedShortcuts, result.Issues.Count));
        }

        /// <summary>从卸载键里找出这个安装目录对应的显示名。</summary>
        private static string FindDisplayName(Installer.Abstractions.Platform.IPlatformServices platform,
                                              string installDir, string productCode)
        {
            var byCode = Installer.Core.Install.UninstallKeys.FindByProductCode(platform.Registry, productCode);
            if (byCode != null && !string.IsNullOrWhiteSpace(byCode.DisplayName))
            {
                return byCode.DisplayName;
            }

            var byDir = Installer.Core.Install.UninstallKeys.FindByInstallLocation(platform.Registry, installDir);
            return byDir.Count == 0 ? null : byDir[0].DisplayName;
        }

        // ─────────────────────────────────────────────────────────────
        // 提权
        // ─────────────────────────────────────────────────────────────

        /// <summary>
        /// 这次安装是否需要管理员权限。
        /// 「所有用户」要写 <c>HKLM</c>，而 stub 声明的是 <c>asInvoker</c> —— 不提权就必然被拒。
        /// </summary>
        private static bool NeedsElevation(InstallerManifest manifest)
        {
            return manifest != null
                   && string.Equals(manifest.Scope, InstallScopes.PerMachine, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>卸载是否需要管理员（卸载键在 HKLM，或安装范围是所有用户）。</summary>
        private static bool NeedsElevationForUninstall(InstallRecord record)
        {
            if (record == null)
            {
                return false;
            }

            if (string.Equals(record.Scope, InstallScopes.PerMachine, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            var key = record.UninstallKeyPath ?? string.Empty;
            return key.IndexOf("HKLM", StringComparison.OrdinalIgnoreCase) >= 0
                   || key.IndexOf("LocalMachine", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        /// <summary>当前进程是不是管理员。</summary>
        private static bool IsElevated()
        {
            try
            {
                var id = System.Security.Principal.WindowsIdentity.GetCurrent();
                var principal = new System.Security.Principal.WindowsPrincipal(id);
                return principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>把原参数重新拼一遍，并追加 <c>--elevated</c>（防无限重启）。</summary>
        private static string RebuildArguments(string[] args)
        {
            var list = new List<string>();

            foreach (var a in args)
            {
                if (string.Equals(a, "--elevated", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                list.Add(a.IndexOf(' ') >= 0 ? "\"" + a + "\"" : a);
            }

            list.Add("--elevated");
            return string.Join(" ", list.ToArray());
        }

        /// <summary>
        /// 以管理员身份重启自己。
        /// 提权后的新实例会继续安装，当前实例直接退出 —— 用户只需要点一次 UAC。
        /// </summary>
        private static int RelaunchElevated(RuntimeOptions options, string[] args)
        {
            var selfPath = GetSelfPath();

            var psi = new ProcessStartInfo
            {
                FileName = selfPath,
                Arguments = RebuildArguments(args),
                UseShellExecute = true,          // runas 必须走 ShellExecute
                Verb = "runas",
                WorkingDirectory = Path.GetDirectoryName(selfPath),
            };

            try
            {
                Process.Start(psi);
                return 0;
            }
            catch (System.ComponentModel.Win32Exception)
            {
                // 用户在 UAC 上点了「否」
                return Report(options, false,
                    "安装到「所有用户」需要管理员权限，但 UAC 授权被取消了。\r\n\r\n" +
                    "如果不想每次都提权：在制作端的「高级选项 → 安装细节」里把安装范围改成" +
                    "「仅当前用户」，这样安装和卸载都不需要管理员。");
            }
        }

        /// <summary>把卸载器复制到 %TEMP%，由副本删掉安装目录残留（含本 exe），再自删。</summary>
        private static void ScheduleSelfCleanup(string installDir, string selfPath)        {
            var tempDir = Path.Combine(Path.GetTempPath(), "installer-cleanup-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDir);

            var copy = Path.Combine(tempDir, "cleanup.exe");
            File.Copy(selfPath, copy, true);

            var pid = Process.GetCurrentProcess().Id;
            var args = "--cleanup \"" + installDir + "\" --wait-pid " + pid;

            Process.Start(new ProcessStartInfo
            {
                FileName = copy,
                Arguments = args,
                UseShellExecute = false,
                CreateNoWindow = true,
            });
        }

        /// <summary>清理模式：等父进程退出 → 清空安装目录 → 自删除。</summary>
        private static int RunCleanup(RuntimeOptions options)
        {
            if (options.WaitPid > 0)
            {
                try
                {
                    using (var p = Process.GetProcessById(options.WaitPid))
                    {
                        p.WaitForExit(15000);
                    }
                }
                catch (ArgumentException)
                {
                }
            }

            System.Threading.Thread.Sleep(300);

            // 只删**卸载器自己留下的**残留物。
            // 绝不递归删整个目录 —— 那是现状 FrmUnInstall.cs:232-257 的做法，
            // 会把用户放在安装目录里的数据一并抹掉。真正属于程序的文件由
            // UninstallEngine 按安装记录精确删除，这里只负责"运行中的 exe 删不掉自己"。
            var dir = options.CleanupDir;
            if (Directory.Exists(dir))
            {
                var leftovers = new[]
                {
                    Path.Combine(dir, "Uninstall.exe"),
                    InstallRecord.GetPath(dir),
                    Path.Combine(dir, ".install.log"),
                    Path.Combine(dir, ".install-journal.jsonl"),
                };

                foreach (var file in leftovers)
                {
                    TryDeleteFile(file);
                }

                var rollbackDir = Path.Combine(dir, ".rollback");
                if (Directory.Exists(rollbackDir))
                {
                    foreach (var f in Directory.GetFiles(rollbackDir))
                    {
                        TryDeleteFile(f);
                    }

                    try
                    {
                        Directory.Delete(rollbackDir, false);
                    }
                    catch (IOException)
                    {
                    }
                }

                // 自底向上删空目录（非空的一律保留）
                foreach (var sub in Directory.GetDirectories(dir, "*", SearchOption.AllDirectories)
                                             .OrderByDescending(d => d.Length))
                {
                    try
                    {
                        if (Directory.GetFileSystemEntries(sub).Length == 0)
                        {
                            Directory.Delete(sub, false);
                        }
                    }
                    catch (IOException)
                    {
                    }
                }

                try
                {
                    if (Directory.GetFileSystemEntries(dir).Length == 0)
                    {
                        Directory.Delete(dir, false);
                    }
                }
                catch (IOException)
                {
                }
            }

            // 副本自己删不掉自己 → 安排重启后删除（唯一可靠的托管自删除方式）
            var self = GetSelfPath();
            MoveFileEx(self, null, MOVEFILE_DELAY_UNTIL_REBOOT);

            return 0;
        }

        private static void TryDeleteFile(string path)
        {
            try
            {
                if (!File.Exists(path))
                {
                    return;
                }

                var attrs = File.GetAttributes(path);
                if ((attrs & FileAttributes.ReadOnly) != 0)
                {
                    File.SetAttributes(path, attrs & ~FileAttributes.ReadOnly);
                }

                File.Delete(path);
            }
            catch (IOException)
            {
            }
            catch (UnauthorizedAccessException)
            {
            }
        }

        private const int MOVEFILE_DELAY_UNTIL_REBOOT = 0x4;

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool MoveFileEx(string existingFileName, string newFileName, int flags);

        // ─────────────────────────────────────────────────────────────
        // 向导界面
        // ─────────────────────────────────────────────────────────────

        /// <summary>走安装向导。</summary>
        private static int RunWizard(RuntimeOptions options, string selfPath,
                                     InstallerManifest manifest, string installDir)
        {
            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);

            var host = new WizardHostImpl(selfPath, options.SandboxRoot, options.LogPath, options.Culture,
                manifest, installDir, false);

            using (var form = new Installer.UI.Forms.WizardForm(manifest, host, installDir, options.Culture))
            {
                System.Windows.Forms.Application.Run(form);
            }

            WriteResult(options.ResultPath, new
            {
                ok = host.ExitCode == 0,
                mode = "install-wizard",
                installDir = installDir,
                sandbox = options.SandboxRoot,
                productCode = manifest.ProductCode,
                exitCode = host.ExitCode,
            });

            return host.ExitCode;
        }

        /// <summary>走卸载向导。</summary>
        private static int RunUninstallWizard(RuntimeOptions options, string selfPath,
                                              InstallRecord record, string installDir)
        {
            // 卸载器是裸 stub（没有容器），所以从安装记录拼一个最小清单给向导用
            var manifest = new InstallerManifest
            {
                SchemaVersion = 2,
                ProductCode = record.ProductCode,
                DefaultCulture = options.Culture ?? "zh-Hans",
                Product = new ProductInfo
                {
                    Name = new LocalizedText { { "zh-Hans", record.ProductName ?? record.ProductCode } },
                    Version = record.ProductVersion,
                },
                Scope = record.Scope ?? InstallScopes.PerUser,
                Files = new List<FileEntry>(),
            };

            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);

            var host = new WizardHostImpl(selfPath, options.SandboxRoot, options.LogPath, options.Culture,
                manifest, installDir, true, options.ProductCode);

            using (var form = new Installer.UI.Forms.WizardForm(manifest, host, installDir,
                       options.Culture, uninstallMode: true))
            {
                System.Windows.Forms.Application.Run(form);
            }

            WriteResult(options.ResultPath, new
            {
                ok = host.ExitCode == 0,
                mode = "uninstall-wizard",
                installDir = installDir,
                productCode = record.ProductCode,
                exitCode = host.ExitCode,
            });

            return host.ExitCode;
        }

        /// <summary>
        /// 把向导的每个页面渲染成 PNG。
        /// 这是给开发/验收用的**视觉检查**入口（不执行安装），打包后的 exe 也能用。
        /// </summary>
        private static int RenderUi(RuntimeOptions options)
        {
            var selfPath = GetSelfPath();

            using (var self = File.OpenRead(selfPath))
            {
                var info = ContainerFormat.Read(self);
                var manifest = ContainerFormat.ReadManifest(self, info);

                var text = new Installer.Abstractions.Platform.StringTable(
                    Installer.UI.Common.WizardStrings.Merge(manifest.Strings),
                    options.Culture ?? Installer.Abstractions.Platform.StringTable.DetectRequestedCulture(),
                    manifest.DefaultCulture);

                var productName = text.Resolve(manifest.Product.Name, manifest.ProductCode);
                var installDir = ResolveInstallDir(options, manifest, CreatePlatform(options), productName);

                System.Windows.Forms.Application.EnableVisualStyles();
                System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);

                var host = new WizardHostImpl(selfPath, options.SandboxRoot, options.LogPath, options.Culture,
                    manifest, installDir, false);

                using (var form = new Installer.UI.Forms.WizardForm(manifest, host, installDir, options.Culture))
                {
                    form.ShowInTaskbar = false;
                    form.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
                    form.Location = new System.Drawing.Point(-4000, -4000);
                    form.Show();
                    System.Windows.Forms.Application.DoEvents();

                    form.RenderPagesToPng(options.RenderUiDir);

                    // 卸载向导也渲染一份
                    using (var un = new Installer.UI.Forms.WizardForm(manifest, host, installDir,
                               options.Culture, uninstallMode: true))
                    {
                        un.ShowInTaskbar = false;
                        un.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
                        un.Location = new System.Drawing.Point(-4000, -4000);
                        un.Show();
                        System.Windows.Forms.Application.DoEvents();

                        un.RenderPagesToPng(Path.Combine(options.RenderUiDir, "uninstall"));
                        un.Close();
                    }

                    form.Close();
                }
            }

            Console.WriteLine("已渲染到 " + options.RenderUiDir);
            return 0;
        }

        // ─────────────────────────────────────────────────────────────
        // 组装
        // ─────────────────────────────────────────────────────────────

        private static IPlatformServices CreatePlatform(RuntimeOptions options, ILogger logger = null)
        {
            logger = logger ?? NullLogger.Instance;

            if (!string.IsNullOrEmpty(options.SandboxRoot))
            {
                Directory.CreateDirectory(options.SandboxRoot);
                return new SandboxPlatformServices(options.SandboxRoot, logger);
            }

            return new WindowsPlatformServices(logger);
        }

        private static ILogger CreateLogger(RuntimeOptions options, string installDir)
        {
            if (options.Silent && string.IsNullOrEmpty(options.LogPath))
            {
                return new FileLogger(Path.Combine(installDir, ".install.log"), false);
            }

            return new FileLogger(options.LogPath ?? Path.Combine(installDir, ".install.log"), !options.Silent);
        }

        private static string ResolveInstallDir(RuntimeOptions options, InstallerManifest manifest,
                                                IPlatformServices platform, string productName)
        {
            var dir = options.Target;

            if (string.IsNullOrWhiteSpace(dir) && !string.IsNullOrEmpty(options.SandboxRoot))
            {
                dir = Path.Combine(options.SandboxRoot, "Programs", SafePathSanitize(productName));
            }

            if (string.IsNullOrWhiteSpace(dir))
            {
                dir = manifest.DefaultInstallDir;
            }

            if (string.IsNullOrWhiteSpace(dir))
            {
                dir = Path.Combine(platform.Environment.GetFolder(SpecialFolderKind.LocalApplicationData),
                    "Programs", SafePathSanitize(productName));
            }

            return Path.GetFullPath(platform.Environment.ExpandEnvironmentVariables(dir));
        }

        private static string SafePathSanitize(string name)
        {
            return Installer.Core.Security.SafePath.SanitizeFileName(name);
        }

        private static string GetSelfPath()
        {
            var asm = Assembly.GetEntryAssembly();
            if (asm != null && !string.IsNullOrEmpty(asm.Location))
            {
                return asm.Location;
            }

            return Process.GetCurrentProcess().MainModule.FileName;
        }

        private static void WriteResult(string resultPath, object payload)
        {
            if (string.IsNullOrWhiteSpace(resultPath))
            {
                return;
            }

            try
            {
                var dir = Path.GetDirectoryName(Path.GetFullPath(resultPath));
                if (!string.IsNullOrEmpty(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                File.WriteAllText(resultPath, JsonConvert.SerializeObject(payload, Formatting.Indented),
                    new UTF8Encoding(false));
            }
            catch (IOException)
            {
            }
        }

        private static int Report(RuntimeOptions options, bool ok, string message)
        {
            if (options.Silent)
            {
                Console.WriteLine(message);
                return ok ? 0 : 1;
            }

            System.Windows.Forms.MessageBox.Show(message, ok ? "安装" : "安装失败",
                System.Windows.Forms.MessageBoxButtons.OK,
                ok ? System.Windows.Forms.MessageBoxIcon.Information : System.Windows.Forms.MessageBoxIcon.Error);
            return ok ? 0 : 1;
        }

        // ─────────────────────────────────────────────────────────────
        // 参数
        // ─────────────────────────────────────────────────────────────

        private sealed class RuntimeOptions
        {
            public string Target;
            public bool Silent;
            public string ResultPath;
            public string Culture;
            public string SandboxRoot;
            public bool DryRun;
            public bool VerifyOnly;
            public bool Uninstall;
            public string LogPath;
            public string CleanupDir;
            public int WaitPid;
            public string RenderUiDir;
            public bool Repair;

            /// <summary>
            /// 产品标识。卸载键里会把它写进命令行（<c>--product-code</c>），
            /// 这样即使安装记录丢了，卸载器也知道自己该删哪一条卸载项。
            /// </summary>
            public string ProductCode;

            /// <summary>内部用：已经提过权了，别再重启自己（防无限循环）。</summary>
            public bool AlreadyElevated;
        }

        private static RuntimeOptions ParseOptions(string[] args)
        {
            var o = new RuntimeOptions();

            for (var i = 0; i < args.Length; i++)
            {
                var a = args[i];
                switch (a.ToLowerInvariant())
                {
                    case "--silent":
                        o.Silent = true;
                        break;
                    case "--dry-run":
                        o.DryRun = true;
                        break;
                    case "--repair":
                        o.Repair = true;
                        o.Silent = true;
                        break;
                    case "--verify-only":
                        o.VerifyOnly = true;
                        break;
                    case "--uninstall":
                        o.Uninstall = true;
                        break;
                    case "--render-ui":
                        o.RenderUiDir = Next(args, ref i);
                        break;
                    case "--product-code":
                        o.ProductCode = Next(args, ref i);
                        break;
                    case "--elevated":
                        o.AlreadyElevated = true;
                        break;
                    case "--target":
                        o.Target = Next(args, ref i);
                        break;
                    case "--result":
                        o.ResultPath = Next(args, ref i);
                        break;
                    case "--culture":
                        o.Culture = Next(args, ref i);
                        break;
                    case "--sandbox":
                        o.SandboxRoot = Next(args, ref i);
                        break;
                    case "--log":
                        o.LogPath = Next(args, ref i);
                        break;
                    case "--cleanup":
                        o.CleanupDir = Next(args, ref i);
                        o.Silent = true;
                        break;
                    case "--wait-pid":
                        int pid;
                        o.WaitPid = int.TryParse(Next(args, ref i), out pid) ? pid : 0;
                        break;
                }
            }

            return o;
        }

        private static string Next(string[] args, ref int i)
        {
            return i + 1 < args.Length ? args[++i] : null;
        }
    }
}
