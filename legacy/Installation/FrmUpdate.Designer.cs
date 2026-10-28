namespace Installation
{
    partial class FrmUpdate
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmUpdate));
            this.LblMainTitle = new DSkin.Controls.DSkinLabel();
            this.PicLogo = new DSkin.Controls.DSkinPictureBox();
            this.PnlMain = new DSkin.Controls.DSkinPanel();
            this.dSkinLabel1 = new DSkin.Controls.DSkinLabel();
            this.PnlInstalling = new DSkin.Controls.DSkinPanel();
            this.LblInstatllInfo = new DSkin.Controls.DSkinLabel();
            this.PiStart = new DSkin.Controls.DSkinProgressIndicator();
            this.InstallProgressBar = new DSkin.Controls.DSkinProgressBar();
            this.BtnInstall = new DSkin.Controls.DSkinButton();
            this.PnlMain.SuspendLayout();
            this.PnlInstalling.SuspendLayout();
            this.SuspendLayout();
            // 
            // LblMainTitle
            // 
            this.LblMainTitle.AutoSize = false;
            this.LblMainTitle.Enabled = false;
            this.LblMainTitle.Font = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.LblMainTitle.ForeColor = System.Drawing.Color.White;
            this.LblMainTitle.Location = new System.Drawing.Point(-1, 190);
            this.LblMainTitle.Name = "LblMainTitle";
            this.LblMainTitle.Size = new System.Drawing.Size(585, 42);
            this.LblMainTitle.TabIndex = 3;
            this.LblMainTitle.Text = "XX软件名称";
            this.LblMainTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.LblMainTitle.TextEffect = DSkin.DirectUI.TextEffects.Glow;
            this.LblMainTitle.TextRenderMode = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            // 
            // PicLogo
            // 
            this.PicLogo.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.PicLogo.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("PicLogo.BackgroundImage")));
            this.PicLogo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.PicLogo.Enabled = false;
            this.PicLogo.Image = null;
            this.PicLogo.Images = new System.Drawing.Image[] {
        null};
            this.PicLogo.Location = new System.Drawing.Point(216, 29);
            this.PicLogo.Name = "PicLogo";
            this.PicLogo.Size = new System.Drawing.Size(150, 150);
            this.PicLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.PicLogo.TabIndex = 16;
            this.PicLogo.Text = "dSkinPictureBox1";
            // 
            // PnlMain
            // 
            this.PnlMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PnlMain.BackColor = System.Drawing.Color.White;
            this.PnlMain.Controls.Add(this.dSkinLabel1);
            this.PnlMain.Controls.Add(this.PnlInstalling);
            this.PnlMain.Controls.Add(this.BtnInstall);
            this.PnlMain.Location = new System.Drawing.Point(0, 241);
            this.PnlMain.Name = "PnlMain";
            this.PnlMain.RightBottom = ((System.Drawing.Image)(resources.GetObject("PnlMain.RightBottom")));
            this.PnlMain.Size = new System.Drawing.Size(583, 122);
            this.PnlMain.TabIndex = 2;
            this.PnlMain.Text = "dSkinPanel1";
            this.PnlMain.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PnlMoveForm);
            // 
            // dSkinLabel1
            // 
            this.dSkinLabel1.AutoEllipsis = true;
            this.dSkinLabel1.AutoSize = false;
            this.dSkinLabel1.EffectValue = 0;
            this.dSkinLabel1.Enabled = false;
            this.dSkinLabel1.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.dSkinLabel1.ForeColor = System.Drawing.Color.Black;
            this.dSkinLabel1.Location = new System.Drawing.Point(26, 40);
            this.dSkinLabel1.Name = "dSkinLabel1";
            this.dSkinLabel1.Size = new System.Drawing.Size(528, 29);
            this.dSkinLabel1.TabIndex = 12;
            this.dSkinLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.dSkinLabel1.TextEffect = DSkin.DirectUI.TextEffects.Glow;
            this.dSkinLabel1.TextInnerPadding = new System.Windows.Forms.Padding(0, 5, 0, 5);
            this.dSkinLabel1.TextRenderMode = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            // 
            // PnlInstalling
            // 
            this.PnlInstalling.BackColor = System.Drawing.Color.Transparent;
            this.PnlInstalling.Controls.Add(this.LblInstatllInfo);
            this.PnlInstalling.Controls.Add(this.PiStart);
            this.PnlInstalling.Controls.Add(this.InstallProgressBar);
            this.PnlInstalling.Enabled = false;
            this.PnlInstalling.Location = new System.Drawing.Point(0, 3);
            this.PnlInstalling.Name = "PnlInstalling";
            this.PnlInstalling.RightBottom = ((System.Drawing.Image)(resources.GetObject("PnlInstalling.RightBottom")));
            this.PnlInstalling.Size = new System.Drawing.Size(583, 31);
            this.PnlInstalling.TabIndex = 11;
            this.PnlInstalling.Text = "dSkinPanel1";
            // 
            // LblInstatllInfo
            // 
            this.LblInstatllInfo.AutoEllipsis = true;
            this.LblInstatllInfo.AutoSize = false;
            this.LblInstatllInfo.Dock = System.Windows.Forms.DockStyle.Right;
            this.LblInstatllInfo.EffectValue = 0;
            this.LblInstatllInfo.Enabled = false;
            this.LblInstatllInfo.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.LblInstatllInfo.ForeColor = System.Drawing.Color.Black;
            this.LblInstatllInfo.Location = new System.Drawing.Point(55, 2);
            this.LblInstatllInfo.Name = "LblInstatllInfo";
            this.LblInstatllInfo.Size = new System.Drawing.Size(528, 29);
            this.LblInstatllInfo.TabIndex = 10;
            this.LblInstatllInfo.Text = "正在安装";
            this.LblInstatllInfo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.LblInstatllInfo.TextEffect = DSkin.DirectUI.TextEffects.Glow;
            this.LblInstatllInfo.TextInnerPadding = new System.Windows.Forms.Padding(0, 5, 0, 5);
            this.LblInstatllInfo.TextRenderMode = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            // 
            // PiStart
            // 
            this.PiStart.CircleColor = System.Drawing.Color.FromArgb(((int)(((byte)(9)))), ((int)(((byte)(140)))), ((int)(((byte)(188)))));
            this.PiStart.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(9)))), ((int)(((byte)(140)))), ((int)(((byte)(188)))));
            this.PiStart.Location = new System.Drawing.Point(17, 2);
            this.PiStart.Name = "PiStart";
            this.PiStart.Percentage = 0F;
            this.PiStart.Size = new System.Drawing.Size(30, 30);
            this.PiStart.TabIndex = 11;
            this.PiStart.Text = "dSkinProgressIndicator1";
            // 
            // InstallProgressBar
            // 
            this.InstallProgressBar.AutoSize = false;
            this.InstallProgressBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.InstallProgressBar.ForeColors = new System.Drawing.Color[] {
        System.Drawing.Color.FromArgb(((int)(((byte)(9)))), ((int)(((byte)(140)))), ((int)(((byte)(188)))))};
            this.InstallProgressBar.ForeColorsAngle = 90F;
            this.InstallProgressBar.Location = new System.Drawing.Point(0, 0);
            this.InstallProgressBar.Name = "InstallProgressBar";
            this.InstallProgressBar.Size = new System.Drawing.Size(583, 2);
            this.InstallProgressBar.TabIndex = 12;
            this.InstallProgressBar.Text = "100%";
            this.InstallProgressBar.Value = 100;
            // 
            // BtnInstall
            // 
            this.BtnInstall.AdaptImage = true;
            this.BtnInstall.BaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(9)))), ((int)(((byte)(163)))), ((int)(((byte)(220)))));
            this.BtnInstall.ButtonBorderColor = System.Drawing.Color.Transparent;
            this.BtnInstall.ButtonBorderWidth = 1;
            this.BtnInstall.DialogResult = System.Windows.Forms.DialogResult.None;
            this.BtnInstall.Font = new System.Drawing.Font("微软雅黑", 10F);
            this.BtnInstall.ForeColor = System.Drawing.Color.White;
            this.BtnInstall.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(195)))), ((int)(((byte)(245)))));
            this.BtnInstall.HoverImage = null;
            this.BtnInstall.Location = new System.Drawing.Point(169, 67);
            this.BtnInstall.Name = "BtnInstall";
            this.BtnInstall.NormalImage = null;
            this.BtnInstall.PressColor = System.Drawing.Color.FromArgb(((int)(((byte)(9)))), ((int)(((byte)(140)))), ((int)(((byte)(188)))));
            this.BtnInstall.PressedImage = null;
            this.BtnInstall.Radius = 6;
            this.BtnInstall.ShowButtonBorder = true;
            this.BtnInstall.Size = new System.Drawing.Size(240, 39);
            this.BtnInstall.TabIndex = 0;
            this.BtnInstall.Text = "立即更新";
            this.BtnInstall.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.BtnInstall.TextPadding = 0;
            this.BtnInstall.Click += new System.EventHandler(this.BtnInstallClick);
            // 
            // FrmUpdate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CanResize = false;
            this.CaptionOffset = new System.Drawing.Point(10, -30);
            this.ClientSize = new System.Drawing.Size(583, 363);
            this.CloseBox.HoverImage = ((System.Drawing.Image)(resources.GetObject("FrmUpdate.CloseBox.HoverImage")));
            this.CloseBox.NormalImage = ((System.Drawing.Image)(resources.GetObject("FrmUpdate.CloseBox.NormalImage")));
            this.CloseBox.PressImage = ((System.Drawing.Image)(resources.GetObject("FrmUpdate.CloseBox.PressImage")));
            this.CloseBox.Size = new System.Drawing.Size(30, 27);
            this.Controls.Add(this.LblMainTitle);
            this.Controls.Add(this.PicLogo);
            this.Controls.Add(this.PnlMain);
            this.DoubleClickMaximized = false;
            this.IconRectangle = new System.Drawing.Rectangle(-30, -30, 20, 20);
            this.IsAccordShow = true;
            this.MaxBox.HoverImage = ((System.Drawing.Image)(resources.GetObject("FrmUpdate.MaxBox.HoverImage")));
            this.MaxBox.NormalImage = ((System.Drawing.Image)(resources.GetObject("FrmUpdate.MaxBox.NormalImage")));
            this.MaxBox.PressImage = ((System.Drawing.Image)(resources.GetObject("FrmUpdate.MaxBox.PressImage")));
            this.MaxBox.Size = new System.Drawing.Size(30, 27);
            this.MaximizeBox = false;
            this.MinBox.HoverImage = ((System.Drawing.Image)(resources.GetObject("FrmUpdate.MinBox.HoverImage")));
            this.MinBox.NormalImage = ((System.Drawing.Image)(resources.GetObject("FrmUpdate.MinBox.NormalImage")));
            this.MinBox.PressImage = ((System.Drawing.Image)(resources.GetObject("FrmUpdate.MinBox.PressImage")));
            this.MinBox.Size = new System.Drawing.Size(30, 27);
            this.Name = "FrmUpdate";
            this.NormalBox.HoverImage = ((System.Drawing.Image)(resources.GetObject("FrmUpdate.NormalBox.HoverImage")));
            this.NormalBox.NormalImage = ((System.Drawing.Image)(resources.GetObject("FrmUpdate.NormalBox.NormalImage")));
            this.NormalBox.PressImage = ((System.Drawing.Image)(resources.GetObject("FrmUpdate.NormalBox.PressImage")));
            this.NormalBox.Size = new System.Drawing.Size(30, 27);
            this.Padding = new System.Windows.Forms.Padding(-4);
            this.Text = "XX软件安装向导";
            this.Load += new System.EventHandler(this.FrmUpdate_Load);
            this.Shown += new System.EventHandler(this.FrmInstallation_Shown);
            this.PnlMain.ResumeLayout(false);
            this.PnlInstalling.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private DSkin.Controls.DSkinButton BtnInstall;
        private DSkin.Controls.DSkinPanel PnlMain;
        private DSkin.Controls.DSkinLabel LblMainTitle;
        private DSkin.Controls.DSkinPictureBox PicLogo;
        private DSkin.Controls.DSkinPanel PnlInstalling;
        private DSkin.Controls.DSkinLabel LblInstatllInfo;
        private DSkin.Controls.DSkinProgressIndicator PiStart;
        private DSkin.Controls.DSkinProgressBar InstallProgressBar;
        private DSkin.Controls.DSkinLabel dSkinLabel1;
    }
}