using System;
using System.Windows.Forms;

namespace Installer.UI.Views
{
    /// <summary>许可协议页。</summary>
    public partial class LicenseView : UserControl
    {
        /// <summary>构造。</summary>
        public LicenseView()
        {
            InitializeComponent();
            HookEvents();
        }

        /// <summary>标题。</summary>
        public string Title
        {
            get { return lblTitle.Text; }
            set { lblTitle.Text = value; }
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

        /// <summary>是否已接受。</summary>
        public bool Accepted
        {
            get { return chkAccept.Checked; }
            set { chkAccept.Checked = value; }
        }

        /// <summary>接受状态变化。参数为新的接受状态。</summary>
        public event Action<bool> AcceptedChanged;

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
