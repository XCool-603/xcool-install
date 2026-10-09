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
        /// <summary>构造。</summary>
        public AdvancedInstallView()
        {
            InitializeComponent();
            btnIcon.Click += (sender, e) => PickIcon();
        }

        /// <summary>写回界面。</summary>
        public void Write(InstallerManifest m, ProjectBuildSettings build)
        {
            cboScope.SelectedIndex = string.Equals(m.Scope, InstallScopes.PerMachine,
                StringComparison.OrdinalIgnoreCase) ? 1 : 0;
            txtDir.Text = m.DefaultInstallDir ?? string.Empty;
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
            btnIcon.Text = text.Get("Output.Change");

            var index = cboScope.SelectedIndex;
            cboScope.Items.Clear();
            cboScope.Items.Add(new AntdUI.SelectItem(text.Get("Advanced.ScopePerUser"), InstallScopes.PerUser));
            cboScope.Items.Add(new AntdUI.SelectItem(text.Get("Advanced.ScopePerMachine"), InstallScopes.PerMachine));
            cboScope.SelectedIndex = index < 0 ? 0 : index;
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
