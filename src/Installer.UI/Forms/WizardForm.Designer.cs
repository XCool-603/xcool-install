namespace Installer.UI.Forms
{
    partial class WizardForm
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

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.rootLayout = new System.Windows.Forms.TableLayoutPanel();
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.headerLayout = new System.Windows.Forms.TableLayoutPanel();
            this.picLogo = new System.Windows.Forms.PictureBox();
            this.lblProduct = new AntdUI.Label();
            this.lblVersion = new AntdUI.Label();
            this.pnlContent = new System.Windows.Forms.Panel();
            this.pnlFooter = new System.Windows.Forms.Panel();
            this.footerLayout = new System.Windows.Forms.TableLayoutPanel();
            this.steps = new AntdUI.Steps();
            this.buttonFlow = new System.Windows.Forms.FlowLayoutPanel();
            this.btnPrimary = new AntdUI.Button();
            this.btnCancel = new AntdUI.Button();
            this.btnBack = new AntdUI.Button();
            this.rootLayout.SuspendLayout();
            this.pnlHeader.SuspendLayout();
            this.headerLayout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).BeginInit();
            this.pnlFooter.SuspendLayout();
            this.footerLayout.SuspendLayout();
            this.buttonFlow.SuspendLayout();
            this.SuspendLayout();
            //
            // rootLayout
            //
            this.rootLayout.ColumnCount = 1;
            this.rootLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rootLayout.Controls.Add(this.pnlHeader, 0, 0);
            this.rootLayout.Controls.Add(this.pnlContent, 0, 1);
            this.rootLayout.Controls.Add(this.pnlFooter, 0, 2);
            this.rootLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rootLayout.Location = new System.Drawing.Point(0, 0);
            this.rootLayout.Name = "rootLayout";
            this.rootLayout.RowCount = 3;
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 64F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 104F));
            this.rootLayout.Size = new System.Drawing.Size(720, 520);
            this.rootLayout.TabIndex = 0;
            //
            // pnlHeader
            //
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(6)))), ((int)(((byte)(157)))), ((int)(((byte)(231)))));
            this.pnlHeader.Controls.Add(this.headerLayout);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Margin = new System.Windows.Forms.Padding(0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(720, 64);
            this.pnlHeader.TabIndex = 0;
            //
            // headerLayout
            //
            this.headerLayout.ColumnCount = 3;
            this.headerLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 56F));
            this.headerLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.headerLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.headerLayout.Controls.Add(this.picLogo, 0, 0);
            this.headerLayout.Controls.Add(this.lblProduct, 1, 0);
            this.headerLayout.Controls.Add(this.lblVersion, 2, 0);
            this.headerLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.headerLayout.Location = new System.Drawing.Point(0, 0);
            this.headerLayout.Name = "headerLayout";
            this.headerLayout.Padding = new System.Windows.Forms.Padding(16, 8, 16, 8);
            this.headerLayout.RowCount = 1;
            this.headerLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.headerLayout.Size = new System.Drawing.Size(720, 64);
            this.headerLayout.TabIndex = 0;
            //
            // picLogo
            //
            this.picLogo.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.picLogo.Location = new System.Drawing.Point(19, 12);
            this.picLogo.Name = "picLogo";
            this.picLogo.Size = new System.Drawing.Size(40, 40);
            this.picLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picLogo.TabIndex = 0;
            this.picLogo.TabStop = false;
            //
            // lblProduct
            //
            this.lblProduct.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblProduct.AutoSize = true;
            this.lblProduct.Font = new System.Drawing.Font("微软雅黑", 13F, System.Drawing.FontStyle.Bold);
            this.lblProduct.ForeColor = System.Drawing.Color.White;
            this.lblProduct.Location = new System.Drawing.Point(75, 20);
            this.lblProduct.Name = "lblProduct";
            this.lblProduct.Size = new System.Drawing.Size(120, 24);
            this.lblProduct.TabIndex = 1;
            this.lblProduct.Text = "产品名称";
            //
            // lblVersion
            //
            this.lblVersion.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblVersion.AutoSize = true;
            this.lblVersion.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.lblVersion.ForeColor = System.Drawing.Color.White;
            this.lblVersion.Location = new System.Drawing.Point(640, 24);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(60, 17);
            this.lblVersion.TabIndex = 2;
            this.lblVersion.Text = "1.0.0";
            //
            // pnlContent
            //
            this.pnlContent.BackColor = System.Drawing.Color.White;
            this.pnlContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlContent.Location = new System.Drawing.Point(0, 64);
            this.pnlContent.Margin = new System.Windows.Forms.Padding(0);
            this.pnlContent.Name = "pnlContent";
            this.pnlContent.Size = new System.Drawing.Size(720, 352);
            this.pnlContent.TabIndex = 1;
            //
            // pnlFooter
            //
            this.pnlFooter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.pnlFooter.Controls.Add(this.footerLayout);
            this.pnlFooter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlFooter.Location = new System.Drawing.Point(0, 416);
            this.pnlFooter.Margin = new System.Windows.Forms.Padding(0);
            this.pnlFooter.Name = "pnlFooter";
            this.pnlFooter.Size = new System.Drawing.Size(720, 104);
            this.pnlFooter.TabIndex = 2;
            //
            // footerLayout
            //
            this.footerLayout.ColumnCount = 1;
            this.footerLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.footerLayout.Controls.Add(this.steps, 0, 0);
            this.footerLayout.Controls.Add(this.buttonFlow, 0, 1);
            this.footerLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.footerLayout.Location = new System.Drawing.Point(0, 0);
            this.footerLayout.Name = "footerLayout";
            this.footerLayout.RowCount = 2;
            this.footerLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 48F));
            this.footerLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.footerLayout.Size = new System.Drawing.Size(720, 104);
            this.footerLayout.TabIndex = 0;
            //
            // steps
            //
            this.steps.Dock = System.Windows.Forms.DockStyle.Fill;
            this.steps.Items.Add(new AntdUI.StepsItem() { ID = "license", Title = "许可协议" });
            this.steps.Items.Add(new AntdUI.StepsItem() { ID = "path", Title = "安装位置" });
            this.steps.Items.Add(new AntdUI.StepsItem() { ID = "options", Title = "选项" });
            this.steps.Items.Add(new AntdUI.StepsItem() { ID = "install", Title = "安装" });
            this.steps.Items.Add(new AntdUI.StepsItem() { ID = "finish", Title = "完成" });
            this.steps.Location = new System.Drawing.Point(16, 3);
            this.steps.Name = "steps";
            this.steps.Size = new System.Drawing.Size(688, 42);
            this.steps.TabIndex = 0;
            //
            // buttonFlow
            //
            this.buttonFlow.Controls.Add(this.btnPrimary);
            this.buttonFlow.Controls.Add(this.btnCancel);
            this.buttonFlow.Controls.Add(this.btnBack);
            this.buttonFlow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonFlow.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.buttonFlow.Location = new System.Drawing.Point(16, 51);
            this.buttonFlow.Name = "buttonFlow";
            this.buttonFlow.Padding = new System.Windows.Forms.Padding(0, 8, 0, 0);
            this.buttonFlow.Size = new System.Drawing.Size(688, 50);
            this.buttonFlow.TabIndex = 1;
            //
            // btnPrimary
            //
            this.btnPrimary.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.btnPrimary.Location = new System.Drawing.Point(568, 11);
            this.btnPrimary.Name = "btnPrimary";
            this.btnPrimary.Radius = 6;
            this.btnPrimary.Size = new System.Drawing.Size(120, 36);
            this.btnPrimary.TabIndex = 0;
            this.btnPrimary.Text = "下一步";
            this.btnPrimary.Type = AntdUI.TTypeMini.Primary;
            this.btnPrimary.Click += new System.EventHandler(this.BtnPrimaryClick);
            //
            // btnCancel
            //
            this.btnCancel.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.btnCancel.Location = new System.Drawing.Point(442, 11);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Radius = 6;
            this.btnCancel.Size = new System.Drawing.Size(120, 36);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "取消";
            this.btnCancel.Click += new System.EventHandler(this.BtnCancelClick);
            //
            // btnBack
            //
            this.btnBack.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.btnBack.Location = new System.Drawing.Point(316, 11);
            this.btnBack.Name = "btnBack";
            this.btnBack.Radius = 6;
            this.btnBack.Size = new System.Drawing.Size(120, 36);
            this.btnBack.TabIndex = 2;
            this.btnBack.Text = "上一步";
            this.btnBack.Click += new System.EventHandler(this.BtnBackClick);
            //
            // WizardForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(720, 520);
            this.Controls.Add(this.rootLayout);
            this.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "WizardForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "安装向导";
            this.rootLayout.ResumeLayout(false);
            this.pnlHeader.ResumeLayout(false);
            this.headerLayout.ResumeLayout(false);
            this.headerLayout.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).EndInit();
            this.pnlFooter.ResumeLayout(false);
            this.footerLayout.ResumeLayout(false);
            this.buttonFlow.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel rootLayout;
        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.TableLayoutPanel headerLayout;
        private System.Windows.Forms.PictureBox picLogo;
        private AntdUI.Label lblProduct;
        private AntdUI.Label lblVersion;
        private System.Windows.Forms.Panel pnlContent;
        private System.Windows.Forms.Panel pnlFooter;
        private System.Windows.Forms.TableLayoutPanel footerLayout;
        private AntdUI.Steps steps;
        private System.Windows.Forms.FlowLayoutPanel buttonFlow;
        private AntdUI.Button btnPrimary;
        private AntdUI.Button btnCancel;
        private AntdUI.Button btnBack;
    }
}
