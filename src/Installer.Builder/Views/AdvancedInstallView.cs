using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Installer.Abstractions.Model;
using Installer.Abstractions.Platform;

namespace Installer.Builder.Views
{
    /// <summary>
    /// 高级选项 · 安装细节（安装范围 / 安装目录 / 图标 / COM）。
    ///
    /// 注意：**"启动程序"不在这里** —— 它跟着文件夹走，放在主界面第 1 步直接选，
    /// 不然一个文件夹里有两个 exe 时用户得专门进一次高级选项。
    /// </summary>
    public partial class AdvancedInstallView : UserControl
    {
        /// <summary>正在往界面里灌值 —— 期间不响应"切换范围时同步目录"的逻辑。</summary>
        private bool _loading;

        /// <summary>当前清单（切范围时要用产品名算默认目录）。</summary>
        private InstallerManifest _manifest;

        /// <summary>构造。</summary>
        public AdvancedInstallView()
        {
            InitializeComponent();
            btnIcon.Click += (sender, e) => PickIcon();
            cboScope.SelectedIndexChanged += (sender, e) => SyncDirToScope();
        }

        /// <summary>写回界面。</summary>
        public void Write(InstallerManifest m, ProjectBuildSettings build)
        {
            _manifest = m;

            _loading = true;
            try
            {
                cboScope.SelectedIndex = string.Equals(m.Scope, InstallScopes.PerMachine,
                    StringComparison.OrdinalIgnoreCase) ? 1 : 0;
                txtDir.Text = m.DefaultInstallDir ?? string.Empty;
                chkAlwaysElevate.Checked = m.AlwaysElevate;
            }
            finally
            {
                _loading = false;
            }

            txtIcon.Text = build == null ? string.Empty : build.IconPath ?? string.Empty;

            txtCom.Text = m.Com == null || m.Com.Count == 0
                ? string.Empty
                : string.Join(Environment.NewLine, m.Com.Select(c => c.Path).ToArray());
        }

        /// <summary>读回清单。**不动 EntryPoint** —— 那一项在主界面上。</summary>
        public void Read(InstallerManifest m, ProjectBuildSettings build)
        {
            m.Scope = cboScope.SelectedIndex == 1 ? InstallScopes.PerMachine : InstallScopes.PerUser;
            m.DefaultInstallDir = txtDir.Text.Trim();
            m.AlwaysElevate = chkAlwaysElevate.Checked;

            if (build != null)
            {
                build.IconPath = txtIcon.Text.Trim();
            }

            var com = new List<ComSpec>();
            foreach (var line in txtCom.Lines)
            {
                var path = line.Trim();
                if (path.Length > 0)
                {
                    com.Add(new ComSpec { Path = path, Mode = ComRegistrationModes.RegSvr32 });
                }
            }

            m.Com = com;
        }

        /// <summary>刷新文案。</summary>
        public void ApplyText(IStringTable text)
        {
            lblScope.Text = text.Get("Advanced.Scope");
            lblDir.Text = text.Get("Advanced.InstallDir");
            lblIcon.Text = text.Get("Advanced.Icon");
            lblCom.Text = text.Get("Advanced.Com");
            lblComHint.Text = text.Get("Advanced.Com.Hint");
            chkAlwaysElevate.Text = text.Get("Advanced.AlwaysElevate");
            btnIcon.Text = text.Get("Output.Change");

            var index = cboScope.SelectedIndex;
            cboScope.Items.Clear();
            cboScope.Items.Add(new AntdUI.SelectItem(text.Get("Advanced.ScopePerUser"), InstallScopes.PerUser));
            cboScope.Items.Add(new AntdUI.SelectItem(text.Get("Advanced.ScopePerMachine"), InstallScopes.PerMachine));
            cboScope.SelectedIndex = index < 0 ? 0 : index;
        }

        /// <summary>
        /// 切换安装范围时把默认目录换成对应的那一档。
        ///
        /// 只在"当前目录正好是另一档的默认值"时才换 —— 用户手填的路径一律不动。
        /// 不换的话会出现"范围=所有用户、目录还在 %LOCALAPPDATA%"这种自相矛盾的包。
        /// </summary>
        private void SyncDirToScope()
        {
            if (_loading)
            {
                return;
            }

            var name = ResolveProductName();
            var allUsers = cboScope.SelectedIndex == 1;
            var wanted = InstallPaths.Suggest(name, allUsers);
            var other = InstallPaths.Suggest(name, !allUsers);
            var current = txtDir.Text.Trim();

            if (string.IsNullOrWhiteSpace(current) ||
                string.Equals(current, other, StringComparison.OrdinalIgnoreCase))
            {
                txtDir.Text = wanted;
            }
        }

        /// <summary>取产品名（优先简体中文，其次默认语言，最后取第一个）。</summary>
        private string ResolveProductName()
        {
            var name = _manifest == null ? null : _manifest.Product == null ? null : _manifest.Product.Name;
            if (name == null || name.Count == 0)
            {
                return "我的产品";
            }

            string value;
            if (name.TryGetValue("zh-Hans", out value) && !string.IsNullOrWhiteSpace(value))
            {
                return value;
            }

            if (_manifest.DefaultCulture != null && name.TryGetValue(_manifest.DefaultCulture, out value)
                && !string.IsNullOrWhiteSpace(value))
            {
                return value;
            }

            return name.Values.FirstOrDefault(v => !string.IsNullOrWhiteSpace(v)) ?? "我的产品";
        }

        private void PickIcon()
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.Filter = "图标 (*.ico)|*.ico|所有文件 (*.*)|*.*";
                dialog.CheckFileExists = true;

                var current = txtIcon.Text.Trim();
                if (!string.IsNullOrEmpty(current))
                {
                    var dir = Path.GetDirectoryName(current);
                    if (!string.IsNullOrEmpty(dir) && Directory.Exists(dir))
                    {
                        dialog.InitialDirectory = dir;
                    }
                }

                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    txtIcon.Text = dialog.FileName;
                }
            }
        }
    }
}
