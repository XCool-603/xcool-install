namespace Installer.UI.Views
{
    partial class PathView
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        /// <remarks>
        /// 本页坐标逐元素照抄 Calicat 设计稿第 3 版「安装设置2」（688x923），不做二次推导。
        ///
        /// 设计稿的内容区是 (24,192) 640x635、左右内边距 32，所以本视图坐标系原点在
        /// 窗口 (56,192)。下表左列是设计稿绝对坐标，右列是本视图坐标（＝ 绝对坐标 − (56,192)）：
        ///
        ///   页面标题     (56,224) -> (  0, 28)   576x26  18px = 13.5pt SemiBold #1F2937
        ///   副标题       (56,260) -> (  0, 65)   576x22  13px = 9.75pt Regular  #4B5563
        ///   位置图标     (56,306) -> (  0,114)    16x16   蓝描边文件夹
        ///   「安装位置」 (83,305) -> ( 26,108)   300x22  14px = 10.5pt SemiBold #1F2937
        ///   路径输入框   (56,336) -> (  0,144)   476x40  圆角 8，边框 2px #1677FF
        ///   浏览按钮     (544,336)-> (488,144)    88x40  圆角 8，1px #E5E7EB
        ///   空间提示     (56,388) -> (  0,194)   576x18  13px = 9.75pt Regular  #9CA3AF
        ///   快捷路径     (56,416) -> (  0,224)  高 30   圆角 8，底 #F5F7FA
        ///     chip1 (56,416) 121 宽 / chip2 (185,416) 151 宽
        ///     chip3 (344,416) 119 宽 / chip4 (471,416) 137 宽，间距 8
        ///     每个 chip 内：图标 (14,6) 16x16、文字 (36,7) 13px = 9.75pt #4B5563
        ///   分隔线       (56,470) -> (  0,278)   576x1   #E5E8EF
        ///   任务图标     (56,499) -> (  0,307)    16x16   蓝描边清单
        ///   「附加任务」 (83,498) -> ( 26,301)   300x22  14px = 10.5pt SemiBold #1F2937
        ///   任务卡 2x2   282x68 圆角 12，行距 12 / 列距 12：
        ///     桌面   (56,529) -> (  0,337)   开始菜单 (350,529) -> (294,337)
        ///     自启   (56,609) -> (  0,417)   完成后运行(350,609) -> (294,417)
        ///   为所有用户安装
        ///     复选框 (56,707) -> (  0,515)    20x20
        ///     文字   (87,700) -> ( 31,504)   400x22  13px = 9.75pt Regular #1F2937
        ///     灰提示 (86,722) -> ( 30,527)   480x18  12px = 9pt    Regular #9CA3AF
        ///   信息条       (56,757) -> (  0,565)   576x42  圆角 10，底 #E6F4FF
        ///     图标   条内 (16,13) 16x16 / 文字 条内 (40,13) 520x16
        ///                                         12px = 9pt Regular #0958D9
        ///
        /// 注意：上表右列的 y 普遍比「设计稿绝对 y − 192」再小 3~5px。原因是
        /// AntdUI.Label 的文字基线比控件顶边低 3~6px（随字号变化），所以要减掉这段
        /// 行内偏移，字形才会正好落在设计稿的位置。这套偏移量是用「渲染 → 量字形
        /// 包围盒 → 与设计稿对齐」反推出来的；欢迎页的标题也是同一套（label y=32
        /// 对应设计稿字形顶 229）。
        ///
        /// 字号换算 pt = 设计稿 px × 0.75（96 DPI）。
        ///
        /// 页面尺寸固定，所以这里用绝对定位而不是嵌套布局容器 ——
        /// 一是能和设计稿一一对应，二是嵌套容器的不透明背景会盖掉父面板的圆角。
        /// AntdUI.Panel 只设 Back，绝不设 WinForms 的 BackColor（设了会用直角盖掉圆角）；
        /// 面板里的 AntdUI.Label 一律 BackColor = Transparent，否则各自画一块白底。
        /// </remarks>
        private void InitializeComponent()
        {
            this.lblTitle = new AntdUI.Label();
            this.lblSubtitle = new AntdUI.Label();
            this.iconPath = new AntdUI.Label();
            this.lblPathCaption = new AntdUI.Label();
            this.txtPath = new AntdUI.Input();
            this.btnBrowse = new AntdUI.Button();
            this.lblSpace = new AntdUI.Label();
            this.chip1 = new AntdUI.Panel();
            this.chip1Icon = new AntdUI.Label();
            this.chip1Text = new AntdUI.Label();
            this.chip2 = new AntdUI.Panel();
            this.chip2Icon = new AntdUI.Label();
            this.chip2Text = new AntdUI.Label();
            this.chip3 = new AntdUI.Panel();
            this.chip3Icon = new AntdUI.Label();
            this.chip3Text = new AntdUI.Label();
            this.chip4 = new AntdUI.Panel();
            this.chip4Icon = new AntdUI.Label();
            this.chip4Text = new AntdUI.Label();
            this.divider = new System.Windows.Forms.Panel();
            this.iconTask = new AntdUI.Label();
            this.lblTaskCaption = new AntdUI.Label();
            this.cardDesktop = new Installer.UI.Views.TaskCardView();
            this.cardStartMenu = new Installer.UI.Views.TaskCardView();
            this.cardAutostart = new Installer.UI.Views.TaskCardView();
            this.cardRunAfter = new Installer.UI.Views.TaskCardView();
            this.chkAllUsers = new AntdUI.Checkbox();
            this.lblAllUsers = new AntdUI.Label();
            this.lblAllUsersHint = new AntdUI.Label();
            this.infoBar = new AntdUI.Panel();
            this.lblInfoIcon = new AntdUI.Label();
            this.lblInfo = new AntdUI.Label();
            this.infoBar.SuspendLayout();
            this.chip1.SuspendLayout();
            this.chip2.SuspendLayout();
            this.chip3.SuspendLayout();
            this.chip4.SuspendLayout();
            this.SuspendLayout();
            //
            // lblTitle —— 设计稿字形顶 (56,225)，label y 取 28 抵消行内偏移，18px = 13.5pt SemiBold
            //
            this.lblTitle.AutoSize = false;
            this.lblTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblTitle.Font = new System.Drawing.Font("微软雅黑", 13.5F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(41)))), ((int)(((byte)(55)))));
            this.lblTitle.Location = new System.Drawing.Point(0, 28);
            this.lblTitle.Margin = new System.Windows.Forms.Padding(0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(576, 26);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "选择安装位置与附加任务";
            //
            // lblSubtitle —— 设计稿字形顶 (56,261)，13px = 9.75pt #4B5563
            //
            this.lblSubtitle.AutoSize = false;
            this.lblSubtitle.BackColor = System.Drawing.Color.Transparent;
            this.lblSubtitle.Font = new System.Drawing.Font("微软雅黑", 9.75F);
            this.lblSubtitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(85)))), ((int)(((byte)(99)))));
            this.lblSubtitle.Location = new System.Drawing.Point(0, 65);
            this.lblSubtitle.Margin = new System.Windows.Forms.Padding(0);
            this.lblSubtitle.Name = "lblSubtitle";
            this.lblSubtitle.Size = new System.Drawing.Size(576, 22);
            this.lblSubtitle.TabIndex = 1;
            this.lblSubtitle.Text = "确认安装目录，并勾选安装时需要一并完成的操作。";
            //
            // iconPath —— 设计稿 (56,306) 16x16，蓝描边文件夹
            //
            this.iconPath.AutoSize = false;
            this.iconPath.BackColor = System.Drawing.Color.Transparent;
            this.iconPath.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.iconPath.IconGap = 0;
            this.iconPath.Location = new System.Drawing.Point(0, 114);
            this.iconPath.Margin = new System.Windows.Forms.Padding(0);
            this.iconPath.Name = "iconPath";
            this.iconPath.PrefixSvg = "<svg viewBox=\"0 0 16 16\"><path d=\"M1.7 3.2h4.3l1.5 1.6h6.8v7.9H1.7z\" fill=\"none\" stroke=\"#1677FF\" stroke-width=\"1.7\" stroke-linejoin=\"round\"/></svg>";
            this.iconPath.Size = new System.Drawing.Size(16, 16);
            this.iconPath.TabIndex = 2;
            this.iconPath.Text = " ";
            //
            // lblPathCaption —— 设计稿字形顶 (83,306)，14px = 10.5pt SemiBold
            //
            this.lblPathCaption.AutoSize = false;
            this.lblPathCaption.BackColor = System.Drawing.Color.Transparent;
            this.lblPathCaption.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Bold);
            this.lblPathCaption.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(41)))), ((int)(((byte)(55)))));
            this.lblPathCaption.Location = new System.Drawing.Point(26, 108);
            this.lblPathCaption.Margin = new System.Windows.Forms.Padding(0);
            this.lblPathCaption.Name = "lblPathCaption";
            this.lblPathCaption.Size = new System.Drawing.Size(300, 22);
            this.lblPathCaption.TabIndex = 3;
            this.lblPathCaption.Text = "安装位置";
            //
            // txtPath —— 设计稿 (56,336) 476x40，圆角 8，2px #1677FF 边框
            //
            this.txtPath.AutoSize = false;
            this.txtPath.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(119)))), ((int)(((byte)(255)))));
            this.txtPath.BorderWidth = 2F;
            this.txtPath.Font = new System.Drawing.Font("微软雅黑", 9.75F);
            this.txtPath.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(41)))), ((int)(((byte)(55)))));
            // 注意：AntdUI.Input.IconGap 和 Button 一样是「图标与文字间距比例」不是像素。
            this.txtPath.IconGap = 0.6F;
            this.txtPath.Location = new System.Drawing.Point(0, 144);
            this.txtPath.Margin = new System.Windows.Forms.Padding(0);
            this.txtPath.Name = "txtPath";
            this.txtPath.PrefixSvg = "<svg viewBox=\"0 0 16 16\"><path d=\"M1.7 3.6h4.3l1.5 1.6h6.8v7.2H1.7z\" fill=\"none\" stroke=\"#9CA3AF\" stroke-width=\"1.4\" stroke-linejoin=\"round\"/></svg>";
            this.txtPath.Radius = 8;
            this.txtPath.Size = new System.Drawing.Size(476, 40);
            this.txtPath.TabIndex = 4;
            this.txtPath.Text = "C:\\Program Files\\空中小火车";
            this.txtPath.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            //
            // btnBrowse —— 设计稿 (544,336) 88x40，圆角 8，1px #E5E7EB
            //
            this.btnBrowse.AutoEllipsis = false;
            this.btnBrowse.AutoSize = false;
            this.btnBrowse.BorderWidth = 1F;
            this.btnBrowse.DefaultBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(231)))), ((int)(((byte)(235)))));
            this.btnBrowse.Font = new System.Drawing.Font("微软雅黑", 9.75F);
            this.btnBrowse.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(85)))), ((int)(((byte)(99)))));
            // 注意：AntdUI.Button.IconGap 是「图标与文字间距比例」不是像素，写 6F 会把文字推到按钮外面。
            // IconSize 也不要显式设：设了之后 AntdUI 量文字可用宽度会算错，把「浏览」截成「浏览…」。
            this.btnBrowse.IconGap = 0.3F;
            this.btnBrowse.IconSvg = "<svg viewBox=\"0 0 16 16\"><path d=\"M1.7 3.6h4.3l1.5 1.6h6.8v7.2H1.7z\" fill=\"none\" stroke=\"#4B5563\" stroke-width=\"1.4\" stroke-linejoin=\"round\"/></svg>";
            this.btnBrowse.Location = new System.Drawing.Point(488, 144);
            this.btnBrowse.Margin = new System.Windows.Forms.Padding(0);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Radius = 8;
            this.btnBrowse.Size = new System.Drawing.Size(88, 40);
            this.btnBrowse.TabIndex = 5;
            this.btnBrowse.Text = "浏览";
            //
            // lblSpace —— 设计稿字形顶 (56,389)，13px = 9.75pt #9CA3AF
            //
            this.lblSpace.AutoSize = false;
            this.lblSpace.BackColor = System.Drawing.Color.Transparent;
            this.lblSpace.Font = new System.Drawing.Font("微软雅黑", 9.75F);
            this.lblSpace.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(163)))), ((int)(((byte)(175)))));
            this.lblSpace.IconGap = 8;
            this.lblSpace.Location = new System.Drawing.Point(0, 194);
            this.lblSpace.Margin = new System.Windows.Forms.Padding(0);
            this.lblSpace.Name = "lblSpace";
            this.lblSpace.PrefixSvg = "<svg viewBox=\"0 0 16 16\"><rect x=\"1.6\" y=\"3.4\" width=\"12.8\" height=\"9.2\" rx=\"1.6\" fill=\"none\" stroke=\"#9CA3AF\" stroke-width=\"1.4\"/><circle cx=\"11.4\" cy=\"8\" r=\"1.1\" fill=\"#9CA3AF\"/></svg>";
            this.lblSpace.Size = new System.Drawing.Size(576, 18);
            this.lblSpace.TabIndex = 6;
            this.lblSpace.Text = "所需空间 90.6 MB  ·  C 盘可用 128.4 GB";
            //
            // chip1..chip4 —— 设计稿 y=416 高 30，圆角 8，底 #F5F7FA，间距 8
            //
            // 用 Panel + 两个 Label 而不是 AntdUI.Button：AntdUI.Button 的内容无论
            // TextAlign 怎么设都按居中绘制，而设计稿里 chip 的图标/文字是左对齐的
            // （图标在 chip 内 (14,6)，文字在 chip 内 (36,7)）。
            //
            this.chip1.Controls.Add(this.chip1Text);
            this.chip1.Controls.Add(this.chip1Icon);
            this.chip1.Back = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(247)))), ((int)(((byte)(250)))));
            this.chip1.Location = new System.Drawing.Point(0, 224);
            this.chip1.Margin = new System.Windows.Forms.Padding(0);
            this.chip1.Name = "chip1";
            this.chip1.Radius = 8;
            this.chip1.Size = new System.Drawing.Size(172, 30);
            this.chip1.TabIndex = 7;
            this.chip1Icon.AutoSize = false;
            this.chip1Icon.BackColor = System.Drawing.Color.Transparent;
            this.chip1Icon.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.chip1Icon.IconGap = 0;
            this.chip1Icon.Location = new System.Drawing.Point(14, 6);
            this.chip1Icon.Margin = new System.Windows.Forms.Padding(0);
            this.chip1Icon.Name = "chip1Icon";
            this.chip1Icon.PrefixSvg = "<svg viewBox=\"0 0 16 16\"><path d=\"M1.7 3.6h4.3l1.5 1.6h6.8v7.2H1.7z\" fill=\"none\" stroke=\"#6B7280\" stroke-width=\"1.4\" stroke-linejoin=\"round\"/></svg>";
            this.chip1Icon.Size = new System.Drawing.Size(16, 16);
            this.chip1Icon.TabIndex = 0;
            this.chip1Icon.Text = " ";
            // AutoSize 必须为 true：AntdUI.Label 在固定宽度下会把文字截掉（内部有约 20px 的
            // 文字区留白），chip 的文字宽度本来就刚好贴着设计稿，只能让它自己量。
            this.chip1Text.AutoSize = true;
            this.chip1Text.BackColor = System.Drawing.Color.Transparent;
            this.chip1Text.Font = new System.Drawing.Font("微软雅黑", 9.75F);
            this.chip1Text.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(85)))), ((int)(((byte)(99)))));
            this.chip1Text.Location = new System.Drawing.Point(36, 7);
            this.chip1Text.Margin = new System.Windows.Forms.Padding(0);
            this.chip1Text.Name = "chip1Text";
            this.chip1Text.Size = new System.Drawing.Size(124, 18);
            this.chip1Text.TabIndex = 1;
            this.chip1Text.Text = "C:\\Program Files (x86)";
            this.chip2.Controls.Add(this.chip2Text);
            this.chip2.Controls.Add(this.chip2Icon);
            this.chip2.Back = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(247)))), ((int)(((byte)(250)))));
            this.chip2.Location = new System.Drawing.Point(180, 224);
            this.chip2.Margin = new System.Windows.Forms.Padding(0);
            this.chip2.Name = "chip2";
            this.chip2.Radius = 8;
            this.chip2.Size = new System.Drawing.Size(110, 30);
            this.chip2.TabIndex = 8;
            this.chip2Icon.AutoSize = false;
            this.chip2Icon.BackColor = System.Drawing.Color.Transparent;
            this.chip2Icon.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.chip2Icon.IconGap = 0;
            this.chip2Icon.Location = new System.Drawing.Point(14, 6);
            this.chip2Icon.Margin = new System.Windows.Forms.Padding(0);
            this.chip2Icon.Name = "chip2Icon";
            this.chip2Icon.PrefixSvg = "<svg viewBox=\"0 0 16 16\"><path d=\"M1.7 3.6h4.3l1.5 1.6h6.8v7.2H1.7z\" fill=\"none\" stroke=\"#6B7280\" stroke-width=\"1.4\" stroke-linejoin=\"round\"/></svg>";
            this.chip2Icon.Size = new System.Drawing.Size(16, 16);
            this.chip2Icon.TabIndex = 0;
            this.chip2Icon.Text = " ";
            this.chip2Text.AutoSize = true;
            this.chip2Text.BackColor = System.Drawing.Color.Transparent;
            this.chip2Text.Font = new System.Drawing.Font("微软雅黑", 9.75F);
            this.chip2Text.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(85)))), ((int)(((byte)(99)))));
            this.chip2Text.Location = new System.Drawing.Point(36, 7);
            this.chip2Text.Margin = new System.Windows.Forms.Padding(0);
            this.chip2Text.Name = "chip2Text";
            this.chip2Text.Size = new System.Drawing.Size(58, 18);
            this.chip2Text.TabIndex = 1;
            this.chip2Text.Text = "AppData";
            this.chip3.Controls.Add(this.chip3Text);
            this.chip3.Controls.Add(this.chip3Icon);
            this.chip3.Back = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(247)))), ((int)(((byte)(250)))));
            this.chip3.Location = new System.Drawing.Point(298, 224);
            this.chip3.Margin = new System.Windows.Forms.Padding(0);
            this.chip3.Name = "chip3";
            this.chip3.Radius = 8;
            this.chip3.Size = new System.Drawing.Size(172, 30);
            this.chip3.TabIndex = 9;
            this.chip3Icon.AutoSize = false;
            this.chip3Icon.BackColor = System.Drawing.Color.Transparent;
            this.chip3Icon.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.chip3Icon.IconGap = 0;
            this.chip3Icon.Location = new System.Drawing.Point(14, 6);
            this.chip3Icon.Margin = new System.Windows.Forms.Padding(0);
            this.chip3Icon.Name = "chip3Icon";
            this.chip3Icon.PrefixSvg = "<svg viewBox=\"0 0 16 16\"><path d=\"M1.7 3.6h4.3l1.5 1.6h6.8v7.2H1.7z\" fill=\"none\" stroke=\"#6B7280\" stroke-width=\"1.4\" stroke-linejoin=\"round\"/></svg>";
            this.chip3Icon.Size = new System.Drawing.Size(16, 16);
            this.chip3Icon.TabIndex = 0;
            this.chip3Icon.Text = " ";
            this.chip3Text.AutoSize = true;
            this.chip3Text.BackColor = System.Drawing.Color.Transparent;
            this.chip3Text.Font = new System.Drawing.Font("微软雅黑", 9.75F);
            this.chip3Text.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(85)))), ((int)(((byte)(99)))));
            this.chip3Text.Location = new System.Drawing.Point(36, 7);
            this.chip3Text.Margin = new System.Windows.Forms.Padding(0);
            this.chip3Text.Name = "chip3Text";
            this.chip3Text.Size = new System.Drawing.Size(124, 18);
            this.chip3Text.TabIndex = 1;
            this.chip3Text.Text = "D:\\Program Files (x86)";
            this.chip4.Controls.Add(this.chip4Text);
            this.chip4.Controls.Add(this.chip4Icon);
            this.chip4.Back = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(247)))), ((int)(((byte)(250)))));
            this.chip4.Location = new System.Drawing.Point(415, 224);
            this.chip4.Margin = new System.Windows.Forms.Padding(0);
            this.chip4.Name = "chip4";
            this.chip4.Radius = 8;
            this.chip4.Size = new System.Drawing.Size(137, 30);
            this.chip4.TabIndex = 10;
            this.chip4.Visible = false;
            this.chip4Icon.AutoSize = false;
            this.chip4Icon.BackColor = System.Drawing.Color.Transparent;
            this.chip4Icon.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.chip4Icon.IconGap = 0;
            this.chip4Icon.Location = new System.Drawing.Point(14, 6);
            this.chip4Icon.Margin = new System.Windows.Forms.Padding(0);
            this.chip4Icon.Name = "chip4Icon";
            this.chip4Icon.PrefixSvg = "<svg viewBox=\"0 0 16 16\"><path d=\"M1.7 3.6h4.3l1.5 1.6h6.8v7.2H1.7z\" fill=\"none\" stroke=\"#6B7280\" stroke-width=\"1.4\" stroke-linejoin=\"round\"/></svg>";
            this.chip4Icon.Size = new System.Drawing.Size(16, 16);
            this.chip4Icon.TabIndex = 0;
            this.chip4Icon.Text = " ";
            this.chip4Text.AutoSize = true;
            this.chip4Text.BackColor = System.Drawing.Color.Transparent;
            this.chip4Text.Font = new System.Drawing.Font("微软雅黑", 9.75F);
            this.chip4Text.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(85)))), ((int)(((byte)(99)))));
            this.chip4Text.Location = new System.Drawing.Point(36, 7);
            this.chip4Text.Margin = new System.Windows.Forms.Padding(0);
            this.chip4Text.Name = "chip4Text";
            this.chip4Text.Size = new System.Drawing.Size(89, 18);
            this.chip4Text.TabIndex = 1;
            this.chip4Text.Text = "D:\\Program Files";
            //
            // divider —— 设计稿 (56,470) 576x1，#E5E8EF
            //
            this.divider.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(232)))), ((int)(((byte)(239)))));
            this.divider.Location = new System.Drawing.Point(0, 266);
            this.divider.Margin = new System.Windows.Forms.Padding(0);
            this.divider.Name = "divider";
            this.divider.Size = new System.Drawing.Size(576, 1);
            this.divider.TabIndex = 11;
            //
            // iconTask —— 设计稿 (56,499) 16x16，蓝描边清单
            //
            this.iconTask.AutoSize = false;
            this.iconTask.BackColor = System.Drawing.Color.Transparent;
            this.iconTask.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.iconTask.IconGap = 0;
            this.iconTask.Location = new System.Drawing.Point(0, 276);
            this.iconTask.Margin = new System.Windows.Forms.Padding(0);
            this.iconTask.Name = "iconTask";
            this.iconTask.PrefixSvg = "<svg viewBox=\"0 0 16 16\"><rect x=\"2\" y=\"3\" width=\"3.6\" height=\"3.6\" rx=\"0.9\" fill=\"#1677FF\"/><rect x=\"7.2\" y=\"4.1\" width=\"6.8\" height=\"1.5\" rx=\"0.75\" fill=\"#1677FF\"/><rect x=\"2\" y=\"9.4\" width=\"3.6\" height=\"3.6\" rx=\"0.9\" fill=\"#1677FF\"/><rect x=\"7.2\" y=\"10.5\" width=\"6.8\" height=\"1.5\" rx=\"0.75\" fill=\"#1677FF\"/></svg>";
            this.iconTask.Size = new System.Drawing.Size(16, 16);
            this.iconTask.TabIndex = 12;
            this.iconTask.Text = " ";
            //
            // lblTaskCaption —— 设计稿字形顶 (83,499)，14px = 10.5pt SemiBold
            //
            this.lblTaskCaption.AutoSize = false;
            this.lblTaskCaption.BackColor = System.Drawing.Color.Transparent;
            this.lblTaskCaption.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Bold);
            this.lblTaskCaption.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(41)))), ((int)(((byte)(55)))));
            this.lblTaskCaption.Location = new System.Drawing.Point(26, 270);
            this.lblTaskCaption.Margin = new System.Windows.Forms.Padding(0);
            this.lblTaskCaption.Name = "lblTaskCaption";
            this.lblTaskCaption.Size = new System.Drawing.Size(300, 22);
            this.lblTaskCaption.TabIndex = 13;
            this.lblTaskCaption.Text = "附加任务";
            //
            // cardDesktop / cardStartMenu / cardAutostart / cardRunAfter
            // 设计稿 282x68，列距 12 / 行距 12，卡内布局见 TaskCardView.Designer.cs
            //
            this.cardDesktop.Location = new System.Drawing.Point(0, 306);
            this.cardDesktop.Margin = new System.Windows.Forms.Padding(0);
            this.cardDesktop.Name = "cardDesktop";
            this.cardDesktop.Size = new System.Drawing.Size(282, 68);
            this.cardDesktop.TabIndex = 14;
            this.cardStartMenu.Location = new System.Drawing.Point(294, 306);
            this.cardStartMenu.Margin = new System.Windows.Forms.Padding(0);
            this.cardStartMenu.Name = "cardStartMenu";
            this.cardStartMenu.Size = new System.Drawing.Size(282, 68);
            this.cardStartMenu.TabIndex = 15;
            this.cardAutostart.Location = new System.Drawing.Point(0, 386);
            this.cardAutostart.Margin = new System.Windows.Forms.Padding(0);
            this.cardAutostart.Name = "cardAutostart";
            this.cardAutostart.Size = new System.Drawing.Size(282, 68);
            this.cardAutostart.TabIndex = 16;
            this.cardRunAfter.Location = new System.Drawing.Point(294, 386);
            this.cardRunAfter.Margin = new System.Windows.Forms.Padding(0);
            this.cardRunAfter.Name = "cardRunAfter";
            this.cardRunAfter.Size = new System.Drawing.Size(282, 68);
            this.cardRunAfter.TabIndex = 17;
            //
            // chkAllUsers —— 设计稿 (56,707) 20x20
            //
            this.chkAllUsers.AutoSize = false;
            this.chkAllUsers.Fill = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(119)))), ((int)(((byte)(255)))));
            this.chkAllUsers.Location = new System.Drawing.Point(0, 465);
            this.chkAllUsers.Margin = new System.Windows.Forms.Padding(0);
            this.chkAllUsers.Name = "chkAllUsers";
            this.chkAllUsers.Size = new System.Drawing.Size(20, 20);
            this.chkAllUsers.TabIndex = 18;
            this.chkAllUsers.Text = "";
            //
            // lblAllUsers —— 设计稿字形顶 (88,701)，13px = 9.75pt #1F2937
            //
            this.lblAllUsers.AutoSize = false;
            this.lblAllUsers.BackColor = System.Drawing.Color.Transparent;
            this.lblAllUsers.Font = new System.Drawing.Font("微软雅黑", 9.75F);
            this.lblAllUsers.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(41)))), ((int)(((byte)(55)))));
            this.lblAllUsers.Location = new System.Drawing.Point(32, 454);
            this.lblAllUsers.Margin = new System.Windows.Forms.Padding(0);
            this.lblAllUsers.Name = "lblAllUsers";
            this.lblAllUsers.Size = new System.Drawing.Size(400, 22);
            this.lblAllUsers.TabIndex = 19;
            this.lblAllUsers.Text = "为所有用户安装";
            //
            // lblAllUsersHint —— 设计稿字形顶 (87,723)，12px = 9pt #9CA3AF
            //
            this.lblAllUsersHint.AutoSize = false;
            this.lblAllUsersHint.BackColor = System.Drawing.Color.Transparent;
            this.lblAllUsersHint.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.lblAllUsersHint.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(163)))), ((int)(((byte)(175)))));
            this.lblAllUsersHint.Location = new System.Drawing.Point(32, 477);
            this.lblAllUsersHint.Margin = new System.Windows.Forms.Padding(0);
            this.lblAllUsersHint.Name = "lblAllUsersHint";
            this.lblAllUsersHint.Size = new System.Drawing.Size(480, 18);
            this.lblAllUsersHint.TabIndex = 20;
            this.lblAllUsersHint.Text = "需要对当前电脑的管理员权限，取消勾选则只为你自己安装。";
            //
            // infoBar —— 设计稿 (56,757) 576x42，圆角 10，底 #E6F4FF
            //
            this.infoBar.Controls.Add(this.lblInfo);
            this.infoBar.Controls.Add(this.lblInfoIcon);
            this.infoBar.Location = new System.Drawing.Point(0, 500);
            this.infoBar.Margin = new System.Windows.Forms.Padding(0);
            this.infoBar.Name = "infoBar";
            this.infoBar.Padding = new System.Windows.Forms.Padding(0);
            this.infoBar.Radius = 10;
            this.infoBar.Size = new System.Drawing.Size(576, 42);
            this.infoBar.TabIndex = 21;
            //
            // lblInfoIcon —— 条内 (16,13) 16x16，蓝描边对勾圆
            //
            this.lblInfoIcon.AutoSize = false;
            this.lblInfoIcon.BackColor = System.Drawing.Color.Transparent;
            this.lblInfoIcon.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.lblInfoIcon.IconGap = 0;
            this.lblInfoIcon.Location = new System.Drawing.Point(16, 13);
            this.lblInfoIcon.Margin = new System.Windows.Forms.Padding(0);
            this.lblInfoIcon.Name = "lblInfoIcon";
            this.lblInfoIcon.PrefixSvg = "<svg viewBox=\"0 0 16 16\"><circle cx=\"8\" cy=\"8\" r=\"7\" fill=\"none\" stroke=\"#1677FF\" stroke-width=\"1.7\"/><path d=\"M4.9 8.2 7.1 10.4 11.3 5.8\" fill=\"none\" stroke=\"#1677FF\" stroke-width=\"1.7\" stroke-linecap=\"round\" stroke-linejoin=\"round\"/></svg>";
            this.lblInfoIcon.Size = new System.Drawing.Size(16, 16);
            this.lblInfoIcon.TabIndex = 0;
            this.lblInfoIcon.Text = " ";
            //
            // lblInfo —— 条内 (40,13) 520x16，12px = 9pt #0958D9
            //
            this.lblInfo.AutoEllipsis = true;
            this.lblInfo.AutoSize = false;
            this.lblInfo.BackColor = System.Drawing.Color.Transparent;
            this.lblInfo.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.lblInfo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(9)))), ((int)(((byte)(88)))), ((int)(((byte)(217)))));
            this.lblInfo.Location = new System.Drawing.Point(40, 13);
            this.lblInfo.Margin = new System.Windows.Forms.Padding(0);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(520, 16);
            this.lblInfo.TabIndex = 1;
            this.lblInfo.Text = "将安装到 C:\\Program Files\\空中小火车  ·  已选择 3 项附加任务";
            //
            // PathView
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.infoBar);
            this.Controls.Add(this.lblAllUsersHint);
            this.Controls.Add(this.lblAllUsers);
            this.Controls.Add(this.chkAllUsers);
            this.Controls.Add(this.cardRunAfter);
            this.Controls.Add(this.cardAutostart);
            this.Controls.Add(this.cardStartMenu);
            this.Controls.Add(this.cardDesktop);
            this.Controls.Add(this.lblTaskCaption);
            this.Controls.Add(this.iconTask);
            this.Controls.Add(this.divider);
            this.Controls.Add(this.chip4);
            this.Controls.Add(this.chip3);
            this.Controls.Add(this.chip2);
            this.Controls.Add(this.chip1);
            this.Controls.Add(this.lblSpace);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.txtPath);
            this.Controls.Add(this.lblPathCaption);
            this.Controls.Add(this.iconPath);
            this.Controls.Add(this.lblSubtitle);
            this.Controls.Add(this.lblTitle);
            this.Font = new System.Drawing.Font("微软雅黑", 9.75F);
            this.Name = "PathView";
            this.Size = new System.Drawing.Size(576, 560);
            this.infoBar.ResumeLayout(false);
            this.chip1.ResumeLayout(false);
            this.chip2.ResumeLayout(false);
            this.chip3.ResumeLayout(false);
            this.chip4.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private AntdUI.Label lblTitle;
        private AntdUI.Label lblSubtitle;
        private AntdUI.Label iconPath;
        private AntdUI.Label lblPathCaption;
        private AntdUI.Input txtPath;
        private AntdUI.Button btnBrowse;
        private AntdUI.Label lblSpace;
        private AntdUI.Panel chip1;
        private AntdUI.Label chip1Icon;
        private AntdUI.Label chip1Text;
        private AntdUI.Panel chip2;
        private AntdUI.Label chip2Icon;
        private AntdUI.Label chip2Text;
        private AntdUI.Panel chip3;
        private AntdUI.Label chip3Icon;
        private AntdUI.Label chip3Text;
        private AntdUI.Panel chip4;
        private AntdUI.Label chip4Icon;
        private AntdUI.Label chip4Text;
        private System.Windows.Forms.Panel divider;
        private AntdUI.Label iconTask;
        private AntdUI.Label lblTaskCaption;
        private Installer.UI.Views.TaskCardView cardDesktop;
        private Installer.UI.Views.TaskCardView cardStartMenu;
        private Installer.UI.Views.TaskCardView cardAutostart;
        private Installer.UI.Views.TaskCardView cardRunAfter;
        private AntdUI.Checkbox chkAllUsers;
        private AntdUI.Label lblAllUsers;
        private AntdUI.Label lblAllUsersHint;
        private AntdUI.Panel infoBar;
        private AntdUI.Label lblInfoIcon;
        private AntdUI.Label lblInfo;
    }
}
