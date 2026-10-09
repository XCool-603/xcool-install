using System;
using System.Windows.Forms;
using Installer.Abstractions.Model;
using Installer.Abstractions.Platform;

namespace Installer.Builder.Views
{
    /// <summary>第 2 步：基本信息（中英文名称 / 版本 / 厂商）。</summary>
    public partial class InfoStepView : UserControl
    {
        /// <summary>构造。</summary>
        public InfoStepView()
        {
            InitializeComponent();
            txtNameZh.TextChanged += (sender, e) => Changed?.Invoke();
        }

        /// <summary>内容变化。</summary>
        public event Action Changed;

        /// <summary>取中/英文名称与版本、厂商。</summary>
        public void Read(out LocalizedText name, out string version, out LocalizedText publisher)
        {
            name = new LocalizedText
            {
                { "zh-Hans", txtNameZh.Text.Trim() },
                { "en", txtNameEn.Text.Trim() },
            };

            version = txtVersion.Text.Trim();

            publisher = new LocalizedText
            {
                { "zh-Hans", txtPublisher.Text.Trim() },
                { "en", txtPublisher.Text.Trim() },
            };
        }

        /// <summary>写回界面。</summary>
        public void Write(LocalizedText name, string version, LocalizedText publisher)
        {
            txtNameZh.Text = Get(name, "zh-Hans");
            txtNameEn.Text = Get(name, "en");
            txtVersion.Text = version ?? string.Empty;
            txtPublisher.Text = Get(publisher, "zh-Hans");
        }

        /// <summary>当前产品名（优先中文）。</summary>
        public string CurrentProductName
        {
            get
            {
                var zh = txtNameZh.Text.Trim();
                return zh.Length > 0 ? zh : txtNameEn.Text.Trim();
            }
        }

        /// <summary>当前版本。</summary>
        public string Version
        {
            get { return txtVersion.Text.Trim(); }
        }

        /// <summary>刷新文案。</summary>
        public void ApplyText(IStringTable text)
        {
            lblName.Text = text.Get("Field.Name");
            lblNameEn.Text = text.Get("Field.NameEn");
            lblVersion.Text = text.Get("Field.Version");
            lblPublisher.Text = text.Get("Field.Publisher");

            txtNameZh.PlaceholderText = "我的产品";
            txtNameEn.PlaceholderText = text.Get("Field.NameEn.Hint");
            txtVersion.PlaceholderText = "1.0.0";
            txtPublisher.PlaceholderText = "—";
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
