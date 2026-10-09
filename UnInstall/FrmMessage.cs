using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace UnInstall
{
    public partial class FrmMessage : FrmBase
    {
        public FrmMessage(string title, string msg, bool bCancel)
        {
            InitializeComponent();
            Text = title;
            LblMsg.Text = msg;
            if (bCancel)
            {
                BtnOk.Visible = BtnCancel.Visible = true;
                //lblOK.Location = new Point(230, 137);
                //lblCancel.Location = new Point(292, 137);
            }
            else
            {
                BtnOk.Visible = true;
                BtnCancel.Visible = false;
                BtnOk.Location = new Point(
                    Width / 2 - BtnOk.Width / 2, (Height - PnlMain.Height - PnlMain.Top) / 2 - BtnOk.Height / 2 + PnlMain.Height + PnlMain.Top);
            }
            this.CloseBox.MouseClick+= (s, e) =>
            {
                e.Handled = true;
                this.DialogResult=DialogResult.Cancel;
            };
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
           this.DialogResult=DialogResult.OK;
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult=DialogResult.Cancel;
        }
    }
}
