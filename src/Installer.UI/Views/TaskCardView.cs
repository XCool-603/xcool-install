using System;
using System.Drawing;
using System.Windows.Forms;
using Installer.UI.Common;

namespace Installer.UI.Views
{
    /// <summary>
    /// 「附加任务」里的一张任务卡（设计稿 2×2 网格中的一格）。
    ///
    /// 结构：复选框 + 图标 + （12F Bold 标题 / 11F 灰描述）。
    /// 抽成独立 UserControl 是为了让 <see cref="PathView"/> 的控件数不爆表
    /// （winforms 技能 R5：单个容器超过 15~20 个控件就该拆）。
    /// </summary>
    public partial class TaskCardView : UserControl
    {
        /// <summary>构造。</summary>
        public TaskCardView()
        {
            InitializeComponent();
            ApplyAccent(WizardTheme.Primary);

            chkTask.CheckedChanged += (sender, e) =>
            {
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

        /// <summary>图标 SVG。</summary>
        public string IconSvg
        {
            get { return lblIcon.PrefixSvg; }
            set { lblIcon.PrefixSvg = value; }
        }

        /// <summary>是否勾选。</summary>
        public bool Checked
        {
            get { return chkTask.Checked; }
            set { chkTask.Checked = value; }
        }

        /// <summary>设置主色（图标描边跟随）。</summary>
        public void ApplyAccent(Color accent)
        {
            cardPanel.Back = WizardTheme.SoftBack;
            cardPanel.BackColor = WizardTheme.SoftBack;
        }
    }
}
