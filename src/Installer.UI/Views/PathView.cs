using System;
using System.Drawing;
using System.Windows.Forms;
using Installer.UI.Common;

namespace Installer.UI.Views
{
    /// <summary>
    /// 安装设置页（设计稿第 3 步）。
    ///
    /// 设计稿把原来的「安装位置」与「选项」两页合成了一页，所以这里同时承载：
    /// 路径输入 + 快捷路径 chips + 2×2 附加任务卡 + 为所有用户安装 + 浅蓝信息条。
    /// </summary>
    public partial class PathView : UserControl
    {
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
        public string Title
        {
            get { return lblTitle.Text; }
            set { lblTitle.Text = value; }
        }

        /// <summary>副标题。</summary>
        public string Subtitle
        {
            get { return lblSubtitle.Text; }
            set { lblSubtitle.Text = value; }
        }

        /// <summary>「安装位置」小节标题。</summary>
        public string PathLabel
        {
            get { return lblPathCaption.Text; }
            set { lblPathCaption.Text = value; }
        }

        /// <summary>「浏览」按钮文案。</summary>
        public string BrowseText
        {
            get { return btnBrowse.Text; }
            set { btnBrowse.Text = value; }
        }

        /// <summary>安装目录。</summary>
        public string InstallPath
        {
            get { return txtPath.Text; }
            set { txtPath.Text = value ?? string.Empty; }
        }

        /// <summary>「所需空间 X · C 盘可用 Y」。</summary>
        public string SpaceText
        {
            get { return lblSpace.Text; }
            set { lblSpace.Text = value ?? string.Empty; }
        }

        /// <summary>「附加任务」小节标题。</summary>
        public string TaskLabel
        {
            get { return lblTaskCaption.Text; }
            set { lblTaskCaption.Text = value; }
        }

        /// <summary>浅蓝信息条文案。</summary>
        public string InfoText
        {
            get { return lblInfo.Text; }
            set { lblInfo.Text = value ?? string.Empty; }
        }

        /// <summary>「为所有用户安装」文案。</summary>
        public string AllUsersText
        {
            get { return chkAllUsers.Text; }
            set { chkAllUsers.Text = value; }
        }

        /// <summary>「为所有用户安装」下方的灰色提示。</summary>
        public string AllUsersHint
        {
            get { return lblAllUsersHint.Text; }
            set { lblAllUsersHint.Text = value ?? string.Empty; }
        }

        /// <summary>是否为所有用户安装。</summary>
        public bool AllUsers
        {
            get { return chkAllUsers.Checked; }
            set { chkAllUsers.Checked = value; }
        }

        /// <summary>桌面快捷方式。</summary>
        public bool DesktopShortcut
        {
            get { return cardDesktop.Checked; }
            set { cardDesktop.Checked = value; }
        }

        /// <summary>开始菜单快捷方式。</summary>
        public bool StartMenuShortcut
        {
            get { return cardStartMenu.Checked; }
            set { cardStartMenu.Checked = value; }
        }

        /// <summary>开机启动。</summary>
        public bool Autostart
        {
            get { return cardAutostart.Checked; }
            set { cardAutostart.Checked = value; }
        }

        /// <summary>安装完成后运行。</summary>
        public bool RunAfterInstall
        {
            get { return cardRunAfter.Checked; }
            set { cardRunAfter.Checked = value; }
        }

        /// <summary>刷新四张任务卡的文案。</summary>
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

            lblTitle.ForeColor = WizardTheme.TextTitle;
            lblSubtitle.ForeColor = WizardTheme.TextBody;

            foreach (var chip in new[] { btnChip1, btnChip2, btnChip3, btnChip4 })
            {
                chip.Radius = 6;
                chip.DefaultBack = WizardTheme.SoftBack;
            }
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
    }
}
