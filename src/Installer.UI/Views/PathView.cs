using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Installer.UI.Common;

namespace Installer.UI.Views
{
    /// <summary>
    /// 安装设置页（设计稿第 3 步「安装设置2」，视图 576x635）。
    ///
    /// 设计稿把原来的「安装位置」与「选项」两页合成了一页，所以这里同时承载：
    /// 路径输入 + 快捷路径 chips + 2×2 附加任务卡 + 为所有用户安装 + 浅蓝信息条。
    ///
    /// 坐标系：视图原点在窗口 (56,192)，控件 Location = 设计稿绝对坐标 − (56,192)。
    /// 逐元素的绝对坐标与字号见 PathView.Designer.cs 的注释块。
    /// </summary>
    public partial class PathView : UserControl
    {
        /// <summary>文件夹描边图标（16x16）。</summary>
        private const string FolderSvg = "<svg viewBox=\"0 0 16 16\"><path d=\"M1.7 3.2h4.3l1.5 1.6h6.8v7.9H1.7z\" fill=\"none\" stroke=\"#1677FF\" stroke-width=\"1.7\" stroke-linejoin=\"round\"/></svg>";

        /// <summary>清单描边图标（16x16）。</summary>
        private const string ListSvg = "<svg viewBox=\"0 0 16 16\"><rect x=\"2\" y=\"3\" width=\"3.6\" height=\"3.6\" rx=\"0.9\" fill=\"#1677FF\"/><rect x=\"7.2\" y=\"4.1\" width=\"6.8\" height=\"1.5\" rx=\"0.75\" fill=\"#1677FF\"/><rect x=\"2\" y=\"9.4\" width=\"3.6\" height=\"3.6\" rx=\"0.9\" fill=\"#1677FF\"/><rect x=\"7.2\" y=\"10.5\" width=\"6.8\" height=\"1.5\" rx=\"0.75\" fill=\"#1677FF\"/></svg>";

        /// <summary>对勾圆描边图标（16x16）。</summary>
        private const string CheckCircleSvg = "<svg viewBox=\"0 0 16 16\"><circle cx=\"8\" cy=\"8\" r=\"7\" fill=\"none\" stroke=\"#1677FF\" stroke-width=\"1.7\"/><path d=\"M4.9 8.2 7.1 10.4 11.3 5.8\" fill=\"none\" stroke=\"#1677FF\" stroke-width=\"1.7\" stroke-linecap=\"round\" stroke-linejoin=\"round\"/></svg>";

        /// <summary>快捷路径 chip 的灰色文件夹图标。</summary>
        private const string ChipFolderSvg = "<svg viewBox=\"0 0 16 16\"><path d=\"M1.7 3.6h4.3l1.5 1.6h6.8v7.2H1.7z\" fill=\"none\" stroke=\"#6B7280\" stroke-width=\"1.4\" stroke-linejoin=\"round\"/></svg>";

        /// <summary>路径输入框前缀的灰色文件夹图标。</summary>
        private const string InputFolderSvg = "<svg viewBox=\"0 0 16 16\"><path d=\"M1.7 3.6h4.3l1.5 1.6h6.8v7.2H1.7z\" fill=\"none\" stroke=\"#9CA3AF\" stroke-width=\"1.4\" stroke-linejoin=\"round\"/></svg>";

        /// <summary>磁盘图标（所需空间行）。</summary>
        private const string DiskSvg = "<svg viewBox=\"0 0 16 16\"><rect x=\"1.6\" y=\"3.4\" width=\"12.8\" height=\"9.2\" rx=\"1.6\" fill=\"none\" stroke=\"#9CA3AF\" stroke-width=\"1.4\"/><circle cx=\"11.4\" cy=\"8\" r=\"1.1\" fill=\"#9CA3AF\"/></svg>";

        /// <summary>浏览按钮的文件夹图标。</summary>
        private const string BrowseFolderSvg = "<svg viewBox=\"0 0 16 16\"><path d=\"M1.7 3.6h4.3l1.5 1.6h6.8v7.2H1.7z\" fill=\"none\" stroke=\"#4B5563\" stroke-width=\"1.4\" stroke-linejoin=\"round\"/></svg>";

        /// <summary>四张任务卡的图标（桌面 / 开始菜单 / 开机自启 / 完成后运行）。</summary>
        private const string CardDesktopSvg = "<svg viewBox=\"0 0 24 24\"><rect x=\"1.2\" y=\"2.6\" width=\"21.6\" height=\"14.8\" rx=\"1.8\" fill=\"none\" stroke=\"#1677FF\" stroke-width=\"2.4\"/><path d=\"M12 17.4v3\" stroke=\"#1677FF\" stroke-width=\"2.4\"/><path d=\"M7.6 20.4h8.8\" stroke=\"#1677FF\" stroke-width=\"2.4\" stroke-linecap=\"round\"/></svg>";

