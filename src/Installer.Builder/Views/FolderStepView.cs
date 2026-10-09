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
    /// 第 1 步：选择要打包的文件夹 + 选启动程序。支持拖拽。
    ///
    /// **启动程序放在这里而不是"高级选项"里** —— 一个文件夹里有两个 exe 是常事，
    /// 为了改这一项专门进一次高级选项太折腾。
    /// </summary>
    public partial class FolderStepView : UserControl
    {
        private IStringTable _text;
        private bool _populating;

        /// <summary>构造。</summary>
        public FolderStepView()
        {
            InitializeComponent();
            btnBrowse.Click += (sender, e) => Pick();
            AllowDrop = true;
            DragEnter += OnDragEnter;
            DragDrop += OnDragDrop;
            txtSource.TextChanged += (sender, e) => OnFolderChanged();
            cboEntry.SelectedIndexChanged += (sender, e) =>
            {
                if (!_populating)
                {
                    Raise();
                }
            };
        }

        /// <summary>文件夹或启动程序变化。</summary>
        public event Action Changed;

        /// <summary>当前文件夹。</summary>
        public string Folder
        {
            get { return txtSource.Text.Trim(); }
            set
            {
                txtSource.Text = value ?? string.Empty;
                RefreshCount();
            }
        }

        /// <summary>当前启动程序（相对打包文件夹）。</summary>
        public string EntryPoint
        {
            get
            {
                // AntdUI.Select 没有 SelectedItem，用 SelectedValue（就是 SelectItem.Tag）
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

            btnBrowse.Text = string.IsNullOrWhiteSpace(Folder)
                ? text.Get("Step.Folder.Pick")
                : text.Get("Step.Folder.Change");
            txtSource.PlaceholderText = text.Get("Step.Folder.Hint");
            lblEntry.Text = text.Get("Field.EntryPoint");

            RefreshCount();
            PopulateEntries(EntryPoint);
        }

        /// <summary>重新统计文件数并刷新提示。</summary>
        public void RefreshCount()
        {
            var dir = Folder;

            if (string.IsNullOrWhiteSpace(dir))
            {
                lblFiles.Text = _text == null ? string.Empty : _text.Get("Files.None");
                return;
            }

            int count;
            long bytes;
            BuilderPresenter.MeasureSource(dir, out count, out bytes);

            if (count == 0)
            {
                lblFiles.Text = _text == null ? string.Empty : _text.Get("Files.Empty");
                return;
            }

            var template = _text == null ? null : _text.Get("Files.Count");
            lblFiles.Text = string.Format(CultureInfo.CurrentCulture,
                string.IsNullOrEmpty(template) ? "{0} / {1}" : template,
                count, FormatSize(bytes));
        }

        private void OnFolderChanged()
        {
            RefreshCount();
            PopulateEntries(null);
            Raise();
        }

        /// <summary>把文件夹里的 exe 列出来给用户选。</summary>
        private void PopulateEntries(string keep)
        {
            var exes = BuilderPresenter.FindExecutables(Folder);

            _populating = true;
            try
            {
                cboEntry.Items.Clear();

                if (exes.Count == 0)
                {
                    cboEntry.Enabled = false;
                    lblEntryHint.Text = _text == null ? string.Empty : _text.Get("Field.EntryPoint.None");
                    return;
                }

                cboEntry.Enabled = true;
                foreach (var e in exes)
                {
                    cboEntry.Items.Add(new AntdUI.SelectItem(e, e));
                }

                // 尽量保住用户原来的选择；没有就用猜的
                var target = keep;
                if (string.IsNullOrWhiteSpace(target) || !exes.Contains(target))
                {
                    target = BuilderPresenter.GuessEntryPoint(Folder);
                }

                var index = exes.IndexOf(target);
                cboEntry.SelectedIndex = index < 0 ? 0 : index;

                lblEntryHint.Text = exes.Count > 1
                    ? string.Format(CultureInfo.CurrentCulture,
                        _text == null ? "{0}" : _text.Get("Field.EntryPoint.Multiple"), exes.Count)
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
                dialog.Description = txtSource.PlaceholderText;
                dialog.SelectedPath = Directory.Exists(Folder) ? Folder : string.Empty;
                dialog.ShowNewFolderButton = true;

                if (dialog.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                Folder = dialog.SelectedPath;
                Raise();
            }
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

            Folder = dir;
            Raise();
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
            if (_text != null)
            {
                btnBrowse.Text = string.IsNullOrWhiteSpace(Folder)
                    ? _text.Get("Step.Folder.Pick")
                    : _text.Get("Step.Folder.Change");
            }

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
