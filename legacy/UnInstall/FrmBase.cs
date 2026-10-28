using System;
using System.Drawing;
using System.Windows.Forms;
using DSkin.Forms;

namespace UnInstall
{
    public partial class FrmBase : DSkinForm
    {
        public FrmBase()
        {
            InitializeComponent();
            this.CloseBox.MouseClick += (s,e)=>
            {

                if (IsAccordClose)
                {
                    e.Handled = true;
                    CloseForm();
                }
            };
        }

        private bool _isAccordClose = false;
        /// <summary>
        /// 关闭时是否自动显示动画
        /// </summary>
        public bool IsAccordClose {
            get { return _isAccordClose; }
            set { _isAccordClose = value; }
        }
        private bool _isAccordShow = false;
        /// <summary>
        /// 关闭时是否自动显示动画
        /// </summary>
        public bool IsAccordShow
        {
            get { return _isAccordShow; }
            set { _isAccordShow = value; }
        }
        public void ShowForm()
        {
            #region 窗体显示特效
            ShowShadow = true;
            //ShowShadow = false;
            Opacity = 0;
            Rectangle rect = Screen.PrimaryScreen.WorkingArea;
            int top = (rect.Height - Height) / 2;
            Location = new Point(rect.Width - Width, top);
            int centerLeft = (rect.Width - Width) / 2;

            DoEffect(() =>
            {
                if (Left > centerLeft + 4)
                {
                    Opacity = 1 - 1.0 * (Left - centerLeft) / (rect.Width - Width - centerLeft);
                    Left -= ((Left - centerLeft) / 5);
                    return true;
                }

                Opacity = 1;
                ShowShadow = true;
                return false;
            });

            #endregion
        }
        public void CloseForm()
        {
            //ShowShadow = false;
            #region 窗体关闭特效
            int left = Left;

            DoEffect(() =>
            {
                if (Left > 4)
                {
                    Opacity = 1.0 * Left / left;
                    Left -= Left / 5;
                    return true;
                }
                Close();
                return false;
            });
            #endregion
        }

        public void CloseForm(DialogResult dialogResult)
        {
            //ShowShadow = false;
            #region 窗体关闭特效
            int left = Left;

            DoEffect(() =>
            {
                if (Left > 4)
                {
                    Opacity = 1.0 * Left / left;
                    Left -= Left / 5;
                    return true;
                }
                this.DialogResult = dialogResult;
                return false;
            });
            #endregion
        }

        private void FrmAnimationBase_Load(object sender, EventArgs e)
        {
            if (!DesignMode && IsAccordShow)
            {
                ShowForm();
            }
        }
    }
}
