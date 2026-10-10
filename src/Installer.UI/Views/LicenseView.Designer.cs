namespace Installer.UI.Views
{
    partial class LicenseView
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
        /// 本页坐标逐元素照抄 Calicat 设计稿第 3 版（许可协议页），不做二次推导。
        /// 设计稿：file_id=2108389704686198784，layer_id=fbb4920b-df1f-4585-a84d-8833fa4dabb2（688×1012）。
        ///
        /// 坐标换算（重要，改任何 Location 前先看这里）：
        ///   设计稿内容区 = (24,192) 640×724，内边距 [28,32,28,32]
        ///   → 正文坐标系原点 = (24+32, 192) = 窗口 (56,192)，视图尺寸 576×724
        ///   → 控件 Location = 设计稿绝对坐标 − (56,192)
        ///   字号：pt = 设计稿 px × 0.75（96 DPI）；尺寸/圆角/内边距在 WinForms 里就是设计稿 px。
        ///
        /// 元素清单（设计稿绝对坐标 → 本视图坐标）：
        ///   页面标题   (56,220) → (0, 28)   576×26   18px=13.5pt SemiBold #1F2937
        ///   副标题     (56,256) → (0, 64)   576×22   13px=9.75pt Regular  #4B5563
        ///   协议正文框 (56,294) → (0,102)  576×506  圆角12 底#FAFBFC 描边1px #E5E8EF 内边距20
        ///     协议抬头 icon (76,316) → 框内 (20,22) 17×16 #1677FF
        ///     协议抬头 文案 (101,314) → 框内 (25,20) 536-25 宽 13px=9.75pt SemiBold #1F2937
        ///     条款 1..6：框内 x=20 宽 536，纵向间距 74
        ///       条款标题 框内 y = 54+74n   536×18  12px=9pt SemiBold #1F2937
        ///       条款正文 框内 y = 76+74n   536×40  12px=9pt Regular  #4B5563
        ///   同意条款行 (56,820) → (0,628)  576×68   圆角12 底#F5F7FA 内边距[14,16,14,16] 间距12
        ///       勾选框   (72,845) → 行内 (16,25) 18×18 圆角4 #1677FF
        ///       同意文案 (102,834)→ 行内 (46,14) 13px=9.75pt Regular #1F2937
        ///       灰色提示 (102,854)→ 行内 (46,36) 12px=9pt      Regular #9CA3AF
        ///
        /// 页面尺寸固定（宿主 pnlContent 按页变高），所以这里用绝对定位而不是嵌套布局容器 ——
        /// 一是能和设计稿一一对应，二是嵌套容器的不透明背景会盖掉父面板的圆角。
        /// </remarks>
        private void InitializeComponent()
        {
            this.lblTitle = new AntdUI.Label();
            this.lblSubtitle = new AntdUI.Label();
            this.licenseCard = new AntdUI.Panel();
            this.lblCardIcon = new AntdUI.Label();
            this.lblCardTitle = new AntdUI.Label();
            this.licenseScroll = new System.Windows.Forms.Panel();
            this.clauseHost = new System.Windows.Forms.Panel();
            this.clause1Title = new AntdUI.Label();
            this.clause1Body = new AntdUI.Label();
            this.clause2Title = new AntdUI.Label();
            this.clause2Body = new AntdUI.Label();
            this.clause3Title = new AntdUI.Label();
            this.clause3Body = new AntdUI.Label();
            this.clause4Title = new AntdUI.Label();
            this.clause4Body = new AntdUI.Label();
            this.clause5Title = new AntdUI.Label();
            this.clause5Body = new AntdUI.Label();
            this.clause6Title = new AntdUI.Label();
            this.clause6Body = new AntdUI.Label();
            this.acceptRow = new AntdUI.Panel();
            this.chkAccept = new AntdUI.Checkbox();
            this.lblAccept = new AntdUI.Label();
            this.lblAcceptHint = new AntdUI.Label();
            this.licenseCard.SuspendLayout();
            this.licenseScroll.SuspendLayout();
            this.clauseHost.SuspendLayout();
            this.acceptRow.SuspendLayout();
            this.SuspendLayout();
            //
            // lblTitle —— 设计稿 (56,220) 576×26，18px = 13.5pt SemiBold
            //
            this.lblTitle.AutoSize = false;
            this.lblTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblTitle.Font = new System.Drawing.Font("微软雅黑", 13.5F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(0, 28);
            this.lblTitle.Margin = new System.Windows.Forms.Padding(0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(576, 26);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "许可协议";
            //
            // lblSubtitle —— 设计稿 (56,256) 576×22，13px = 9.75pt
            //
            this.lblSubtitle.AutoSize = false;
            this.lblSubtitle.BackColor = System.Drawing.Color.Transparent;
            this.lblSubtitle.Font = new System.Drawing.Font("微软雅黑", 9.75F);
            this.lblSubtitle.Location = new System.Drawing.Point(0, 64);
            this.lblSubtitle.Margin = new System.Windows.Forms.Padding(0);
            this.lblSubtitle.Name = "lblSubtitle";
            this.lblSubtitle.Size = new System.Drawing.Size(576, 22);
            this.lblSubtitle.TabIndex = 1;
            this.lblSubtitle.Text = "请仔细阅读下面的许可协议。安装「空中小火车」前，你需要接受本协议的全部条款。";
            //
            // licenseCard —— 设计稿 (56,294) 576×506，圆角 12，底 #FAFBFC，描边 1px #E5E8EF，内边距 20
            //
            // 只设 AntdUI 的 Back，不设 WinForms 的 BackColor（设了会用直角填充盖掉圆角）。
            //
            this.licenseCard.Back = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(251)))), ((int)(((byte)(252)))));
            this.licenseCard.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(232)))), ((int)(((byte)(239)))));
            this.licenseCard.BorderWidth = 1F;
            this.licenseCard.Controls.Add(this.licenseScroll);
            this.licenseCard.Controls.Add(this.lblCardTitle);
            this.licenseCard.Controls.Add(this.lblCardIcon);
            this.licenseCard.Location = new System.Drawing.Point(0, 102);
            this.licenseCard.Margin = new System.Windows.Forms.Padding(0);
            this.licenseCard.Name = "licenseCard";
            this.licenseCard.Padding = new System.Windows.Forms.Padding(20);
            this.licenseCard.Radius = 12;
            this.licenseCard.Size = new System.Drawing.Size(576, 506);
            this.licenseCard.TabIndex = 2;
            //
            // lblCardIcon —— 设计稿 (76,316)，即框内 (20,22)，17×16，文档图标 #1677FF
            //
            this.lblCardIcon.AutoSize = false;
            this.lblCardIcon.BackColor = System.Drawing.Color.Transparent;
            this.lblCardIcon.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.lblCardIcon.IconGap = 0;
            this.lblCardIcon.Location = new System.Drawing.Point(20, 22);
            this.lblCardIcon.Margin = new System.Windows.Forms.Padding(0);
            this.lblCardIcon.Name = "lblCardIcon";
            this.lblCardIcon.PrefixSvg = "<svg viewBox=\"0 0 24 24\"><path d=\"M6.2 2.6h7.4l4.2 4.2v14.6H6.2z\" fill=\"none\" stroke=\"#1677FF\" stroke-width=\"1.8\" stroke-linejoin=\"round\"/><path d=\"M13.4 2.9v4.1h4.1\" fill=\"none\" stroke=\"#1677FF\" stroke-width=\"1.8\" stroke-linejoin=\"round\"/><path d=\"M9.2 12.8h5.6M9.2 16.4h5.6\" stroke=\"#1677FF\" stroke-width=\"1.6\" stroke-linecap=\"round\"/></svg>";
            this.lblCardIcon.Size = new System.Drawing.Size(17, 16);
            this.lblCardIcon.TabIndex = 0;
            this.lblCardIcon.Text = " ";
            //
            // lblCardTitle —— 设计稿 (101,314)，即框内 (25,20)，13px = 9.75pt SemiBold
            //
            // 设计稿宽度是 fit_content（实测 173），这里给足框内可用宽度 536-25，避免真实产品名被截断。
            //
            this.lblCardTitle.AutoSize = false;
            this.lblCardTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblCardTitle.Font = new System.Drawing.Font("微软雅黑", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblCardTitle.Location = new System.Drawing.Point(25, 20);
            this.lblCardTitle.Margin = new System.Windows.Forms.Padding(0);
            this.lblCardTitle.Name = "lblCardTitle";
            this.lblCardTitle.Size = new System.Drawing.Size(511, 20);
            this.lblCardTitle.TabIndex = 1;
            this.lblCardTitle.Text = "空中小火车 最终用户许可协议";
            //
            // licenseScroll —— 协议正文滚动宿主，框内 (20,40) 536×446（内边距 20 + 抬头 20 之后到底部内边距）
            //
            // 设计稿的正文框是固定高（506）且不带滚动条；这里加一层 AutoScroll 只作为安全网：
            // 文案没超出时不会出现滚动条，像素与设计稿一致；超出时用户仍能读完条款。
            //
            this.licenseScroll.AutoScroll = true;
            this.licenseScroll.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(251)))), ((int)(((byte)(252)))));
            this.licenseScroll.Controls.Add(this.clauseHost);
            this.licenseScroll.Location = new System.Drawing.Point(20, 40);
            this.licenseScroll.Margin = new System.Windows.Forms.Padding(0);
            this.licenseScroll.Name = "licenseScroll";
            this.licenseScroll.Size = new System.Drawing.Size(536, 446);
            this.licenseScroll.TabIndex = 2;
            //
            // clauseHost —— 条款宿主，宽 519 = 536 − 17（竖向滚动条宽度），保证滚动条出现时也不会出横向滚动条
            //
            // 设计稿「协议抬头 → 第一条条款」之间还有 14 的间距（抬头容器 padding-top:14），
            // 所以宿主整体下移 14：条款 1 标题落在框内 y = 40 + 14 = 54，与设计稿一致。
            //
            this.clauseHost.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(251)))), ((int)(((byte)(252)))));
            this.clauseHost.Controls.Add(this.clause6Body);
            this.clauseHost.Controls.Add(this.clause6Title);
            this.clauseHost.Controls.Add(this.clause5Body);
            this.clauseHost.Controls.Add(this.clause5Title);
            this.clauseHost.Controls.Add(this.clause4Body);
            this.clauseHost.Controls.Add(this.clause4Title);
            this.clauseHost.Controls.Add(this.clause3Body);
            this.clauseHost.Controls.Add(this.clause3Title);
            this.clauseHost.Controls.Add(this.clause2Body);
            this.clauseHost.Controls.Add(this.clause2Title);
            this.clauseHost.Controls.Add(this.clause1Body);
            this.clauseHost.Controls.Add(this.clause1Title);
            this.clauseHost.Location = new System.Drawing.Point(0, 14);
            this.clauseHost.Margin = new System.Windows.Forms.Padding(0);
            this.clauseHost.Name = "clauseHost";
            this.clauseHost.Size = new System.Drawing.Size(519, 432);
            this.clauseHost.TabIndex = 0;
            //
            // 条款 1..6 —— 设计稿框内 x=20 宽 536，纵向间距 74；标题 536×18，正文 536×40
            //   标题 12px = 9pt SemiBold #1F2937；正文 12px = 9pt Regular #4B5563
            //
            this.clause1Title.AutoSize = false;
            this.clause1Title.BackColor = System.Drawing.Color.Transparent;
            this.clause1Title.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold);
            this.clause1Title.Location = new System.Drawing.Point(0, 0);
            this.clause1Title.Margin = new System.Windows.Forms.Padding(0);
            this.clause1Title.Name = "clause1Title";
            this.clause1Title.Size = new System.Drawing.Size(519, 18);
            this.clause1Title.TabIndex = 0;
            this.clause1Title.Text = "1. 授权范围";
            this.clause1Body.AutoSize = false;
            this.clause1Body.BackColor = System.Drawing.Color.Transparent;
            this.clause1Body.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.clause1Body.Location = new System.Drawing.Point(0, 22);
            this.clause1Body.Margin = new System.Windows.Forms.Padding(0);
            this.clause1Body.Name = "clause1Body";
            this.clause1Body.Size = new System.Drawing.Size(519, 40);
            this.clause1Body.TabIndex = 1;
            this.clause1Body.Text = "本协议授予你在一台设备上安装并使用「空中小火车」的非独占、不可转让的权利。你可以为备份目的复制一份安装包，但不得对外分发。";
            this.clause1Body.TextMultiLine = true;
            this.clause2Title.AutoSize = false;
            this.clause2Title.BackColor = System.Drawing.Color.Transparent;
            this.clause2Title.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold);
            this.clause2Title.Location = new System.Drawing.Point(0, 74);
            this.clause2Title.Margin = new System.Windows.Forms.Padding(0);
            this.clause2Title.Name = "clause2Title";
            this.clause2Title.Size = new System.Drawing.Size(519, 18);
            this.clause2Title.TabIndex = 2;
            this.clause2Title.Text = "2. 使用限制";
            this.clause2Body.AutoSize = false;
            this.clause2Body.BackColor = System.Drawing.Color.Transparent;
            this.clause2Body.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.clause2Body.Location = new System.Drawing.Point(0, 96);
            this.clause2Body.Margin = new System.Windows.Forms.Padding(0);
            this.clause2Body.Name = "clause2Body";
            this.clause2Body.Size = new System.Drawing.Size(519, 40);
            this.clause2Body.TabIndex = 3;
            this.clause2Body.Text = "你不得对本软件进行反向工程、反编译或反汇编，不得移除或修改软件中的版权声明与标识，不得将本软件用于任何违法用途。";
            this.clause2Body.TextMultiLine = true;
            this.clause3Title.AutoSize = false;
            this.clause3Title.BackColor = System.Drawing.Color.Transparent;
            this.clause3Title.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold);
            this.clause3Title.Location = new System.Drawing.Point(0, 148);
            this.clause3Title.Margin = new System.Windows.Forms.Padding(0);
            this.clause3Title.Name = "clause3Title";
            this.clause3Title.Size = new System.Drawing.Size(519, 18);
            this.clause3Title.TabIndex = 4;
            this.clause3Title.Text = "3. 数据与隐私";
            this.clause3Body.AutoSize = false;
            this.clause3Body.BackColor = System.Drawing.Color.Transparent;
            this.clause3Body.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.clause3Body.Location = new System.Drawing.Point(0, 170);
            this.clause3Body.Margin = new System.Windows.Forms.Padding(0);
            this.clause3Body.Name = "clause3Body";
            this.clause3Body.Size = new System.Drawing.Size(519, 40);
            this.clause3Body.TabIndex = 5;
            this.clause3Body.Text = "本软件会在本地保存你的配置信息与操作记录，不会在未经许可的情况下上传到服务器。相关数据仅用于改善软件功能。";
            this.clause3Body.TextMultiLine = true;
            this.clause4Title.AutoSize = false;
            this.clause4Title.BackColor = System.Drawing.Color.Transparent;
            this.clause4Title.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold);
            this.clause4Title.Location = new System.Drawing.Point(0, 222);
            this.clause4Title.Margin = new System.Windows.Forms.Padding(0);
            this.clause4Title.Name = "clause4Title";
            this.clause4Title.Size = new System.Drawing.Size(519, 18);
            this.clause4Title.TabIndex = 6;
            this.clause4Title.Text = "4. 免责声明";
            this.clause4Body.AutoSize = false;
            this.clause4Body.BackColor = System.Drawing.Color.Transparent;
            this.clause4Body.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.clause4Body.Location = new System.Drawing.Point(0, 244);
            this.clause4Body.Margin = new System.Windows.Forms.Padding(0);
            this.clause4Body.Name = "clause4Body";
            this.clause4Body.Size = new System.Drawing.Size(519, 40);
            this.clause4Body.TabIndex = 7;
            this.clause4Body.Text = "本软件按「现状」提供，不对其适用性、稳定性作出明示或默示的担保。因使用本软件产生的数据丢失或其他损失，开发者不承担相应责任。";
            this.clause4Body.TextMultiLine = true;
            this.clause5Title.AutoSize = false;
            this.clause5Title.BackColor = System.Drawing.Color.Transparent;
            this.clause5Title.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold);
            this.clause5Title.Location = new System.Drawing.Point(0, 296);
            this.clause5Title.Margin = new System.Windows.Forms.Padding(0);
            this.clause5Title.Name = "clause5Title";
            this.clause5Title.Size = new System.Drawing.Size(519, 18);
            this.clause5Title.TabIndex = 8;
            this.clause5Title.Text = "5. 协议终止";
            this.clause5Body.AutoSize = false;
            this.clause5Body.BackColor = System.Drawing.Color.Transparent;
            this.clause5Body.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.clause5Body.Location = new System.Drawing.Point(0, 318);
            this.clause5Body.Margin = new System.Windows.Forms.Padding(0);
            this.clause5Body.Name = "clause5Body";
            this.clause5Body.Size = new System.Drawing.Size(519, 40);
            this.clause5Body.TabIndex = 9;
            this.clause5Body.Text = "如果你违反本协议的任何条款，本协议授予的使用权将自动终止。终止后你应停止使用并卸载本软件。";
            this.clause5Body.TextMultiLine = true;
            this.clause6Title.AutoSize = false;
            this.clause6Title.BackColor = System.Drawing.Color.Transparent;
            this.clause6Title.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold);
            this.clause6Title.Location = new System.Drawing.Point(0, 370);
            this.clause6Title.Margin = new System.Windows.Forms.Padding(0);
            this.clause6Title.Name = "clause6Title";
            this.clause6Title.Size = new System.Drawing.Size(519, 18);
            this.clause6Title.TabIndex = 10;
            this.clause6Title.Text = "6. 其他约定";
            this.clause6Body.AutoSize = false;
            this.clause6Body.BackColor = System.Drawing.Color.Transparent;
            this.clause6Body.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.clause6Body.Location = new System.Drawing.Point(0, 392);
            this.clause6Body.Margin = new System.Windows.Forms.Padding(0);
            this.clause6Body.Name = "clause6Body";
            this.clause6Body.Size = new System.Drawing.Size(519, 40);
            this.clause6Body.TabIndex = 11;
            this.clause6Body.Text = "安装并继续使用本软件，即表示你已阅读并同意上述全部条款。本协议内容如发生变更，将在新版本中进行说明。";
            this.clause6Body.TextMultiLine = true;
            //
            // acceptRow —— 设计稿 (56,820) 576×68，圆角 12，底 #F5F7FA，内边距 [14,16,14,16]，间距 12
            //
            // 只设 AntdUI 的 Back，不设 WinForms 的 BackColor（设了会用直角填充盖掉圆角）。
            //
            this.acceptRow.Back = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(247)))), ((int)(((byte)(250)))));
            this.acceptRow.Controls.Add(this.lblAcceptHint);
            this.acceptRow.Controls.Add(this.lblAccept);
            this.acceptRow.Controls.Add(this.chkAccept);
            this.acceptRow.Location = new System.Drawing.Point(0, 628);
            this.acceptRow.Margin = new System.Windows.Forms.Padding(0);
            this.acceptRow.Name = "acceptRow";
            this.acceptRow.Padding = new System.Windows.Forms.Padding(16, 14, 16, 14);
            this.acceptRow.Radius = 12;
            this.acceptRow.Size = new System.Drawing.Size(576, 68);
            this.acceptRow.TabIndex = 3;
            //
            // chkAccept —— 设计稿 (72,845)，即行内 (16,25)，18×18 圆角 4
            //
            // 勾选框与文案在设计稿里是两个独立元素（文案另起一列），所以这里不设 Text，
            // 文案由 lblAccept / lblAcceptHint 承担；点文案也能切换（见 LicenseView.cs）。
            // 字号取 11.25pt 而不是 9pt：AntdUI 的勾选框边长跟着字号走，9pt 只画出 14px，
            // 11.25pt 才是设计稿的 18px（控件本身仍然是 18×18，不参与排版）。
            //
            this.chkAccept.AutoSize = false;
            this.chkAccept.BackColor = System.Drawing.Color.Transparent;
            this.chkAccept.Font = new System.Drawing.Font("微软雅黑", 11.25F);
            this.chkAccept.Location = new System.Drawing.Point(16, 25);
            this.chkAccept.Margin = new System.Windows.Forms.Padding(0);
            this.chkAccept.Name = "chkAccept";
            this.chkAccept.Size = new System.Drawing.Size(18, 18);
            this.chkAccept.TabIndex = 0;
            this.chkAccept.Text = " ";
            //
            // lblAccept —— 设计稿 (102,834)，即行内 (46,14)，13px = 9.75pt
            //
            // 设计稿宽度是 fit_content（实测 324），这里给足行内可用宽度 576-16*2-46，避免真实产品名被截断。
            //
            this.lblAccept.AutoSize = false;
            this.lblAccept.BackColor = System.Drawing.Color.Transparent;
            this.lblAccept.Font = new System.Drawing.Font("微软雅黑", 9.75F);
            this.lblAccept.Location = new System.Drawing.Point(46, 14);
            this.lblAccept.Margin = new System.Windows.Forms.Padding(0);
            this.lblAccept.Name = "lblAccept";
            this.lblAccept.Size = new System.Drawing.Size(498, 20);
            this.lblAccept.TabIndex = 1;
            this.lblAccept.Text = "我已阅读并同意「空中小火车」许可协议";
            //
            // lblAcceptHint —— 设计稿 (102,854)，即行内 (46,36)，12px = 9pt #9CA3AF
            //
            this.lblAcceptHint.AutoSize = false;
            this.lblAcceptHint.BackColor = System.Drawing.Color.Transparent;
            this.lblAcceptHint.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.lblAcceptHint.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(163)))), ((int)(((byte)(175)))));
            this.lblAcceptHint.Location = new System.Drawing.Point(46, 36);
            this.lblAcceptHint.Margin = new System.Windows.Forms.Padding(0);
            this.lblAcceptHint.Name = "lblAcceptHint";
            this.lblAcceptHint.Size = new System.Drawing.Size(498, 18);
            this.lblAcceptHint.TabIndex = 2;
            this.lblAcceptHint.Text = "勾选后才能继续安装；若不同意，可点击「取消」退出安装。";
            //
            // LicenseView
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.acceptRow);
            this.Controls.Add(this.licenseCard);
            this.Controls.Add(this.lblSubtitle);
            this.Controls.Add(this.lblTitle);
            this.Font = new System.Drawing.Font("微软雅黑", 9.75F);
            this.Name = "LicenseView";
            this.Size = new System.Drawing.Size(576, 724);
            this.licenseCard.ResumeLayout(false);
            this.licenseScroll.ResumeLayout(false);
            this.clauseHost.ResumeLayout(false);
            this.acceptRow.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private AntdUI.Label lblTitle;
        private AntdUI.Label lblSubtitle;
        private AntdUI.Panel licenseCard;
        private AntdUI.Label lblCardIcon;
        private AntdUI.Label lblCardTitle;
        private System.Windows.Forms.Panel licenseScroll;
        private System.Windows.Forms.Panel clauseHost;
        private AntdUI.Label clause1Title;
        private AntdUI.Label clause1Body;
        private AntdUI.Label clause2Title;
        private AntdUI.Label clause2Body;
        private AntdUI.Label clause3Title;
        private AntdUI.Label clause3Body;
        private AntdUI.Label clause4Title;
        private AntdUI.Label clause4Body;
        private AntdUI.Label clause5Title;
        private AntdUI.Label clause5Body;
        private AntdUI.Label clause6Title;
        private AntdUI.Label clause6Body;
        private AntdUI.Panel acceptRow;
        private AntdUI.Checkbox chkAccept;
        private AntdUI.Label lblAccept;
        private AntdUI.Label lblAcceptHint;
    }
}
