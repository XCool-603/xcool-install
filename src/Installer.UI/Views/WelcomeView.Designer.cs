namespace Installer.UI.Views
{
    partial class WelcomeView
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
        /// 本页坐标逐元素照抄 Calicat 设计稿第 3 版（欢迎页），不做二次推导。
        ///
        /// 设计稿的内容区是 (24,192) 640x424、内边距 32，所以本视图坐标系原点在
        /// 窗口 (56,192)。下表是「设计稿绝对坐标 − (56,192)」：
        ///
        ///   页面标题   (56,224) -> (0, 32)     18px = 13.5pt SemiBold #1F2937
        ///   正文段1    (56,262) -> (0, 70)     13px = 9.75pt Regular  #4B5563
        ///   正文段2    (56,290) -> (0, 98)     13px = 9.75pt Regular  #4B5563
        ///   信息卡     (56,336) -> (0,144) 576x186 圆角14 底#F5F7FA 内边距20
        ///   提示条     (56,542) -> (0,350) 576x42  圆角10 底#FFFBE6 内边距 12/16
        ///
        /// 页面尺寸固定，所以这里用绝对定位而不是嵌套布局容器 ——
        /// 一是能和设计稿一一对应，二是嵌套容器的不透明背景会盖掉父面板的圆角。
        /// </remarks>
        private void InitializeComponent()
        {
            this.lblTitle = new AntdUI.Label();
            this.lblBody = new AntdUI.Label();
            this.lblBody2 = new AntdUI.Label();
            this.infoCard = new AntdUI.Panel();
            this.row1 = new AntdUI.Panel();
            this.boxDir = new AntdUI.Panel();
            this.iconDir = new AntdUI.Label();
            this.lblDirCaption = new AntdUI.Label();
            this.lblDirValue = new AntdUI.Label();
            this.row2 = new AntdUI.Panel();
            this.boxSpace = new AntdUI.Panel();
            this.iconSpace = new AntdUI.Label();
            this.lblSpaceCaption = new AntdUI.Label();
            this.lblSpaceValue = new AntdUI.Label();
            this.row3 = new AntdUI.Panel();
            this.boxVersion = new AntdUI.Panel();
            this.iconVersion = new AntdUI.Label();
            this.lblVersionCaption = new AntdUI.Label();
            this.lblVersionValue = new AntdUI.Label();
            this.hintPanel = new AntdUI.Panel();
            this.lblHintIcon = new AntdUI.Label();
            this.lblHint = new AntdUI.Label();
            this.infoCard.SuspendLayout();
            this.row1.SuspendLayout();
            this.boxDir.SuspendLayout();
            this.row2.SuspendLayout();
            this.boxSpace.SuspendLayout();
            this.row3.SuspendLayout();
            this.boxVersion.SuspendLayout();
            this.hintPanel.SuspendLayout();
            this.SuspendLayout();
            //
            // lblTitle —— 设计稿 (56,224) 576x26，18px = 13.5pt SemiBold
            //
            this.lblTitle.AutoSize = false;
            this.lblTitle.Font = new System.Drawing.Font("微软雅黑", 13.5F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(0, 32);
            this.lblTitle.Margin = new System.Windows.Forms.Padding(0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(576, 26);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "欢迎使用安装向导";
            //
            // lblBody —— 设计稿两段：(56,262) 与 (56,290)，各 576x22，13px = 9.75pt
            //
            // 设计稿是两段独立文字（段间距 6px）。这里合并成一个标签、中间空一行，
            // 视觉结果一致且少一个控件。
            //
            this.lblBody.AutoSize = false;
            this.lblBody.Font = new System.Drawing.Font("微软雅黑", 9.75F);
            this.lblBody.Location = new System.Drawing.Point(0, 70);
            this.lblBody.Margin = new System.Windows.Forms.Padding(0);
            this.lblBody.Name = "lblBody";
            this.lblBody.Size = new System.Drawing.Size(576, 22);
            this.lblBody.TabIndex = 1;
            this.lblBody.Text = "";
            //
            // lblBody2 —— 设计稿 (56,290) 576x22，13px = 9.75pt #4B5563
            //
            this.lblBody2.AutoSize = false;
            this.lblBody2.Font = new System.Drawing.Font("微软雅黑", 9.75F);
            this.lblBody2.Location = new System.Drawing.Point(0, 98);
            this.lblBody2.Margin = new System.Windows.Forms.Padding(0);
            this.lblBody2.Name = "lblBody2";
            this.lblBody2.Size = new System.Drawing.Size(576, 22);
            this.lblBody2.TabIndex = 2;
            this.lblBody2.Text = "";
            //
            // infoCard —— 设计稿 (56,336) 576x186，圆角 14，底 #F5F7FA，内边距 20
            //
            this.infoCard.Controls.Add(this.row3);
            this.infoCard.Controls.Add(this.row2);
            this.infoCard.Controls.Add(this.row1);
            this.infoCard.Location = new System.Drawing.Point(0, 144);
            this.infoCard.Margin = new System.Windows.Forms.Padding(0);
            this.infoCard.Name = "infoCard";
            this.infoCard.Padding = new System.Windows.Forms.Padding(20);
            this.infoCard.Radius = 14;
            this.infoCard.Size = new System.Drawing.Size(576, 186);
            this.infoCard.TabIndex = 2;
            //
            // row1 / row2 / row3 —— 设计稿 y 356 / 410 / 464，行距 54，每行 536x38
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
            // boxDir / boxSpace / boxVersion —— 设计稿 32x32，圆角 9，底 #E6F4FF
            //
            // 只设 AntdUI 的 Back，不设 WinForms 的 BackColor（设了会用直角盖掉圆角）。
            //
            this.boxDir.Controls.Add(this.iconDir);
            this.boxDir.Location = new System.Drawing.Point(0, 3);
            this.boxDir.Margin = new System.Windows.Forms.Padding(0);
            this.boxDir.Name = "boxDir";
            this.boxDir.Radius = 9;
            this.boxDir.Size = new System.Drawing.Size(32, 32);
            this.boxDir.TabIndex = 0;
            this.boxSpace.Controls.Add(this.iconSpace);
            this.boxSpace.Location = new System.Drawing.Point(0, 3);
            this.boxSpace.Margin = new System.Windows.Forms.Padding(0);
            this.boxSpace.Name = "boxSpace";
            this.boxSpace.Radius = 9;
            this.boxSpace.Size = new System.Drawing.Size(32, 32);
            this.boxSpace.TabIndex = 0;
            this.boxVersion.Controls.Add(this.iconVersion);
            this.boxVersion.Location = new System.Drawing.Point(0, 3);
            this.boxVersion.Margin = new System.Windows.Forms.Padding(0);
            this.boxVersion.Name = "boxVersion";
            this.boxVersion.Radius = 9;
            this.boxVersion.Size = new System.Drawing.Size(32, 32);
            this.boxVersion.TabIndex = 0;
            //
            // iconDir / iconSpace / iconVersion —— 设计稿 18x18，居中于 32x32 块内
            //
            this.iconDir.AutoSize = false;
            this.iconDir.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.iconDir.IconGap = 0;
            this.iconDir.Location = new System.Drawing.Point(4, 4);
            this.iconDir.Margin = new System.Windows.Forms.Padding(0);
            this.iconDir.Name = "iconDir";
            this.iconDir.PrefixSvg = "<svg viewBox=\"0 0 24 24\"><path d=\"M3 6.4h6l2.4 2.4h9.6v11.2H3z\" fill=\"none\" stroke=\"#1677FF\" stroke-width=\"1.8\" stroke-linejoin=\"round\"/></svg>";
            this.iconDir.Size = new System.Drawing.Size(24, 24);
            this.iconDir.TabIndex = 0;
            this.iconDir.Text = " ";
            this.iconSpace.AutoSize = false;
            this.iconSpace.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.iconSpace.IconGap = 0;
            this.iconSpace.Location = new System.Drawing.Point(4, 4);
            this.iconSpace.Margin = new System.Windows.Forms.Padding(0);
            this.iconSpace.Name = "iconSpace";
            this.iconSpace.PrefixSvg = "<svg viewBox=\"0 0 24 24\"><rect x=\"3\" y=\"4\" width=\"18\" height=\"16\" rx=\"2.4\" fill=\"none\" stroke=\"#1677FF\" stroke-width=\"1.8\"/><path d=\"M3 9.4h18\" stroke=\"#1677FF\" stroke-width=\"1.8\"/><circle cx=\"6.6\" cy=\"6.8\" r=\"0.9\" fill=\"#1677FF\"/></svg>";
            this.iconSpace.Size = new System.Drawing.Size(24, 24);
            this.iconSpace.TabIndex = 0;
            this.iconSpace.Text = " ";
            this.iconVersion.AutoSize = false;
            this.iconVersion.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.iconVersion.IconGap = 0;
            this.iconVersion.Location = new System.Drawing.Point(4, 4);
            this.iconVersion.Margin = new System.Windows.Forms.Padding(0);
            this.iconVersion.Name = "iconVersion";
            this.iconVersion.PrefixSvg = "<svg viewBox=\"0 0 24 24\"><path d=\"M12 3.4 20.6 8v8L12 20.6 3.4 16V8z\" fill=\"none\" stroke=\"#1677FF\" stroke-width=\"1.8\" stroke-linejoin=\"round\"/><circle cx=\"12\" cy=\"12\" r=\"2.4\" fill=\"#1677FF\"/></svg>";
            this.iconVersion.Size = new System.Drawing.Size(24, 24);
            this.iconVersion.TabIndex = 0;
            this.iconVersion.Text = " ";
            //
            // 标签 / 值 —— 设计稿文本列 x=44（图标块 32 + 间距 12）
            //   标签 (44, 0)  12px = 9pt    #9CA3AF
            //   值   (44,18)  13px = 9.75pt #1F2937
            //
            this.lblDirCaption.AutoSize = false;
            this.lblDirCaption.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.lblDirCaption.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(163)))), ((int)(((byte)(175)))));
            this.lblDirCaption.Location = new System.Drawing.Point(44, 0);
            this.lblDirCaption.Margin = new System.Windows.Forms.Padding(0);
            this.lblDirCaption.Name = "lblDirCaption";
            this.lblDirCaption.Size = new System.Drawing.Size(492, 18);
            this.lblDirCaption.TabIndex = 1;
            this.lblDirCaption.Text = "默认安装位置";
            this.lblDirValue.AutoSize = false;
            this.lblDirValue.Font = new System.Drawing.Font("微软雅黑", 9.75F);
            this.lblDirValue.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(41)))), ((int)(((byte)(55)))));
            this.lblDirValue.Location = new System.Drawing.Point(44, 18);
            this.lblDirValue.Margin = new System.Windows.Forms.Padding(0);
            this.lblDirValue.Name = "lblDirValue";
            this.lblDirValue.Size = new System.Drawing.Size(492, 20);
            this.lblDirValue.TabIndex = 2;
            this.lblDirValue.Text = "";
            this.lblSpaceCaption.AutoSize = false;
            this.lblSpaceCaption.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.lblSpaceCaption.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(163)))), ((int)(((byte)(175)))));
            this.lblSpaceCaption.Location = new System.Drawing.Point(44, 0);
            this.lblSpaceCaption.Margin = new System.Windows.Forms.Padding(0);
            this.lblSpaceCaption.Name = "lblSpaceCaption";
            this.lblSpaceCaption.Size = new System.Drawing.Size(492, 18);
            this.lblSpaceCaption.TabIndex = 1;
            this.lblSpaceCaption.Text = "需要磁盘空间";
            this.lblSpaceValue.AutoSize = false;
            this.lblSpaceValue.Font = new System.Drawing.Font("微软雅黑", 9.75F);
            this.lblSpaceValue.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(41)))), ((int)(((byte)(55)))));
            this.lblSpaceValue.Location = new System.Drawing.Point(44, 18);
            this.lblSpaceValue.Margin = new System.Windows.Forms.Padding(0);
            this.lblSpaceValue.Name = "lblSpaceValue";
            this.lblSpaceValue.Size = new System.Drawing.Size(492, 20);
            this.lblSpaceValue.TabIndex = 2;
            this.lblSpaceValue.Text = "";
            this.lblVersionCaption.AutoSize = false;
            this.lblVersionCaption.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.lblVersionCaption.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(163)))), ((int)(((byte)(175)))));
            this.lblVersionCaption.Location = new System.Drawing.Point(44, 0);
            this.lblVersionCaption.Margin = new System.Windows.Forms.Padding(0);
            this.lblVersionCaption.Name = "lblVersionCaption";
            this.lblVersionCaption.Size = new System.Drawing.Size(492, 18);
            this.lblVersionCaption.TabIndex = 1;
            this.lblVersionCaption.Text = "版本与发布者";
            this.lblVersionValue.AutoSize = false;
            this.lblVersionValue.Font = new System.Drawing.Font("微软雅黑", 9.75F);
            this.lblVersionValue.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(41)))), ((int)(((byte)(55)))));
            this.lblVersionValue.Location = new System.Drawing.Point(44, 18);
            this.lblVersionValue.Margin = new System.Windows.Forms.Padding(0);
            this.lblVersionValue.Name = "lblVersionValue";
            this.lblVersionValue.Size = new System.Drawing.Size(492, 20);
            this.lblVersionValue.TabIndex = 2;
            this.lblVersionValue.Text = "";
            //
            // 把行内控件挂上去
            //
            this.row1.Controls.Add(this.lblDirValue);
            this.row1.Controls.Add(this.lblDirCaption);
            this.row1.Controls.Add(this.boxDir);
            this.row2.Controls.Add(this.lblSpaceValue);
            this.row2.Controls.Add(this.lblSpaceCaption);
            this.row2.Controls.Add(this.boxSpace);
            this.row3.Controls.Add(this.lblVersionValue);
            this.row3.Controls.Add(this.lblVersionCaption);
            this.row3.Controls.Add(this.boxVersion);
            //
            // hintPanel —— 设计稿 (56,542) 576x42，圆角 10，底 #FFFBE6，内边距 12/16
            //
            this.hintPanel.Controls.Add(this.lblHint);
            this.hintPanel.Controls.Add(this.lblHintIcon);
            this.hintPanel.Location = new System.Drawing.Point(0, 350);
            this.hintPanel.Margin = new System.Windows.Forms.Padding(0);
            this.hintPanel.Name = "hintPanel";
            this.hintPanel.Padding = new System.Windows.Forms.Padding(16, 12, 16, 12);
            this.hintPanel.Radius = 10;
            this.hintPanel.Size = new System.Drawing.Size(576, 42);
            this.hintPanel.TabIndex = 3;
            //
            // lblHintIcon —— 设计稿 ⓘ 位于 (72,555)，即面板内 (0,1)，16x16 #FAAD14
            //
            this.lblHintIcon.AutoSize = false;
            this.lblHintIcon.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.lblHintIcon.IconGap = 0;
            this.lblHintIcon.Location = new System.Drawing.Point(16, 13);
            this.lblHintIcon.Margin = new System.Windows.Forms.Padding(0);
            this.lblHintIcon.Name = "lblHintIcon";
            this.lblHintIcon.PrefixSvg = "<svg viewBox=\"0 0 24 24\"><circle cx=\"12\" cy=\"12\" r=\"9.4\" fill=\"none\" stroke=\"#FAAD14\" stroke-width=\"1.9\"/><path d=\"M12 10.8v6\" stroke=\"#FAAD14\" stroke-width=\"1.9\" stroke-linecap=\"round\"/><circle cx=\"12\" cy=\"7.4\" r=\"1.25\" fill=\"#FAAD14\"/></svg>";
            this.lblHintIcon.Size = new System.Drawing.Size(20, 20);
            this.lblHintIcon.TabIndex = 0;
            this.lblHintIcon.Text = " ";
            //
            // lblHint —— 设计稿 (96,554)，即面板内 (24,0)，12px = 9pt #7C6016
            //
            this.lblHint.AutoSize = false;
            this.lblHint.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.lblHint.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(124)))), ((int)(((byte)(96)))), ((int)(((byte)(22)))));
            this.lblHint.Location = new System.Drawing.Point(40, 12);
            this.lblHint.Margin = new System.Windows.Forms.Padding(0);
            this.lblHint.Name = "lblHint";
            this.lblHint.Size = new System.Drawing.Size(536, 18);
            this.lblHint.TabIndex = 1;
            this.lblHint.Text = "";
            //
            // WelcomeView
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.hintPanel);
            this.Controls.Add(this.infoCard);
            this.Controls.Add(this.lblBody2);
            this.Controls.Add(this.lblBody);
            this.Controls.Add(this.lblTitle);
            this.Font = new System.Drawing.Font("微软雅黑", 9.75F);
            this.Name = "WelcomeView";
            this.Size = new System.Drawing.Size(576, 424);
            this.infoCard.ResumeLayout(false);
            this.row1.ResumeLayout(false);
            this.boxDir.ResumeLayout(false);
            this.row2.ResumeLayout(false);
            this.boxSpace.ResumeLayout(false);
            this.row3.ResumeLayout(false);
            this.boxVersion.ResumeLayout(false);
            this.hintPanel.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private AntdUI.Label lblTitle;
        private AntdUI.Label lblBody;
        private AntdUI.Label lblBody2;
        private AntdUI.Panel infoCard;
        private AntdUI.Panel row1;
        private AntdUI.Panel boxDir;
        private AntdUI.Label iconDir;
        private AntdUI.Label lblDirCaption;
        private AntdUI.Label lblDirValue;
        private AntdUI.Panel row2;
        private AntdUI.Panel boxSpace;
        private AntdUI.Label iconSpace;
        private AntdUI.Label lblSpaceCaption;
        private AntdUI.Label lblSpaceValue;
        private AntdUI.Panel row3;
        private AntdUI.Panel boxVersion;
        private AntdUI.Label iconVersion;
        private AntdUI.Label lblVersionCaption;
        private AntdUI.Label lblVersionValue;
        private AntdUI.Panel hintPanel;
        private AntdUI.Label lblHintIcon;
        private AntdUI.Label lblHint;
    }
}
