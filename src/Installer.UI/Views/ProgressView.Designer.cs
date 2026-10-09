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

        #region 组件设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.rootLayout = new System.Windows.Forms.TableLayoutPanel();
            this.lblTitle = new AntdUI.Label();
            this.progress = new AntdUI.Progress();
            this.lblStep = new AntdUI.Label();
            this.txtLog = new AntdUI.Input();
            this.rootLayout.SuspendLayout();
            this.SuspendLayout();
            //
            // rootLayout
            //
            this.rootLayout.ColumnCount = 1;
            this.rootLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rootLayout.Controls.Add(this.lblTitle, 0, 0);
            this.rootLayout.Controls.Add(this.progress, 0, 1);
            this.rootLayout.Controls.Add(this.lblStep, 0, 2);
            this.rootLayout.Controls.Add(this.txtLog, 0, 3);
            this.rootLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rootLayout.Location = new System.Drawing.Point(0, 0);
            this.rootLayout.Name = "rootLayout";
            this.rootLayout.Padding = new System.Windows.Forms.Padding(24, 16, 24, 16);
            this.rootLayout.RowCount = 4;
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 48F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rootLayout.Size = new System.Drawing.Size(600, 400);
            this.rootLayout.TabIndex = 0;
            //
            // lblTitle
            //
            this.lblTitle.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("微软雅黑", 14F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(27, 20);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(100, 25);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "正在安装";
            //
            // progress
            //
            this.progress.Dock = System.Windows.Forms.DockStyle.Fill;
            this.progress.Location = new System.Drawing.Point(27, 59);
            this.progress.Name = "progress";
            this.progress.Shape = AntdUI.TShapeProgress.Default;
            this.progress.Size = new System.Drawing.Size(546, 42);
            this.progress.TabIndex = 1;
            this.progress.Text = "0%";
            this.progress.Value = 0F;
            //
            // lblStep
            //
            this.lblStep.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblStep.AutoSize = true;
            this.lblStep.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.lblStep.Location = new System.Drawing.Point(27, 115);
            this.lblStep.Name = "lblStep";
            this.lblStep.Size = new System.Drawing.Size(200, 17);
            this.lblStep.TabIndex = 2;
            this.lblStep.Text = "正在准备…";
            //
            // txtLog
            //
            this.txtLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtLog.Font = new System.Drawing.Font("Consolas", 9F);
            this.txtLog.Location = new System.Drawing.Point(27, 139);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.Size = new System.Drawing.Size(546, 242);
            this.txtLog.TabIndex = 3;
            //
            // ProgressView
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.rootLayout);
            this.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.Name = "ProgressView";
            this.Size = new System.Drawing.Size(600, 400);
            this.rootLayout.ResumeLayout(false);
            this.rootLayout.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel rootLayout;
        private AntdUI.Label lblTitle;
        private AntdUI.Progress progress;
        private AntdUI.Label lblStep;
        private AntdUI.Input txtLog;
    }
}
