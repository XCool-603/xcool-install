namespace Installer.Builder.Views
{
    partial class InfoStepView
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
            this.lblName = new AntdUI.Label();
            this.txtNameZh = new AntdUI.Input();
            this.lblNameEn = new AntdUI.Label();
            this.txtNameEn = new AntdUI.Input();
            this.lblVersion = new AntdUI.Label();
            this.txtVersion = new AntdUI.Input();
            this.lblPublisher = new AntdUI.Label();
            this.txtPublisher = new AntdUI.Input();
            this.rootLayout.SuspendLayout();
            this.SuspendLayout();
            //
            // rootLayout
            //
            this.rootLayout.ColumnCount = 4;
            this.rootLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 76F));
            this.rootLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.rootLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 76F));
            this.rootLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.rootLayout.Controls.Add(this.lblStep, 0, 0);
            this.rootLayout.Controls.Add(this.lblName, 0, 1);
            this.rootLayout.Controls.Add(this.txtNameZh, 1, 1);
            this.rootLayout.Controls.Add(this.lblNameEn, 2, 1);
            this.rootLayout.Controls.Add(this.txtNameEn, 3, 1);
            this.rootLayout.Controls.Add(this.lblVersion, 0, 2);
            this.rootLayout.Controls.Add(this.txtVersion, 1, 2);
            this.rootLayout.Controls.Add(this.lblPublisher, 2, 2);
            this.rootLayout.Controls.Add(this.txtPublisher, 3, 2);
            this.rootLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rootLayout.Location = new System.Drawing.Point(0, 0);
            this.rootLayout.Name = "rootLayout";
            this.rootLayout.RowCount = 3;
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 52F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 52F));
            this.rootLayout.SetColumnSpan(this.lblStep, 4);
            this.rootLayout.Size = new System.Drawing.Size(760, 134);
            this.rootLayout.TabIndex = 0;
            //
            // lblStep
            //
            this.lblStep.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblStep.AutoSize = true;
            this.lblStep.Font = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Bold);
            this.lblStep.Location = new System.Drawing.Point(0, 5);
            this.lblStep.Margin = new System.Windows.Forms.Padding(0);
            this.lblStep.Name = "lblStep";
            this.lblStep.Size = new System.Drawing.Size(160, 19);
            this.lblStep.TabIndex = 0;
            this.lblStep.Text = "② 填写基本信息";
            //
            // lblName
            //
            this.lblName.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(0, 47);
            this.lblName.Margin = new System.Windows.Forms.Padding(0);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(60, 17);
            this.lblName.TabIndex = 1;
            this.lblName.Text = "产品名称";
            //
            // txtNameZh
            //
            this.txtNameZh.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtNameZh.Location = new System.Drawing.Point(76, 36);
            this.txtNameZh.Margin = new System.Windows.Forms.Padding(0, 6, 12, 6);
            this.txtNameZh.Name = "txtNameZh";
            this.txtNameZh.PlaceholderText = "我的产品";
            this.txtNameZh.Size = new System.Drawing.Size(292, 40);
            this.txtNameZh.TabIndex = 2;
            //
            // lblNameEn
            //
            this.lblNameEn.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblNameEn.AutoSize = true;
            this.lblNameEn.Location = new System.Drawing.Point(380, 47);
            this.lblNameEn.Margin = new System.Windows.Forms.Padding(0);
            this.lblNameEn.Name = "lblNameEn";
            this.lblNameEn.Size = new System.Drawing.Size(60, 17);
            this.lblNameEn.TabIndex = 3;
            this.lblNameEn.Text = "英文名称";
            //
            // txtNameEn
            //
            this.txtNameEn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtNameEn.Location = new System.Drawing.Point(456, 36);
            this.txtNameEn.Margin = new System.Windows.Forms.Padding(0, 6, 0, 6);
            this.txtNameEn.Name = "txtNameEn";
            this.txtNameEn.PlaceholderText = "可留空（英文系统会显示中文名）";
            this.txtNameEn.Size = new System.Drawing.Size(304, 40);
            this.txtNameEn.TabIndex = 4;
            //
            // lblVersion
            //
            this.lblVersion.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblVersion.AutoSize = true;
            this.lblVersion.Location = new System.Drawing.Point(0, 99);
            this.lblVersion.Margin = new System.Windows.Forms.Padding(0);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(50, 17);
            this.lblVersion.TabIndex = 5;
            this.lblVersion.Text = "版本号";
            //
            // txtVersion
            //
            this.txtVersion.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtVersion.Location = new System.Drawing.Point(76, 88);
            this.txtVersion.Margin = new System.Windows.Forms.Padding(0, 6, 12, 6);
            this.txtVersion.Name = "txtVersion";
            this.txtVersion.PlaceholderText = "1.0.0";
            this.txtVersion.Size = new System.Drawing.Size(292, 40);
            this.txtVersion.TabIndex = 6;
            //
            // lblPublisher
            //
            this.lblPublisher.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblPublisher.AutoSize = true;
            this.lblPublisher.Location = new System.Drawing.Point(380, 99);
            this.lblPublisher.Margin = new System.Windows.Forms.Padding(0);
            this.lblPublisher.Name = "lblPublisher";
            this.lblPublisher.Size = new System.Drawing.Size(40, 17);
            this.lblPublisher.TabIndex = 7;
            this.lblPublisher.Text = "厂商";
            //
            // txtPublisher
            //
            this.txtPublisher.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtPublisher.Location = new System.Drawing.Point(456, 88);
            this.txtPublisher.Margin = new System.Windows.Forms.Padding(0, 6, 0, 6);
            this.txtPublisher.Name = "txtPublisher";
            this.txtPublisher.PlaceholderText = "—";
            this.txtPublisher.Size = new System.Drawing.Size(304, 40);
            this.txtPublisher.TabIndex = 8;
            //
            // InfoStepView
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.rootLayout);
            this.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.Name = "InfoStepView";
            this.Size = new System.Drawing.Size(760, 134);
            this.rootLayout.ResumeLayout(false);
            this.rootLayout.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel rootLayout;
        private AntdUI.Label lblStep;
        private AntdUI.Label lblName;
        private AntdUI.Input txtNameZh;
        private AntdUI.Label lblNameEn;
        private AntdUI.Input txtNameEn;
        private AntdUI.Label lblVersion;
        private AntdUI.Input txtVersion;
        private AntdUI.Label lblPublisher;
        private AntdUI.Input txtPublisher;
    }
}
