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

        #region 组件设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.rootLayout = new System.Windows.Forms.TableLayoutPanel();
            this.icon = new AntdUI.Label();
            this.lblTitle = new AntdUI.Label();
            this.lblBody = new AntdUI.Label();
            this.chkRun = new AntdUI.Checkbox();
            this.rootLayout.SuspendLayout();
            this.SuspendLayout();
            //
            // rootLayout
            //
            this.rootLayout.ColumnCount = 2;
            this.rootLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 72F));
            this.rootLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rootLayout.Controls.Add(this.icon, 0, 0);
            this.rootLayout.Controls.Add(this.lblTitle, 1, 0);
            this.rootLayout.Controls.Add(this.lblBody, 1, 1);
            this.rootLayout.Controls.Add(this.chkRun, 1, 2);
            this.rootLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rootLayout.Location = new System.Drawing.Point(0, 0);
            this.rootLayout.Name = "rootLayout";
            this.rootLayout.Padding = new System.Windows.Forms.Padding(24, 24, 24, 16);
            this.rootLayout.RowCount = 3;
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 56F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            this.rootLayout.Size = new System.Drawing.Size(600, 400);
            this.rootLayout.TabIndex = 0;
            //
            // icon
            //
            this.icon.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.icon.AutoSize = true;
            this.icon.Font = new System.Drawing.Font("微软雅黑", 28F, System.Drawing.FontStyle.Bold);
            this.icon.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(82)))), ((int)(((byte)(196)))), ((int)(((byte)(26)))));
            this.icon.Location = new System.Drawing.Point(27, 32);
            this.icon.Name = "icon";
            this.icon.Size = new System.Drawing.Size(48, 50);
            this.icon.TabIndex = 0;
            this.icon.Text = "✔";
            //
            // lblTitle
            //
            this.lblTitle.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("微软雅黑", 16F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(99, 38);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(150, 30);
            this.lblTitle.TabIndex = 1;
            this.lblTitle.Text = "安装完成";
            //
            // lblBody
            //
            this.lblBody.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblBody.Font = new System.Drawing.Font("微软雅黑", 10F);
            this.lblBody.Location = new System.Drawing.Point(99, 83);
            this.lblBody.Name = "lblBody";
            this.lblBody.Size = new System.Drawing.Size(474, 261);
            this.lblBody.TabIndex = 2;
            this.lblBody.Text = "已安装完成。";
            //
            // chkRun
            //
            this.chkRun.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.chkRun.AutoSize = true;
            this.chkRun.Checked = true;
            this.chkRun.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.chkRun.Location = new System.Drawing.Point(99, 350);
            this.chkRun.Name = "chkRun";
            this.chkRun.Size = new System.Drawing.Size(200, 23);
            this.chkRun.TabIndex = 3;
            this.chkRun.Text = "立即运行";
            //
            // FinishView
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.rootLayout);
            this.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.Name = "FinishView";
            this.Size = new System.Drawing.Size(600, 400);
            this.rootLayout.ResumeLayout(false);
            this.rootLayout.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel rootLayout;
        private AntdUI.Label icon;
        private AntdUI.Label lblTitle;
        private AntdUI.Label lblBody;
        private AntdUI.Checkbox chkRun;
    }
}
