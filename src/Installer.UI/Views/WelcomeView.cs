using System;
using System.Windows.Forms;

namespace Installer.UI.Views
{
    /// <summary>欢迎页。</summary>
    public partial class WelcomeView : UserControl
    {
        /// <summary>构造。</summary>
        public WelcomeView()
        {
            InitializeComponent();
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

        /// <summary>Logo 图片（可为 null）。</summary>
        public System.Drawing.Image Logo
        {
            get { return picLogo.Image; }
            set
            {
                picLogo.Image = value;
                picLogo.Visible = value != null;
            }
        }
    }
}
