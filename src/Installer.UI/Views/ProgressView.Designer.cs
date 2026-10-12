namespace Installer.UI.Views
{
    partial class ProgressView
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
        /// 逐元素照抄 Calicat 设计稿第 3 版「正在安装3」
        /// （图层 c92453cd-bda1-4c26-951c-5e8b59bccf1a，688×720），不做二次推导。
        ///
        /// 【坐标换算】
        ///   设计稿内容区 = (24,192) 640×424，内边距 32 → 正文坐标系原点 = 窗口 (56,192)。
        ///   本视图挂在窗口 (56,192)，尺寸 576×432（窗口高 720 − 288）。
        ///   所以：控件 Location = 设计稿绝对坐标 − (56,192)。
        ///
        /// 【字号换算】pt = 设计稿 px × 0.75（96 DPI）。
        ///
        ///   元素            设计稿绝对        视图坐标         尺寸      规格
        ///   ------------------------------------------------------------------------
        ///   页面标题        (56,224)        (0,32)         576×26   18px=13.5pt Bold #1F2937
        ///   副标题          (56,262)        (0,70)         576×22   13px=9.75pt #4B5563
        ///   阶段图标        (57,311)        (1,119)        14×14    #1677FF 圆弧
        ///   阶段文字        (80,307)        (24,115)       300×22   13px=9.75pt Bold #1677FF
        ///   百分比          (526,307)       (470,115)      106×22   13px=9.75pt Bold #1677FF 右对齐
        ///   进度条          (56,338)        (0,146)        576×12   圆角 6，轨 #F0F2F5，填 #1677FF
        ///   统计卡          (56,374)        (0,182)        576×102  圆角 12，底 #F5F7FA
        ///     文件图标      (72,390)        卡内 (16,16)   16×16    #9CA3AF
        ///     当前文件      (96,390)        卡内 (40,16)   520×18   12px=9pt #4B5563
        ///     标签行        (72,420)        卡内 (16,47)   高 17    12px=9pt #9CA3AF
        ///     数值行        (72,440)        卡内 (16,66)   高 20    13px=9.75pt #1F2937
        ///   步骤图标 ×4     (56,499+26i)    (0,307+26i)    14×14
        ///   步骤文字 ×4     (80,495+26i)    (24,303+26i)   552×22   12px=9pt
        ///
        /// 统计卡里 4 列实测 x（卡内坐标）= 16 / 168 / 372 / 512 —— 设计稿不是等分栅格，
        /// 所以这里按实测值绝对定位，列宽 152 / 204 / 140 / 56。
        ///
        /// 页面尺寸固定，所以用绝对定位而不是嵌套布局容器 —— 一是能和设计稿一一对应，
        /// 二是嵌套容器的不透明背景会盖掉父面板的圆角。
        /// 注意：WinForms 的 Padding 不参与绝对定位（只影响 Dock/Anchor 子控件），
        /// 所以面板内子控件坐标一律写成「设计稿绝对坐标 − 面板绝对坐标」，不依赖 Padding。
        /// </remarks>
        private void InitializeComponent()
        {
            this.lblTitle = new AntdUI.Label();
            this.lblSubtitle = new AntdUI.Label();
            this.iconPhase = new AntdUI.Label();
            this.lblPhase = new AntdUI.Label();
            this.lblPercent = new AntdUI.Label();
            this.progressBar = new AntdUI.Progress();
            this.statsCard = new AntdUI.Panel();
            this.iconFile = new AntdUI.Label();
            this.lblCurrentFile = new AntdUI.Label();
            this.cap1 = new AntdUI.Label();
            this.cap2 = new AntdUI.Label();
            this.cap3 = new AntdUI.Label();
            this.cap4 = new AntdUI.Label();
            this.val1 = new AntdUI.Label();
            this.val2 = new AntdUI.Label();
            this.val3 = new AntdUI.Label();
            this.val4 = new AntdUI.Label();
            this.stepIcon1 = new AntdUI.Label();
            this.stepIcon2 = new AntdUI.Label();
            this.stepIcon3 = new AntdUI.Label();
            this.stepIcon4 = new AntdUI.Label();
            this.step1 = new AntdUI.Label();
            this.step2 = new AntdUI.Label();
            this.step3 = new AntdUI.Label();
            this.step4 = new AntdUI.Label();
            this.statsCard.SuspendLayout();
            this.SuspendLayout();
            //
            // lblTitle —— 设计稿 (56,224) 576×26，18px = 13.5pt Bold #1F2937
            //
            this.lblTitle.AutoSize = false;
            this.lblTitle.Font = new System.Drawing.Font("微软雅黑", 13.5F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(41)))), ((int)(((byte)(55)))));
            this.lblTitle.Location = new System.Drawing.Point(0, 32);
            this.lblTitle.Margin = new System.Windows.Forms.Padding(0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(576, 26);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "正在安装「空中小火车」";
            //
            // lblSubtitle —— 设计稿 (56,262) 576×22，13px = 9.75pt #4B5563
            //
            this.lblSubtitle.AutoSize = false;
            this.lblSubtitle.Font = new System.Drawing.Font("微软雅黑", 9.75F);
            this.lblSubtitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(85)))), ((int)(((byte)(99)))));
            this.lblSubtitle.Location = new System.Drawing.Point(0, 70);
            this.lblSubtitle.Margin = new System.Windows.Forms.Padding(0);
            this.lblSubtitle.Name = "lblSubtitle";
            this.lblSubtitle.Size = new System.Drawing.Size(576, 22);
            this.lblSubtitle.TabIndex = 1;
            this.lblSubtitle.Text = "安装程序正在解压并写入文件，请不要关闭窗口或关机。";
            //
            // iconPhase —— 设计稿 (57,311) 14×14，圆弧进度图标 #1677FF
            //
            this.iconPhase.AutoSize = false;
            this.iconPhase.Font = new System.Drawing.Font("微软雅黑", 10.5F);
            this.iconPhase.IconGap = 0;
            this.iconPhase.Location = new System.Drawing.Point(1, 119);
            this.iconPhase.Margin = new System.Windows.Forms.Padding(0);
            this.iconPhase.Name = "iconPhase";
            this.iconPhase.PrefixSvg = "<svg viewBox=\"0 0 24 24\"><path d=\"M12 3a9 9 0 1 0 9 9\" fill=\"none\" stroke=\"#1677FF\" stroke-width=\"2.2\" stroke-linecap=\"round\"/></svg>";
            this.iconPhase.Size = new System.Drawing.Size(14, 14);
            this.iconPhase.TabIndex = 2;
            this.iconPhase.Text = " ";
            //
            // lblPhase —— 设计稿 (80,307) 300×22，14px = 10.5pt Bold #1677FF
            //
            this.lblPhase.AutoSize = false;
            this.lblPhase.Font = new System.Drawing.Font("微软雅黑", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblPhase.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(119)))), ((int)(((byte)(255)))));
            this.lblPhase.Location = new System.Drawing.Point(24, 115);
            this.lblPhase.Margin = new System.Windows.Forms.Padding(0);
            this.lblPhase.Name = "lblPhase";
            this.lblPhase.Size = new System.Drawing.Size(300, 22);
            this.lblPhase.TabIndex = 3;
            this.lblPhase.Text = "正在解压文件";
            //
            // lblPercent —— 设计稿右对齐到 x=630，14px = 10.5pt Bold #1677FF
            //
            this.lblPercent.AutoSize = false;
            this.lblPercent.Font = new System.Drawing.Font("微软雅黑", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblPercent.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(119)))), ((int)(((byte)(255)))));
            this.lblPercent.Location = new System.Drawing.Point(470, 115);
            this.lblPercent.Margin = new System.Windows.Forms.Padding(0);
            this.lblPercent.Name = "lblPercent";
            this.lblPercent.Size = new System.Drawing.Size(106, 22);
            this.lblPercent.TabIndex = 4;
            this.lblPercent.Text = "62%";
            this.lblPercent.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // progressBar —— 设计稿 (56,338) 576×12，圆角 6，轨道 #F0F2F5，填充 #1677FF
            //
            // 三个坑（都是 AntdUI 2.4.3 Progress 的实现细节，见 Progress.PaintShapeRound）：
            //   1) useSystemText=false 时它会**自动生成**「62%」并在右侧留出文字宽度，
            //      把进度条本身截短。要彻底不画文字，必须 useSystemText=true 且 Text=null
            //      （空字符串不算 null，仍会走「有文字」分支）。
            //   2) 条高 = MeasureString(Font).Height × ValueRatio，默认 0.4 → 只有 7px。
            //      9.75pt 的 微软雅黑 MeasureString 高 18.78，0.66 × 18.78 = 12.4 → 12px。
            //   3) Value 的补间动画会让截图拍到中间帧，Animation=0 直接跳到目标值。
            //
            this.progressBar.Animation = 0;
            this.progressBar.Back = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(242)))), ((int)(((byte)(245)))));
            this.progressBar.Fill = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(119)))), ((int)(((byte)(255)))));
            this.progressBar.Location = new System.Drawing.Point(0, 146);
            this.progressBar.Margin = new System.Windows.Forms.Padding(0);
            this.progressBar.Name = "progressBar";
            this.progressBar.Radius = 6;
            this.progressBar.Shape = AntdUI.TShapeProgress.Default;
            this.progressBar.Size = new System.Drawing.Size(576, 12);
            this.progressBar.TabIndex = 5;
            this.progressBar.Text = null;
            this.progressBar.UseSystemText = true;
            this.progressBar.Value = 0.62F;
            this.progressBar.ValueRatio = 0.66F;
            //
            // statsCard —— 设计稿 (56,374) 576×102，圆角 12，底 #F5F7FA
            //
            this.statsCard.Controls.Add(this.val4);
            this.statsCard.Controls.Add(this.val3);
            this.statsCard.Controls.Add(this.val2);
            this.statsCard.Controls.Add(this.val1);
            this.statsCard.Controls.Add(this.cap4);
            this.statsCard.Controls.Add(this.cap3);
            this.statsCard.Controls.Add(this.cap2);
            this.statsCard.Controls.Add(this.cap1);
            this.statsCard.Controls.Add(this.lblCurrentFile);
            this.statsCard.Controls.Add(this.iconFile);
            this.statsCard.Location = new System.Drawing.Point(0, 182);
            this.statsCard.Margin = new System.Windows.Forms.Padding(0);
            this.statsCard.Name = "statsCard";
            this.statsCard.Radius = 12;
            this.statsCard.Size = new System.Drawing.Size(576, 102);
            this.statsCard.TabIndex = 6;
            //
            // iconFile —— 设计稿 (72,390)，卡内 (16,16)，16×16 灰色文档图标
            //
            this.iconFile.AutoSize = false;
            this.iconFile.Font = new System.Drawing.Font("微软雅黑", 12F);
            this.iconFile.IconGap = 0;
            this.iconFile.Location = new System.Drawing.Point(16, 16);
            this.iconFile.Margin = new System.Windows.Forms.Padding(0);
            this.iconFile.Name = "iconFile";
            this.iconFile.PrefixSvg = "<svg viewBox=\"0 0 24 24\"><rect x=\"4\" y=\"3\" width=\"16\" height=\"18\" rx=\"2.4\" fill=\"none\" stroke=\"#9CA3AF\" stroke-width=\"1.9\"/><path d=\"M8 9h8M8 13h8M8 17h5\" stroke=\"#9CA3AF\" stroke-width=\"1.9\" stroke-linecap=\"round\"/></svg>";
            this.iconFile.Size = new System.Drawing.Size(16, 16);
            this.iconFile.TabIndex = 0;
            this.iconFile.Text = " ";
            //
            // lblCurrentFile —— 设计稿 (96,390)，卡内 (40,16)，12px = 9pt #4B5563
            //
            this.lblCurrentFile.AutoSize = false;
            this.lblCurrentFile.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.lblCurrentFile.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(85)))), ((int)(((byte)(99)))));
            this.lblCurrentFile.Location = new System.Drawing.Point(40, 16);
            this.lblCurrentFile.Margin = new System.Windows.Forms.Padding(0);
            this.lblCurrentFile.Name = "lblCurrentFile";
            this.lblCurrentFile.Size = new System.Drawing.Size(520, 18);
            this.lblCurrentFile.TabIndex = 1;
            this.lblCurrentFile.Text = "正在写入：bin\\CqVideo\\ffmpeg.dll";
            //
            // cap1..cap4 —— 设计稿 y=420，卡内 y=46，12px = 9pt #9CA3AF
            // 卡内 x = 16 / 168 / 372 / 512
            //
            this.cap1.AutoSize = false;
            this.cap1.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.cap1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(163)))), ((int)(((byte)(175)))));
            this.cap1.Location = new System.Drawing.Point(16, 47);
            this.cap1.Margin = new System.Windows.Forms.Padding(0);
            this.cap1.Name = "cap1";
            this.cap1.Size = new System.Drawing.Size(152, 17);
            this.cap1.TabIndex = 2;
            this.cap1.Text = "已解压文件";
            this.cap2.AutoSize = false;
            this.cap2.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.cap2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(163)))), ((int)(((byte)(175)))));
            this.cap2.Location = new System.Drawing.Point(168, 47);
            this.cap2.Margin = new System.Windows.Forms.Padding(0);
            this.cap2.Name = "cap2";
            this.cap2.Size = new System.Drawing.Size(204, 17);
            this.cap2.TabIndex = 3;
            this.cap2.Text = "已写入大小";
            this.cap3.AutoSize = false;
            this.cap3.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.cap3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(163)))), ((int)(((byte)(175)))));
            this.cap3.Location = new System.Drawing.Point(372, 47);
            this.cap3.Margin = new System.Windows.Forms.Padding(0);
            this.cap3.Name = "cap3";
            this.cap3.Size = new System.Drawing.Size(140, 17);
            this.cap3.TabIndex = 4;
            this.cap3.Text = "已用时间";
            this.cap4.AutoSize = false;
            this.cap4.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.cap4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(163)))), ((int)(((byte)(175)))));
            this.cap4.Location = new System.Drawing.Point(512, 47);
            this.cap4.Margin = new System.Windows.Forms.Padding(0);
            this.cap4.Name = "cap4";
            this.cap4.Size = new System.Drawing.Size(56, 17);
            this.cap4.TabIndex = 5;
            this.cap4.Text = "预计剩余";
            //
            // val1..val4 —— 设计稿 y=440，卡内 y=66，13px = 9.75pt #1F2937
            //
            this.val1.AutoSize = false;
            this.val1.Font = new System.Drawing.Font("微软雅黑", 9.75F);
            this.val1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(41)))), ((int)(((byte)(55)))));
            this.val1.Location = new System.Drawing.Point(16, 66);
            this.val1.Margin = new System.Windows.Forms.Padding(0);
            this.val1.Name = "val1";
            this.val1.Size = new System.Drawing.Size(152, 20);
            this.val1.TabIndex = 6;
            this.val1.Text = "123 / 199";
            this.val2.AutoSize = false;
            this.val2.Font = new System.Drawing.Font("微软雅黑", 9.75F);
            this.val2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(41)))), ((int)(((byte)(55)))));
            this.val2.Location = new System.Drawing.Point(168, 66);
            this.val2.Margin = new System.Windows.Forms.Padding(0);
            this.val2.Name = "val2";
            this.val2.Size = new System.Drawing.Size(204, 20);
            this.val2.TabIndex = 7;
            this.val2.Text = "56.4 MB / 90.6 MB";
            this.val3.AutoSize = false;
            this.val3.Font = new System.Drawing.Font("微软雅黑", 9.75F);
            this.val3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(41)))), ((int)(((byte)(55)))));
            this.val3.Location = new System.Drawing.Point(372, 66);
            this.val3.Margin = new System.Windows.Forms.Padding(0);
            this.val3.Name = "val3";
            this.val3.Size = new System.Drawing.Size(140, 20);
            this.val3.TabIndex = 8;
            this.val3.Text = "00:12";
            this.val4.AutoSize = false;
            this.val4.Font = new System.Drawing.Font("微软雅黑", 9.75F);
            this.val4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(41)))), ((int)(((byte)(55)))));
            this.val4.Location = new System.Drawing.Point(512, 66);
            this.val4.Margin = new System.Windows.Forms.Padding(0);
            this.val4.Name = "val4";
            this.val4.Size = new System.Drawing.Size(56, 20);
            this.val4.TabIndex = 9;
            this.val4.Text = "约 8 秒";
            //
            // stepIcon1..4 —— 设计稿 (56,499+26i)，视图 (0,307+26i)，14×14
            //
            this.stepIcon1.AutoSize = false;
            this.stepIcon1.Font = new System.Drawing.Font("微软雅黑", 10.5F);
            this.stepIcon1.IconGap = 0;
            this.stepIcon1.Location = new System.Drawing.Point(0, 307);
            this.stepIcon1.Margin = new System.Windows.Forms.Padding(0);
            this.stepIcon1.Name = "stepIcon1";
            this.stepIcon1.PrefixSvg = "<svg viewBox=\"0 0 16 16\"><circle cx=\"8\" cy=\"8\" r=\"7\" fill=\"#52C41A\"/><path d=\"M4.9 8.2l2.1 2.1 4.2-4.3\" fill=\"none\" stroke=\"#FFFFFF\" stroke-width=\"1.6\" stroke-linecap=\"round\" stroke-linejoin=\"round\"/></svg>";
            this.stepIcon1.Size = new System.Drawing.Size(14, 14);
            this.stepIcon1.TabIndex = 7;
            this.stepIcon1.Text = " ";
            this.stepIcon2.AutoSize = false;
            this.stepIcon2.Font = new System.Drawing.Font("微软雅黑", 10.5F);
            this.stepIcon2.IconGap = 0;
            this.stepIcon2.Location = new System.Drawing.Point(0, 333);
            this.stepIcon2.Margin = new System.Windows.Forms.Padding(0);
            this.stepIcon2.Name = "stepIcon2";
            this.stepIcon2.PrefixSvg = "<svg viewBox=\"0 0 24 24\"><path d=\"M12 3a9 9 0 1 0 9 9\" fill=\"none\" stroke=\"#1677FF\" stroke-width=\"2.2\" stroke-linecap=\"round\"/></svg>";
            this.stepIcon2.Size = new System.Drawing.Size(14, 14);
            this.stepIcon2.TabIndex = 8;
            this.stepIcon2.Text = " ";
            this.stepIcon3.AutoSize = false;
            this.stepIcon3.Font = new System.Drawing.Font("微软雅黑", 10.5F);
            this.stepIcon3.IconGap = 0;
            this.stepIcon3.Location = new System.Drawing.Point(0, 359);
            this.stepIcon3.Margin = new System.Windows.Forms.Padding(0);
            this.stepIcon3.Name = "stepIcon3";
            this.stepIcon3.PrefixSvg = "<svg viewBox=\"0 0 16 16\"><circle cx=\"8\" cy=\"8\" r=\"6.6\" fill=\"none\" stroke=\"#D0D5DD\" stroke-width=\"1.5\"/></svg>";
            this.stepIcon3.Size = new System.Drawing.Size(14, 14);
            this.stepIcon3.TabIndex = 9;
            this.stepIcon3.Text = " ";
            this.stepIcon4.AutoSize = false;
            this.stepIcon4.Font = new System.Drawing.Font("微软雅黑", 10.5F);
            this.stepIcon4.IconGap = 0;
            this.stepIcon4.Location = new System.Drawing.Point(0, 385);
            this.stepIcon4.Margin = new System.Windows.Forms.Padding(0);
            this.stepIcon4.Name = "stepIcon4";
            this.stepIcon4.PrefixSvg = "<svg viewBox=\"0 0 16 16\"><circle cx=\"8\" cy=\"8\" r=\"6.6\" fill=\"none\" stroke=\"#D0D5DD\" stroke-width=\"1.5\"/></svg>";
            this.stepIcon4.Size = new System.Drawing.Size(14, 14);
            this.stepIcon4.TabIndex = 10;
            this.stepIcon4.Text = " ";
            //
            // step1..step4 —— 设计稿 (80,495+26i)，视图 (24,303+26i)，552×22，12px = 9pt
            //
            this.step1.AutoSize = false;
            this.step1.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.step1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(82)))), ((int)(((byte)(196)))), ((int)(((byte)(26)))));
            this.step1.Location = new System.Drawing.Point(24, 303);
            this.step1.Margin = new System.Windows.Forms.Padding(0);
            this.step1.Name = "step1";
            this.step1.Size = new System.Drawing.Size(552, 22);
            this.step1.TabIndex = 11;
            this.step1.Text = "准备安装目录 —— 已完成";
            this.step2.AutoSize = false;
            this.step2.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.step2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(119)))), ((int)(((byte)(255)))));
            this.step2.Location = new System.Drawing.Point(24, 329);
            this.step2.Margin = new System.Windows.Forms.Padding(0);
            this.step2.Name = "step2";
            this.step2.Size = new System.Drawing.Size(552, 22);
            this.step2.TabIndex = 12;
            this.step2.Text = "解压程序文件 —— 进行中 62%";
            this.step3.AutoSize = false;
            this.step3.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.step3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(163)))), ((int)(((byte)(175)))));
            this.step3.Location = new System.Drawing.Point(24, 355);
            this.step3.Margin = new System.Windows.Forms.Padding(0);
            this.step3.Name = "step3";
            this.step3.Size = new System.Drawing.Size(552, 22);
            this.step3.TabIndex = 13;
            this.step3.Text = "创建桌面与开始菜单快捷方式 —— 等待中";
            this.step4.AutoSize = false;
            this.step4.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.step4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(163)))), ((int)(((byte)(175)))));
            this.step4.Location = new System.Drawing.Point(24, 381);
            this.step4.Margin = new System.Windows.Forms.Padding(0);
            this.step4.Name = "step4";
            this.step4.Size = new System.Drawing.Size(552, 22);
            this.step4.TabIndex = 14;
            this.step4.Text = "写入注册表与卸载信息 —— 等待中";
            //
            // ProgressView
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.step4);
            this.Controls.Add(this.step3);
            this.Controls.Add(this.step2);
            this.Controls.Add(this.step1);
            this.Controls.Add(this.stepIcon4);
            this.Controls.Add(this.stepIcon3);
            this.Controls.Add(this.stepIcon2);
            this.Controls.Add(this.stepIcon1);
            this.Controls.Add(this.statsCard);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.lblPercent);
            this.Controls.Add(this.lblPhase);
            this.Controls.Add(this.iconPhase);
            this.Controls.Add(this.lblSubtitle);
            this.Controls.Add(this.lblTitle);
            this.Font = new System.Drawing.Font("微软雅黑", 9.75F);
            this.Name = "ProgressView";
            this.Size = new System.Drawing.Size(576, 432);
            this.statsCard.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private AntdUI.Label lblTitle;
        private AntdUI.Label lblSubtitle;
        private AntdUI.Label iconPhase;
        private AntdUI.Label lblPhase;
        private AntdUI.Label lblPercent;
        private AntdUI.Progress progressBar;
        private AntdUI.Panel statsCard;
        private AntdUI.Label iconFile;
        private AntdUI.Label lblCurrentFile;
        private AntdUI.Label cap1;
        private AntdUI.Label cap2;
        private AntdUI.Label cap3;
        private AntdUI.Label cap4;
        private AntdUI.Label val1;
        private AntdUI.Label val2;
        private AntdUI.Label val3;
        private AntdUI.Label val4;
        private AntdUI.Label stepIcon1;
        private AntdUI.Label stepIcon2;
        private AntdUI.Label stepIcon3;
        private AntdUI.Label stepIcon4;
        private AntdUI.Label step1;
        private AntdUI.Label step2;
        private AntdUI.Label step3;
        private AntdUI.Label step4;
    }
}
