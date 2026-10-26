using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Installer.Abstractions.Install;
using Installer.Abstractions.Model;
using Installer.Abstractions.Packaging;
using Installer.Builder.Common;
using Installer.Builder.Presenters;
using Installer.UI.Forms;
using Installer.UI.Models;

namespace Installer.Builder.Forms
{
    /// <summary>
    /// 安装包制作助手主窗体 —— **一屏三步**。
    ///
    /// 设计原则：
    /// ① 界面上只有 3 项必填：文件夹、产品名称、版本号；
    /// ② 模板 stub 自动找（<see cref="StubLocator"/>），用户看不到也不需要知道；
    /// ③ 输出路径自动推导，想换才点"更换位置"；
    /// ④ 许可协议 / 启动程序 / COM 注册这些收进"高级选项"。
    /// </summary>
    public partial class BuilderForm : AntdUI.BaseForm
    {
        private readonly BuilderPresenter _presenter = new BuilderPresenter();

        /// <summary>初始化期间为 true：避免控件事件把默认值抹掉。</summary>
        private bool _initializing = true;

        private bool _building;

        /// <summary>
        /// 渲染/截图模式。这个模式下**绝不能写自动记忆** ——
        /// 否则开发时跑一次 --render-ui 就会把用户记住的工程覆盖掉。
        /// </summary>
        private bool _rendering;

        /// <summary>构造。</summary>
        public BuilderForm()
        {
            InitializeComponent();
            WireEvents();

            // 显式设置窗口图标 —— 标题栏 / 任务栏 / Alt+Tab 都跟着它。
            this.Icon = Installer.UI.Common.AppIcon.Load();

            // 恢复上次的工程：**productCode 必须跨会话保持**，否则同一个产品装几次
            // 就会在「程序和功能」里留下几条记录（见 BuilderPresenter.AutoSavePath 的说明）
            _presenter.TryLoadAutoSaved();

            ApplyText();
            WriteToViews();
            UpdateOutputPath();
            UpdateStatus(_presenter.S("Status.Ready"));

            _initializing = false;
            _presenter.Document.Dirty = false;
            UpdateTitle();
        }

        // ─────────────────────────────────────────────────────────────
        // 装配
        // ─────────────────────────────────────────────────────────────

        private void WireEvents()
        {
            folderStep.Changed += () =>
            {
                if (_initializing) return;
                MarkDirty();
                UpdateOutputPath();
                UpdateStatus(string.IsNullOrWhiteSpace(folderStep.Folder)
                    ? _presenter.S("Status.Ready")
                    : _presenter.S("Status.ReadyToBuild"));
            };

            // 选了文件夹就把文件夹名填进产品名；换了启动程序就跟着 exe 名走。
            // 是否覆盖由"产品名跟随文件夹名"复选框决定（见 ApplyAutoName）。
            folderStep.FolderChosen += AutoFillProductName;
            folderStep.EntryChosen += AutoFillProductNameFromEntry;
            folderStep.AutoNameToggled += OnAutoNameToggled;

            // 用户自己敲了产品名 → 自动关掉"跟随"，让他的输入留得住
            infoStep.ProductNameEdited += OnProductNameEdited;

            btnOpen.Click += (sender, e) => OpenProject();
            btnSave.Click += (sender, e) => SaveProject();

            infoStep.Changed += () =>
            {
                if (_initializing) return;
                MarkDirty();
                UpdateOutputPath();
            };

            shortcutStep.Changed += () => { if (!_initializing) MarkDirty(); };

            outputBar.OutputChanged += () =>
            {
                _presenter.Document.Project.Build.OutputPath = outputBar.OutputPath;
                MarkDirty();
            };

            outputBar.PreviewClicked += PreviewWizard;
            outputBar.BuildClicked += StartBuild;

            btnAdvanced.Click += (sender, e) => OpenAdvanced();

            cboLang.SelectedIndexChanged += (sender, e) => SwitchLanguage();

            // 关窗口时把工程记下来，下次打开还是同一个 productCode
            FormClosing += (sender, e) =>
            {
                if (_rendering)
                {
                    return;   // 截图模式不碰用户的工程
                }

                try
                {
                    ReadFromViews();
                    _presenter.AutoSave();
                }
                catch (Exception ex)
                {
                    // 读不到界面当前状态，就**不能**落盘 —— 否则会把上一次的
                    // 快照当成最新状态存下来，用户下次打开看到的是过期数据。
                    // 而且这里必须让用户看见，不能静默吞掉。
                    UpdateStatus("自动保存失败（工程未更新）：" + ex.Message);
                }
            };
        }

