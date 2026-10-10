using System;
using System.Drawing;
using System.Windows.Forms;
using Installer.UI.Common;

namespace Installer.UI.Views
{
    /// <summary>
    /// 「附加任务」里的一张任务卡（设计稿 2×2 网格中的一格，282x68，圆角 12）。
    ///
    /// 结构：复选框 + 图标块 + （13px Bold 标题 / 12px 灰描述）。
    /// 抽成独立 UserControl 是为了让 <see cref="PathView"/> 的控件数不爆表
    /// （winforms 技能 R5：单个容器超过 15~20 个控件就该拆）。
    ///
    /// 卡片有两套视觉，跟随勾选状态切换（设计稿里勾选的卡是 #F5F7FA 无描边，
    /// 未勾选的卡是白底 + 1px 描边）：
    ///   勾选   → 卡片底 #F5F7FA、无描边；图标块 #E6F4FF；图标 #1677FF
    ///   未勾选 → 卡片底 白、1px #E5E7EB；图标块 #F5F7FA；图标 #9CA3AF
    /// </summary>
    public partial class TaskCardView : UserControl
    {
        /// <summary>图标 SVG 模板（描边色统一写成 #1677FF，渲染前按状态替换）。</summary>
        private string _iconSvg;

        /// <summary>当前主色（勾选态的图标描边）。</summary>
        private Color _accent = WizardTheme.Primary;

        /// <summary>构造。</summary>
        public TaskCardView()
        {
            InitializeComponent();

            _iconSvg = lblIcon.PrefixSvg;

            ApplyAccent(WizardTheme.Primary);
            ApplyCheckedVisual();

            chkTask.CheckedChanged += (sender, e) =>
            {
                ApplyCheckedVisual();

                var handler = CheckedChangedInternal;
                if (handler != null)
                {
                    handler(Checked);
                }
            };
        }

        /// <summary>勾选状态变化（参数为新状态）。</summary>
        public event Action<bool> CheckedChangedInternal;

        /// <summary>标题。</summary>
        public string Title
        {
            get { return lblTitle.Text; }
            set { lblTitle.Text = value; }
        }

        /// <summary>描述。</summary>
        public string Description
        {
            get { return lblDesc.Text; }
            set { lblDesc.Text = value; }
        }

        /// <summary>图标 SVG（模板，描边色写 #1677FF 即可，未勾选时会自动变灰）。</summary>
        public string IconSvg
        {
            get { return _iconSvg; }
            set
            {
                _iconSvg = value ?? string.Empty;
                ApplyIconColor();
            }
        }

        /// <summary>是否勾选。</summary>
        public bool Checked
        {
            get { return chkTask.Checked; }
            set { chkTask.Checked = value; }
        }

        /// <summary>设置主色（勾选态图标与复选框跟随）。</summary>
        public void ApplyAccent(Color accent)
        {
            chkTask.Fill = accent;
            _accent = accent;
            ApplyIconColor();
        }

        /// <summary>按勾选状态切换卡片 / 图标块 / 描边。</summary>
        private void ApplyCheckedVisual()
        {
            if (chkTask.Checked)
            {
                cardPanel.Back = WizardTheme.SoftBack;
                cardPanel.BorderWidth = 0F;
                boxIcon.Back = WizardTheme.PrimarySoft;
            }
            else
            {
                cardPanel.Back = Color.White;
                cardPanel.BorderColor = WizardTheme.Border;
                cardPanel.BorderWidth = 1F;
                boxIcon.Back = WizardTheme.SoftBack;
            }

            ApplyIconColor();
        }

        /// <summary>图标描边色跟随状态（勾选蓝、未勾选灰）。</summary>
        private void ApplyIconColor()
        {
            var color = chkTask.Checked ? _accent : WizardTheme.TextMuted;

            if (string.IsNullOrEmpty(_iconSvg))
            {
                lblIcon.PrefixSvg = null;
                return;
            }

            lblIcon.PrefixSvg = _iconSvg.Replace("#1677FF", ToHex(color));
        }

        private static string ToHex(Color color)
        {
            return "#" + color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2");
        }
    }
}
