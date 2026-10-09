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
        private void InitializeComponent()
        {
            this.rootLayout = new System.Windows.Forms.TableLayoutPanel();
            this.messageRow = new System.Windows.Forms.TableLayoutPanel();
            this.badgePanel = new AntdUI.Panel();
            this.lblBadge = new AntdUI.Label();
            this.cell = new System.Windows.Forms.TableLayoutPanel();
            this.lblTitle = new AntdUI.Label();
            this.lblBody = new AntdUI.Label();
            this.chipFlow = new System.Windows.Forms.FlowLayoutPanel();
            this.chip1 = new AntdUI.Label();
            this.chip2 = new AntdUI.Label();
            this.chip3 = new AntdUI.Label();
            this.runPanel = new AntdUI.Panel();
            this.runLayout = new System.Windows.Forms.TableLayoutPanel();
            this.chkRun = new AntdUI.Checkbox();
            this.lblRunHint = new AntdUI.Label();
            this.lblUninstallHint = new AntdUI.Label();
            this.rootLayout.SuspendLayout();
            this.messageRow.SuspendLayout();
            this.badgePanel.SuspendLayout();
            this.cell.SuspendLayout();
            this.chipFlow.SuspendLayout();
            this.runPanel.SuspendLayout();
            this.runLayout.SuspendLayout();
            this.SuspendLayout();
            //
            // rootLayout
            //
            this.rootLayout.ColumnCount = 1;
            this.rootLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rootLayout.Controls.Add(this.messageRow, 0, 0);
            this.rootLayout.Controls.Add(this.chipFlow, 0, 1);
            this.rootLayout.Controls.Add(this.runPanel, 0, 2);
            this.rootLayout.Controls.Add(this.lblUninstallHint, 0, 3);
            this.rootLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rootLayout.Location = new System.Drawing.Point(0, 0);
            this.rootLayout.Name = "rootLayout";
            this.rootLayout.RowCount = 5;
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 86F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 70F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rootLayout.Size = new System.Drawing.Size(600, 400);
            this.rootLayout.TabIndex = 0;
            //
            // messageRow
            //
            this.messageRow.ColumnCount = 2;
            this.messageRow.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.messageRow.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.messageRow.Controls.Add(this.badgePanel, 0, 0);
            this.messageRow.Controls.Add(this.cell, 1, 0);
            this.messageRow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.messageRow.Location = new System.Drawing.Point(0, 0);
            this.messageRow.Margin = new System.Windows.Forms.Padding(0);
            this.messageRow.Name = "messageRow";
            this.messageRow.RowCount = 1;
            this.messageRow.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.messageRow.Size = new System.Drawing.Size(600, 86);
            this.messageRow.TabIndex = 0;
            //
            // badgePanel
            //
            this.badgePanel.Controls.Add(this.lblBadge);
            this.badgePanel.Location = new System.Drawing.Point(0, 8);
            this.badgePanel.Margin = new System.Windows.Forms.Padding(0);
            this.badgePanel.Name = "badgePanel";
            this.badgePanel.Radius = 32;
            this.badgePanel.Size = new System.Drawing.Size(64, 64);
            this.badgePanel.TabIndex = 0;
            //
            // lblBadge
            //
            this.lblBadge.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblBadge.AutoSize = false;
            this.lblBadge.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.lblBadge.IconGap = 0;
            this.lblBadge.Location = new System.Drawing.Point(18, 18);
            this.lblBadge.Margin = new System.Windows.Forms.Padding(0);
            this.lblBadge.Name = "lblBadge";
            this.lblBadge.PrefixSvg = "<svg viewBox=\"0 0 24 24\"><path d=\"M5 12.6l4.6 4.6L19 7.4\" fill=\"none\" stroke=\"#FFFFFF\" stroke-width=\"3\" stroke-linecap=\"round\" stroke-linejoin=\"round\"/></svg>";
            this.lblBadge.Size = new System.Drawing.Size(28, 28);
            this.lblBadge.TabIndex = 0;
            this.lblBadge.Text = " ";
            //
            // cell
            //
            this.cell.ColumnCount = 1;
            this.cell.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.cell.Controls.Add(this.lblTitle, 0, 0);
            this.cell.Controls.Add(this.lblBody, 0, 1);
            this.cell.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cell.Location = new System.Drawing.Point(80, 0);
            this.cell.Margin = new System.Windows.Forms.Padding(0);
            this.cell.Name = "cell";
            this.cell.RowCount = 2;
            this.cell.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 44F));
            this.cell.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.cell.Size = new System.Drawing.Size(520, 86);
            this.cell.TabIndex = 1;
            //
            // lblTitle
            //
            this.lblTitle.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("微软雅黑", 17F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(0, 7);
            this.lblTitle.Margin = new System.Windows.Forms.Padding(0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(300, 30);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "安装完成";
            //
            // lblBody
            //
            this.lblBody.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblBody.AutoSize = true;
            this.lblBody.Font = new System.Drawing.Font("微软雅黑", 12F);
            this.lblBody.Location = new System.Drawing.Point(0, 48);
            this.lblBody.Margin = new System.Windows.Forms.Padding(0);
            this.lblBody.Name = "lblBody";
            this.lblBody.Size = new System.Drawing.Size(400, 21);
            this.lblBody.TabIndex = 1;
            this.lblBody.Text = "";
            //
            // chipFlow
            //
            this.chipFlow.Controls.Add(this.chip1);
            this.chipFlow.Controls.Add(this.chip2);
            this.chipFlow.Controls.Add(this.chip3);
            this.chipFlow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chipFlow.Location = new System.Drawing.Point(0, 86);
            this.chipFlow.Margin = new System.Windows.Forms.Padding(0);
            this.chipFlow.Name = "chipFlow";
            this.chipFlow.Padding = new System.Windows.Forms.Padding(80, 8, 0, 0);
            this.chipFlow.Size = new System.Drawing.Size(600, 40);
            this.chipFlow.TabIndex = 1;
            this.chipFlow.WrapContents = false;
            //
            // chip1
            //
            this.chip1.AutoSize = true;
            this.chip1.Font = new System.Drawing.Font("微软雅黑", 11F);
            this.chip1.IconGap = 4;
            this.chip1.Location = new System.Drawing.Point(80, 8);
            this.chip1.Margin = new System.Windows.Forms.Padding(0, 0, 8, 0);
            this.chip1.Name = "chip1";
            this.chip1.Padding = new System.Windows.Forms.Padding(8, 3, 10, 3);
            this.chip1.PrefixSvg = "<svg viewBox=\"0 0 16 16\"><path d=\"M3.6 8.4l3 3 5.8-6\" fill=\"none\" stroke=\"#52C41A\" stroke-width=\"1.9\" stroke-linecap=\"round\" stroke-linejoin=\"round\"/></svg>";
            this.chip1.Size = new System.Drawing.Size(110, 25);
            this.chip1.TabIndex = 0;
            this.chip1.Text = "桌面快捷方式";
            //
            // chip2
            //
            this.chip2.AutoSize = true;
            this.chip2.Font = new System.Drawing.Font("微软雅黑", 11F);
            this.chip2.IconGap = 4;
            this.chip2.Location = new System.Drawing.Point(198, 8);
            this.chip2.Margin = new System.Windows.Forms.Padding(0, 0, 8, 0);
            this.chip2.Name = "chip2";
            this.chip2.Padding = new System.Windows.Forms.Padding(8, 3, 10, 3);
            this.chip2.PrefixSvg = "<svg viewBox=\"0 0 16 16\"><path d=\"M3.6 8.4l3 3 5.8-6\" fill=\"none\" stroke=\"#52C41A\" stroke-width=\"1.9\" stroke-linecap=\"round\" stroke-linejoin=\"round\"/></svg>";
            this.chip2.Size = new System.Drawing.Size(90, 25);
            this.chip2.TabIndex = 1;
            this.chip2.Text = "开始菜单";
            //
            // chip3
            //
            this.chip3.AutoSize = true;
            this.chip3.Font = new System.Drawing.Font("微软雅黑", 11F);
            this.chip3.IconGap = 4;
            this.chip3.Location = new System.Drawing.Point(296, 8);
            this.chip3.Margin = new System.Windows.Forms.Padding(0, 0, 8, 0);
            this.chip3.Name = "chip3";
            this.chip3.Padding = new System.Windows.Forms.Padding(8, 3, 10, 3);
            this.chip3.PrefixSvg = "<svg viewBox=\"0 0 16 16\"><path d=\"M3.6 8.4l3 3 5.8-6\" fill=\"none\" stroke=\"#52C41A\" stroke-width=\"1.9\" stroke-linecap=\"round\" stroke-linejoin=\"round\"/></svg>";
            this.chip3.Size = new System.Drawing.Size(120, 25);
            this.chip3.TabIndex = 2;
            this.chip3.Text = "安装后自动运行";
            //
            // runPanel
            //
            this.runPanel.Controls.Add(this.runLayout);
            this.runPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.runPanel.Location = new System.Drawing.Point(0, 126);
            this.runPanel.Margin = new System.Windows.Forms.Padding(0);
            this.runPanel.Name = "runPanel";
            this.runPanel.Padding = new System.Windows.Forms.Padding(16, 10, 16, 10);
            this.runPanel.Radius = 12;
            this.runPanel.Size = new System.Drawing.Size(600, 70);
            this.runPanel.TabIndex = 2;
            //
            // runLayout
            //
            this.runLayout.ColumnCount = 1;
            this.runLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.runLayout.Controls.Add(this.chkRun, 0, 0);
            this.runLayout.Controls.Add(this.lblRunHint, 0, 1);
            this.runLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.runLayout.Location = new System.Drawing.Point(16, 10);
            this.runLayout.Margin = new System.Windows.Forms.Padding(0);
            this.runLayout.Name = "runLayout";
            this.runLayout.RowCount = 2;
            this.runLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.runLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.runLayout.Size = new System.Drawing.Size(568, 50);
            this.runLayout.TabIndex = 0;
            //
            // chkRun
            //
            this.chkRun.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.chkRun.AutoSize = true;
            this.chkRun.Font = new System.Drawing.Font("微软雅黑", 12F);
            this.chkRun.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(41)))), ((int)(((byte)(55)))));
            this.chkRun.Location = new System.Drawing.Point(0, 2);
            this.chkRun.Margin = new System.Windows.Forms.Padding(0);
            this.chkRun.Name = "chkRun";
            this.chkRun.Size = new System.Drawing.Size(260, 25);
            this.chkRun.TabIndex = 0;
            this.chkRun.Text = "立即运行";
            //
            // lblRunHint
            //
            this.lblRunHint.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblRunHint.AutoSize = true;
            this.lblRunHint.Font = new System.Drawing.Font("微软雅黑", 11F);
            this.lblRunHint.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(163)))), ((int)(((byte)(175)))));
            this.lblRunHint.Location = new System.Drawing.Point(0, 32);
            this.lblRunHint.Margin = new System.Windows.Forms.Padding(0);
            this.lblRunHint.Name = "lblRunHint";
            this.lblRunHint.Size = new System.Drawing.Size(400, 19);
            this.lblRunHint.TabIndex = 1;
            this.lblRunHint.Text = "";
            //
            // lblUninstallHint
            //
            this.lblUninstallHint.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblUninstallHint.AutoSize = true;
            this.lblUninstallHint.Font = new System.Drawing.Font("微软雅黑", 11F);
            this.lblUninstallHint.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(163)))), ((int)(((byte)(175)))));
            this.lblUninstallHint.Location = new System.Drawing.Point(0, 196);
            this.lblUninstallHint.Margin = new System.Windows.Forms.Padding(0, 6, 0, 0);
            this.lblUninstallHint.Name = "lblUninstallHint";
            this.lblUninstallHint.Size = new System.Drawing.Size(400, 19);
            this.lblUninstallHint.TabIndex = 3;
            this.lblUninstallHint.Text = "";
            //
            // FinishView
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.rootLayout);
            this.Font = new System.Drawing.Font("微软雅黑", 12F);
            this.Name = "FinishView";
            this.Size = new System.Drawing.Size(600, 400);
            this.rootLayout.ResumeLayout(false);
            this.rootLayout.PerformLayout();
            this.messageRow.ResumeLayout(false);
            this.badgePanel.ResumeLayout(false);
            this.cell.ResumeLayout(false);
            this.cell.PerformLayout();
            this.chipFlow.ResumeLayout(false);
            this.chipFlow.PerformLayout();
            this.runPanel.ResumeLayout(false);
            this.runLayout.ResumeLayout(false);
            this.runLayout.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel rootLayout;
        private System.Windows.Forms.TableLayoutPanel messageRow;
        private AntdUI.Panel badgePanel;
        private AntdUI.Label lblBadge;
        private System.Windows.Forms.TableLayoutPanel cell;
        private AntdUI.Label lblTitle;
        private AntdUI.Label lblBody;
        private System.Windows.Forms.FlowLayoutPanel chipFlow;
        private AntdUI.Label chip1;
        private AntdUI.Label chip2;
        private AntdUI.Label chip3;
        private AntdUI.Panel runPanel;
        private System.Windows.Forms.TableLayoutPanel runLayout;
        private AntdUI.Checkbox chkRun;
        private AntdUI.Label lblRunHint;
        private AntdUI.Label lblUninstallHint;
    }
}
