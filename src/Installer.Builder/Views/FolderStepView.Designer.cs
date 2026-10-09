namespace Installer.Builder.Views
{
    partial class FolderStepView
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
            this.lblStep = new AntdUI.Label();
            this.dropPanel = new AntdUI.Panel();
            this.dropLayout = new System.Windows.Forms.TableLayoutPanel();
            this.lblDropIcon = new AntdUI.Label();
            this.lblDropText = new AntdUI.Label();
            this.lblDropSub = new AntdUI.Label();
            this.btnBrowse = new AntdUI.Button();
            this.pickedLayout = new System.Windows.Forms.TableLayoutPanel();
            this.lblPickedPath = new AntdUI.Label();
            this.btnChange = new AntdUI.Button();
            this.lblPickedStats = new AntdUI.Label();
            this.entryLayout = new System.Windows.Forms.TableLayoutPanel();
            this.lblEntry = new AntdUI.Label();
            this.cboEntry = new AntdUI.Select();
            this.lblEntryHint = new AntdUI.Label();
            this.chkAutoName = new AntdUI.Checkbox();
            this.rootLayout.SuspendLayout();
            this.dropPanel.SuspendLayout();
            this.dropLayout.SuspendLayout();
            this.pickedLayout.SuspendLayout();
            this.entryLayout.SuspendLayout();
            this.SuspendLayout();
            //
            // rootLayout
            //
            this.rootLayout.ColumnCount = 1;
            this.rootLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rootLayout.Controls.Add(this.lblStep, 0, 0);
            this.rootLayout.Controls.Add(this.dropPanel, 0, 1);
            this.rootLayout.Controls.Add(this.entryLayout, 0, 2);
            this.rootLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rootLayout.Location = new System.Drawing.Point(0, 0);
            this.rootLayout.Name = "rootLayout";
            this.rootLayout.RowCount = 4;
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 140F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 46F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rootLayout.Size = new System.Drawing.Size(832, 286);
            this.rootLayout.TabIndex = 0;
            //
            // lblStep
            //
            this.lblStep.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblStep.AutoSize = true;
            this.lblStep.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Bold);
            this.lblStep.IconGap = 6;
            this.lblStep.Location = new System.Drawing.Point(0, 5);
            this.lblStep.Margin = new System.Windows.Forms.Padding(0);
            this.lblStep.Name = "lblStep";
            this.lblStep.PrefixSvg = "<svg viewBox=\"0 0 20 20\"><rect width=\"20\" height=\"20\" rx=\"6\" fill=\"#1677FF\"/><text x=\"10\" y=\"15\" font-size=\"13\" fill=\"#FFFFFF\" text-anchor=\"middle\">1</text></svg>";
            this.lblStep.Size = new System.Drawing.Size(200, 20);
            this.lblStep.TabIndex = 0;
            this.lblStep.Text = "选择要打包的文件夹";
            //
            // dropPanel
            //
            this.dropPanel.Back = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(244)))), ((int)(((byte)(255)))));
            this.dropPanel.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(119)))), ((int)(((byte)(255)))));
            this.dropPanel.BorderStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            this.dropPanel.BorderWidth = 1.5F;
            this.dropPanel.Controls.Add(this.dropLayout);
            this.dropPanel.Controls.Add(this.pickedLayout);
            this.dropPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dropPanel.Location = new System.Drawing.Point(0, 30);
            this.dropPanel.Margin = new System.Windows.Forms.Padding(0);
            this.dropPanel.Name = "dropPanel";
            this.dropPanel.Radius = 14;
            this.dropPanel.Size = new System.Drawing.Size(832, 200);
            this.dropPanel.TabIndex = 1;
            //
            // dropLayout —— 空态：图标 / 提示 / 副提示 / 按钮（竖排）
            //
            this.dropLayout.ColumnCount = 1;
            this.dropLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.dropLayout.Controls.Add(this.lblDropIcon, 0, 0);
            this.dropLayout.Controls.Add(this.lblDropText, 0, 1);
            this.dropLayout.Controls.Add(this.lblDropSub, 0, 2);
            this.dropLayout.Controls.Add(this.btnBrowse, 0, 3);
            this.dropLayout.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(244)))), ((int)(((byte)(255)))));
            this.dropLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dropLayout.Location = new System.Drawing.Point(0, 0);
            this.dropLayout.Margin = new System.Windows.Forms.Padding(0);
            this.dropLayout.Name = "dropLayout";
            this.dropLayout.RowCount = 4;
            this.dropLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34F));
            this.dropLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.dropLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 22F));
            this.dropLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.dropLayout.Size = new System.Drawing.Size(832, 200);
            this.dropLayout.TabIndex = 0;
            //
            // lblDropIcon
            //
            this.lblDropIcon.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblDropIcon.AutoSize = false;
            this.lblDropIcon.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.lblDropIcon.IconGap = 0;
            this.lblDropIcon.Location = new System.Drawing.Point(356, 3);
            this.lblDropIcon.Margin = new System.Windows.Forms.Padding(0);
            this.lblDropIcon.Name = "lblDropIcon";
            this.lblDropIcon.PrefixSvg = "<svg viewBox=\"0 0 24 24\"><path d=\"M9 16h6v-6h4l-7-7-7 7h4v6zm-4 2h14v2H5v-2z\" fill=\"#1677FF\"/></svg>";
            this.lblDropIcon.Size = new System.Drawing.Size(48, 28);
            this.lblDropIcon.TabIndex = 0;
            this.lblDropIcon.Text = " ";
            //
            // lblDropText
            //
            this.lblDropText.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblDropText.AutoSize = true;
            this.lblDropText.Font = new System.Drawing.Font("微软雅黑", 10.5F);
            this.lblDropText.Location = new System.Drawing.Point(310, 40);
            this.lblDropText.Margin = new System.Windows.Forms.Padding(0);
            this.lblDropText.Name = "lblDropText";
            this.lblDropText.Size = new System.Drawing.Size(140, 20);
            this.lblDropText.TabIndex = 1;
            this.lblDropText.Text = "把文件夹拖到这里";
            //
            // lblDropSub
            //
            this.lblDropSub.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblDropSub.AutoSize = true;
            this.lblDropSub.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.lblDropSub.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(163)))), ((int)(((byte)(175)))));
            this.lblDropSub.Location = new System.Drawing.Point(320, 67);
            this.lblDropSub.Margin = new System.Windows.Forms.Padding(0);
            this.lblDropSub.Name = "lblDropSub";
            this.lblDropSub.Size = new System.Drawing.Size(120, 17);
            this.lblDropSub.TabIndex = 2;
            this.lblDropSub.Text = "或者点下面的按钮";
            //
            // btnBrowse
            //
            this.btnBrowse.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnBrowse.Location = new System.Drawing.Point(320, 95);
            this.btnBrowse.Margin = new System.Windows.Forms.Padding(0, 4, 0, 4);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Radius = 8;
            this.btnBrowse.Size = new System.Drawing.Size(120, 38);
            this.btnBrowse.TabIndex = 3;
            this.btnBrowse.Text = "选择文件夹";
            this.btnBrowse.Type = AntdUI.TTypeMini.Primary;
            //
            // pickedLayout —— 已选态：路径 + 更换按钮（横排）
            //
            this.pickedLayout.ColumnCount = 2;
            this.pickedLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.pickedLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.pickedLayout.Controls.Add(this.lblPickedPath, 0, 0);
            this.pickedLayout.Controls.Add(this.btnChange, 1, 0);
            this.pickedLayout.Controls.Add(this.lblPickedStats, 0, 1);
            this.pickedLayout.SetColumnSpan(this.lblPickedStats, 2);
            this.pickedLayout.BackColor = System.Drawing.Color.White;
            this.pickedLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pickedLayout.Location = new System.Drawing.Point(0, 0);
            this.pickedLayout.Margin = new System.Windows.Forms.Padding(0);
            this.pickedLayout.Name = "pickedLayout";
            this.pickedLayout.RowCount = 1;
            this.pickedLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.pickedLayout.Size = new System.Drawing.Size(832, 200);
            this.pickedLayout.TabIndex = 1;
            this.pickedLayout.Visible = false;
            //
            // lblPickedPath
            //
            this.lblPickedPath.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblPickedPath.AutoEllipsis = true;
            this.lblPickedPath.AutoSize = false;
            this.lblPickedPath.Font = new System.Drawing.Font("微软雅黑", 10.5F);
            this.lblPickedPath.Location = new System.Drawing.Point(16, 0);
            this.lblPickedPath.Margin = new System.Windows.Forms.Padding(16, 0, 12, 0);
            this.lblPickedPath.Name = "lblPickedPath";
            this.lblPickedPath.Size = new System.Drawing.Size(660, 40);
            this.lblPickedPath.TabIndex = 0;
            this.lblPickedPath.Text = "";
            //
            // btnChange
            //
            this.btnChange.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnChange.BorderWidth = 1F;
            this.btnChange.DefaultBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(232)))), ((int)(((byte)(239)))));
            this.btnChange.Location = new System.Drawing.Point(716, 1);
            this.btnChange.Margin = new System.Windows.Forms.Padding(0, 0, 16, 0);
            this.btnChange.Name = "btnChange";
            this.btnChange.Radius = 8;
            this.btnChange.Size = new System.Drawing.Size(100, 38);
            this.btnChange.TabIndex = 1;
            this.btnChange.Text = "更换文件夹";
            //
            //
            // lblPickedStats
            //
            this.lblPickedStats.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblPickedStats.AutoSize = true;
            this.lblPickedStats.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.lblPickedStats.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(163)))), ((int)(((byte)(175)))));
            this.lblPickedStats.Location = new System.Drawing.Point(16, 44);
            this.lblPickedStats.Margin = new System.Windows.Forms.Padding(16, 0, 0, 0);
            this.lblPickedStats.Name = "lblPickedStats";
            this.lblPickedStats.Size = new System.Drawing.Size(200, 17);
            this.lblPickedStats.TabIndex = 1;
            this.lblPickedStats.Text = "";
            //
            //
            // entryLayout
            //
            this.entryLayout.ColumnCount = 4;
            this.entryLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 76F));
            this.entryLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 300F));
            this.entryLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.entryLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 200F));
            this.entryLayout.Controls.Add(this.lblEntry, 0, 0);
            this.entryLayout.Controls.Add(this.cboEntry, 1, 0);
            this.entryLayout.Controls.Add(this.lblEntryHint, 2, 0);
            this.entryLayout.Controls.Add(this.chkAutoName, 3, 0);
            this.entryLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.entryLayout.Location = new System.Drawing.Point(0, 230);
            this.entryLayout.Margin = new System.Windows.Forms.Padding(0);
            this.entryLayout.Name = "entryLayout";
            this.entryLayout.RowCount = 1;
            this.entryLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.entryLayout.Size = new System.Drawing.Size(760, 46);
            this.entryLayout.TabIndex = 3;
            //
            // lblEntry
            //
            this.lblEntry.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblEntry.AutoSize = true;
            this.lblEntry.Location = new System.Drawing.Point(0, 14);
            this.lblEntry.Margin = new System.Windows.Forms.Padding(0);
            this.lblEntry.Name = "lblEntry";
            this.lblEntry.Size = new System.Drawing.Size(60, 17);
            this.lblEntry.TabIndex = 0;
            this.lblEntry.Text = "启动程序";
            //
            // cboEntry
            //
            this.cboEntry.AutoSize = false;
            this.cboEntry.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.cboEntry.PlaceholderText = "选择启动程序";
            this.cboEntry.Location = new System.Drawing.Point(76, 6);
            this.cboEntry.Margin = new System.Windows.Forms.Padding(0, 6, 12, 6);
            this.cboEntry.Name = "cboEntry";
            this.cboEntry.Size = new System.Drawing.Size(288, 34);
            this.cboEntry.TabIndex = 1;
            //
            // lblEntryHint
            //
            this.lblEntryHint.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblEntryHint.AutoSize = true;
            this.lblEntryHint.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.lblEntryHint.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(163)))), ((int)(((byte)(175)))));
            this.lblEntryHint.Location = new System.Drawing.Point(376, 14);
            this.lblEntryHint.Margin = new System.Windows.Forms.Padding(0);
            this.lblEntryHint.Name = "lblEntryHint";
            this.lblEntryHint.Size = new System.Drawing.Size(200, 17);
            this.lblEntryHint.TabIndex = 2;
            this.lblEntryHint.Text = "";
            //
            // chkAutoName
            //
            this.chkAutoName.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.chkAutoName.AutoSize = true;
            this.chkAutoName.Checked = true;
            this.chkAutoName.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAutoName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(41)))), ((int)(((byte)(55)))));
            this.chkAutoName.Location = new System.Drawing.Point(576, 11);
            this.chkAutoName.Margin = new System.Windows.Forms.Padding(0);
            this.chkAutoName.Name = "chkAutoName";
            this.chkAutoName.Size = new System.Drawing.Size(180, 23);
            this.chkAutoName.TabIndex = 3;
            this.chkAutoName.Text = "产品名跟随文件夹名";
            //
            // FolderStepView
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.rootLayout);
            this.Font = new System.Drawing.Font("微软雅黑", 9.75F);
            this.Name = "FolderStepView";
            this.Size = new System.Drawing.Size(832, 286);
            this.rootLayout.ResumeLayout(false);
            this.rootLayout.PerformLayout();
            this.dropPanel.ResumeLayout(false);
            this.dropLayout.ResumeLayout(false);
            this.dropLayout.PerformLayout();
            this.pickedLayout.ResumeLayout(false);
            this.entryLayout.ResumeLayout(false);
            this.entryLayout.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel rootLayout;
        private AntdUI.Label lblStep;
        private AntdUI.Panel dropPanel;
        private System.Windows.Forms.TableLayoutPanel dropLayout;
        private AntdUI.Label lblDropIcon;
        private AntdUI.Label lblDropText;
        private AntdUI.Label lblDropSub;
        private AntdUI.Button btnBrowse;
        private System.Windows.Forms.TableLayoutPanel pickedLayout;
        private AntdUI.Label lblPickedPath;
        private AntdUI.Button btnChange;
        private AntdUI.Label lblPickedStats;
        private System.Windows.Forms.TableLayoutPanel entryLayout;
        private AntdUI.Label lblEntry;
        private AntdUI.Select cboEntry;
        private AntdUI.Label lblEntryHint;
        private AntdUI.Checkbox chkAutoName;
    }
}
