using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Installer.Abstractions.Model;
using Installer.Abstractions.Platform;
using Installer.Builder.Presenters;

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
            _loading = true;
            try
            {
                cboScope.SelectedIndex = string.Equals(m.Scope, InstallScopes.PerMachine,
                    StringComparison.OrdinalIgnoreCase) ? 1 : 0;

                // 这里显示的是**根目录**，不是最终目录：
                // 最终目录是「根\厂商\产品」，拼装发生在打包时（Build → InstallPaths.Compose）。
                // 直接回显最终目录的后果：用户再点一次打包就会变成「...\产品\厂商\产品」。
                txtDir.Text = InstallPaths.SplitBase(m.DefaultInstallDir,
                    BuilderPresenter.PickVendorName(m), BuilderPresenter.PickProductName(m)) ?? string.Empty;

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

            // 注意：存进清单的是**根目录**。最终目录（根\厂商\产品）在打包时算，
            // 这样改了产品名/厂商名之后目录会自动跟着变，不会留一个旧名字的死路径。
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

            // 输入框里填的是**根目录**，最终目录是「根目录\厂商\产品名」——
            // 这条规则写在占位提示里，用户清空输入框时就能看到。
            txtDir.PlaceholderText = text.Get("Advanced.InstallDir.Hint");
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
        /// 切换安装范围时把**根目录**换成对应的那一档
        /// （<c>%LOCALAPPDATA%\Programs</c> ↔ <c>%ProgramFiles%</c>）。
        ///
        /// 只在"当前值正好是另一档的默认根目录"时才换 —— 用户手填的路径一律不动。
        /// 不换的话会出现"范围=所有用户、根目录还在 %LOCALAPPDATA%"这种自相矛盾的包。
        /// </summary>
        private void SyncDirToScope()
        {
            if (_loading)
            {
                return;
            }

            var allUsers = cboScope.SelectedIndex == 1;
            var wanted = InstallPaths.Root(allUsers);
            var other = InstallPaths.Root(!allUsers);
            var current = txtDir.Text.Trim();

            if (string.IsNullOrWhiteSpace(current) ||
                string.Equals(current, other, StringComparison.OrdinalIgnoreCase))
            {
                txtDir.Text = wanted;
            }
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
