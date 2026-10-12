namespace Installer.UI.Views
{
    partial class WizardHeaderView
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
            this.brandPanel = new AntdUI.Panel();
            this.iconPanel = new AntdUI.Panel();
            this.lblIcon = new AntdUI.Label();
            this.lblProduct = new AntdUI.Label();
            this.lblSubtitle = new AntdUI.Label();
            this.brandPanel.SuspendLayout();
            this.iconPanel.SuspendLayout();
            this.SuspendLayout();
            //
            // brandPanel —— 蓝色品牌区，上方两角随卡片圆角
            //
            // 注意：这里**故意不用内层 TableLayoutPanel**。
            // 内层容器会用它自己的不透明背景铺满整个品牌区，
            // 把 AntdUI.Panel 的圆角填充盖成直角（渲染出来就是方块）。
            // 品牌区是固定尺寸区域，三个元素直接绝对定位即可。
            //
            this.brandPanel.Controls.Add(this.iconPanel);
            this.brandPanel.Controls.Add(this.lblProduct);
            this.brandPanel.Controls.Add(this.lblSubtitle);
            this.brandPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.brandPanel.Location = new System.Drawing.Point(0, 0);
            this.brandPanel.Margin = new System.Windows.Forms.Padding(0);
            this.brandPanel.Name = "brandPanel";
            this.brandPanel.Radius = 16;
            this.brandPanel.RadiusAlign = AntdUI.TAlignRound.Top;
            this.brandPanel.Size = new System.Drawing.Size(640, 104);
            this.brandPanel.TabIndex = 0;
            //
            // iconPanel —— 56x56 圆角块（白 18% 叠在品牌蓝上）
            //
            this.iconPanel.Controls.Add(this.lblIcon);
            this.iconPanel.Location = new System.Drawing.Point(32, 24);
            this.iconPanel.Margin = new System.Windows.Forms.Padding(0);
            this.iconPanel.Name = "iconPanel";
            this.iconPanel.Radius = 16;
            this.iconPanel.Size = new System.Drawing.Size(56, 56);
            this.iconPanel.TabIndex = 0;
            //
            // lblIcon —— 白色立方体（产品图标占位）
            //
            this.lblIcon.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblIcon.AutoSize = false;
            this.lblIcon.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.lblIcon.IconGap = 0;
            // 设计稿墨迹 16x18 且居中于 56x56 容器；AntdUI 默认只画 9x10 且贴左上
            this.lblIcon.IconRatio = 1.2F;
            this.lblIcon.Location = new System.Drawing.Point(17, 16);
            this.lblIcon.Margin = new System.Windows.Forms.Padding(0);
            this.lblIcon.Name = "lblIcon";
            this.lblIcon.PrefixSvg = "<svg viewBox=\"0 0 24 24\"><path d=\"M12 2.6 3.4 7v10L12 21.4 20.6 17V7L12 2.6z\" fill=\"none\" stroke=\"#FFFFFF\" stroke-width=\"1.6\"/><path d=\"M3.6 7.1 12 11.5l8.4-4.4M12 11.5v9.7\" fill=\"none\" stroke=\"#FFFFFF\" stroke-width=\"1.6\"/></svg>";
            this.lblIcon.Size = new System.Drawing.Size(24, 24);
            this.lblIcon.TabIndex = 0;
            this.lblIcon.Text = " ";
            //
            // lblProduct —— 产品名 20px = 15pt SemiBold 白
            //
            this.lblProduct.AutoSize = true;
            this.lblProduct.Font = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Bold);
            this.lblProduct.Location = new System.Drawing.Point(104, 26);
            this.lblProduct.Margin = new System.Windows.Forms.Padding(0);
            this.lblProduct.Name = "lblProduct";
            this.lblProduct.Size = new System.Drawing.Size(200, 26);
            this.lblProduct.TabIndex = 1;
            this.lblProduct.Text = "产品名称";
            //
            // lblSubtitle —— 「安装向导 · 版本 1.0.0」13px = 9.75pt
            //
            this.lblSubtitle.AutoSize = true;
            this.lblSubtitle.Font = new System.Drawing.Font("微软雅黑", 9.75F);
            this.lblSubtitle.Location = new System.Drawing.Point(104, 58);
            this.lblSubtitle.Margin = new System.Windows.Forms.Padding(0);
            this.lblSubtitle.Name = "lblSubtitle";
            this.lblSubtitle.Size = new System.Drawing.Size(200, 17);
            this.lblSubtitle.TabIndex = 2;
            this.lblSubtitle.Text = "安装向导 · 版本 1.0.0";
            //
            // WizardHeaderView
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.brandPanel);
            this.Font = new System.Drawing.Font("微软雅黑", 9.75F);
            this.Name = "WizardHeaderView";
            this.Size = new System.Drawing.Size(640, 104);
            this.brandPanel.ResumeLayout(false);
            this.brandPanel.PerformLayout();
            this.iconPanel.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private AntdUI.Panel brandPanel;
        private AntdUI.Panel iconPanel;
        private AntdUI.Label lblIcon;
        private AntdUI.Label lblProduct;
        private AntdUI.Label lblSubtitle;
    }
}
