using System;
using System.Windows.Forms;

namespace Installer.UI.Views
{
    /// <summary>完成页 / 失败页（同一个视图，靠文案与图标区分）。</summary>
    public partial class FinishView : UserControl
    {
        /// <summary>构造。</summary>
        public FinishView()
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

        /// <summary>"安装完成后运行"的文案。</summary>
        public string RunText
        {
            get { return chkRun.Text; }
            set { chkRun.Text = value; }
        }

        /// <summary>是否勾选"立即运行"。</summary>
        public bool RunChecked
        {
            get { return chkRun.Checked; }
            set { chkRun.Checked = value; }
        }

        /// <summary>是否显示"立即运行"。</summary>
        public bool ShowRunOption
        {
            get { return chkRun.Visible; }
            set { chkRun.Visible = value; }
        }

        /// <summary>设置成功/失败外观。</summary>
        public void SetState(bool success)
        {
            icon.Visible = true;
            icon.Text = success ? "✔" : "✖";
            icon.ForeColor = success ? System.Drawing.Color.FromArgb(82, 196, 26)
                                     : System.Drawing.Color.FromArgb(255, 77, 79);
        }

        /// <summary>中性态：不显示成功/失败图标（用于"卸载确认"这类既非成功也非失败的页面）。</summary>
        public void SetNeutral()
        {
            icon.Visible = false;
        }
    }
}