        private void ApplyText()
        {
            var t = _presenter.Text;

            Text = _presenter.S("App.Title");
            btnAdvanced.Text = _presenter.S("Advanced.Open");
            btnOpen.Text = _presenter.S("Menu.Open");
            btnSave.Text = _presenter.S("Menu.Save");

            folderStep.ApplyText(t);
            infoStep.ApplyText(t);
            shortcutStep.ApplyText(t);
            outputBar.ApplyText(t);

            var index = cboLang.SelectedIndex;
            cboLang.Items.Clear();
            cboLang.Items.Add(new AntdUI.SelectItem("中文", "zh-Hans"));
            cboLang.Items.Add(new AntdUI.SelectItem("English", "en"));
            cboLang.SelectedIndex = index < 0 ? (_presenter.Culture == "en" ? 1 : 0) : index;

            UpdateTitle();
        }

        private void UpdateTitle()
        {
            var name = _presenter.Document.Path == null
                ? null
                : Path.GetFileName(_presenter.Document.Path);

            Text = _presenter.S("App.Title")
                   + (name == null ? string.Empty : " — " + name)
                   + (_presenter.Document.Dirty ? " *" : string.Empty);
        }

        /// <summary>把工程里的值显示到界面。</summary>
        private void WriteToViews()
        {
            // **模型 → 视图是单向的**：这期间任何控件事件都不能反过来写模型。
            // 不加这个保护的后果：设置 infoStep 的文本框会触发 TextChanged →
            // UpdateOutputPath → ReadFromViews → 用"还没同步的"快捷方式复选框
            // 覆盖清单，把快捷方式悄悄清空。
            var wasInitializing = _initializing;
            _initializing = true;

            try
            {
                WriteToViewsCore();
            }
            finally
            {
                _initializing = wasInitializing;
            }
        }

        private void WriteToViewsCore()
        {
            var m = _presenter.Document.Project.Manifest;
            var b = _presenter.Document.Project.Build ?? new ProjectBuildSettings();

            folderStep.Folder = b.SourceDir;
            folderStep.EntryPoint = m.EntryPoint;
            infoStep.Write(m.Product.Name, m.Product.Version, m.Product.Publisher);
            shortcutStep.Write(m);
            outputBar.OutputPath = _presenter.ResolveOutputPath();
        }

        /// <summary>把界面上的值写回工程。</summary>
        private void ReadFromViews()
        {
            var m = _presenter.Document.Project.Manifest;
            if (_presenter.Document.Project.Build == null)
            {
                _presenter.Document.Project.Build = new ProjectBuildSettings();
            }

            var b = _presenter.Document.Project.Build;

            b.SourceDir = folderStep.Folder;
            m.EntryPoint = folderStep.EntryPoint;

            Installer.Abstractions.Model.LocalizedText name;
            string version;
            Installer.Abstractions.Model.LocalizedText publisher;
            infoStep.Read(out name, out version, out publisher);

            m.Product.Name = name;
            m.Product.Version = version;
            m.Product.Publisher = publisher;

            shortcutStep.Read(m);
        }

        private void MarkDirty()
        {
            if (_initializing || _building)
            {
                return;
            }

            _presenter.Document.Dirty = true;
            UpdateTitle();
        }

        private void UpdateOutputPath()
        {
            ReadFromViews();

            // 用户没有固定过输出路径 → 自动推导
            _presenter.Document.Project.Build.OutputPath = null;
            outputBar.OutputPath = _presenter.ResolveOutputPath();

            UpdateFolderDependentState();
        }

