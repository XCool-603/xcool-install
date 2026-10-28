namespace Installation
{
    partial class FrmMessage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMessage));
            this.PnlMain = new DSkin.Controls.DSkinPanel();
            this.LblMsg = new DSkin.Controls.DSkinLabel();
            this.BtnOk = new DSkin.Controls.DSkinButton();
            this.BtnCancel = new DSkin.Controls.DSkinButton();
            this.PnlMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // PnlMain
            // 
            this.PnlMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PnlMain.BackColor = System.Drawing.Color.White;
            this.PnlMain.Controls.Add(this.LblMsg);
            this.PnlMain.Location = new System.Drawing.Point(0, 37);
            this.PnlMain.Name = "PnlMain";
            this.PnlMain.RightBottom = ((System.Drawing.Image)(resources.GetObject("PnlMain.RightBottom")));
            this.PnlMain.Size = new System.Drawing.Size(273, 86);
            this.PnlMain.TabIndex = 0;
            this.PnlMain.Text = "dSkinPanel1";
            // 
            // LblMsg
            // 
            this.LblMsg.AutoSize = false;
            this.LblMsg.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LblMsg.EffectValue = 0;
            this.LblMsg.Enabled = false;
            this.LblMsg.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.LblMsg.ForeColor = System.Drawing.Color.Black;
            this.LblMsg.Location = new System.Drawing.Point(0, 0);
            this.LblMsg.Name = "LblMsg";
            this.LblMsg.Size = new System.Drawing.Size(273, 86);
            this.LblMsg.TabIndex = 11;
            this.LblMsg.Text = "正在安装";
            this.LblMsg.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.LblMsg.TextEffect = DSkin.DirectUI.TextEffects.Glow;
            this.LblMsg.TextRenderMode = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
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
            this.BtnOk.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.BtnOk.HoverImage = null;
            this.BtnOk.IsPureColor = true;
            this.BtnOk.Location = new System.Drawing.Point(58, 128);
            this.BtnOk.Name = "BtnOk";
            this.BtnOk.NormalImage = null;
            this.BtnOk.PressColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.BtnOk.PressedImage = null;
            this.BtnOk.Radius = 0;
            this.BtnOk.ShowButtonBorder = true;
            this.BtnOk.Size = new System.Drawing.Size(75, 29);
            this.BtnOk.TabIndex = 1;
            this.BtnOk.Text = "确定";
            this.BtnOk.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.BtnOk.TextEffect = DSkin.DirectUI.TextEffects.Glow;
            this.BtnOk.TextPadding = 0;
            this.BtnOk.Click += new System.EventHandler(this.BtnOk_Click);
            // 
            // BtnCancel
            // 
            this.BtnCancel.AdaptImage = true;
            this.BtnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.BtnCancel.BaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(255)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.BtnCancel.ButtonBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.BtnCancel.ButtonBorderWidth = 1;
            this.BtnCancel.DialogResult = System.Windows.Forms.DialogResult.None;
            this.BtnCancel.Font = new System.Drawing.Font("微软雅黑", 10F);
            this.BtnCancel.ForeColor = System.Drawing.Color.White;
            this.BtnCancel.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(255)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.BtnCancel.HoverImage = null;
            this.BtnCancel.IsPureColor = true;
            this.BtnCancel.Location = new System.Drawing.Point(139, 128);
            this.BtnCancel.Name = "BtnCancel";
            this.BtnCancel.NormalImage = null;
            this.BtnCancel.PressColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(255)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.BtnCancel.PressedImage = null;
            this.BtnCancel.Radius = 0;
            this.BtnCancel.ShowButtonBorder = true;
            this.BtnCancel.Size = new System.Drawing.Size(75, 29);
            this.BtnCancel.TabIndex = 2;
            this.BtnCancel.Text = "取消";
            this.BtnCancel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.BtnCancel.TextEffect = DSkin.DirectUI.TextEffects.Glow;
            this.BtnCancel.TextPadding = 0;
            this.BtnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
            // 
            // FrmMessage
            // 
            this.AnimationType = DSkin.Forms.AnimationTypes.FadeinFadeoutEffect;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.RoyalBlue;
            this.CanResize = false;
            this.ClientSize = new System.Drawing.Size(273, 162);
            this.CloseBox.HoverImage = ((System.Drawing.Image)(resources.GetObject("FrmMessage.CloseBox.HoverImage")));
            this.CloseBox.NormalImage = ((System.Drawing.Image)(resources.GetObject("FrmMessage.CloseBox.NormalImage")));
            this.CloseBox.PressImage = ((System.Drawing.Image)(resources.GetObject("FrmMessage.CloseBox.PressImage")));
            this.CloseBox.Size = new System.Drawing.Size(30, 27);
            this.Controls.Add(this.BtnCancel);
            this.Controls.Add(this.BtnOk);
            this.Controls.Add(this.PnlMain);
            this.DoubleClickMaximized = false;
            this.EnableAnimation = true;
            this.MaxBox.HoverImage = ((System.Drawing.Image)(resources.GetObject("FrmMessage.MaxBox.HoverImage")));
            this.MaxBox.NormalImage = ((System.Drawing.Image)(resources.GetObject("FrmMessage.MaxBox.NormalImage")));
            this.MaxBox.PressImage = ((System.Drawing.Image)(resources.GetObject("FrmMessage.MaxBox.PressImage")));
            this.MaxBox.Size = new System.Drawing.Size(30, 27);
            this.MaximizeBox = false;
            this.MinBox.HoverImage = ((System.Drawing.Image)(resources.GetObject("FrmMessage.MinBox.HoverImage")));
            this.MinBox.NormalImage = ((System.Drawing.Image)(resources.GetObject("FrmMessage.MinBox.NormalImage")));
            this.MinBox.PressImage = ((System.Drawing.Image)(resources.GetObject("FrmMessage.MinBox.PressImage")));
            this.MinBox.Size = new System.Drawing.Size(30, 27);
            this.MinimizeBox = false;
            this.Name = "FrmMessage";
            this.NormalBox.HoverImage = ((System.Drawing.Image)(resources.GetObject("FrmMessage.NormalBox.HoverImage")));
            this.NormalBox.NormalImage = ((System.Drawing.Image)(resources.GetObject("FrmMessage.NormalBox.NormalImage")));
            this.NormalBox.PressImage = ((System.Drawing.Image)(resources.GetObject("FrmMessage.NormalBox.PressImage")));
            this.NormalBox.Size = new System.Drawing.Size(30, 27);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "系统消息";
            this.PnlMain.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private DSkin.Controls.DSkinPanel PnlMain;
        private DSkin.Controls.DSkinButton BtnOk;
        private DSkin.Controls.DSkinButton BtnCancel;
        private DSkin.Controls.DSkinLabel LblMsg;
    }
}