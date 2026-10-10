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
        private void InitializeComponent()
        {
            this.rootLayout = new System.Windows.Forms.TableLayoutPanel();
            this.lblTitle = new AntdUI.Label();
            this.lblBody = new AntdUI.Label();
            this.infoCard = new AntdUI.Panel();
            this.infoLayout = new System.Windows.Forms.TableLayoutPanel();
            this.boxDir = new AntdUI.Panel();
            this.boxSpace = new AntdUI.Panel();
            this.boxVersion = new AntdUI.Panel();
            this.iconDir = new AntdUI.Label();
            this.cellDir = new System.Windows.Forms.TableLayoutPanel();
            this.lblDirCaption = new AntdUI.Label();
            this.lblDirValue = new AntdUI.Label();
            this.iconSpace = new AntdUI.Label();
            this.cellSpace = new System.Windows.Forms.TableLayoutPanel();
            this.lblSpaceCaption = new AntdUI.Label();
            this.lblSpaceValue = new AntdUI.Label();
            this.iconVersion = new AntdUI.Label();
            this.cellVersion = new System.Windows.Forms.TableLayoutPanel();
            this.lblVersionCaption = new AntdUI.Label();
            this.lblVersionValue = new AntdUI.Label();
            this.hintPanel = new AntdUI.Panel();
            this.lblHint = new AntdUI.Label();
            this.rootLayout.SuspendLayout();
            this.infoCard.SuspendLayout();
            this.infoLayout.SuspendLayout();
            this.cellDir.SuspendLayout();
            this.cellSpace.SuspendLayout();
            this.cellVersion.SuspendLayout();
            this.hintPanel.SuspendLayout();
            this.SuspendLayout();
            //
            // rootLayout
            //
            this.rootLayout.ColumnCount = 1;
            this.rootLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rootLayout.Controls.Add(this.lblTitle, 0, 0);
            this.rootLayout.Controls.Add(this.lblBody, 0, 1);
            this.rootLayout.Controls.Add(this.infoCard, 0, 2);
            this.rootLayout.Controls.Add(this.hintPanel, 0, 3);
            this.rootLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rootLayout.Location = new System.Drawing.Point(0, 0);
            this.rootLayout.Name = "rootLayout";
            this.rootLayout.RowCount = 5;
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 86F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 186F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 62F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rootLayout.Size = new System.Drawing.Size(600, 400);
            this.rootLayout.TabIndex = 0;
            //
            // lblTitle
            //
            this.lblTitle.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("微软雅黑", 13.5F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(0, 0);
            this.lblTitle.Margin = new System.Windows.Forms.Padding(0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(300, 30);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "欢迎使用安装向导";
            //
            // lblBody
            //
            this.lblBody.AutoSize = false;
            this.lblBody.Font = new System.Drawing.Font("微软雅黑", 9.75F);
            this.lblBody.Location = new System.Drawing.Point(0, 40);
            this.lblBody.Margin = new System.Windows.Forms.Padding(0, 8, 0, 0);
            this.lblBody.Name = "lblBody";
            this.lblBody.Size = new System.Drawing.Size(600, 88);
            this.lblBody.TabIndex = 1;
            this.lblBody.Text = "";
            //
            // infoCard
            //
            this.infoCard.Controls.Add(this.infoLayout);
            this.infoCard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.infoCard.Location = new System.Drawing.Point(0, 136);
            this.infoCard.Margin = new System.Windows.Forms.Padding(0);
            this.infoCard.Name = "infoCard";
            this.infoCard.Padding = new System.Windows.Forms.Padding(16, 12, 16, 12);
            this.infoCard.Radius = 12;
            this.infoCard.Size = new System.Drawing.Size(600, 186);
            this.infoCard.TabIndex = 2;
            //
            // infoLayout
            //
            this.infoLayout.ColumnCount = 2;
            this.infoLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            this.infoLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.boxDir.Controls.Add(this.iconDir);
            this.infoLayout.Controls.Add(this.boxDir, 0, 0);
            this.infoLayout.Controls.Add(this.cellDir, 1, 0);
            this.boxSpace.Controls.Add(this.iconSpace);
            this.infoLayout.Controls.Add(this.boxSpace, 0, 1);
            this.infoLayout.Controls.Add(this.cellSpace, 1, 1);
            this.boxVersion.Controls.Add(this.iconVersion);
            this.infoLayout.Controls.Add(this.boxVersion, 0, 2);
            this.infoLayout.Controls.Add(this.cellVersion, 1, 2);
            this.infoLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.infoLayout.Location = new System.Drawing.Point(16, 12);
            this.infoLayout.Margin = new System.Windows.Forms.Padding(0);
            this.infoLayout.Name = "infoLayout";
            this.infoLayout.RowCount = 3;
            this.infoLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33F));
            this.infoLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33F));
            this.infoLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.34F));
            this.infoLayout.Size = new System.Drawing.Size(568, 162);
            this.infoLayout.TabIndex = 0;
            //
            // boxDir / boxSpace / boxVersion —— 信息卡每行的 32x32 浅蓝底图标块
            //
            // 设计稿：32x32，圆角 9，底色 #E6F4FF，图标 #1677FF。
            // 只设 AntdUI 的 Back，不设 WinForms 的 BackColor（设了会用直角盖掉圆角）。
            //
            this.boxDir.Location = new System.Drawing.Point(0, 0);
            this.boxDir.Margin = new System.Windows.Forms.Padding(0, 0, 12, 0);
            this.boxDir.Name = "boxDir";
            this.boxDir.Radius = 9;
            this.boxDir.Size = new System.Drawing.Size(32, 32);
            this.boxDir.TabIndex = 0;
            this.boxSpace.Location = new System.Drawing.Point(0, 0);
            this.boxSpace.Margin = new System.Windows.Forms.Padding(0, 0, 12, 0);
            this.boxSpace.Name = "boxSpace";
            this.boxSpace.Radius = 9;
            this.boxSpace.Size = new System.Drawing.Size(32, 32);
            this.boxSpace.TabIndex = 1;
            this.boxVersion.Location = new System.Drawing.Point(0, 0);
            this.boxVersion.Margin = new System.Windows.Forms.Padding(0, 0, 12, 0);
            this.boxVersion.Name = "boxVersion";
            this.boxVersion.Radius = 9;
            this.boxVersion.Size = new System.Drawing.Size(32, 32);
            this.boxVersion.TabIndex = 2;
            //
            // iconDir
            //
            this.iconDir.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.iconDir.AutoSize = false;
            this.iconDir.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.iconDir.IconGap = 0;
            this.iconDir.Location = new System.Drawing.Point(6, 8);
            this.iconDir.Margin = new System.Windows.Forms.Padding(0);
            this.iconDir.Name = "iconDir";
            this.iconDir.PrefixSvg = "<svg viewBox=\"0 0 16 16\"><path d=\"M1.8 3.6h4.2l1.6 1.6h6.6v7.2H1.8z\" fill=\"none\" stroke=\"#1677FF\" stroke-width=\"1.3\"/></svg>";
            this.iconDir.Size = new System.Drawing.Size(24, 24);
            this.iconDir.TabIndex = 0;
            this.iconDir.Text = " ";
            //
            // cellDir
            //
            this.cellDir.ColumnCount = 1;
            this.cellDir.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.cellDir.Controls.Add(this.lblDirCaption, 0, 0);
            this.cellDir.Controls.Add(this.lblDirValue, 0, 1);
            this.cellDir.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cellDir.Location = new System.Drawing.Point(36, 0);
            this.cellDir.Margin = new System.Windows.Forms.Padding(0);
            this.cellDir.Name = "cellDir";
            this.cellDir.RowCount = 2;
            this.cellDir.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 22F));
            this.cellDir.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.cellDir.Size = new System.Drawing.Size(532, 42);
            this.cellDir.TabIndex = 1;
            //
            // lblDirCaption
            //
            this.lblDirCaption.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblDirCaption.AutoSize = true;
            this.lblDirCaption.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.lblDirCaption.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(163)))), ((int)(((byte)(175)))));
            this.lblDirCaption.Location = new System.Drawing.Point(0, 0);
            this.lblDirCaption.Margin = new System.Windows.Forms.Padding(0);
            this.lblDirCaption.Name = "lblDirCaption";
            this.lblDirCaption.Size = new System.Drawing.Size(100, 19);
            this.lblDirCaption.TabIndex = 0;
            this.lblDirCaption.Text = "默认安装位置";
            //
            // lblDirValue
            //
            this.lblDirValue.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblDirValue.AutoSize = true;
            this.lblDirValue.Font = new System.Drawing.Font("微软雅黑", 9.75F);
            this.lblDirValue.Location = new System.Drawing.Point(0, 18);
            this.lblDirValue.Margin = new System.Windows.Forms.Padding(0);
            this.lblDirValue.Name = "lblDirValue";
            this.lblDirValue.Size = new System.Drawing.Size(200, 24);
            this.lblDirValue.TabIndex = 1;
            this.lblDirValue.Text = "—";
            //
            // iconSpace
            //
            this.iconSpace.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.iconSpace.AutoSize = false;
            this.iconSpace.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.iconSpace.IconGap = 0;
            this.iconSpace.Location = new System.Drawing.Point(6, 50);
            this.iconSpace.Margin = new System.Windows.Forms.Padding(0);
            this.iconSpace.Name = "iconSpace";
            this.iconSpace.PrefixSvg = "<svg viewBox=\"0 0 16 16\"><rect x=\"2\" y=\"2.5\" width=\"12\" height=\"11\" rx=\"1.6\" fill=\"none\" stroke=\"#1677FF\" stroke-width=\"1.3\"/><path d=\"M2 6.2h12\" stroke=\"#1677FF\" stroke-width=\"1.3\"/></svg>";
            this.iconSpace.Size = new System.Drawing.Size(24, 24);
            this.iconSpace.TabIndex = 2;
            this.iconSpace.Text = " ";
            //
            // cellSpace
            //
            this.cellSpace.ColumnCount = 1;
            this.cellSpace.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.cellSpace.Controls.Add(this.lblSpaceCaption, 0, 0);
            this.cellSpace.Controls.Add(this.lblSpaceValue, 0, 1);
            this.cellSpace.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cellSpace.Location = new System.Drawing.Point(36, 42);
            this.cellSpace.Margin = new System.Windows.Forms.Padding(0);
            this.cellSpace.Name = "cellSpace";
            this.cellSpace.RowCount = 2;
            this.cellSpace.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 22F));
            this.cellSpace.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.cellSpace.Size = new System.Drawing.Size(532, 42);
            this.cellSpace.TabIndex = 3;
            //
            // lblSpaceCaption
            //
            this.lblSpaceCaption.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblSpaceCaption.AutoSize = true;
            this.lblSpaceCaption.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.lblSpaceCaption.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(163)))), ((int)(((byte)(175)))));
            this.lblSpaceCaption.Location = new System.Drawing.Point(0, 0);
            this.lblSpaceCaption.Margin = new System.Windows.Forms.Padding(0);
            this.lblSpaceCaption.Name = "lblSpaceCaption";
            this.lblSpaceCaption.Size = new System.Drawing.Size(100, 19);
            this.lblSpaceCaption.TabIndex = 0;
            this.lblSpaceCaption.Text = "需要磁盘空间";
            //
            // lblSpaceValue
            //
            this.lblSpaceValue.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblSpaceValue.AutoSize = true;
            this.lblSpaceValue.Font = new System.Drawing.Font("微软雅黑", 9.75F);
            this.lblSpaceValue.Location = new System.Drawing.Point(0, 18);
            this.lblSpaceValue.Margin = new System.Windows.Forms.Padding(0);
            this.lblSpaceValue.Name = "lblSpaceValue";
            this.lblSpaceValue.Size = new System.Drawing.Size(200, 24);
            this.lblSpaceValue.TabIndex = 1;
            this.lblSpaceValue.Text = "—";
            //
            // iconVersion
            //
            this.iconVersion.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.iconVersion.AutoSize = false;
            this.iconVersion.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.iconVersion.IconGap = 0;
            this.iconVersion.Location = new System.Drawing.Point(6, 93);
            this.iconVersion.Margin = new System.Windows.Forms.Padding(0);
            this.iconVersion.Name = "iconVersion";
            this.iconVersion.PrefixSvg = "<svg viewBox=\"0 0 16 16\"><path d=\"M8 1.8l5.4 3v6.4L8 14.2 2.6 11.2V4.8z\" fill=\"none\" stroke=\"#1677FF\" stroke-width=\"1.3\"/></svg>";
            this.iconVersion.Size = new System.Drawing.Size(24, 24);
            this.iconVersion.TabIndex = 4;
            this.iconVersion.Text = " ";
            //
            // cellVersion
            //
            this.cellVersion.ColumnCount = 1;
            this.cellVersion.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.cellVersion.Controls.Add(this.lblVersionCaption, 0, 0);
            this.cellVersion.Controls.Add(this.lblVersionValue, 0, 1);
            this.cellVersion.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cellVersion.Location = new System.Drawing.Point(36, 84);
            this.cellVersion.Margin = new System.Windows.Forms.Padding(0);
            this.cellVersion.Name = "cellVersion";
            this.cellVersion.RowCount = 2;
            this.cellVersion.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 22F));
            this.cellVersion.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.cellVersion.Size = new System.Drawing.Size(532, 42);
            this.cellVersion.TabIndex = 5;
            //
            // lblVersionCaption
            //
            this.lblVersionCaption.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblVersionCaption.AutoSize = true;
            this.lblVersionCaption.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.lblVersionCaption.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(163)))), ((int)(((byte)(175)))));
            this.lblVersionCaption.Location = new System.Drawing.Point(0, 0);
            this.lblVersionCaption.Margin = new System.Windows.Forms.Padding(0);
            this.lblVersionCaption.Name = "lblVersionCaption";
            this.lblVersionCaption.Size = new System.Drawing.Size(100, 19);
            this.lblVersionCaption.TabIndex = 0;
            this.lblVersionCaption.Text = "版本与发布者";
            //
            // lblVersionValue
            //
            this.lblVersionValue.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblVersionValue.AutoSize = true;
            this.lblVersionValue.Font = new System.Drawing.Font("微软雅黑", 9.75F);
            this.lblVersionValue.Location = new System.Drawing.Point(0, 18);
            this.lblVersionValue.Margin = new System.Windows.Forms.Padding(0);
            this.lblVersionValue.Name = "lblVersionValue";
            this.lblVersionValue.Size = new System.Drawing.Size(200, 24);
            this.lblVersionValue.TabIndex = 1;
            this.lblVersionValue.Text = "—";
            //
            // hintPanel
            //
            this.hintPanel.Controls.Add(this.lblHint);
            this.hintPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.hintPanel.Location = new System.Drawing.Point(0, 322);
            this.hintPanel.Margin = new System.Windows.Forms.Padding(0, 20, 0, 0);
            this.hintPanel.Name = "hintPanel";
            this.hintPanel.Padding = new System.Windows.Forms.Padding(16, 12, 16, 12);
            this.hintPanel.Radius = 8;
            this.hintPanel.Size = new System.Drawing.Size(576, 42);
            this.hintPanel.TabIndex = 3;
            //
            // lblHint
            //
            this.lblHint.AutoSize = false;
            this.lblHint.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblHint.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.lblHint.PrefixSvg = "<svg viewBox=\"0 0 16 16\"><circle cx=\"8\" cy=\"8\" r=\"6.6\" fill=\"none\" stroke=\"#FAAD14\" stroke-width=\"1.4\"/><path d=\"M8 7.2v4\" stroke=\"#FAAD14\" stroke-width=\"1.4\" stroke-linecap=\"round\"/><circle cx=\"8\" cy=\"4.9\" r=\"0.9\" fill=\"#FAAD14\"/></svg>";
            this.lblHint.Location = new System.Drawing.Point(16, 12);
            this.lblHint.Margin = new System.Windows.Forms.Padding(0);
            this.lblHint.Name = "lblHint";
            this.lblHint.Size = new System.Drawing.Size(544, 18);
            this.lblHint.TabIndex = 0;
            this.lblHint.Text = "";
            //
            // WelcomeView
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.rootLayout);
            this.Font = new System.Drawing.Font("微软雅黑", 9.75F);
            this.Name = "WelcomeView";
            this.Size = new System.Drawing.Size(600, 400);
            this.rootLayout.ResumeLayout(false);
            this.rootLayout.PerformLayout();
            this.infoCard.ResumeLayout(false);
            this.infoLayout.ResumeLayout(false);
            this.cellDir.ResumeLayout(false);
            this.cellDir.PerformLayout();
            this.cellSpace.ResumeLayout(false);
            this.cellSpace.PerformLayout();
            this.cellVersion.ResumeLayout(false);
            this.cellVersion.PerformLayout();
            this.hintPanel.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel rootLayout;
        private AntdUI.Label lblTitle;
        private AntdUI.Label lblBody;
        private AntdUI.Panel infoCard;
        private System.Windows.Forms.TableLayoutPanel infoLayout;
        private AntdUI.Panel boxDir;
        private AntdUI.Panel boxSpace;
        private AntdUI.Panel boxVersion;
        private AntdUI.Label iconDir;
        private System.Windows.Forms.TableLayoutPanel cellDir;
        private AntdUI.Label lblDirCaption;
        private AntdUI.Label lblDirValue;
        private AntdUI.Label iconSpace;
        private System.Windows.Forms.TableLayoutPanel cellSpace;
        private AntdUI.Label lblSpaceCaption;
        private AntdUI.Label lblSpaceValue;
        private AntdUI.Label iconVersion;
        private System.Windows.Forms.TableLayoutPanel cellVersion;
        private AntdUI.Label lblVersionCaption;
        private AntdUI.Label lblVersionValue;
        private AntdUI.Panel hintPanel;
        private AntdUI.Label lblHint;
    }
}
