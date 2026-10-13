using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Installer.UI.Common;

namespace Installer.UI.Views
{
    /// <summary>
    /// 安装完成 / 安装失败 / 卸载完成页（设计稿「安装完成3」/「卸载完成」）。
    ///
    /// 结构：左侧 80×80 圆徽（成功绿 / 失败红）+ 标题 + 正文
    ///       + 绿色标签 chips + 勾选行（立即运行）+ 灰色卸载提示。
    ///
    /// 逐元素坐标见 FinishView.Designer.cs 顶部的换算表。
    /// 本页被「安装完成」和「卸载完成」复用：SetState / SetNeutral / ShowRunOption
    /// 只改颜色与可见性，不动坐标。
    /// </summary>
    public partial class FinishView : UserControl
    {
        /// <summary>第一个 chip 的 x（设计稿 160 → 视图 104）。</summary>
        private const int ChipStartX = 104;

        /// <summary>chip 的 y（设计稿 292 → 视图 100）。</summary>
        private const int ChipTop = 100;

        /// <summary>chip 高（设计稿 26）。</summary>
        private const int ChipHeight = 26;

        /// <summary>chip 间距（设计稿 8）。</summary>
        private const int ChipGap = 8;

        /// <summary>chip 左内边距（设计稿 10）。</summary>
        private const int ChipPadLeft = 10;

        /// <summary>chip 右内边距（设计稿 13）。</summary>
        private const int ChipPadRight = 13;

        /// <summary>chip 里的 ✓ 图标宽（设计稿 12）。</summary>
        private const int ChipIconWidth = 12;

        /// <summary>chip 里图标与文字的间距（设计稿 4）。</summary>
        private const int ChipIconGap = 4;

        /// <summary>构造。</summary>
        public FinishView()
        {
            InitializeComponent();

            // 文案是独立标签，点它等于点复选框。
            lblRun.Click += LblRunClick;
            chkRun.CheckedChanged += ChkRunCheckedChanged;

            ApplyAccent(WizardTheme.Success);
        }

        /// <summary>标题。</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Title
        {
            get { return lblTitle.Text; }
            set { lblTitle.Text = value; }
        }

        /// <summary>正文。</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Body
        {
            get { return lblBody.Text; }
            set { lblBody.Text = value; }
        }

        /// <summary>勾选框文案（「立即运行 xxx」）。</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string RunText
        {
            get { return lblRun.Text; }
            set { lblRun.Text = value; }
        }

        /// <summary>勾选框下方的灰色提示。</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string RunHint
        {
            get { return lblRunHint.Text; }
            set { lblRunHint.Text = value ?? string.Empty; }
        }

        /// <summary>最下方的灰色提示（卸载指引）。</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string FooterHint
        {
            get { return lblUninstallHint.Text; }
            set { lblUninstallHint.Text = value ?? string.Empty; }
        }

        /// <summary>是否显示「立即运行」勾选区。</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ShowRunOption
        {
            get { return runPanel.Visible; }
            set { runPanel.Visible = value; }
        }

        /// <summary>勾选状态。</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool RunChecked
        {
            get { return chkRun.Checked; }
            set { chkRun.Checked = value; }
        }

        /// <summary>设置绿色标签（最多 3 个）。空的会被隐藏，其余按设计稿间距左移。</summary>
        public void SetChips(string a, string b, string c)
        {
            var panels = new[] { chipPanel1, chipPanel2, chipPanel3 };
            var labels = new[] { chip1, chip2, chip3 };
            var texts = new[] { a, b, c };
            var x = ChipStartX;

            for (var i = 0; i < panels.Length; i++)
            {
                var text = texts[i];
                var show = !string.IsNullOrEmpty(text);

                panels[i].Visible = show;
                labels[i].Text = show ? text : string.Empty;

                if (!show)
                {
                    continue;
                }

                // 宽 = 左内边距 + 图标 + 间距 + 文字 + 右内边距（设计稿 111 / 87 / 123）
                var inner = ChipIconWidth + ChipIconGap + MeasureChipText(text, labels[i].Font);
                var width = ChipPadLeft + inner + ChipPadRight;

                panels[i].Location = new Point(x, ChipTop);
                panels[i].Size = new Size(width, ChipHeight);
                labels[i].Size = new Size(inner, ChipHeight);
                x += width + ChipGap;
            }
        }

        /// <summary>成功态（绿徽 + 勾）或失败态（红徽 + 叉）。</summary>
        public void SetState(bool success)
        {
            ApplyAccent(success ? WizardTheme.Success : WizardTheme.Danger);

            if (!success)
            {
                lblBadge.PrefixSvg = "<svg viewBox=\"0 0 24 24\"><path d=\"M7 7l10 10M17 7L7 17\" fill=\"none\" stroke=\"#FFFFFF\" stroke-width=\"2.6\" stroke-linecap=\"round\"/></svg>";
            }
            else
            {
                lblBadge.PrefixSvg = "<svg viewBox=\"0 0 24 24\"><path d=\"M5 12.6l4.6 4.6L19 7.4\" fill=\"none\" stroke=\"#FFFFFF\" stroke-width=\"2.6\" stroke-linecap=\"round\" stroke-linejoin=\"round\"/></svg>";
            }
        }

        /// <summary>中性态（没有明确成败时）。</summary>
        public void SetNeutral()
        {
            ApplyAccent(WizardTheme.TextMuted);
            ShowRunOption = false;
        }

        /// <summary>设置徽标颜色（同时影响标签配色）。</summary>
        public void ApplyAccent(Color accent)
        {
            badgePanel.Back = accent;

            runPanel.Back = WizardTheme.SoftBack;

            lblTitle.ForeColor = WizardTheme.TextTitle;
            lblBody.ForeColor = WizardTheme.TextBody;

            foreach (var panel in new[] { chipPanel1, chipPanel2, chipPanel3 })
            {
                panel.Back = WizardTheme.SuccessBack;
            }

            foreach (var chip in new[] { chip1, chip2, chip3 })
            {
                chip.ForeColor = WizardTheme.Success;
            }
        }

        private void LblRunClick(object sender, EventArgs e)
        {
            chkRun.Checked = !chkRun.Checked;
        }

        private void ChkRunCheckedChanged(object sender, EventArgs e)
        {
            // 勾选行整体作为一个开关：文案颜色跟着走，未勾选时压暗。
            lblRun.ForeColor = chkRun.Checked ? WizardTheme.TextTitle : WizardTheme.TextMuted;
        }

        /// <summary>
        /// 量一段 chip 文字的实际宽度。
        ///
        /// 用 GDI+ 的 GenericTypographic 而不是 TextRenderer.MeasureText：
        /// 后者会带上 GDI 的字距溢出（6 个汉字量出 80 而不是 72），
        /// 每个 chip 都会胖 8px，三个 chip 就整体右移 24px，和设计稿对不上。
        /// GenericTypographic 得到的就是字形的推进宽度，汉字 = 1em，
        /// 「桌面快捷方式」= 6×12 = 72，chip 宽 10+12+4+72+13 = 111，与设计稿一致。
        /// </summary>
        private static int MeasureChipText(string text, Font font)
        {
            using (var g = Graphics.FromHwnd(IntPtr.Zero))
            {
                return (int)Math.Ceiling(g.MeasureString(text, font, int.MaxValue, StringFormat.GenericTypographic).Width);
            }
        }
    }
}
