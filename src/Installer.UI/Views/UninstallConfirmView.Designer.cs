namespace Installer.UI.Views
{
    partial class UninstallConfirmView
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
        /// 本页坐标逐元素照抄 Calicat 设计稿第 3 版「确认卸载」（688×764），不做二次推导。
        ///
        /// 坐标系换算（与 WelcomeView 完全一致）：
        ///   窗口卡片在 (24,24) 宽 640；卡片内从上到下是 头部 104 / 步骤条 64 / 内容 / 底栏 72。
        ///   内容区 = (24,192) 640×(窗口高−288)，内边距 32 → 正文坐标系原点 = 窗口 (56,192)。
        ///   本视图尺寸 = 576 × (764 − 288) = 576 × 476。
        ///   **控件 Location = 设计稿绝对坐标 − (56,192)**，Size 直接照抄设计稿 px。
        ///
        /// 换算表（设计稿绝对坐标 → 本视图坐标）：
        ///   页面标题   (56,224) -> (0, 32)   576x26   18px = 13.5pt Bold   #1F2937
        ///   副标题     (56,262) -> (0, 70)   576x22   13px = 9.75pt Regular #4B5563
        ///   信息卡     (56,306) -> (0,114)   576x186  圆角14 底#F5F7FA 内边距20
        ///     行1/2/3 行距 54，行高 38（卡片内 20/74/128）；图标块 32x32 圆角9 底#E6F4FF，行内 (0,3)
        ///       图标  行内 (6,3) 24x24 承载框，IconRatio 1.25 → 墨迹落回设计稿的 15x13
        ///       标签  行内 (44, 0)  12px = 9pt    #9CA3AF
        ///       值    行内 (44,18)  13px = 9.75pt #1F2937
        ///   复选框     (56,522) -> (0,330)   20x20
        ///   复选框标题 (88,512) -> (32,320)  12px = 9pt Bold   #1F2937
        ///   复选框提示 (88,534) -> (32,342)  12px = 9pt Regular #9CA3AF
        ///   警告条     (56,572) -> (0,380)   576x64   圆角10 底#FFF1F0 描边1px #FFDFDB
        ///       图标   条内 (16,8) 24x24 承载框，IconRatio 1.1 → 墨迹 13x12 @条内(17,14) #FF4D4F
        ///       行1    条内 (40,12) 12px = 9pt #D5303D
        ///       行2    条内 (40,34) 12px = 9pt #D5303D
        ///
        /// ⚠ 两条 AntdUI 实测坑（都写进了注释，别再踩）：
        ///   1) 内边距一律算进 Location，**不要用 Panel.Padding**：AntdUI.Panel 重写了
        ///      DisplayRectangle，子控件按它自己的 padding 字段排版，WinForms 的
        ///      Control.Padding 对子控件不生效（实测会让内容整体向左上偏 20px）。
        ///   2) AntdUI 把 PrefixSvg 按固定像素绘制并锚在 Label 左上，与 Label 尺寸无关；
        ///      默认画出来比设计稿小 ~25%，所以图标要显式给 IconRatio（见上面的取值）。
        ///
        /// 页面尺寸固定，所以这里用绝对定位而不是嵌套布局容器 ——
        /// 一是能和设计稿一一对应，二是嵌套容器的不透明背景会盖掉父面板的圆角。
        /// </remarks>
        private void InitializeComponent()
        {
            this.lblTitle = new AntdUI.Label();
            this.lblSubtitle = new AntdUI.Label();
            this.infoCard = new AntdUI.Panel();
            this.row1 = new AntdUI.Panel();
            this.boxInfo1 = new AntdUI.Panel();
            this.iconInfo1 = new AntdUI.Label();
            this.lblCaption1 = new AntdUI.Label();
            this.lblValue1 = new AntdUI.Label();
            this.row2 = new AntdUI.Panel();
            this.boxInfo2 = new AntdUI.Panel();
            this.iconInfo2 = new AntdUI.Label();
            this.lblCaption2 = new AntdUI.Label();
            this.lblValue2 = new AntdUI.Label();
            this.row3 = new AntdUI.Panel();
            this.boxInfo3 = new AntdUI.Panel();
            this.iconInfo3 = new AntdUI.Label();
            this.lblCaption3 = new AntdUI.Label();
            this.lblValue3 = new AntdUI.Label();
            this.chkCleanup = new AntdUI.Checkbox();
            this.lblCleanupText = new AntdUI.Label();
            this.lblCleanupHint = new AntdUI.Label();
            this.warnPanel = new AntdUI.Panel();
            this.lblWarnIcon = new AntdUI.Label();
            this.lblWarnLine1 = new AntdUI.Label();
            this.lblWarnLine2 = new AntdUI.Label();
            this.infoCard.SuspendLayout();
            this.row1.SuspendLayout();
            this.boxInfo1.SuspendLayout();
            this.row2.SuspendLayout();
            this.boxInfo2.SuspendLayout();
            this.row3.SuspendLayout();
            this.boxInfo3.SuspendLayout();
            this.warnPanel.SuspendLayout();
            this.SuspendLayout();
            //
            // lblTitle —— 设计稿 (56,224) 576x26，18px = 13.5pt Bold #1F2937
            //
            this.lblTitle.AutoSize = false;
            this.lblTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblTitle.Font = new System.Drawing.Font("微软雅黑", 13.5F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(0, 32);
            this.lblTitle.Margin = new System.Windows.Forms.Padding(0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(576, 26);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "卸载「空中小火车」";
            //
            // lblSubtitle —— 设计稿 (56,262) 576x22，13px = 9.75pt Regular #4B5563
            //
            this.lblSubtitle.AutoSize = false;
            this.lblSubtitle.BackColor = System.Drawing.Color.Transparent;
            this.lblSubtitle.Font = new System.Drawing.Font("微软雅黑", 9.75F);
            this.lblSubtitle.Location = new System.Drawing.Point(0, 70);
            this.lblSubtitle.Margin = new System.Windows.Forms.Padding(0);
            this.lblSubtitle.Name = "lblSubtitle";
            this.lblSubtitle.Size = new System.Drawing.Size(576, 22);
            this.lblSubtitle.TabIndex = 1;
            this.lblSubtitle.Text = "卸载程序将移除「空中小火车 1.0.0」的程序文件、快捷方式与注册表项。";
            //
            // infoCard —— 设计稿 (56,306) 576x186，圆角 14，底 #F5F7FA，内边距 20
            //
            // 只设 AntdUI 的 Back，不设 WinForms 的 BackColor（设了会用直角盖掉圆角）。
            //
            // 注意：**不要用 Padding 来做内边距**。AntdUI.Panel 重写了 DisplayRectangle，
            // 子控件按它自己的 padding 字段排版，WinForms 的 Control.Padding 对子控件无效
            // —— 实测设了 Padding(20) 后行仍然贴在 (0,0)，整块内容会向左上偏 20px。
            // 所以这里把 20 的内边距直接算进行坐标：行1/2/3 = (20,20) / (20,74) / (20,128)。
            //
            this.infoCard.Controls.Add(this.row3);
            this.infoCard.Controls.Add(this.row2);
            this.infoCard.Controls.Add(this.row1);
            this.infoCard.Location = new System.Drawing.Point(0, 114);
            this.infoCard.Margin = new System.Windows.Forms.Padding(0);
            this.infoCard.Name = "infoCard";
            this.infoCard.Radius = 14;
            this.infoCard.Size = new System.Drawing.Size(576, 186);
            this.infoCard.TabIndex = 2;
            //
            // row1 / row2 / row3 —— 设计稿 y 326 / 380 / 434（卡片内 20 / 74 / 128），每行 536x38
            //
            this.row1.Location = new System.Drawing.Point(20, 20);
            this.row1.Margin = new System.Windows.Forms.Padding(0);
            this.row1.Name = "row1";
            this.row1.Size = new System.Drawing.Size(536, 38);
            this.row1.TabIndex = 0;
            this.row2.Location = new System.Drawing.Point(20, 74);
            this.row2.Margin = new System.Windows.Forms.Padding(0);
            this.row2.Name = "row2";
            this.row2.Size = new System.Drawing.Size(536, 38);
            this.row2.TabIndex = 1;
            this.row3.Location = new System.Drawing.Point(20, 128);
            this.row3.Margin = new System.Windows.Forms.Padding(0);
            this.row3.Name = "row3";
            this.row3.Size = new System.Drawing.Size(536, 38);
            this.row3.TabIndex = 2;
            //
            // boxInfo1 / boxInfo2 / boxInfo3 —— 设计稿 32x32，圆角 9，底 #E6F4FF，行内 (0,3)
            //
            this.boxInfo1.Controls.Add(this.iconInfo1);
            this.boxInfo1.Location = new System.Drawing.Point(0, 3);
            this.boxInfo1.Margin = new System.Windows.Forms.Padding(0);
            this.boxInfo1.Name = "boxInfo1";
            this.boxInfo1.Radius = 9;
            this.boxInfo1.Size = new System.Drawing.Size(32, 32);
            this.boxInfo1.TabIndex = 0;
            this.boxInfo2.Controls.Add(this.iconInfo2);
            this.boxInfo2.Location = new System.Drawing.Point(0, 3);
            this.boxInfo2.Margin = new System.Windows.Forms.Padding(0);
            this.boxInfo2.Name = "boxInfo2";
            this.boxInfo2.Radius = 9;
            this.boxInfo2.Size = new System.Drawing.Size(32, 32);
            this.boxInfo2.TabIndex = 0;
            this.boxInfo3.Controls.Add(this.iconInfo3);
            this.boxInfo3.Location = new System.Drawing.Point(0, 3);
            this.boxInfo3.Margin = new System.Windows.Forms.Padding(0);
            this.boxInfo3.Name = "boxInfo3";
            this.boxInfo3.Radius = 9;
            this.boxInfo3.Size = new System.Drawing.Size(32, 32);
            this.boxInfo3.TabIndex = 0;
            //
            // iconInfo1 / iconInfo2 / iconInfo3 —— 设计稿 24x24 居中于 32x32 块内，描边 #1677FF
            //
            this.iconInfo1.AutoSize = false;
            this.iconInfo1.BackColor = System.Drawing.Color.Transparent;
            this.iconInfo1.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.iconInfo1.IconGap = 0;
            this.iconInfo1.IconRatio = 1.25F;
            this.iconInfo1.Location = new System.Drawing.Point(6, 3);
            this.iconInfo1.Margin = new System.Windows.Forms.Padding(0);
            this.iconInfo1.Name = "iconInfo1";
            this.iconInfo1.PrefixSvg = "<svg viewBox=\"0 0 24 24\"><path d=\"M3 6.4h6l2.4 2.4h9.6v11.2H3z\" fill=\"none\" stroke=\"#1677FF\" stroke-width=\"1.8\" stroke-linejoin=\"round\"/></svg>";
            this.iconInfo1.Size = new System.Drawing.Size(24, 24);
            this.iconInfo1.TabIndex = 0;
            this.iconInfo1.Text = " ";
            this.iconInfo2.AutoSize = false;
            this.iconInfo2.BackColor = System.Drawing.Color.Transparent;
            this.iconInfo2.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.iconInfo2.IconGap = 0;
            this.iconInfo2.IconRatio = 1.25F;
            this.iconInfo2.Location = new System.Drawing.Point(6, 3);
            this.iconInfo2.Margin = new System.Windows.Forms.Padding(0);
            this.iconInfo2.Name = "iconInfo2";
            this.iconInfo2.PrefixSvg = "<svg viewBox=\"0 0 24 24\"><rect x=\"3\" y=\"4\" width=\"18\" height=\"16\" rx=\"2.4\" fill=\"none\" stroke=\"#1677FF\" stroke-width=\"1.8\"/><path d=\"M3 13.8h18\" stroke=\"#1677FF\" stroke-width=\"1.8\"/><circle cx=\"14.2\" cy=\"16.4\" r=\"1\" fill=\"#1677FF\"/></svg>";
            this.iconInfo2.Size = new System.Drawing.Size(24, 24);
            this.iconInfo2.TabIndex = 0;
            this.iconInfo2.Text = " ";
            this.iconInfo3.AutoSize = false;
            this.iconInfo3.BackColor = System.Drawing.Color.Transparent;
            this.iconInfo3.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.iconInfo3.IconGap = 0;
            this.iconInfo3.IconRatio = 1.25F;
            this.iconInfo3.Location = new System.Drawing.Point(6, 3);
            this.iconInfo3.Margin = new System.Windows.Forms.Padding(0);
            this.iconInfo3.Name = "iconInfo3";
            this.iconInfo3.PrefixSvg = "<svg viewBox=\"0 0 24 24\"><rect x=\"3\" y=\"5.2\" width=\"18\" height=\"14.8\" rx=\"2.2\" fill=\"none\" stroke=\"#1677FF\" stroke-width=\"1.8\"/><path d=\"M3 9.8h18\" stroke=\"#1677FF\" stroke-width=\"1.8\"/><path d=\"M9.3 3.4v3.6M16.1 3.4v3.6\" stroke=\"#1677FF\" stroke-width=\"1.8\" stroke-linecap=\"round\"/></svg>";
            this.iconInfo3.Size = new System.Drawing.Size(24, 24);
            this.iconInfo3.TabIndex = 0;
            this.iconInfo3.Text = " ";
            //
            // 标签 / 值 —— 设计稿文本列 x=120（图标块 32 + 间距 12，再加卡片内边距 20）
            //   标签 (44, 0)  12px = 9pt    #9CA3AF
            //   值   (44,18)  13px = 9.75pt #1F2937
            //
            this.lblCaption1.AutoSize = false;
            this.lblCaption1.BackColor = System.Drawing.Color.Transparent;
            this.lblCaption1.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.lblCaption1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(163)))), ((int)(((byte)(175)))));
            this.lblCaption1.Location = new System.Drawing.Point(44, 0);
            this.lblCaption1.Margin = new System.Windows.Forms.Padding(0);
            this.lblCaption1.Name = "lblCaption1";
            this.lblCaption1.Size = new System.Drawing.Size(492, 18);
            this.lblCaption1.TabIndex = 1;
            this.lblCaption1.Text = "当前安装位置";
            this.lblValue1.AutoSize = false;
            this.lblValue1.BackColor = System.Drawing.Color.Transparent;
            this.lblValue1.Font = new System.Drawing.Font("微软雅黑", 9.75F);
            this.lblValue1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(41)))), ((int)(((byte)(55)))));
            this.lblValue1.Location = new System.Drawing.Point(44, 18);
            this.lblValue1.Margin = new System.Windows.Forms.Padding(0);
            this.lblValue1.Name = "lblValue1";
            this.lblValue1.Size = new System.Drawing.Size(492, 20);
            this.lblValue1.TabIndex = 2;
            this.lblValue1.Text = "C:\\Program Files\\空中小火车";
            this.lblCaption2.AutoSize = false;
            this.lblCaption2.BackColor = System.Drawing.Color.Transparent;
            this.lblCaption2.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.lblCaption2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(163)))), ((int)(((byte)(175)))));
            this.lblCaption2.Location = new System.Drawing.Point(44, 0);
            this.lblCaption2.Margin = new System.Windows.Forms.Padding(0);
            this.lblCaption2.Name = "lblCaption2";
            this.lblCaption2.Size = new System.Drawing.Size(492, 18);
            this.lblCaption2.TabIndex = 1;
            this.lblCaption2.Text = "占用磁盘空间";
            this.lblValue2.AutoSize = false;
            this.lblValue2.BackColor = System.Drawing.Color.Transparent;
            this.lblValue2.Font = new System.Drawing.Font("微软雅黑", 9.75F);
            this.lblValue2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(41)))), ((int)(((byte)(55)))));
            this.lblValue2.Location = new System.Drawing.Point(44, 18);
            this.lblValue2.Margin = new System.Windows.Forms.Padding(0);
            this.lblValue2.Name = "lblValue2";
            this.lblValue2.Size = new System.Drawing.Size(492, 20);
            this.lblValue2.TabIndex = 2;
            this.lblValue2.Text = "90.6 MB（卸载后将释放）";
            this.lblCaption3.AutoSize = false;
            this.lblCaption3.BackColor = System.Drawing.Color.Transparent;
            this.lblCaption3.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.lblCaption3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(163)))), ((int)(((byte)(175)))));
            this.lblCaption3.Location = new System.Drawing.Point(44, 0);
            this.lblCaption3.Margin = new System.Windows.Forms.Padding(0);
            this.lblCaption3.Name = "lblCaption3";
            this.lblCaption3.Size = new System.Drawing.Size(492, 18);
            this.lblCaption3.TabIndex = 1;
            this.lblCaption3.Text = "安装时间与版本";
            this.lblValue3.AutoSize = false;
            this.lblValue3.BackColor = System.Drawing.Color.Transparent;
            this.lblValue3.Font = new System.Drawing.Font("微软雅黑", 9.75F);
            this.lblValue3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(41)))), ((int)(((byte)(55)))));
            this.lblValue3.Location = new System.Drawing.Point(44, 18);
            this.lblValue3.Margin = new System.Windows.Forms.Padding(0);
            this.lblValue3.Name = "lblValue3";
            this.lblValue3.Size = new System.Drawing.Size(492, 20);
            this.lblValue3.TabIndex = 2;
            this.lblValue3.Text = "2024-06-12 · 1.0.0";
            //
            // 把行内控件挂上去
            //
            this.row1.Controls.Add(this.lblValue1);
            this.row1.Controls.Add(this.lblCaption1);
            this.row1.Controls.Add(this.boxInfo1);
            this.row2.Controls.Add(this.lblValue2);
            this.row2.Controls.Add(this.lblCaption2);
            this.row2.Controls.Add(this.boxInfo2);
            this.row3.Controls.Add(this.lblValue3);
            this.row3.Controls.Add(this.lblCaption3);
            this.row3.Controls.Add(this.boxInfo3);
            //
            // chkCleanup —— 设计稿 (56,522) 20x20（垂直居中于下方两行文字）
            //
            // 勾选色由 ApplyAccent 设成 Danger（设计稿卸载向导的红色）。
            //
            this.chkCleanup.AutoSize = false;
            this.chkCleanup.BackColor = System.Drawing.Color.Transparent;
            this.chkCleanup.CheckedChanged += new AntdUI.BoolEventHandler(this.CleanupCheckedChanged);
            this.chkCleanup.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.chkCleanup.Location = new System.Drawing.Point(0, 330);
            this.chkCleanup.Margin = new System.Windows.Forms.Padding(0);
            this.chkCleanup.Name = "chkCleanup";
            this.chkCleanup.Size = new System.Drawing.Size(20, 20);
            this.chkCleanup.TabIndex = 3;
            this.chkCleanup.Text = " ";
            //
            // lblCleanupText —— 设计稿 (88,512)，即 (32,320)，12px = 9pt Bold #1F2937
            //
            this.lblCleanupText.AutoSize = false;
            this.lblCleanupText.BackColor = System.Drawing.Color.Transparent;
            this.lblCleanupText.Click += new System.EventHandler(this.CleanupTextClick);
            this.lblCleanupText.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold);
            this.lblCleanupText.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(41)))), ((int)(((byte)(55)))));
            this.lblCleanupText.Location = new System.Drawing.Point(32, 320);
            this.lblCleanupText.Margin = new System.Windows.Forms.Padding(0);
            this.lblCleanupText.Name = "lblCleanupText";
            this.lblCleanupText.Size = new System.Drawing.Size(544, 18);
            this.lblCleanupText.TabIndex = 4;
            this.lblCleanupText.Text = "同时删除用户配置与缓存数据";
            //
            // lblCleanupHint —— 设计稿 (88,534)，即 (32,342)，12px = 9pt Regular #9CA3AF
            //
            this.lblCleanupHint.AutoSize = false;
            this.lblCleanupHint.BackColor = System.Drawing.Color.Transparent;
            this.lblCleanupHint.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.lblCleanupHint.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(163)))), ((int)(((byte)(175)))));
            this.lblCleanupHint.Location = new System.Drawing.Point(32, 342);
            this.lblCleanupHint.Margin = new System.Windows.Forms.Padding(0);
            this.lblCleanupHint.Name = "lblCleanupHint";
            this.lblCleanupHint.Size = new System.Drawing.Size(544, 18);
            this.lblCleanupHint.TabIndex = 5;
            this.lblCleanupHint.Text = "包括个人设置、登录状态与本地缓存；取消勾选则保留，方便下次安装时继续使用。";
            //
            // warnPanel —— 设计稿 (56,572) 576x64，圆角 10，底 #FFF1F0，描边 1px #FFDFDB，内边距 12/16
            //
            // 同 infoCard：不用 Padding（AntdUI.Panel 的 Control.Padding 对子控件无效），
            // 内边距 16/12 直接算进行内坐标 —— 图标 (16,13)、行1 (40,12)、行2 (40,34)。
            //
            this.warnPanel.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(223)))), ((int)(((byte)(219)))));
            this.warnPanel.BorderWidth = 1F;
            this.warnPanel.Controls.Add(this.lblWarnLine2);
            this.warnPanel.Controls.Add(this.lblWarnLine1);
            this.warnPanel.Controls.Add(this.lblWarnIcon);
            this.warnPanel.Location = new System.Drawing.Point(0, 380);
            this.warnPanel.Margin = new System.Windows.Forms.Padding(0);
            this.warnPanel.Name = "warnPanel";
            this.warnPanel.Radius = 10;
            this.warnPanel.Size = new System.Drawing.Size(576, 64);
            this.warnPanel.TabIndex = 6;
            //
            // lblWarnIcon —— 设计稿 ⚠ 墨迹位于 (73,586)~(85,597)，即条内 (17,14)，13x12
            //
            // 24x24 的承载框放在条内 (16,8)：AntdUI 把 PrefixSvg 以固定像素绘制并锚在框的左上，
            // 实测墨迹正好落在条内 (17,14)。图标框尺寸本身不参与设计稿对齐，只影响墨迹落点。
            // 描边用设计稿实测值 #FF4D4F（注意：设计稿里「红色填充」是 #FF4D4F，
            // 而警告条文字是更深的 #D5303D，两者不是同一个红）。
            //
            this.lblWarnIcon.AutoSize = false;
            this.lblWarnIcon.BackColor = System.Drawing.Color.Transparent;
            this.lblWarnIcon.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.lblWarnIcon.IconGap = 0;
            this.lblWarnIcon.IconRatio = 1.1F;
            this.lblWarnIcon.Location = new System.Drawing.Point(16, 8);
            this.lblWarnIcon.Margin = new System.Windows.Forms.Padding(0);
            this.lblWarnIcon.Name = "lblWarnIcon";
            this.lblWarnIcon.PrefixSvg = "<svg viewBox=\"0 0 24 24\"><path d=\"M12 3.6 21.2 19.4H2.8z\" fill=\"none\" stroke=\"#FF4D4F\" stroke-width=\"1.8\" stroke-linejoin=\"round\"/><path d=\"M12 9.6v4\" stroke=\"#FF4D4F\" stroke-width=\"1.8\" stroke-linecap=\"round\"/><circle cx=\"12\" cy=\"16.4\" r=\"1.05\" fill=\"#FF4D4F\"/></svg>";
            this.lblWarnIcon.Size = new System.Drawing.Size(24, 24);
            this.lblWarnIcon.TabIndex = 0;
            this.lblWarnIcon.Text = " ";
            //
            // lblWarnLine1 / lblWarnLine2 —— 设计稿 (96,584) 与 (96,606)，即条内 (40,12) 与 (40,34)
            //   12px = 9pt Regular #D5303D
            //
            this.lblWarnLine1.AutoSize = false;
            this.lblWarnLine1.BackColor = System.Drawing.Color.Transparent;
            this.lblWarnLine1.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.lblWarnLine1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(213)))), ((int)(((byte)(48)))), ((int)(((byte)(61)))));
            this.lblWarnLine1.Location = new System.Drawing.Point(40, 12);
            this.lblWarnLine1.Margin = new System.Windows.Forms.Padding(0);
            this.lblWarnLine1.Name = "lblWarnLine1";
            this.lblWarnLine1.Size = new System.Drawing.Size(520, 18);
            this.lblWarnLine1.TabIndex = 1;
            this.lblWarnLine1.Text = "卸载后程序将无法启动，桌面与开始菜单的快捷方式也会一并移除。";
            this.lblWarnLine2.AutoSize = false;
            this.lblWarnLine2.BackColor = System.Drawing.Color.Transparent;
            this.lblWarnLine2.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.lblWarnLine2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(213)))), ((int)(((byte)(48)))), ((int)(((byte)(61)))));
            this.lblWarnLine2.Location = new System.Drawing.Point(40, 34);
            this.lblWarnLine2.Margin = new System.Windows.Forms.Padding(0);
            this.lblWarnLine2.Name = "lblWarnLine2";
            this.lblWarnLine2.Size = new System.Drawing.Size(520, 18);
            this.lblWarnLine2.TabIndex = 2;
            this.lblWarnLine2.Text = "如需重新使用，请再次运行安装包进行安装。";
            //
            // UninstallConfirmView
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.warnPanel);
            this.Controls.Add(this.lblCleanupHint);
            this.Controls.Add(this.lblCleanupText);
            this.Controls.Add(this.chkCleanup);
            this.Controls.Add(this.infoCard);
            this.Controls.Add(this.lblSubtitle);
            this.Controls.Add(this.lblTitle);
            this.Font = new System.Drawing.Font("微软雅黑", 9.75F);
            this.Name = "UninstallConfirmView";
            this.Size = new System.Drawing.Size(576, 476);
            this.infoCard.ResumeLayout(false);
            this.row1.ResumeLayout(false);
            this.boxInfo1.ResumeLayout(false);
            this.row2.ResumeLayout(false);
            this.boxInfo2.ResumeLayout(false);
            this.row3.ResumeLayout(false);
            this.boxInfo3.ResumeLayout(false);
            this.warnPanel.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private AntdUI.Label lblTitle;
        private AntdUI.Label lblSubtitle;
        private AntdUI.Panel infoCard;
        private AntdUI.Panel row1;
        private AntdUI.Panel boxInfo1;
        private AntdUI.Label iconInfo1;
        private AntdUI.Label lblCaption1;
        private AntdUI.Label lblValue1;
        private AntdUI.Panel row2;
        private AntdUI.Panel boxInfo2;
        private AntdUI.Label iconInfo2;
        private AntdUI.Label lblCaption2;
        private AntdUI.Label lblValue2;
        private AntdUI.Panel row3;
        private AntdUI.Panel boxInfo3;
        private AntdUI.Label iconInfo3;
        private AntdUI.Label lblCaption3;
        private AntdUI.Label lblValue3;
        private AntdUI.Checkbox chkCleanup;
        private AntdUI.Label lblCleanupText;
        private AntdUI.Label lblCleanupHint;
        private AntdUI.Panel warnPanel;
        private AntdUI.Label lblWarnIcon;
        private AntdUI.Label lblWarnLine1;
        private AntdUI.Label lblWarnLine2;
    }
}
