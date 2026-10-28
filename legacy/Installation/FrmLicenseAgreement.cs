using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Installation
{
    public partial class FrmLicenseAgreement : FrmBase
    {
        public FrmLicenseAgreement()
        {
            InitializeComponent();
            this.CloseBox.MouseClick += (s, e) =>
            {
                e.Handled = true;
                CloseForm(DialogResult.Cancel);
            };
            TxtInstallationPath.Text = Code.LicenseAgreement;
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }
    }
}
