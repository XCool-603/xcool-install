using System;
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

        /// <summary>构造。</summary>
        public BuilderForm()
        {
            InitializeComponent();
            WireEvents();

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
                MarkDirty();
                UpdateOutputPath();
                UpdateStatus(_presenter.S("Status.Ready"));
            };

            infoStep.Changed += () =>
            {
                MarkDirty();
                UpdateOutputPath();
            };

            shortcutStep.Changed += MarkDirty;

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
                try
                {
                    ReadFromViews();
                }
                catch (Exception)
                {
                }

                _presenter.AutoSave();
            };
        }

        private void ApplyText()
        {
            var t = _presenter.Text;

            Text = _presenter.S("App.Title");
            lblTitle.Text = _presenter.S("App.Title");
            lblStep1.Text = _presenter.S("Step.Folder");
            lblStep2.Text = _presenter.S("Step.Info");
            lblStep3.Text = _presenter.S("Step.Shortcuts");
            btnAdvanced.Text = _presenter.S("Advanced.Open");

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
        }

        private void UpdateStatus(string message)
        {
            lblStatus.Text = message;
        }

        // ─────────────────────────────────────────────────────────────
        // 动作
        // ─────────────────────────────────────────────────────────────

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
            UpdateStatus(_presenter.S("Status.Building"));

            var reporter = new Progress<BuildProgress>(p =>
            {
                var percent = Math.Max(0, Math.Min(100, p.Percent));
                UpdateStatus(p.Message + "  (" + percent + "%)");
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
        public void PrepareForRendering()
        {
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.Manual;
            Location = new Point(-4000, -4000);
        }

        /// <summary>截图模式：预置一个文件夹（用来验证依赖文件夹的界面，比如启动程序下拉框）。</summary>
        public void PresetSourceFolder(string folder)
        {
            _initializing = true;
            try
            {
                folderStep.Folder = folder;
                _presenter.Document.Project.Build.SourceDir = folder;
                UpdateOutputPath();
            }
            finally
            {
                _initializing = false;
            }

            _presenter.Document.Dirty = false;
            UpdateTitle();
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