        /// <summary>
        /// 按"有没有选文件夹"刷新依赖它的界面：
        /// ②③ 卡片淡化、主按钮可用性、预计安装后大小。
        /// </summary>
        private void UpdateFolderDependentState()
        {
            var hasFolder = !string.IsNullOrWhiteSpace(folderStep.Folder);

            // 未选文件夹时第 2、3 步淡化。
            // 设计稿的做法是卡片背景 白 → #F7F8FA、徽章 #1677FF → #9DC5FC，
            // 而不是只把 Enabled 置 false（AntdUI 的禁用态对比度只有 55，几乎读不出）。
            card2.Enabled = hasFolder;
            card3.Enabled = hasFolder;

            var cardBack = hasFolder
                ? System.Drawing.Color.White
                : System.Drawing.Color.FromArgb(247, 248, 250);

            // 注意：AntdUI.Panel 画背景用的是 Back，不是继承来的 BackColor！
            card2.Back = cardBack;
            card3.Back = cardBack;

            infoStep.SetDimmed(!hasFolder);
            shortcutStep.SetDimmed(!hasFolder);

            outputBar.CanBuild = hasFolder;

            // 拖拽区收缩：空态 200 / 已选 64；卡片行高跟着走
            var dropHeight = hasFolder
                ? Views.FolderStepView.DropZoneCollapsed
                : Views.FolderStepView.DropZoneExpanded;

            folderStep.SetDropZoneHeight(dropHeight);
            SetCard1RowHeight(dropHeight + 158F);   // 30 标题 + 56 启动程序行 + 16 间距 + 40 内边距 + 余量

            int count;
            long bytes;
            BuilderPresenter.MeasureSource(folderStep.Folder, out count, out bytes);

            outputBar.SetEstimatedSize(count == 0
                ? string.Empty
                : string.Format(CultureInfo.CurrentCulture,
                    _presenter.S("Output.Estimate"), FormatSize(bytes)));
        }

        /// <summary>
        /// 设置第一张卡片（第 1 步）的行高。
        /// 拖拽区在空态/已选态之间收缩，卡片高度必须跟着走 ——
        /// TableLayoutPanel 的 Absolute 行高不会自动适配内容。
        /// </summary>
        private void SetCard1RowHeight(float height)
        {
            if (contentLayout.RowStyles.Count > 0)
            {
                contentLayout.RowStyles[0].Height = height;
            }
        }

        private void UpdateStatus(string message)
        {
            lblStatus.Text = message;
        }

        // ─────────────────────────────────────────────────────────────
        // 动作
        // ─────────────────────────────────────────────────────────────

        /// <summary>上一次自动填入的产品名，用于判断"这个名字是不是我填的"。</summary>
        /// <summary>用户选了文件夹 → 产品名跟随**文件夹名**（与复选框文案一致）。</summary>
        private void AutoFillProductName(string folder)
        {
            ApplyAutoName(BuilderPresenter.SuggestProductName(folder));
        }

        /// <summary>用户换了启动程序 → 产品名跟随 **exe 名**（去扩展名）。</summary>
        private void AutoFillProductNameFromEntry(string entry)
        {
            ApplyAutoName(string.IsNullOrWhiteSpace(entry)
                ? null
                : System.IO.Path.GetFileNameWithoutExtension(entry));
        }

        /// <summary>用户勾上"产品名跟随文件夹名" → 立刻重算一次，让勾选动作当场可见。</summary>
        private void OnAutoNameToggled(bool on)
        {
            if (_initializing || !on)
            {
                return;
            }

            ApplyAutoName(BestAutoName());
        }

        /// <summary>用户自己敲了产品名 → 关掉"跟随"，让他的输入留得住。</summary>
        private void OnProductNameEdited()
        {
            if (_initializing)
            {
                return;
            }

            folderStep.AutoNameFromFolder = false;
        }

        /// <summary>当前最合适的产品名：启动程序更具体，优先用它；没有就退回文件夹名。</summary>
        private string BestAutoName()
        {
            var entry = folderStep.EntryPoint;
            if (!string.IsNullOrWhiteSpace(entry))
            {
                var fromExe = System.IO.Path.GetFileNameWithoutExtension(entry);
                if (!string.IsNullOrWhiteSpace(fromExe))
                {
                    return fromExe;
                }
            }

            return BuilderPresenter.SuggestProductName(folderStep.Folder);
        }