        private const string CardStartMenuSvg = "<svg viewBox=\"0 0 24 24\"><circle cx=\"7.2\" cy=\"7.2\" r=\"4.4\" fill=\"none\" stroke=\"#1677FF\" stroke-width=\"2.4\"/><circle cx=\"16.8\" cy=\"7.2\" r=\"4.4\" fill=\"none\" stroke=\"#1677FF\" stroke-width=\"2.4\"/><circle cx=\"7.2\" cy=\"16.8\" r=\"4.4\" fill=\"none\" stroke=\"#1677FF\" stroke-width=\"2.4\"/><circle cx=\"16.8\" cy=\"16.8\" r=\"4.4\" fill=\"none\" stroke=\"#1677FF\" stroke-width=\"2.4\"/></svg>";

        private const string CardAutostartSvg = "<svg viewBox=\"0 0 24 24\"><path d=\"M20.6 12a8.6 8.6 0 1 1-2.7-6.3\" fill=\"none\" stroke=\"#1677FF\" stroke-width=\"2.4\" stroke-linecap=\"round\"/><path d=\"M20.8 2.8v4.6h-4.6\" fill=\"none\" stroke=\"#1677FF\" stroke-width=\"2.4\" stroke-linecap=\"round\" stroke-linejoin=\"round\"/></svg>";

        private const string CardRunAfterSvg = "<svg viewBox=\"0 0 24 24\"><circle cx=\"12\" cy=\"12\" r=\"9.6\" fill=\"none\" stroke=\"#1677FF\" stroke-width=\"2.4\"/><path d=\"M9.4 7.4 17.4 12l-8 4.6z\" fill=\"#1677FF\"/></svg>";

        /// <summary>构造。</summary>
        public PathView()
        {
            InitializeComponent();
            ApplyAccent(WizardTheme.Primary);
            HookEvents();
        }

        /// <summary>点「浏览」。</summary>
        public event Action BrowseClick;

        /// <summary>安装路径变化。</summary>
        public event Action PathChanged;

        /// <summary>任一附加任务勾选变化。</summary>
        public event Action OptionsChanged;

        /// <summary>标题。</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Title
        {
            get { return lblTitle.Text; }
            set { lblTitle.Text = value; }
        }

        /// <summary>副标题。</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Subtitle
        {
            get { return lblSubtitle.Text; }
            set { lblSubtitle.Text = value; }
        }

        /// <summary>「安装位置」小节标题。</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string PathLabel
        {
            get { return lblPathCaption.Text; }
            set { lblPathCaption.Text = value; }
        }

        /// <summary>「浏览」按钮文案。</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string BrowseText
        {
            get { return btnBrowse.Text; }
            set { btnBrowse.Text = value; }
        }

        /// <summary>安装目录。</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string InstallPath
        {
            get { return txtPath.Text; }
            set { txtPath.Text = value ?? string.Empty; }
        }

        /// <summary>「所需空间 X · C 盘可用 Y」。</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string SpaceText
        {
            get { return lblSpace.Text; }
            set { lblSpace.Text = value ?? string.Empty; }
        }

        /// <summary>「附加任务」小节标题。</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string TaskLabel
        {
            get { return lblTaskCaption.Text; }
            set { lblTaskCaption.Text = value; }
        }

        /// <summary>浅蓝信息条文案。</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string InfoText
        {
            get { return lblInfo.Text; }
            set { lblInfo.Text = value ?? string.Empty; }
        }

        /// <summary>「为所有用户安装」文案。</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string AllUsersText
        {
            get { return lblAllUsers.Text; }
            set { lblAllUsers.Text = value; }
        }

        /// <summary>「为所有用户安装」下方的灰色提示。</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string AllUsersHint
        {
            get { return lblAllUsersHint.Text; }
            set { lblAllUsersHint.Text = value ?? string.Empty; }
        }

        /// <summary>是否为所有用户安装。</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool AllUsers
        {
            get { return chkAllUsers.Checked; }
            set { chkAllUsers.Checked = value; }
        }

        /// <summary>桌面快捷方式。</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool DesktopShortcut
        {
            get { return cardDesktop.Checked; }
            set { cardDesktop.Checked = value; }
        }

        /// <summary>开始菜单快捷方式。</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool StartMenuShortcut
        {
            get { return cardStartMenu.Checked; }
            set { cardStartMenu.Checked = value; }
        }

