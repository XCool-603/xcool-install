using System;
using System.ComponentModel;
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
        /// <summary>线条图的颜色（跟随主色：安装=蓝，卸载=红）。</summary>
        private Color _glyphColor = WizardTheme.Primary;

        /// <summary>品牌区底色（图标里的结构缝用它画，缝才看不出来）。</summary>
        private Color _headerBack = WizardTheme.Primary;

        /// <summary>当前是不是卸载向导（决定画包裹还是垃圾桶）。</summary>
        private bool _uninstall;
        /// <summary>构造。</summary>
        public WizardHeaderView()
        {
            InitializeComponent();
            ApplyAccent(WizardTheme.Primary);
            ApplyGlyph(false);
        }

        /// <summary>
        /// 画品牌区里的图标：**纯色线条图**（安装=向下箭头，卸载=垃圾桶）。
        ///
        /// 图标块是白底，线条用主色 —— 蓝底/红底上都对比清楚，也不再受"产品图标太花"影响。
        /// </summary>
        public void ApplyGlyph(bool uninstall)
        {
            _uninstall = uninstall;

            var old = picIcon.Image;
            picIcon.Image = HeaderGlyph.Create(uninstall, picIcon.Width, _glyphColor, _headerBack);
            if (old != null)
            {
                old.Dispose();
            }

            picIcon.Visible = true;

            // 立方体占位图彻底不用了
            lblIcon.Visible = false;
        }

        /// <summary>设置主色（安装 = 蓝，卸载 = 红）。</summary>
        public void ApplyAccent(Color accent)
        {
            _glyphColor = Color.White;
            _headerBack = accent;
            brandPanel.Back = accent;
            // 判定性实验：暂时不设 WinForms 的 BackColor

            // 图标直接画在品牌区上：白底圆角块去掉了（用户要求"别加纯色背景"），
            // 图形用白色实心，蓝底/红底上都清楚。

            // 产品名：品牌区上的标题一律白字（以前没设 ForeColor，默认黑色 → 蓝底黑字）
            lblProduct.ForeColor = Color.White;

            // 副标题比产品名淡一档
            lblSubtitle.ForeColor = Blend(accent, Color.White, 0.78);

            // 主色变了，线条图要跟着重画
            ApplyGlyph(_uninstall);
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
