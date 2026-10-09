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
            this.folderLayout = new System.Windows.Forms.TableLayoutPanel();
            this.txtSource = new AntdUI.Input();
            this.btnBrowse = new AntdUI.Button();
            this.lblFiles = new AntdUI.Label();
            this.entryLayout = new System.Windows.Forms.TableLayoutPanel();
            this.lblEntry = new AntdUI.Label();
            this.cboEntry = new AntdUI.Select();
            this.lblEntryHint = new AntdUI.Label();
            this.rootLayout.SuspendLayout();
            this.folderLayout.SuspendLayout();
            this.entryLayout.SuspendLayout();
            this.SuspendLayout();
            //
            // rootLayout
            //
            this.rootLayout.ColumnCount = 1;
            this.rootLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rootLayout.Controls.Add(this.folderLayout, 0, 0);
            this.rootLayout.Controls.Add(this.lblFiles, 0, 1);
            this.rootLayout.Controls.Add(this.entryLayout, 0, 2);
            this.rootLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rootLayout.Location = new System.Drawing.Point(0, 0);
            this.rootLayout.Name = "rootLayout";
            this.rootLayout.RowCount = 4;
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 62F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 46F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rootLayout.Size = new System.Drawing.Size(780, 140);
            this.rootLayout.TabIndex = 0;
            //
            // folderLayout
            //
            this.folderLayout.ColumnCount = 2;
            this.folderLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.folderLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 132F));
            this.folderLayout.Controls.Add(this.txtSource, 0, 0);
            this.folderLayout.Controls.Add(this.btnBrowse, 1, 0);
            this.folderLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.folderLayout.Location = new System.Drawing.Point(0, 0);
            this.folderLayout.Margin = new System.Windows.Forms.Padding(0);
            this.folderLayout.Name = "folderLayout";
            this.folderLayout.RowCount = 1;
            this.folderLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.folderLayout.Size = new System.Drawing.Size(780, 62);
            this.folderLayout.TabIndex = 0;
            //
            // txtSource
            //
            this.txtSource.AllowClear = true;
            this.txtSource.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtSource.Location = new System.Drawing.Point(0, 11);
            this.txtSource.Margin = new System.Windows.Forms.Padding(0, 11, 10, 11);
            this.txtSource.Name = "txtSource";
            this.txtSource.PlaceholderText = "把文件夹拖到这里，或者点右边的按钮";
            this.txtSource.Size = new System.Drawing.Size(638, 40);
            this.txtSource.TabIndex = 0;
            //
            // btnBrowse
            //
            this.btnBrowse.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnBrowse.Location = new System.Drawing.Point(648, 11);
            this.btnBrowse.Margin = new System.Windows.Forms.Padding(0, 11, 0, 11);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Radius = 6;
            this.btnBrowse.Size = new System.Drawing.Size(132, 40);
            this.btnBrowse.TabIndex = 1;
            this.btnBrowse.Text = "选择文件夹";
            this.btnBrowse.Type = AntdUI.TTypeMini.Primary;
            //
            // lblFiles
            //
            this.lblFiles.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblFiles.AutoSize = true;
            this.lblFiles.Location = new System.Drawing.Point(2, 66);
            this.lblFiles.Name = "lblFiles";
            this.lblFiles.Size = new System.Drawing.Size(200, 17);
            this.lblFiles.TabIndex = 1;
            this.lblFiles.Text = "";
            //
            // entryLayout
            //
            this.entryLayout.ColumnCount = 3;
            this.entryLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 76F));
            this.entryLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 300F));
            this.entryLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.entryLayout.Controls.Add(this.lblEntry, 0, 0);
            this.entryLayout.Controls.Add(this.cboEntry, 1, 0);
            this.entryLayout.Controls.Add(this.lblEntryHint, 2, 0);
            this.entryLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.entryLayout.Location = new System.Drawing.Point(0, 88);
            this.entryLayout.Margin = new System.Windows.Forms.Padding(0);
            this.entryLayout.Name = "entryLayout";
            this.entryLayout.RowCount = 1;
            this.entryLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.entryLayout.Size = new System.Drawing.Size(780, 46);
            this.entryLayout.TabIndex = 2;
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
            // FolderStepView
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.rootLayout);
            this.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.Name = "FolderStepView";
            this.Size = new System.Drawing.Size(780, 140);
            this.rootLayout.ResumeLayout(false);
            this.rootLayout.PerformLayout();
            this.folderLayout.ResumeLayout(false);
            this.entryLayout.ResumeLayout(false);
            this.entryLayout.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel rootLayout;
        private System.Windows.Forms.TableLayoutPanel folderLayout;
        private AntdUI.Input txtSource;
        private AntdUI.Button btnBrowse;
        private AntdUI.Label lblFiles;
        private System.Windows.Forms.TableLayoutPanel entryLayout;
        private AntdUI.Label lblEntry;
        private AntdUI.Select cboEntry;
        private AntdUI.Label lblEntryHint;
    }
}
