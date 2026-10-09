using System;
using System.IO;
using System.Windows.Forms;
using Installer.Abstractions.Platform;

namespace Installer.Builder.Views
{
    /// <summary>底部：输出路径（自动推导，可换）+ 预览 / 生成按钮。</summary>
    public partial class OutputBar : UserControl
    {
        /// <summary>构造。</summary>
        public OutputBar()
        {
            InitializeComponent();
            btnChange.Click += (sender, e) => ChangeOutput();
        }

        /// <summary>点了"预览"。</summary>
        public event Action PreviewClicked;

        /// <summary>点了"生成安装包"。</summary>
        public event Action BuildClicked;

        /// <summary>输出路径被改。</summary>
        public event Action OutputChanged;

        /// <summary>当前输出路径。</summary>
        public string OutputPath
        {
            get { return _outputPath; }
            set
            {
                _outputPath = value;
                ShowOutput();
            }
        }

        private string _outputPath;

        /// <summary>是否正在生成。</summary>
        public bool Building
        {
            get { return btnBuild.Loading; }
            set
            {
                btnBuild.Loading = value;
                btnBuild.Enabled = !value;
                btnPreview.Enabled = !value;
                btnChange.Enabled = !value;
            }
        }

        /// <summary>刷新文案。</summary>
        public void ApplyText(IStringTable text)
        {
            lblPrefix.Text = text.Get("Output.Label");
            btnChange.Text = text.Get("Output.Change");
            btnPreview.Text = text.Get("Btn.Preview");
            btnBuild.Text = text.Get("Btn.Build");
            _emptyText = text.Get("Output.Empty");
            ShowOutput();
        }

        private string _emptyText = "（选好文件夹后自动确定）";

        private void ShowOutput()
        {
            lblPath.Text = string.IsNullOrWhiteSpace(_outputPath) ? _emptyText : _outputPath;
        }

        private void ChangeOutput()
        {
            using (var dialog = new SaveFileDialog())
            {
                dialog.Filter = "安装包 (*.exe)|*.exe|所有文件 (*.*)|*.*";
                dialog.DefaultExt = "exe";
                dialog.OverwritePrompt = false;

                if (!string.IsNullOrWhiteSpace(_outputPath))
                {
                    dialog.FileName = Path.GetFileName(_outputPath);
                    var dir = Path.GetDirectoryName(_outputPath);
                    if (!string.IsNullOrEmpty(dir) && Directory.Exists(dir))
                    {
                        dialog.InitialDirectory = dir;
                    }
                }

                if (dialog.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                OutputPath = dialog.FileName;

                var handler = OutputChanged;
                if (handler != null)
                {
                    handler();
                }
            }
        }

        private void OnPreview(object sender, EventArgs e)
        {
            var handler = PreviewClicked;
            if (handler != null)
            {
                handler();
            }
        }

        private void OnBuild(object sender, EventArgs e)
        {
            var handler = BuildClicked;
            if (handler != null)
            {
                handler();
            }
        }
    }
}
