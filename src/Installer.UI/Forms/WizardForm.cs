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
        private UninstallConfirmView _uninstallConfirmView;
        private PathView _pathView;
        private OptionsView _optionsView;
        private ProgressView _progressView;
        private FinishView _finishView;
        private UserControl _currentView;
        private bool _finished;

        /// <summary>
        /// 设计期构造函数。
        ///
        /// VS 的 WinForms 设计器**必须**能无参实例化类型，否则只会报
        /// 「Constructor on type ... not found」，设计视图打不开。
        /// 这里只搭外壳（InitializeComponent），不接 Presenter / Host —— 那是运行时的事。
        /// </summary>
        public WizardForm()
        {
            InitializeComponent();
            ApplyCardRegion();
            MakeLabelsTransparent(this);
            this.Icon = Installer.UI.Common.AppIcon.Load();
        }

        /// <summary>构造。</summary>
        /// <param name="manifest">清单。</param>
        /// <param name="host">安装引擎桥。</param>
        /// <param name="defaultInstallDir">已解析的默认安装目录。</param>
        /// <param name="culture">界面语言；null 表示跟随系统。</param>
        /// <param name="uninstallMode">是否为卸载模式。</param>
        public WizardForm(InstallerManifest manifest, IWizardHost host, string defaultInstallDir,
                          string culture = null, bool uninstallMode = false)
        {            if (manifest == null)
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
            ApplyCardRegion();
            MakeLabelsTransparent(this);

            // 显式设置窗口图标 —— 标题栏 / 任务栏 / Alt+Tab 都跟着它。
            // 向导跑在 Setup.exe 里，所以拿到的是打包时打进去的那个图标。
            this.Icon = Installer.UI.Common.AppIcon.Load();

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

        /// <summary>当前主色（安装 = 蓝，卸载 = 红）。</summary>
        private Color _accent = WizardTheme.Primary;

        /// <summary>
        /// 把卡片裁成圆角。
        ///
        /// 为什么要手动裁：AntdUI.Panel 的 Radius 只作用于它**自己**的填充，
        /// 而卡片里铺满了 cardLayout（TableLayoutPanel）和品牌区的 Panel，
        /// 它们的直角会把卡片的圆角盖掉。用 Region 裁剪整个卡片（含子控件）才有效。
        /// </summary>
        /// <summary>
        /// 把 AntdUI.Panel 内部的 Label 背景设为透明。
        ///
        /// 为什么需要：AntdUI.Panel 的圆角靠它自己的 Back 画出来，
        /// 所以不能给面板设 WinForms 的 BackColor（会盖成直角）。
        /// 但面板一旦没有 BackColor，里面的 Label 就失去可继承的背景，
        /// 各自画一块白底 —— 表现就是「产品名后面有个白方块」。
        /// 显式设成透明即可两全。
        /// </summary>
        private static void MakeLabelsTransparent(System.Windows.Forms.Control root)
        {
            foreach (System.Windows.Forms.Control c in root.Controls)
            {
                var label = c as AntdUI.Label;
                if (label != null)
                {
                    label.BackColor = System.Drawing.Color.Transparent;
                }

                if (c.Controls.Count > 0)
                {
                    MakeLabelsTransparent(c);
                }
            }
        }

        /// <summary>
        /// 卡片裁圆角（已不再需要，保留为空实现会误导人，所以整段删掉了 —— 见 git 历史）。
        ///
        /// 卡片现在铺满整个窗口、Radius=0，窗口四角是方的，没有圆角可裁。
        /// </summary>
        private void ApplyCardRegion()
        {
            // 故意留空：历史上这里用 GraphicsPath 给卡片裁 16px 圆角，
            // 防止子控件（表头/底栏的不透明白底）把圆角盖成方角。
            // 改成"无留白铺满窗口"之后不需要了。
            // 保留方法体而不是删除，是为了不改动 3 处调用点、让 diff 最小。
            card.Region = null;
        }

        /// <summary>
        /// 设计稿里每页的高度都不一样（欢迎 712、许可协议 1012、安装设置 923…），
        /// 所以窗口要跟着当前页变高，否则高页面底部会被裁掉。
        /// 数值取自 Calicat 设计稿第 3 版各页 frame 的实际高度。
        /// </summary>
        private static int DesignHeight(WizardPage page)
        {
            switch (page)
            {
                case WizardPage.Welcome:
                    return 712;
                case WizardPage.License:
                    // 设计稿是 1012，但 1080p 屏上几乎顶满、观感太"长"，
                    // 正文框本身可滚动，所以压到 860（内容一字不少，只是可视高度变小）
                    return 860;
                case WizardPage.Path:
                case WizardPage.Options:
                    // 设计稿是 923，压缩到 810（收紧任务区与信息条之间的空隙）
                    return 848;
                case WizardPage.Installing:
                case WizardPage.Uninstalling:
                    return 720;
                case WizardPage.UninstallConfirm:
                    return 764;
                default:
                    return 584;
            }
        }

        /// <summary>
        /// 按当前页把窗口与卡片调整到设计稿高度。
        /// 卡片四周各留 24（设计稿的页面 padding），所以卡片高 = 窗口高 - 48。
        /// </summary>
        private void ApplyPageSize(WizardPage page)
        {
            var h = DesignHeight(page);

            if (ClientSize.Height != h)
            {
                // 先放开约束再改尺寸，否则会被 Minimum/MaximumSize 顶回去
                MinimumSize = new System.Drawing.Size(0, 0);
                MaximumSize = new System.Drawing.Size(0, 0);
                ClientSize = new System.Drawing.Size(Common.WizardTheme.WindowWidth, h);
                MinimumSize = new System.Drawing.Size(Common.WizardTheme.WindowWidth, h);
                MaximumSize = new System.Drawing.Size(Common.WizardTheme.WindowWidth, h);
            }

            ApplyCardBox(h);
        }

        /// <summary>
        /// 卡片铺满整个窗口 —— 不留边距、不画投影。
        /// 白色直接到窗口四边，蓝色标题从 y=0 开始。
        /// 圆角由**窗口**负责（见 <see cref="ApplyWindowRegion"/>），不是控件。
        /// </summary>
        private void ApplyCardBox(int h)
        {
            card.Location = new System.Drawing.Point(0, 0);
            card.Size = new System.Drawing.Size(ClientSize.Width, h);

            ApplyWindowRegion();
            card.Invalidate(true);
        }

        /// <summary>
        /// 给**窗口**裁 16px 圆角。
        ///
        /// 为什么圆角必须做在窗口上、而不是做在控件上：
        ///   控件带圆角时，被切掉的那块露出的是**父容器的底色** ——
        ///   卡片铺满窗口后就是白色，于是窗口四角出现白色缺口
        ///   （蓝色标题区以前 Radius=16，左上/右上就是两个白角）。
        ///   窗口自己带 Region 时，切掉的那块直接是**桌面**，不会有任何露白。
        ///
        /// 窗口是 FormBorderStyle.None，所以 Region 就是最终外形。
        /// </summary>
        private void ApplyWindowRegion()
        {
            var r = Common.WizardTheme.CardRadius;
            var w = ClientSize.Width;
            var h = ClientSize.Height;

            if (w <= 0 || h <= 0)
            {
                return;
            }

            // 半径不能超过短边的一半，否则 GraphicsPath 画出来的弧会自交
            var max = Math.Min(w, h) / 2;
            if (r > max)
            {
                r = max;
            }

            using (var path = new System.Drawing.Drawing2D.GraphicsPath())
            {
                var d = r * 2;

                path.AddArc(0, 0, d, d, 180, 90);
                path.AddArc(w - d, 0, d, d, 270, 90);
                path.AddArc(w - d, h - d, d, d, 0, 90);
                path.AddArc(0, h - d, d, d, 90, 90);
                path.CloseFigure();

                var old = Region;
                Region = new System.Drawing.Region(path);
                if (old != null)
                {
                    old.Dispose();
                }
            }
        }

        /// <summary>窗口真正显示后再补一次卡片布局。
        ///
        /// 构造函数里的 Render() 跑在 Show() 之前，那时控件的最终尺寸还没落定；
        /// 这里补一次保证首次显示时卡片就在设计稿位置上。代价可以忽略。
        /// </summary>
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            ApplyCardBox(DesignHeight(_presenter.Current));
        }

        private void ApplyAppearance()
        {
            Text = _presenter.ProductName + " " + _presenter.ProductVersion;

            // 品牌区：产品名 + 「安装向导 · 版本 x」/「卸载向导 · 版本 x」
            var uninstall = _presenter.Current == WizardPage.UninstallConfirm
                            || _presenter.Current == WizardPage.Uninstalling
                            || _presenter.Current == WizardPage.Uninstalled;

            headerView.ApplyText(_presenter.Text, _presenter.ProductName, _presenter.ProductVersion, uninstall);

            var ui = _manifest.Ui;
            // 卸载向导主色换红（设计稿：卸载流程的当前步骤点与主按钮都是红）
            var accent = _uninstallMode ? WizardTheme.Danger : WizardTheme.Primary;

            if (ui != null && !string.IsNullOrEmpty(ui.AccentColor))
            {
                Color parsed;
                if (TryParseColor(ui.AccentColor, out parsed))
                {
                    accent = parsed;
                }
            }

            // 品牌头部始终是品牌蓝 —— 设计稿里卸载向导的头部也没有变红，
            // 只有步骤条的当前点与主按钮换成红色。所以 accent 只作用于后两者。
            headerView.ApplyAccent(WizardTheme.Primary);
            stepBar.Accent = accent;
            _accent = accent;

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
            _uninstallConfirmView = new UninstallConfirmView { Dock = DockStyle.Fill };
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
            pnlContent.Controls.Add(_uninstallConfirmView);
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
            ApplyPageSize(page);

            if (_uninstallMode)
            {
                // 卸载模式：确认 → 进度 → 完成
                switch (page)
                {
                    case WizardPage.UninstallConfirm:
                        // 设计稿第 3 版给「确认卸载」单独画了一页（信息卡 + 删除数据勾选 + 红色警告条），
                        // 不再拿完成页凑。
                        ShowView(_uninstallConfirmView);
                        _uninstallConfirmView.ApplyAccent(_accent);
                        _uninstallConfirmView.Title = string.Format(CultureInfo.CurrentCulture,
                            _presenter.Text("Uninstall.ConfirmTitle"), _presenter.ProductName);
                        _uninstallConfirmView.Subtitle = string.Format(CultureInfo.CurrentCulture,
                            _presenter.Text("Uninstall.ConfirmSubtitle"), _presenter.ProductName, _presenter.ProductVersion);
                        _uninstallConfirmView.ApplyCaptions(_presenter.Text);
                        _uninstallConfirmView.DirValue = _presenter.Options.InstallDir;
                        _uninstallConfirmView.SpaceValue = string.Format(CultureInfo.CurrentCulture,
                            _presenter.Text("Uninstall.SpaceValue"), BuildInstalledSizeText());
                        _uninstallConfirmView.VersionValue = _presenter.ProductVersion + " · " + _presenter.Publisher;
                        _uninstallConfirmView.DeleteDataText = _presenter.Text("Uninstall.DeleteData");
                        _uninstallConfirmView.DeleteDataHint = _presenter.Text("Uninstall.DeleteData.Hint");
                        _uninstallConfirmView.DeleteUserData = false;
                        _uninstallConfirmView.WarningLine1 = _presenter.Text("Uninstall.Warn1");
                        _uninstallConfirmView.WarningLine2 = _presenter.Text("Uninstall.Warn2");
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
                    _welcomeView.Title = string.Format(CultureInfo.CurrentCulture,
                        _presenter.Text("Welcome.Title"), _presenter.ProductName);
                    _welcomeView.Body = string.Format(CultureInfo.CurrentCulture,
                        _presenter.Text("Welcome.Body"), _presenter.ProductName, _presenter.ProductVersion)
;
                    _welcomeView.Body2 = _presenter.Text("Welcome.Body2");
                    _welcomeView.ApplyCaptions(_presenter.Text);
                    _welcomeView.DirValue = _presenter.Options.InstallDir;
                    _welcomeView.SpaceValue = BuildSpaceText();
                    _welcomeView.VersionValue = _presenter.ProductVersion + " · " + _presenter.Publisher;
                    // 没有许可协议时别提示「下一步要先同意许可协议」
                    _welcomeView.Hint = _presenter.LicenseRequired
                        ? _presenter.Text("Welcome.Hint")
                        : _presenter.Text("Welcome.HintNoLicense");
                    break;

                case WizardPage.License:
                    ShowView(_licenseView);
                    _licenseView.Title = _presenter.Text("License.Title");
                    _licenseView.Subtitle = string.Format(
                        System.Globalization.CultureInfo.CurrentCulture,
                        _presenter.Text("License.Subtitle"), _presenter.ProductName);
                    _licenseView.AcceptText = string.Format(
                        System.Globalization.CultureInfo.CurrentCulture,
                        _presenter.Text("License.Accept"), _presenter.ProductName);
                    _licenseView.AcceptHint = _presenter.Text("License.AcceptHint");
                    _licenseView.LicenseText = _presenter.LicenseText;
                    _licenseView.Accepted = _presenter.Options.AcceptLicense;
                    _licenseView.ApplyCaptions(_presenter.Text, _presenter.ProductName);
                    break;

                case WizardPage.Path:
                    ShowView(_pathView);
                    _pathView.Title = _presenter.Text("Path.Title");
                    _pathView.Subtitle = _presenter.Text("Path.Subtitle");
                    _pathView.PathLabel = _presenter.Text("Path.Label");
                    _pathView.TaskLabel = _presenter.Text("Options.Title");
                    _pathView.BrowseText = _presenter.Text("Path.Browse");
                    _pathView.InstallPath = _presenter.Options.InstallDir;
                    _pathView.SpaceText = BuildSpaceText();

                    _pathView.ApplyTaskText(
                        _presenter.Text("Options.Desktop"), _presenter.Text("Options.StartMenu"),
                        _presenter.Text("Options.Autostart"), _presenter.Text("Options.RunAfter"),
                        _presenter.Text("Options.Desktop.Desc"), _presenter.Text("Options.StartMenu.Desc"),
                        _presenter.Text("Options.Autostart.Desc"), _presenter.Text("Options.RunAfter.Desc"));

                    _pathView.AllUsersText = _presenter.Text("Options.AllUsers");
                    _pathView.AllUsersHint = _presenter.Text("Options.AllUsers.Hint");
                    _pathView.AllUsers = _presenter.Options.AllUsers;

                    _pathView.DesktopShortcut = _presenter.Options.DesktopShortcut;
                    _pathView.StartMenuShortcut = _presenter.Options.StartMenuShortcut;
                    _pathView.Autostart = _presenter.Options.Autostart;
                    _pathView.RunAfterInstall = _presenter.Options.RunAfterInstall;

                    _pathView.InfoText = BuildSettingsInfo();
                    break;

                case WizardPage.Options:
                    // 设计稿把「安装位置」与「附加任务」合成一页，Options 已不再进入。
                    // 保留分支只为兼容旧状态（永远不会走到）。
                    ShowView(_pathView);
                    break;

                case WizardPage.Installing:
                    ShowView(_progressView);
                    _progressView.ApplyAccent(_accent);
                    _progressView.Title = string.Format(CultureInfo.CurrentCulture,
                        _presenter.Text("Progress.Title"), _presenter.ProductName);
                    _progressView.Subtitle = _presenter.Text("Progress.Subtitle");
                    _progressView.StepText = _presenter.Text("Progress.Preparing");
                    _progressView.CurrentFileText = string.Empty;
                    _progressView.SetStatCaptions(
                        _presenter.Text("Progress.CapFiles"), _presenter.Text("Progress.CapSize"),
                        _presenter.Text("Progress.CapElapsed"), _presenter.Text("Progress.CapRemaining"));
                    _progressView.SetStats("—", "—", "00:00", "—");
                    _progressView.SetSteps(new[]
                    {
                        _presenter.Text("Progress.Step1"), _presenter.Text("Progress.Step2"),
                        _presenter.Text("Progress.Step3"), _presenter.Text("Progress.Step4"),
                    }, 0);
                    _progressView.SetPercent(0);
                    break;

                case WizardPage.Finish:
                    ShowView(_finishView);
                    _finishView.Title = string.Format(CultureInfo.CurrentCulture,
                        _presenter.Text("Finish.Title"), _presenter.ProductName);
                    _finishView.Body = string.Format(CultureInfo.CurrentCulture,
                        _presenter.Text("Finish.Body"), _presenter.Options.InstallDir, _manifest.Files.Count, BuildSpaceText());
                    _finishView.RunText = string.Format(CultureInfo.CurrentCulture,
                        _presenter.Text("Options.RunAfter"), _presenter.ProductName);
                    _finishView.RunHint = _presenter.Text("Finish.RunHint");
                    _finishView.FooterHint = string.Format(CultureInfo.CurrentCulture,
                        _presenter.Text("Finish.UninstallHint"), _presenter.ProductName);
                    _finishView.SetChips(
                        HasShortcut("Desktop") ? _presenter.Text("Options.Desktop") : null,
                        HasShortcut("StartMenu") ? _presenter.Text("Options.StartMenu") : null,
                        _manifest.Autostart ? _presenter.Text("Options.Autostart") : null);
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

        /// <summary>浅蓝信息条：「将安装到 X · 已选择 N 项附加任务」。</summary>
        /// <summary>清单里是否声明了指定位置的快捷方式。</summary>
        private bool HasShortcut(string location)
        {
            if (_manifest.Shortcuts == null)
            {
                return false;
            }

            foreach (var s in _manifest.Shortcuts)
            {
                if (s != null && string.Equals(s.Location, location, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        private string BuildSettingsInfo()
        {
            var count = 0;
            var o = _presenter.Options;
            if (o.DesktopShortcut) { count++; }
            if (o.StartMenuShortcut) { count++; }
            if (o.Autostart) { count++; }
            if (o.RunAfterInstall) { count++; }

            return string.Format(
                System.Globalization.CultureInfo.CurrentCulture,
                _presenter.Text("Options.InfoFormat"),
                o.InstallDir,
                count);
        }

        /// <summary>卸载页用的「已占用大小」（卸载模式下清单是空的，所以直接量安装目录）。</summary>
        private string BuildInstalledSizeText()
        {
            try
            {
                var dir = _presenter.Options.InstallDir;
                if (string.IsNullOrEmpty(dir) || !Directory.Exists(dir))
                {
                    return FormatSize(-1);
                }

                long total = 0;
                foreach (var file in Directory.GetFiles(dir, "*", SearchOption.AllDirectories))
                {
                    total += new FileInfo(file).Length;
                }

                return FormatSize(total);
            }
            catch (IOException)
            {
                return FormatSize(-1);
            }
            catch (UnauthorizedAccessException)
            {
                return FormatSize(-1);
            }
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

                return string.Format(CultureInfo.CurrentCulture, _presenter.Text("Welcome.SpaceValue"),
                    FormatSize(need), FormatSize(free));
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

            stepBar.Visible = _presenter.ShowSteps;
            if (_presenter.ShowSteps)
            {
                stepBar.Current = Math.Min(_presenter.StepIndex, Math.Max(0, stepBar.Steps.Length - 1));

                // 底栏左侧「第 N 步，共 M 步」。StepIndex 是 0 起的，显示要 +1。
                lblStepInfo.Text = string.Format(
                    System.Globalization.CultureInfo.CurrentCulture,
                    _presenter.Text("Wizard.StepInfo"),
                    _presenter.StepIndex + 1,
                    stepBar.Steps.Length);
            }
            else
            {
                lblStepInfo.Text = string.Empty;
            }

            // 安装中：主按钮变成"取消"，用 Error 色提示
            // 安装/卸载进行中时主按钮是「安装中…」的静态状态，不该用红色警告色
            btnPrimary.Type = AntdUI.TTypeMini.Primary;

            bool busy = page == WizardPage.Installing || page == WizardPage.Uninstalling;
            btnPrimary.Enabled = busy || _presenter.PrimaryEnabled;
        }

        /// <summary>步骤指示器的文案 key（顺序与 Designer 里的占位条目一致）。</summary>
        private static readonly string[] StepKeys =
        {
            "Wizard.StepWelcome", "Step.License", "Wizard.StepSettings", "Step.Install", "Wizard.Finish",
        };

        /// <summary>
        /// 本地化步骤指示器的标签。
        /// Designer 里写的是占位字面量（设计器要求每个控件都有占位内容），
        /// 真正的文案在这里按语言刷 —— 否则英文界面会露出中文。
        /// </summary>
        private void LocalizeSteps()
        {
            var keys = _presenter.StepKeys;
            var titles = new string[keys.Length];
            for (var i = 0; i < keys.Length; i++)
            {
                titles[i] = _presenter.Text(keys[i]);
            }

            stepBar.Steps = titles;
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
                _progressView.CurrentFileText = p.Message;
            }

            // 步骤列表按进度推进：0~25 / 25~50 / 50~75 / 75~100
            var stage = Math.Min(3, Math.Max(0, p.Percent / 25));
            _progressView.SetSteps(new[]
            {
                _presenter.Text("Progress.Step1"), _presenter.Text("Progress.Step2"),
                _presenter.Text("Progress.Step3"), _presenter.Text("Progress.Step4"),
            }, stage);

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
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            // 窗口圆角跟着尺寸走（这里同时负责了 ApplyCardBox 里的那次调用，
            // 但 ApplyPageSize 直接改 ClientSize 时也要立刻生效，所以两边都调）。
            ApplyWindowRegion();
            MakeLabelsTransparent(this);
        }

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
            // ⚠️ 列表里**不能有 WizardPage.Options**：
            // 设计稿把「安装位置」与「附加任务」并成了一页，
            // 流程是 Welcome -> License -> Path -> Installing -> Finish，
            // Options 已经不可达（Primary() 从 Path 直接进 Installing）。
            // 留着它只会生成一张和 Path.png 重复、名字却对不上的图。
            var pages = _uninstallMode
                ? new[] { WizardPage.UninstallConfirm, WizardPage.Uninstalling, WizardPage.Uninstalled }
                : new[] { WizardPage.Welcome, WizardPage.License, WizardPage.Path,
                          WizardPage.Installing, WizardPage.Finish, WizardPage.Failed };

            foreach (var page in pages)
            {
                ForcePage(page);
                Render();
                ApplyPreviewDetail(page);

                // 自检：文件名必须和**实际渲染的页**一致。
                //
                // 为什么必须断言：以前 ForcePage 里写死 Primary() 次数，
                // 在"不需要许可协议"的工程上每一步都少走一格 ——
                // License.png 里其实是 Path、Path.png 里其实是"正在安装"。
                // 图照样生成、文件名照样正确，验收表却全是假的。
                // 这类"静默错页"必须让脚本直接失败，不能靠人眼看出来。
                if (_presenter.Current != page)
                {
                    throw new InvalidOperationException(
                        "渲染错页：想要 " + page + "，实际停在 " + _presenter.Current +
                        "。请检查 ForcePage() 的推进逻辑（步数取决于是否需要许可协议）。");
                }

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
                    _progressView.SetPercent(62);
                    _progressView.StepText = @"正在解压文件 3/5：bin\App.exe";
                    _progressView.CurrentFileText = @"正在写入：bin\CqVideo\ffmpeg.dll";
                    _progressView.SetStats("123 / 199", "56.4 MB / 90.6 MB", "00:12", "约 8 秒");
                    _progressView.SetSteps(new[]
                    {
                        _presenter.Text("Progress.Step1") + " —— 已完成",
                        _presenter.Text("Progress.Step2") + " —— 进行中 62%",
                        _presenter.Text("Progress.Step3") + " —— 等待中",
                        _presenter.Text("Progress.Step4") + " —— 等待中",
                    }, 1);
                    break;

                case WizardPage.Uninstalling:
                    _progressView.SetIndeterminate(false);
                    _progressView.SetPercent(60);
                    _progressView.StepText = "正在删除程序文件…";
                    _progressView.CurrentFileText = @"正在删除：bin\lib.dll";
                    _progressView.SetStats("123 / 199", "56.4 MB / 90.6 MB", "00:06", "约 4 秒");
                    _progressView.SetSteps(new[]
                    {
                        "结束关联进程 —— 已完成",
                        "删除程序文件 —— 进行中 60%",
                        "移除桌面与开始菜单快捷方式 —— 等待中",
                        "清理注册表与卸载信息 —— 等待中",
                    }, 1);
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
                case WizardPage.Path:
                case WizardPage.Options:
                    // 直接设页，**不要改回用 Primary() 逐步推进**。
                    //
                    // 为什么逐步推进不可行：
                    //   ① 不需要许可协议时（_licenseRequired=false），
                    //      Welcome --Primary--> **直接到 Path**，License 页根本不可达；
                    //   ② Path --Primary--> **直接到 Installing**
                    //      （设计稿把「安装位置」与「附加任务」并成一页，没有独立 Options 页）。
                    // 于是"想截 License 页"在流程上做不到 —— 只能直接设页。
                    //
                    // 以前写死 1/2/3 次 Primary()，在"不需要许可协议"的工程上每一步都
                    // 少走一格：License.png 里其实是 Path、Path.png 里其实是"正在安装"。
                    // 图照样生成、文件名照样正确，验收表却全是假的。
                    //
                    // RenderPagesToPngCore() 里加了自检，页名和实际页不符会直接抛异常。
                    _presenter.ForceCurrentForPreview(page);
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
