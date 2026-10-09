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
            btnChange.Click += (sender, e) => Pick();
            AllowDrop = true;
            DragEnter += OnDragEnter;
            DragDrop += OnDragDrop;
            DragLeave += OnDragLeave;
            cboEntry.SelectedIndexChanged += (sender, e) =>
            {
                if (!_populating)
                {
                    Raise();
                }
            };

            chkAutoName.CheckedChanged += (sender, e) => Raise();

            // 整个卡片都可以拖
            dropPanel.AllowDrop = true;
            dropPanel.DragEnter += OnDragEnter;
            dropPanel.DragDrop += OnDragDrop;
            dropPanel.DragLeave += OnDragLeave;
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

        /// <summary>空态拖拽区高度（竖排：图标 / 提示 / 副提示 / 按钮）。</summary>
        public const float DropZoneExpanded = 200F;

        /// <summary>已选态拖拽区高度（横排：路径 + 更换按钮）。</summary>
        public const float DropZoneCollapsed = 64F;

        /// <summary>设置拖拽区高度（由宿主在收缩动画里逐帧调用）。</summary>
        public void SetDropZoneHeight(float height)
        {
            if (rootLayout.RowStyles.Count > 1)
            {
                rootLayout.RowStyles[1].Height = height;
            }
        }

        /// <summary>产品名是否跟随文件夹名（默认开）。</summary>
        public bool AutoNameFromFolder
        {
            get { return chkAutoName.Checked; }
            set { chkAutoName.Checked = value; }
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
            chkAutoName.Text = text.Get("Field.AutoName");
            ApplyFolderState();
        }

        // ─────────────────────────────────────────────────────────────

        /// <summary>根据"有没有选文件夹"切换卡片外观。</summary>
        private void ApplyFolderState()
        {
            if (string.IsNullOrWhiteSpace(_folder))
            {
                // 空态：竖排提示 + 虚线框
                dropLayout.Visible = true;
                pickedLayout.Visible = false;
                dropPanel.BorderStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                dropPanel.BackColor = System.Drawing.Color.FromArgb(250, 251, 253);

                lblDropText.ForeColor = System.Drawing.Color.FromArgb(31, 41, 55);
                lblDropText.Text = T("Step.Folder.Hint", "把文件夹拖到这里");
                lblDropSub.Text = T("Step.Folder.Sub", "或者点下面的按钮");
                btnBrowse.Text = T("Step.Folder.Pick", "选择文件夹");
                lblPickedStats.Text = string.Empty;
                lblEntryHint.Text = string.Empty;
                cboEntry.Enabled = false;
                ResetEntrySelection();
                return;
            }

            // 已选态：横排「路径 + 更换文件夹」+ 实线框（比空态矮，把空间让给下面的字段）
            dropLayout.Visible = false;
            pickedLayout.Visible = true;
            dropPanel.BorderStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            dropPanel.BackColor = System.Drawing.Color.White;

            lblPickedPath.Text = _folder;
            btnChange.Text = T("Step.Folder.Change", "更换文件夹");

            RefreshCount();
            PopulateEntries(EntryPoint);

            if (!Directory.Exists(_folder))
            {
                lblPickedStats.Text = T("Files.Missing", "（文件夹不存在）");
            }
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
                lblPickedStats.Text = string.Empty;
                return;
            }

            int count;
            long bytes;
            BuilderPresenter.MeasureSource(_folder, out count, out bytes);

            if (count == 0)
            {
                lblPickedStats.Text = T("Files.Empty", "这个文件夹是空的");
                return;
            }

            var template = T("Files.Count", "共 {0} 个文件，{1}");
            lblPickedStats.Text = string.Format(CultureInfo.CurrentCulture, template, count, FormatSize(bytes));
        }

        /// <summary>把文件夹里的 exe 列出来给用户选。</summary>
        private void PopulateEntries(string keep)
        {
            var exes = BuilderPresenter.FindExecutables(_folder);

            _populating = true;
            try
            {
                // **必须显式重置**：AntdUI.Select 在 Items.Clear() 之后仍会显示上一次的文本，
                // 于是"换了文件夹但启动程序还显示旧值"。这是实际踩到的坑。
                ResetEntrySelection();

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

        /// <summary>把下拉框彻底清空（含 AntdUI 内部缓存的显示文本）。</summary>
        private void ResetEntrySelection()
        {
            cboEntry.Items.Clear();
            cboEntry.SelectedIndex = -1;
            cboEntry.SelectedValue = null;
            cboEntry.Text = string.Empty;
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
            var ok = FirstFolder(e) != null;
            e.Effect = ok ? DragDropEffects.Copy : DragDropEffects.None;

            if (ok)
            {
                SetDropHover(true);
            }
        }

        private void OnDragLeave(object sender, EventArgs e)
        {
            SetDropHover(false);
        }

        /// <summary>
        /// 拖拽悬停态：浅蓝底 #E6F4FF + 2px 蓝色实线边 + 蓝色加粗提示（设计稿取值）。
        /// 离开或已选文件夹时还原成空态。
        /// </summary>
        private void SetDropHover(bool hover)
        {
            if (!hover || !string.IsNullOrWhiteSpace(_folder))
            {
                ApplyFolderState();
                return;
            }

            dropLayout.Visible = true;
            pickedLayout.Visible = false;

            dropPanel.BackColor = System.Drawing.Color.FromArgb(230, 244, 255);
            dropPanel.BorderColor = System.Drawing.Color.FromArgb(22, 119, 255);
            dropPanel.BorderWidth = 2F;
            dropPanel.BorderStyle = System.Drawing.Drawing2D.DashStyle.Solid;

            lblDropText.ForeColor = System.Drawing.Color.FromArgb(22, 119, 255);
            lblDropText.Text = T("Step.Folder.Release", "松开即可选择这个文件夹");
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
