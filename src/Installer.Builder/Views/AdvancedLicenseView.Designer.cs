namespace Installer.Builder.Views
{
    partial class AdvancedLicenseView
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
            this.chkRequired = new AntdUI.Checkbox();
            this.lblZh = new AntdUI.Label();
            this.txtZh = new AntdUI.Input();
            this.lblEn = new AntdUI.Label();
            this.txtEn = new AntdUI.Input();
            this.rootLayout.SuspendLayout();
            this.SuspendLayout();
            //
            // rootLayout
            //
            this.rootLayout.ColumnCount = 2;
            this.rootLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.rootLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rootLayout.Controls.Add(this.chkRequired, 0, 0);
            this.rootLayout.Controls.Add(this.lblZh, 0, 1);
            this.rootLayout.Controls.Add(this.txtZh, 1, 1);
            this.rootLayout.Controls.Add(this.lblEn, 0, 2);
            this.rootLayout.Controls.Add(this.txtEn, 1, 2);
            this.rootLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rootLayout.Location = new System.Drawing.Point(0, 0);
            this.rootLayout.Name = "rootLayout";
            this.rootLayout.Padding = new System.Windows.Forms.Padding(12);
            this.rootLayout.RowCount = 3;
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.rootLayout.SetColumnSpan(this.chkRequired, 2);
            this.rootLayout.Size = new System.Drawing.Size(640, 420);
            this.rootLayout.TabIndex = 0;
            //
            // chkRequired
            //
            this.chkRequired.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.chkRequired.AutoSize = true;
            this.chkRequired.Location = new System.Drawing.Point(15, 20);
            this.chkRequired.Name = "chkRequired";
            this.chkRequired.Size = new System.Drawing.Size(160, 23);
            this.chkRequired.TabIndex = 0;
            this.chkRequired.Text = "必须接受才能安装";
            //
            // lblZh
            //
            this.lblZh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
            this.lblZh.Margin = new System.Windows.Forms.Padding(0, 12, 12, 0);
            this.lblZh.AutoSize = true;
            this.lblZh.Location = new System.Drawing.Point(15, 141);
            this.lblZh.Name = "lblZh";
            this.lblZh.Size = new System.Drawing.Size(40, 17);
            this.lblZh.TabIndex = 1;
            this.lblZh.Text = "中文";
            //
            // txtZh
            //
            this.txtZh.AutoSize = false;
            this.txtZh.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtZh.Location = new System.Drawing.Point(75, 55);
            this.txtZh.Multiline = true;
            this.txtZh.Name = "txtZh";
            this.txtZh.Size = new System.Drawing.Size(550, 191);
            this.txtZh.TabIndex = 2;
            //
            // lblEn
            //
            this.lblEn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
            this.lblEn.Margin = new System.Windows.Forms.Padding(0, 12, 12, 0);
            this.lblEn.AutoSize = true;
            this.lblEn.Location = new System.Drawing.Point(15, 339);
            this.lblEn.Name = "lblEn";
            this.lblEn.Size = new System.Drawing.Size(40, 17);
            this.lblEn.TabIndex = 3;
            this.lblEn.Text = "英文";
            //
            // txtEn
            //
            this.txtEn.AutoSize = false;
            this.txtEn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtEn.Location = new System.Drawing.Point(75, 253);
            this.txtEn.Multiline = true;
            this.txtEn.Name = "txtEn";
            this.txtEn.Size = new System.Drawing.Size(550, 152);
            this.txtEn.TabIndex = 4;
            //
            // AdvancedLicenseView
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.rootLayout);
            this.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.Name = "AdvancedLicenseView";
            this.Size = new System.Drawing.Size(640, 420);
            this.rootLayout.ResumeLayout(false);
            this.rootLayout.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel rootLayout;
        private AntdUI.Checkbox chkRequired;
        private AntdUI.Label lblZh;
        private AntdUI.Input txtZh;
        private AntdUI.Label lblEn;
        private AntdUI.Input txtEn;
    }
}
