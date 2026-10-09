using System;
using System.Drawing;
using System.Windows.Forms;
using Installer.UI.Common;

namespace Installer.UI.Views
{
    /// <summary>
    /// 安装完成 / 安装失败 / 卸载完成页。
    ///
    /// 结构：左侧 64×64 圆徽（成功绿 / 失败红）+ 标题 + 正文
    ///       + 绿色标签 chips + 勾选行（立即运行）+ 灰色提示。
    /// </summary>
    public partial class FinishView : UserControl
    {
        /// <summary>构造。</summary>
        public FinishView()
        {
            InitializeComponent();
            ApplyAccent(WizardTheme.Success);
        }

        /// <summary>标题。</summary>
        public string Title
        {
            get { return lblTitle.Text; }
            set { lblTitle.Text = value; }
        }

        /// <summary>正文。</summary>
        public string Body
        {
            get { return lblBody.Text; }
            set { lblBody.Text = value; }
        }

        /// <summary>勾选框文案（「立即运行 xxx」）。</summary>
        public string RunText
        {
            get { return chkRun.Text; }
            set { chkRun.Text = value; }
        }

        /// <summary>勾选框下方的灰色提示。</summary>
        public string RunHint
        {
            get { return lblRunHint.Text; }
            set { lblRunHint.Text = value ?? string.Empty; }
        }

        /// <summary>最下方的灰色提示（卸载指引）。</summary>
        public string FooterHint
        {
            get { return lblUninstallHint.Text; }
            set { lblUninstallHint.Text = value ?? string.Empty; }
        }

        /// <summary>是否显示「立即运行」勾选区。</summary>
        public bool ShowRunOption
        {
            get { return runPanel.Visible; }
            set { runPanel.Visible = value; }
        }

        /// <summary>勾选状态。</summary>
        public bool RunChecked
        {
            get { return chkRun.Checked; }
            set { chkRun.Checked = value; }
        }

        /// <summary>设置绿色标签（最多 3 个）。</summary>
        public void SetChips(string a, string b, string c)
        {
            var chips = new[] { chip1, chip2, chip3 };
            var texts = new[] { a, b, c };

            for (var i = 0; i < chips.Length; i++)
            {
                var text = texts[i];
                chips[i].Text = text ?? string.Empty;
                chips[i].Visible = !string.IsNullOrEmpty(text);
            }
        }

        /// <summary>成功态（绿徽 + 绿标签）或失败态（红徽）。</summary>
        public void SetState(bool success)
        {
            ApplyAccent(success ? WizardTheme.Success : WizardTheme.Danger);

            if (!success)
            {
                lblBadge.PrefixSvg = "<svg viewBox=\"0 0 24 24\"><path d=\"M7 7l10 10M17 7L7 17\" fill=\"none\" stroke=\"#FFFFFF\" stroke-width=\"3\" stroke-linecap=\"round\"/></svg>";
            }
            else
            {
                lblBadge.PrefixSvg = "<svg viewBox=\"0 0 24 24\"><path d=\"M5 12.6l4.6 4.6L19 7.4\" fill=\"none\" stroke=\"#FFFFFF\" stroke-width=\"3\" stroke-linecap=\"round\" stroke-linejoin=\"round\"/></svg>";
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
            badgePanel.BackColor = accent;

            runPanel.Back = WizardTheme.SoftBack;
            runPanel.BackColor = WizardTheme.SoftBack;

            lblTitle.ForeColor = WizardTheme.TextTitle;
            lblBody.ForeColor = WizardTheme.TextBody;

            foreach (var chip in new[] { chip1, chip2, chip3 })
            {
                chip.BackColor = WizardTheme.SuccessBack;
                chip.ForeColor = WizardTheme.Success;
            }
        }
    }
}
