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

        #region Windows Form Designer generated code

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.card = new AntdUI.Panel();
            this.cardLayout = new System.Windows.Forms.TableLayoutPanel();
            this.headerView = new Installer.UI.Views.WizardHeaderView();
            this.stepBar = new Installer.UI.Views.StepBarView();
            this.pnlContent = new System.Windows.Forms.Panel();
            this.footerLayout = new System.Windows.Forms.TableLayoutPanel();
            this.lblStepInfo = new AntdUI.Label();
            this.buttonFlow = new System.Windows.Forms.FlowLayoutPanel();
            this.btnPrimary = new AntdUI.Button();
            this.btnBack = new AntdUI.Button();
            this.btnCancel = new AntdUI.Button();
            this.card.SuspendLayout();
            this.cardLayout.SuspendLayout();
            this.footerLayout.SuspendLayout();
            this.buttonFlow.SuspendLayout();
            this.SuspendLayout();
            //
            // card —— 白圆角卡片，浮在灰底上
            //
            this.card.Back = System.Drawing.Color.White;
            this.card.Controls.Add(this.cardLayout);
            this.card.Location = new System.Drawing.Point(24, 24);
            this.card.Margin = new System.Windows.Forms.Padding(0);
            this.card.Name = "card";
            this.card.Radius = 16;
            this.card.Shadow = 12;
            this.card.ShadowColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.card.ShadowOpacity = 0.18F;
            this.card.Size = new System.Drawing.Size(640, 664);
            this.card.TabIndex = 0;
            //
            // cardLayout —— 品牌头 / 步骤条 / 内容 / 底栏
            //
            this.cardLayout.ColumnCount = 1;
            this.cardLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.cardLayout.Controls.Add(this.headerView, 0, 0);
            this.cardLayout.Controls.Add(this.stepBar, 0, 1);
            this.cardLayout.Controls.Add(this.pnlContent, 0, 2);
            this.cardLayout.Controls.Add(this.footerLayout, 0, 3);
            this.cardLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cardLayout.Location = new System.Drawing.Point(0, 0);
            this.cardLayout.Margin = new System.Windows.Forms.Padding(0);
            this.cardLayout.Name = "cardLayout";
            this.cardLayout.RowCount = 4;
            this.cardLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 104F));
            this.cardLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 64F));
            this.cardLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.cardLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 72F));
            this.cardLayout.Size = new System.Drawing.Size(640, 664);
            this.cardLayout.TabIndex = 0;
            //
            // headerView —— 蓝色品牌区
            //
            this.headerView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.headerView.Location = new System.Drawing.Point(0, 0);
            this.headerView.Margin = new System.Windows.Forms.Padding(0);
            this.headerView.Name = "headerView";
            this.headerView.Size = new System.Drawing.Size(640, 104);
            this.headerView.TabIndex = 0;
            //
            // stepBar —— 步骤条
            //
            this.stepBar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stepBar.Location = new System.Drawing.Point(0, 104);
            this.stepBar.Margin = new System.Windows.Forms.Padding(0);
            this.stepBar.Name = "stepBar";
            this.stepBar.Size = new System.Drawing.Size(640, 64);
            this.stepBar.TabIndex = 1;
            //
            // pnlContent —— 页面宿主
            //
            this.pnlContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlContent.Location = new System.Drawing.Point(0, 168);
            this.pnlContent.Margin = new System.Windows.Forms.Padding(0);
            this.pnlContent.Name = "pnlContent";
            this.pnlContent.Padding = new System.Windows.Forms.Padding(32, 0, 32, 0);
            this.pnlContent.Size = new System.Drawing.Size(640, 424);
            this.pnlContent.TabIndex = 2;
            //
            // footerLayout
            //
            this.footerLayout.ColumnCount = 2;
            this.footerLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.footerLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 330F));
            this.footerLayout.Controls.Add(this.lblStepInfo, 0, 0);
            this.footerLayout.Controls.Add(this.buttonFlow, 1, 0);
            this.footerLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.footerLayout.Location = new System.Drawing.Point(0, 592);
            this.footerLayout.Margin = new System.Windows.Forms.Padding(0);
            this.footerLayout.Name = "footerLayout";
            this.footerLayout.RowCount = 1;
            this.footerLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.footerLayout.Size = new System.Drawing.Size(640, 72);
            this.footerLayout.TabIndex = 3;
            //
            // lblStepInfo —— 「第 N 步，共 M 步」
            //
            this.lblStepInfo.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblStepInfo.AutoSize = true;
            this.lblStepInfo.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.lblStepInfo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(163)))), ((int)(((byte)(175)))));
            this.lblStepInfo.Location = new System.Drawing.Point(32, 27);
            this.lblStepInfo.Margin = new System.Windows.Forms.Padding(32, 0, 0, 0);
            this.lblStepInfo.Name = "lblStepInfo";
            this.lblStepInfo.Size = new System.Drawing.Size(120, 19);
            this.lblStepInfo.TabIndex = 0;
            this.lblStepInfo.Text = "第 1 步，共 5 步";
            //
            // buttonFlow —— 右对齐按钮组
            //
            this.buttonFlow.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.buttonFlow.Controls.Add(this.btnPrimary);
            this.buttonFlow.Controls.Add(this.btnBack);
            this.buttonFlow.Controls.Add(this.btnCancel);
            this.buttonFlow.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.buttonFlow.Location = new System.Drawing.Point(370, 16);
            this.buttonFlow.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.buttonFlow.Name = "buttonFlow";
            this.buttonFlow.Size = new System.Drawing.Size(246, 40);
            this.buttonFlow.TabIndex = 1;
            this.buttonFlow.WrapContents = false;
            //
            // btnPrimary —— 下一步 / 安装 / 完成 / 卸载
            //
            this.btnPrimary.Location = new System.Drawing.Point(0, 0);
            this.btnPrimary.Margin = new System.Windows.Forms.Padding(12, 0, 0, 0);
            this.btnPrimary.Name = "btnPrimary";
            this.btnPrimary.Radius = 8;
            this.btnPrimary.Size = new System.Drawing.Size(120, 40);
            this.btnPrimary.TabIndex = 0;
            this.btnPrimary.Text = "下一步";
            this.btnPrimary.IconSvg = "<svg viewBox=\"0 0 16 16\"><path d=\"M3 8h9.4M8.6 4.2 12.8 8l-4.2 3.8\" fill=\"none\" stroke=\"#FFFFFF\" stroke-width=\"1.6\" stroke-linecap=\"round\" stroke-linejoin=\"round\"/></svg>";
            this.btnPrimary.Type = AntdUI.TTypeMini.Primary;
            this.btnPrimary.Click += new System.EventHandler(this.BtnPrimaryClick);
            //
            // btnBack —— 上一步
            //
            this.btnBack.BorderWidth = 1F;
            this.btnBack.DefaultBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(232)))), ((int)(((byte)(239)))));
            this.btnBack.Location = new System.Drawing.Point(126, 0);
            this.btnBack.Margin = new System.Windows.Forms.Padding(12, 0, 0, 0);
            this.btnBack.Name = "btnBack";
            this.btnBack.Radius = 8;
            this.btnBack.Size = new System.Drawing.Size(100, 40);
            this.btnBack.TabIndex = 1;
            this.btnBack.Text = "上一步";
            this.btnBack.Click += new System.EventHandler(this.BtnBackClick);
            //
            // btnCancel —— 取消
            //
            this.btnCancel.BorderWidth = 1F;
            this.btnCancel.DefaultBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(232)))), ((int)(((byte)(239)))));
            this.btnCancel.Location = new System.Drawing.Point(238, 0);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(12, 0, 0, 0);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Radius = 8;
            this.btnCancel.Size = new System.Drawing.Size(100, 40);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "取消";
            this.btnCancel.Click += new System.EventHandler(this.BtnCancelClick);
            //
            // WizardForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(247)))), ((int)(((byte)(250)))));
            this.ClientSize = new System.Drawing.Size(688, 712);
            this.Controls.Add(this.card);
            this.Font = new System.Drawing.Font("微软雅黑", 12F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MinimumSize = new System.Drawing.Size(688, 712);
            this.Name = "WizardForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "安装向导";
            this.card.ResumeLayout(false);
            this.cardLayout.ResumeLayout(false);
            this.footerLayout.ResumeLayout(false);
            this.footerLayout.PerformLayout();
            this.buttonFlow.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private AntdUI.Panel card;
        private System.Windows.Forms.TableLayoutPanel cardLayout;
        private Installer.UI.Views.WizardHeaderView headerView;
        private Installer.UI.Views.StepBarView stepBar;
        private System.Windows.Forms.Panel pnlContent;
        private System.Windows.Forms.TableLayoutPanel footerLayout;
        private AntdUI.Label lblStepInfo;
        private System.Windows.Forms.FlowLayoutPanel buttonFlow;
        private AntdUI.Button btnPrimary;
        private AntdUI.Button btnBack;
        private AntdUI.Button btnCancel;
    }
}
