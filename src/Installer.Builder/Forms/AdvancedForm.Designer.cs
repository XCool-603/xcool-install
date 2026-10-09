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

        #region Windows Form Designer generated code

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.pageHeader = new AntdUI.PageHeader();
            this.contentLayout = new System.Windows.Forms.TableLayoutPanel();
            this.tabs = new AntdUI.Tabs();
            this.footerLayout = new System.Windows.Forms.TableLayoutPanel();
            this.divider = new System.Windows.Forms.Panel();
            this.buttonLayout = new System.Windows.Forms.FlowLayoutPanel();
            this.btnOk = new AntdUI.Button();
            this.btnCancel = new AntdUI.Button();
            this.pageHeader.SuspendLayout();
            this.contentLayout.SuspendLayout();
            this.footerLayout.SuspendLayout();
            this.buttonLayout.SuspendLayout();
            this.SuspendLayout();
            //
            // pageHeader —— 标题栏（与主界面同一套：无边框 + 自绘标题栏）
            //
            this.pageHeader.BackColor = System.Drawing.Color.White;
            this.pageHeader.DividerShow = true;
            this.pageHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pageHeader.DragMove = true;
            this.pageHeader.Location = new System.Drawing.Point(0, 0);
            this.pageHeader.Margin = new System.Windows.Forms.Padding(0);
            this.pageHeader.MaximizeBox = false;
            this.pageHeader.MinimizeBox = false;
            this.pageHeader.Name = "pageHeader";
            this.pageHeader.Padding = new System.Windows.Forms.Padding(20, 0, 0, 0);
            this.pageHeader.ShowButton = true;
            this.pageHeader.ShowIcon = false;
            this.pageHeader.Size = new System.Drawing.Size(700, 52);
            this.pageHeader.TabIndex = 0;
            this.pageHeader.Text = "高级选项";
            this.pageHeader.UseTitleFont = true;
            //
            // contentLayout
            //
            this.contentLayout.ColumnCount = 1;
            this.contentLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.contentLayout.Controls.Add(this.tabs, 0, 0);
            this.contentLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.contentLayout.Location = new System.Drawing.Point(0, 52);
            this.contentLayout.Name = "contentLayout";
            this.contentLayout.Padding = new System.Windows.Forms.Padding(20, 16, 20, 0);
            this.contentLayout.RowCount = 1;
            this.contentLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.contentLayout.Size = new System.Drawing.Size(700, 444);
            this.contentLayout.TabIndex = 1;
            //
            // tabs
            //
            this.tabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabs.Location = new System.Drawing.Point(20, 16);
            this.tabs.Margin = new System.Windows.Forms.Padding(0);
            this.tabs.Name = "tabs";
            this.tabs.Size = new System.Drawing.Size(660, 428);
            this.tabs.TabIndex = 0;
            //
            // footerLayout —— 上方 1px 分隔线 + 按钮行
            //
            this.footerLayout.BackColor = System.Drawing.Color.White;
            this.footerLayout.ColumnCount = 1;
            this.footerLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.footerLayout.Controls.Add(this.divider, 0, 0);
            this.footerLayout.Controls.Add(this.buttonLayout, 0, 1);
            this.footerLayout.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.footerLayout.Location = new System.Drawing.Point(0, 496);
            this.footerLayout.Margin = new System.Windows.Forms.Padding(0);
            this.footerLayout.Name = "footerLayout";
            this.footerLayout.RowCount = 2;
            this.footerLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 1F));
            this.footerLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 63F));
            this.footerLayout.Size = new System.Drawing.Size(700, 64);
            this.footerLayout.TabIndex = 2;
            //
            // divider
            //
            this.divider.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(232)))), ((int)(((byte)(239)))));
            this.divider.Dock = System.Windows.Forms.DockStyle.Fill;
            this.divider.Location = new System.Drawing.Point(0, 0);
            this.divider.Margin = new System.Windows.Forms.Padding(0);
            this.divider.Name = "divider";
            this.divider.Size = new System.Drawing.Size(700, 1);
            this.divider.TabIndex = 0;
            //
            // buttonLayout
            //
            this.buttonLayout.BackColor = System.Drawing.Color.White;
            this.buttonLayout.Controls.Add(this.btnOk);
            this.buttonLayout.Controls.Add(this.btnCancel);
            this.buttonLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonLayout.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.buttonLayout.Location = new System.Drawing.Point(0, 1);
            this.buttonLayout.Margin = new System.Windows.Forms.Padding(0);
            this.buttonLayout.Name = "buttonLayout";
            this.buttonLayout.Padding = new System.Windows.Forms.Padding(0, 12, 20, 0);
            this.buttonLayout.Size = new System.Drawing.Size(700, 63);
            this.buttonLayout.TabIndex = 1;
            //
            // btnOk
            //
            this.btnOk.Location = new System.Drawing.Point(580, 12);
            this.btnOk.Margin = new System.Windows.Forms.Padding(12, 0, 0, 0);
            this.btnOk.Name = "btnOk";
            this.btnOk.Radius = 8;
            this.btnOk.Size = new System.Drawing.Size(100, 40);
            this.btnOk.TabIndex = 0;
            this.btnOk.Text = "确定";
            this.btnOk.Type = AntdUI.TTypeMini.Primary;
            //
            // btnCancel
            //
            this.btnCancel.BorderWidth = 1F;
            this.btnCancel.DefaultBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(232)))), ((int)(((byte)(239)))));
            this.btnCancel.Location = new System.Drawing.Point(468, 12);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(12, 0, 0, 0);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Radius = 8;
            this.btnCancel.Size = new System.Drawing.Size(100, 40);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "取消";
            //
            // AdvancedForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(247)))), ((int)(((byte)(250)))));
            this.ClientSize = new System.Drawing.Size(700, 560);
            this.Controls.Add(this.contentLayout);
            this.Controls.Add(this.footerLayout);
            this.Controls.Add(this.pageHeader);
            this.Font = new System.Drawing.Font("微软雅黑", 9.75F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MinimumSize = new System.Drawing.Size(640, 520);
            this.Name = "AdvancedForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "高级选项";
            this.pageHeader.ResumeLayout(false);
            this.contentLayout.ResumeLayout(false);
            this.footerLayout.ResumeLayout(false);
            this.buttonLayout.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private AntdUI.PageHeader pageHeader;
        private System.Windows.Forms.TableLayoutPanel contentLayout;
        private AntdUI.Tabs tabs;
        private System.Windows.Forms.TableLayoutPanel footerLayout;
        private System.Windows.Forms.Panel divider;
        private System.Windows.Forms.FlowLayoutPanel buttonLayout;
        private AntdUI.Button btnOk;
        private AntdUI.Button btnCancel;
    }
}
