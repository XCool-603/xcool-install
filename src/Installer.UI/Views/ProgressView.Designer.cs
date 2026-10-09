namespace Installer.UI.Views
{
    partial class ProgressView
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
            this.phaseRow = new System.Windows.Forms.TableLayoutPanel();
            this.lblPhase = new AntdUI.Label();
            this.lblPercent = new AntdUI.Label();
            this.progressBar = new AntdUI.Progress();
            this.statsCard = new AntdUI.Panel();
            this.statsLayout = new System.Windows.Forms.TableLayoutPanel();
            this.lblCurrentFile = new AntdUI.Label();
            this.cellsRow = new System.Windows.Forms.TableLayoutPanel();
            this.cap1 = new AntdUI.Label();
            this.cap2 = new AntdUI.Label();
            this.cap3 = new AntdUI.Label();
            this.cap4 = new AntdUI.Label();
            this.val1 = new AntdUI.Label();
            this.val2 = new AntdUI.Label();
            this.val3 = new AntdUI.Label();
            this.val4 = new AntdUI.Label();
            this.step1 = new AntdUI.Label();
            this.step2 = new AntdUI.Label();
            this.step3 = new AntdUI.Label();
            this.step4 = new AntdUI.Label();
            this.rootLayout.SuspendLayout();
            this.phaseRow.SuspendLayout();
            this.statsCard.SuspendLayout();
            this.statsLayout.SuspendLayout();
            this.cellsRow.SuspendLayout();
            this.SuspendLayout();
            //
            // rootLayout
            //
            this.rootLayout.ColumnCount = 1;
            this.rootLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rootLayout.Controls.Add(this.lblTitle, 0, 0);
            this.rootLayout.Controls.Add(this.lblSubtitle, 0, 1);
            this.rootLayout.Controls.Add(this.phaseRow, 0, 2);
            this.rootLayout.Controls.Add(this.progressBar, 0, 3);
            this.rootLayout.Controls.Add(this.statsCard, 0, 4);
            this.rootLayout.Controls.Add(this.step1, 0, 5);
            this.rootLayout.Controls.Add(this.step2, 0, 6);
            this.rootLayout.Controls.Add(this.step3, 0, 7);
            this.rootLayout.Controls.Add(this.step4, 0, 8);
            this.rootLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rootLayout.Location = new System.Drawing.Point(0, 0);
            this.rootLayout.Name = "rootLayout";
            this.rootLayout.RowCount = 10;
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 16F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 146F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
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
            this.lblTitle.Text = "正在安装";
            //
            // lblSubtitle
            //
            this.lblSubtitle.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblSubtitle.AutoSize = true;
            this.lblSubtitle.Font = new System.Drawing.Font("微软雅黑", 12F);
            this.lblSubtitle.Location = new System.Drawing.Point(0, 36);
            this.lblSubtitle.Margin = new System.Windows.Forms.Padding(0);
            this.lblSubtitle.Name = "lblSubtitle";
            this.lblSubtitle.Size = new System.Drawing.Size(400, 21);
            this.lblSubtitle.TabIndex = 1;
            this.lblSubtitle.Text = "";
            //
            // phaseRow
            //
            this.phaseRow.ColumnCount = 2;
            this.phaseRow.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.phaseRow.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.phaseRow.Controls.Add(this.lblPhase, 0, 0);
            this.phaseRow.Controls.Add(this.lblPercent, 1, 0);
            this.phaseRow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.phaseRow.Location = new System.Drawing.Point(0, 64);
            this.phaseRow.Margin = new System.Windows.Forms.Padding(0);
            this.phaseRow.Name = "phaseRow";
            this.phaseRow.RowCount = 1;
            this.phaseRow.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.phaseRow.Size = new System.Drawing.Size(600, 26);
            this.phaseRow.TabIndex = 2;
            //
            // lblPhase
            //
            this.lblPhase.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblPhase.AutoSize = true;
            this.lblPhase.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold);
            this.lblPhase.IconGap = 6;
            this.lblPhase.PrefixSvg = "<svg viewBox=\"0 0 16 16\"><path d=\"M8 2.6a5.4 5.4 0 1 0 5.4 5.4\" fill=\"none\" stroke=\"#1677FF\" stroke-width=\"1.6\" stroke-linecap=\"round\"/><path d=\"M8 2.6V5.2\" fill=\"none\" stroke=\"#1677FF\" stroke-width=\"1.6\" stroke-linecap=\"round\"/></svg>";
            this.lblPhase.Location = new System.Drawing.Point(0, 2);
            this.lblPhase.Margin = new System.Windows.Forms.Padding(0);
            this.lblPhase.Name = "lblPhase";
            this.lblPhase.Size = new System.Drawing.Size(200, 21);
            this.lblPhase.TabIndex = 0;
            this.lblPhase.Text = "正在准备";
            //
            // lblPercent
            //
            this.lblPercent.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblPercent.AutoSize = true;
            this.lblPercent.Font = new System.Drawing.Font("微软雅黑", 13F, System.Drawing.FontStyle.Bold);
            this.lblPercent.Location = new System.Drawing.Point(520, 1);
            this.lblPercent.Margin = new System.Windows.Forms.Padding(0);
            this.lblPercent.Name = "lblPercent";
            this.lblPercent.Size = new System.Drawing.Size(80, 24);
            this.lblPercent.TabIndex = 1;
            this.lblPercent.Text = "0%";
            //
            // progressBar
            //
            this.progressBar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.progressBar.Location = new System.Drawing.Point(0, 90);
            this.progressBar.Margin = new System.Windows.Forms.Padding(0);
            this.progressBar.Name = "progressBar";
            this.progressBar.Radius = 4;
            this.progressBar.Shape = AntdUI.TShapeProgress.Default;
            this.progressBar.Size = new System.Drawing.Size(600, 16);
            this.progressBar.TabIndex = 3;
            this.progressBar.Text = "";
            this.progressBar.Value = 0F;
            //
            // statsCard
            //
            this.statsCard.Controls.Add(this.statsLayout);
            this.statsCard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.statsCard.Location = new System.Drawing.Point(0, 118);
            this.statsCard.Margin = new System.Windows.Forms.Padding(0, 12, 0, 0);
            this.statsCard.Name = "statsCard";
            this.statsCard.Padding = new System.Windows.Forms.Padding(16, 12, 16, 12);
            this.statsCard.Radius = 12;
            this.statsCard.Size = new System.Drawing.Size(600, 134);
            this.statsCard.TabIndex = 4;
            //
            // statsLayout
            //
            this.statsLayout.ColumnCount = 1;
            this.statsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.statsLayout.Controls.Add(this.lblCurrentFile, 0, 0);
            this.statsLayout.Controls.Add(this.cellsRow, 0, 1);
            this.statsLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.statsLayout.Location = new System.Drawing.Point(16, 12);
            this.statsLayout.Margin = new System.Windows.Forms.Padding(0);
            this.statsLayout.Name = "statsLayout";
            this.statsLayout.RowCount = 2;
            this.statsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.statsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.statsLayout.Size = new System.Drawing.Size(568, 110);
            this.statsLayout.TabIndex = 0;
            //
            // lblCurrentFile
            //
            this.lblCurrentFile.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblCurrentFile.AutoSize = true;
            this.lblCurrentFile.Font = new System.Drawing.Font("微软雅黑", 11F);
            this.lblCurrentFile.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(128)))), ((int)(((byte)(140)))));
            this.lblCurrentFile.Location = new System.Drawing.Point(0, 4);
            this.lblCurrentFile.Margin = new System.Windows.Forms.Padding(0);
            this.lblCurrentFile.Name = "lblCurrentFile";
            this.lblCurrentFile.Size = new System.Drawing.Size(400, 19);
            this.lblCurrentFile.TabIndex = 0;
            this.lblCurrentFile.Text = "";
            //
            // cellsRow —— 4 列 × 2 行（上行标签，下行值）
            //
            this.cellsRow.ColumnCount = 4;
            this.cellsRow.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.cellsRow.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.cellsRow.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.cellsRow.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.cellsRow.Controls.Add(this.cap1, 0, 0);
            this.cellsRow.Controls.Add(this.cap2, 1, 0);
            this.cellsRow.Controls.Add(this.cap3, 2, 0);
            this.cellsRow.Controls.Add(this.cap4, 3, 0);
            this.cellsRow.Controls.Add(this.val1, 0, 1);
            this.cellsRow.Controls.Add(this.val2, 1, 1);
            this.cellsRow.Controls.Add(this.val3, 2, 1);
            this.cellsRow.Controls.Add(this.val4, 3, 1);
            this.cellsRow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cellsRow.Location = new System.Drawing.Point(0, 28);
            this.cellsRow.Margin = new System.Windows.Forms.Padding(0);
            this.cellsRow.Name = "cellsRow";
            this.cellsRow.RowCount = 2;
            this.cellsRow.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 22F));
            this.cellsRow.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.cellsRow.Size = new System.Drawing.Size(568, 82);
            this.cellsRow.TabIndex = 1;
            //
            // cap1..cap4
            //
            this.cap1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.cap1.AutoSize = true;
            this.cap1.Font = new System.Drawing.Font("微软雅黑", 11F);
            this.cap1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(163)))), ((int)(((byte)(175)))));
            this.cap1.Location = new System.Drawing.Point(0, 1);
            this.cap1.Margin = new System.Windows.Forms.Padding(0);
            this.cap1.Name = "cap1";
            this.cap1.Size = new System.Drawing.Size(80, 19);
            this.cap1.TabIndex = 0;
            this.cap1.Text = "已解压文件";
            this.cap2.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.cap2.AutoSize = true;
            this.cap2.Font = new System.Drawing.Font("微软雅黑", 11F);
            this.cap2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(163)))), ((int)(((byte)(175)))));
            this.cap2.Location = new System.Drawing.Point(142, 1);
            this.cap2.Margin = new System.Windows.Forms.Padding(0);
            this.cap2.Name = "cap2";
            this.cap2.Size = new System.Drawing.Size(80, 19);
            this.cap2.TabIndex = 1;
            this.cap2.Text = "已写入大小";
            this.cap3.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.cap3.AutoSize = true;
            this.cap3.Font = new System.Drawing.Font("微软雅黑", 11F);
            this.cap3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(163)))), ((int)(((byte)(175)))));
            this.cap3.Location = new System.Drawing.Point(284, 1);
            this.cap3.Margin = new System.Windows.Forms.Padding(0);
            this.cap3.Name = "cap3";
            this.cap3.Size = new System.Drawing.Size(70, 19);
            this.cap3.TabIndex = 2;
            this.cap3.Text = "已用时间";
            this.cap4.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.cap4.AutoSize = true;
            this.cap4.Font = new System.Drawing.Font("微软雅黑", 11F);
            this.cap4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(163)))), ((int)(((byte)(175)))));
            this.cap4.Location = new System.Drawing.Point(426, 1);
            this.cap4.Margin = new System.Windows.Forms.Padding(0);
            this.cap4.Name = "cap4";
            this.cap4.Size = new System.Drawing.Size(70, 19);
            this.cap4.TabIndex = 3;
            this.cap4.Text = "预计剩余";
            //
            // val1..val4
            //
            this.val1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.val1.AutoSize = true;
            this.val1.Font = new System.Drawing.Font("微软雅黑", 13F);
            this.val1.Location = new System.Drawing.Point(0, 22);
            this.val1.Margin = new System.Windows.Forms.Padding(0);
            this.val1.Name = "val1";
            this.val1.Size = new System.Drawing.Size(100, 24);
            this.val1.TabIndex = 4;
            this.val1.Text = "—";
            this.val2.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.val2.AutoSize = true;
            this.val2.Font = new System.Drawing.Font("微软雅黑", 13F);
            this.val2.Location = new System.Drawing.Point(142, 22);
            this.val2.Margin = new System.Windows.Forms.Padding(0);
            this.val2.Name = "val2";
            this.val2.Size = new System.Drawing.Size(100, 24);
            this.val2.TabIndex = 5;
            this.val2.Text = "—";
            this.val3.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.val3.AutoSize = true;
            this.val3.Font = new System.Drawing.Font("微软雅黑", 13F);
            this.val3.Location = new System.Drawing.Point(284, 22);
            this.val3.Margin = new System.Windows.Forms.Padding(0);
            this.val3.Name = "val3";
            this.val3.Size = new System.Drawing.Size(100, 24);
            this.val3.TabIndex = 6;
            this.val3.Text = "—";
            this.val4.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.val4.AutoSize = true;
            this.val4.Font = new System.Drawing.Font("微软雅黑", 13F);
            this.val4.Location = new System.Drawing.Point(426, 22);
            this.val4.Margin = new System.Windows.Forms.Padding(0);
            this.val4.Name = "val4";
            this.val4.Size = new System.Drawing.Size(100, 24);
            this.val4.TabIndex = 7;
            this.val4.Text = "—";
            //
            // step1..step4
            //
            this.step1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.step1.AutoSize = true;
            this.step1.Font = new System.Drawing.Font("微软雅黑", 12F);
            this.step1.IconGap = 6;
            this.step1.Location = new System.Drawing.Point(0, 252);
            this.step1.Margin = new System.Windows.Forms.Padding(0);
            this.step1.Name = "step1";
            this.step1.Size = new System.Drawing.Size(300, 21);
            this.step1.TabIndex = 5;
            this.step1.Text = "";
            this.step2.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.step2.AutoSize = true;
            this.step2.Font = new System.Drawing.Font("微软雅黑", 12F);
            this.step2.IconGap = 6;
            this.step2.Location = new System.Drawing.Point(0, 280);
            this.step2.Margin = new System.Windows.Forms.Padding(0);
            this.step2.Name = "step2";
            this.step2.Size = new System.Drawing.Size(300, 21);
            this.step2.TabIndex = 6;
            this.step2.Text = "";
            this.step3.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.step3.AutoSize = true;
            this.step3.Font = new System.Drawing.Font("微软雅黑", 12F);
            this.step3.IconGap = 6;
            this.step3.Location = new System.Drawing.Point(0, 308);
            this.step3.Margin = new System.Windows.Forms.Padding(0);
            this.step3.Name = "step3";
            this.step3.Size = new System.Drawing.Size(300, 21);
            this.step3.TabIndex = 7;
            this.step3.Text = "";
            this.step4.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.step4.AutoSize = true;
            this.step4.Font = new System.Drawing.Font("微软雅黑", 12F);
            this.step4.IconGap = 6;
            this.step4.Location = new System.Drawing.Point(0, 336);
            this.step4.Margin = new System.Windows.Forms.Padding(0);
            this.step4.Name = "step4";
            this.step4.Size = new System.Drawing.Size(300, 21);
            this.step4.TabIndex = 8;
            this.step4.Text = "";
            //
            // ProgressView
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.rootLayout);
            this.Font = new System.Drawing.Font("微软雅黑", 12F);
            this.Name = "ProgressView";
            this.Size = new System.Drawing.Size(600, 440);
            this.rootLayout.ResumeLayout(false);
            this.rootLayout.PerformLayout();
            this.phaseRow.ResumeLayout(false);
            this.phaseRow.PerformLayout();
            this.statsCard.ResumeLayout(false);
            this.statsLayout.ResumeLayout(false);
            this.statsLayout.PerformLayout();
            this.cellsRow.ResumeLayout(false);
            this.cellsRow.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel rootLayout;
        private AntdUI.Label lblTitle;
        private AntdUI.Label lblSubtitle;
        private System.Windows.Forms.TableLayoutPanel phaseRow;
        private AntdUI.Label lblPhase;
        private AntdUI.Label lblPercent;
        private AntdUI.Progress progressBar;
        private AntdUI.Panel statsCard;
        private System.Windows.Forms.TableLayoutPanel statsLayout;
        private AntdUI.Label lblCurrentFile;
        private System.Windows.Forms.TableLayoutPanel cellsRow;
        private AntdUI.Label cap1;
        private AntdUI.Label cap2;
        private AntdUI.Label cap3;
        private AntdUI.Label cap4;
        private AntdUI.Label val1;
        private AntdUI.Label val2;
        private AntdUI.Label val3;
        private AntdUI.Label val4;
        private AntdUI.Label step1;
        private AntdUI.Label step2;
        private AntdUI.Label step3;
        private AntdUI.Label step4;
    }
}
