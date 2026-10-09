using UnInstall.Properties;

namespace UnInstall
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
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(213)))), ((int)(((byte)(47)))), ((int)(((byte)(47)))));
            this.CaptionFont = new System.Drawing.Font("微软雅黑", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.CaptionOffset = new System.Drawing.Point(10, 3);
            this.CaptionShowMode = DSkin.TextShowModes.Ordinary;
            this.CaptionTextRender = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            this.ClientSize = new System.Drawing.Size(344, 267);
            this.CloseBox.HoverImage = ((System.Drawing.Image)(resources.GetObject("FrmBase.CloseBox.HoverImage")));
            this.CloseBox.NormalImage = ((System.Drawing.Image)(resources.GetObject("FrmBase.CloseBox.NormalImage")));
            this.CloseBox.PressImage = ((System.Drawing.Image)(resources.GetObject("FrmBase.CloseBox.PressImage")));
            this.CloseBox.Size = new System.Drawing.Size(30, 27);
            this.EnableAnimation = false;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.IconRectangle = new System.Drawing.Rectangle(0, 0, 32, 32);
            this.MaxBox.HoverImage = ((System.Drawing.Image)(resources.GetObject("FrmBase.MaxBox.HoverImage")));
            this.MaxBox.NormalImage = ((System.Drawing.Image)(resources.GetObject("FrmBase.MaxBox.NormalImage")));
            this.MaxBox.PressImage = ((System.Drawing.Image)(resources.GetObject("FrmBase.MaxBox.PressImage")));
            this.MaxBox.Size = new System.Drawing.Size(30, 27);
            this.MinBox.HoverImage = ((System.Drawing.Image)(resources.GetObject("FrmBase.MinBox.HoverImage")));
            this.MinBox.NormalImage = ((System.Drawing.Image)(resources.GetObject("FrmBase.MinBox.NormalImage")));
            this.MinBox.PressImage = ((System.Drawing.Image)(resources.GetObject("FrmBase.MinBox.PressImage")));
            this.MinBox.Size = new System.Drawing.Size(30, 27);
            this.Name = "FrmBase";
            this.NormalBox.HoverImage = ((System.Drawing.Image)(resources.GetObject("FrmBase.NormalBox.HoverImage")));
            this.NormalBox.NormalImage = ((System.Drawing.Image)(resources.GetObject("FrmBase.NormalBox.NormalImage")));
            this.NormalBox.PressImage = ((System.Drawing.Image)(resources.GetObject("FrmBase.NormalBox.PressImage")));
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

