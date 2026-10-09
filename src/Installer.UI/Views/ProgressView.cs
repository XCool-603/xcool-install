using System;
using System.Windows.Forms;

namespace Installer.UI.Views
{
    /// <summary>进度页。</summary>
    public partial class ProgressView : UserControl
    {
        /// <summary>构造。</summary>
        public ProgressView()
        {
            InitializeComponent();
        }

        /// <summary>标题。</summary>
        public string Title
        {
            get { return lblTitle.Text; }
            set { lblTitle.Text = value; }
        }

        /// <summary>当前步骤文案。</summary>
        public string StepText
        {
            get { return lblStep.Text; }
            set { lblStep.Text = value; }
        }

        /// <summary>日志文本。</summary>
        public string LogText
        {
            get { return txtLog.Text; }
            set { txtLog.Text = value; }
        }

        /// <summary>追加一行日志（自动滚动到底部）。</summary>
        public void AppendLog(string line)
        {
            if (string.IsNullOrEmpty(line))
            {
                return;
            }

            txtLog.Text = string.IsNullOrEmpty(txtLog.Text) ? line : txtLog.Text + Environment.NewLine + line;
        }

        /// <summary>设置进度（0–100）。AntdUI 的 Progress.Value 是 0–1 的比例。</summary>
        public void SetPercent(int percent)
        {
            if (percent < 0)
            {
                percent = 0;
            }

            if (percent > 100)
            {
                percent = 100;
            }

            progress.Value = percent / 100f;
            progress.Text = percent + "%";
        }

        /// <summary>把进度条切成不确定状态（安装中）。</summary>
        public void SetIndeterminate(bool on)
        {
            progress.Loading = on;
        }

        /// <summary>把进度条标成失败色。</summary>
        public void SetError(bool on)
        {
            progress.State = on ? AntdUI.TType.Error : AntdUI.TType.None;
        }
    }
}
