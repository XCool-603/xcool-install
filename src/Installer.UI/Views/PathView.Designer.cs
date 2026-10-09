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
            this.pathSection = new System.Windows.Forms.TableLayoutPanel();
            this.lblPathCaption = new AntdUI.Label();
            this.pathRow = new System.Windows.Forms.TableLayoutPanel();
            this.txtPath = new AntdUI.Input();
            this.btnBrowse = new AntdUI.Button();
            this.lblSpace = new AntdUI.Label();
            this.chipFlow = new System.Windows.Forms.FlowLayoutPanel();
            this.btnChip1 = new AntdUI.Button();
            this.btnChip2 = new AntdUI.Button();
            this.btnChip3 = new AntdUI.Button();
            this.btnChip4 = new AntdUI.Button();
            this.divider = new System.Windows.Forms.Panel();
            this.taskSection = new System.Windows.Forms.TableLayoutPanel();
            this.lblTaskCaption = new AntdUI.Label();
            this.taskGrid = new System.Windows.Forms.TableLayoutPanel();
            this.cardDesktop = new Installer.UI.Views.TaskCardView();
            this.cardStartMenu = new Installer.UI.Views.TaskCardView();
            this.cardAutostart = new Installer.UI.Views.TaskCardView();
            this.cardRunAfter = new Installer.UI.Views.TaskCardView();
            this.allUsersRow = new System.Windows.Forms.TableLayoutPanel();
            this.chkAllUsers = new AntdUI.Checkbox();
            this.lblAllUsersHint = new AntdUI.Label();
            this.infoBar = new AntdUI.Panel();
            this.lblInfo = new AntdUI.Label();
            this.rootLayout.SuspendLayout();
            this.pathSection.SuspendLayout();
            this.pathRow.SuspendLayout();
            this.chipFlow.SuspendLayout();
            this.taskSection.SuspendLayout();
            this.taskGrid.SuspendLayout();
            this.allUsersRow.SuspendLayout();
            this.infoBar.SuspendLayout();
            this.SuspendLayout();
            //
            // rootLayout
            //
            this.rootLayout.ColumnCount = 1;
            this.rootLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rootLayout.Controls.Add(this.lblTitle, 0, 0);
            this.rootLayout.Controls.Add(this.lblSubtitle, 0, 1);
            this.rootLayout.Controls.Add(this.pathSection, 0, 2);
            this.rootLayout.Controls.Add(this.divider, 0, 3);
            this.rootLayout.Controls.Add(this.taskSection, 0, 4);
            this.rootLayout.Controls.Add(this.allUsersRow, 0, 5);
            this.rootLayout.Controls.Add(this.infoBar, 0, 6);
            this.rootLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rootLayout.Location = new System.Drawing.Point(0, 0);
            this.rootLayout.Name = "rootLayout";
            this.rootLayout.RowCount = 8;
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 126F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 5F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 174F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rootLayout.Size = new System.Drawing.Size(600, 440);
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
            this.lblTitle.Size = new System.Drawing.Size(300, 30);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "选择安装位置与附加任务";
            //
            // lblSubtitle
            //
            this.lblSubtitle.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblSubtitle.AutoSize = true;
            this.lblSubtitle.Font = new System.Drawing.Font("微软雅黑", 12F);
            this.lblSubtitle.Location = new System.Drawing.Point(0, 38);
            this.lblSubtitle.Margin = new System.Windows.Forms.Padding(0);
            this.lblSubtitle.Name = "lblSubtitle";
            this.lblSubtitle.Size = new System.Drawing.Size(400, 21);
            this.lblSubtitle.TabIndex = 1;
            this.lblSubtitle.Text = "";
            //
            // pathSection
            //
            this.pathSection.ColumnCount = 1;
            this.pathSection.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.pathSection.Controls.Add(this.lblPathCaption, 0, 0);
            this.pathSection.Controls.Add(this.pathRow, 0, 1);
            this.pathSection.Controls.Add(this.lblSpace, 0, 2);
            this.pathSection.Controls.Add(this.chipFlow, 0, 3);
            this.pathSection.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pathSection.Location = new System.Drawing.Point(0, 60);
            this.pathSection.Margin = new System.Windows.Forms.Padding(0);
            this.pathSection.Name = "pathSection";
            this.pathSection.RowCount = 4;
            this.pathSection.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.pathSection.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 44F));
            this.pathSection.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 22F));
            this.pathSection.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34F));
            this.pathSection.Size = new System.Drawing.Size(600, 126);
            this.pathSection.TabIndex = 2;
            //
            // lblPathCaption
            //
            this.lblPathCaption.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblPathCaption.AutoSize = true;
            this.lblPathCaption.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold);
            this.lblPathCaption.IconGap = 6;
            this.lblPathCaption.Location = new System.Drawing.Point(0, 2);
            this.lblPathCaption.Margin = new System.Windows.Forms.Padding(0);
            this.lblPathCaption.Name = "lblPathCaption";
            this.lblPathCaption.PrefixSvg = "<svg viewBox=\"0 0 16 16\"><path d=\"M1.8 3.6h4.2l1.6 1.6h6.6v7.2H1.8z\" fill=\"none\" stroke=\"#1677FF\" stroke-width=\"1.3\"/></svg>";
            this.lblPathCaption.Size = new System.Drawing.Size(120, 21);
            this.lblPathCaption.TabIndex = 0;
            this.lblPathCaption.Text = "安装位置";
            //
            // pathRow
            //
            this.pathRow.ColumnCount = 2;
            this.pathRow.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.pathRow.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 96F));
            this.pathRow.Controls.Add(this.txtPath, 0, 0);
            this.pathRow.Controls.Add(this.btnBrowse, 1, 0);
            this.pathRow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pathRow.Location = new System.Drawing.Point(0, 26);
            this.pathRow.Margin = new System.Windows.Forms.Padding(0);
            this.pathRow.Name = "pathRow";
            this.pathRow.RowCount = 1;
            this.pathRow.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.pathRow.Size = new System.Drawing.Size(600, 44);
            this.pathRow.TabIndex = 1;
            //
            // txtPath
            //
            this.txtPath.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.txtPath.AutoSize = false;
            this.txtPath.Location = new System.Drawing.Point(0, 2);
            this.txtPath.Margin = new System.Windows.Forms.Padding(0, 2, 12, 2);
            this.txtPath.Name = "txtPath";
            this.txtPath.PrefixSvg = "<svg viewBox=\"0 0 16 16\"><path d=\"M1.8 3.6h4.2l1.6 1.6h6.6v7.2H1.8z\" fill=\"none\" stroke=\"#9CA3AF\" stroke-width=\"1.3\"/></svg>";
            this.txtPath.Size = new System.Drawing.Size(492, 40);
            this.txtPath.TabIndex = 0;
            //
            // btnBrowse
            //
            this.btnBrowse.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnBrowse.BorderWidth = 1F;
            this.btnBrowse.DefaultBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(232)))), ((int)(((byte)(239)))));
            this.btnBrowse.IconGap = 0.3F;
            this.btnBrowse.IconSvg = "<svg viewBox=\"0 0 16 16\"><path d=\"M1.8 3.6h4.2l1.6 1.6h6.6v7.2H1.8z\" fill=\"none\" stroke=\"#4B5563\" stroke-width=\"1.3\"/></svg>";
            this.btnBrowse.Location = new System.Drawing.Point(504, 2);
            this.btnBrowse.Margin = new System.Windows.Forms.Padding(0);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Radius = 8;
            this.btnBrowse.Size = new System.Drawing.Size(96, 40);
            this.btnBrowse.TabIndex = 1;
            this.btnBrowse.Text = "浏览";
            //
            // lblSpace
            //
            this.lblSpace.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblSpace.AutoSize = true;
            this.lblSpace.Font = new System.Drawing.Font("微软雅黑", 11F);
            this.lblSpace.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(163)))), ((int)(((byte)(175)))));
            this.lblSpace.Location = new System.Drawing.Point(0, 70);
            this.lblSpace.Margin = new System.Windows.Forms.Padding(0);
            this.lblSpace.Name = "lblSpace";
            this.lblSpace.Size = new System.Drawing.Size(300, 19);
            this.lblSpace.TabIndex = 2;
            this.lblSpace.Text = "";
            //
            // chipFlow
            //
            this.chipFlow.Controls.Add(this.btnChip1);
            this.chipFlow.Controls.Add(this.btnChip2);
            this.chipFlow.Controls.Add(this.btnChip3);
            this.chipFlow.Controls.Add(this.btnChip4);
            this.chipFlow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chipFlow.Location = new System.Drawing.Point(0, 92);
            this.chipFlow.Margin = new System.Windows.Forms.Padding(0);
            this.chipFlow.Name = "chipFlow";
            this.chipFlow.Size = new System.Drawing.Size(600, 34);
            this.chipFlow.TabIndex = 3;
            this.chipFlow.WrapContents = false;
            //
            // btnChip1
            //
            this.btnChip1.Location = new System.Drawing.Point(0, 2);
            this.btnChip1.Margin = new System.Windows.Forms.Padding(0, 0, 8, 0);
            this.btnChip1.Name = "btnChip1";
            this.btnChip1.Radius = 6;
            this.btnChip1.AutoSize = false;
            this.btnChip1.Size = new System.Drawing.Size(120, 30);
            this.btnChip1.TabIndex = 0;
            this.btnChip1.Text = "Program Files";
            //
            // btnChip2
            //
            this.btnChip2.Location = new System.Drawing.Point(104, 0);
            this.btnChip2.Margin = new System.Windows.Forms.Padding(0, 0, 8, 0);
            this.btnChip2.Name = "btnChip2";
            this.btnChip2.Radius = 6;
            this.btnChip2.AutoSize = false;
            this.btnChip2.Size = new System.Drawing.Size(160, 30);
            this.btnChip2.TabIndex = 1;
            this.btnChip2.Text = "Program Files (x86)";
            //
            // btnChip3
            //
            this.btnChip3.Location = new System.Drawing.Point(238, 0);
            this.btnChip3.Margin = new System.Windows.Forms.Padding(0, 0, 8, 0);
            this.btnChip3.Name = "btnChip3";
            this.btnChip3.Radius = 6;
            this.btnChip3.AutoSize = false;
            this.btnChip3.Size = new System.Drawing.Size(120, 30);
            this.btnChip3.TabIndex = 2;
            this.btnChip3.Text = "本地 AppData";
            //
            // btnChip4
            //
            this.btnChip4.Location = new System.Drawing.Point(342, 0);
            this.btnChip4.Margin = new System.Windows.Forms.Padding(0, 0, 8, 0);
            this.btnChip4.Name = "btnChip4";
            this.btnChip4.Radius = 6;
            this.btnChip4.AutoSize = false;
            this.btnChip4.Size = new System.Drawing.Size(140, 30);
            this.btnChip4.TabIndex = 3;
            this.btnChip4.Text = "D:\\Program Files";
            //
            // divider
            //
            this.divider.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(232)))), ((int)(((byte)(239)))));
            this.divider.Dock = System.Windows.Forms.DockStyle.Fill;
            this.divider.Location = new System.Drawing.Point(0, 186);
            this.divider.Margin = new System.Windows.Forms.Padding(0, 8, 0, 8);
            this.divider.Name = "divider";
            this.divider.Size = new System.Drawing.Size(600, 1);
            this.divider.TabIndex = 3;
            //
            // taskSection
            //
            this.taskSection.ColumnCount = 1;
            this.taskSection.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.taskSection.Controls.Add(this.lblTaskCaption, 0, 0);
            this.taskSection.Controls.Add(this.taskGrid, 0, 1);
            this.taskSection.Dock = System.Windows.Forms.DockStyle.Fill;
            this.taskSection.Location = new System.Drawing.Point(0, 191);
            this.taskSection.Margin = new System.Windows.Forms.Padding(0);
            this.taskSection.Name = "taskSection";
            this.taskSection.RowCount = 2;
            this.taskSection.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.taskSection.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.taskSection.Size = new System.Drawing.Size(600, 174);
            this.taskSection.TabIndex = 4;
            //
            // lblTaskCaption
            //
            this.lblTaskCaption.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblTaskCaption.AutoSize = true;
            this.lblTaskCaption.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold);
            this.lblTaskCaption.IconGap = 6;
            this.lblTaskCaption.Location = new System.Drawing.Point(0, 2);
            this.lblTaskCaption.Margin = new System.Windows.Forms.Padding(0);
            this.lblTaskCaption.Name = "lblTaskCaption";
            this.lblTaskCaption.PrefixSvg = "<svg viewBox=\"0 0 16 16\"><rect x=\"2\" y=\"2.5\" width=\"5\" height=\"5\" rx=\"1\" fill=\"#1677FF\"/><rect x=\"9\" y=\"2.5\" width=\"5\" height=\"5\" rx=\"1\" fill=\"#1677FF\"/><rect x=\"2\" y=\"9\" width=\"5\" height=\"5\" rx=\"1\" fill=\"#1677FF\"/><rect x=\"9\" y=\"9\" width=\"5\" height=\"5\" rx=\"1\" fill=\"#1677FF\"/></svg>";
            this.lblTaskCaption.Size = new System.Drawing.Size(120, 21);
            this.lblTaskCaption.TabIndex = 0;
            this.lblTaskCaption.Text = "附加任务";
            //
            // taskGrid
            //
            this.taskGrid.ColumnCount = 2;
            this.taskGrid.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.taskGrid.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.taskGrid.Controls.Add(this.cardDesktop, 0, 0);
            this.taskGrid.Controls.Add(this.cardStartMenu, 1, 0);
            this.taskGrid.Controls.Add(this.cardAutostart, 0, 1);
            this.taskGrid.Controls.Add(this.cardRunAfter, 1, 1);
            this.taskGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.taskGrid.Location = new System.Drawing.Point(0, 26);
            this.taskGrid.Margin = new System.Windows.Forms.Padding(0);
            this.taskGrid.Name = "taskGrid";
            this.taskGrid.RowCount = 2;
            this.taskGrid.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.taskGrid.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.taskGrid.Size = new System.Drawing.Size(600, 148);
            this.taskGrid.TabIndex = 1;
            //
            // cardDesktop
            //
            this.cardDesktop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cardDesktop.Location = new System.Drawing.Point(0, 0);
            this.cardDesktop.Margin = new System.Windows.Forms.Padding(0, 0, 6, 5);
            this.cardDesktop.Name = "cardDesktop";
            this.cardDesktop.Size = new System.Drawing.Size(294, 70);
            this.cardDesktop.TabIndex = 0;
            //
            // cardStartMenu
            //
            this.cardStartMenu.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cardStartMenu.Location = new System.Drawing.Point(300, 0);
            this.cardStartMenu.Margin = new System.Windows.Forms.Padding(6, 0, 0, 5);
            this.cardStartMenu.Name = "cardStartMenu";
            this.cardStartMenu.Size = new System.Drawing.Size(300, 70);
            this.cardStartMenu.TabIndex = 1;
            //
            // cardAutostart
            //
            this.cardAutostart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cardAutostart.Location = new System.Drawing.Point(0, 75);
            this.cardAutostart.Margin = new System.Windows.Forms.Padding(0, 5, 6, 0);
            this.cardAutostart.Name = "cardAutostart";
            this.cardAutostart.Size = new System.Drawing.Size(294, 75);
            this.cardAutostart.TabIndex = 2;
            //
            // cardRunAfter
            //
            this.cardRunAfter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cardRunAfter.Location = new System.Drawing.Point(300, 75);
            this.cardRunAfter.Margin = new System.Windows.Forms.Padding(6, 5, 0, 0);
            this.cardRunAfter.Name = "cardRunAfter";
            this.cardRunAfter.Size = new System.Drawing.Size(300, 75);
            this.cardRunAfter.TabIndex = 3;
            //
            // allUsersRow
            //
            this.allUsersRow.ColumnCount = 2;
            this.allUsersRow.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 150F));
            this.allUsersRow.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.allUsersRow.Controls.Add(this.chkAllUsers, 0, 0);
            this.allUsersRow.Controls.Add(this.lblAllUsersHint, 1, 0);
            this.allUsersRow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.allUsersRow.Location = new System.Drawing.Point(0, 365);
            this.allUsersRow.Margin = new System.Windows.Forms.Padding(0);
            this.allUsersRow.Name = "allUsersRow";
            this.allUsersRow.RowCount = 1;
            this.allUsersRow.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.allUsersRow.Size = new System.Drawing.Size(600, 42);
            this.allUsersRow.TabIndex = 5;
            //
            // chkAllUsers
            //
            this.chkAllUsers.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.chkAllUsers.AutoSize = true;
            this.chkAllUsers.Font = new System.Drawing.Font("微软雅黑", 12F);
            this.chkAllUsers.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(41)))), ((int)(((byte)(55)))));
            this.chkAllUsers.Location = new System.Drawing.Point(0, 9);
            this.chkAllUsers.Margin = new System.Windows.Forms.Padding(0);
            this.chkAllUsers.Name = "chkAllUsers";
            this.chkAllUsers.Size = new System.Drawing.Size(140, 25);
            this.chkAllUsers.TabIndex = 0;
            this.chkAllUsers.Text = "为所有用户安装";
            //
            // lblAllUsersHint
            //
            this.lblAllUsersHint.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblAllUsersHint.AutoSize = true;
            this.lblAllUsersHint.Font = new System.Drawing.Font("微软雅黑", 11F);
            this.lblAllUsersHint.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(163)))), ((int)(((byte)(175)))));
            this.lblAllUsersHint.Location = new System.Drawing.Point(150, 11);
            this.lblAllUsersHint.Margin = new System.Windows.Forms.Padding(0);
            this.lblAllUsersHint.Name = "lblAllUsersHint";
            this.lblAllUsersHint.Size = new System.Drawing.Size(400, 19);
            this.lblAllUsersHint.TabIndex = 1;
            this.lblAllUsersHint.Text = "";
            //
            // infoBar
            //
            this.infoBar.Controls.Add(this.lblInfo);
            this.infoBar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.infoBar.Location = new System.Drawing.Point(0, 403);
            this.infoBar.Margin = new System.Windows.Forms.Padding(0);
            this.infoBar.Name = "infoBar";
            this.infoBar.Padding = new System.Windows.Forms.Padding(14, 6, 14, 6);
            this.infoBar.Radius = 8;
            this.infoBar.Size = new System.Drawing.Size(600, 40);
            this.infoBar.TabIndex = 6;
            //
            // lblInfo
            //
            this.lblInfo.AutoSize = false;
            this.lblInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblInfo.Font = new System.Drawing.Font("微软雅黑", 11F);
            this.lblInfo.Location = new System.Drawing.Point(14, 9);
            this.lblInfo.Margin = new System.Windows.Forms.Padding(0);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(572, 22);
            this.lblInfo.TabIndex = 0;
            this.lblInfo.Text = "";
            //
            // PathView
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.rootLayout);
            this.Font = new System.Drawing.Font("微软雅黑", 12F);
            this.Name = "PathView";
            this.Size = new System.Drawing.Size(600, 440);
            this.rootLayout.ResumeLayout(false);
            this.rootLayout.PerformLayout();
            this.pathSection.ResumeLayout(false);
            this.pathSection.PerformLayout();
            this.pathRow.ResumeLayout(false);
            this.chipFlow.ResumeLayout(false);
            this.taskSection.ResumeLayout(false);
            this.taskSection.PerformLayout();
            this.taskGrid.ResumeLayout(false);
            this.allUsersRow.ResumeLayout(false);
            this.allUsersRow.PerformLayout();
            this.infoBar.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel rootLayout;
        private AntdUI.Label lblTitle;
        private AntdUI.Label lblSubtitle;
        private System.Windows.Forms.TableLayoutPanel pathSection;
        private AntdUI.Label lblPathCaption;
        private System.Windows.Forms.TableLayoutPanel pathRow;
        private AntdUI.Input txtPath;
        private AntdUI.Button btnBrowse;
        private AntdUI.Label lblSpace;
        private System.Windows.Forms.FlowLayoutPanel chipFlow;
        private AntdUI.Button btnChip1;
        private AntdUI.Button btnChip2;
        private AntdUI.Button btnChip3;
        private AntdUI.Button btnChip4;
        private System.Windows.Forms.Panel divider;
        private System.Windows.Forms.TableLayoutPanel taskSection;
        private AntdUI.Label lblTaskCaption;
        private System.Windows.Forms.TableLayoutPanel taskGrid;
        private Installer.UI.Views.TaskCardView cardDesktop;
        private Installer.UI.Views.TaskCardView cardStartMenu;
        private Installer.UI.Views.TaskCardView cardAutostart;
        private Installer.UI.Views.TaskCardView cardRunAfter;
        private System.Windows.Forms.TableLayoutPanel allUsersRow;
        private AntdUI.Checkbox chkAllUsers;
        private AntdUI.Label lblAllUsersHint;
        private AntdUI.Panel infoBar;
        private AntdUI.Label lblInfo;
    }
}