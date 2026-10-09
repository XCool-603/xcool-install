using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Net;
using System.Windows.Forms;
using DSkin.Controls;
using DSkin.DirectUI;
using DSkin.Forms;
using IPadDemo;

namespace DSkin.Controls
{
    public class DSkinIpadViewItem : DuiButton
    {
        public DSkinIpadViewItem()
        {
            this.Text = "Item";
            this.IsPureColor = true;
        }


        #region 属性
        [DefaultValue(true)]
        public override bool IsPureColor
        {
            get
            {
                return base.IsPureColor;
            }
            set
            {
                base.IsPureColor = value;
            }
        }

        #region 父容器【DSkinIpadView】
        /// <summary>
        /// 父容器【DSkinIpadView】
        /// </summary>
        public DSkinIpadView HostDSkinIpadView
        {
            get
            {
                return this.HostControl as DSkinIpadView;
            }
        }
        #endregion

        #region Item容器Top动画移动到新的Top位置
        /// <summary>
        /// Item容器Top动画移动到新的Top位置
        /// </summary>
        public int MoveToPoint = 0;
        #endregion

        #region 移动后标签的新下标，用于重新排序位置
        /// <summary>
        /// 移动后标签的新下标，用于重新排序位置
        /// </summary>
        private int _newTabPagIndex = -1;

        [Description("移动后标签的新下标，用于重新排序位置")]
        public int NewTabPageIndex
        {
            get { return _newTabPagIndex; }
            set
            {
                _newTabPagIndex = value;
            }
        }
        #endregion

        #region 当前标签是否选中
        /// <summary>
        /// 当前标签是否选中
        /// </summary>
        private bool _isSelected = false;
        [Description("当前标签是否选中")]
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                DSkinIpadView host = HostDSkinIpadView;
                if (_isSelected)
                {
                    this.ControlState = ControlStates.Pressed;
                }
                else
                {
                    this.ControlState = ControlStates.Normal;
                }
            }
        }
        #endregion

        #endregion

        #region 标签容器
        private Point _oldLocation = new Point(0, 0);
        public Point OldLocation {
            get { return _oldLocation; }
            set { _oldLocation = value; }
        }

        /// <summary>
        /// 是否可移动标签
        /// </summary>
        public bool ChangedMove = false;


        #region 移动标签方法及事件

        private bool isleftdown = false;
        ////显示按钮的透明度
        //private float _btnOpacity = 1.0f;
        //存储当前鼠标位置
        private Point MouseXY;
        ////防止重复左右移动
        public bool IsMove = false;
        ////是否显示添加标签按钮
        //private bool _isshowaddbutton = true;
        /// <summary>
        /// 标签按钮Down事件，标记开始移动标签
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseDown(DuiMouseEventArgs e)
        {
            base.OnMouseDown(e);
            DSkinIpadView hostControl = HostDSkinIpadView;
            if (hostControl == null)
            {
                return;
            }
            if (e.Button == MouseButtons.Left && e.Clicks == 1)
            {
                isleftdown = true;
                //hostControl.SelectedItem = this;
                IsSelected = true;

                //OldLocation = this.Location;

                MouseXY = new Point(-e.X, -e.Y);
                //_isshowaddbutton = false;
            }
            else if (e.Button == MouseButtons.Right)
            {
                //hostChromeTab.MenuSelecteditem = this;
                //if (hostChromeTab.ItemMenu != null)
                //{
                //    hostChromeTab.ItemMenu.Show(new Point(Item.LocationToScreen.X + e.Location.X, Item.LocationToScreen.Y + e.Location.Y));
                //}
            }
        }
        /// <summary>
        /// 标签按钮Move事件，移动标签和其他标签交换位置
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseMove(DuiMouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (e.Button == MouseButtons.Left && isleftdown)
            {
                DSkinIpadView hostControl = HostDSkinIpadView;
                if (hostControl == null)
                {
                    return;
                }
                DSkinIpadViewItem changedTabItem = hostControl.FindMoveItemToItem(this);
                if (changedTabItem != null && (!changedTabItem.IsMove))
                {
                    //bool ischanged = true;
                    //Item.Tag = new Point(0, hostControl.GetHeight(changedTabItem.NewTabPageIndex));
                    int index = changedTabItem.NewTabPageIndex;
                    changedTabItem.NewTabPageIndex = NewTabPageIndex;
                    NewTabPageIndex = index;
                    changedTabItem.IsMove = true;
                    Point move = this.OldLocation;
                    this.OldLocation = changedTabItem.OldLocation;
                    changedTabItem.OldLocation = move;
                    MoveForVertical(changedTabItem);
                }
                Point p = Control.MousePosition;
                p.Offset(MouseXY.X, MouseXY.Y);
                Point nowp = hostControl.PointToClient(p);
                this.Location = new Point(nowp.X, nowp.Y);
            }
        }

        public void MoveForVertical(DSkinIpadViewItem item)
        {
            item.DoEffect(item.Top, item.OldLocation.Y, 120, "Top", (a) => { });
            item.DoEffect(item.Left, item.OldLocation.X, 120, "Left", (a) => { });
            item.DoEffect(() =>
            {
                if (item.Top != item.OldLocation.Y || item.Left != item.OldLocation.X)
                {
                    return true;
                }
                item.IsMove = false;
                return false;
            });
           
        }

        /// <summary>
        /// 标签按钮Up事件，停止其他标签交换位置并自动还原到已交换的标签位置
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseUp(DuiMouseEventArgs e)
        {
            MouseChangeControlState = !IsSelected;
            base.OnMouseUp(e);
            MouseChangeControlState = true;
            if (e.Button == MouseButtons.Left)
            {
                isleftdown = false;

                DSkinIpadView hostControl = HostDSkinIpadView;
                if (hostControl == null)
                {
                    return;
                }
                MoveForVertical(this);
                //Point locationPoint = new Point(0, hostControl.OffsetTop + hostControl.GetHeight(this.NewTabPageIndex));
                //this.DoEffect(this.Top, locationPoint.Y, 120, "Top", (a) => { });

                //Point locationPoint = new Point(hostControl.OffsetLeft + hostControl.GetWidth(this.NewTabPageIndex), 0);
                //this.DoEffect(this.Left, locationPoint.X, 120, "Left", (a) => { });

            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            MouseChangeControlState = !IsSelected;
            base.OnMouseLeave(e);
            MouseChangeControlState = true;
        }
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            MouseChangeControlState = !IsSelected;
            base.OnMouseEnter(e);
            MouseChangeControlState = true;
        }
        #endregion
        #endregion

        public int MoveToLeft = 0;

        private void InitializeComponent()
        {
            // 
            // DSkinIpadViewItem
            // 
            this.BaseColor = System.Drawing.Color.Transparent;
            this.ButtonBorderColor = System.Drawing.Color.Transparent;
            this.IsPureColor = true;
            this.Size = new System.Drawing.Size(100, 100);

        }
    }
    //public enum MoveDerection
    //{
    //    /// <summary>
    //    /// 垂直方向移动
    //    /// </summary>
    //    Vertical,
    //    /// <summary>
    //    /// 水平方向移动
    //    /// </summary>
    //    Horizontal
    //}
}
