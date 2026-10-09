using System;
using System.Windows.Forms;

namespace Installer.UI.Views
{
    /// <summary>安装选项页。</summary>
    public partial class OptionsView : UserControl
    {
        /// <summary>构造。</summary>
        public OptionsView()
        {
            InitializeComponent();
        }

        /// <summary>标题。</summary>
        public string Title
        {
            get { return lblTitle.Text; }
            set { lblTitle.Text = value; }
        }

        /// <summary>桌面快捷方式文案。</summary>
        public string DesktopText
        {
            get { return chkDesktop.Text; }
            set { chkDesktop.Text = value; }
        }

        /// <summary>开始菜单快捷方式文案。</summary>
        public string StartMenuText
        {
            get { return chkStartMenu.Text; }
            set { chkStartMenu.Text = value; }
        }

        /// <summary>开机启动文案。</summary>
        public string AutostartText
        {
            get { return chkAutostart.Text; }
            set { chkAutostart.Text = value; }
        }

        /// <summary>安装后运行文案。</summary>
        public string RunAfterText
        {
            get { return chkRunAfter.Text; }
            set { chkRunAfter.Text = value; }
        }

        /// <summary>桌面快捷方式。</summary>
        public bool DesktopShortcut
        {
            get { return chkDesktop.Checked; }
            set { chkDesktop.Checked = value; }
        }

        /// <summary>开始菜单快捷方式。</summary>
        public bool StartMenuShortcut
        {
            get { return chkStartMenu.Checked; }
            set { chkStartMenu.Checked = value; }
        }

        /// <summary>开机启动。</summary>
        public bool Autostart
        {
            get { return chkAutostart.Checked; }
            set { chkAutostart.Checked = value; }
        }

        /// <summary>安装后运行。</summary>
        public bool RunAfterInstall
        {
            get { return chkRunAfter.Checked; }
            set { chkRunAfter.Checked = value; }
        }

        /// <summary>按清单能力决定哪些选项可见。</summary>
        public void ApplyVisibility(bool hasDesktop, bool hasStartMenu, bool hasAutostart, bool hasRunAfter)
        {
            chkDesktop.Visible = hasDesktop;
            chkStartMenu.Visible = hasStartMenu;
            chkAutostart.Visible = hasAutostart;
            chkRunAfter.Visible = hasRunAfter;
        }
    }
}
