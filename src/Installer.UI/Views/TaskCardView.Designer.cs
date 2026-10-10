namespace Installer.UI.Views
{
    partial class TaskCardView
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
        /// <remarks>
        /// 本页坐标逐元素照抄 Calicat 设计稿第 3 版「安装设置2」里的任务卡，不做二次推导。
        ///
        /// 卡片在窗口里的绝对位置是 (56,529) / (350,529) / (56,609) / (350,609)，
        /// 尺寸 282x68（圆角 12）。下面写的是「卡内偏移」＝ 设计稿绝对坐标 − 卡片左上角，
        /// 所以四个位置共用同一套卡内数值：
        ///
        ///   复选框   卡内 (14,25)  18x18   圆角 5
        ///   图标块   卡内 (44,20)  28x28   圆角 8   底 #E6F4FF（未勾选时 #F5F7FA）
        ///   图标     卡内 (50,26)  16x16   居中于图标块
        ///   标题     卡内 (84,14)  13px = 9.75pt SemiBold #1F2937
        ///   描述     卡内 (84,36)  12px = 9pt    Regular  #9CA3AF
        ///
        /// 卡片底色随勾选状态切换：勾选 → #F5F7FA 无描边；未勾选 → 白底 + 1px #E5E7EB。
        /// 这部分逻辑在 TaskCardView.cs 的 ApplyCheckedVisual()，不在本文件里。
        ///
        /// 字号换算 pt = 设计稿 px × 0.75（96 DPI）。
        /// AntdUI.Panel 只设 Back，绝不设 WinForms 的 BackColor（设了会用直角盖掉圆角）；
        /// 面板里的 AntdUI.Label 一律 BackColor = Transparent，否则各自画一块白底。
        /// </remarks>
        private void InitializeComponent()
        {
            this.cardPanel = new AntdUI.Panel();
            this.lblTitle = new AntdUI.Label();
            this.lblDesc = new AntdUI.Label();
            this.boxIcon = new AntdUI.Panel();
            this.lblIcon = new AntdUI.Label();
            this.chkTask = new AntdUI.Checkbox();
            this.cardPanel.SuspendLayout();
            this.boxIcon.SuspendLayout();
            this.SuspendLayout();
            //
            // cardPanel —— 卡片本体，铺满 282x68，圆角 12
            //
            this.cardPanel.Controls.Add(this.lblDesc);
            this.cardPanel.Controls.Add(this.lblTitle);
            this.cardPanel.Controls.Add(this.boxIcon);
            this.cardPanel.Controls.Add(this.chkTask);
            this.cardPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cardPanel.Location = new System.Drawing.Point(0, 0);
            this.cardPanel.Margin = new System.Windows.Forms.Padding(0);
            this.cardPanel.Name = "cardPanel";
            this.cardPanel.Padding = new System.Windows.Forms.Padding(0);
            this.cardPanel.Radius = 12;
            this.cardPanel.Size = new System.Drawing.Size(282, 68);
            this.cardPanel.TabIndex = 0;
            //
            // chkTask —— 卡内 (14,25) 18x18
            //
            this.chkTask.AutoSize = false;
            this.chkTask.Fill = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(119)))), ((int)(((byte)(255)))));
            this.chkTask.Location = new System.Drawing.Point(14, 25);
            this.chkTask.Margin = new System.Windows.Forms.Padding(0);
            this.chkTask.Name = "chkTask";
            this.chkTask.Size = new System.Drawing.Size(18, 18);
            this.chkTask.TabIndex = 0;
            this.chkTask.Text = "";
            //
            // boxIcon —— 卡内 (44,20) 28x28，圆角 8，底 #E6F4FF
            //
            this.boxIcon.Controls.Add(this.lblIcon);
            this.boxIcon.Location = new System.Drawing.Point(44, 20);
            this.boxIcon.Margin = new System.Windows.Forms.Padding(0);
            this.boxIcon.Name = "boxIcon";
            this.boxIcon.Radius = 8;
            this.boxIcon.Size = new System.Drawing.Size(28, 28);
            this.boxIcon.TabIndex = 1;
            //
            // lblIcon —— 图标块内居中 16x16
            //
            this.lblIcon.AutoSize = false;
            this.lblIcon.BackColor = System.Drawing.Color.Transparent;
            this.lblIcon.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.lblIcon.IconGap = 0;
            this.lblIcon.Location = new System.Drawing.Point(6, 6);
            this.lblIcon.Margin = new System.Windows.Forms.Padding(0);
            this.lblIcon.Name = "lblIcon";
            this.lblIcon.PrefixSvg = "<svg viewBox=\"0 0 24 24\"><rect x=\"1.2\" y=\"2.6\" width=\"21.6\" height=\"14.8\" rx=\"1.8\" fill=\"none\" stroke=\"#1677FF\" stroke-width=\"2.4\"/><path d=\"M12 17.4v3\" stroke=\"#1677FF\" stroke-width=\"2.4\"/><path d=\"M7.6 20.4h8.8\" stroke=\"#1677FF\" stroke-width=\"2.4\" stroke-linecap=\"round\"/></svg>";
            this.lblIcon.Size = new System.Drawing.Size(16, 16);
            this.lblIcon.TabIndex = 0;
            this.lblIcon.Text = " ";
            //
            // lblTitle —— 卡内 (84,14) 184x20，13px = 9.75pt SemiBold #1F2937
            //
            // y 取 14 而不是设计稿的字形顶 18：AntdUI.Label 的文字比控件顶边低约 4px，
            // 14 + 4 = 18 正好落在设计稿的字形顶。下面 lblDesc 同理。
            //
            this.lblTitle.AutoSize = false;
            this.lblTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblTitle.Font = new System.Drawing.Font("微软雅黑", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(41)))), ((int)(((byte)(55)))));
            this.lblTitle.Location = new System.Drawing.Point(84, 14);
            this.lblTitle.Margin = new System.Windows.Forms.Padding(0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(184, 20);
            this.lblTitle.TabIndex = 2;
            this.lblTitle.Text = "创建桌面快捷方式";
            //
            // lblDesc —— 卡内 (84,36) 184x18，12px = 9pt #9CA3AF
            //
            this.lblDesc.AutoSize = false;
            this.lblDesc.BackColor = System.Drawing.Color.Transparent;
            this.lblDesc.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.lblDesc.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(163)))), ((int)(((byte)(175)))));
            this.lblDesc.Location = new System.Drawing.Point(84, 36);
            this.lblDesc.Margin = new System.Windows.Forms.Padding(0);
            this.lblDesc.Name = "lblDesc";
            this.lblDesc.Size = new System.Drawing.Size(184, 18);
            this.lblDesc.TabIndex = 3;
            this.lblDesc.Text = "在桌面创建启动图标";
            //
            // TaskCardView
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.cardPanel);
            this.Font = new System.Drawing.Font("微软雅黑", 9.75F);
            this.Name = "TaskCardView";
            this.Size = new System.Drawing.Size(282, 68);
            this.cardPanel.ResumeLayout(false);
            this.boxIcon.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private AntdUI.Panel cardPanel;
        private AntdUI.Checkbox chkTask;
        private AntdUI.Panel boxIcon;
        private AntdUI.Label lblIcon;
        private AntdUI.Label lblTitle;
        private AntdUI.Label lblDesc;
    }
}
