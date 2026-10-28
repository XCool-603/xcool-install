namespace InstalltionEdit
{
    partial class FrmLicenseAgreement
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmLicenseAgreement));
            this.BtnOk = new DSkin.Controls.DSkinButton();
            this.PnlMain = new DSkin.Controls.DSkinPanel();
            this.TxtInstallationPath = new DSkin.Controls.DSkinTextBox();
            this.dSkinPanel1 = new DSkin.Controls.DSkinPanel();
            this.PnlMain.SuspendLayout();
            this.dSkinPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // BtnOk
            // 
            this.BtnOk.AdaptImage = true;
            this.BtnOk.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.BtnOk.BaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.BtnOk.ButtonBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.BtnOk.ButtonBorderWidth = 1;
            this.BtnOk.DialogResult = System.Windows.Forms.DialogResult.None;
            this.BtnOk.Font = new System.Drawing.Font("微软雅黑", 10F);
            this.BtnOk.ForeColor = System.Drawing.Color.White;
            this.BtnOk.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.BtnOk.HoverImage = null;
            this.BtnOk.IsPureColor = true;
            this.BtnOk.Location = new System.Drawing.Point(268, 8);
            this.BtnOk.Name = "BtnOk";
            this.BtnOk.NormalImage = null;
            this.BtnOk.PressColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.BtnOk.PressedImage = null;
            this.BtnOk.Radius = 0;
            this.BtnOk.ShowButtonBorder = true;
            this.BtnOk.Size = new System.Drawing.Size(119, 44);
            this.BtnOk.TabIndex = 4;
            this.BtnOk.Text = "确定保存";
            this.BtnOk.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.BtnOk.TextEffect = DSkin.DirectUI.TextEffects.Glow;
            this.BtnOk.TextPadding = 0;
            this.BtnOk.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
            this.BtnOk.Click += new System.EventHandler(this.BtnOk_Click);
            // 
            // PnlMain
            // 
            this.PnlMain.BackColor = System.Drawing.Color.White;
            this.PnlMain.Controls.Add(this.TxtInstallationPath);
            this.PnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PnlMain.Location = new System.Drawing.Point(4, 34);
            this.PnlMain.Name = "PnlMain";
            this.PnlMain.Size = new System.Drawing.Size(654, 532);
            this.PnlMain.TabIndex = 3;
            this.PnlMain.Text = "dSkinPanel1";
            // 
            // TxtInstallationPath
            // 
            this.TxtInstallationPath.BackColor = System.Drawing.Color.White;
            this.TxtInstallationPath.BitmapCache = false;
            this.TxtInstallationPath.BorderColor = System.Drawing.Color.Transparent;
            this.TxtInstallationPath.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.TxtInstallationPath.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TxtInstallationPath.FocusBorderBold = false;
            this.TxtInstallationPath.FocusedBorderColor = System.Drawing.Color.Transparent;
            this.TxtInstallationPath.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.TxtInstallationPath.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(39)))), ((int)(((byte)(134)))), ((int)(((byte)(228)))));
            this.TxtInstallationPath.HoverBorderColor = System.Drawing.Color.Transparent;
            this.TxtInstallationPath.Location = new System.Drawing.Point(0, 0);
            this.TxtInstallationPath.Multiline = true;
            this.TxtInstallationPath.Name = "TxtInstallationPath";
            this.TxtInstallationPath.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.TxtInstallationPath.Size = new System.Drawing.Size(654, 532);
            this.TxtInstallationPath.TabIndex = 2;
            this.TxtInstallationPath.TransparencyKey = System.Drawing.Color.Empty;
            this.TxtInstallationPath.WaterColor = System.Drawing.Color.Red;
            this.TxtInstallationPath.WaterFont = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.TxtInstallationPath.WaterText = "没有许可协议";
            this.TxtInstallationPath.WaterTextOffset = new System.Drawing.Point(280, 5);
            this.TxtInstallationPath.WaterTextTextRenderMode = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            // 
            // dSkinPanel1
            // 
            this.dSkinPanel1.BackColor = System.Drawing.Color.Transparent;
            this.dSkinPanel1.Controls.Add(this.BtnOk);
            this.dSkinPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.dSkinPanel1.Location = new System.Drawing.Point(4, 566);
            this.dSkinPanel1.Name = "dSkinPanel1";
            this.dSkinPanel1.Size = new System.Drawing.Size(654, 62);
            this.dSkinPanel1.TabIndex = 6;
            this.dSkinPanel1.Text = "dSkinPanel1";
            // 
            // FrmLicenseAgreement
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CanResize = false;
            this.ClientSize = new System.Drawing.Size(662, 632);
            this.CloseBox.HoverImage = ((System.Drawing.Image)(resources.GetObject("FrmLicenseAgreement.CloseBox.HoverImage")));
            this.CloseBox.NormalColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.CloseBox.NormalImage = ((System.Drawing.Image)(resources.GetObject("FrmLicenseAgreement.CloseBox.NormalImage")));
            this.CloseBox.PressImage = ((System.Drawing.Image)(resources.GetObject("FrmLicenseAgreement.CloseBox.PressImage")));
            this.CloseBox.Size = new System.Drawing.Size(30, 27);
            this.Controls.Add(this.PnlMain);
            this.Controls.Add(this.dSkinPanel1);
            this.DoubleClickMaximized = false;
            this.IsAccordShow = true;
            this.MaxBox.HoverImage = ((System.Drawing.Image)(resources.GetObject("FrmLicenseAgreement.MaxBox.HoverImage")));
            this.MaxBox.NormalColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.MaxBox.NormalImage = ((System.Drawing.Image)(resources.GetObject("FrmLicenseAgreement.MaxBox.NormalImage")));
            this.MaxBox.PressBackColor = System.Drawing.Color.DodgerBlue;
            this.MaxBox.PressImage = ((System.Drawing.Image)(resources.GetObject("FrmLicenseAgreement.MaxBox.PressImage")));
            this.MaxBox.Size = new System.Drawing.Size(30, 27);
            this.MaximizeBox = false;
            this.MinBox.HoverImage = ((System.Drawing.Image)(resources.GetObject("FrmLicenseAgreement.MinBox.HoverImage")));
            this.MinBox.NormalColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.MinBox.NormalImage = ((System.Drawing.Image)(resources.GetObject("FrmLicenseAgreement.MinBox.NormalImage")));
            this.MinBox.PressBackColor = System.Drawing.Color.DodgerBlue;
            this.MinBox.PressImage = ((System.Drawing.Image)(resources.GetObject("FrmLicenseAgreement.MinBox.PressImage")));
            this.MinBox.Size = new System.Drawing.Size(30, 27);
            this.MinimizeBox = false;
            this.Name = "FrmLicenseAgreement";
            this.NormalBox.HoverImage = ((System.Drawing.Image)(resources.GetObject("FrmLicenseAgreement.NormalBox.HoverImage")));
            this.NormalBox.NormalImage = ((System.Drawing.Image)(resources.GetObject("FrmLicenseAgreement.NormalBox.NormalImage")));
            this.NormalBox.PressImage = ((System.Drawing.Image)(resources.GetObject("FrmLicenseAgreement.NormalBox.PressImage")));
            this.NormalBox.Size = new System.Drawing.Size(30, 27);
            this.Text = "软件许可协议";
            this.PnlMain.ResumeLayout(false);
            this.PnlMain.PerformLayout();
            this.dSkinPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private DSkin.Controls.DSkinButton BtnOk;
        private DSkin.Controls.DSkinPanel PnlMain;
        private DSkin.Controls.DSkinPanel dSkinPanel1;
        private DSkin.Controls.DSkinTextBox TxtInstallationPath;
    }
}