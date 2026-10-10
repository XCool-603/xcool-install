using System;
using System.Drawing;
using System.Windows.Forms;
using Installer.UI.Common;

namespace Installer.UI.Views
{
    /// <summary>
    /// 向导顶部的蓝色品牌区（高 104）。
    ///
    /// 设计稿特征：卡片顶部一块品牌蓝，左侧 48×48 圆角图标块，右侧产品名 + 副标题。
    /// 卸载向导用同一块，只是副标题换成「卸载向导 · 版本 x」、主色换成红。
    /// </summary>
    public partial class WizardHeaderView : UserControl
    {
        /// <summary>构造。</summary>
        public WizardHeaderView()
        {
            InitializeComponent();
            ApplyAccent(WizardTheme.Primary);
        }

        /// <summary>设置主色（安装 = 蓝，卸载 = 红）。</summary>
        public void ApplyAccent(Color accent)
        {
            brandPanel.Back = accent;
            // 判定性实验：暂时不设 WinForms 的 BackColor

            // 图标块是「白 20% 叠在主色上」，用实色近似（AntdUI.Panel.Back 不吃 alpha）
            var iconBack = Blend(accent, Color.White, 0.20);
            iconPanel.Back = iconBack;

            // 副标题比产品名淡一档
            lblSubtitle.ForeColor = Blend(accent, Color.White, 0.78);
        }

        /// <summary>刷新文案。</summary>
        /// <param name="lookup">文案查找（key → 文本），可为 null。</param>
        /// <param name="productName">产品名。</param>
        /// <param name="version">版本号。</param>
        /// <param name="uninstall">是否卸载向导。</param>
        public void ApplyText(Func<string, string> lookup, string productName, string version, bool uninstall)
        {
            lblProduct.Text = string.IsNullOrEmpty(productName) ? "（未命名产品）" : productName;

            var kind = uninstall
                ? Fallback(lookup, "Wizard.UninstallKind", "卸载向导")
                : Fallback(lookup, "Wizard.InstallKind", "安装向导");

            lblSubtitle.Text = kind
                               + " · "
                               + Fallback(lookup, "Wizard.Version", "版本")
                               + " "
                               + (version ?? string.Empty);
        }

        private static string Fallback(Func<string, string> lookup, string key, string fallback)
        {
            if (lookup == null)
            {
                return fallback;
            }

            var v = lookup(key);

            // 文案表查不到时会原样返回 key，这种情况也算缺失
            if (string.Equals(v, key, StringComparison.Ordinal))
            {
                return fallback;
            }
            return string.IsNullOrEmpty(v) ? fallback : v;
        }

        /// <summary>按比例把两种颜色混合。</summary>
        private static Color Blend(Color baseColor, Color over, double ratio)
        {
            var r = (int)(baseColor.R * (1 - ratio) + over.R * ratio);
            var g = (int)(baseColor.G * (1 - ratio) + over.G * ratio);
            var b = (int)(baseColor.B * (1 - ratio) + over.B * ratio);
            return Color.FromArgb(r, g, b);
        }
    }
}
