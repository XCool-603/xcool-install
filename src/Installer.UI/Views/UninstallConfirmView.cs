using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Installer.UI.Common;

namespace Installer.UI.Views
{
    /// <summary>
    /// 确认卸载页（设计稿第 3 版「确认卸载」，688×764）。
    ///
    /// 结构：大标题 + 副标题 + 浅灰信息卡（3 行：图标 + 标签 + 值）
    ///       + 复选框（同时删除用户配置与缓存数据）+ 红色警告条。
    ///
    /// 这是卸载向导的第 1 页。此前这一页是借 <see cref="FinishView"/>（SetNeutral）凑出来的，
    /// 结构与设计稿差得远 —— 独立成视图后安装/卸载两条线互不干扰。
    ///
    /// 坐标系与字号换算见 Designer.cs 顶部的注释块。
    /// </summary>
    public partial class UninstallConfirmView : UserControl
    {
        /// <summary>构造。</summary>
        public UninstallConfirmView()
        {
            InitializeComponent();

            // 卸载向导的红色（#D5303D）；安装向导是蓝，这里不跟随清单主色。
            ApplyAccent(WizardTheme.Danger);
        }

        /// <summary>「同时删除用户配置与缓存数据」勾选状态变化。</summary>
        public event EventHandler DeleteUserDataChanged;

        /// <summary>大标题，设计稿：卸载「{产品名}」。</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Title
        {
            get { return lblTitle.Text; }
            set { lblTitle.Text = value ?? string.Empty; }
        }

        /// <summary>副标题。</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Subtitle
        {
            get { return lblSubtitle.Text; }
            set { lblSubtitle.Text = value ?? string.Empty; }
        }

        /// <summary>信息卡第 1 行：当前安装位置。</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string DirValue
        {
            get { return lblValue1.Text; }
            set { lblValue1.Text = value ?? "—"; }
        }

        /// <summary>信息卡第 2 行：占用磁盘空间。</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string SpaceValue
        {
            get { return lblValue2.Text; }
            set { lblValue2.Text = value ?? "—"; }
        }

        /// <summary>信息卡第 3 行：安装时间与版本。</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string VersionValue
        {
            get { return lblValue3.Text; }
            set { lblValue3.Text = value ?? "—"; }
        }

        /// <summary>复选框文案。</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string DeleteDataText
        {
            get { return lblCleanupText.Text; }
            set { lblCleanupText.Text = value ?? string.Empty; }
        }

        /// <summary>复选框下面的灰色提示。</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string DeleteDataHint
        {
            get { return lblCleanupHint.Text; }
            set { lblCleanupHint.Text = value ?? string.Empty; }
        }

        /// <summary>是否勾选「同时删除用户配置与缓存数据」。</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool DeleteUserData
        {
            get { return chkCleanup.Checked; }
            set { chkCleanup.Checked = value; }
        }

        /// <summary>红色警告条第 1 行。</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string WarningLine1
        {
            get { return lblWarnLine1.Text; }
            set { lblWarnLine1.Text = value ?? string.Empty; }
        }

        /// <summary>红色警告条第 2 行。</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string WarningLine2
        {
            get { return lblWarnLine2.Text; }
            set { lblWarnLine2.Text = value ?? string.Empty; }
        }

        /// <summary>
        /// 设置主色。
        ///
        /// AntdUI.Panel 的圆角靠它自己的 Back 画出来，所以这里只设 Back，绝不设 BackColor
        /// （设了 BackColor 会用直角把圆角盖掉）；面板内的 Label 已在 Designer 里
        /// 显式设成 Transparent，不会各画一块白底。
        /// </summary>
        /// <param name="accent">复选框勾选色；卸载向导传 #D5303D。</param>
        public void ApplyAccent(Color accent)
        {
            // 信息卡：浅灰底
            infoCard.Back = WizardTheme.SoftBack;

            // 行容器默认底色是白的，会盖在信息卡的浅灰底上，跟着设成同一色
            row1.Back = WizardTheme.SoftBack;
            row2.Back = WizardTheme.SoftBack;
            row3.Back = WizardTheme.SoftBack;

            // 图标块：设计稿里卸载页沿用浅蓝底 #E6F4FF（不是红色调）
            boxInfo1.Back = WizardTheme.PrimarySoft;
            boxInfo2.Back = WizardTheme.PrimarySoft;
            boxInfo3.Back = WizardTheme.PrimarySoft;

            // 红色警告条：底 #FFF1F0（设计稿实测），文字 #D5303D
            warnPanel.Back = WizardTheme.DangerBack;
            lblWarnLine1.ForeColor = WizardTheme.Danger;
            lblWarnLine2.ForeColor = WizardTheme.Danger;

            // AntdUI 的复选框默认勾选色是品牌蓝，卸载向导要红
            chkCleanup.Fill = accent;

            lblTitle.ForeColor = WizardTheme.TextTitle;
            lblSubtitle.ForeColor = WizardTheme.TextBody;
        }

        /// <summary>刷新信息卡的三个标签文案。</summary>
        /// <param name="lookup">文案查表函数（null 时用设计稿内置中文）。</param>
        public void ApplyCaptions(Func<string, string> lookup)
        {
            lblCaption1.Text = Lookup(lookup, "Uninstall.CaptionDir", "当前安装位置");
            lblCaption2.Text = Lookup(lookup, "Uninstall.CaptionSpace", "占用磁盘空间");
            lblCaption3.Text = Lookup(lookup, "Uninstall.CaptionVersion", "安装时间与版本");
        }

        /// <summary>点复选框右边那行文字也能切换勾选状态。</summary>
        private void CleanupTextClick(object sender, EventArgs e)
        {
            chkCleanup.Checked = !chkCleanup.Checked;
        }

        private void CleanupCheckedChanged(object sender, AntdUI.BoolEventArgs e)
        {
            var handler = DeleteUserDataChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
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
