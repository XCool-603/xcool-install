namespace Installer.Builder.Forms
{
    partial class BuilderForm
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

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.rootLayout = new System.Windows.Forms.TableLayoutPanel();
            this.headerLayout = new System.Windows.Forms.TableLayoutPanel();
            this.lblTitle = new AntdUI.Label();
            this.headerRight = new System.Windows.Forms.FlowLayoutPanel();
            this.cboLang = new AntdUI.Select();
            this.btnSave = new AntdUI.Button();
            this.btnOpen = new AntdUI.Button();
            this.bodyLayout = new System.Windows.Forms.TableLayoutPanel();
            this.card1 = new AntdUI.Panel();
            this.folderStep = new Installer.Builder.Views.FolderStepView();
            this.card2 = new AntdUI.Panel();
            this.infoStep = new Installer.Builder.Views.InfoStepView();
            this.card3 = new AntdUI.Panel();
            this.shortcutStep = new Installer.Builder.Views.ShortcutStepView();
            this.btnAdvanced = new AntdUI.Button();
            this.outputBar = new Installer.Builder.Views.OutputBar();
            this.lblStatus = new AntdUI.Label();
            this.rootLayout.SuspendLayout();
            this.headerLayout.SuspendLayout();
            this.headerRight.SuspendLayout();
            this.bodyLayout.SuspendLayout();
            this.card1.SuspendLayout();
            this.card2.SuspendLayout();
            this.card3.SuspendLayout();
            this.SuspendLayout();
            //
            // rootLayout
            //
            this.rootLayout.ColumnCount = 1;
            this.rootLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rootLayout.Controls.Add(this.headerLayout, 0, 0);
            this.rootLayout.Controls.Add(this.bodyLayout, 0, 1);
            this.rootLayout.Controls.Add(this.outputBar, 0, 2);
            this.rootLayout.Controls.Add(this.lblStatus, 0, 3);
            this.rootLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rootLayout.Location = new System.Drawing.Point(0, 0);
            this.rootLayout.Name = "rootLayout";
            this.rootLayout.Padding = new System.Windows.Forms.Padding(22, 14, 22, 0);
            this.rootLayout.RowCount = 4;
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 58F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 94F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.rootLayout.Size = new System.Drawing.Size(884, 812);
            this.rootLayout.TabIndex = 0;
            //
            // headerLayout
            //
            this.headerLayout.ColumnCount = 2;
            this.headerLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.headerLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 330F));
            this.headerLayout.Controls.Add(this.lblTitle, 0, 0);
            this.headerLayout.Controls.Add(this.headerRight, 1, 0);
            this.headerLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.headerLayout.Location = new System.Drawing.Point(22, 14);
            this.headerLayout.Margin = new System.Windows.Forms.Padding(0);
            this.headerLayout.Name = "headerLayout";
            this.headerLayout.RowCount = 1;
            this.headerLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.headerLayout.Size = new System.Drawing.Size(840, 58);
            this.headerLayout.TabIndex = 0;
            //
            // lblTitle
            //
            this.lblTitle.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(0, 13);
            this.lblTitle.Margin = new System.Windows.Forms.Padding(0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(180, 27);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "安装包制作助手";
            //
            // headerRight
            //
            this.headerRight.Controls.Add(this.btnSave);
            this.headerRight.Controls.Add(this.btnOpen);
            this.headerRight.Controls.Add(this.cboLang);
            this.headerRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.headerRight.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.headerRight.Location = new System.Drawing.Point(510, 0);
            this.headerRight.Margin = new System.Windows.Forms.Padding(0);
            this.headerRight.Name = "headerRight";
            this.headerRight.Size = new System.Drawing.Size(330, 58);
            this.headerRight.TabIndex = 1;
            //
            // cboLang
            //
            this.cboLang.Location = new System.Drawing.Point(200, 13);
            this.cboLang.Margin = new System.Windows.Forms.Padding(10, 13, 0, 0);
            this.cboLang.Name = "cboLang";
            this.cboLang.Size = new System.Drawing.Size(130, 34);
            this.cboLang.TabIndex = 0;
            //
            // btnSave
            //
            this.btnSave.Location = new System.Drawing.Point(102, 13);
            this.btnSave.Margin = new System.Windows.Forms.Padding(10, 13, 0, 0);
            this.btnSave.Name = "btnSave";
            this.btnSave.Radius = 6;
            this.btnSave.Size = new System.Drawing.Size(88, 34);
            this.btnSave.TabIndex = 1;
            this.btnSave.Text = "保存";
            //
            // btnOpen
            //
            this.btnOpen.Location = new System.Drawing.Point(4, 13);
            this.btnOpen.Margin = new System.Windows.Forms.Padding(10, 13, 0, 0);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Radius = 6;
            this.btnOpen.Size = new System.Drawing.Size(88, 34);
            this.btnOpen.TabIndex = 2;
            this.btnOpen.Text = "打开";
            //
            // bodyLayout
            //
            this.bodyLayout.ColumnCount = 1;
            this.bodyLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.bodyLayout.Controls.Add(this.card1, 0, 0);
            this.bodyLayout.Controls.Add(this.card2, 0, 1);
            this.bodyLayout.Controls.Add(this.card3, 0, 2);
            this.bodyLayout.Controls.Add(this.btnAdvanced, 0, 3);
            this.bodyLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bodyLayout.Location = new System.Drawing.Point(22, 72);
            this.bodyLayout.Margin = new System.Windows.Forms.Padding(0);
            this.bodyLayout.Name = "bodyLayout";
            this.bodyLayout.RowCount = 5;
            this.bodyLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 272F));
            this.bodyLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 166F));
            this.bodyLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 112F));
            this.bodyLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 48F));
            this.bodyLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.bodyLayout.Size = new System.Drawing.Size(840, 616);
            this.bodyLayout.TabIndex = 1;
            //
            // card1
            //
            this.card1.BackColor = System.Drawing.Color.White;
            this.card1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(228)))), ((int)(((byte)(232)))), ((int)(((byte)(240)))));
            this.card1.BorderWidth = 1F;
            this.card1.Controls.Add(this.folderStep);
            this.card1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.card1.Location = new System.Drawing.Point(0, 0);
            this.card1.Margin = new System.Windows.Forms.Padding(0, 0, 0, 10);
            this.card1.Name = "card1";
            this.card1.Padding = new System.Windows.Forms.Padding(14);
            this.card1.Radius = 10;
            this.card1.Size = new System.Drawing.Size(840, 262);
            this.card1.TabIndex = 0;
            //
            // folderStep
            //
            this.folderStep.Dock = System.Windows.Forms.DockStyle.Fill;
            this.folderStep.Location = new System.Drawing.Point(14, 14);
            this.folderStep.Margin = new System.Windows.Forms.Padding(0);
            this.folderStep.Name = "folderStep";
            this.folderStep.Size = new System.Drawing.Size(812, 234);
            this.folderStep.TabIndex = 0;
            //
            // card2
            //
            this.card2.BackColor = System.Drawing.Color.White;
            this.card2.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(228)))), ((int)(((byte)(232)))), ((int)(((byte)(240)))));
            this.card2.BorderWidth = 1F;
            this.card2.Controls.Add(this.infoStep);
            this.card2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.card2.Location = new System.Drawing.Point(0, 272);
            this.card2.Margin = new System.Windows.Forms.Padding(0, 0, 0, 10);
            this.card2.Name = "card2";
            this.card2.Padding = new System.Windows.Forms.Padding(14);
            this.card2.Radius = 10;
            this.card2.Size = new System.Drawing.Size(840, 156);
            this.card2.TabIndex = 1;
            //
            // infoStep
            //
            this.infoStep.Dock = System.Windows.Forms.DockStyle.Fill;
            this.infoStep.Location = new System.Drawing.Point(14, 14);
            this.infoStep.Margin = new System.Windows.Forms.Padding(0);
            this.infoStep.Name = "infoStep";
            this.infoStep.Size = new System.Drawing.Size(812, 128);
            this.infoStep.TabIndex = 0;
            //
            // card3
            //
            this.card3.BackColor = System.Drawing.Color.White;
            this.card3.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(228)))), ((int)(((byte)(232)))), ((int)(((byte)(240)))));
            this.card3.BorderWidth = 1F;
            this.card3.Controls.Add(this.shortcutStep);
            this.card3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.card3.Location = new System.Drawing.Point(0, 438);
            this.card3.Margin = new System.Windows.Forms.Padding(0, 0, 0, 10);
            this.card3.Name = "card3";
            this.card3.Padding = new System.Windows.Forms.Padding(14);
            this.card3.Radius = 10;
            this.card3.Size = new System.Drawing.Size(840, 102);
            this.card3.TabIndex = 2;
            //
            // shortcutStep
            //
            this.shortcutStep.Dock = System.Windows.Forms.DockStyle.Fill;
            this.shortcutStep.Location = new System.Drawing.Point(14, 14);
            this.shortcutStep.Margin = new System.Windows.Forms.Padding(0);
            this.shortcutStep.Name = "shortcutStep";
            this.shortcutStep.Size = new System.Drawing.Size(812, 74);
            this.shortcutStep.TabIndex = 0;
            //
            // btnAdvanced
            //
            this.btnAdvanced.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnAdvanced.Location = new System.Drawing.Point(0, 556);
            this.btnAdvanced.Margin = new System.Windows.Forms.Padding(0, 8, 0, 8);
            this.btnAdvanced.Name = "btnAdvanced";
            this.btnAdvanced.Radius = 6;
            this.btnAdvanced.Size = new System.Drawing.Size(140, 32);
            this.btnAdvanced.TabIndex = 3;
            this.btnAdvanced.Text = "高级选项…";
            //
            // outputBar
            //
            this.outputBar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.outputBar.Location = new System.Drawing.Point(22, 688);
            this.outputBar.Margin = new System.Windows.Forms.Padding(0);
            this.outputBar.Name = "outputBar";
            this.outputBar.Size = new System.Drawing.Size(840, 94);
            this.outputBar.TabIndex = 2;
            //
            // lblStatus
            //
            this.lblStatus.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(22, 782);
            this.lblStatus.Margin = new System.Windows.Forms.Padding(0);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(200, 17);
            this.lblStatus.TabIndex = 3;
            this.lblStatus.Text = "选一个文件夹就可以开始了";
            //
            // BuilderForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(247)))), ((int)(((byte)(250)))));
            this.ClientSize = new System.Drawing.Size(884, 812);
            this.Controls.Add(this.rootLayout);
            this.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.MinimumSize = new System.Drawing.Size(820, 780);
            this.Name = "BuilderForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "安装包制作助手";
            this.rootLayout.ResumeLayout(false);
            this.rootLayout.PerformLayout();
            this.headerLayout.ResumeLayout(false);
            this.headerLayout.PerformLayout();
            this.headerRight.ResumeLayout(false);
            this.bodyLayout.ResumeLayout(false);
            this.bodyLayout.PerformLayout();
            this.card1.ResumeLayout(false);
            this.card2.ResumeLayout(false);
            this.card3.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel rootLayout;
        private System.Windows.Forms.TableLayoutPanel headerLayout;
        private AntdUI.Label lblTitle;
        private System.Windows.Forms.FlowLayoutPanel headerRight;
        private AntdUI.Select cboLang;
        private AntdUI.Button btnSave;
        private AntdUI.Button btnOpen;
        private System.Windows.Forms.TableLayoutPanel bodyLayout;
        private AntdUI.Panel card1;
        private Installer.Builder.Views.FolderStepView folderStep;
        private AntdUI.Panel card2;
        private Installer.Builder.Views.InfoStepView infoStep;
        private AntdUI.Panel card3;
        private Installer.Builder.Views.ShortcutStepView shortcutStep;
        private AntdUI.Button btnAdvanced;
        private Installer.Builder.Views.OutputBar outputBar;
        private AntdUI.Label lblStatus;
    }
}
