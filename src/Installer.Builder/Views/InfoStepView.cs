using System;
using System.Windows.Forms;
using Installer.Abstractions.Model;
using Installer.Abstractions.Platform;

using Installer.Builder.Common;

namespace Installer.Builder.Views
{
    /// <summary>第 2 步：基本信息（中英文名称 / 版本 / 厂商）。</summary>
    public partial class InfoStepView : UserControl
    {
        /// <summary>构造。</summary>
        private bool _mirroring;
        private bool _englishEditedByUser;

        public InfoStepView()
        {
            InitializeComponent();

            txtNameZh.TextChanged += (sender, e) =>
            {
                MirrorEnglish();
                Changed?.Invoke();
            };

            // 用户自己填过英文名之后就不再跟随
            txtNameEn.TextChanged += (sender, e) =>
            {
                if (!_mirroring)
                {
                    _englishEditedByUser = true;
                }
            };
        }

        /// <summary>
        /// 英文名留空时跟随中文名。
        /// 这样英文系统上跑向导不会显示空产品名 —— 打包时也会做同样的补齐，
        /// 这里只是让它**在界面上可见**。
        /// </summary>
        private void MirrorEnglish()
        {
            if (_englishEditedByUser)
            {
                return;
            }

            _mirroring = true;
            try
            {
                txtNameEn.Text = txtNameZh.Text.Trim();
            }
            finally
            {
                _mirroring = false;
            }
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
            _englishEditedByUser = false;
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

        /// <summary>填入产品名（自动推导时用）。</summary>
        public void SetProductName(string name)
        {
            txtNameZh.Text = name ?? string.Empty;
        }

        /// <summary>刷新文案。</summary>
        public void ApplyText(IStringTable text)
        {
            lblStep.Text = text.Get("Step.Info");
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
    
        /// <summary>
        /// 未选文件夹时把整张卡片淡化。
        /// 设计稿的做法是**改背景色**（白 → #F7F8FA）+ 徽章变浅，
        /// 不是简单地把控件 Enabled 置 false。
        /// </summary>
        public void SetDimmed(bool dimmed)
        {
            var back = dimmed
                ? System.Drawing.Color.FromArgb(247, 248, 250)
                : System.Drawing.Color.White;

            BackColor = back;
            lblStep.PrefixSvg = StepBadge.Create(2, dimmed);
        }}
}
