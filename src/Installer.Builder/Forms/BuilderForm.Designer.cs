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
            this.cboLang = new AntdUI.Select();
            this.bodyLayout = new System.Windows.Forms.TableLayoutPanel();
            this.lblStep1 = new AntdUI.Label();
            this.folderStep = new Installer.Builder.Views.FolderStepView();
            this.lblStep2 = new AntdUI.Label();
            this.infoStep = new Installer.Builder.Views.InfoStepView();
            this.lblStep3 = new AntdUI.Label();
            this.shortcutStep = new Installer.Builder.Views.ShortcutStepView();
            this.btnAdvanced = new AntdUI.Button();
            this.outputBar = new Installer.Builder.Views.OutputBar();
            this.lblStatus = new AntdUI.Label();
            this.rootLayout.SuspendLayout();
            this.headerLayout.SuspendLayout();
            this.bodyLayout.SuspendLayout();
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
            this.rootLayout.Padding = new System.Windows.Forms.Padding(20, 12, 20, 0);
            this.rootLayout.RowCount = 4;
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 52F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 96F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.rootLayout.Size = new System.Drawing.Size(820, 648);
            this.rootLayout.TabIndex = 0;
            //
            // headerLayout
            //
            this.headerLayout.ColumnCount = 2;
            this.headerLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.headerLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 150F));
            this.headerLayout.Controls.Add(this.lblTitle, 0, 0);
            this.headerLayout.Controls.Add(this.cboLang, 1, 0);
            this.headerLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.headerLayout.Location = new System.Drawing.Point(20, 12);
            this.headerLayout.Margin = new System.Windows.Forms.Padding(0);
            this.headerLayout.Name = "headerLayout";
            this.headerLayout.RowCount = 1;
            this.headerLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.headerLayout.Size = new System.Drawing.Size(780, 52);
            this.headerLayout.TabIndex = 0;
            //
            // lblTitle
            //
            this.lblTitle.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("微软雅黑", 14F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(0, 11);
            this.lblTitle.Margin = new System.Windows.Forms.Padding(0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(160, 25);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "安装包制作助手";
            //
            // cboLang
            //
            this.cboLang.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.cboLang.Location = new System.Drawing.Point(640, 9);
            this.cboLang.Name = "cboLang";
            this.cboLang.Size = new System.Drawing.Size(140, 34);
            this.cboLang.TabIndex = 1;
            //
            // bodyLayout
            //
            this.bodyLayout.ColumnCount = 1;
            this.bodyLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.bodyLayout.Controls.Add(this.lblStep1, 0, 0);
            this.bodyLayout.Controls.Add(this.folderStep, 0, 1);
            this.bodyLayout.Controls.Add(this.lblStep2, 0, 2);
            this.bodyLayout.Controls.Add(this.infoStep, 0, 3);
            this.bodyLayout.Controls.Add(this.lblStep3, 0, 4);
            this.bodyLayout.Controls.Add(this.shortcutStep, 0, 5);
            this.bodyLayout.Controls.Add(this.btnAdvanced, 0, 6);
            this.bodyLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bodyLayout.Location = new System.Drawing.Point(20, 64);
            this.bodyLayout.Margin = new System.Windows.Forms.Padding(0);
            this.bodyLayout.Name = "bodyLayout";
            this.bodyLayout.RowCount = 8;
            this.bodyLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.bodyLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 140F));
            this.bodyLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.bodyLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 104F));
            this.bodyLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.bodyLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            this.bodyLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 46F));
            this.bodyLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.bodyLayout.Size = new System.Drawing.Size(780, 404);
            this.bodyLayout.TabIndex = 1;
            //
            // lblStep1
            //
            this.lblStep1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblStep1.AutoSize = true;
            this.lblStep1.Font = new System.Drawing.Font("微软雅黑", 9.5F, System.Drawing.FontStyle.Bold);
            this.lblStep1.Location = new System.Drawing.Point(0, 7);
            this.lblStep1.Margin = new System.Windows.Forms.Padding(0);
            this.lblStep1.Name = "lblStep1";
            this.lblStep1.Size = new System.Drawing.Size(200, 18);
            this.lblStep1.TabIndex = 0;
            this.lblStep1.Text = "第 1 步 · 选择要打包的文件夹";
            //
            // folderStep
            //
            this.folderStep.Dock = System.Windows.Forms.DockStyle.Fill;
            this.folderStep.Location = new System.Drawing.Point(0, 32);
            this.folderStep.Margin = new System.Windows.Forms.Padding(0);
            this.folderStep.Name = "folderStep";
            this.folderStep.Size = new System.Drawing.Size(780, 140);
            this.folderStep.TabIndex = 1;
            //
            // lblStep2
            //
            this.lblStep2.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblStep2.AutoSize = true;
            this.lblStep2.Font = new System.Drawing.Font("微软雅黑", 9.5F, System.Drawing.FontStyle.Bold);
            this.lblStep2.Location = new System.Drawing.Point(0, 179);
            this.lblStep2.Margin = new System.Windows.Forms.Padding(0);
            this.lblStep2.Name = "lblStep2";
            this.lblStep2.Size = new System.Drawing.Size(160, 18);
            this.lblStep2.TabIndex = 2;
            this.lblStep2.Text = "第 2 步 · 填写基本信息";
            //
            // infoStep
            //
            this.infoStep.Dock = System.Windows.Forms.DockStyle.Fill;
            this.infoStep.Location = new System.Drawing.Point(0, 204);
            this.infoStep.Margin = new System.Windows.Forms.Padding(0);
            this.infoStep.Name = "infoStep";
            this.infoStep.Size = new System.Drawing.Size(780, 104);
            this.infoStep.TabIndex = 3;
            //
            // lblStep3
            //
            this.lblStep3.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblStep3.AutoSize = true;
            this.lblStep3.Font = new System.Drawing.Font("微软雅黑", 9.5F, System.Drawing.FontStyle.Bold);
            this.lblStep3.Location = new System.Drawing.Point(0, 315);
            this.lblStep3.Margin = new System.Windows.Forms.Padding(0);
            this.lblStep3.Name = "lblStep3";
            this.lblStep3.Size = new System.Drawing.Size(120, 18);
            this.lblStep3.TabIndex = 4;
            this.lblStep3.Text = "第 3 步 · 快捷方式";
            //
            // shortcutStep
            //
            this.shortcutStep.Dock = System.Windows.Forms.DockStyle.Fill;
            this.shortcutStep.Location = new System.Drawing.Point(0, 340);
            this.shortcutStep.Margin = new System.Windows.Forms.Padding(0);
            this.shortcutStep.Name = "shortcutStep";
            this.shortcutStep.Size = new System.Drawing.Size(780, 38);
            this.shortcutStep.TabIndex = 5;
            //
            // btnAdvanced
            //
            this.btnAdvanced.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnAdvanced.Location = new System.Drawing.Point(0, 386);
            this.btnAdvanced.Margin = new System.Windows.Forms.Padding(0);
            this.btnAdvanced.Name = "btnAdvanced";
            this.btnAdvanced.Radius = 6;
            this.btnAdvanced.Size = new System.Drawing.Size(140, 36);
            this.btnAdvanced.TabIndex = 6;
            this.btnAdvanced.Text = "高级选项…";
            //
            // outputBar
            //
            this.outputBar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.outputBar.Location = new System.Drawing.Point(20, 468);
            this.outputBar.Margin = new System.Windows.Forms.Padding(0);
            this.outputBar.Name = "outputBar";
            this.outputBar.Size = new System.Drawing.Size(780, 96);
            this.outputBar.TabIndex = 2;
            //
            // lblStatus
            //
            this.lblStatus.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(20, 576);
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
            this.ClientSize = new System.Drawing.Size(820, 648);
            this.Controls.Add(this.rootLayout);
            this.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.MinimumSize = new System.Drawing.Size(720, 520);
            this.Name = "BuilderForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "安装包制作助手";
            this.rootLayout.ResumeLayout(false);
            this.rootLayout.PerformLayout();
            this.headerLayout.ResumeLayout(false);
            this.headerLayout.PerformLayout();
            this.bodyLayout.ResumeLayout(false);
            this.bodyLayout.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel rootLayout;
        private System.Windows.Forms.TableLayoutPanel headerLayout;
        private AntdUI.Label lblTitle;
        private AntdUI.Select cboLang;
        private System.Windows.Forms.TableLayoutPanel bodyLayout;
        private AntdUI.Label lblStep1;
        private Installer.Builder.Views.FolderStepView folderStep;
        private AntdUI.Label lblStep2;
        private Installer.Builder.Views.InfoStepView infoStep;
        private AntdUI.Label lblStep3;
        private Installer.Builder.Views.ShortcutStepView shortcutStep;
        private AntdUI.Button btnAdvanced;
        private Installer.Builder.Views.OutputBar outputBar;
        private AntdUI.Label lblStatus;
    }
}
