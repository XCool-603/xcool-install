namespace Installer.Builder.Views
{
    partial class OutputBar
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
            this.pathLayout = new System.Windows.Forms.TableLayoutPanel();
            this.lblPrefix = new AntdUI.Label();
            this.lblPath = new AntdUI.Label();
            this.lblSize = new AntdUI.Label();
            this.btnChange = new AntdUI.Button();
            this.buttonLayout = new System.Windows.Forms.FlowLayoutPanel();
            this.btnPreview = new AntdUI.Button();
            this.btnBuild = new AntdUI.Button();
            this.rootLayout.SuspendLayout();
            this.pathLayout.SuspendLayout();
            this.buttonLayout.SuspendLayout();
            this.SuspendLayout();
            //
            // rootLayout
            //
            this.rootLayout.ColumnCount = 1;
            this.rootLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rootLayout.Controls.Add(this.pathLayout, 0, 0);
            this.rootLayout.Controls.Add(this.buttonLayout, 0, 1);
            this.rootLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rootLayout.Padding = new System.Windows.Forms.Padding(24, 0, 24, 0);
            this.rootLayout.Location = new System.Drawing.Point(0, 0);
            this.rootLayout.Name = "rootLayout";
            this.rootLayout.RowCount = 2;
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rootLayout.Size = new System.Drawing.Size(760, 92);
            this.rootLayout.TabIndex = 0;
            //
            // pathLayout
            //
            this.pathLayout.ColumnCount = 4;
            this.pathLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 70F));
            this.pathLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.pathLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 150F));
            this.pathLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 110F));
            this.pathLayout.Controls.Add(this.lblPrefix, 0, 0);
            this.pathLayout.Controls.Add(this.lblPath, 1, 0);
            this.pathLayout.Controls.Add(this.lblSize, 2, 0);
            this.pathLayout.Controls.Add(this.btnChange, 3, 0);
            this.pathLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pathLayout.Location = new System.Drawing.Point(0, 0);
            this.pathLayout.Margin = new System.Windows.Forms.Padding(0);
            this.pathLayout.Name = "pathLayout";
            this.pathLayout.RowCount = 1;
            this.pathLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.pathLayout.Size = new System.Drawing.Size(760, 36);
            this.pathLayout.TabIndex = 0;
            //
            // lblPrefix
            //
            this.lblPrefix.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblPrefix.AutoSize = true;
            this.lblPrefix.Location = new System.Drawing.Point(0, 9);
            this.lblPrefix.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.lblPrefix.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.lblPrefix.Name = "lblPrefix";
            this.lblPrefix.Size = new System.Drawing.Size(60, 17);
            this.lblPrefix.TabIndex = 0;
            this.lblPrefix.Text = "将生成：";
            //
            // lblPath
            //
            this.lblPath.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblPath.AutoSize = false;
            this.lblPath.AutoEllipsis = true;
            this.lblPath.Location = new System.Drawing.Point(70, 6);
            this.lblPath.Margin = new System.Windows.Forms.Padding(0, 0, 12, 0);
            this.lblPath.Font = new System.Drawing.Font("微软雅黑", 9.75F);
            this.lblPath.Name = "lblPath";
            this.lblPath.Size = new System.Drawing.Size(568, 24);
            this.lblPath.TabIndex = 1;
            this.lblPath.Text = "（选好文件夹后自动确定）";
            //
            // lblSize
            //
            this.lblSize.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblSize.AutoSize = true;
            this.lblSize.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.lblSize.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(163)))), ((int)(((byte)(175)))));
            this.lblSize.Location = new System.Drawing.Point(640, 9);
            this.lblSize.Margin = new System.Windows.Forms.Padding(12, 0, 0, 0);
            this.lblSize.Name = "lblSize";
            this.lblSize.Size = new System.Drawing.Size(100, 17);
            this.lblSize.TabIndex = 2;
            this.lblSize.Text = "";
            //
            // btnChange
            //
            this.btnChange.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnChange.Location = new System.Drawing.Point(650, 2);
            this.btnChange.Margin = new System.Windows.Forms.Padding(0, 2, 0, 2);
            this.btnChange.BorderWidth = 1F;
            this.btnChange.DefaultBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(232)))), ((int)(((byte)(239)))));
            this.btnChange.Name = "btnChange";
            this.btnChange.Radius = 8;
            this.btnChange.Size = new System.Drawing.Size(110, 32);
            this.btnChange.TabIndex = 2;
            this.btnChange.Text = "更换位置…";
            //
            // buttonLayout
            //
            this.buttonLayout.Controls.Add(this.btnBuild);
            this.buttonLayout.Controls.Add(this.btnPreview);
            this.buttonLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonLayout.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.buttonLayout.Location = new System.Drawing.Point(0, 36);
            this.buttonLayout.Margin = new System.Windows.Forms.Padding(0);
            this.buttonLayout.Name = "buttonLayout";
            this.buttonLayout.Size = new System.Drawing.Size(760, 56);
            this.buttonLayout.TabIndex = 1;
            //
            // btnPreview
            //
            this.btnPreview.Location = new System.Drawing.Point(636, 11);
            this.btnPreview.Margin = new System.Windows.Forms.Padding(12, 11, 0, 11);
            this.btnPreview.Name = "btnPreview";
            this.btnPreview.Radius = 8;
            this.btnPreview.Size = new System.Drawing.Size(100, 40);
            this.btnPreview.TabIndex = 0;
            this.btnPreview.Text = "预览向导";
            this.btnPreview.Click += new System.EventHandler(this.OnPreview);
            //
            // btnBuild
            //
            this.btnBuild.Location = new System.Drawing.Point(476, 11);
            this.btnBuild.Margin = new System.Windows.Forms.Padding(12, 11, 0, 11);
            this.btnBuild.Name = "btnBuild";
            this.btnBuild.Radius = 8;
            this.btnBuild.Size = new System.Drawing.Size(140, 44);
            this.btnBuild.TabIndex = 1;
            this.btnBuild.Text = "生成安装包";
            this.btnBuild.Type = AntdUI.TTypeMini.Primary;
            this.btnBuild.Click += new System.EventHandler(this.OnBuild);
            //
            // OutputBar
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.rootLayout);
            this.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.Name = "OutputBar";
            this.Size = new System.Drawing.Size(760, 92);
            this.rootLayout.ResumeLayout(false);
            this.rootLayout.PerformLayout();
            this.pathLayout.ResumeLayout(false);
            this.pathLayout.PerformLayout();
            this.buttonLayout.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel rootLayout;
        private System.Windows.Forms.TableLayoutPanel pathLayout;
        private AntdUI.Label lblPrefix;
        private AntdUI.Label lblPath;
        private AntdUI.Label lblSize;
        private AntdUI.Button btnChange;
        private System.Windows.Forms.FlowLayoutPanel buttonLayout;
        private AntdUI.Button btnPreview;
        private AntdUI.Button btnBuild;
    }
}
