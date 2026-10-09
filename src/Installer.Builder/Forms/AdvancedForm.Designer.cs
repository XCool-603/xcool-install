namespace Installer.Builder.Forms
{
    partial class AdvancedForm
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
            this.tabs = new AntdUI.Tabs();
            this.buttonLayout = new System.Windows.Forms.FlowLayoutPanel();
            this.btnOk = new AntdUI.Button();
            this.btnCancel = new AntdUI.Button();
            this.rootLayout.SuspendLayout();
            this.buttonLayout.SuspendLayout();
            this.SuspendLayout();
            //
            // rootLayout
            //
            this.rootLayout.ColumnCount = 1;
            this.rootLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rootLayout.Controls.Add(this.tabs, 0, 0);
            this.rootLayout.Controls.Add(this.buttonLayout, 0, 1);
            this.rootLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rootLayout.Location = new System.Drawing.Point(0, 0);
            this.rootLayout.Name = "rootLayout";
            this.rootLayout.Padding = new System.Windows.Forms.Padding(12, 12, 12, 0);
            this.rootLayout.RowCount = 2;
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.rootLayout.Size = new System.Drawing.Size(660, 500);
            this.rootLayout.TabIndex = 0;
            //
            // tabs
            //
            this.tabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabs.Location = new System.Drawing.Point(15, 15);
            this.tabs.Name = "tabs";
            this.tabs.Size = new System.Drawing.Size(630, 422);
            this.tabs.TabIndex = 0;
            //
            // buttonLayout
            //
            this.buttonLayout.Controls.Add(this.btnOk);
            this.buttonLayout.Controls.Add(this.btnCancel);
            this.buttonLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonLayout.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.buttonLayout.Location = new System.Drawing.Point(12, 440);
            this.buttonLayout.Margin = new System.Windows.Forms.Padding(0);
            this.buttonLayout.Name = "buttonLayout";
            this.buttonLayout.Size = new System.Drawing.Size(636, 60);
            this.buttonLayout.TabIndex = 1;
            //
            // btnOk
            //
            this.btnOk.Location = new System.Drawing.Point(508, 12);
            this.btnOk.Margin = new System.Windows.Forms.Padding(12, 12, 0, 12);
            this.btnOk.Name = "btnOk";
            this.btnOk.Radius = 6;
            this.btnOk.Size = new System.Drawing.Size(128, 38);
            this.btnOk.TabIndex = 0;
            this.btnOk.Text = "确定";
            this.btnOk.Type = AntdUI.TTypeMini.Primary;
            this.btnOk.Click += new System.EventHandler(this.BtnOkClick);
            //
            // btnCancel
            //
            this.btnCancel.Location = new System.Drawing.Point(368, 12);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(12, 12, 0, 12);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Radius = 6;
            this.btnCancel.Size = new System.Drawing.Size(128, 38);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "取消";
            this.btnCancel.Click += new System.EventHandler(this.BtnCancelClick);
            //
            // AdvancedForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(660, 500);
            this.Controls.Add(this.rootLayout);
            this.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(560, 420);
            this.Name = "AdvancedForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "高级选项";
            this.rootLayout.ResumeLayout(false);
            this.buttonLayout.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel rootLayout;
        private AntdUI.Tabs tabs;
        private System.Windows.Forms.FlowLayoutPanel buttonLayout;
        private AntdUI.Button btnOk;
        private AntdUI.Button btnCancel;
    }
}
