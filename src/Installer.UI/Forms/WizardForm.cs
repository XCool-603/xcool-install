using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using Installer.Abstractions.Install;
using Installer.Abstractions.Model;
using Installer.Abstractions.Platform;
using Installer.Core.Install;
using Installer.UI.Common;
using Installer.UI.Models;
using Installer.UI.Presenters;
using Installer.UI.Views;

namespace Installer.UI.Forms
{
    /// <summary>
    /// 安装/卸载向导。
    ///
    /// 现状对照：<c>FrmInstallation</c> 是 777 行、UI 与安装逻辑焊死、用按钮文字当状态、
    /// 三份拷贝（安装端 / 更新端 / 制作端预览各一份）。
    /// 这里是**唯一**一份向导，安装、卸载、制作端预览共用。
    /// </summary>
    public partial class WizardForm : AntdUI.BaseForm
    {
        private readonly WizardPresenter _presenter;
        private readonly IWizardHost _host;
        private readonly InstallerManifest _manifest;
        private readonly bool _uninstallMode;
        private readonly System.Windows.Forms.Timer _closeTimer;

        private WelcomeView _welcomeView;
        private LicenseView _licenseView;
        private PathView _pathView;
        private OptionsView _optionsView;
        private ProgressView _progressView;
        private FinishView _finishView;
        private UserControl _currentView;
        private bool _finished;

        /// <summary>构造。</summary>
        /// <param name="manifest">清单。</param>
        /// <param name="host">安装引擎桥。</param>
        /// <param name="defaultInstallDir">已解析的默认安装目录。</param>
        /// <param name="culture">界面语言；null 表示跟随系统。</param>
        /// <param name="uninstallMode">是否为卸载模式。</param>
        public WizardForm(InstallerManifest manifest, IWizardHost host, string defaultInstallDir,
                          string culture = null, bool uninstallMode = false)
        {
            if (manifest == null)
            {
                throw new ArgumentNullException("manifest");
            }

            if (host == null)
            {
                throw new ArgumentNullException("host");
            }

            _manifest = manifest;
            _host = host;
            _uninstallMode = uninstallMode;

            // 内置中英文案 + 清单覆盖 —— 于是"支持英文"是默认行为，不需要制作端做任何事
            var strings = WizardStrings.Merge(manifest.Strings);
            var table = new StringTable(strings, culture ?? StringTable.DetectRequestedCulture(), manifest.DefaultCulture);

            _presenter = new WizardPresenter(manifest, table, defaultInstallDir);

            InitializeComponent();

            ApplyAppearance();
            BuildViews();

            _closeTimer = new System.Windows.Forms.Timer { Interval = 900 };
            _closeTimer.Tick += CloseTimerTick;

            if (_uninstallMode)
            {
                _presenter.EnterUninstallConfirm();
            }

            Render();
        }

        // ─────────────────────────────────────────────────────────────
        // 外观（全部来自清单，不硬编码）
        // ─────────────────────────────────────────────────────────────

        private void ApplyAppearance()
        {
            Text = _presenter.ProductName + " " + _presenter.ProductVersion;

            lblProduct.Text = _presenter.ProductName;
            lblVersion.Text = _presenter.ProductVersion;

            var ui = _manifest.Ui;
            if (ui != null && !string.IsNullOrEmpty(ui.AccentColor))
            {
                Color accent;
                if (TryParseColor(ui.AccentColor, out accent))
                {
                    pnlHeader.BackColor = accent;
                }
            }

            if (ui != null && string.IsNullOrEmpty(ui.AccentColor))
            {
                pnlHeader.BackColor = Color.FromArgb(6, 157, 231);
            }

            if (ui != null && !string.IsNullOrEmpty(ui.Theme) &&
                string.Equals(ui.Theme, "dark", StringComparison.OrdinalIgnoreCase))
            {
                AntdUI.Config.Mode = AntdUI.TMode.Dark;
            }
        }

        private static bool TryParseColor(string hex, out Color color)
        {
            color = Color.Empty;
            if (string.IsNullOrEmpty(hex))
            {
                return false;
            }

            var s = hex.TrimStart('#');
            if (s.Length != 6 && s.Length != 8)
            {
                return false;
            }

            int v;
            if (!int.TryParse(s, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out v))
            {
                return false;
            }

            color = s.Length == 6
                ? Color.FromArgb(255, (v >> 16) & 0xFF, (v >> 8) & 0xFF, v & 0xFF)
                : Color.FromArgb((v >> 24) & 0xFF, (v >> 16) & 0xFF, (v >> 8) & 0xFF, v & 0xFF);
            return true;
        }

