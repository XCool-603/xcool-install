namespace Installer.UI.Views
{
    partial class TaskCardView
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
            this.cardPanel = new AntdUI.Panel();
            this.layout = new System.Windows.Forms.TableLayoutPanel();
            this.chkTask = new AntdUI.Checkbox();
            this.lblIcon = new AntdUI.Label();
            this.cell = new System.Windows.Forms.TableLayoutPanel();
            this.lblTitle = new AntdUI.Label();
            this.lblDesc = new AntdUI.Label();
            this.cardPanel.SuspendLayout();
            this.layout.SuspendLayout();
            this.cell.SuspendLayout();
            this.SuspendLayout();
            //
            // cardPanel
            //
            this.cardPanel.Controls.Add(this.layout);
            this.cardPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cardPanel.Location = new System.Drawing.Point(0, 0);
            this.cardPanel.Margin = new System.Windows.Forms.Padding(0);
            this.cardPanel.Name = "cardPanel";
            this.cardPanel.Padding = new System.Windows.Forms.Padding(14, 8, 14, 8);
            this.cardPanel.Radius = 12;
            this.cardPanel.Size = new System.Drawing.Size(280, 76);
            this.cardPanel.TabIndex = 0;
            //
            // layout —— 复选框 / 图标 / 文案
            //
            this.layout.ColumnCount = 3;
            this.layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.layout.Controls.Add(this.chkTask, 0, 0);
            this.layout.Controls.Add(this.lblIcon, 1, 0);
            this.layout.Controls.Add(this.cell, 2, 0);
            this.layout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layout.Location = new System.Drawing.Point(14, 12);
            this.layout.Margin = new System.Windows.Forms.Padding(0);
            this.layout.Name = "layout";
            this.layout.RowCount = 1;
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.layout.Size = new System.Drawing.Size(252, 52);
            this.layout.TabIndex = 0;
            //
            // chkTask
            //
            this.chkTask.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.chkTask.AutoSize = true;
            this.chkTask.Location = new System.Drawing.Point(0, 12);
            this.chkTask.Margin = new System.Windows.Forms.Padding(0);
            this.chkTask.Name = "chkTask";
            this.chkTask.Size = new System.Drawing.Size(26, 26);
            this.chkTask.TabIndex = 0;
            this.chkTask.Text = "";
            //
            // lblIcon
            //
            this.lblIcon.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblIcon.AutoSize = false;
            this.lblIcon.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.lblIcon.IconGap = 0;
            this.lblIcon.Location = new System.Drawing.Point(34, 14);
            this.lblIcon.Margin = new System.Windows.Forms.Padding(0);
            this.lblIcon.Name = "lblIcon";
            this.lblIcon.Size = new System.Drawing.Size(24, 24);
            this.lblIcon.TabIndex = 1;
            this.lblIcon.Text = " ";
            //
            // cell —— 标题 + 描述
            //
            this.cell.ColumnCount = 1;
            this.cell.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.cell.Controls.Add(this.lblTitle, 0, 0);
            this.cell.Controls.Add(this.lblDesc, 0, 1);
            this.cell.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cell.Location = new System.Drawing.Point(62, 0);
            this.cell.Margin = new System.Windows.Forms.Padding(0);
            this.cell.Name = "cell";
            this.cell.RowCount = 2;
            this.cell.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.cell.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.cell.Size = new System.Drawing.Size(190, 52);
            this.cell.TabIndex = 2;
            //
            // lblTitle
            //
            this.lblTitle.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(0, 2);
            this.lblTitle.Margin = new System.Windows.Forms.Padding(0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(150, 21);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "任务标题";
            //
            // lblDesc
            //
            this.lblDesc.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblDesc.AutoSize = true;
            this.lblDesc.Font = new System.Drawing.Font("微软雅黑", 11F);
            this.lblDesc.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(163)))), ((int)(((byte)(175)))));
            this.lblDesc.Location = new System.Drawing.Point(0, 28);
            this.lblDesc.Margin = new System.Windows.Forms.Padding(0);
            this.lblDesc.Name = "lblDesc";
            this.lblDesc.Size = new System.Drawing.Size(150, 19);
            this.lblDesc.TabIndex = 1;
            this.lblDesc.Text = "任务描述";
            //
            // TaskCardView
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.cardPanel);
            this.Font = new System.Drawing.Font("微软雅黑", 12F);
            this.Name = "TaskCardView";
            this.Size = new System.Drawing.Size(280, 76);
            this.cardPanel.ResumeLayout(false);
            this.layout.ResumeLayout(false);
            this.cell.ResumeLayout(false);
            this.cell.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        private AntdUI.Panel cardPanel;
        private System.Windows.Forms.TableLayoutPanel layout;
        private AntdUI.Checkbox chkTask;
        private AntdUI.Label lblIcon;
        private System.Windows.Forms.TableLayoutPanel cell;
        private AntdUI.Label lblTitle;
        private AntdUI.Label lblDesc;
    }
}
