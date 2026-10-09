namespace Installer.UI.Views
{
    partial class PathView
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
            this.lblPath = new AntdUI.Label();
            this.txtPath = new AntdUI.Input();
            this.btnBrowse = new AntdUI.Button();
            this.lblSpace = new AntdUI.Label();
            this.rootLayout.SuspendLayout();
            this.SuspendLayout();
            //
            // rootLayout
            //
            this.rootLayout.ColumnCount = 2;
            this.rootLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rootLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 110F));
            this.rootLayout.Controls.Add(this.lblTitle, 0, 0);
            this.rootLayout.Controls.Add(this.lblPath, 0, 1);
            this.rootLayout.Controls.Add(this.txtPath, 0, 2);
            this.rootLayout.Controls.Add(this.btnBrowse, 1, 2);
            this.rootLayout.Controls.Add(this.lblSpace, 0, 3);
            this.rootLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rootLayout.Location = new System.Drawing.Point(0, 0);
            this.rootLayout.Name = "rootLayout";
            this.rootLayout.Padding = new System.Windows.Forms.Padding(24, 16, 24, 16);
            this.rootLayout.RowCount = 4;
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 44F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rootLayout.SetColumnSpan(this.lblTitle, 2);
            this.rootLayout.SetColumnSpan(this.lblSpace, 2);
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
            this.lblTitle.Size = new System.Drawing.Size(120, 25);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "选择安装位置";
            //
            // lblPath
            //
            this.lblPath.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblPath.AutoSize = true;
            this.lblPath.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.lblPath.Location = new System.Drawing.Point(27, 62);
            this.lblPath.Name = "lblPath";
            this.lblPath.Size = new System.Drawing.Size(68, 17);
            this.lblPath.TabIndex = 1;
            this.lblPath.Text = "安装到：";
            //
            // txtPath
            //
            this.txtPath.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtPath.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.txtPath.Location = new System.Drawing.Point(27, 91);
            this.txtPath.Name = "txtPath";
            this.txtPath.Size = new System.Drawing.Size(436, 38);
            this.txtPath.TabIndex = 2;
            //
            // btnBrowse
            //
            this.btnBrowse.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnBrowse.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.btnBrowse.Location = new System.Drawing.Point(469, 91);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Radius = 6;
            this.btnBrowse.Size = new System.Drawing.Size(104, 38);
            this.btnBrowse.TabIndex = 3;
            this.btnBrowse.Text = "浏览…";
            //
            // lblSpace
            //
            this.lblSpace.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblSpace.AutoSize = true;
            this.lblSpace.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.lblSpace.Location = new System.Drawing.Point(27, 149);
            this.lblSpace.Name = "lblSpace";
            this.lblSpace.Size = new System.Drawing.Size(200, 17);
            this.lblSpace.TabIndex = 4;
            this.lblSpace.Text = "可用空间：--，需要约：--";
            //
            // PathView
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.rootLayout);
            this.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.Name = "PathView";
            this.Size = new System.Drawing.Size(600, 400);
            this.rootLayout.ResumeLayout(false);
            this.rootLayout.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel rootLayout;
        private AntdUI.Label lblTitle;
        private AntdUI.Label lblPath;
        private AntdUI.Input txtPath;
        private AntdUI.Button btnBrowse;
        private AntdUI.Label lblSpace;
    }
}
