namespace Installer.UI.Views
{
    partial class FinishView
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
        /// 逐元素照抄 Calicat 设计稿第 3 版「安装完成3」
        /// （图层 b8b6009c-1762-4ac8-b25b-8b4e1b6de7b2，688×584），不做二次推导。
        ///
        /// 【坐标换算】
        ///   设计稿内容区 = (24,192) 640×424，内边距 32 → 正文坐标系原点 = 窗口 (56,192)。
        ///   本视图挂在窗口 (56,192)，尺寸 576×296（窗口高 584 − 288）。
        ///   所以：控件 Location = 设计稿绝对坐标 − (56,192)。
        ///
        /// 【字号换算】pt = 设计稿 px × 0.75（96 DPI）。
        ///
        ///   元素            设计稿绝对        视图坐标         尺寸      规格
        ///   ------------------------------------------------------------------------
        ///   成功圆徽        (56,231)        (0,39)         80×80    圆角 40（=圆），底 #52C41A
        ///     白勾          圆徽内 (20,20)   圆徽内 (20,20)  40×40   白色对勾
        ///   页面标题        (160,224)       (104,32)       416×26   18px=13.5pt Bold #1F2937
        ///   正文            (160,258)       (104,66)       432×22   13px=9.75pt #4B5563
        ///   绿色标签 ×3     (160,292) 起    (104,100) 起    高 26    12px=9pt，圆角 10，底 #F0FBE8
        ///     实测宽        111 / 87 / 123，间距 8，x = 160 / 279 / 374
        ///   运行卡          (56,350)        (0,158)        576×72   圆角 12，底 #F5F7FA
        ///     复选框        (72,377)        卡内 (16,27)   18×18    #1677FF
        ///     浅蓝图标块    (102,370)       卡内 (46,20)   32×32    圆角 8，底 #E6F4FF
        ///       播放图标    块内居中        块内 (8,8)     16×16    #1677FF
        ///     勾选文案      (146,366)       卡内 (90,16)   470×18   12px=9pt #1F2937
        ///     勾选提示      (146,386)       卡内 (90,36)   470×18   12px=9pt #9CA3AF
        ///   卸载提示 ⓘ      (56,440)        (0,248)        14×14    #9CA3AF
        ///   卸载提示文案    (80,429)        (24,237)       384×40   12px=9pt #9CA3AF（两行）
        ///
        /// 本页同时被「安装完成」和「卸载完成」复用：SetState/SetNeutral/ShowRunOption
        /// 只改颜色与可见性，不动坐标，所以卸载态的结构与安装态一致。
        ///
        /// 页面尺寸固定，所以用绝对定位而不是嵌套布局容器 —— 一是能和设计稿一一对应，
        /// 二是嵌套容器的不透明背景会盖掉父面板的圆角。
        /// 注意：WinForms 的 Padding 不参与绝对定位（只影响 Dock/Anchor 子控件），
        /// 所以面板内子控件坐标一律写成「设计稿绝对坐标 − 面板绝对坐标」，不依赖 Padding。
        /// </remarks>
        private void InitializeComponent()
        {
            this.badgePanel = new AntdUI.Panel();
            this.lblBadge = new AntdUI.Label();
            this.lblTitle = new AntdUI.Label();
            this.lblBody = new AntdUI.Label();
            this.chipPanel1 = new AntdUI.Panel();
            this.chip1 = new AntdUI.Label();
            this.chipPanel2 = new AntdUI.Panel();
            this.chip2 = new AntdUI.Label();
            this.chipPanel3 = new AntdUI.Panel();
            this.chip3 = new AntdUI.Label();
            this.runPanel = new AntdUI.Panel();
            this.runIconBox = new AntdUI.Panel();
            this.lblRunIcon = new AntdUI.Label();
            this.chkRun = new AntdUI.Checkbox();
            this.lblRun = new AntdUI.Label();
            this.lblRunHint = new AntdUI.Label();
            this.iconUninstall = new AntdUI.Label();
            this.lblUninstallHint = new AntdUI.Label();
            this.badgePanel.SuspendLayout();
            this.chipPanel1.SuspendLayout();
            this.chipPanel2.SuspendLayout();
            this.chipPanel3.SuspendLayout();
            this.runPanel.SuspendLayout();
            this.runIconBox.SuspendLayout();
            this.SuspendLayout();
            //
            // badgePanel —— 设计稿 (56,231) 80×80，圆角 40（即正圆），底色由 ApplyAccent 设
            //
            this.badgePanel.Controls.Add(this.lblBadge);
            this.badgePanel.Location = new System.Drawing.Point(0, 39);
            this.badgePanel.Margin = new System.Windows.Forms.Padding(0);
            this.badgePanel.Name = "badgePanel";
            this.badgePanel.Radius = 40;
            this.badgePanel.Size = new System.Drawing.Size(80, 80);
            this.badgePanel.TabIndex = 0;
            //
            // lblBadge —— 圆徽内 (20,20) 40×40，白色对勾（字号只用来定图标大小）
            //
            this.lblBadge.AutoSize = false;
            this.lblBadge.Font = new System.Drawing.Font("微软雅黑", 30F);
            this.lblBadge.IconGap = 0;
            this.lblBadge.Location = new System.Drawing.Point(20, 20);
            this.lblBadge.Margin = new System.Windows.Forms.Padding(0);
            this.lblBadge.Name = "lblBadge";
            this.lblBadge.PrefixSvg = "<svg viewBox=\"0 0 24 24\"><path d=\"M5 12.6l4.6 4.6L19 7.4\" fill=\"none\" stroke=\"#FFFFFF\" stroke-width=\"2.6\" stroke-linecap=\"round\" stroke-linejoin=\"round\"/></svg>";
            this.lblBadge.Size = new System.Drawing.Size(40, 40);
            this.lblBadge.TabIndex = 0;
            this.lblBadge.Text = " ";
            //
            // lblTitle —— 设计稿 (160,224) 416×26，18px = 13.5pt Bold #1F2937
            //
            this.lblTitle.AutoSize = false;
            this.lblTitle.Font = new System.Drawing.Font("微软雅黑", 13.5F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(41)))), ((int)(((byte)(55)))));
            this.lblTitle.Location = new System.Drawing.Point(104, 32);
            this.lblTitle.Margin = new System.Windows.Forms.Padding(0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(416, 26);
            this.lblTitle.TabIndex = 1;
            this.lblTitle.Text = "「空中小火车」安装完成";
            //
            // lblBody —— 设计稿 (160,258) 432×22，13px = 9.75pt #4B5563
            //
            // 宽度按设计稿正文墨迹实测右边界 x=592 定：160..592 共 432，
            // 比标题的 416 宽 —— 正文比标题长，用 416 会把设计稿自己的尾字裁掉。
            //
            this.lblBody.AutoSize = false;
            this.lblBody.Font = new System.Drawing.Font("微软雅黑", 9.75F);
            this.lblBody.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(85)))), ((int)(((byte)(99)))));
            this.lblBody.Location = new System.Drawing.Point(104, 66);
            this.lblBody.Margin = new System.Windows.Forms.Padding(0);
            this.lblBody.Name = "lblBody";
            this.lblBody.Size = new System.Drawing.Size(432, 22);
            this.lblBody.TabIndex = 2;
            this.lblBody.Text = "已成功安装到 C:\\Program Files\\空中小火车，共写入 199 个文件、90.6 MB。";
            //
            // chipPanel1..3 —— 设计稿 y=292，视图 y=100，高 26，圆角 10，底 #F0FBE8
            // 实测 x/宽 = 160/111、279/87、374/123；SetChips 会按可见项重排 x 与宽。
            // AntdUI.Label 没有 Radius/Back，所以圆角底用 AntdUI.Panel 承载，
            // 里面的 Label 设 BackColor = Transparent，否则会画白底盖掉圆角。
            //
            this.chipPanel1.Back = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(251)))), ((int)(((byte)(232)))));
            this.chipPanel1.Controls.Add(this.chip1);
            this.chipPanel1.Location = new System.Drawing.Point(104, 100);
            this.chipPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.chipPanel1.Name = "chipPanel1";
            this.chipPanel1.Radius = 10;
            this.chipPanel1.Size = new System.Drawing.Size(111, 26);
            this.chipPanel1.TabIndex = 3;
            this.chipPanel2.Back = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(251)))), ((int)(((byte)(232)))));
            this.chipPanel2.Controls.Add(this.chip2);
            this.chipPanel2.Location = new System.Drawing.Point(223, 100);
            this.chipPanel2.Margin = new System.Windows.Forms.Padding(0);
            this.chipPanel2.Name = "chipPanel2";
            this.chipPanel2.Radius = 10;
            this.chipPanel2.Size = new System.Drawing.Size(87, 26);
            this.chipPanel2.TabIndex = 4;
            this.chipPanel3.Back = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(251)))), ((int)(((byte)(232)))));
            this.chipPanel3.Controls.Add(this.chip3);
            this.chipPanel3.Location = new System.Drawing.Point(318, 100);
            this.chipPanel3.Margin = new System.Windows.Forms.Padding(0);
            this.chipPanel3.Name = "chipPanel3";
            this.chipPanel3.Radius = 10;
            this.chipPanel3.Size = new System.Drawing.Size(123, 26);
            this.chipPanel3.TabIndex = 5;
            //
            // chip1..chip3 —— 面板内 (10,0)，宽 = 面板宽 − 23（左 10 + 右 13），12px = 9pt
            //
            this.chip1.AutoSize = false;
            this.chip1.BackColor = System.Drawing.Color.Transparent;
            this.chip1.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.chip1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(82)))), ((int)(((byte)(196)))), ((int)(((byte)(26)))));
            this.chip1.IconGap = 4;
            this.chip1.Location = new System.Drawing.Point(10, 0);
            this.chip1.Margin = new System.Windows.Forms.Padding(0);
            this.chip1.Name = "chip1";
            this.chip1.PrefixSvg = "<svg viewBox=\"0 0 16 16\"><path d=\"M2.6 8.4l3.6 3.6 7-7.4\" fill=\"none\" stroke=\"#52C41A\" stroke-width=\"2\" stroke-linecap=\"round\" stroke-linejoin=\"round\"/></svg>";
            this.chip1.Size = new System.Drawing.Size(88, 26);
            this.chip1.TabIndex = 0;
            this.chip1.Text = "桌面快捷方式";
            this.chip1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.chip2.AutoSize = false;
            this.chip2.BackColor = System.Drawing.Color.Transparent;
            this.chip2.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.chip2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(82)))), ((int)(((byte)(196)))), ((int)(((byte)(26)))));
            this.chip2.IconGap = 4;
            this.chip2.Location = new System.Drawing.Point(10, 0);
            this.chip2.Margin = new System.Windows.Forms.Padding(0);
            this.chip2.Name = "chip2";
            this.chip2.PrefixSvg = "<svg viewBox=\"0 0 16 16\"><path d=\"M2.6 8.4l3.6 3.6 7-7.4\" fill=\"none\" stroke=\"#52C41A\" stroke-width=\"2\" stroke-linecap=\"round\" stroke-linejoin=\"round\"/></svg>";
            this.chip2.Size = new System.Drawing.Size(64, 26);
            this.chip2.TabIndex = 0;
            this.chip2.Text = "开始菜单";
            this.chip2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.chip3.AutoSize = false;
            this.chip3.BackColor = System.Drawing.Color.Transparent;
            this.chip3.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.chip3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(82)))), ((int)(((byte)(196)))), ((int)(((byte)(26)))));
            this.chip3.IconGap = 4;
            this.chip3.Location = new System.Drawing.Point(10, 0);
            this.chip3.Margin = new System.Windows.Forms.Padding(0);
            this.chip3.Name = "chip3";
            this.chip3.PrefixSvg = "<svg viewBox=\"0 0 16 16\"><path d=\"M2.6 8.4l3.6 3.6 7-7.4\" fill=\"none\" stroke=\"#52C41A\" stroke-width=\"2\" stroke-linecap=\"round\" stroke-linejoin=\"round\"/></svg>";
            this.chip3.Size = new System.Drawing.Size(100, 26);
            this.chip3.TabIndex = 0;
            this.chip3.Text = "安装后自动运行";
            this.chip3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // runPanel —— 设计稿 (56,350) 576×72，圆角 12，底 #F5F7FA
            //
            this.runPanel.Controls.Add(this.lblRunHint);
            this.runPanel.Controls.Add(this.lblRun);
            this.runPanel.Controls.Add(this.runIconBox);
            this.runPanel.Controls.Add(this.chkRun);
            this.runPanel.Location = new System.Drawing.Point(0, 158);
            this.runPanel.Margin = new System.Windows.Forms.Padding(0);
            this.runPanel.Name = "runPanel";
            this.runPanel.Radius = 12;
            this.runPanel.Size = new System.Drawing.Size(576, 72);
            this.runPanel.TabIndex = 6;
            //
            // chkRun —— 设计稿 (72,377)，卡内 (16,27)，18×18；文案由 lblRun 承担
            //
            this.chkRun.AutoSize = false;
            this.chkRun.Checked = true;
            this.chkRun.Location = new System.Drawing.Point(16, 27);
            this.chkRun.Margin = new System.Windows.Forms.Padding(0);
            this.chkRun.Name = "chkRun";
            this.chkRun.Size = new System.Drawing.Size(18, 18);
            this.chkRun.TabIndex = 0;
            this.chkRun.Text = "";
            //
            // runIconBox —— 设计稿 (102,370)，卡内 (46,20)，32×32，圆角 8，底 #E6F4FF
            //
            this.runIconBox.Back = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(244)))), ((int)(((byte)(255)))));
            this.runIconBox.Controls.Add(this.lblRunIcon);
            this.runIconBox.Location = new System.Drawing.Point(46, 20);
            this.runIconBox.Margin = new System.Windows.Forms.Padding(0);
            this.runIconBox.Name = "runIconBox";
            this.runIconBox.Radius = 8;
            this.runIconBox.Size = new System.Drawing.Size(32, 32);
            this.runIconBox.TabIndex = 1;
            //
            // lblRunIcon —— 浅蓝块内 (7,7) 18×18，蓝色播放图标
            //
            this.lblRunIcon.AutoSize = false;
            this.lblRunIcon.Font = new System.Drawing.Font("微软雅黑", 13F);
            this.lblRunIcon.IconGap = 0;
            this.lblRunIcon.Location = new System.Drawing.Point(7, 7);
            this.lblRunIcon.Margin = new System.Windows.Forms.Padding(0);
            this.lblRunIcon.Name = "lblRunIcon";
            this.lblRunIcon.PrefixSvg = "<svg viewBox=\"0 0 24 24\"><circle cx=\"12\" cy=\"12\" r=\"9.6\" fill=\"none\" stroke=\"#1677FF\" stroke-width=\"1.9\"/><path d=\"M10.2 8.4l5.4 3.6-5.4 3.6z\" fill=\"#1677FF\"/></svg>";
            this.lblRunIcon.Size = new System.Drawing.Size(18, 18);
            this.lblRunIcon.TabIndex = 0;
            this.lblRunIcon.Text = " ";
            //
            // lblRun —— 设计稿 (146,366)，卡内 (90,16)，12px = 9pt #1F2937
            //
            this.lblRun.AutoSize = false;
            this.lblRun.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.lblRun.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(41)))), ((int)(((byte)(55)))));
            this.lblRun.Location = new System.Drawing.Point(90, 16);
            this.lblRun.Margin = new System.Windows.Forms.Padding(0);
            this.lblRun.Name = "lblRun";
            this.lblRun.Size = new System.Drawing.Size(470, 18);
            this.lblRun.TabIndex = 2;
            this.lblRun.Text = "立即运行「空中小火车」";
            //
            // lblRunHint —— 设计稿 (146,386)，卡内 (90,36)，12px = 9pt #9CA3AF
            //
            this.lblRunHint.AutoSize = false;
            this.lblRunHint.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.lblRunHint.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(163)))), ((int)(((byte)(175)))));
            this.lblRunHint.Location = new System.Drawing.Point(90, 36);
            this.lblRunHint.Margin = new System.Windows.Forms.Padding(0);
            this.lblRunHint.Name = "lblRunHint";
            this.lblRunHint.Size = new System.Drawing.Size(470, 18);
            this.lblRunHint.TabIndex = 3;
            this.lblRunHint.Text = "点击「完成」后将自动启动程序";
            //
            // iconUninstall —— 设计稿 (56,440) 14×14，灰色 ⓘ
            //
            this.iconUninstall.AutoSize = false;
            this.iconUninstall.Font = new System.Drawing.Font("微软雅黑", 10.5F);
            this.iconUninstall.IconGap = 0;
            this.iconUninstall.Location = new System.Drawing.Point(0, 248);
            this.iconUninstall.Margin = new System.Windows.Forms.Padding(0);
            this.iconUninstall.Name = "iconUninstall";
            this.iconUninstall.PrefixSvg = "<svg viewBox=\"0 0 16 16\"><circle cx=\"8\" cy=\"8\" r=\"6.9\" fill=\"none\" stroke=\"#9CA3AF\" stroke-width=\"1.4\"/><path d=\"M8 7.4v4.2\" stroke=\"#9CA3AF\" stroke-width=\"1.4\" stroke-linecap=\"round\"/><circle cx=\"8\" cy=\"4.9\" r=\"0.95\" fill=\"#9CA3AF\"/></svg>";
            this.iconUninstall.Size = new System.Drawing.Size(14, 14);
            this.iconUninstall.TabIndex = 7;
            this.iconUninstall.Text = " ";
            //
            // lblUninstallHint —— 设计稿 (80,429) 384×40，12px = 9pt #9CA3AF（两行）
            //
            this.lblUninstallHint.AutoSize = false;
            this.lblUninstallHint.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.lblUninstallHint.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(163)))), ((int)(((byte)(175)))));
            this.lblUninstallHint.Location = new System.Drawing.Point(24, 237);
            this.lblUninstallHint.Margin = new System.Windows.Forms.Padding(0);
            this.lblUninstallHint.Name = "lblUninstallHint";
            this.lblUninstallHint.Size = new System.Drawing.Size(384, 40);
            this.lblUninstallHint.TabIndex = 8;
            this.lblUninstallHint.Text = "如需卸载，可在「设置 → 应用 → 已安装的应用」中找到「空中小火车」。";
            this.lblUninstallHint.TextMultiLine = true;
            //
            // FinishView
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.lblUninstallHint);
            this.Controls.Add(this.iconUninstall);
            this.Controls.Add(this.runPanel);
            this.Controls.Add(this.chipPanel3);
            this.Controls.Add(this.chipPanel2);
            this.Controls.Add(this.chipPanel1);
            this.Controls.Add(this.lblBody);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.badgePanel);
            this.Font = new System.Drawing.Font("微软雅黑", 9.75F);
            this.Name = "FinishView";
            this.Size = new System.Drawing.Size(576, 296);
            this.badgePanel.ResumeLayout(false);
            this.chipPanel1.ResumeLayout(false);
            this.chipPanel2.ResumeLayout(false);
            this.chipPanel3.ResumeLayout(false);
            this.runPanel.ResumeLayout(false);
            this.runIconBox.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private AntdUI.Panel badgePanel;
        private AntdUI.Label lblBadge;
        private AntdUI.Label lblTitle;
        private AntdUI.Label lblBody;
        private AntdUI.Panel chipPanel1;
        private AntdUI.Label chip1;
        private AntdUI.Panel chipPanel2;
        private AntdUI.Label chip2;
        private AntdUI.Panel chipPanel3;
        private AntdUI.Label chip3;
        private AntdUI.Panel runPanel;
        private AntdUI.Panel runIconBox;
        private AntdUI.Label lblRunIcon;
        private AntdUI.Checkbox chkRun;
        private AntdUI.Label lblRun;
        private AntdUI.Label lblRunHint;
        private AntdUI.Label iconUninstall;
        private AntdUI.Label lblUninstallHint;
    }
}
