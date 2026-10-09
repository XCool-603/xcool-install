namespace Installer.UI.Views
{
    partial class WelcomeView
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
            this.picLogo = new System.Windows.Forms.PictureBox();
            this.lblTitle = new AntdUI.Label();
            this.lblBody = new AntdUI.Label();
            this.rootLayout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).BeginInit();
            this.SuspendLayout();
            //
            // rootLayout
            //
            this.rootLayout.ColumnCount = 1;
            this.rootLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rootLayout.Controls.Add(this.picLogo, 0, 0);
            this.rootLayout.Controls.Add(this.lblTitle, 0, 1);
            this.rootLayout.Controls.Add(this.lblBody, 0, 2);
            this.rootLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rootLayout.Location = new System.Drawing.Point(0, 0);
            this.rootLayout.Name = "rootLayout";
            this.rootLayout.Padding = new System.Windows.Forms.Padding(24, 16, 24, 16);
            this.rootLayout.RowCount = 3;
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 96F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 48F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rootLayout.Size = new System.Drawing.Size(600, 400);
            this.rootLayout.TabIndex = 0;
            //
            // picLogo
            //
            this.picLogo.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.picLogo.Location = new System.Drawing.Point(27, 19);
            this.picLogo.Name = "picLogo";
            this.picLogo.Size = new System.Drawing.Size(96, 90);
            this.picLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picLogo.TabIndex = 0;
            this.picLogo.TabStop = false;
            //
            // lblTitle
            //
            this.lblTitle.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("微软雅黑", 18F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(27, 118);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(120, 31);
            this.lblTitle.TabIndex = 1;
            this.lblTitle.Text = "欢迎使用";
            //
            // lblBody
            //
            this.lblBody.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblBody.Font = new System.Drawing.Font("微软雅黑", 10F);
            this.lblBody.Location = new System.Drawing.Point(27, 163);
            this.lblBody.Name = "lblBody";
            this.lblBody.Size = new System.Drawing.Size(546, 218);
            this.lblBody.TabIndex = 2;
            this.lblBody.Text = "这个向导将引导你完成安装。";
            //
            // WelcomeView
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.rootLayout);
            this.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.Name = "WelcomeView";
            this.Size = new System.Drawing.Size(600, 400);
            this.rootLayout.ResumeLayout(false);
            this.rootLayout.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel rootLayout;
        private System.Windows.Forms.PictureBox picLogo;
        private AntdUI.Label lblTitle;
        private AntdUI.Label lblBody;
    }
}
