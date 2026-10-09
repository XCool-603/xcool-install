namespace Installer.UI.Views
{
    partial class LicenseView
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
            this.rootLayout = new System.Windows.Forms.TableLayoutPanel();
            this.lblTitle = new AntdUI.Label();
            this.lblSubtitle = new AntdUI.Label();
            this.licenseCard = new AntdUI.Panel();
            this.txtLicense = new System.Windows.Forms.TextBox();
            this.acceptPanel = new AntdUI.Panel();
            this.acceptLayout = new System.Windows.Forms.TableLayoutPanel();
            this.chkAccept = new AntdUI.Checkbox();
            this.lblAcceptHint = new AntdUI.Label();
            this.rootLayout.SuspendLayout();
            this.licenseCard.SuspendLayout();
            this.acceptPanel.SuspendLayout();
            this.acceptLayout.SuspendLayout();
            this.SuspendLayout();
            //
            // rootLayout
            //
            this.rootLayout.ColumnCount = 1;
            this.rootLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rootLayout.Controls.Add(this.lblTitle, 0, 0);
            this.rootLayout.Controls.Add(this.lblSubtitle, 0, 1);
            this.rootLayout.Controls.Add(this.licenseCard, 0, 2);
            this.rootLayout.Controls.Add(this.acceptPanel, 0, 3);
            this.rootLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rootLayout.Location = new System.Drawing.Point(0, 0);
            this.rootLayout.Name = "rootLayout";
            this.rootLayout.RowCount = 4;
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 84F));
            this.rootLayout.Size = new System.Drawing.Size(600, 400);
            this.rootLayout.TabIndex = 0;
            //
            // lblTitle
            //
            this.lblTitle.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("微软雅黑", 17F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(0, 0);
            this.lblTitle.Margin = new System.Windows.Forms.Padding(0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(200, 30);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "许可协议";
            //
            // lblSubtitle
            //
            this.lblSubtitle.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblSubtitle.AutoSize = true;
            this.lblSubtitle.Font = new System.Drawing.Font("微软雅黑", 12F);
            this.lblSubtitle.Location = new System.Drawing.Point(0, 42);
            this.lblSubtitle.Margin = new System.Windows.Forms.Padding(0);
            this.lblSubtitle.Name = "lblSubtitle";
            this.lblSubtitle.Size = new System.Drawing.Size(400, 21);
            this.lblSubtitle.TabIndex = 1;
            this.lblSubtitle.Text = "";
            //
            // licenseCard
            //
            this.licenseCard.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(232)))), ((int)(((byte)(239)))));
            this.licenseCard.BorderWidth = 1F;
            this.licenseCard.Controls.Add(this.txtLicense);
            this.licenseCard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.licenseCard.Location = new System.Drawing.Point(0, 72);
            this.licenseCard.Margin = new System.Windows.Forms.Padding(0, 0, 0, 12);
            this.licenseCard.Name = "licenseCard";
            this.licenseCard.Padding = new System.Windows.Forms.Padding(16, 14, 8, 14);
            this.licenseCard.Radius = 12;
            this.licenseCard.Size = new System.Drawing.Size(600, 232);
            this.licenseCard.TabIndex = 2;
            //
            // txtLicense
            //
            this.txtLicense.BackColor = System.Drawing.Color.White;
            this.txtLicense.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtLicense.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtLicense.Font = new System.Drawing.Font("微软雅黑", 11F);
            this.txtLicense.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(85)))), ((int)(((byte)(99)))));
            this.txtLicense.Location = new System.Drawing.Point(16, 14);
            this.txtLicense.Multiline = true;
            this.txtLicense.Name = "txtLicense";
            this.txtLicense.ReadOnly = true;
            this.txtLicense.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLicense.Size = new System.Drawing.Size(576, 204);
            this.txtLicense.TabIndex = 0;
            this.txtLicense.WordWrap = true;
            //
            // acceptPanel
            //
            this.acceptPanel.Controls.Add(this.acceptLayout);
            this.acceptPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.acceptPanel.Location = new System.Drawing.Point(0, 316);
            this.acceptPanel.Margin = new System.Windows.Forms.Padding(0);
            this.acceptPanel.Name = "acceptPanel";
            this.acceptPanel.Padding = new System.Windows.Forms.Padding(16, 10, 16, 10);
            this.acceptPanel.Radius = 12;
            this.acceptPanel.Size = new System.Drawing.Size(600, 84);
            this.acceptPanel.TabIndex = 3;
            //
            // acceptLayout
            //
            this.acceptLayout.ColumnCount = 1;
            this.acceptLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.acceptLayout.Controls.Add(this.chkAccept, 0, 0);
            this.acceptLayout.Controls.Add(this.lblAcceptHint, 0, 1);
            this.acceptLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.acceptLayout.Location = new System.Drawing.Point(16, 10);
            this.acceptLayout.Margin = new System.Windows.Forms.Padding(0);
            this.acceptLayout.Name = "acceptLayout";
            this.acceptLayout.RowCount = 2;
            this.acceptLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34F));
            this.acceptLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.acceptLayout.Size = new System.Drawing.Size(568, 64);
            this.acceptLayout.TabIndex = 0;
            //
            // chkAccept
            //
            this.chkAccept.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.chkAccept.AutoSize = true;
            this.chkAccept.Font = new System.Drawing.Font("微软雅黑", 13F);
            this.chkAccept.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(41)))), ((int)(((byte)(55)))));
            this.chkAccept.Location = new System.Drawing.Point(0, 3);
            this.chkAccept.Margin = new System.Windows.Forms.Padding(0);
            this.chkAccept.Name = "chkAccept";
            this.chkAccept.Size = new System.Drawing.Size(300, 28);
            this.chkAccept.TabIndex = 0;
            this.chkAccept.Text = "我已阅读并同意许可协议";
            //
            // lblAcceptHint
            //
            this.lblAcceptHint.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblAcceptHint.AutoSize = true;
            this.lblAcceptHint.Font = new System.Drawing.Font("微软雅黑", 11F);
            this.lblAcceptHint.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(163)))), ((int)(((byte)(175)))));
            this.lblAcceptHint.Location = new System.Drawing.Point(0, 36);
            this.lblAcceptHint.Margin = new System.Windows.Forms.Padding(0);
            this.lblAcceptHint.Name = "lblAcceptHint";
            this.lblAcceptHint.Size = new System.Drawing.Size(400, 19);
            this.lblAcceptHint.TabIndex = 1;
            this.lblAcceptHint.Text = "";
            //
            // LicenseView
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.rootLayout);
            this.Font = new System.Drawing.Font("微软雅黑", 12F);
            this.Name = "LicenseView";
            this.Size = new System.Drawing.Size(600, 400);
            this.rootLayout.ResumeLayout(false);
            this.rootLayout.PerformLayout();
            this.licenseCard.ResumeLayout(false);
            this.licenseCard.PerformLayout();
            this.acceptPanel.ResumeLayout(false);
            this.acceptLayout.ResumeLayout(false);
            this.acceptLayout.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel rootLayout;
        private AntdUI.Label lblTitle;
        private AntdUI.Label lblSubtitle;
        private AntdUI.Panel licenseCard;
        private System.Windows.Forms.TextBox txtLicense;
        private AntdUI.Panel acceptPanel;
        private System.Windows.Forms.TableLayoutPanel acceptLayout;
        private AntdUI.Checkbox chkAccept;
        private AntdUI.Label lblAcceptHint;
    }
}