        /// <summary>
        /// 套用自动推导出的产品名。
        ///
        /// 唯一的开关就是"产品名跟随文件夹名"那个复选框：勾着就跟随，取消就一个字符都不碰。
        ///
        /// 以前这里用的是一段隐藏的"当前名字 != 上次自动填的名字就不覆盖"启发式：
        /// 用户一旦手动填过产品名（或从工程里载入了名字），自动命名就**永久失效**；
        /// 而界面上那个复选框宿主从来没读过 —— 于是"选启动程序 → 产品名跟着变"
        /// 看起来完全没生效。现在开关可见、行为可预期。
        /// </summary>
        private void ApplyAutoName(string suggested)
        {
            if (!folderStep.AutoNameFromFolder)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(suggested))
            {
                return;
            }

            infoStep.SetProductName(suggested);
        }

        /// <summary>
        /// 打开 / 新建工程之后决定"产品名跟随"的初始状态：
        /// 工程里已经有产品名 → 关掉跟随（工程里的名字是权威，不该被启动程序的选择冲掉）；
        /// 名字还是空的（新建工程）→ 打开跟随，选文件夹即自动命名。
        /// </summary>
        private void SyncAutoNameToggle()
        {
            folderStep.AutoNameFromFolder = string.IsNullOrWhiteSpace(infoStep.CurrentProductName);
        }

        private void OpenProject()
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.Title = _presenter.S("Menu.OpenTitle");
                dialog.Filter = _presenter.S("Filter.Project");

