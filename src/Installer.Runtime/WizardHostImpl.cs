using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Installer.Abstractions.Install;
using Installer.Abstractions.Model;
using Installer.Abstractions.Platform;
using Installer.Core.Install;
using Installer.Core.Packaging;
using Installer.Core.Platform;
using Installer.Core.Uninstall;
using Installer.UI.Models;

namespace Installer.Runtime
{
    /// <summary>
    /// 把 <see cref="Installer.UI.Forms.WizardForm"/> 接到安装引擎上。
    ///
    /// 界面只通过 <see cref="IWizardHost"/> 与引擎通信，所以向导不知道
    /// <c>InstallEngine</c> 的存在，引擎也不知道界面的存在。
    /// </summary>
    internal sealed class WizardHostImpl : IWizardHost
    {
        private readonly string _selfPath;
        private readonly string _sandboxRoot;
        private readonly string _logPath;
        private readonly InstallerManifest _manifest;
        private readonly string _installDir;
        private readonly bool _uninstallMode;
        private readonly string _culture;
        private readonly string _productCode;

        private CancellationTokenSource _cts;

        public WizardHostImpl(string selfPath, string sandboxRoot, string logPath, string culture,
                              InstallerManifest manifest, string installDir, bool uninstallMode,
                              string productCode = null)
        {
            _selfPath = selfPath;
            _sandboxRoot = sandboxRoot;
            _logPath = logPath;
            _culture = culture;
            _manifest = manifest;
            _installDir = installDir;
            _uninstallMode = uninstallMode;
            _productCode = productCode;
        }

        /// <summary>向导关闭后由宿主读取的退出码。</summary>
        public int ExitCode { get; private set; }

        /// <summary>
        /// 真正要装到哪 —— **以清单里的当前值为准**，而不是构造时的快照。
        ///
        /// 用户可能在向导的"安装位置"页改过路径，那时 <c>ApplyToManifest</c> 会把新值写回清单。
        /// 如果这里还用构造时捕获的 <c>_installDir</c>，用户的选择就被无声地丢掉了。
        /// </summary>
        private string EffectiveInstallDir
        {
            get
            {
                var dir = _manifest == null ? null : _manifest.DefaultInstallDir;
                if (string.IsNullOrWhiteSpace(dir))
                {
                    dir = _installDir;
                }

                return string.IsNullOrWhiteSpace(dir) ? dir : Environment.ExpandEnvironmentVariables(dir);
            }
        }

        /// <summary>取消令牌（供引擎回滚）。</summary>
        public CancellationToken Token
        {
            get { return _cts == null ? CancellationToken.None : _cts.Token; }
        }

        // ─────────────────────────────────────────────────────────────

        /// <inheritdoc />
        public void StartInstall(WizardOptions options, Action<InstallProgress> onProgress, Action<bool, string> onDone)
        {
            _cts = new CancellationTokenSource();
            var token = _cts.Token;

            // 用户在向导里的选择已经由 Presenter 写回 manifest（ApplyToManifest）
            Task.Run(() =>
            {
                try
                {
                    using (var self = new FileStream(_selfPath, FileMode.Open, FileAccess.Read, FileShare.Read, 1 << 20))
                    {
                        var info = ContainerFormat.Read(self);
                        var logger = new FileLogger(_logPath, false);
                        var platform = CreatePlatform(logger);

                        // 内置中英文案 + 清单覆盖：与向导界面用的是同一套文案表
                        var text = new StringTable(
                            Installer.UI.Common.WizardStrings.Merge(_manifest.Strings),
                            _culture ?? StringTable.DetectRequestedCulture(),
                            _manifest.DefaultCulture);

                        var installDir = EffectiveInstallDir;

                        var context = new InstallContext
                        {
                            Manifest = _manifest,
                            Mode = InstallMode.Install,
                            InstallDir = installDir,
                            Payload = ContainerFormat.OpenPayload(self, info),
                            StubLength = info.StubLength,
                            CurrentExecutablePath = _selfPath,
                            Platform = platform,
                            Journal = new InstallJournal(
                                Path.Combine(installDir, ".install-journal.jsonl"),
                                Path.Combine(installDir, ".rollback")),
                            Log = logger,
                            Strings = _manifest.Strings,
                            Text = text,
                            SandboxRoot = _sandboxRoot,
                            CancellationToken = token,
                        };

                        var engine = new InstallEngine();
                        var result = engine.Run(context, new ActionProgress(onProgress));

                        ExitCode = result.Success ? 0 : 1;
                        onDone(result.Success, result.Success ? installDir : result.ErrorMessage);
                    }
                }
                catch (Exception ex)
                {
                    ExitCode = 1;
                    onDone(false, ex.Message);
                }
            });
        }

        /// <inheritdoc />
        public void StartUninstall(Action<InstallProgress> onProgress, Action<bool, string> onDone)
        {
            _cts = new CancellationTokenSource();

            Task.Run(() =>
            {
                try
                {
                    var logger = new FileLogger(_logPath, false);
                    var platform = CreatePlatform(logger);

                    var record = UninstallEngine.LoadRecord(_installDir);

                    UninstallRunResult result;

                    if (record == null)
                    {
                        // 安装记录丢了也要能卸掉 —— 否则用户在控制面板里点"卸载"永远没反应
                        logger.Warn("找不到安装记录，进入降级卸载：" + _installDir);
                        result = new UninstallEngine().RunWithoutRecord(
                            _installDir, platform, _productCode, new ActionProgress(onProgress));
                    }
                    else
                    {
                        result = new UninstallEngine().Run(record, platform, new ActionProgress(onProgress));
                    }

                    ExitCode = result.Success ? 0 : 1;
                    onDone(result.Success, result.Success ? string.Empty : string.Join("；", result.Issues.ToArray()));
                }
                catch (Exception ex)
                {
                    ExitCode = 1;
                    onDone(false, ex.Message);
                }
            });
        }

        /// <inheritdoc />
        public void RequestCancel()
        {
            if (_cts != null && !_cts.IsCancellationRequested)
            {
                _cts.Cancel();
            }
        }

        /// <inheritdoc />
        public void LaunchEntryPoint()
        {
            if (string.IsNullOrEmpty(_manifest.EntryPoint))
            {
                return;
            }

            var exe = Path.Combine(EffectiveInstallDir, _manifest.EntryPoint);
            if (!File.Exists(exe))
            {
                return;
            }

            Process.Start(new ProcessStartInfo
            {
                FileName = exe,
                WorkingDirectory = EffectiveInstallDir,
                UseShellExecute = true,
            });
        }

        private IPlatformServices CreatePlatform(ILogger logger)
        {
            if (!string.IsNullOrEmpty(_sandboxRoot))
            {
                Directory.CreateDirectory(_sandboxRoot);
                return new SandboxPlatformServices(_sandboxRoot, logger);
            }

            return new WindowsPlatformServices(logger);
        }

        /// <summary>把 <c>Action</c> 适配成 <see cref="IProgress{T}"/>。</summary>
        private sealed class ActionProgress : IProgress<InstallProgress>
        {
            private readonly Action<InstallProgress> _action;

            public ActionProgress(Action<InstallProgress> action)
            {
                _action = action;
            }

            public void Report(InstallProgress value)
            {
                if (_action != null)
                {
                    _action(value);
                }
            }
        }
    }
}
