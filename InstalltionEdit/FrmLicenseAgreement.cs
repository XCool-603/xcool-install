using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace InstalltionEdit
{
    public partial class FrmLicenseAgreement : FrmBase
    {
        public string License = "";
        public FrmLicenseAgreement(string license)
        {
            InitializeComponent();
            License = license;
            this.CloseBox.MouseClick += (s, e) =>
            {
                e.Handled = true;
                CloseForm(DialogResult.Cancel);
            };
            TxtInstallationPath.Text = license;
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            License = TxtInstallationPath.Text;
            this.DialogResult = DialogResult.OK;
        }
    }
}