                if (dialog.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                try
                {
                    _presenter.Load(dialog.FileName);
                    WriteToViews();
                    SyncAutoNameToggle();
                    UpdateOutputPath();
                    _presenter.Document.Dirty = false;
                    UpdateTitle();
                    UpdateStatus(_presenter.S("Status.Ready"));
                }
                catch (Exception ex)
                {
                    AntdUI.Message.error(this, _presenter.S("Msg.Error.Title") + "：" + ex.Message);
                }
            }
        }

        private void SaveProject()
        {
            ReadFromViews();

            var path = _presenter.Document.Path;

            if (string.IsNullOrEmpty(path))
            {
                using (var dialog = new SaveFileDialog())
                {
                    dialog.Title = _presenter.S("Menu.SaveTitle");
                    dialog.Filter = _presenter.S("Filter.Project");
                    dialog.FileName = "app.wmpkg.json";

                    if (dialog.ShowDialog(this) != DialogResult.OK)
                    {
                        return;
                    }

                    path = dialog.FileName;
                }
            }

            try
            {
                _presenter.Save(path);
                UpdateTitle();
                UpdateStatus(_presenter.S("Menu.Saved"));
                AntdUI.Message.success(this, _presenter.S("Menu.Saved"));
            }
            catch (Exception ex)
            {
                AntdUI.Message.error(this, _presenter.S("Msg.Error.Title") + "：" + ex.Message);
            }
        }

        private void OpenAdvanced()
        {
            ReadFromViews();

            using (var dialog = new AdvancedForm(_presenter))
            {
                if (dialog.ShowDialog(this) == DialogResult.OK && dialog.Changed)
                {
                    MarkDirty();
                    outputBar.OutputPath = _presenter.ResolveOutputPath();
                }
            }
        }

        private void SwitchLanguage()
        {
            if (_initializing || cboLang.SelectedIndex < 0)
            {
                return;
            }

            ReadFromViews();
            _presenter.SetCulture(cboLang.SelectedIndex == 1 ? "en" : "zh-Hans");
            ApplyText();
            WriteToViews();
            UpdateOutputPath();
        }

        private void PreviewWizard()
        {
            ReadFromViews();

            var problems = _presenter.Validate();
            if (problems.Count > 0)
            {
                AntdUI.Message.warn(this, string.Join(Environment.NewLine, problems.ToArray()));
                return;
            }

            var manifest = _presenter.Document.Project.Manifest;
            var installDir = Environment.ExpandEnvironmentVariables(
                string.IsNullOrWhiteSpace(manifest.DefaultInstallDir)
                    ? @"C:\Temp\Preview"
                    : manifest.DefaultInstallDir);

            try
            {
                using (var form = new WizardForm(manifest, new PreviewHost(), installDir, null))
                {
                    form.ShowDialog(this);
                }
            }
            catch (Exception ex)
            {
                AntdUI.Message.error(this, _presenter.S("Msg.Error.Title") + "：" + ex.Message);
            }
        }

        private void StartBuild()
        {
            if (_building)
            {
                return;
            }

            ReadFromViews();

            var problems = _presenter.Validate();
            if (problems.Count > 0)
            {
                AntdUI.Modal.open(new AntdUI.Modal.Config(this, _presenter.S("Msg.Fix.Title"),
                    string.Join(Environment.NewLine + Environment.NewLine, problems.ToArray())));
                return;
            }

            _building = true;
            outputBar.Building = true;

            // 进度条：打包期间显示在状态栏右侧
            progress.Value = 0F;
            progress.Text = "0%";
            progress.Visible = true;

            UpdateStatus(_presenter.S("Status.Building"));

            var reporter = new Progress<BuildProgress>(p =>
            {
                var percent = Math.Max(0, Math.Min(100, p.Percent));
                progress.Value = percent / 100F;   // AntdUI.Progress.Value 是 0~1
                progress.Text = percent + "%";
                UpdateStatus(p.Message);
            });

            Task.Run(() =>
            {
                try
                {
                    var result = _presenter.Build(reporter);
                    BeginInvoke(new Action(() => OnBuildDone(result, null)));
                }
                catch (Exception ex)
                {
                    BeginInvoke(new Action(() => OnBuildDone(null, ex)));
                }
            });
        }

        private void OnBuildDone(PackageBuildResult result, Exception error)
        {
            _building = false;
            outputBar.Building = false;
            progress.Visible = false;

            if (error != null)
            {
                UpdateStatus(string.Format(CultureInfo.CurrentCulture, _presenter.S("Status.Failed"), error.Message));
                AntdUI.Modal.open(new AntdUI.Modal.Config(this, _presenter.S("Msg.Error.Title"), error.Message));
                return;
            }

            var fileName = Path.GetFileName(result.OutputPath);
            var size = FormatSize(result.OutputBytes);

            // 打包成功后立刻记下来，避免"下次打开又换了一个 productCode"
            _presenter.AutoSave();

            UpdateStatus(string.Format(CultureInfo.CurrentCulture, _presenter.S("Status.Done"), fileName, size));

            // 打包时的"体检"结果先弹出来 —— 最典型的是"程序会往安装目录里写数据"，
            // 这种包装到 Program Files 之后普通用户写不进去，属于装完才会炸的坑，
            // 必须在制作阶段就提醒。
            if (result.Warnings != null && result.Warnings.Count > 0)
            {
                AntdUI.Modal.open(new AntdUI.Modal.Config(this,
                    _presenter.S("Msg.Warn.Title"),
                    string.Join("\r\n\r\n", result.Warnings.ToArray())));
            }

            var body = string.Format(CultureInfo.CurrentCulture, _presenter.S("Msg.Done.Body"),
                result.OutputPath, size);

            if (AntdUI.Modal.open(new AntdUI.Modal.Config(this, _presenter.S("Msg.Done.Title"), body))
                == DialogResult.OK)
            {
                try
                {
                    Process.Start("explorer.exe", "/select,\"" + result.OutputPath + "\"");
                }
                catch (Exception ex)
                {
                    AntdUI.Message.warn(this, ex.Message);
                }
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

            return (bytes / 1024.0).ToString("0.0 KB", CultureInfo.CurrentCulture);
        }

        /// <summary>预览用的空宿主：只走界面，不真的安装。</summary>
        private sealed class PreviewHost : IWizardHost
        {
            public void StartInstall(WizardOptions options, Action<InstallProgress> onProgress, Action<bool, string> onDone)
            {
                onDone(false, "这是预览，不会真的安装。");
            }

            public void RequestCancel()
            {
            }

            public void StartUninstall(Action<InstallProgress> onProgress, Action<bool, string> onDone)
            {
                onDone(false, "这是预览，不会真的卸载。");
            }

            /// <summary>
            /// 预览模式永远不提权。
            /// 制作人员点「预览向导」只是想看界面长什么样，不该被弹一个 UAC 框。
            /// </summary>
            public bool NeedsElevationRestart(WizardOptions options)
            {
                return false;
            }

            /// <summary>预览模式不可能走到这里（上面恒返回 false），兜底返回 false。</summary>
            public bool RestartElevated(WizardOptions options)
            {
                return false;
            }

            public void LaunchEntryPoint()
            {
            }
        }

        // ─────────────────────────────────────────────────────────────
        // 视觉验收
        // ─────────────────────────────────────────────────────────────

        /// <summary>把主界面和高级选项各渲染成 PNG（开发/验收用）。</summary>
        public void RenderToPng(string directory)
        {
            Directory.CreateDirectory(directory);

            var animation = AntdUI.Config.Animation;
            AntdUI.Config.Animation = false;

            try
            {
                CaptureMain(Path.Combine(directory, "主界面.png"));

                using (var advanced = new AdvancedForm(_presenter))
                {
                    advanced.ShowInTaskbar = false;
                    advanced.StartPosition = FormStartPosition.Manual;
                    advanced.Location = new Point(-4000, -4000);
                    advanced.Show();
                    Application.DoEvents();

                    CaptureForm(advanced, Path.Combine(directory, "高级选项-许可协议.png"));

                    if (advanced.Controls.Count > 0)
                    {
                        var tabs = FindTabs(advanced);
                        if (tabs != null && tabs.Pages.Count > 1)
                        {
                            tabs.SelectedIndex = 1;
                            Application.DoEvents();
                            CaptureForm(advanced, Path.Combine(directory, "高级选项-安装细节.png"));
                        }
                    }

                    advanced.Close();
                }
            }
            finally
            {
                AntdUI.Config.Animation = animation;
            }
        }

        /// <summary>截图模式：不显示窗口，直接渲染。</summary>
        /// <summary>把关键控件的运行时边界写出来（排查布局问题时用）。</summary>
        public void DumpBounds(string dir)
        {
            var sb = new System.Text.StringBuilder();
            Dump(this, sb, 0);

            System.IO.Directory.CreateDirectory(dir);
            System.IO.File.WriteAllText(System.IO.Path.Combine(dir, "bounds.txt"), sb.ToString());
        }

        private static void Dump(System.Windows.Forms.Control c, System.Text.StringBuilder sb, int depth)
        {
            sb.Append(new string(' ', depth * 2))
              .Append(c.GetType().Name).Append(" [").Append(c.Name).Append("] ")
              .Append("Bounds=").Append(c.Bounds)
              .Append(" Margin=").Append(c.Margin)
              .Append(" Dock=").Append(c.Dock)
              .AppendLine();

            foreach (System.Windows.Forms.Control child in c.Controls)
            {
                Dump(child, sb, depth + 1);
            }
        }

        public void PrepareForRendering()
        {
            _rendering = true;

            // 截图必须可复现，也不能受"用户上次的工程"影响 → 从干净的默认工程开始
            _presenter.NewProject();
            WriteToViews();
            SyncAutoNameToggle();
            UpdateOutputPath();

            ShowInTaskbar = false;
            StartPosition = FormStartPosition.Manual;
            Location = new Point(-4000, -4000);
        }

        /// <summary>诊断用。</summary>
        public string DebugState()
        {
            var m = _presenter.Document.Project.Manifest;
            return "manifest.shortcuts=[" +
                   string.Join(",", m.Shortcuts.Select(s => s.Location).ToArray()) + "]  " +
                   shortcutStep.DebugChecked();
        }

        /// <summary>截图模式：预置一个文件夹（用来验证依赖文件夹的界面，比如启动程序下拉框）。</summary>
        public void PresetSourceFolder(string folder)
        {
            _initializing = true;
            try
            {
                folderStep.Folder = folder;
                _presenter.Document.Project.Build.SourceDir = folder;
                AutoFillProductName(folder);   // 和用户真选文件夹时一样
                UpdateOutputPath();
                UpdateStatus(_presenter.S("Status.ReadyToBuild"));
            }
            finally
            {
                _initializing = false;
            }

            _presenter.Document.Dirty = false;
            UpdateTitle();
        }

        /// <summary>
        /// 验收 / CI 用：跑一遍"产品名跟随"的完整链路，返回可断言的 key=value 行。
        ///
        /// 对应这条用户反馈：**选了别的启动程序，产品名根本不变**。
        /// 根因是自动命名被一段隐藏的"用户改过就不覆盖"启发式永久关掉了，
        /// 而界面上那个唯一的开关（"产品名跟随文件夹名"复选框）宿主从来没读过。
        ///
        /// 这里把三条保证固化成可断言输出：
        ///   ① 换启动程序 → 产品名跟着 exe 名变（pickChanged=True）
        ///   ② 用户自己敲名字 → 跟随自动关掉，且名字留住（autoNameAfterTyping=False）
        ///   ③ 重新勾上跟随 → 立刻按当前启动程序重算（nameAfterRecheck == pick1）
        /// </summary>
        public string SelfTestAutoName(string folder)
        {
            var lines = new List<string>();

            _initializing = true;
            try
            {
                folderStep.Folder = folder;
                SyncAutoNameToggle();
            }
            finally
            {
                _initializing = false;
            }

            var exes = BuilderPresenter.FindExecutables(folder);
            lines.Add("exes=" + exes.Count.ToString(CultureInfo.InvariantCulture));
            lines.Add("autoNameOnLoad=" + folderStep.AutoNameFromFolder);

            if (exes.Count < 2)
            {
                lines.Add("AUTONAME-NEED-2-EXES");
                return string.Join("\n", lines.ToArray());
            }

            // ① 换启动程序 → 产品名跟着变。
            //    先点最后一个、再点第一个：两次都保证是"真的换了选项"。
            //    （目标索引恰好等于当前选中项时，WinForms 不触发 SelectedIndexChanged，
            //      断言就会变成空跑 —— 第一版自检就踩了这个坑。）
            folderStep.SimulateEntryPick(exes[exes.Count - 1]);
            var pickedLast = infoStep.CurrentProductName;
            lines.Add("pickLast=" + pickedLast);

            folderStep.SimulateEntryPick(exes[0]);
            var pickedFirst = infoStep.CurrentProductName;
            lines.Add("pickFirst=" + pickedFirst);
            lines.Add("pickChanged=" + (!string.Equals(pickedLast, pickedFirst, StringComparison.Ordinal)));

            // ② 用户自己敲名字 → 跟随自动关掉、名字留住。
            //    Write() 直接写文本框，等价于"用户敲进去"，会触发 ProductNameEdited。
            //    名字故意用纯 ASCII：这条输出要被 CI 解析，中文在重定向管道里会因
            //    控制台编码不同而乱码（实测踩过）。
            infoStep.Write(
                new LocalizedText { { "zh-Hans", "MyOwnName" }, { "en", "MyOwnName" } },
                infoStep.Version,
                new LocalizedText { { "zh-Hans", "TestPublisher" }, { "en", "TestPublisher" } });
            lines.Add("autoNameAfterTyping=" + folderStep.AutoNameFromFolder);
            lines.Add("nameAfterTyping=" + infoStep.CurrentProductName);

            // ③ 重新勾上"跟随" → 立刻按当前启动程序重算
            folderStep.AutoNameFromFolder = true;
            lines.Add("nameAfterRecheck=" + infoStep.CurrentProductName);
            lines.Add("AUTONAME-DONE");

            return string.Join("\n", lines.ToArray());
        }

        private void CaptureMain(string path)
        {
            CaptureForm(this, path);
        }

        private static void CaptureForm(Form form, string path)
        {
            using (var bmp = new Bitmap(form.Width, form.Height))
            {
                form.DrawToBitmap(bmp, new Rectangle(0, 0, form.Width, form.Height));
                bmp.Save(path, System.Drawing.Imaging.ImageFormat.Png);
            }
        }

        private static AntdUI.Tabs FindTabs(Control root)
        {
            foreach (Control c in root.Controls)
            {
                var tabs = c as AntdUI.Tabs;
                if (tabs != null)
                {
                    return tabs;
                }

                if (c.HasChildren)
                {
                    var nested = FindTabs(c);
                    if (nested != null)
                    {
                        return nested;
                    }
                }
            }

            return null;
        }
    }
}
