using System;
using System.Windows.Forms;

namespace Installer.UI.Views
{
    /// <summary>安装位置页。</summary>
    public partial class PathView : UserControl
    {
        /// <summary>构造。</summary>
        public PathView()
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

        /// <summary>路径标签。</summary>
        public string PathLabel
        {
            get { return lblPath.Text; }
            set { lblPath.Text = value; }
        }

        /// <summary>浏览按钮文案。</summary>
        public string BrowseText
        {
            get { return btnBrowse.Text; }
            set { btnBrowse.Text = value; }
        }

        /// <summary>安装路径。</summary>
        public string InstallPath
        {
            get { return txtPath.Text; }
            set { txtPath.Text = value; }
        }

        /// <summary>空间提示。</summary>
        public string SpaceText
        {
            get { return lblSpace.Text; }
            set { lblSpace.Text = value; }
        }

        /// <summary>点"浏览"。</summary>
        public event Action BrowseClick;

        /// <summary>路径变化。</summary>
        public event Action PathChanged;

        /// <summary>由构造函数挂接（避免把 AntdUI 的事件类型泄露到公开 API）。</summary>
        private void HookEvents()
        {
            btnBrowse.Click += (sender, e) =>
            {
                var handler = BrowseClick;
                if (handler != null)
                {
                    handler();
                }
            };

            txtPath.TextChanged += (sender, e) =>
            {
                var handler = PathChanged;
                if (handler != null)
                {
                    handler();
                }
            };
        }
    }
}
