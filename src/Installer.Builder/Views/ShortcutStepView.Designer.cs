namespace Installer.Builder.Views
{
    partial class ShortcutStepView
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
            this.flow = new System.Windows.Forms.FlowLayoutPanel();
            this.chkDesktop = new AntdUI.Checkbox();
            this.chkStartMenu = new AntdUI.Checkbox();
            this.chkAutostart = new AntdUI.Checkbox();
            this.chkRunAfter = new AntdUI.Checkbox();
            this.flow.SuspendLayout();
            this.SuspendLayout();
            //
            // flow
            //
            this.flow.Controls.Add(this.chkDesktop);
            this.flow.Controls.Add(this.chkStartMenu);
            this.flow.Controls.Add(this.chkAutostart);
            this.flow.Controls.Add(this.chkRunAfter);
            this.flow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flow.Location = new System.Drawing.Point(0, 0);
            this.flow.Name = "flow";
            this.flow.Size = new System.Drawing.Size(760, 36);
            this.flow.TabIndex = 0;
            this.flow.WrapContents = false;
            //
            // chkDesktop
            //
            this.chkDesktop.AutoSize = true;
            this.chkDesktop.Location = new System.Drawing.Point(0, 6);
            this.chkDesktop.Margin = new System.Windows.Forms.Padding(0, 6, 24, 6);
            this.chkDesktop.Name = "chkDesktop";
            this.chkDesktop.Size = new System.Drawing.Size(130, 23);
            this.chkDesktop.TabIndex = 0;
            this.chkDesktop.Text = "创建桌面快捷方式";
            this.chkDesktop.CheckedChanged += new AntdUI.BoolEventHandler(this.OnCheckedChanged);
            //
            // chkStartMenu
            //
            this.chkStartMenu.AutoSize = true;
            this.chkStartMenu.Location = new System.Drawing.Point(154, 6);
            this.chkStartMenu.Margin = new System.Windows.Forms.Padding(0, 6, 24, 6);
            this.chkStartMenu.Name = "chkStartMenu";
            this.chkStartMenu.Size = new System.Drawing.Size(160, 23);
            this.chkStartMenu.TabIndex = 1;
            this.chkStartMenu.Text = "创建开始菜单快捷方式";
            this.chkStartMenu.CheckedChanged += new AntdUI.BoolEventHandler(this.OnCheckedChanged);
            //
            // chkAutostart
            //
            this.chkAutostart.AutoSize = true;
            this.chkAutostart.Location = new System.Drawing.Point(338, 6);
            this.chkAutostart.Margin = new System.Windows.Forms.Padding(0, 6, 24, 6);
            this.chkAutostart.Name = "chkAutostart";
            this.chkAutostart.Size = new System.Drawing.Size(110, 23);
            this.chkAutostart.TabIndex = 2;
            this.chkAutostart.Text = "开机自动启动";
            this.chkAutostart.CheckedChanged += new AntdUI.BoolEventHandler(this.OnCheckedChanged);
            //
            // chkRunAfter
            //
            this.chkRunAfter.AutoSize = true;
            this.chkRunAfter.Location = new System.Drawing.Point(472, 6);
            this.chkRunAfter.Margin = new System.Windows.Forms.Padding(0, 6, 0, 6);
            this.chkRunAfter.Name = "chkRunAfter";
            this.chkRunAfter.Size = new System.Drawing.Size(150, 23);
            this.chkRunAfter.TabIndex = 3;
            this.chkRunAfter.Text = "安装完成后立即运行";
            this.chkRunAfter.CheckedChanged += new AntdUI.BoolEventHandler(this.OnCheckedChanged);
            //
            // ShortcutStepView
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.flow);
            this.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.Name = "ShortcutStepView";
            this.Size = new System.Drawing.Size(760, 36);
            this.flow.ResumeLayout(false);
            this.flow.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flow;
        private AntdUI.Checkbox chkDesktop;
        private AntdUI.Checkbox chkStartMenu;
        private AntdUI.Checkbox chkAutostart;
        private AntdUI.Checkbox chkRunAfter;
    }
}
