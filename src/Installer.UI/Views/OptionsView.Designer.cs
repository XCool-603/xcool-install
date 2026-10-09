namespace Installer.UI.Views
{
    partial class OptionsView
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
            this.chkDesktop = new AntdUI.Checkbox();
            this.chkStartMenu = new AntdUI.Checkbox();
            this.chkAutostart = new AntdUI.Checkbox();
            this.chkRunAfter = new AntdUI.Checkbox();
            this.rootLayout.SuspendLayout();
            this.SuspendLayout();
            //
            // rootLayout
            //
            this.rootLayout.ColumnCount = 1;
            this.rootLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rootLayout.Controls.Add(this.lblTitle, 0, 0);
            this.rootLayout.Controls.Add(this.chkDesktop, 0, 1);
            this.rootLayout.Controls.Add(this.chkStartMenu, 0, 2);
            this.rootLayout.Controls.Add(this.chkAutostart, 0, 3);
            this.rootLayout.Controls.Add(this.chkRunAfter, 0, 4);
            this.rootLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rootLayout.Location = new System.Drawing.Point(0, 0);
            this.rootLayout.Name = "rootLayout";
            this.rootLayout.Padding = new System.Windows.Forms.Padding(24, 16, 24, 16);
            this.rootLayout.RowCount = 6;
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
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
            this.lblTitle.Text = "安装选项";
            //
            // chkDesktop
            //
            this.chkDesktop.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.chkDesktop.AutoSize = true;
            this.chkDesktop.Checked = true;
            this.chkDesktop.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.chkDesktop.Location = new System.Drawing.Point(27, 62);
            this.chkDesktop.Name = "chkDesktop";
            this.chkDesktop.Size = new System.Drawing.Size(200, 23);
            this.chkDesktop.TabIndex = 1;
            this.chkDesktop.Text = "创建桌面快捷方式";
            //
            // chkStartMenu
            //
            this.chkStartMenu.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.chkStartMenu.AutoSize = true;
            this.chkStartMenu.Checked = true;
            this.chkStartMenu.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.chkStartMenu.Location = new System.Drawing.Point(27, 98);
            this.chkStartMenu.Name = "chkStartMenu";
            this.chkStartMenu.Size = new System.Drawing.Size(200, 23);
            this.chkStartMenu.TabIndex = 2;
            this.chkStartMenu.Text = "创建开始菜单快捷方式";
            //
            // chkAutostart
            //
            this.chkAutostart.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.chkAutostart.AutoSize = true;
            this.chkAutostart.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.chkAutostart.Location = new System.Drawing.Point(27, 134);
            this.chkAutostart.Name = "chkAutostart";
            this.chkAutostart.Size = new System.Drawing.Size(200, 23);
            this.chkAutostart.TabIndex = 3;
            this.chkAutostart.Text = "开机自动启动";
            //
            // chkRunAfter
            //
            this.chkRunAfter.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.chkRunAfter.AutoSize = true;
            this.chkRunAfter.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.chkRunAfter.Location = new System.Drawing.Point(27, 170);
            this.chkRunAfter.Name = "chkRunAfter";
            this.chkRunAfter.Size = new System.Drawing.Size(200, 23);
            this.chkRunAfter.TabIndex = 4;
            this.chkRunAfter.Text = "安装完成后立即运行";
            //
            // OptionsView
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.rootLayout);
            this.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.Name = "OptionsView";
            this.Size = new System.Drawing.Size(600, 400);
            this.rootLayout.ResumeLayout(false);
            this.rootLayout.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel rootLayout;
        private AntdUI.Label lblTitle;
        private AntdUI.Checkbox chkDesktop;
        private AntdUI.Checkbox chkStartMenu;
        private AntdUI.Checkbox chkAutostart;
        private AntdUI.Checkbox chkRunAfter;
    }
}
