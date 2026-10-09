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

        #region 组件设计器生成的代码

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
            this.lblFiles = new AntdUI.Label();
            this.entryLayout = new System.Windows.Forms.TableLayoutPanel();
            this.lblEntry = new AntdUI.Label();
            this.cboEntry = new AntdUI.Select();
            this.lblEntryHint = new AntdUI.Label();
            this.chkAutoName = new AntdUI.Checkbox();
            this.rootLayout.SuspendLayout();
            this.dropPanel.SuspendLayout();
            this.dropLayout.SuspendLayout();
            this.entryLayout.SuspendLayout();
            this.SuspendLayout();
            //
            // rootLayout
            //
            this.rootLayout.ColumnCount = 1;
            this.rootLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rootLayout.Controls.Add(this.lblStep, 0, 0);
            this.rootLayout.Controls.Add(this.dropPanel, 0, 1);
            this.rootLayout.Controls.Add(this.lblFiles, 0, 2);
            this.rootLayout.Controls.Add(this.entryLayout, 0, 3);
            this.rootLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rootLayout.Location = new System.Drawing.Point(0, 0);
            this.rootLayout.Name = "rootLayout";
            this.rootLayout.RowCount = 5;
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 132F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 46F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rootLayout.Size = new System.Drawing.Size(760, 240);
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
            this.lblStep.Size = new System.Drawing.Size(200, 19);
            this.lblStep.TabIndex = 0;
            this.lblStep.Text = "① 选择要打包的文件夹";
            //
            // dropPanel
            //
            this.dropPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(251)))), ((int)(((byte)(253)))));
            this.dropPanel.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(200)))), ((int)(((byte)(215)))));
            this.dropPanel.BorderStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            this.dropPanel.BorderWidth = 1.5F;
            this.dropPanel.Controls.Add(this.dropLayout);
            this.dropPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dropPanel.Location = new System.Drawing.Point(0, 30);
            this.dropPanel.Margin = new System.Windows.Forms.Padding(0);
            this.dropPanel.Name = "dropPanel";
            this.dropPanel.Radius = 10;
            this.dropPanel.Size = new System.Drawing.Size(760, 132);
            this.dropPanel.TabIndex = 1;
            //
            // dropLayout
            //
            this.dropLayout.ColumnCount = 1;
            this.dropLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.dropLayout.Controls.Add(this.lblDropIcon, 0, 0);
            this.dropLayout.Controls.Add(this.lblDropText, 0, 1);
            this.dropLayout.Controls.Add(this.lblDropSub, 0, 2);
            this.dropLayout.Controls.Add(this.btnBrowse, 0, 3);
            this.dropLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dropLayout.Location = new System.Drawing.Point(0, 0);
            this.dropLayout.Margin = new System.Windows.Forms.Padding(0);
            this.dropLayout.Name = "dropLayout";
            this.dropLayout.RowCount = 4;
            this.dropLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34F));
            this.dropLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.dropLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.dropLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.dropLayout.Size = new System.Drawing.Size(760, 132);
            this.dropLayout.TabIndex = 0;
            //
            // lblDropIcon
            //
            this.lblDropIcon.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblDropIcon.AutoSize = true;
            this.lblDropIcon.Font = new System.Drawing.Font("Segoe UI Emoji", 15F);
            this.lblDropIcon.Location = new System.Drawing.Point(366, 1);
            this.lblDropIcon.Margin = new System.Windows.Forms.Padding(0);
            this.lblDropIcon.Name = "lblDropIcon";
            this.lblDropIcon.Size = new System.Drawing.Size(28, 27);
            this.lblDropIcon.TabIndex = 0;
            this.lblDropIcon.Text = "📁";
            //
            // lblDropText
            //
            this.lblDropText.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblDropText.AutoSize = true;
            this.lblDropText.Font = new System.Drawing.Font("微软雅黑", 10F);
            this.lblDropText.Location = new System.Drawing.Point(300, 39);
            this.lblDropText.Margin = new System.Windows.Forms.Padding(0);
            this.lblDropText.Name = "lblDropText";
            this.lblDropText.Size = new System.Drawing.Size(160, 19);
            this.lblDropText.TabIndex = 1;
            this.lblDropText.Text = "把文件夹拖到这里";
            //
            // lblDropSub
            //
            this.lblDropSub.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblDropSub.AutoSize = true;
            this.lblDropSub.Location = new System.Drawing.Point(330, 63);
            this.lblDropSub.Margin = new System.Windows.Forms.Padding(0);
            this.lblDropSub.Name = "lblDropSub";
            this.lblDropSub.Size = new System.Drawing.Size(100, 17);
            this.lblDropSub.TabIndex = 2;
            this.lblDropSub.Text = "或者点下面的按钮";
            //
            // btnBrowse
            //
            this.btnBrowse.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnBrowse.Location = new System.Drawing.Point(320, 86);
            this.btnBrowse.Margin = new System.Windows.Forms.Padding(0, 4, 0, 4);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Radius = 8;
            this.btnBrowse.Size = new System.Drawing.Size(120, 38);
            this.btnBrowse.TabIndex = 3;
            this.btnBrowse.Text = "选择文件夹";
            this.btnBrowse.Type = AntdUI.TTypeMini.Primary;
            //
            // lblFiles
            //
            this.lblFiles.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblFiles.AutoSize = true;
            this.lblFiles.Location = new System.Drawing.Point(2, 166);
            this.lblFiles.Name = "lblFiles";
            this.lblFiles.Size = new System.Drawing.Size(200, 17);
            this.lblFiles.TabIndex = 2;
            this.lblFiles.Text = "";
            //
            // entryLayout
            //
            this.entryLayout.ColumnCount = 4;
            this.entryLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 76F));
            this.entryLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 300F));
            this.entryLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.entryLayout.Controls.Add(this.lblEntry, 0, 0);
            this.entryLayout.Controls.Add(this.cboEntry, 1, 0);
            this.entryLayout.Controls.Add(this.lblEntryHint, 2, 0);
            this.entryLayout.Controls.Add(this.chkAutoName, 3, 0);
            this.entryLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.entryLayout.Location = new System.Drawing.Point(0, 188);
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
            this.cboEntry.Anchor = System.Windows.Forms.AnchorStyles.Left;
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
            this.chkAutoName.Location = new System.Drawing.Point(580, 14);
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
            this.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.Name = "FolderStepView";
            this.Size = new System.Drawing.Size(760, 240);
            this.rootLayout.ResumeLayout(false);
            this.rootLayout.PerformLayout();
            this.dropPanel.ResumeLayout(false);
            this.dropLayout.ResumeLayout(false);
            this.dropLayout.PerformLayout();
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
        private AntdUI.Label lblFiles;
        private System.Windows.Forms.TableLayoutPanel entryLayout;
        private AntdUI.Label lblEntry;
        private AntdUI.Select cboEntry;
        private AntdUI.Label lblEntryHint;
        private AntdUI.Checkbox chkAutoName;
    }
}
