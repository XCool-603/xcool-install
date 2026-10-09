namespace Installer.Builder.Views
{
    partial class AdvancedInstallView
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
            this.lblScope = new AntdUI.Label();
            this.cboScope = new AntdUI.Select();
            this.lblDir = new AntdUI.Label();
            this.txtDir = new AntdUI.Input();
            this.lblIcon = new AntdUI.Label();
            this.iconLayout = new System.Windows.Forms.TableLayoutPanel();
            this.txtIcon = new AntdUI.Input();
            this.btnIcon = new AntdUI.Button();
            this.lblCom = new AntdUI.Label();
            this.txtCom = new AntdUI.Input();
            this.lblComHint = new AntdUI.Label();
            this.rootLayout.SuspendLayout();
            this.iconLayout.SuspendLayout();
            this.SuspendLayout();
            //
            // rootLayout
            //
            this.rootLayout.ColumnCount = 2;
            this.rootLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 132F));
            this.rootLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rootLayout.Controls.Add(this.lblScope, 0, 0);
            this.rootLayout.Controls.Add(this.cboScope, 1, 0);
            this.rootLayout.Controls.Add(this.lblDir, 0, 1);
            this.rootLayout.Controls.Add(this.txtDir, 1, 1);
            this.rootLayout.Controls.Add(this.lblIcon, 0, 2);
            this.rootLayout.Controls.Add(this.iconLayout, 1, 2);
            this.rootLayout.Controls.Add(this.lblCom, 0, 3);
            this.rootLayout.Controls.Add(this.txtCom, 1, 3);
            this.rootLayout.Controls.Add(this.lblComHint, 1, 4);
            this.rootLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rootLayout.Location = new System.Drawing.Point(0, 0);
            this.rootLayout.Name = "rootLayout";
            this.rootLayout.Padding = new System.Windows.Forms.Padding(12);
            this.rootLayout.RowCount = 5;
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 48F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 48F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 48F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.rootLayout.Size = new System.Drawing.Size(640, 420);
            this.rootLayout.TabIndex = 0;
            //
            // lblScope
            //
            this.lblScope.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
            this.lblScope.Margin = new System.Windows.Forms.Padding(0, 12, 12, 0);
            this.lblScope.AutoSize = true;
            this.lblScope.Location = new System.Drawing.Point(15, 27);
            this.lblScope.Name = "lblScope";
            this.lblScope.Size = new System.Drawing.Size(60, 17);
            this.lblScope.TabIndex = 0;
            this.lblScope.Text = "安装范围";
            //
            // cboScope
            //
            this.cboScope.AutoSize = false;
            this.cboScope.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cboScope.Location = new System.Drawing.Point(147, 13);
            this.cboScope.Name = "cboScope";
            this.cboScope.Size = new System.Drawing.Size(478, 40);
            this.cboScope.TabIndex = 1;
            //
            // lblDir
            //
            this.lblDir.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
            this.lblDir.Margin = new System.Windows.Forms.Padding(0, 12, 12, 0);
            this.lblDir.AutoSize = true;
            this.lblDir.Location = new System.Drawing.Point(15, 75);
            this.lblDir.Name = "lblDir";
            this.lblDir.Size = new System.Drawing.Size(80, 17);
            this.lblDir.TabIndex = 2;
            this.lblDir.Text = "默认安装目录";
            //
            // txtDir
            //
            this.txtDir.AutoSize = false;
            this.txtDir.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtDir.Location = new System.Drawing.Point(147, 61);
            this.txtDir.Name = "txtDir";
            this.txtDir.PlaceholderText = "%LOCALAPPDATA%\\Programs\\MyProduct";
            this.txtDir.Size = new System.Drawing.Size(478, 40);
            this.txtDir.TabIndex = 3;
            //
            // lblIcon
            //
            this.lblIcon.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
            this.lblIcon.Margin = new System.Windows.Forms.Padding(0, 12, 12, 0);
            this.lblIcon.AutoSize = true;
            this.lblIcon.Location = new System.Drawing.Point(15, 123);
            this.lblIcon.Name = "lblIcon";
            this.lblIcon.Size = new System.Drawing.Size(60, 17);
            this.lblIcon.TabIndex = 4;
            this.lblIcon.Text = "安装包图标";
            //
            // iconLayout
            //
            this.iconLayout.ColumnCount = 2;
            this.iconLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.iconLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.iconLayout.Controls.Add(this.txtIcon, 0, 0);
            this.iconLayout.Controls.Add(this.btnIcon, 1, 0);
            this.iconLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.iconLayout.Location = new System.Drawing.Point(147, 109);
            this.iconLayout.Name = "iconLayout";
            this.iconLayout.RowCount = 1;
            this.iconLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.iconLayout.Size = new System.Drawing.Size(478, 42);
            this.iconLayout.TabIndex = 5;
            //
            // txtIcon
            //
            this.txtIcon.AutoSize = false;
            this.txtIcon.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtIcon.Location = new System.Drawing.Point(0, 0);
            this.txtIcon.Margin = new System.Windows.Forms.Padding(0);
            this.txtIcon.Name = "txtIcon";
            this.txtIcon.PlaceholderText = "app.ico";
            this.txtIcon.Size = new System.Drawing.Size(368, 40);
            this.txtIcon.TabIndex = 0;
            //
            // btnIcon
            //
            this.btnIcon.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnIcon.Location = new System.Drawing.Point(378, 0);
            this.btnIcon.Margin = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.btnIcon.Name = "btnIcon";
            this.btnIcon.Radius = 6;
            this.btnIcon.Size = new System.Drawing.Size(90, 40);
            this.btnIcon.TabIndex = 1;
            this.btnIcon.Text = "更换…";
            //
            // lblCom
            //
            this.lblCom.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
            this.lblCom.Margin = new System.Windows.Forms.Padding(0, 12, 12, 0);
            this.lblCom.AutoSize = true;
            this.lblCom.Location = new System.Drawing.Point(15, 242);
            this.lblCom.Name = "lblCom";
            this.lblCom.Size = new System.Drawing.Size(70, 17);
            this.lblCom.TabIndex = 6;
            this.lblCom.Text = "COM 组件";
            //
            // txtCom
            //
            this.txtCom.AutoSize = false;
            this.txtCom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtCom.Location = new System.Drawing.Point(147, 157);
            this.txtCom.Multiline = true;
            this.txtCom.Name = "txtCom";
            this.txtCom.PlaceholderText = "bin\\x.ocx";
            this.txtCom.Size = new System.Drawing.Size(478, 232);
            this.txtCom.TabIndex = 7;
            //
            // lblComHint
            //
            this.lblComHint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
            this.lblComHint.AutoSize = true;
            this.lblComHint.Location = new System.Drawing.Point(147, 394);
            this.lblComHint.Name = "lblComHint";
            this.lblComHint.Size = new System.Drawing.Size(300, 17);
            this.lblComHint.TabIndex = 8;
            this.lblComHint.Text = "填相对打包文件夹的路径，一行一个";
            //
            // AdvancedInstallView
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.rootLayout);
            this.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.Name = "AdvancedInstallView";
            this.Size = new System.Drawing.Size(640, 420);
            this.rootLayout.ResumeLayout(false);
            this.rootLayout.PerformLayout();
            this.iconLayout.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel rootLayout;
        private AntdUI.Label lblScope;
        private AntdUI.Select cboScope;
        private AntdUI.Label lblDir;
        private AntdUI.Input txtDir;
        private AntdUI.Label lblIcon;
        private System.Windows.Forms.TableLayoutPanel iconLayout;
        private AntdUI.Input txtIcon;
        private AntdUI.Button btnIcon;
        private AntdUI.Label lblCom;
        private AntdUI.Input txtCom;
        private AntdUI.Label lblComHint;
    }
}
