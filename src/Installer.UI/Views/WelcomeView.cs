using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Installer.UI.Common;

namespace Installer.UI.Views
{
    /// <summary>
    /// 欢迎页（设计稿第 1 步）。
    ///
    /// 结构：大标题 + 两段正文 + 浅灰信息卡（3 行：图标 + 标签 + 值）+ 黄色提示条。
    /// </summary>
    public partial class WelcomeView : UserControl
    {
        /// <summary>构造。</summary>
        public WelcomeView()
        {
            InitializeComponent();
            ApplyAccent(WizardTheme.Primary);
        }

        /// <summary>大标题。</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Title
        {
            get { return lblTitle.Text; }
            set { lblTitle.Text = value; }
        }

        /// <summary>正文第一段。</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Body
        {
            get { return lblBody.Text; }
            set { lblBody.Text = value; }
        }

        /// <summary>正文第二段（设计稿是独立一段，不是同一段里的换行）。</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Body2
        {
            get { return lblBody2.Text; }
            set { lblBody2.Text = value; }
        }

        /// <summary>设置主色（安装 = 蓝，卸载 = 红）。</summary>
        public void ApplyAccent(Color accent)
        {
            infoCard.Back = WizardTheme.SoftBack;

            // 信息卡每行的图标块：浅蓝底（只设 Back，设 BackColor 会把圆角盖成直角）
            // 行容器默认底色是白的，会盖在信息卡的浅灰底上（只设 Back，不设 BackColor）
            row1.Back = WizardTheme.SoftBack;
            row2.Back = WizardTheme.SoftBack;
            row3.Back = WizardTheme.SoftBack;

            boxDir.Back = WizardTheme.PrimarySoft;
            boxSpace.Back = WizardTheme.PrimarySoft;
            boxVersion.Back = WizardTheme.PrimarySoft;

            hintPanel.Back = WizardTheme.WarnBack;
            lblHint.ForeColor = WizardTheme.WarnText;

            lblTitle.ForeColor = WizardTheme.TextTitle;
            lblBody.ForeColor = WizardTheme.TextBody;
        }

        /// <summary>默认安装位置。</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string DirValue
        {
            get { return lblDirValue.Text; }
            set { lblDirValue.Text = value ?? "—"; }
        }

        /// <summary>需要磁盘空间。</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string SpaceValue
        {
            get { return lblSpaceValue.Text; }
            set { lblSpaceValue.Text = value ?? "—"; }
        }

        /// <summary>版本与发布者。</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string VersionValue
        {
            get { return lblVersionValue.Text; }
            set { lblVersionValue.Text = value ?? "—"; }
        }

        /// <summary>黄色提示条文案。</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Hint
        {
            get { return lblHint.Text; }
            set { lblHint.Text = value ?? string.Empty; }
        }

        /// <summary>
        /// 更新页：把信息卡三行换成「当前已安装版本 / 将要安装的版本 / 安装目录」。
        ///
        /// 复用欢迎页的布局（标题 + 说明 + 三行信息卡 + 提示条），
        /// 因为「更新信息」页本来就是一张纯信息页，没必要再做一套控件。
        /// </summary>
        public void ApplyUpdateCaptions(string installed, string next, string dir)
        {
            lblDirCaption.Text = installed;
            lblSpaceCaption.Text = next;
            lblVersionCaption.Text = dir;
        }

        /// <summary>刷新信息卡的三个标签文案。</summary>
        public void ApplyCaptions(Func<string, string> lookup)
        {
            lblDirCaption.Text = Lookup(lookup, "Welcome.CaptionDir", "默认安装位置");
            lblSpaceCaption.Text = Lookup(lookup, "Welcome.CaptionSpace", "需要磁盘空间");
            lblVersionCaption.Text = Lookup(lookup, "Welcome.CaptionVersion", "版本与发布者");
        }

        private static string Lookup(Func<string, string> lookup, string key, string fallback)
        {
            if (lookup == null)
            {
                return fallback;
            }

            var v = lookup(key);
            return string.IsNullOrEmpty(v) || string.Equals(v, key, StringComparison.Ordinal) ? fallback : v;
        }
    }
}
