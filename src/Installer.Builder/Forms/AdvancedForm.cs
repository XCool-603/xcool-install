using System;
using System.Windows.Forms;
using Installer.Abstractions.Model;
using Installer.Abstractions.Platform;
using Installer.Builder.Presenters;
using Installer.Builder.Views;

namespace Installer.Builder.Forms
{
    /// <summary>
    /// 高级选项对话框。
    ///
    /// **主界面上不放这些东西** —— 许可协议、启动程序、COM 注册都是少数情况下才需要改的，
    /// 摆在主界面只会把"选文件夹 → 生成"这条主路径淹掉。
    /// </summary>
    public partial class AdvancedForm : AntdUI.BaseForm
    {
        private readonly BuilderPresenter _presenter;
        private readonly AdvancedLicenseView _licenseView = new AdvancedLicenseView { Dock = DockStyle.Fill };
        private readonly AdvancedInstallView _installView = new AdvancedInstallView { Dock = DockStyle.Fill };

        /// <summary>构造。</summary>
        /// <param name="presenter">制作端状态。</param>
        public AdvancedForm(BuilderPresenter presenter)
        {
            _presenter = presenter;
            InitializeComponent();

            // 让两个视图撑满 TabPage 并保持白底，视觉上就是一张卡片
            _licenseView.Dock = System.Windows.Forms.DockStyle.Fill;
            _licenseView.BackColor = System.Drawing.Color.White;
            _installView.Dock = System.Windows.Forms.DockStyle.Fill;
            _installView.BackColor = System.Drawing.Color.White;

            var licensePage = new AntdUI.TabPage { Text = "许可协议" };
            licensePage.Controls.Add(_licenseView);

            var installPage = new AntdUI.TabPage { Text = "安装细节" };
            installPage.Controls.Add(_installView);

            tabs.Pages.Add(licensePage);
            tabs.Pages.Add(installPage);

            ApplyText();
            Write();
        }

        /// <summary>确定后是否要标记工程为脏。</summary>
        public bool Changed { get; private set; }

        private void ApplyText()
        {
            var t = _presenter.Text;

            Text = _presenter.S("Advanced.Title");
            btnOk.Text = _presenter.S("Advanced.Ok");
            btnCancel.Text = _presenter.S("Advanced.Cancel");

            if (tabs.Pages.Count >= 2)
            {
                tabs.Pages[0].Text = _presenter.S("Advanced.License");
                tabs.Pages[1].Text = _presenter.S("Tab.Install");
            }

            _licenseView.ApplyText(t);
            _installView.ApplyText(t);
        }

        private void Write()
        {
            _licenseView.Write(_presenter.Document.Project.Manifest);
            _installView.Write(_presenter.Document.Project.Manifest, _presenter.Document.Project.Build);
        }

        private void BtnOkClick(object sender, EventArgs e)
        {
            _licenseView.Read(_presenter.Document.Project.Manifest);
            _installView.Read(_presenter.Document.Project.Manifest, _presenter.Document.Project.Build);

            Changed = true;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void BtnCancelClick(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
