namespace UnInstall
{
    partial class FrmUnInstall
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
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmUnInstall));
            this.PnlMain = new DSkin.Controls.DSkinPanel();
            this.BtnUnInstall = new DSkin.Controls.DSkinButton();
            this.dSkinPictureBox1 = new DSkin.Controls.DSkinPictureBox();
            this.PnlAgree = new DSkin.Controls.DSkinPanel();
            this.CkAgree = new DSkin.Controls.DSkinCheckBox();
            this.PnlInstalling = new DSkin.Controls.DSkinPanel();
            this.LblUnInstatllInfo = new DSkin.Controls.DSkinLabel();
            this.PiStart = new DSkin.Controls.DSkinProgressIndicator();
            this.LblMainTitle = new DSkin.Controls.DSkinLabel();
            this.PnlMain.SuspendLayout();
            this.PnlAgree.SuspendLayout();
            this.PnlInstalling.SuspendLayout();
            this.SuspendLayout();
            // 
            // PnlMain
            // 
            this.PnlMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PnlMain.BackColor = System.Drawing.Color.White;
            this.PnlMain.Controls.Add(this.BtnUnInstall);
            this.PnlMain.Controls.Add(this.dSkinPictureBox1);
            this.PnlMain.Controls.Add(this.PnlAgree);
            this.PnlMain.Controls.Add(this.PnlInstalling);
            this.PnlMain.Location = new System.Drawing.Point(0, 103);
            this.PnlMain.Name = "PnlMain";
            this.PnlMain.RightBottom = ((System.Drawing.Image)(resources.GetObject("PnlMain.RightBottom")));
            this.PnlMain.Size = new System.Drawing.Size(663, 340);
            this.PnlMain.TabIndex = 3;
            this.PnlMain.Text = "dSkinPanel1";
            this.PnlMain.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PnlMoveForm);
            // 
            // BtnUnInstall
            // 
            this.BtnUnInstall.AdaptImage = true;
            this.BtnUnInstall.BaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(209)))), ((int)(((byte)(27)))), ((int)(((byte)(27)))));
            this.BtnUnInstall.ButtonBorderColor = System.Drawing.Color.Transparent;
            this.BtnUnInstall.ButtonBorderWidth = 1;
            this.BtnUnInstall.DialogResult = System.Windows.Forms.DialogResult.None;
            this.BtnUnInstall.Font = new System.Drawing.Font("微软雅黑", 10F);
            this.BtnUnInstall.ForeColor = System.Drawing.Color.White;
            this.BtnUnInstall.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(27)))), ((int)(((byte)(27)))));
            this.BtnUnInstall.HoverImage = null;
            this.BtnUnInstall.Location = new System.Drawing.Point(539, 297);
            this.BtnUnInstall.Name = "BtnUnInstall";
            this.BtnUnInstall.NormalImage = null;
            this.BtnUnInstall.PressColor = System.Drawing.Color.FromArgb(((int)(((byte)(152)))), ((int)(((byte)(19)))), ((int)(((byte)(19)))));
            this.BtnUnInstall.PressedImage = null;
            this.BtnUnInstall.Radius = 6;
            this.BtnUnInstall.ShowButtonBorder = true;
            this.BtnUnInstall.Size = new System.Drawing.Size(118, 36);
            this.BtnUnInstall.TabIndex = 18;
            this.BtnUnInstall.Text = "狠心卸载";
            this.BtnUnInstall.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.BtnUnInstall.TextPadding = 0;
            this.BtnUnInstall.Click += new System.EventHandler(this.BtnUnInstall_Click);
            // 
            // dSkinPictureBox1
            // 
            this.dSkinPictureBox1.BackgroundImage = global::UnInstall.Properties.Resources.gril;
            this.dSkinPictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.dSkinPictureBox1.Enabled = false;
            this.dSkinPictureBox1.Image = null;
            this.dSkinPictureBox1.Images = new System.Drawing.Image[] {
        null};
            this.dSkinPictureBox1.Location = new System.Drawing.Point(3, 39);
            this.dSkinPictureBox1.Name = "dSkinPictureBox1";
            this.dSkinPictureBox1.Size = new System.Drawing.Size(205, 255);
            this.dSkinPictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.dSkinPictureBox1.TabIndex = 17;
            this.dSkinPictureBox1.Text = "dSkinPictureBox1";
            // 
            // PnlAgree
            // 
            this.PnlAgree.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.PnlAgree.BackColor = System.Drawing.Color.Transparent;
            this.PnlAgree.Controls.Add(this.CkAgree);
            this.PnlAgree.Location = new System.Drawing.Point(1, 300);
            this.PnlAgree.Name = "PnlAgree";
            this.PnlAgree.RightBottom = ((System.Drawing.Image)(resources.GetObject("PnlAgree.RightBottom")));
            this.PnlAgree.Size = new System.Drawing.Size(363, 32);
            this.PnlAgree.TabIndex = 12;
            this.PnlAgree.Text = "dSkinPanel1";
            this.PnlAgree.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PnlMoveForm);
            // 
            // CkAgree
            // 
            this.CkAgree.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.CkAgree.CheckAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.CkAgree.Checked = true;
            this.CkAgree.CheckedHover = global::UnInstall.Properties.Resources.ckedh;
            this.CkAgree.CheckedNormal = global::UnInstall.Properties.Resources.ckedn;
            this.CkAgree.CheckedPressed = global::UnInstall.Properties.Resources.ckedd;
            this.CkAgree.CheckFlagColor = System.Drawing.Color.Black;
            this.CkAgree.CheckFlagColorDisabled = System.Drawing.Color.Gray;
            this.CkAgree.CheckRectBackColorDisabled = System.Drawing.Color.Silver;
            this.CkAgree.CheckRectBackColorHighLight = System.Drawing.Color.FromArgb(((int)(((byte)(93)))), ((int)(((byte)(151)))), ((int)(((byte)(2)))));
            this.CkAgree.CheckRectBackColorNormal = System.Drawing.Color.Transparent;
            this.CkAgree.CheckRectBackColorPressed = System.Drawing.Color.Transparent;
            this.CkAgree.CheckRectColor = System.Drawing.Color.DodgerBlue;
            this.CkAgree.CheckRectColorDisabled = System.Drawing.Color.Gray;
            this.CkAgree.CheckRectWidth = 13;
            this.CkAgree.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CkAgree.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.CkAgree.ForeColor = System.Drawing.Color.DimGray;
            this.CkAgree.InnerPaddingWidth = 2;
            this.CkAgree.InnerRectInflate = 3;
            this.CkAgree.Location = new System.Drawing.Point(6, 6);
            this.CkAgree.Name = "CkAgree";
            this.CkAgree.Size = new System.Drawing.Size(210, 21);
            this.CkAgree.SpaceBetweenCheckMarkAndText = 3;
            this.CkAgree.TabIndex = 6;
            this.CkAgree.Text = "同时删除电脑中的用户数据和设置";
            this.CkAgree.TextColorDisabled = System.Drawing.Color.Gray;
            this.CkAgree.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            this.CkAgree.UncheckedHover = global::UnInstall.Properties.Resources.ckh;
            this.CkAgree.UncheckedNormal = global::UnInstall.Properties.Resources.ckn;
            this.CkAgree.UncheckedPressed = global::UnInstall.Properties.Resources.ckd;
            // 
            // PnlInstalling
            // 
            this.PnlInstalling.BackColor = System.Drawing.Color.Transparent;
            this.PnlInstalling.Controls.Add(this.LblUnInstatllInfo);
            this.PnlInstalling.Controls.Add(this.PiStart);
            this.PnlInstalling.Enabled = false;
            this.PnlInstalling.Location = new System.Drawing.Point(270, 141);
            this.PnlInstalling.Name = "PnlInstalling";
            this.PnlInstalling.RightBottom = ((System.Drawing.Image)(resources.GetObject("PnlInstalling.RightBottom")));
            this.PnlInstalling.Size = new System.Drawing.Size(365, 40);
            this.PnlInstalling.TabIndex = 11;
            this.PnlInstalling.Text = "dSkinPanel1";
            // 
            // LblUnInstatllInfo
            // 
            this.LblUnInstatllInfo.EffectValue = 0;
            this.LblUnInstatllInfo.Enabled = false;
            this.LblUnInstatllInfo.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.LblUnInstatllInfo.ForeColor = System.Drawing.Color.Black;
            this.LblUnInstatllInfo.Location = new System.Drawing.Point(129, 10);
            this.LblUnInstatllInfo.Name = "LblUnInstatllInfo";
            this.LblUnInstatllInfo.Size = new System.Drawing.Size(54, 18);
            this.LblUnInstatllInfo.TabIndex = 10;
            this.LblUnInstatllInfo.Text = "正在卸载";
            this.LblUnInstatllInfo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.LblUnInstatllInfo.TextEffect = DSkin.DirectUI.TextEffects.Glow;
            this.LblUnInstatllInfo.TextRenderMode = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            // 
            // PiStart
            // 
            this.PiStart.CircleColor = System.Drawing.Color.FromArgb(((int)(((byte)(152)))), ((int)(((byte)(19)))), ((int)(((byte)(19)))));
            this.PiStart.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(152)))), ((int)(((byte)(19)))), ((int)(((byte)(19)))));
            this.PiStart.Location = new System.Drawing.Point(92, 6);
            this.PiStart.Name = "PiStart";
            this.PiStart.Percentage = 0F;
            this.PiStart.Size = new System.Drawing.Size(26, 26);
            this.PiStart.TabIndex = 11;
            this.PiStart.Text = "dSkinProgressIndicator1";
            // 
            // LblMainTitle
            // 
            this.LblMainTitle.AutoSize = false;
            this.LblMainTitle.Enabled = false;
            this.LblMainTitle.Font = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.LblMainTitle.ForeColor = System.Drawing.Color.White;
            this.LblMainTitle.Location = new System.Drawing.Point(1, 56);
            this.LblMainTitle.Name = "LblMainTitle";
            this.LblMainTitle.Size = new System.Drawing.Size(659, 41);
            this.LblMainTitle.TabIndex = 4;
            this.LblMainTitle.Text = "XX软件名称";
            this.LblMainTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.LblMainTitle.TextEffect = DSkin.DirectUI.TextEffects.Glow;
            this.LblMainTitle.TextRenderMode = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            // 
            // FrmUnInstall
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(213)))), ((int)(((byte)(47)))), ((int)(((byte)(47)))));
            this.CanResize = false;
            this.CaptionOffset = new System.Drawing.Point(10, -30);
            this.ClientSize = new System.Drawing.Size(661, 440);
            this.CloseBox.HoverImage = ((System.Drawing.Image)(resources.GetObject("FrmUnInstall.CloseBox.HoverImage")));
            this.CloseBox.NormalImage = ((System.Drawing.Image)(resources.GetObject("FrmUnInstall.CloseBox.NormalImage")));
            this.CloseBox.PressImage = ((System.Drawing.Image)(resources.GetObject("FrmUnInstall.CloseBox.PressImage")));
            this.CloseBox.Size = new System.Drawing.Size(30, 27);
            this.Controls.Add(this.LblMainTitle);
            this.Controls.Add(this.PnlMain);
            this.DoubleClickMaximized = false;
            this.IconRectangle = new System.Drawing.Rectangle(-30, -30, 1, 1);
            this.IsAccordShow = true;
            this.MaxBox.HoverImage = ((System.Drawing.Image)(resources.GetObject("FrmUnInstall.MaxBox.HoverImage")));
            this.MaxBox.NormalImage = ((System.Drawing.Image)(resources.GetObject("FrmUnInstall.MaxBox.NormalImage")));
            this.MaxBox.PressImage = ((System.Drawing.Image)(resources.GetObject("FrmUnInstall.MaxBox.PressImage")));
            this.MaxBox.Size = new System.Drawing.Size(30, 27);
            this.MaximizeBox = false;
            this.MinBox.HoverImage = ((System.Drawing.Image)(resources.GetObject("FrmUnInstall.MinBox.HoverImage")));
            this.MinBox.NormalImage = ((System.Drawing.Image)(resources.GetObject("FrmUnInstall.MinBox.NormalImage")));
            this.MinBox.PressImage = ((System.Drawing.Image)(resources.GetObject("FrmUnInstall.MinBox.PressImage")));
            this.MinBox.Size = new System.Drawing.Size(30, 27);
            this.Name = "FrmUnInstall";
            this.NormalBox.HoverImage = ((System.Drawing.Image)(resources.GetObject("FrmUnInstall.NormalBox.HoverImage")));
            this.NormalBox.NormalImage = ((System.Drawing.Image)(resources.GetObject("FrmUnInstall.NormalBox.NormalImage")));
            this.NormalBox.PressImage = ((System.Drawing.Image)(resources.GetObject("FrmUnInstall.NormalBox.PressImage")));
            this.NormalBox.Size = new System.Drawing.Size(30, 27);
            this.Load += new System.EventHandler(this.FrmUnInstall_Load);
            this.PnlMain.ResumeLayout(false);
            this.PnlAgree.ResumeLayout(false);
            this.PnlAgree.PerformLayout();
            this.PnlInstalling.ResumeLayout(false);
            this.PnlInstalling.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private DSkin.Controls.DSkinPanel PnlMain;
        private DSkin.Controls.DSkinPanel PnlAgree;
        private DSkin.Controls.DSkinLabel LblUnInstatllInfo;
        private DSkin.Controls.DSkinProgressIndicator PiStart;
        private DSkin.Controls.DSkinCheckBox CkAgree;
        private DSkin.Controls.DSkinPictureBox dSkinPictureBox1;
        private DSkin.Controls.DSkinLabel LblMainTitle;
        private DSkin.Controls.DSkinButton BtnUnInstall;
        private DSkin.Controls.DSkinPanel PnlInstalling;
    }
}

