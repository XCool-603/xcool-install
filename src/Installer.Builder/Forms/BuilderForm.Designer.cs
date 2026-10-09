namespace Installer.Builder.Forms
{
    partial class BuilderForm
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
            this.headerRight = new System.Windows.Forms.FlowLayoutPanel();
            this.cboLang = new AntdUI.Select();
            this.btnSave = new AntdUI.Button();
            this.btnOpen = new AntdUI.Button();
            this.contentLayout = new System.Windows.Forms.TableLayoutPanel();
            this.card1 = new AntdUI.Panel();
            this.folderStep = new Installer.Builder.Views.FolderStepView();
            this.card2 = new AntdUI.Panel();
            this.infoStep = new Installer.Builder.Views.InfoStepView();
            this.card3 = new AntdUI.Panel();
            this.shortcutStep = new Installer.Builder.Views.ShortcutStepView();
            this.btnAdvanced = new AntdUI.Button();
            this.footerLayout = new System.Windows.Forms.TableLayoutPanel();
            this.divider = new System.Windows.Forms.Panel();
            this.outputBar = new Installer.Builder.Views.OutputBar();
            this.statusLayout = new System.Windows.Forms.TableLayoutPanel();
            this.lblStatus = new AntdUI.Label();
            this.progress = new AntdUI.Progress();
            this.pageHeader.SuspendLayout();
            this.headerRight.SuspendLayout();
            this.contentLayout.SuspendLayout();
            this.card1.SuspendLayout();
            this.card2.SuspendLayout();
            this.card3.SuspendLayout();
            this.footerLayout.SuspendLayout();
            this.statusLayout.SuspendLayout();
            this.SuspendLayout();
            //
            // pageHeader —— 顶部栏：品牌 + 操作按钮 + 窗口按钮，全在一行（高 64）
            //
            this.pageHeader.Controls.Add(this.headerRight);
            this.pageHeader.DividerShow = true;
            this.pageHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pageHeader.DragMove = true;
            this.pageHeader.EnableDoubleClickMaximize = true;
            this.pageHeader.IconRatio = 1.6F;
            this.pageHeader.IconSvg = "<svg viewBox=\"0 0 28 28\"><rect width=\"28\" height=\"28\" rx=\"8\" fill=\"#1677FF\"/><path d=\"M14 6.5l-6.5 3.2v4.6c0 4 2.8 7.8 6.5 8.7 3.7-.9 6.5-4.7 6.5-8.7V9.7L14 6.5z\" fill=\"#FFFFFF\"/></svg>";
            this.pageHeader.Location = new System.Drawing.Point(0, 0);
            this.pageHeader.Margin = new System.Windows.Forms.Padding(0);
            this.pageHeader.MaximizeBox = true;
            this.pageHeader.MinimizeBox = true;
            this.pageHeader.Name = "pageHeader";
            this.pageHeader.Padding = new System.Windows.Forms.Padding(24, 0, 0, 0);
            this.pageHeader.ShowButton = true;
            this.pageHeader.ShowIcon = true;
            this.pageHeader.Size = new System.Drawing.Size(920, 64);
            this.pageHeader.TabIndex = 0;
            this.pageHeader.Text = "安装包制作助手";
            this.pageHeader.UseTitleFont = true;
            //
            // headerRight —— 挂在 PageHeader 里；DisplayRectangle 会自动避开窗口按钮
            //
            this.headerRight.Controls.Add(this.cboLang);
            this.headerRight.Controls.Add(this.btnSave);
            this.headerRight.Controls.Add(this.btnOpen);
            this.headerRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.headerRight.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.headerRight.Location = new System.Drawing.Point(578, 0);
            this.headerRight.Margin = new System.Windows.Forms.Padding(0);
            this.headerRight.Name = "headerRight";
            this.headerRight.Size = new System.Drawing.Size(342, 64);
            this.headerRight.TabIndex = 0;
            this.headerRight.WrapContents = false;
            //
            // cboLang
            //
            this.cboLang.Location = new System.Drawing.Point(212, 14);
            this.cboLang.Margin = new System.Windows.Forms.Padding(6, 14, 0, 0);
            this.cboLang.Name = "cboLang";
            this.cboLang.Size = new System.Drawing.Size(130, 36);
            this.cboLang.TabIndex = 2;
            //
            // btnSave
            //
            this.btnSave.BorderWidth = 1F;
            this.btnSave.DefaultBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(232)))), ((int)(((byte)(239)))));
            this.btnSave.IconGap = 0.3F;
            this.btnSave.IconSvg = "<svg viewBox=\"0 0 16 16\"><path d=\"M2.5 2.5h8.2l2.8 2.8v8.2h-11z\" fill=\"none\" stroke=\"#1F2937\" stroke-width=\"1.3\"/><path d=\"M5 2.5h5.5v4H5z\" fill=\"none\" stroke=\"#1F2937\" stroke-width=\"1.3\"/></svg>";
            this.btnSave.Location = new System.Drawing.Point(118, 14);
            this.btnSave.Margin = new System.Windows.Forms.Padding(6, 14, 0, 0);
            this.btnSave.Name = "btnSave";
            this.btnSave.Radius = 8;
            this.btnSave.Size = new System.Drawing.Size(88, 36);
            this.btnSave.TabIndex = 1;
            this.btnSave.Text = "保存";
            //
            // btnOpen
            //
            this.btnOpen.BorderWidth = 1F;
            this.btnOpen.DefaultBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(232)))), ((int)(((byte)(239)))));
            this.btnOpen.IconGap = 0.3F;
            this.btnOpen.IconSvg = "<svg viewBox=\"0 0 16 16\"><path d=\"M1.8 3.6h4.2l1.6 1.6h6.6v7.2H1.8z\" fill=\"none\" stroke=\"#1F2937\" stroke-width=\"1.3\"/></svg>";
            this.btnOpen.Location = new System.Drawing.Point(24, 14);
            this.btnOpen.Margin = new System.Windows.Forms.Padding(6, 14, 0, 0);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Radius = 8;
            this.btnOpen.Size = new System.Drawing.Size(88, 36);
            this.btnOpen.TabIndex = 0;
            this.btnOpen.Text = "打开";
            //
            // contentLayout —— 卡片区
            //
            this.contentLayout.ColumnCount = 1;
            this.contentLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.contentLayout.Controls.Add(this.card1, 0, 0);
            this.contentLayout.Controls.Add(this.card2, 0, 1);
            this.contentLayout.Controls.Add(this.card3, 0, 2);
            this.contentLayout.Controls.Add(this.btnAdvanced, 0, 3);
            this.contentLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.contentLayout.Location = new System.Drawing.Point(0, 64);
            this.contentLayout.Name = "contentLayout";
            this.contentLayout.Padding = new System.Windows.Forms.Padding(24, 20, 24, 0);
            this.contentLayout.RowCount = 5;
            this.contentLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 354F));
            this.contentLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 194F));
            this.contentLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 126F));
            this.contentLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 44F));
            this.contentLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.contentLayout.Size = new System.Drawing.Size(920, 738);
            this.contentLayout.TabIndex = 1;
            //
            // card1
            //
            this.card1.BackColor = System.Drawing.Color.White;
            this.card1.BorderWidth = 0F;
            this.card1.Controls.Add(this.folderStep);
            this.card1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.card1.Location = new System.Drawing.Point(24, 20);
            this.card1.Margin = new System.Windows.Forms.Padding(0, 0, 0, 16);
            this.card1.Name = "card1";
            this.card1.Padding = new System.Windows.Forms.Padding(20);
            this.card1.Radius = 14;
            this.card1.Shadow = 8;
            this.card1.ShadowOpacity = 0.08F;
            this.card1.Size = new System.Drawing.Size(872, 338);
            this.card1.TabIndex = 0;
            //
            // folderStep
            //
            this.folderStep.BackColor = System.Drawing.Color.White;
            this.folderStep.Dock = System.Windows.Forms.DockStyle.Fill;
            this.folderStep.Location = new System.Drawing.Point(20, 20);
            this.folderStep.Margin = new System.Windows.Forms.Padding(0);
            this.folderStep.Name = "folderStep";
            this.folderStep.Size = new System.Drawing.Size(832, 298);
            this.folderStep.TabIndex = 0;
            //
            // card2
            //
            this.card2.BackColor = System.Drawing.Color.White;
            this.card2.BorderWidth = 0F;
            this.card2.Controls.Add(this.infoStep);
            this.card2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.card2.Location = new System.Drawing.Point(24, 374);
            this.card2.Margin = new System.Windows.Forms.Padding(0, 0, 0, 16);
            this.card2.Name = "card2";
            this.card2.Padding = new System.Windows.Forms.Padding(20);
            this.card2.Radius = 14;
            this.card2.Shadow = 8;
            this.card2.ShadowOpacity = 0.08F;
            this.card2.Size = new System.Drawing.Size(872, 178);
            this.card2.TabIndex = 1;
            //
            // infoStep
            //
            this.infoStep.BackColor = System.Drawing.Color.White;
            this.infoStep.Dock = System.Windows.Forms.DockStyle.Fill;
            this.infoStep.Location = new System.Drawing.Point(20, 20);
            this.infoStep.Margin = new System.Windows.Forms.Padding(0);
            this.infoStep.Name = "infoStep";
            this.infoStep.Size = new System.Drawing.Size(832, 138);
            this.infoStep.TabIndex = 0;
            //
            // card3
            //
            this.card3.BackColor = System.Drawing.Color.White;
            this.card3.BorderWidth = 0F;
            this.card3.Controls.Add(this.shortcutStep);
            this.card3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.card3.Location = new System.Drawing.Point(24, 568);
            this.card3.Margin = new System.Windows.Forms.Padding(0, 0, 0, 12);
            this.card3.Name = "card3";
            this.card3.Padding = new System.Windows.Forms.Padding(20);
            this.card3.Radius = 14;
            this.card3.Shadow = 8;
            this.card3.ShadowOpacity = 0.08F;
            this.card3.Size = new System.Drawing.Size(872, 102);
            this.card3.TabIndex = 2;
            //
            // shortcutStep
            //
            this.shortcutStep.BackColor = System.Drawing.Color.White;
            this.shortcutStep.Dock = System.Windows.Forms.DockStyle.Fill;
            this.shortcutStep.Location = new System.Drawing.Point(20, 20);
            this.shortcutStep.Margin = new System.Windows.Forms.Padding(0);
            this.shortcutStep.Name = "shortcutStep";
            this.shortcutStep.Size = new System.Drawing.Size(832, 62);
            this.shortcutStep.TabIndex = 0;
            //
            // btnAdvanced
            //
            this.btnAdvanced.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnAdvanced.BorderWidth = 1F;
            this.btnAdvanced.DefaultBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(232)))), ((int)(((byte)(239)))));
            this.btnAdvanced.IconGap = 0.35F;
            this.btnAdvanced.IconSvg = "<svg viewBox=\"0 0 16 16\"><path d=\"M2 4.5h12M2 8h12M2 11.5h12\" stroke=\"#4B5563\" stroke-width=\"1.3\" stroke-linecap=\"round\"/><circle cx=\"5.5\" cy=\"4.5\" r=\"1.9\" fill=\"#FFFFFF\" stroke=\"#4B5563\" stroke-width=\"1.3\"/><circle cx=\"10.5\" cy=\"8\" r=\"1.9\" fill=\"#FFFFFF\" stroke=\"#4B5563\" stroke-width=\"1.3\"/><circle cx=\"6.5\" cy=\"11.5\" r=\"1.9\" fill=\"#FFFFFF\" stroke=\"#4B5563\" stroke-width=\"1.3\"/></svg>";
            this.btnAdvanced.Location = new System.Drawing.Point(24, 682);
            this.btnAdvanced.Margin = new System.Windows.Forms.Padding(0);
            this.btnAdvanced.Name = "btnAdvanced";
            this.btnAdvanced.Radius = 8;
            this.btnAdvanced.Size = new System.Drawing.Size(110, 36);
            this.btnAdvanced.TabIndex = 3;
            this.btnAdvanced.Text = "高级选项";
            //
            // footerLayout —— 底部栏 88 + 状态栏 27，上方 1px 分隔线
            //
            this.footerLayout.ColumnCount = 1;
            this.footerLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.footerLayout.Controls.Add(this.divider, 0, 0);
            this.footerLayout.Controls.Add(this.outputBar, 0, 1);
            this.footerLayout.Controls.Add(this.statusLayout, 0, 2);
            this.footerLayout.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.footerLayout.Location = new System.Drawing.Point(0, 802);
            this.footerLayout.Margin = new System.Windows.Forms.Padding(0);
            this.footerLayout.Name = "footerLayout";
            this.footerLayout.RowCount = 3;
            this.footerLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 1F));
            this.footerLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 88F));
            this.footerLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27F));
            this.footerLayout.Size = new System.Drawing.Size(920, 116);
            this.footerLayout.TabIndex = 2;
            //
            // divider
            //
            this.divider.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(232)))), ((int)(((byte)(239)))));
            this.divider.Dock = System.Windows.Forms.DockStyle.Fill;
            this.divider.Location = new System.Drawing.Point(0, 0);
            this.divider.Margin = new System.Windows.Forms.Padding(0);
            this.divider.Name = "divider";
            this.divider.Size = new System.Drawing.Size(920, 1);
            this.divider.TabIndex = 0;
            //
            // outputBar
            //
            this.outputBar.BackColor = System.Drawing.Color.White;
            this.outputBar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.outputBar.Location = new System.Drawing.Point(0, 1);
            this.outputBar.Margin = new System.Windows.Forms.Padding(0);
            this.outputBar.Name = "outputBar";
            this.outputBar.Size = new System.Drawing.Size(920, 88);
            this.outputBar.TabIndex = 1;
            //
            // statusLayout
            //
            this.statusLayout.ColumnCount = 2;
            this.statusLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.statusLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 320F));
            this.statusLayout.Controls.Add(this.lblStatus, 0, 0);
            this.statusLayout.Controls.Add(this.progress, 1, 0);
            this.statusLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.statusLayout.Location = new System.Drawing.Point(0, 89);
            this.statusLayout.Margin = new System.Windows.Forms.Padding(0);
            this.statusLayout.Name = "statusLayout";
            this.statusLayout.RowCount = 1;
            this.statusLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.statusLayout.Size = new System.Drawing.Size(920, 27);
            this.statusLayout.TabIndex = 2;
            //
            // lblStatus
            //
            this.lblStatus.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblStatus.AutoSize = true;
            this.lblStatus.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.lblStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(163)))), ((int)(((byte)(175)))));
            this.lblStatus.Location = new System.Drawing.Point(24, 5);
            this.lblStatus.Margin = new System.Windows.Forms.Padding(24, 0, 0, 0);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(200, 17);
            this.lblStatus.TabIndex = 0;
            this.lblStatus.Text = "选一个文件夹就可以开始了";
            //
            // progress
            //
            this.progress.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.progress.Location = new System.Drawing.Point(576, 1);
            this.progress.Margin = new System.Windows.Forms.Padding(0, 0, 24, 0);
            this.progress.Name = "progress";
            this.progress.Radius = 3;
            this.progress.Shape = AntdUI.TShapeProgress.Default;
            this.progress.Size = new System.Drawing.Size(320, 25);
            this.progress.TabIndex = 1;
            this.progress.Text = "0%";
            this.progress.Value = 0F;
            this.progress.Visible = false;
            //
            // BuilderForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(247)))), ((int)(((byte)(250)))));
            this.ClientSize = new System.Drawing.Size(920, 918);
            this.Controls.Add(this.contentLayout);
            this.Controls.Add(this.footerLayout);
            this.Controls.Add(this.pageHeader);
            this.Font = new System.Drawing.Font("微软雅黑", 9.75F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MinimumSize = new System.Drawing.Size(860, 880);
            this.Name = "BuilderForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "安装包制作助手";
            this.pageHeader.ResumeLayout(false);
            this.headerRight.ResumeLayout(false);
            this.contentLayout.ResumeLayout(false);
            this.card1.ResumeLayout(false);
            this.card2.ResumeLayout(false);
            this.card3.ResumeLayout(false);
            this.footerLayout.ResumeLayout(false);
            this.statusLayout.ResumeLayout(false);
            this.statusLayout.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        private AntdUI.PageHeader pageHeader;
        private System.Windows.Forms.FlowLayoutPanel headerRight;
        private AntdUI.Select cboLang;
        private AntdUI.Button btnSave;
        private AntdUI.Button btnOpen;
        private System.Windows.Forms.TableLayoutPanel contentLayout;
        private AntdUI.Panel card1;
        private Installer.Builder.Views.FolderStepView folderStep;
        private AntdUI.Panel card2;
        private Installer.Builder.Views.InfoStepView infoStep;
        private AntdUI.Panel card3;
        private Installer.Builder.Views.ShortcutStepView shortcutStep;
        private AntdUI.Button btnAdvanced;
        private System.Windows.Forms.TableLayoutPanel footerLayout;
        private System.Windows.Forms.Panel divider;
        private Installer.Builder.Views.OutputBar outputBar;
        private System.Windows.Forms.TableLayoutPanel statusLayout;
        private AntdUI.Label lblStatus;
        private AntdUI.Progress progress;
    }
}
