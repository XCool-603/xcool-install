namespace InstalltionEdit
{
    partial class FrmInstallation
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmInstallation));
            this.BtnInstall = new DSkin.Controls.DSkinButton();
            this.TxtInstalltionEditPath = new DSkin.Controls.DSkinTextBox();
            this.PnlMain = new DSkin.Controls.DSkinPanel();
            this.PnlCustom = new DSkin.Controls.DSkinPanel();
            this.CkCustomOptions = new DSkin.Controls.DSkinCheckBox();
            this.PnlAgree = new DSkin.Controls.DSkinPanel();
            this.LblAgreement = new DSkin.Controls.DSkinLabel();
            this.CkAgree = new DSkin.Controls.DSkinCheckBox();
            this.PnlInstalling = new DSkin.Controls.DSkinPanel();
            this.LblInstatllInfo = new DSkin.Controls.DSkinLabel();
            this.PiStart = new DSkin.Controls.DSkinProgressIndicator();
            this.InstallProgressBar = new DSkin.Controls.DSkinProgressBar();
            this.PnlCustomOptions = new DSkin.Controls.DSkinPanel();
            this.CkQuickLaunchBar = new DSkin.Controls.DSkinCheckBox();
            this.CkStartClient = new DSkin.Controls.DSkinCheckBox();
            this.CkStartUp = new DSkin.Controls.DSkinCheckBox();
            this.CkDesktopShortcuts = new DSkin.Controls.DSkinCheckBox();
            this.BtnBrowse = new DSkin.Controls.DSkinButton();
            this.PnlPath = new DSkin.Controls.DSkinPanel();
            this.LblMainTitle = new DSkin.Controls.DSkinLabel();
            this.PicLogo = new DSkin.Controls.DSkinPictureBox();
            this.PnlMain.SuspendLayout();
            this.PnlCustom.SuspendLayout();
            this.PnlAgree.SuspendLayout();
            this.PnlInstalling.SuspendLayout();
            this.PnlCustomOptions.SuspendLayout();
            this.PnlPath.SuspendLayout();
            this.SuspendLayout();
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
            this.BtnInstall.IsPureColor = true;
            this.BtnInstall.Location = new System.Drawing.Point(170, 44);
            this.BtnInstall.Name = "BtnInstall";
            this.BtnInstall.NormalImage = null;
            this.BtnInstall.PressColor = System.Drawing.Color.FromArgb(((int)(((byte)(9)))), ((int)(((byte)(140)))), ((int)(((byte)(188)))));
            this.BtnInstall.PressedImage = null;
            this.BtnInstall.Radius = 6;
            this.BtnInstall.ShowButtonBorder = true;
            this.BtnInstall.Size = new System.Drawing.Size(240, 39);
            this.BtnInstall.TabIndex = 0;
            this.BtnInstall.Text = "立即安装";
            this.BtnInstall.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.BtnInstall.TextPadding = 0;
            this.BtnInstall.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
            this.BtnInstall.Click += new System.EventHandler(this.BtnInstallClick);
            // 
            // TxtInstalltionEditPath
            // 
            this.TxtInstalltionEditPath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.TxtInstalltionEditPath.BackColor = System.Drawing.Color.White;
            this.TxtInstalltionEditPath.BitmapCache = false;
            this.TxtInstalltionEditPath.BorderColor = System.Drawing.Color.Transparent;
            this.TxtInstalltionEditPath.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.TxtInstalltionEditPath.FocusBorderBold = false;
            this.TxtInstalltionEditPath.FocusedBorderColor = System.Drawing.Color.Transparent;
            this.TxtInstalltionEditPath.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.TxtInstalltionEditPath.ForeColor = System.Drawing.Color.Gray;
            this.TxtInstalltionEditPath.HoverBorderColor = System.Drawing.Color.Transparent;
            this.TxtInstalltionEditPath.Location = new System.Drawing.Point(5, 6);
            this.TxtInstalltionEditPath.Name = "TxtInstalltionEditPath";
            this.TxtInstalltionEditPath.Size = new System.Drawing.Size(352, 16);
            this.TxtInstalltionEditPath.TabIndex = 1;
            this.TxtInstalltionEditPath.Text = "C:\\Program Files\\XX软件\\";
            this.TxtInstalltionEditPath.TransparencyKey = System.Drawing.Color.Empty;
            this.TxtInstalltionEditPath.WaterFont = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.TxtInstalltionEditPath.WaterText = "";
            this.TxtInstalltionEditPath.WaterTextOffset = new System.Drawing.Point(0, 0);
            this.TxtInstalltionEditPath.MouseEnter += new System.EventHandler(this.PnlPathEnter);
            // 
            // PnlMain
            // 
            this.PnlMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PnlMain.BackColor = System.Drawing.Color.White;
            this.PnlMain.Controls.Add(this.PnlCustom);
            this.PnlMain.Controls.Add(this.PnlAgree);
            this.PnlMain.Controls.Add(this.PnlInstalling);
            this.PnlMain.Controls.Add(this.PnlCustomOptions);
            this.PnlMain.Controls.Add(this.BtnInstall);
            this.PnlMain.Location = new System.Drawing.Point(-1, 243);
            this.PnlMain.Name = "PnlMain";
            this.PnlMain.Size = new System.Drawing.Size(585, 259);
            this.PnlMain.TabIndex = 2;
            this.PnlMain.Text = "dSkinPanel1";
            this.PnlMain.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PnlMoveForm);
            // 
            // PnlCustom
            // 
            this.PnlCustom.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.PnlCustom.BackColor = System.Drawing.Color.Transparent;
            this.PnlCustom.Controls.Add(this.CkCustomOptions);
            this.PnlCustom.Location = new System.Drawing.Point(447, 223);
            this.PnlCustom.Name = "PnlCustom";
            this.PnlCustom.Size = new System.Drawing.Size(137, 32);
            this.PnlCustom.TabIndex = 13;
            this.PnlCustom.Text = "dSkinPanel2";
            // 
            // CkCustomOptions
            // 
            this.CkCustomOptions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.CkCustomOptions.CheckAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.CkCustomOptions.Checked = false;
            this.CkCustomOptions.CheckedHover = ((System.Drawing.Image)(resources.GetObject("CkCustomOptions.CheckedHover")));
            this.CkCustomOptions.CheckedNormal = ((System.Drawing.Image)(resources.GetObject("CkCustomOptions.CheckedNormal")));
            this.CkCustomOptions.CheckedPressed = ((System.Drawing.Image)(resources.GetObject("CkCustomOptions.CheckedPressed")));
            this.CkCustomOptions.CheckFlagColor = System.Drawing.Color.Black;
            this.CkCustomOptions.CheckFlagColorDisabled = System.Drawing.Color.Gray;
            this.CkCustomOptions.CheckRectBackColorDisabled = System.Drawing.Color.Silver;
            this.CkCustomOptions.CheckRectBackColorHighLight = System.Drawing.Color.FromArgb(((int)(((byte)(93)))), ((int)(((byte)(151)))), ((int)(((byte)(2)))));
            this.CkCustomOptions.CheckRectBackColorNormal = System.Drawing.Color.Transparent;
            this.CkCustomOptions.CheckRectBackColorPressed = System.Drawing.Color.Transparent;
            this.CkCustomOptions.CheckRectColor = System.Drawing.Color.DodgerBlue;
            this.CkCustomOptions.CheckRectColorDisabled = System.Drawing.Color.Gray;
            this.CkCustomOptions.CheckRectWidth = 15;
            this.CkCustomOptions.CheckState = System.Windows.Forms.CheckState.Unchecked;
            this.CkCustomOptions.Cursor = System.Windows.Forms.Cursors.Hand;
            this.CkCustomOptions.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.CkCustomOptions.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(39)))), ((int)(((byte)(134)))), ((int)(((byte)(228)))));
            this.CkCustomOptions.InnerPaddingWidth = 2;
            this.CkCustomOptions.InnerRectInflate = 3;
            this.CkCustomOptions.Location = new System.Drawing.Point(39, 2);
            this.CkCustomOptions.Name = "CkCustomOptions";
            this.CkCustomOptions.Size = new System.Drawing.Size(88, 21);
            this.CkCustomOptions.SpaceBetweenCheckMarkAndText = 3;
            this.CkCustomOptions.TabIndex = 7;
            this.CkCustomOptions.Text = "自定义选项";
            this.CkCustomOptions.TextColorDisabled = System.Drawing.Color.Gray;
            this.CkCustomOptions.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            this.CkCustomOptions.UncheckedHover = ((System.Drawing.Image)(resources.GetObject("CkCustomOptions.UncheckedHover")));
            this.CkCustomOptions.UncheckedNormal = ((System.Drawing.Image)(resources.GetObject("CkCustomOptions.UncheckedNormal")));
            this.CkCustomOptions.UncheckedPressed = ((System.Drawing.Image)(resources.GetObject("CkCustomOptions.UncheckedPressed")));
            this.CkCustomOptions.CheckedChanged += new System.EventHandler(this.dSkinCheckBox2_CheckedChanged);
            // 
            // PnlAgree
            // 
            this.PnlAgree.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.PnlAgree.BackColor = System.Drawing.Color.Transparent;
            this.PnlAgree.Controls.Add(this.LblAgreement);
            this.PnlAgree.Controls.Add(this.CkAgree);
            this.PnlAgree.Location = new System.Drawing.Point(1, 224);
            this.PnlAgree.Name = "PnlAgree";
            this.PnlAgree.Size = new System.Drawing.Size(285, 32);
            this.PnlAgree.TabIndex = 12;
            this.PnlAgree.Text = "dSkinPanel1";
            // 
            // LblAgreement
            // 
            this.LblAgreement.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.LblAgreement.Cursor = System.Windows.Forms.Cursors.Hand;
            this.LblAgreement.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.LblAgreement.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(39)))), ((int)(((byte)(134)))), ((int)(((byte)(228)))));
            this.LblAgreement.Location = new System.Drawing.Point(100, 4);
            this.LblAgreement.Name = "LblAgreement";
            this.LblAgreement.Size = new System.Drawing.Size(79, 18);
            this.LblAgreement.TabIndex = 6;
            this.LblAgreement.Text = "软件许可协议";
            this.LblAgreement.TextRenderMode = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            this.LblAgreement.Click += new System.EventHandler(this.LblAgreement_Click);
            // 
            // CkAgree
            // 
            this.CkAgree.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.CkAgree.CheckAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.CkAgree.Checked = true;
            this.CkAgree.CheckedHover = ((System.Drawing.Image)(resources.GetObject("CkAgree.CheckedHover")));
            this.CkAgree.CheckedNormal = ((System.Drawing.Image)(resources.GetObject("CkAgree.CheckedNormal")));
            this.CkAgree.CheckedPressed = ((System.Drawing.Image)(resources.GetObject("CkAgree.CheckedPressed")));
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
            this.CkAgree.InnerPaddingWidth = 2;
            this.CkAgree.InnerRectInflate = 3;
            this.CkAgree.Location = new System.Drawing.Point(8, 2);
            this.CkAgree.Name = "CkAgree";
            this.CkAgree.Size = new System.Drawing.Size(86, 21);
            this.CkAgree.SpaceBetweenCheckMarkAndText = 3;
            this.CkAgree.TabIndex = 5;
            this.CkAgree.Text = "阅读并同意";
            this.CkAgree.TextColorDisabled = System.Drawing.Color.Gray;
            this.CkAgree.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            this.CkAgree.UncheckedHover = ((System.Drawing.Image)(resources.GetObject("CkAgree.UncheckedHover")));
            this.CkAgree.UncheckedNormal = ((System.Drawing.Image)(resources.GetObject("CkAgree.UncheckedNormal")));
            this.CkAgree.UncheckedPressed = ((System.Drawing.Image)(resources.GetObject("CkAgree.UncheckedPressed")));
            this.CkAgree.CheckedChanged += new System.EventHandler(this.CkAgreeCheckedChanged);
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
            this.PnlInstalling.Size = new System.Drawing.Size(583, 31);
            this.PnlInstalling.TabIndex = 11;
            this.PnlInstalling.Text = "dSkinPanel1";
            // 
            // LblInstatllInfo
            // 
            this.LblInstatllInfo.AutoEllipsis = true;
            this.LblInstatllInfo.AutoSize = false;
            this.LblInstatllInfo.EffectValue = 0;
            this.LblInstatllInfo.Enabled = false;
            this.LblInstatllInfo.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.LblInstatllInfo.ForeColor = System.Drawing.Color.Black;
            this.LblInstatllInfo.Location = new System.Drawing.Point(49, 9);
            this.LblInstatllInfo.Name = "LblInstatllInfo";
            this.LblInstatllInfo.Size = new System.Drawing.Size(525, 14);
            this.LblInstatllInfo.TabIndex = 10;
            this.LblInstatllInfo.Text = "正在安装";
            this.LblInstatllInfo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.LblInstatllInfo.TextEffect = DSkin.DirectUI.TextEffects.Glow;
            this.LblInstatllInfo.TextRenderMode = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            // 
            // PiStart
            // 
            this.PiStart.CircleColor = System.Drawing.Color.FromArgb(((int)(((byte)(9)))), ((int)(((byte)(140)))), ((int)(((byte)(188)))));
            this.PiStart.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(9)))), ((int)(((byte)(140)))), ((int)(((byte)(188)))));
            this.PiStart.Location = new System.Drawing.Point(20, 3);
            this.PiStart.Name = "PiStart";
            this.PiStart.Percentage = 0F;
            this.PiStart.Size = new System.Drawing.Size(26, 26);
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
            this.InstallProgressBar.Text = "10%";
            this.InstallProgressBar.Value = 10;
            // 
            // PnlCustomOptions
            // 
            this.PnlCustomOptions.BackColor = System.Drawing.Color.White;
            this.PnlCustomOptions.Controls.Add(this.CkQuickLaunchBar);
            this.PnlCustomOptions.Controls.Add(this.CkStartClient);
            this.PnlCustomOptions.Controls.Add(this.CkStartUp);
            this.PnlCustomOptions.Controls.Add(this.CkDesktopShortcuts);
            this.PnlCustomOptions.Controls.Add(this.BtnBrowse);
            this.PnlCustomOptions.Controls.Add(this.PnlPath);
            this.PnlCustomOptions.Location = new System.Drawing.Point(77, 93);
            this.PnlCustomOptions.Name = "PnlCustomOptions";
            this.PnlCustomOptions.Size = new System.Drawing.Size(437, 114);
            this.PnlCustomOptions.TabIndex = 9;
            this.PnlCustomOptions.Text = "dSkinPanel2";
            this.PnlCustomOptions.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PnlMoveForm);
            // 
            // CkQuickLaunchBar
            // 
            this.CkQuickLaunchBar.CheckAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.CkQuickLaunchBar.Checked = true;
            this.CkQuickLaunchBar.CheckedHover = ((System.Drawing.Image)(resources.GetObject("CkQuickLaunchBar.CheckedHover")));
            this.CkQuickLaunchBar.CheckedNormal = ((System.Drawing.Image)(resources.GetObject("CkQuickLaunchBar.CheckedNormal")));
            this.CkQuickLaunchBar.CheckedPressed = ((System.Drawing.Image)(resources.GetObject("CkQuickLaunchBar.CheckedPressed")));
            this.CkQuickLaunchBar.CheckFlagColor = System.Drawing.Color.Black;
            this.CkQuickLaunchBar.CheckFlagColorDisabled = System.Drawing.Color.Gray;
            this.CkQuickLaunchBar.CheckRectBackColorDisabled = System.Drawing.Color.Silver;
            this.CkQuickLaunchBar.CheckRectBackColorHighLight = System.Drawing.Color.FromArgb(((int)(((byte)(93)))), ((int)(((byte)(151)))), ((int)(((byte)(2)))));
            this.CkQuickLaunchBar.CheckRectBackColorNormal = System.Drawing.Color.Transparent;
            this.CkQuickLaunchBar.CheckRectBackColorPressed = System.Drawing.Color.Transparent;
            this.CkQuickLaunchBar.CheckRectColor = System.Drawing.Color.DodgerBlue;
            this.CkQuickLaunchBar.CheckRectColorDisabled = System.Drawing.Color.Gray;
            this.CkQuickLaunchBar.CheckRectWidth = 13;
            this.CkQuickLaunchBar.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CkQuickLaunchBar.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.CkQuickLaunchBar.InnerPaddingWidth = 2;
            this.CkQuickLaunchBar.InnerRectInflate = 3;
            this.CkQuickLaunchBar.Location = new System.Drawing.Point(230, 49);
            this.CkQuickLaunchBar.Name = "CkQuickLaunchBar";
            this.CkQuickLaunchBar.Size = new System.Drawing.Size(123, 21);
            this.CkQuickLaunchBar.SpaceBetweenCheckMarkAndText = 3;
            this.CkQuickLaunchBar.TabIndex = 8;
            this.CkQuickLaunchBar.Text = "添加到快速启动栏";
            this.CkQuickLaunchBar.TextColorDisabled = System.Drawing.Color.Gray;
            this.CkQuickLaunchBar.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            this.CkQuickLaunchBar.UncheckedHover = ((System.Drawing.Image)(resources.GetObject("CkQuickLaunchBar.UncheckedHover")));
            this.CkQuickLaunchBar.UncheckedNormal = ((System.Drawing.Image)(resources.GetObject("CkQuickLaunchBar.UncheckedNormal")));
            this.CkQuickLaunchBar.UncheckedPressed = ((System.Drawing.Image)(resources.GetObject("CkQuickLaunchBar.UncheckedPressed")));
            // 
            // CkStartClient
            // 
            this.CkStartClient.CheckAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.CkStartClient.Checked = false;
            this.CkStartClient.CheckedHover = ((System.Drawing.Image)(resources.GetObject("CkStartClient.CheckedHover")));
            this.CkStartClient.CheckedNormal = ((System.Drawing.Image)(resources.GetObject("CkStartClient.CheckedNormal")));
            this.CkStartClient.CheckedPressed = ((System.Drawing.Image)(resources.GetObject("CkStartClient.CheckedPressed")));
            this.CkStartClient.CheckFlagColor = System.Drawing.Color.Black;
            this.CkStartClient.CheckFlagColorDisabled = System.Drawing.Color.Gray;
            this.CkStartClient.CheckRectBackColorDisabled = System.Drawing.Color.Silver;
            this.CkStartClient.CheckRectBackColorHighLight = System.Drawing.Color.FromArgb(((int)(((byte)(93)))), ((int)(((byte)(151)))), ((int)(((byte)(2)))));
            this.CkStartClient.CheckRectBackColorNormal = System.Drawing.Color.Transparent;
            this.CkStartClient.CheckRectBackColorPressed = System.Drawing.Color.Transparent;
            this.CkStartClient.CheckRectColor = System.Drawing.Color.DodgerBlue;
            this.CkStartClient.CheckRectColorDisabled = System.Drawing.Color.Gray;
            this.CkStartClient.CheckRectWidth = 13;
            this.CkStartClient.CheckState = System.Windows.Forms.CheckState.Unchecked;
            this.CkStartClient.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.CkStartClient.InnerPaddingWidth = 2;
            this.CkStartClient.InnerRectInflate = 3;
            this.CkStartClient.Location = new System.Drawing.Point(230, 76);
            this.CkStartClient.Name = "CkStartClient";
            this.CkStartClient.Size = new System.Drawing.Size(123, 21);
            this.CkStartClient.SpaceBetweenCheckMarkAndText = 3;
            this.CkStartClient.TabIndex = 8;
            this.CkStartClient.Text = "安装完后立即启动";
            this.CkStartClient.TextColorDisabled = System.Drawing.Color.Gray;
            this.CkStartClient.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            this.CkStartClient.UncheckedHover = ((System.Drawing.Image)(resources.GetObject("CkStartClient.UncheckedHover")));
            this.CkStartClient.UncheckedNormal = ((System.Drawing.Image)(resources.GetObject("CkStartClient.UncheckedNormal")));
            this.CkStartClient.UncheckedPressed = ((System.Drawing.Image)(resources.GetObject("CkStartClient.UncheckedPressed")));
            // 
            // CkStartUp
            // 
            this.CkStartUp.CheckAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.CkStartUp.Checked = true;
            this.CkStartUp.CheckedHover = ((System.Drawing.Image)(resources.GetObject("CkStartUp.CheckedHover")));
            this.CkStartUp.CheckedNormal = ((System.Drawing.Image)(resources.GetObject("CkStartUp.CheckedNormal")));
            this.CkStartUp.CheckedPressed = ((System.Drawing.Image)(resources.GetObject("CkStartUp.CheckedPressed")));
            this.CkStartUp.CheckFlagColor = System.Drawing.Color.Black;
            this.CkStartUp.CheckFlagColorDisabled = System.Drawing.Color.Gray;
            this.CkStartUp.CheckRectBackColorDisabled = System.Drawing.Color.Silver;
            this.CkStartUp.CheckRectBackColorHighLight = System.Drawing.Color.FromArgb(((int)(((byte)(93)))), ((int)(((byte)(151)))), ((int)(((byte)(2)))));
            this.CkStartUp.CheckRectBackColorNormal = System.Drawing.Color.Transparent;
            this.CkStartUp.CheckRectBackColorPressed = System.Drawing.Color.Transparent;
            this.CkStartUp.CheckRectColor = System.Drawing.Color.DodgerBlue;
            this.CkStartUp.CheckRectColorDisabled = System.Drawing.Color.Gray;
            this.CkStartUp.CheckRectWidth = 13;
            this.CkStartUp.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CkStartUp.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.CkStartUp.InnerPaddingWidth = 2;
            this.CkStartUp.InnerRectInflate = 3;
            this.CkStartUp.Location = new System.Drawing.Point(23, 76);
            this.CkStartUp.Name = "CkStartUp";
            this.CkStartUp.Size = new System.Drawing.Size(99, 21);
            this.CkStartUp.SpaceBetweenCheckMarkAndText = 3;
            this.CkStartUp.TabIndex = 8;
            this.CkStartUp.Text = "开机自动启动";
            this.CkStartUp.TextColorDisabled = System.Drawing.Color.Gray;
            this.CkStartUp.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            this.CkStartUp.UncheckedHover = ((System.Drawing.Image)(resources.GetObject("CkStartUp.UncheckedHover")));
            this.CkStartUp.UncheckedNormal = ((System.Drawing.Image)(resources.GetObject("CkStartUp.UncheckedNormal")));
            this.CkStartUp.UncheckedPressed = ((System.Drawing.Image)(resources.GetObject("CkStartUp.UncheckedPressed")));
            // 
            // CkDesktopShortcuts
            // 
            this.CkDesktopShortcuts.CheckAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.CkDesktopShortcuts.Checked = true;
            this.CkDesktopShortcuts.CheckedHover = ((System.Drawing.Image)(resources.GetObject("CkDesktopShortcuts.CheckedHover")));
            this.CkDesktopShortcuts.CheckedNormal = ((System.Drawing.Image)(resources.GetObject("CkDesktopShortcuts.CheckedNormal")));
            this.CkDesktopShortcuts.CheckedPressed = ((System.Drawing.Image)(resources.GetObject("CkDesktopShortcuts.CheckedPressed")));
            this.CkDesktopShortcuts.CheckFlagColor = System.Drawing.Color.Black;
            this.CkDesktopShortcuts.CheckFlagColorDisabled = System.Drawing.Color.Gray;
            this.CkDesktopShortcuts.CheckRectBackColorDisabled = System.Drawing.Color.Silver;
            this.CkDesktopShortcuts.CheckRectBackColorHighLight = System.Drawing.Color.FromArgb(((int)(((byte)(93)))), ((int)(((byte)(151)))), ((int)(((byte)(2)))));
            this.CkDesktopShortcuts.CheckRectBackColorNormal = System.Drawing.Color.Transparent;
            this.CkDesktopShortcuts.CheckRectBackColorPressed = System.Drawing.Color.Transparent;
            this.CkDesktopShortcuts.CheckRectColor = System.Drawing.Color.DodgerBlue;
            this.CkDesktopShortcuts.CheckRectColorDisabled = System.Drawing.Color.Gray;
            this.CkDesktopShortcuts.CheckRectWidth = 13;
            this.CkDesktopShortcuts.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CkDesktopShortcuts.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.CkDesktopShortcuts.InnerPaddingWidth = 2;
            this.CkDesktopShortcuts.InnerRectInflate = 3;
            this.CkDesktopShortcuts.Location = new System.Drawing.Point(22, 49);
            this.CkDesktopShortcuts.Name = "CkDesktopShortcuts";
            this.CkDesktopShortcuts.Size = new System.Drawing.Size(99, 21);
            this.CkDesktopShortcuts.SpaceBetweenCheckMarkAndText = 3;
            this.CkDesktopShortcuts.TabIndex = 8;
            this.CkDesktopShortcuts.Text = "生成快捷方式";
            this.CkDesktopShortcuts.TextColorDisabled = System.Drawing.Color.Gray;
            this.CkDesktopShortcuts.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            this.CkDesktopShortcuts.UncheckedHover = ((System.Drawing.Image)(resources.GetObject("CkDesktopShortcuts.UncheckedHover")));
            this.CkDesktopShortcuts.UncheckedNormal = ((System.Drawing.Image)(resources.GetObject("CkDesktopShortcuts.UncheckedNormal")));
            this.CkDesktopShortcuts.UncheckedPressed = ((System.Drawing.Image)(resources.GetObject("CkDesktopShortcuts.UncheckedPressed")));
            // 
            // BtnBrowse
            // 
            this.BtnBrowse.AdaptImage = true;
            this.BtnBrowse.BaseColor = System.Drawing.Color.Transparent;
            this.BtnBrowse.ButtonBorderColor = System.Drawing.Color.Empty;
            this.BtnBrowse.ButtonBorderWidth = 1;
            this.BtnBrowse.DialogResult = System.Windows.Forms.DialogResult.None;
            this.BtnBrowse.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.BtnBrowse.ForeColor = System.Drawing.Color.Black;
            this.BtnBrowse.HoverColor = System.Drawing.Color.Empty;
            this.BtnBrowse.HoverImage = ((System.Drawing.Image)(resources.GetObject("BtnBrowse.HoverImage")));
            this.BtnBrowse.IsPureColor = false;
            this.BtnBrowse.Location = new System.Drawing.Point(391, 8);
            this.BtnBrowse.Name = "BtnBrowse";
            this.BtnBrowse.NormalImage = ((System.Drawing.Image)(resources.GetObject("BtnBrowse.NormalImage")));
            this.BtnBrowse.PressColor = System.Drawing.Color.Empty;
            this.BtnBrowse.PressedImage = ((System.Drawing.Image)(resources.GetObject("BtnBrowse.PressedImage")));
            this.BtnBrowse.Radius = 6;
            this.BtnBrowse.ShowButtonBorder = true;
            this.BtnBrowse.Size = new System.Drawing.Size(30, 30);
            this.BtnBrowse.TabIndex = 4;
            this.BtnBrowse.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.BtnBrowse.TextPadding = 0;
            this.BtnBrowse.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
            this.BtnBrowse.Click += new System.EventHandler(this.BtnBrowse_Click);
            // 
            // PnlPath
            // 
            this.PnlPath.BackColor = System.Drawing.Color.White;
            this.PnlPath.Controls.Add(this.TxtInstalltionEditPath);
            this.PnlPath.Location = new System.Drawing.Point(22, 9);
            this.PnlPath.Name = "PnlPath";
            this.PnlPath.Size = new System.Drawing.Size(364, 29);
            this.PnlPath.TabIndex = 2;
            this.PnlPath.Text = "dSkinPanel2";
            this.PnlPath.MouseEnter += new System.EventHandler(this.PnlPathEnter);
            this.PnlPath.MouseLeave += new System.EventHandler(this.PnlPathLeave);
            // 
            // LblMainTitle
            // 
            this.LblMainTitle.AutoSize = false;
            this.LblMainTitle.Enabled = false;
            this.LblMainTitle.Font = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.LblMainTitle.ForeColor = System.Drawing.Color.White;
            this.LblMainTitle.Location = new System.Drawing.Point(-1, 173);
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
            this.PicLogo.Interval = 40;
            this.PicLogo.Location = new System.Drawing.Point(-1, 1);
            this.PicLogo.Name = "PicLogo";
            this.PicLogo.Size = new System.Drawing.Size(584, 174);
            this.PicLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.PicLogo.TabIndex = 16;
            this.PicLogo.Text = "dSkinPictureBox1";
            // 
            // FrmInstallation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CanResize = false;
            this.CaptionOffset = new System.Drawing.Point(10, -30);
            this.ClientSize = new System.Drawing.Size(583, 500);
            this.CloseBox.NormalColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.CloseBox.Size = new System.Drawing.Size(30, 27);
            this.Controls.Add(this.LblMainTitle);
            this.Controls.Add(this.PicLogo);
            this.Controls.Add(this.PnlMain);
            this.DoubleClickMaximized = false;
            this.IconRectangle = new System.Drawing.Rectangle(-30, -30, 20, 20);
            this.IsAccordShow = true;
            this.MaxBox.NormalColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.MaxBox.PressBackColor = System.Drawing.Color.DodgerBlue;
            this.MaxBox.Size = new System.Drawing.Size(30, 27);
            this.MaximizeBox = false;
            this.MinBox.NormalColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.MinBox.PressBackColor = System.Drawing.Color.DodgerBlue;
            this.MinBox.Size = new System.Drawing.Size(30, 27);
            this.Name = "FrmInstallation";
            this.NormalBox.Size = new System.Drawing.Size(30, 27);
            this.Text = "XX软件安装向导";
            this.Shown += new System.EventHandler(this.FrmInstallation_Shown);
            this.PnlMain.ResumeLayout(false);
            this.PnlCustom.ResumeLayout(false);
            this.PnlAgree.ResumeLayout(false);
            this.PnlAgree.PerformLayout();
            this.PnlInstalling.ResumeLayout(false);
            this.PnlCustomOptions.ResumeLayout(false);
            this.PnlPath.ResumeLayout(false);
            this.PnlPath.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private DSkin.Controls.DSkinButton BtnInstall;
        private DSkin.Controls.DSkinTextBox TxtInstalltionEditPath;
        private DSkin.Controls.DSkinPanel PnlMain;
        private DSkin.Controls.DSkinPanel PnlPath;
        private DSkin.Controls.DSkinLabel LblMainTitle;
        private DSkin.Controls.DSkinButton BtnBrowse;
        private DSkin.Controls.DSkinCheckBox CkAgree;
        private DSkin.Controls.DSkinCheckBox CkCustomOptions;
        private DSkin.Controls.DSkinLabel LblAgreement;
        private DSkin.Controls.DSkinCheckBox CkQuickLaunchBar;
        private DSkin.Controls.DSkinCheckBox CkStartClient;
        private DSkin.Controls.DSkinCheckBox CkStartUp;
        private DSkin.Controls.DSkinCheckBox CkDesktopShortcuts;
        private DSkin.Controls.DSkinPanel PnlCustomOptions;
        private DSkin.Controls.DSkinPictureBox PicLogo;
        private DSkin.Controls.DSkinPanel PnlInstalling;
        private DSkin.Controls.DSkinLabel LblInstatllInfo;
        private DSkin.Controls.DSkinProgressIndicator PiStart;
        private DSkin.Controls.DSkinPanel PnlCustom;
        private DSkin.Controls.DSkinPanel PnlAgree;
        private DSkin.Controls.DSkinProgressBar InstallProgressBar;
    }
}