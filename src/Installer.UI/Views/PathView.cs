using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Installer.UI.Common;
using Installer.UI.Models;

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
        /// <summary>
        /// 正在把模型灌进界面（<see cref="ApplyOptions"/>）。期间控件事件**不得**反过来写模型。
        /// </summary>
        private bool _applying;

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

        /// <summary>
        /// 点某个快捷路径 chip（C:\Program Files (x86) / AppData / D:\Program Files (x86)）。
        /// 参数是**根目录**（已展开环境变量）。
        /// </summary>
        public event Action<string> PathChipClicked;

        /// <summary>安装路径变化。</summary>
        public event Action PathChanged;

        /// <summary>任一附加任务勾选变化。</summary>
        public event Action OptionsChanged;

        /// <summary>「为所有用户安装」勾选变化（切换范围要连带换默认目录）。</summary>
        public event Action AllUsersChanged;

        /// <summary>
        /// 把界面上这一排勾选框的**当前状态读进** <paramref name="options"/>。
        ///
        /// 单独抽成方法的理由很实际：这个方向以前整条是断的 ——
        /// <see cref="OptionsChanged"/> 声明了、勾选时也触发了，但全项目没有任何订阅者，
        /// 于是用户在界面上取消勾选完全没效果（照样建桌面快捷方式、照样开机自启）。
        /// 抽出来之后，"界面 → 选项"这个方向可以被单测直接钉住。
        /// </summary>
        /// <param name="options">要写入的选项对象；为 null 时忽略。</param>
        public void ReadInto(WizardOptions options)
        {
            if (options == null)
            {
                return;
            }

            if (!string.IsNullOrWhiteSpace(InstallPath))
            {
                options.InstallDir = InstallPath;
            }

            options.DesktopShortcut = DesktopShortcut;
            options.StartMenuShortcut = StartMenuShortcut;
            options.Autostart = Autostart;
            options.RunAfterInstall = RunAfterInstall;
            options.AllUsers = AllUsers;
        }

        /// <summary>
        /// **原子**地把选项灌进界面（模型 → 视图）。
        ///
        /// 为什么必须有这个方法，而不是让调用方一个个设属性：那样会踩到一条极隐蔽的坑 ——
        /// 设第一个勾选框时会触发 <see cref="OptionsChanged"/>，窗体回调里
        /// <see cref="ReadInto"/> 把**其余三个还没灌进去的勾选框**（此刻还是未勾）读回模型，
        /// 于是清单里明明是「桌面 + 开始菜单 + 开机自启 + 完成后运行」，
        /// 装出来只有第一个生效，另外三项被静默抹成 false。
        /// 用户反馈的「制作端勾了快捷方式，装的时候没按勾选」就是这么来的。
        ///
        /// 这里用 <see cref="_applying"/> 把灌值期间的控件事件全部屏蔽掉，
        /// 保证"模型 → 视图"是单向的（设计文档 §9 不变量 #9）。
        /// </summary>
        /// <param name="options">要显示的选项；为 null 时忽略。</param>
        public void ApplyOptions(WizardOptions options)
        {
            if (options == null)
            {
                return;
            }

            _applying = true;
            try
            {
                InstallPath = options.InstallDir ?? string.Empty;
                AllUsers = options.AllUsers;
                DesktopShortcut = options.DesktopShortcut;
                StartMenuShortcut = options.StartMenuShortcut;
                Autostart = options.Autostart;
                RunAfterInstall = options.RunAfterInstall;
            }
            finally
            {
                _applying = false;
            }
        }

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

            // 只给**真正会用到的三个** chip 上色。
            // chip4 是设计稿里原来的「Program Files」，已停用（见 ChipProgramFilesX86 的注释）。
            foreach (var chip in new[] { chip1, chip2, chip3 })
            {
                chip.Radius = WizardTheme.FieldRadius;
                chip.Back = WizardTheme.SoftBack;
            }

            foreach (var chipIcon in new[] { chip1Icon, chip2Icon, chip3Icon })
            {
                chipIcon.PrefixSvg = ChipFolderSvg;
            }

            foreach (var chipText in new[] { chip1Text, chip2Text, chip3Text })
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
                if (_applying)
                {
                    return;
                }

                var handler = PathChanged;
                if (handler != null)
                {
                    handler();
                }
            };

            // 设计稿把「为所有用户安装」的复选框和文字拆成两个控件（文字要跟两行文案居中对齐），
            // 这里补回「点文字也能勾选」的可用性。
            lblAllUsers.Click += (sender, e) => chkAllUsers.Checked = !chkAllUsers.Checked;

            chkAllUsers.CheckedChanged += (sender, e) =>
            {
                if (_applying)
                {
                    return;
                }

                var handler = AllUsersChanged;
                if (handler != null)
                {
                    handler();
                }
            };

            Action<bool> onTask = _ =>
            {
                if (_applying)
                {
                    return;
                }

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

            // 快捷路径 chips。
            //
            // 这几个 chip 以前**只是装饰**：画得跟设计稿一样，但点了完全没反应 ——
            // 用户「Program Files 点不了」说的就是这个。现在点一下就把安装位置换到那个根目录
            // （具体拼成什么路径由 Presenter 决定：根\厂商\产品）。
            //
            // 事件必须挂到 chip 里的图标和文字上：AntdUI.Label 自己吃鼠标消息，
            // 只挂 Panel 的 Click 时，点在文字上（占了大半个 chip）不会有任何反应。
            HookChip(chip1, chip1Icon, chip1Text, ChipProgramFilesX86);
            HookChip(chip2, chip2Icon, chip2Text, ChipLocalAppData);
            HookChip(chip3, chip3Icon, chip3Text, ChipDriveD);

            // 这台机器没有 D 盘时，"D:\Program Files" 这个选项不该出现
            // （点了只会得到一个装不进去的路径）。
            ApplyChipAvailability();
        }

        /// <summary>chip 序号：C:\Program Files (x86)（所有用户）。</summary>
        public const int ChipProgramFilesX86 = 1;

        /// <summary>chip 序号：AppData（<c>%LOCALAPPDATA%</c>，仅当前用户）。</summary>
        public const int ChipLocalAppData = 2;

        /// <summary>chip 序号：D:\Program Files (x86)（所有用户；没有 D 盘时不可用）。</summary>
        public const int ChipDriveD = 3;

        /// <summary>chip 3 的固定根目录。</summary>
        private const string DriveDRoot = @"D:\Program Files (x86)";

        /// <summary>
        /// 按本机实际情况显示/隐藏 chip。
        ///
        /// 现在只有三个选项：<c>C:\Program Files (x86)</c> / <c>AppData</c> / <c>D:\Program Files (x86)</c>。
        /// 原来那个「Program Files」已经被删掉了：安装器是 **x86 进程**，
        /// <c>SpecialFolder.ProgramFiles</c> 在 64 位系统上会被 WOW64 重定向到
        /// <c>Program Files (x86)</c> —— 它和「Program Files (x86)」选出来的是**同一个目录**，
        /// 用户点了发现"两个没区别"（实测反馈）。留着只会误导。
        /// </summary>
        public void ApplyChipAvailability()
        {
            // chip4 是停用的老「Program Files」，设计稿里的位置留着但不显示
            chip4.Visible = false;

            chip3.Visible = DriveExists(DriveDRoot);
        }

        /// <summary>根目录所在盘符是否可用（没有 D 盘就不要给「D:\Program Files」这个选项）。</summary>
        private static bool DriveExists(string path)
        {
            var root = Path.GetPathRoot(path);
            return !string.IsNullOrEmpty(root) && Directory.Exists(root);
        }

        /// <summary>
        /// 第 <paramref name="index"/> 个快捷路径 chip 的根目录。
        /// 抽出来是为了**能被单测和键盘访问直接打到** —— 只挂在鼠标事件上的话，
        /// "点了没反应"这种问题只能靠人手点一遍才能发现。
        ///
        /// 注意：这里**不带**"该目录属于所有用户还是当前用户"的信息。
        /// 点 chip 只换安装位置，不动「为所有用户安装」—— 那是用户自己的选择
        /// （用户明确要求过：别替我勾）。安装范围与目录不一致时，
        /// 安装/卸载都会按"目录写不写得进去"自己决定要不要提权，不会装坏。
        /// </summary>
        /// <param name="index">见 <see cref="ChipProgramFilesX86"/> / <see cref="ChipLocalAppData"/> / <see cref="ChipDriveD"/>。</param>
        /// <param name="root">根目录（取不到时为 null）。</param>
        public bool TryGetChipTarget(int index, out string root)
        {
            root = null;

            switch (index)
            {
                case ChipProgramFilesX86:
                    // 用 ProgramFilesX86：它在 32/64 位进程里都指向真正的 (x86) 目录。
                    // 不能用 ProgramFiles —— 32 位进程里那是同一个目录（WOW64 重定向）。
                    root = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);

                    // 极老的 32 位系统没有 (x86) 目录，退回 ProgramFiles
                    if (string.IsNullOrEmpty(root))
                    {
                        root = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
                    }

                    break;

                case ChipLocalAppData:
                    root = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                    break;

                case ChipDriveD:
                    root = DriveDRoot;
                    break;

                default:
                    return false;
            }

            if (string.IsNullOrEmpty(root))
            {
                return false;
            }

            // 盘符不存在的路径点了也没用（会装不进去）
            return DriveExists(root);
        }

        /// <summary>
        /// 触发第 <paramref name="index"/> 个快捷路径 chip。
        /// 点击与键盘/测试都走这里，行为只有一份。
        /// </summary>
        public void ActivateChip(int index)
        {
            string root;
            if (!TryGetChipTarget(index, out root))
            {
                // 取不到目录（或盘符不存在）就别自作主张
                return;
            }

            var handler = PathChipClicked;
            if (handler != null)
            {
                handler(root);
            }
        }

        /// <summary>给一个 chip 挂上"能点"的全部表现：手型、悬停变色、点击回调。</summary>
        private void HookChip(AntdUI.Panel chip, AntdUI.Label icon, AntdUI.Label text, int index)
        {
            var controls = new Control[] { chip, icon, text };

            foreach (var c in controls)
            {
                c.Cursor = Cursors.Hand;
                c.MouseEnter += (sender, e) => chip.Back = WizardTheme.PrimarySoft;
                c.MouseLeave += (sender, e) => chip.Back = WizardTheme.SoftBack;
                c.MouseClick += (sender, e) =>
                {
                    if (e.Button != MouseButtons.Left)
                    {
                        return;
                    }

                    ActivateChip(index);
                };
            }
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
