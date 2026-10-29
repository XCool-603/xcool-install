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
using Installer.UI.Models;
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
                    // 没有容器 —— 两种完全不同的情况，不能一律报错：
                    //
                    // ① 这是部署在安装目录里的 Uninstall.exe 被**双击**运行了。
                    //    控制面板/设置里点"卸载"时命令行带 --uninstall，但资源管理器
                    //    双击**不带任何参数** —— 于是会掉进安装流程，用户看到的是一句
                    //    莫名其妙的"这个 exe 里没有本程序的安装包容器"（用户实测反馈）。
                    // ② 真的是尚未打包的 stub 模板，或者旧版 v1 安装包。
                    //
                    // 部署出来的卸载器本身就是 stub 的前 N 字节（本来就没有容器），
                    // 所以只能靠"自己叫什么 / 旁边有没有安装记录"来区分。
                    if (LooksLikeDeployedUninstaller(selfPath))
                    {
                        return RunUninstall(options, args);
                    }

                    throw new InvalidDataException(
                        "这个 exe 里没有本程序的安装包容器。\r\n" +
                        "它可能是一个尚未打包的 stub 模板，或者是旧版（v1）安装包。\r\n\r\n" +
                        "如果你是想卸载已安装的程序：请运行安装目录里的 Uninstall.exe，" +
                        "或者从「设置 → 应用」里卸载。");
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

                // 提权必须发生在**进向导之前**，用户只需要点一次 UAC。
                //
                // 判据有两类，缺一不可：
                // ① 「所有用户」/「始终请求管理员权限」—— 要写 HKLM，asInvoker 必被拒；
                // ② 安装目录本身写不进去 —— 用户把目录改到了 Program Files，
                //    以前不判这一条，结果是装到一半才失败（还得回滚）。
                if (!options.AlreadyElevated
                    && ElevationRules.InstallNeedsAdmin(manifest, installDir, IsElevated()))
                {
                    return RelaunchElevated(options, args);
                }

                // 非静默 → 走向导界面
                if (!options.Silent)
                {
                    return RunWizard(options, selfPath, manifest, installDir, LoadResume(options));
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

                // 静默安装的控制台进度：只打**步骤**（Percent >= 0）。
                // 逐文件上报（Percent = -1）在静默模式下没意义，几百行刷屏反而看不清走到哪一步。
                var progress = options.Silent
                    ? new Progress<InstallProgress>(p =>
                    {
                        if (p.Percent < 0)
                        {
                            return;
                        }

                        Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "  [{0,3}] {1}",
                            p.Percent, p.Message));
                    })
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

        /// <summary>
        /// 这个 exe 是不是"部署在安装目录里的卸载器"？
        ///
        /// 判据（满足其一即可）：
        /// ① 文件名就叫 <c>Uninstall.exe</c> —— 安装时 <c>DeployUninstallerStep</c> 就是这么放的；
        /// ② 同目录下存在安装记录 <c>.install-record.json</c> —— 覆盖用户把卸载器改过名的情况。
        ///
        /// 部署出来的卸载器只是 stub 的前 <c>StubLength</c> 字节，**本来就没有容器**，
        /// 所以不能用"有没有容器"来判断，只能用上面这两条。
        /// </summary>
        private static bool LooksLikeDeployedUninstaller(string selfPath)
        {
            try
            {
                if (string.IsNullOrEmpty(selfPath))
                {
                    return false;
                }

                var selfName = Path.GetFileName(selfPath);
                if (string.Equals(selfName, InstallRecord.UninstallerFileName, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(selfName, InstallRecord.MenuUninstallerFileName, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }

                var dir = Path.GetDirectoryName(selfPath);
                return !string.IsNullOrEmpty(dir) && File.Exists(InstallRecord.GetPath(dir));
            }
            catch (Exception)
            {
                return false;
            }
        }

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
                ? new Progress<InstallProgress>(p =>
                {
                    // 只打步骤级进度；逐文件上报（Percent = -1）在静默模式下不刷屏
                    if (p.Percent < 0)
                    {
                        return;
                    }

                    Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "  [{0,3}] {1}",
                        p.Percent, p.Message));
                })
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
                undeletedFiles = result.UndeletedPaths,
                pendingRebootFiles = result.PendingRebootPaths,
                blockedFiles = result.BlockedPaths,
            });

            // 卸载器自己还在安装目录里，删不掉自己 —— 复制到 %TEMP% 让副本收尾。
            //
            // 条件不能只看 Success：被占用而删不掉的文件会进 BlockedPaths，
            // 那种情况 Success 已经是 false（没提权时登记不了"重启后删除"更是必然 false），
            // 但**副本能删掉它们**（它不加载安装目录里的同名 DLL，且等本进程退出后才动手）。
            // 只看 Success 的后果：非提权环境下那个 DLL 和整个产品目录永远留在那里。
            if (result.Success || result.BlockedPaths.Count > 0)
            {
                try
                {
                    ScheduleSelfCleanup(record.InstallDir, selfPath, result.BlockedPaths);
                }
                catch (Exception ex)
                {
                    logger.Warn("安排自删除失败（安装目录可能残留 Uninstall.exe）：" + ex.Message);
                }
            }

            return Report(options, result.Success, string.Format(CultureInfo.InvariantCulture,
                "卸载完成。\r\n删除文件：{0} 个，快捷方式：{1} 个" +
                (result.ModifiedFiles.Count > 0 ? "\r\n注意：有 {2} 个文件在安装后被修改过，已按记录删除。" : string.Empty) +
                (result.PendingRebootPaths.Count > 0
                    ? "\r\n有 {4} 个文件当时被占用，已交给收尾程序在卸载器退出后重试；若仍删不掉，会在下次重启时自动清理。"
                    : string.Empty) +
                (result.UndeletedPaths.Count > 0
                    ? "\r\n有 {5} 个文件被占用且无法安排删除，需要手动清理。"
                    : string.Empty) +
                (result.Issues.Count > 0 ? "\r\n问题 {3} 条，详见结果文件。" : string.Empty),
                result.DeletedFiles, result.DeletedShortcuts, result.ModifiedFiles.Count, result.Issues.Count,
                result.PendingRebootPaths.Count, result.UndeletedPaths.Count));
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

            if (result.Success || result.BlockedPaths.Count > 0)
            {
                try
                {
                    ScheduleSelfCleanup(installDir, selfPath, result.BlockedPaths);
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

        // 判据本身在 Installer.Core.Platform.ElevationRules 里 —— 那里是纯函数，
        // 可以穷举测试（这台验证机 UAC 是关的，弹不出 UAC 就只能靠单测覆盖决策）。
        // 这里只负责"判完之后真的重启自己"。

        /// <summary>卸载是否需要管理员（卸载键在 HKLM、scope 所有用户、或安装目录写不进去）。</summary>
        private static bool NeedsElevationForUninstall(InstallRecord record)
        {
            if (record == null)
            {
                return false;
            }

            // 目录写不进去也算（"仅当前用户"却装到 Program Files 的组合）：
            // 不提权的话删文件会全部失败，用户看到的是"卸载成功但文件还在"。
            var writable = ElevationProbe.CanWrite(record.InstallDir);

            return ElevationRules.UninstallNeedsAdmin(record.Scope, record.UninstallKeyPath, writable);
        }

        /// <summary>当前进程是不是管理员。</summary>
        internal static bool IsElevated()
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
            var ok = StartElevated(selfPath, RebuildArguments(args));

            if (ok)
            {
                return 0;
            }

            // 用户在 UAC 上点了「否」
            return Report(options, false,
                "这次安装需要管理员权限，但 UAC 授权被取消了。\r\n\r\n" +
                "如果不想每次都提权：在制作端的「高级选项 → 安装细节」里把安装范围改成" +
                "「仅当前用户」，并把默认安装目录留在 %LOCALAPPDATA% 下面，" +
                "这样安装和卸载都不需要管理员。");
        }

        /// <summary>
        /// 以管理员身份启动 <paramref name="selfPath"/>。
        /// </summary>
        /// <returns>启动成功返回 true；用户在 UAC 上点了「否」返回 false。</returns>
        private static bool StartElevated(string selfPath, string arguments)
        {
            var psi = new ProcessStartInfo
            {
                FileName = selfPath,
                Arguments = arguments,
                UseShellExecute = true,          // runas 必须走 ShellExecute
                Verb = "runas",
                WorkingDirectory = Path.GetDirectoryName(selfPath),
            };

            try
            {
                Process.Start(psi);
                return true;
            }
            catch (System.ComponentModel.Win32Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 向导里发现"用户这次的选择需要管理员"时调用：把自己重启成管理员，
        /// 并把用户已经做过的选择写进续跑文件，让新实例直接从「安装位置」页接着走。
        ///
        /// 以前不做这一步的后果：用户在向导里勾了「为所有用户安装」、或者把目录
        /// 改到 Program Files，安装照样以普通权限开始 —— 进度条跑到一半才失败。
        /// </summary>
        /// <returns>启动成功返回 true；UAC 被拒返回 false（调用方应留在原页面）。</returns>
        internal static bool RestartElevatedForWizard(string selfPath, WizardOptions wizardOptions,
                                                      string logPath, string culture, string sandboxRoot)
        {
            var resumePath = Path.Combine(Path.GetTempPath(),
                "installer-resume-" + Guid.NewGuid().ToString("N") + ".json");

            try
            {
                File.WriteAllText(resumePath,
                    JsonConvert.SerializeObject(wizardOptions, Formatting.Indented),
                    new UTF8Encoding(false));
            }
            catch (Exception)
            {
                // 写不了续跑文件就别提权了：提权后用户要从头再点一遍，体验更差。
                return false;
            }

            var parts = new List<string> { "--resume " + Quote(resumePath) };

            if (!string.IsNullOrWhiteSpace(wizardOptions.InstallDir))
            {
                parts.Add("--target " + Quote(wizardOptions.InstallDir));
            }

            if (!string.IsNullOrWhiteSpace(logPath))
            {
                parts.Add("--log " + Quote(logPath));
            }

            if (!string.IsNullOrWhiteSpace(culture))
            {
                parts.Add("--culture " + culture);
            }

            if (!string.IsNullOrWhiteSpace(sandboxRoot))
            {
                parts.Add("--sandbox " + Quote(sandboxRoot));
            }

            parts.Add("--elevated");

            if (StartElevated(selfPath, string.Join(" ", parts.ToArray())))
            {
                return true;
            }

            try
            {
                File.Delete(resumePath);
            }
            catch (Exception)
            {
            }

            return false;
        }

        /// <summary>命令行参数里带空格的加引号。</summary>
        private static string Quote(string value)
        {
            return value.IndexOf(' ') >= 0 ? "\"" + value + "\"" : value;
        }

        /// <summary>
        /// 读取"提权后重启"留下的续跑文件（**读完即删**，不留在 %TEMP% 里）。
        /// 解析失败就当没有 —— 大不了用户从头再走一遍向导，绝不能因此装不上。
        /// </summary>
        private static WizardOptions LoadResume(RuntimeOptions options)
        {
            var path = options.ResumePath;
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
            {
                return null;
            }

            try
            {
                var json = File.ReadAllText(path, Encoding.UTF8);
                return JsonConvert.DeserializeObject<WizardOptions>(json);
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                try
                {
                    File.Delete(path);
                }
                catch (Exception)
                {
                }
            }
        }

        /// <summary>
        /// 把卸载器复制到 %TEMP%，由副本删掉安装目录残留（含本 exe 自己），再自删。
        /// </summary>
        /// <param name="retryPaths">
        /// 本进程删不掉的绝对路径，交给副本在本进程退出后重试。
        ///
        /// 最典型的是安装目录里的 <c>Newtonsoft.Json.dll</c>：卸载器本身运行在安装目录下，
        /// .NET 探测程序集时会优先命中"应用程序基目录"里的同名 DLL，于是把用户 App 的那份
        /// 加载进了自己的进程 —— 卸载时去删它，锁的持有者恰恰就是卸载器本人，必然失败。
        /// 副本从 %TEMP% 启动，不会加载安装目录的 DLL，而且等本进程退出后才动手，
        /// 那时锁已释放，一次就能删掉（不必等到重启）。
        /// </param>
        private static void ScheduleSelfCleanup(string installDir, string selfPath,
                                               IEnumerable<string> retryPaths = null)
        {
            var tempDir = Path.Combine(Path.GetTempPath(), "installer-cleanup-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDir);

            var copy = Path.Combine(tempDir, "cleanup.exe");
            File.Copy(selfPath, copy, true);

            var pid = Process.GetCurrentProcess().Id;
            var args = "--cleanup \"" + installDir + "\" --wait-pid " + pid;

            if (retryPaths != null)
            {
                var list = retryPaths.Where(p => !string.IsNullOrEmpty(p))
                                     .Distinct(StringComparer.OrdinalIgnoreCase)
                                     .ToList();
                if (list.Count > 0)
                {
                    // 清单走文件而不是命令行：命令行有长度上限，中文路径也更容易踩引号坑
                    var pending = Path.Combine(tempDir, "pending.txt");
                    File.WriteAllLines(pending, list.ToArray(), new UTF8Encoding(false));
                    args += " --pending \"" + pending + "\"";
                }
            }

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

            // 这一步只负责"运行中的 exe 删不掉自己"以及它带出来的锁。
            // 整个产品目录的清空是 UninstallEngine.WipeProductDirectory 在卸载时做的
            // （安装目录已经分层成「根\厂商\产品」，是我们独占的目录）；
            // 这里等主进程退出后把**剩下的空目录/厂商目录**收干净，作为第二道防线。
            var dir = options.CleanupDir;
            if (Directory.Exists(dir))
            {
                var leftovers = new[]
                {
                    Path.Combine(dir, InstallRecord.UninstallerFileName),
                    Path.Combine(dir, InstallRecord.MenuUninstallerFileName),
                    InstallRecord.GetPath(dir),
                    Path.Combine(dir, ".install.log"),
                    Path.Combine(dir, ".install-journal.jsonl"),
                };

                foreach (var file in leftovers)
                {
                    TryDeleteFile(file);
                }

                // 运行时产生的日志目录不在安装清单里，卸载引擎删它时可能被占用。
                // 这里是第二道防线：等主程序退出之后再整棵删一次。
                var logDir = Path.Combine(dir, "Log");
                if (Directory.Exists(logDir))
                {
                    try
                    {
                        Directory.Delete(logDir, true);
                    }
                    catch (IOException)
                    {
                    }
                    catch (UnauthorizedAccessException)
                    {
                    }
                }

                // 卸载器**本进程**占着的文件（典型：它从安装目录加载进来的 Newtonsoft.Json.dll）。
                // 现在本进程已经退出，锁随之释放 —— 这一次就能删干净，不必等到重启。
                if (!string.IsNullOrEmpty(options.PendingPath) && File.Exists(options.PendingPath))
                {
                    foreach (var line in File.ReadAllLines(options.PendingPath))
                    {
                        var target = line.Trim();
                        if (target.Length == 0)
                        {
                            continue;
                        }

                        if (Directory.Exists(target))
                        {
                            try
                            {
                                Directory.Delete(target, true);
                            }
                            catch (IOException)
                            {
                            }
                            catch (UnauthorizedAccessException)
                            {
                            }

                            continue;
                        }

                        TryDeleteFile(target);
                    }
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
                if (Directory.Exists(dir))
                {
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

            // 产品目录已经清空了 —— 顺手把空出来的**厂商目录**也回收掉，
            // 否则每卸一个产品就在 Program Files 下留一个空壳。
            //
            // 注意：这段必须放在 `if (Directory.Exists(dir))` **外面**。
            // 产品目录常常是上面的 --pending 循环（本进程删不掉、交给副本重试的那批路径）
            // 才删掉的，那时 `Directory.Exists(dir)` 已经是 false，
            // 写在里面就永远不会执行 —— 实测就是这样留下一层空的厂商目录。
            if (!string.IsNullOrEmpty(dir))
            {
                try
                {
                    var parent = Path.GetDirectoryName(dir.TrimEnd('\\', '/'));
                    if (!string.IsNullOrEmpty(parent) && Directory.Exists(parent) &&
                        !Installer.Abstractions.Model.InstallPaths.IsSharedRoot(parent) &&
                        Directory.GetFileSystemEntries(parent).Length == 0)
                    {
                        Directory.Delete(parent, false);
                    }
                }
                catch (IOException)
                {
                }
                catch (UnauthorizedAccessException)
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
                                     InstallerManifest manifest, string installDir,
                                     WizardOptions resume = null)
        {
            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);

            var host = new WizardHostImpl(selfPath, options.SandboxRoot, options.LogPath, options.Culture,
                manifest, installDir, false);

            var relaunched = false;

            using (var form = new Installer.UI.Forms.WizardForm(manifest, host, installDir, options.Culture))
            {
                // 已经装过同一产品 → 走「更新」流程：跳过「安装设置」页，
                // 改显示「当前已安装 X / 将要安装 Y」，按钮变「更新 / 重新安装 / 仍要安装」。
                var installed = DetectInstalled(options, manifest, installDir);
                if (installed != null)
                {
                    form.SetInstalled(installed);
                }

                // 提权重启后的续跑：把上次的选择原样恢复，并直接停在「安装位置」页。
                // 用户已经确认过许可协议、勾过选项，不能让他重来一遍。
                if (resume != null)
                {
                    form.ApplyResume(resume);
                }

                System.Windows.Forms.Application.Run(form);
                relaunched = form.RelaunchedForElevation;
            }

            WriteResult(options.ResultPath, new
            {
                // 提权让位不算"安装成功"—— 真正干活的是新启动的那个管理员实例
                ok = !relaunched && host.ExitCode == 0,
                mode = relaunched ? "install-wizard-elevating" : "install-wizard",
                installDir = installDir,
                sandbox = options.SandboxRoot,
                productCode = manifest.ProductCode,
                exitCode = host.ExitCode,
                elevating = relaunched,
            });

            return host.ExitCode;
        }

        /// <summary>
        /// 找"这台电脑上已经装着的同一个产品"。
        ///
        /// 两条路，按可靠性排序：
        /// <list type="number">
        /// <item>目标目录里的 <c>.install-record.json</c> —— 最准，版本和目录都在里面；</item>
        /// <item>标准卸载键（按 productCode）的 <c>InstallLocation</c> —— 用户上次装到了别的目录、
        ///       或者新包的默认目录变了时靠它兜住。</item>
        /// </list>
        /// 找不到返回 null（全新安装）。
        /// </summary>
        private static Installer.UI.Models.InstalledProduct DetectInstalled(
            RuntimeOptions options, InstallerManifest manifest, string installDir)
        {
            var local = TryReadInstalled(installDir);
            if (local != null)
            {
                return local;
            }

            var code = manifest == null ? null : manifest.ProductCode;
            if (string.IsNullOrWhiteSpace(code))
            {
                return null;
            }

            try
            {
                var platform = CreatePlatform(options);
                var sub = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\" + code;

                foreach (var hive in new[] { RegistryRoot.CurrentUser, RegistryRoot.LocalMachine })
                {
                    if (!platform.Registry.SubKeyExists(hive, sub))
                    {
                        continue;
                    }

                    var location = platform.Registry.GetString(hive, sub, "InstallLocation");
                    if (string.IsNullOrWhiteSpace(location))
                    {
                        continue;
                    }

                    var fromRecord = TryReadInstalled(location);
                    if (fromRecord != null)
                    {
                        return fromRecord;
                    }

                    var version = platform.Registry.GetString(hive, sub, "DisplayVersion");
                    if (!string.IsNullOrWhiteSpace(version))
                    {
                        return new Installer.UI.Models.InstalledProduct
                        {
                            Version = version,
                            InstallDir = location,
                        };
                    }
                }
            }
            catch (Exception)
            {
                // 读注册表失败不影响安装：按全新安装处理
            }

            return null;
        }

        /// <summary>读某个目录里的安装记录，转成"已安装产品"。</summary>
        internal static Installer.UI.Models.InstalledProduct TryReadInstalled(string dir)
        {
            if (string.IsNullOrWhiteSpace(dir))
            {
                return null;
            }

            try
            {
                var record = UninstallEngine.LoadRecord(dir);
                if (record == null || string.IsNullOrWhiteSpace(record.ProductVersion))
                {
                    return null;
                }

                return new Installer.UI.Models.InstalledProduct
                {
                    Version = record.ProductVersion,
                    InstallDir = string.IsNullOrWhiteSpace(record.InstallDir) ? dir : record.InstallDir,
                };
            }
            catch (Exception)
            {
                return null;
            }
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

            // 向导跑完之后，卸载器自己（就是本进程）还躺在安装目录里 —— 进程删不掉自己的
            // 镜像文件。必须在这里安排自清理：把副本复制到 %TEMP%，由副本等本进程退出后
            // 删掉安装目录里的 Uninstall.exe。
            //
            // 以前这条路径**整个漏了这一步**：安静卸载（Uninstall）里有，
            // 向导卸载（RunUninstallWizard）里没有。于是 e2e 走 --silent 全绿，
            // 而用户在控制面板点"卸载"（走向导）就会永久残留 Uninstall.exe。
            var cleanupScheduled = false;
            if (host.UninstallCompleted || host.BlockedPaths.Count > 0)
            {
                try
                {
                    ScheduleSelfCleanup(installDir, selfPath, host.BlockedPaths);
                    cleanupScheduled = true;
                }
                catch (Exception)
                {
                    // 自清理失败不影响卸载本身，但必须如实写进结果文件 ——
                    // 它意味着 Uninstall.exe 会留在安装目录里。
                }
            }

            WriteResult(options.ResultPath, new
            {
                ok = host.ExitCode == 0,
                mode = "uninstall-wizard",
                installDir = installDir,
                productCode = record.ProductCode,
                exitCode = host.ExitCode,
                uninstallCompleted = host.UninstallCompleted,
                selfCleanupScheduled = cleanupScheduled,
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

                    // 让首次绘制真正落定再截图。
                    // Show() 之后只泵一次消息时，**第一个被渲染的页面**
                    // （安装向导 = 欢迎，卸载向导 = 确认卸载）可能还没画完，
                    // DrawToBitmap 会抓到未绘制状态 —— 表现就是缺一圈卡片投影、
                    // 底部露出一条平坦的 #F5F7FA 灰带，而后续页面全都正常。
                    System.Threading.Thread.Sleep(400);
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
                        System.Threading.Thread.Sleep(400);
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
                dir = Installer.Abstractions.Model.InstallPaths.Compose(
                    Path.Combine(options.SandboxRoot, "Programs"), VendorNameOf(manifest), productName);
            }

            if (string.IsNullOrWhiteSpace(dir))
            {
                dir = manifest.DefaultInstallDir;
            }

            if (string.IsNullOrWhiteSpace(dir))
            {
                // 清单里没写默认目录（老包 / 手写清单）：落到「根\厂商\产品」，
                // 而不是「根\产品」—— 层级规则只有一份，就是 InstallPaths。
                dir = Installer.Abstractions.Model.InstallPaths.Compose(
                    Installer.Abstractions.Model.InstallPaths.PerUserRoot,
                    VendorNameOf(manifest),
                    productName);
            }

            // 制作机上配的根目录（比如 D:\Program Files）在这台机器上可能根本不存在。
            var expanded = platform.Environment.ExpandEnvironmentVariables(dir);
            expanded = ApplyMissingRootFallback(expanded, manifest, VendorNameOf(manifest), productName, platform);

            return Path.GetFullPath(expanded);
        }

        /// <summary>
        /// 安装根盘符在这台机器上不存在时，退回到系统默认根目录。
        ///
        /// 为什么要这一层：安装包是在**制作机**上生成的。制作机上可能有 D 盘，
        /// 于是根目录被填成 <c>D:\Program Files</c>；装到一台没有 D 盘的机器上，
        /// 这条路根本创建不出来（以前会一路失败到解压那一步，错误信息还看不出原因）。
        ///
        /// 回退目标：
        /// <list type="bullet">
        /// <item>perMachine → <c>%ProgramFiles%</c>（安装器是 x86 进程，展开即
        /// <c>C:\Program Files (x86)</c>，与「所有用户」语义一致）</item>
        /// <item>perUser → <c>%LOCALAPPDATA%\Programs</c></item>
        /// </list>
        /// 厂商 / 产品两层会重新拼上，不会退化成裸根目录。
        /// </summary>
        /// <param name="expandedDir">已展开环境变量的安装目录。</param>
        /// <param name="manifest">清单（只用来判 scope）。</param>
        /// <param name="vendorName">厂商名（可空）。</param>
        /// <param name="productName">产品名。</param>
        /// <param name="platform">平台服务（要环境变量展开与日志）。</param>
        internal static string ApplyMissingRootFallback(string expandedDir, InstallerManifest manifest,
                                                       string vendorName, string productName,
                                                       IPlatformServices platform)
        {
            if (string.IsNullOrWhiteSpace(expandedDir) || platform == null)
            {
                return expandedDir;
            }

            var root = Path.GetPathRoot(expandedDir);
            if (string.IsNullOrEmpty(root) || Directory.Exists(root))
            {
                return expandedDir;
            }

            var allUsers = string.Equals(manifest == null ? null : manifest.Scope, InstallScopes.PerMachine,
                StringComparison.OrdinalIgnoreCase);

            var fallbackRoot = platform.Environment.ExpandEnvironmentVariables(
                Installer.Abstractions.Model.InstallPaths.Root(allUsers));

            platform.Log.Warn(string.Format(CultureInfo.InvariantCulture,
                "安装位置所在磁盘 {0} 不存在，回退到 {1}。", root, fallbackRoot));

            return Installer.Abstractions.Model.InstallPaths.Compose(fallbackRoot, vendorName, productName);
        }

        /// <summary>清单里的厂商名（按默认语言取；没填返回空串 = 不建厂商层级）。</summary>
        private static string VendorNameOf(InstallerManifest manifest)
        {
            if (manifest == null || manifest.Product == null)
            {
                return string.Empty;
            }

            return Installer.Abstractions.Model.InstallPaths.SanitizeOrEmpty(
                Installer.Abstractions.Model.InstallPaths.PickText(
                    manifest.Product.Publisher, null, manifest.DefaultCulture));
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

            /// <summary>--pending 指定的"退出后重试删除"清单文件（每行一个绝对路径）。</summary>
            public string PendingPath;
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

            /// <summary>
            /// 内部用：向导里提权重启时留下的"续跑文件"（JSON 序列化的 <c>WizardOptions</c>）。
            /// 新实例读完即删，然后把用户的选择原样恢复。
            /// </summary>
            public string ResumePath;
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
                    case "--resume":
                        o.ResumePath = Next(args, ref i);
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
                    case "--pending":
                        o.PendingPath = Next(args, ref i);
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
