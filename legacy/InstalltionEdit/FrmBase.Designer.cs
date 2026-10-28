namespace InstalltionEdit
{
    partial class FrmBase
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
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmBase));
            this.SuspendLayout();
            // 
            // FrmBase
            // 
            this.AnimationType = DSkin.Forms.AnimationTypes.Custom;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DodgerBlue;
            this.CaptionFont = new System.Drawing.Font("微软雅黑", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.CaptionOffset = new System.Drawing.Point(10, 3);
            this.CaptionShowMode = DSkin.TextShowModes.Ordinary;
            this.CaptionTextRender = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            this.ClientSize = new System.Drawing.Size(399, 310);
            this.CloseBox.NormalColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.CloseBox.Size = new System.Drawing.Size(30, 27);
            this.EnableAnimation = false;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.IconRectangle = new System.Drawing.Rectangle(4, 6, 24, 24);
            this.MaxBox.NormalColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.MaxBox.PressBackColor = System.Drawing.Color.DodgerBlue;
            this.MaxBox.Size = new System.Drawing.Size(30, 27);
            this.MinBox.NormalColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.MinBox.PressBackColor = System.Drawing.Color.DodgerBlue;
            this.MinBox.Size = new System.Drawing.Size(30, 27);
            this.Name = "FrmBase";
            this.NormalBox.Size = new System.Drawing.Size(30, 27);
            this.RoundStyle = DSkin.Common.RoundStyle.None;
            this.ShadowWidth = 20;
            this.ShowShadow = true;
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.FrmAnimationBase_Load);
            this.ResumeLayout(false);

        }

        #endregion
    }
}

