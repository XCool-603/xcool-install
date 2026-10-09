using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Installer.Abstractions.Model;
using Installer.Abstractions.Platform;

namespace Installer.Builder.Views
{
    /// <summary>第 3 步：快捷方式与启动行为。</summary>
    public partial class ShortcutStepView : UserControl
    {
        /// <summary>构造。</summary>
        public ShortcutStepView()
        {
            InitializeComponent();
            ForeColor = System.Drawing.Color.FromArgb(31, 41, 55);
        }

        /// <summary>选项变化。</summary>
        public event Action Changed;

        /// <summary>写回界面。</summary>
        public void Write(InstallerManifest m)
        {
            chkDesktop.Checked = Has(m, "Desktop");
            chkStartMenu.Checked = Has(m, "StartMenu");
            chkAutostart.Checked = m.Autostart;
            chkRunAfter.Checked = m.RunAfterInstall;
        }

        /// <summary>读回清单。</summary>
        public void Read(InstallerManifest m)
        {
            var shortcuts = new List<ShortcutSpec>();
            if (chkDesktop.Checked)
            {
                shortcuts.Add(new ShortcutSpec { Location = "Desktop", Name = new LocalizedText() });
            }

            if (chkStartMenu.Checked)
            {
                shortcuts.Add(new ShortcutSpec { Location = "StartMenu", Name = new LocalizedText() });
            }

            m.Shortcuts = shortcuts;
            m.Autostart = chkAutostart.Checked;
            m.RunAfterInstall = chkRunAfter.Checked;
        }

        /// <summary>刷新文案。</summary>
        public void ApplyText(IStringTable text)
        {
            lblStep.Text = text.Get("Step.Shortcuts");
            chkDesktop.Text = text.Get("Shortcut.Desktop");
            chkStartMenu.Text = text.Get("Shortcut.StartMenu");
            chkAutostart.Text = text.Get("Shortcut.Autostart");
            chkRunAfter.Text = text.Get("Shortcut.RunAfter");
        }

        /// <summary>诊断用：打印四个复选框的实际状态。</summary>
        public string DebugChecked()
        {
            return string.Format("desktop={0} startmenu={1} autostart={2} runafter={3}",
                chkDesktop.Checked, chkStartMenu.Checked, chkAutostart.Checked, chkRunAfter.Checked);
        }

        private void OnCheckedChanged(object sender, AntdUI.BoolEventArgs e)
        {
            var handler = Changed;
            if (handler != null)
            {
                handler();
            }
        }

        private static bool Has(InstallerManifest m, string location)
        {
            return m.Shortcuts != null && m.Shortcuts.Any(s =>
                string.Equals(s.Location, location, StringComparison.OrdinalIgnoreCase));
        }
    }
}
