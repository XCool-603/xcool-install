using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Installer.Abstractions.Platform;
using Installer.Builder.Presenters;

namespace Installer.Builder.Views
{
    /// <summary>
    /// 第 1 步：选择要打包的文件夹 + 选启动程序。
    ///
    /// 设计要点：
    /// ① **一个大拖拽卡片**是整屏的视觉焦点，而不是一行输入框；
    /// ② 启动程序直接在这里选（一个文件夹里有两个 exe 是常事）；
    /// ③ 选好文件夹后由宿主把文件夹名填进产品名（见 <see cref="FolderChosen"/>）。
    /// </summary>
    public partial class FolderStepView : UserControl
    {
        private IStringTable _text;
        private bool _populating;
        private string _folder = string.Empty;

        /// <summary>构造。</summary>
        public FolderStepView()
        {
            InitializeComponent();
            btnBrowse.Click += (sender, e) => Pick();
            AllowDrop = true;
            DragEnter += OnDragEnter;
            DragDrop += OnDragDrop;
            cboEntry.SelectedIndexChanged += (sender, e) =>
            {
                if (!_populating)
                {
                    Raise();
                }
            };

            // 整个卡片都可以拖
            dropPanel.AllowDrop = true;
            dropPanel.DragEnter += OnDragEnter;
            dropPanel.DragDrop += OnDragDrop;
        }

        /// <summary>文件夹或启动程序变化。</summary>
        public event Action Changed;

        /// <summary>用户**主动**选了一个新文件夹（拖拽或浏览），用于自动填产品名。</summary>
        public event Action<string> FolderChosen;

        /// <summary>当前文件夹。</summary>
        public string Folder
        {
            get { return _folder; }
            set
            {
                _folder = (value ?? string.Empty).Trim();
                ApplyFolderState();
            }
        }

        /// <summary>当前启动程序（相对打包文件夹）。</summary>
        public string EntryPoint
        {
            get
            {
                return cboEntry.SelectedValue == null
                    ? null
                    : Convert.ToString(cboEntry.SelectedValue, CultureInfo.InvariantCulture);
            }
            set
            {
                PopulateEntries(value);
            }
        }

        /// <summary>刷新文案。</summary>
        public void ApplyText(IStringTable text)
        {
            _text = text;
            lblStep.Text = text.Get("Step.Folder");
            lblEntry.Text = text.Get("Field.EntryPoint");
            ApplyFolderState();
        }

        // ─────────────────────────────────────────────────────────────

        /// <summary>根据"有没有选文件夹"切换卡片外观。</summary>
        private void ApplyFolderState()
        {
            if (string.IsNullOrWhiteSpace(_folder))
            {
                lblDropIcon.Text = "📁";
                lblDropText.Text = T("Step.Folder.Hint", "把文件夹拖到这里");
                lblDropSub.Text = T("Step.Folder.Sub", "或者点下面的按钮");
                btnBrowse.Text = T("Step.Folder.Pick", "选择文件夹");
                lblFiles.Text = string.Empty;
                lblEntryHint.Text = string.Empty;
                cboEntry.Enabled = false;
                cboEntry.Items.Clear();
                return;
            }

            lblDropIcon.Text = "📦";
            lblDropText.Text = _folder;
            lblDropSub.Text = Directory.Exists(_folder) ? string.Empty : T("Files.Missing", "（文件夹不存在）");
            btnBrowse.Text = T("Step.Folder.Change", "更换文件夹");

            RefreshCount();
            PopulateEntries(EntryPoint);
        }

        private string T(string key, string fallback)
        {
            if (_text == null)
            {
                return fallback;
            }

            var v = _text.Get(key);
            return string.IsNullOrEmpty(v) ? fallback : v;
        }

        /// <summary>重新统计文件数并刷新提示。</summary>
        public void RefreshCount()
        {
            if (string.IsNullOrWhiteSpace(_folder))
            {
                lblFiles.Text = string.Empty;
                return;
            }

            int count;
            long bytes;
            BuilderPresenter.MeasureSource(_folder, out count, out bytes);

            if (count == 0)
            {
                lblFiles.Text = T("Files.Empty", "这个文件夹是空的");
                return;
            }

            var template = T("Files.Count", "共 {0} 个文件，{1}");
            lblFiles.Text = string.Format(CultureInfo.CurrentCulture, template, count, FormatSize(bytes));
        }

        /// <summary>把文件夹里的 exe 列出来给用户选。</summary>
        private void PopulateEntries(string keep)
        {
            var exes = BuilderPresenter.FindExecutables(_folder);

            _populating = true;
            try
            {
                cboEntry.Items.Clear();

                if (exes.Count == 0)
                {
                    cboEntry.Enabled = false;
                    lblEntryHint.Text = T("Field.EntryPoint.None", "（这个文件夹里没有 .exe）");
                    return;
                }

                cboEntry.Enabled = true;
                foreach (var e in exes)
                {
                    cboEntry.Items.Add(new AntdUI.SelectItem(e, e));
                }

                var target = keep;
                if (string.IsNullOrWhiteSpace(target) || !exes.Contains(target))
                {
                    target = BuilderPresenter.GuessEntryPoint(_folder);
                }

                var index = exes.IndexOf(target);
                cboEntry.SelectedIndex = index < 0 ? 0 : index;

                lblEntryHint.Text = exes.Count > 1
                    ? string.Format(CultureInfo.CurrentCulture,
                        T("Field.EntryPoint.Multiple", "发现 {0} 个可执行文件，请选要启动的那个"), exes.Count)
                    : string.Empty;
            }
            finally
            {
                _populating = false;
            }
        }

        private void Pick()
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = T("Step.Folder.Pick", "选择要打包的文件夹");
                dialog.SelectedPath = Directory.Exists(_folder) ? _folder : string.Empty;
                dialog.ShowNewFolderButton = true;

                if (dialog.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                SetFolderFromUser(dialog.SelectedPath);
            }
        }

        private void SetFolderFromUser(string dir)
        {
            _folder = (dir ?? string.Empty).Trim();
            ApplyFolderState();

            var chosen = FolderChosen;
            if (chosen != null)
            {
                chosen(_folder);
            }

            Raise();
        }

        private void OnDragEnter(object sender, DragEventArgs e)
        {
            e.Effect = FirstFolder(e) != null ? DragDropEffects.Copy : DragDropEffects.None;
        }

        private void OnDragDrop(object sender, DragEventArgs e)
        {
            var dir = FirstFolder(e);
            if (dir == null)
            {
                return;
            }

            SetFolderFromUser(dir);
        }

        private static string FirstFolder(DragEventArgs e)
        {
            if (e.Data == null || !e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                return null;
            }

            var items = e.Data.GetData(DataFormats.FileDrop) as string[];
            if (items == null || items.Length == 0)
            {
                return null;
            }

            return Directory.Exists(items[0]) ? items[0] : Path.GetDirectoryName(items[0]);
        }

        private void Raise()
        {
            var handler = Changed;
            if (handler != null)
            {
                handler();
            }
        }

        private static string FormatSize(long bytes)
        {
            if (bytes >= 1024L * 1024 * 1024)
            {
                return (bytes / 1024.0 / 1024 / 1024).ToString("0.0 GB", CultureInfo.CurrentCulture);
            }

            if (bytes >= 1024 * 1024)
            {
                return (bytes / 1024.0 / 1024).ToString("0.0 MB", CultureInfo.CurrentCulture);
            }

            if (bytes >= 1024)
            {
                return (bytes / 1024.0).ToString("0.0 KB", CultureInfo.CurrentCulture);
            }

            return bytes + " B";
        }
    }
}