        // ─────────────────────────────────────────────────────────────
        // 页面
        // ─────────────────────────────────────────────────────────────

        private void BuildViews()
        {
            _welcomeView = new WelcomeView { Dock = DockStyle.Fill };
            _licenseView = new LicenseView { Dock = DockStyle.Fill };
            _pathView = new PathView { Dock = DockStyle.Fill };
            _optionsView = new OptionsView { Dock = DockStyle.Fill };
            _finishView = new FinishView { Dock = DockStyle.Fill };

            _licenseView.AcceptedChanged += accepted =>
            {
                _presenter.Options.AcceptLicense = accepted;
                UpdateButtons();
            };

            _pathView.BrowseClick += BrowseClick;
            _pathView.PathChanged += () =>
            {
                _presenter.Options.InstallDir = _pathView.InstallPath;
                UpdateButtons();
            };

            _progressView = new ProgressView { Dock = DockStyle.Fill };

            pnlContent.Controls.Add(_welcomeView);
            pnlContent.Controls.Add(_licenseView);
            pnlContent.Controls.Add(_pathView);
            pnlContent.Controls.Add(_optionsView);
            pnlContent.Controls.Add(_progressView);
            pnlContent.Controls.Add(_finishView);
        }

        private void BrowseClick()
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = _presenter.Text("Path.Label");
                dialog.SelectedPath = _presenter.Options.InstallDir;
                dialog.ShowNewFolderButton = true;

                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    _pathView.InstallPath = dialog.SelectedPath;
                    _presenter.Options.InstallDir = dialog.SelectedPath;
                }
            }
        }

        /// <summary>按当前状态刷新界面。**状态是枚举，不是按钮文字。**</summary>
        private void Render()
        {
            var page = _presenter.Current;

            if (_uninstallMode)
            {
                // 卸载模式：确认 → 进度 → 完成
                switch (page)
                {
                    case WizardPage.UninstallConfirm:
                        ShowView(_finishView);
                        _finishView.Title = _presenter.Text("Uninstall.Title");
                        _finishView.Body = string.Format(CultureInfo.CurrentCulture,
                            _presenter.Text("Uninstall.Confirm"), _presenter.ProductName);
                        _finishView.ShowRunOption = false;
                        _finishView.SetNeutral();
                        break;

                    case WizardPage.Uninstalling:
                        ShowView(_progressView);
                        _progressView.Title = _presenter.Text("Uninstall.Running");
                        _progressView.StepText = _presenter.Text("Uninstall.Progress");
                        _progressView.SetPercent(0);
                        break;

                    case WizardPage.Uninstalled:
                        ShowView(_finishView);
                        _finishView.Title = _presenter.Text("Uninstall.Done");
                        _finishView.Body = _presenter.ProductName;
                        _finishView.ShowRunOption = false;
                        _finishView.SetState(true);
                        break;
                }

                UpdateButtons();
                return;
            }

            switch (page)
            {
                case WizardPage.Welcome:
                    ShowView(_welcomeView);
                    _welcomeView.Title = _presenter.Text("Welcome.Title") + " " + _presenter.ProductName;
                    _welcomeView.Body = _presenter.Text("Welcome.Body");
                    _welcomeView.Logo = picLogo.Image;
                    break;

                case WizardPage.License:
                    ShowView(_licenseView);
                    _licenseView.Title = _presenter.Text("License.Title");
                    _licenseView.AcceptText = _presenter.Text("License.Accept");
                    _licenseView.LicenseText = _presenter.LicenseText;
                    _licenseView.Accepted = _presenter.Options.AcceptLicense;
                    break;

                case WizardPage.Path:
                    ShowView(_pathView);
                    _pathView.Title = _presenter.Text("Path.Title");
                    _pathView.PathLabel = _presenter.Text("Path.Label");
                    _pathView.BrowseText = _presenter.Text("Path.Browse");
                    _pathView.InstallPath = _presenter.Options.InstallDir;
                    _pathView.SpaceText = BuildSpaceText();
                    break;

                case WizardPage.Options:
                    ShowView(_optionsView);
                    _optionsView.Title = _presenter.Text("Options.Title");
                    _optionsView.DesktopText = _presenter.Text("Options.Desktop");
                    _optionsView.StartMenuText = _presenter.Text("Options.StartMenu");
                    _optionsView.AutostartText = _presenter.Text("Options.Autostart");
                    _optionsView.RunAfterText = _presenter.Text("Options.RunAfter");
                    _optionsView.DesktopShortcut = _presenter.Options.DesktopShortcut;
                    _optionsView.StartMenuShortcut = _presenter.Options.StartMenuShortcut;
                    _optionsView.Autostart = _presenter.Options.Autostart;
                    _optionsView.RunAfterInstall = _presenter.Options.RunAfterInstall;
                    break;

                case WizardPage.Installing:
                    ShowView(_progressView);
                    _progressView.Title = _presenter.Text("Progress.Title");
                    _progressView.StepText = _presenter.Text("Progress.Preparing");
                    _progressView.SetPercent(0);
                    break;

                case WizardPage.Finish:
                    ShowView(_finishView);
                    _finishView.Title = _presenter.Text("Finish.Title");
                    _finishView.Body = string.Format(CultureInfo.CurrentCulture,
                        _presenter.Text("Finish.Body"), _presenter.ProductName, _presenter.Options.InstallDir);
                    _finishView.RunText = _presenter.Text("Options.RunAfter");
                    _finishView.ShowRunOption = _manifest.RunAfterInstall;
                    _finishView.RunChecked = true;
                    _finishView.SetState(true);
                    break;

                case WizardPage.Failed:
                    ShowView(_finishView);
                    _finishView.Title = _presenter.Text("Failed.Title");
                    _finishView.Body = string.Format(CultureInfo.CurrentCulture,
                        _presenter.Text("Failed.Body"), _presenter.ErrorMessage);
                    _finishView.ShowRunOption = false;
                    _finishView.SetState(false);
                    break;
            }

            UpdateButtons();
        }

        private string BuildSpaceText()
        {
            try
            {
                var root = Path.GetPathRoot(_presenter.Options.InstallDir);
                long free = -1;
                if (!string.IsNullOrEmpty(root))
                {
                    foreach (var d in DriveInfo.GetDrives())
                    {
                        if (d.IsReady && string.Equals(d.Name.TrimEnd('\\'), root.TrimEnd('\\'),
                                StringComparison.OrdinalIgnoreCase))
                        {
                            free = d.AvailableFreeSpace;
                            break;
                        }
                    }
                }

                long need = 0;
                foreach (var f in _manifest.Files)
                {
                    need += f.Size;
                }

                return string.Format(CultureInfo.CurrentCulture, _presenter.Text("Path.Space"),
                    FormatSize(free), FormatSize(need));
            }
            catch (IOException)
            {
                return string.Empty;
            }
        }

        private static string FormatSize(long bytes)
        {
            if (bytes < 0)
            {
                return "--";
            }

            if (bytes >= 1024L * 1024 * 1024)
            {
                return (bytes / 1024.0 / 1024 / 1024).ToString("0.0 GB", CultureInfo.CurrentCulture);
            }

            if (bytes >= 1024 * 1024)
            {
                return (bytes / 1024.0 / 1024).ToString("0.0 MB", CultureInfo.CurrentCulture);
            }

            return (bytes / 1024.0).ToString("0.0 KB", CultureInfo.CurrentCulture);
        }

        private void ShowView(UserControl view)
        {
            if (ReferenceEquals(_currentView, view))
            {
                return;
            }

            foreach (Control c in pnlContent.Controls)
            {
                c.Visible = ReferenceEquals(c, view);
            }

            _currentView = view;
        }

        private void UpdateButtons()
        {
            var page = _presenter.Current;

            LocalizeSteps();

            btnPrimary.Text = _presenter.Text(_presenter.PrimaryKey);
            btnBack.Text = _presenter.Text("Wizard.Back");
            btnCancel.Text = _presenter.Text(_presenter.SecondaryKey);

            btnPrimary.Enabled = _presenter.PrimaryEnabled;
            btnBack.Visible = _presenter.CanGoBack;
            btnCancel.Visible = !_presenter.PrimaryIsCancel;

            steps.Visible = _presenter.ShowSteps;
            if (_presenter.ShowSteps)
            {
                steps.Current = Math.Min(_presenter.StepIndex, steps.Items.Count - 1);
            }

            // 安装中：主按钮变成"取消"，用 Error 色提示
            btnPrimary.Type = _presenter.PrimaryIsCancel ? AntdUI.TTypeMini.Error : AntdUI.TTypeMini.Primary;

            bool busy = page == WizardPage.Installing || page == WizardPage.Uninstalling;
            btnPrimary.Enabled = busy || _presenter.PrimaryEnabled;
        }

        /// <summary>步骤指示器的文案 key（顺序与 Designer 里的占位条目一致）。</summary>
        private static readonly string[] StepKeys =
        {
            "Step.License", "Step.Path", "Step.Options", "Step.Install", "Wizard.Finish",
        };

        /// <summary>
        /// 本地化步骤指示器的标签。
        /// Designer 里写的是占位字面量（设计器要求每个控件都有占位内容），
        /// 真正的文案在这里按语言刷 —— 否则英文界面会露出中文。
        /// </summary>
        private void LocalizeSteps()
        {
            for (var i = 0; i < steps.Items.Count; i++)
            {
                if (i < StepKeys.Length)
                {
                    steps.Items[i].Title = _presenter.Text(StepKeys[i]);
                }
            }
        }

        // ─────────────────────────────────────────────────────────────
        // 交互
        // ─────────────────────────────────────────────────────────────

        private void BtnPrimaryClick(object sender, EventArgs e)
        {
            if (_presenter.IsCancelling)
            {
                _host.RequestCancel();
                btnPrimary.Enabled = false;
                btnPrimary.Text = _presenter.Text("Wizard.Cancel");
                return;
            }

            var before = _presenter.Current;
            if (!_presenter.Primary())
            {
                // 校验未通过
                if (before == WizardPage.License)
                {
                    AntdUI.Message.warn(this, _presenter.Text("License.MustAccept"));
                }
                else if (before == WizardPage.Path)
                {
                    AntdUI.Message.warn(this, _presenter.Text("Path.Empty"));
                }

                return;
            }

            var after = _presenter.Current;

            if (after == WizardPage.Installing && before != WizardPage.Installing)
            {
                StartInstall();
                return;
            }

            if (_uninstallMode && after == WizardPage.Uninstalling)
            {
                StartUninstall();
                return;
            }

            if (after == WizardPage.Finish || after == WizardPage.Uninstalled)
            {
                FinishAndClose();
                return;
            }

            Render();
        }

        private void BtnBackClick(object sender, EventArgs e)
        {
            _presenter.Back();
            Render();
        }

        private void BtnCancelClick(object sender, EventArgs e)
        {
            if (_presenter.Current == WizardPage.Finish || _presenter.Current == WizardPage.Uninstalled)
            {
                FinishAndClose();
                return;
            }

            if (!_presenter.CanClose)
            {
                _host.RequestCancel();
                return;
            }

            var result = AntdUI.Modal.open(new AntdUI.Modal.Config(this,
                _presenter.ProductName,
                _presenter.Text("Wizard.Cancel") + " ?")
            {
                Keyboard = true,
                MaskClosable = false,
            });

            if (result == DialogResult.OK)
            {
                DialogResult = DialogResult.Cancel;
                Close();
            }
        }

        private void StartInstall()
        {
            Render();

            _presenter.ApplyToManifest();

            _progressView.SetIndeterminate(true);

            IProgress<InstallProgress> progress = new Progress<InstallProgress>(OnProgress);
            _host.StartInstall(_presenter.Options, p => progress.Report(p), OnDone);
        }

        private void StartUninstall()
        {
            Render();
            _progressView.SetIndeterminate(true);

            IProgress<InstallProgress> progress = new Progress<InstallProgress>(OnProgress);
            _host.StartUninstall(p => progress.Report(p), OnDone);
        }

        /// <summary>进度回调（可能在后台线程）。</summary>
        private void OnProgress(InstallProgress p)
        {
            if (IsDisposed || !IsHandleCreated)
            {
                return;
            }

            if (InvokeRequired)
            {
                BeginInvoke(new Action<InstallProgress>(OnProgress), p);
                return;
            }

            _progressView.SetIndeterminate(false);
            _progressView.SetPercent(p.Percent);

            if (!string.IsNullOrEmpty(p.Message))
            {
                _progressView.StepText = p.Message;
                _progressView.AppendLog(p.Message);
            }

            if (p.IsRollingBack)
            {
                _progressView.Title = _presenter.Text("Progress.RollingBack");
                _progressView.SetError(true);
            }
        }

        /// <summary>完成回调（可能在后台线程）。</summary>
        private void OnDone(bool success, string message)
        {
            if (IsDisposed || !IsHandleCreated)
            {
                return;
            }

            if (InvokeRequired)
            {
                BeginInvoke(new Action<bool, string>(OnDone), success, message);
                return;
            }

            _progressView.SetIndeterminate(false);

            if (success)
            {
                _progressView.SetPercent(100);
                _finished = true;

                if (_uninstallMode)
                {
                    _presenter.EnterUninstalled();
                }
                else
                {
                    _presenter.EnterFinish();
                }

                Render();
                _closeTimer.Start();
            }
            else
            {
                _progressView.SetError(true);

                if (_uninstallMode)
                {
                    _presenter.EnterFailed(message);
                    _presenter.EnterUninstalled();
                }
                else
                {
                    _presenter.EnterFailed(message);
                }

                Render();
            }
        }

        private void CloseTimerTick(object sender, EventArgs e)
        {
            _closeTimer.Stop();

            if (_uninstallMode)
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void FinishAndClose()
        {
            if (!_uninstallMode && _finishView.RunChecked && _manifest.RunAfterInstall)
            {
                try
                {
                    _host.LaunchEntryPoint();
                }
                catch (Exception ex)
                {
                    AntdUI.Message.warn(this, ex.Message);
                }
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        /// <summary>关闭前拦截：安装/卸载中不允许直接关。</summary>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (!_presenter.CanClose && !_finished)
            {
                e.Cancel = true;
                _host.RequestCancel();
                return;
            }

            base.OnFormClosing(e);
        }

        /// <summary>渲染每个页面到 PNG（用于视觉验收，不参与安装流程）。</summary>
        public void RenderPagesToPng(string directory)
        {
            Directory.CreateDirectory(directory);

            // 截图时关掉动画：否则进度条会停在动画中间帧，量出来的百分比是假的
            var animation = AntdUI.Config.Animation;
            AntdUI.Config.Animation = false;

            try
            {
                RenderPagesToPngCore(directory);
            }
            finally
            {
                AntdUI.Config.Animation = animation;
            }
        }

        private void RenderPagesToPngCore(string directory)
        {
            var pages = _uninstallMode
                ? new[] { WizardPage.UninstallConfirm, WizardPage.Uninstalling, WizardPage.Uninstalled }
                : new[] { WizardPage.Welcome, WizardPage.License, WizardPage.Path,
                          WizardPage.Options, WizardPage.Installing, WizardPage.Finish, WizardPage.Failed };

            foreach (var page in pages)
            {
                ForcePage(page);
                Render();
                ApplyPreviewDetail(page);

                using (var bmp = new Bitmap(Width, Height))
                {
                    DrawToBitmap(bmp, new Rectangle(0, 0, Width, Height));
                    bmp.Save(Path.Combine(directory, page + ".png"), System.Drawing.Imaging.ImageFormat.Png);
                }
            }
        }

        /// <summary>截图专用的演示细节（必须在 Render 之后调用，否则会被覆盖）。</summary>
        private void ApplyPreviewDetail(WizardPage page)
        {
            switch (page)
            {
                case WizardPage.Installing:
                    _progressView.SetIndeterminate(false);
                    _progressView.SetPercent(42);
                    _progressView.StepText = @"正在解压文件 3/5：bin\App.exe";
                    _progressView.AppendLog("步骤 3/9：extract");
                    _progressView.AppendLog("已解压 3 个条目。");
                    break;

                case WizardPage.Uninstalling:
                    _progressView.SetIndeterminate(false);
                    _progressView.SetPercent(60);
                    _progressView.StepText = "正在删除程序文件…";
                    break;
            }
        }

        private void ForcePage(WizardPage page)
        {
            // 先重置，避免"累加式"推进导致截图错位
            _presenter.Reset();

            switch (page)
            {
                case WizardPage.Welcome:
                    break;

                case WizardPage.License:
                    _presenter.Primary();
                    break;

                case WizardPage.Path:
                    _presenter.Options.AcceptLicense = true;
                    _presenter.Primary();   // → License
                    _presenter.Primary();   // → Path
                    break;

                case WizardPage.Options:
                    _presenter.Options.AcceptLicense = true;
                    _presenter.Primary();   // → License
                    _presenter.Primary();   // → Path
                    _presenter.Primary();   // → Options
                    break;

                case WizardPage.Installing:
                    _presenter.EnterInstalling();
                    break;

                case WizardPage.Finish:
                    _presenter.EnterFinish();
                    break;

                case WizardPage.Failed:
                    _presenter.EnterInstalling();
                    _presenter.EnterFailed("磁盘空间不足：需要约 128.0 MB，可用 12.4 MB。");
                    break;

                case WizardPage.UninstallConfirm:
                    _presenter.EnterUninstallConfirm();
                    break;

                case WizardPage.Uninstalling:
                    _presenter.EnterUninstalling();
                    break;

                case WizardPage.Uninstalled:
                    _presenter.EnterUninstalled();
                    break;
            }
        }
    }
}