        /// <summary>开机启动。</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool Autostart
        {
            get { return cardAutostart.Checked; }
            set { cardAutostart.Checked = value; }
        }

        /// <summary>安装完成后运行。</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool RunAfterInstall
        {
            get { return cardRunAfter.Checked; }
            set { cardRunAfter.Checked = value; }
        }

        /// <summary>刷新四张任务卡的文案（图标固定，跟随设计稿）。</summary>
        public void ApplyTaskText(string desktop, string startMenu, string autostart, string runAfter,
            string desktopDesc, string startMenuDesc, string autostartDesc, string runAfterDesc)
        {
            cardDesktop.Title = desktop;
            cardDesktop.Description = desktopDesc;
            cardStartMenu.Title = startMenu;
            cardStartMenu.Description = startMenuDesc;
            cardAutostart.Title = autostart;
            cardAutostart.Description = autostartDesc;
            cardRunAfter.Title = runAfter;
            cardRunAfter.Description = runAfterDesc;
        }

        /// <summary>设置主色。</summary>
        public void ApplyAccent(Color accent)
        {
            infoBar.Back = WizardTheme.InfoBack;
            lblInfo.ForeColor = WizardTheme.InfoText;
            lblInfoIcon.PrefixSvg = Recolor(CheckCircleSvg, accent);

            lblTitle.ForeColor = WizardTheme.TextTitle;
            lblSubtitle.ForeColor = WizardTheme.TextBody;
            lblPathCaption.ForeColor = WizardTheme.TextTitle;
            lblTaskCaption.ForeColor = WizardTheme.TextTitle;
            lblAllUsers.ForeColor = WizardTheme.TextTitle;

            iconPath.PrefixSvg = Recolor(FolderSvg, accent);
            iconTask.PrefixSvg = Recolor(ListSvg, accent);

            txtPath.PrefixSvg = InputFolderSvg;
            txtPath.BorderColor = accent;
            lblSpace.PrefixSvg = DiskSvg;

            btnBrowse.IconSvg = BrowseFolderSvg;
            btnBrowse.DefaultBorderColor = WizardTheme.Border;
            btnBrowse.ForeColor = WizardTheme.TextBody;

            foreach (var chip in new[] { chip1, chip2, chip3, chip4 })
            {
                chip.Radius = WizardTheme.FieldRadius;
                chip.Back = WizardTheme.SoftBack;
            }

            foreach (var chipIcon in new[] { chip1Icon, chip2Icon, chip3Icon, chip4Icon })
            {
                chipIcon.PrefixSvg = ChipFolderSvg;
            }

            foreach (var chipText in new[] { chip1Text, chip2Text, chip3Text, chip4Text })
            {
                chipText.ForeColor = WizardTheme.TextBody;
            }

            cardDesktop.IconSvg = CardDesktopSvg;
            cardStartMenu.IconSvg = CardStartMenuSvg;
            cardAutostart.IconSvg = CardAutostartSvg;
            cardRunAfter.IconSvg = CardRunAfterSvg;

            cardDesktop.ApplyAccent(accent);
            cardStartMenu.ApplyAccent(accent);
            cardAutostart.ApplyAccent(accent);
            cardRunAfter.ApplyAccent(accent);

            chkAllUsers.Fill = accent;
        }

        private void HookEvents()
        {
            btnBrowse.Click += (sender, e) =>
            {
                var handler = BrowseClick;
                if (handler != null)
                {
                    handler();
                }
            };

            txtPath.TextChanged += (sender, e) =>
            {
                var handler = PathChanged;
                if (handler != null)
                {
                    handler();
                }
            };

            // 设计稿把「为所有用户安装」的复选框和文字拆成两个控件（文字要跟两行文案居中对齐），
            // 这里补回「点文字也能勾选」的可用性。
            lblAllUsers.Click += (sender, e) => chkAllUsers.Checked = !chkAllUsers.Checked;

            Action<bool> onTask = _ =>
            {
                var handler = OptionsChanged;
                if (handler != null)
                {
                    handler();
                }
            };

            cardDesktop.CheckedChangedInternal += onTask;
            cardStartMenu.CheckedChangedInternal += onTask;
            cardAutostart.CheckedChangedInternal += onTask;
            cardRunAfter.CheckedChangedInternal += onTask;
        }

        /// <summary>把 SVG 里的 #1677FF 换成指定颜色。</summary>
        private static string Recolor(string svg, Color color)
        {
            if (string.IsNullOrEmpty(svg))
            {
                return svg;
            }

            return svg.Replace("#1677FF", ToHex(color));
        }

        private static string ToHex(Color color)
        {
            return "#" + color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2");
        }
    }
}
