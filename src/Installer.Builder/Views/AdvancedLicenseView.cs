using System;
using System.Windows.Forms;
using Installer.Abstractions.Model;
using Installer.Abstractions.Platform;

namespace Installer.Builder.Views
{
    /// <summary>高级选项 · 许可协议。</summary>
    public partial class AdvancedLicenseView : UserControl
    {
        /// <summary>构造。</summary>
        public AdvancedLicenseView()
        {
            InitializeComponent();
        }

        /// <summary>写回界面。</summary>
        public void Write(InstallerManifest m)
        {
            if (m.License == null)
            {
                m.License = new LicenseInfo { Text = new LocalizedText() };
            }

            chkRequired.Checked = m.License.Required;
            txtZh.Text = Get(m.License.Text, "zh-Hans");
            txtEn.Text = Get(m.License.Text, "en");
        }

        /// <summary>读回清单。</summary>
        public void Read(InstallerManifest m)
        {
            if (m.License == null)
            {
                m.License = new LicenseInfo();
            }

            m.License.Required = chkRequired.Checked;
            m.License.Text = new LocalizedText
            {
                { "zh-Hans", txtZh.Text },
                { "en", txtEn.Text },
            };
        }

        /// <summary>刷新文案。</summary>
        public void ApplyText(IStringTable text)
        {
            chkRequired.Text = text.Get("Advanced.LicenseRequired");
            lblZh.Text = text.Get("Advanced.LicenseZh");
            lblEn.Text = text.Get("Advanced.LicenseEn");
        }

        private static string Get(LocalizedText t, string culture)
        {
            if (t == null)
            {
                return string.Empty;
            }

            string v;
            return t.TryGetValue(culture, out v) ? v : string.Empty;
        }
    }
}
