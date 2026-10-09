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
            this.rootLayout = new System.Windows.Forms.TableLayoutPanel();
            this.lblStep = new AntdUI.Label();
            this.flow = new System.Windows.Forms.FlowLayoutPanel();
            this.chkDesktop = new AntdUI.Checkbox();
            this.chkStartMenu = new AntdUI.Checkbox();
            this.chkAutostart = new AntdUI.Checkbox();
            this.chkRunAfter = new AntdUI.Checkbox();
            this.rootLayout.SuspendLayout();
            this.flow.SuspendLayout();
            this.SuspendLayout();
            //
            // rootLayout
            //
            this.rootLayout.ColumnCount = 1;
            this.rootLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rootLayout.Controls.Add(this.lblStep, 0, 0);
            this.rootLayout.Controls.Add(this.flow, 0, 1);
            this.rootLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rootLayout.Location = new System.Drawing.Point(0, 0);
            this.rootLayout.Name = "rootLayout";
            this.rootLayout.RowCount = 2;
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 48F));
            this.rootLayout.Size = new System.Drawing.Size(832, 74);
            this.rootLayout.TabIndex = 0;
            //
            // lblStep
            //
            this.lblStep.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblStep.AutoSize = true;
            this.lblStep.Font = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Bold);
            this.lblStep.Location = new System.Drawing.Point(0, 3);
            this.lblStep.Margin = new System.Windows.Forms.Padding(0);
            this.lblStep.PrefixSvg = "<svg viewBox=\"0 0 20 20\"><rect width=\"20\" height=\"20\" rx=\"6\" fill=\"#1677FF\"/><text x=\"10\" y=\"15\" font-size=\"13\" fill=\"#FFFFFF\" text-anchor=\"middle\">3</text></svg>";
            this.lblStep.IconGap = 6;
            this.lblStep.Name = "lblStep";
            this.lblStep.Size = new System.Drawing.Size(120, 19);
            this.lblStep.TabIndex = 0;
            this.lblStep.Text = "快捷方式";
            //
            // flow
            //
            this.flow.Controls.Add(this.chkDesktop);
            this.flow.Controls.Add(this.chkStartMenu);
            this.flow.Controls.Add(this.chkAutostart);
            this.flow.Controls.Add(this.chkRunAfter);
            this.flow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flow.Location = new System.Drawing.Point(0, 26);
            this.flow.Margin = new System.Windows.Forms.Padding(0);
            this.flow.Name = "flow";
            this.flow.Size = new System.Drawing.Size(760, 40);
            this.flow.TabIndex = 1;
            this.flow.WrapContents = false;
            //
            // chkDesktop
            //
            this.chkDesktop.AutoSize = true;
            this.chkDesktop.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(41)))), ((int)(((byte)(55)))));
            this.chkDesktop.Location = new System.Drawing.Point(0, 4);
            this.chkDesktop.Margin = new System.Windows.Forms.Padding(0, 4, 28, 4);
            this.chkDesktop.Name = "chkDesktop";
            this.chkDesktop.Size = new System.Drawing.Size(130, 23);
            this.chkDesktop.TabIndex = 0;
            this.chkDesktop.Text = "创建桌面快捷方式";
            this.chkDesktop.CheckedChanged += new AntdUI.BoolEventHandler(this.OnCheckedChanged);
            //
            // chkStartMenu
            //
            this.chkStartMenu.AutoSize = true;
            this.chkStartMenu.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(41)))), ((int)(((byte)(55)))));
            this.chkStartMenu.Location = new System.Drawing.Point(158, 4);
            this.chkStartMenu.Margin = new System.Windows.Forms.Padding(0, 4, 28, 4);
            this.chkStartMenu.Name = "chkStartMenu";
            this.chkStartMenu.Size = new System.Drawing.Size(160, 23);
            this.chkStartMenu.TabIndex = 1;
            this.chkStartMenu.Text = "创建开始菜单快捷方式";
            this.chkStartMenu.CheckedChanged += new AntdUI.BoolEventHandler(this.OnCheckedChanged);
            //
            // chkAutostart
            //
            this.chkAutostart.AutoSize = true;
            this.chkAutostart.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(41)))), ((int)(((byte)(55)))));
            this.chkAutostart.Location = new System.Drawing.Point(346, 4);
            this.chkAutostart.Margin = new System.Windows.Forms.Padding(0, 4, 28, 4);
            this.chkAutostart.Name = "chkAutostart";
            this.chkAutostart.Size = new System.Drawing.Size(110, 23);
            this.chkAutostart.TabIndex = 2;
            this.chkAutostart.Text = "开机自动启动";
            this.chkAutostart.CheckedChanged += new AntdUI.BoolEventHandler(this.OnCheckedChanged);
            //
            // chkRunAfter
            //
            this.chkRunAfter.AutoSize = true;
            this.chkRunAfter.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(41)))), ((int)(((byte)(55)))));
            this.chkRunAfter.Location = new System.Drawing.Point(484, 4);
            this.chkRunAfter.Margin = new System.Windows.Forms.Padding(0, 4, 0, 4);
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
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.rootLayout);
            this.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.Name = "ShortcutStepView";
            this.Size = new System.Drawing.Size(832, 74);
            this.rootLayout.ResumeLayout(false);
            this.rootLayout.PerformLayout();
            this.flow.ResumeLayout(false);
            this.flow.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel rootLayout;
        private AntdUI.Label lblStep;
        private System.Windows.Forms.FlowLayoutPanel flow;
        private AntdUI.Checkbox chkDesktop;
        private AntdUI.Checkbox chkStartMenu;
        private AntdUI.Checkbox chkAutostart;
        private AntdUI.Checkbox chkRunAfter;
    }
}
