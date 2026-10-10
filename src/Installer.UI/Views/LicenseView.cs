using System;
using System.Drawing;
using System.Windows.Forms;
using Installer.UI.Common;

namespace Installer.UI.Views
{
    /// <summary>
    /// 许可协议页（设计稿第 2 步）。
    ///
    /// 结构：标题 + 副标题 + 描边卡片（内含可滚动正文）+ 浅灰勾选行（复选框 + 提示）。
    /// 未勾选时由 Presenter 让「下一步」保持禁用。
    /// </summary>
    public partial class LicenseView : UserControl
    {
        /// <summary>构造。</summary>
        public LicenseView()
        {
            InitializeComponent();
            HookEvents();
            ApplyAccent(WizardTheme.Primary);
        }

        /// <summary>标题。</summary>
        public string Title
        {
            get { return lblTitle.Text; }
            set { lblTitle.Text = value; }
        }

        /// <summary>副标题。</summary>
        public string Subtitle
        {
            get { return lblSubtitle.Text; }
            set { lblSubtitle.Text = value; }
        }

        /// <summary>协议正文。</summary>
        public string LicenseText
        {
            get { return txtLicense.Text; }
            set { txtLicense.Text = value; }
        }

        /// <summary>接受文案。</summary>
        public string AcceptText
        {
            get { return chkAccept.Text; }
            set { chkAccept.Text = value; }
        }

        /// <summary>勾选行下方的灰色提示。</summary>
        public string AcceptHint
        {
            get { return lblAcceptHint.Text; }
            set { lblAcceptHint.Text = value ?? string.Empty; }
        }

        /// <summary>是否已接受。</summary>
        public bool Accepted
        {
            get { return chkAccept.Checked; }
            set { chkAccept.Checked = value; }
        }

        /// <summary>接受状态变化。参数为新的接受状态。</summary>
        public event Action<bool> AcceptedChanged;

        /// <summary>设置主色（安装 = 蓝，卸载 = 红）。</summary>
        public void ApplyAccent(Color accent)
        {
            acceptPanel.Back = WizardTheme.SoftBack;

            lblTitle.ForeColor = WizardTheme.TextTitle;
            lblSubtitle.ForeColor = WizardTheme.TextBody;
        }

        /// <summary>由构造函数挂接（Designer 里只订阅 WinForms 认得的事件）。</summary>
        private void HookEvents()
        {
            chkAccept.CheckedChanged += (sender, e) =>
            {
                var handler = AcceptedChanged;
                if (handler != null)
                {
                    handler(e.Value);
                }
            };
        }
    }
}
