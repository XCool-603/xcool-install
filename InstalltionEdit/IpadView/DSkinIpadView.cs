using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DSkin.Common;
using DSkin.Controls;
using DSkin.DirectUI;

namespace IPadDemo
{
    /// <summary>
    /// 动态列表，支持用鼠标拖拽调整顺序，以及绑定TabControl
    /// </summary>
    [Designer("DSkin.Design.DSkinIpadViewDesigner"), Description("动态列表，支持用鼠标拖拽调整顺序，以及绑定TabControl")]
    public class DSkinIpadView : DSkinBaseControl
    {
        #region 构造函数

        public DSkinIpadView()
        {
            this.SizeChanged += delegate
            {
                this.InnerDuiControl.ManagedTask(LayoutContent);
            };
            this.DUIControls.Add(BottomContainer);
            this.DUIControls.Add(_container);
            this.InnerDuiControl.ManagedTask(LayoutContent);
            this.MouseWheel += (s, e) =>
            {

            };
        }
        #endregion
        private DuiBaseControl _container = new DuiBaseControl { Dock = DockStyle.Fill };
        private DuiBaseControl BottomContainer = new DuiBaseControl { Dock = DockStyle.Bottom,Height = 20};
        #region 重写属性

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public override DuiControlCollection DUIControls
        {
            get { return base.DUIControls; }
        }

        #endregion

        #region 属性
        private int _row = 4;
        private int _columns = 4;
        private int b_ottomBtnSize = 20;
         [Description("矩阵的行数"),Category("DSkin")]
        public int MatrixRows
        {
            get { return _row; }
             set
             {
                 _row = value;
                 this.InnerDuiControl.ManagedTask(LayoutContent);
             }
        }
         [Description("矩阵的列数"), Category("DSkin")]
         public int MatrixColumns
        {
            get { return _columns; }
             set
             {
                 _columns = value;
                 this.InnerDuiControl.ManagedTask(LayoutContent);
             }
        } 
        [Description("底部泡泡按钮的大小，宽度=高度"), Category("DSkin")]
         public int BottomBtnSize
        {
            get { return b_ottomBtnSize; }
             set
             {
                 b_ottomBtnSize = value;
                 this.InnerDuiControl.ManagedTask(LayoutContent);
             }
        }
         [Description("是否底部显示泡泡按钮"), Category("DSkin")]
         public bool IsShowBottomContainer
         {
             get { return BottomContainer.Visible; }
             set
             {
                 BottomContainer.Visible = value;
                 if (BottomContainer.Visible)
                 {
                     BottomContainer.Height = 20;
                 }
                 else
                 {
                     BottomContainer.Height = 0;
                 }
                 this.InnerDuiControl.ManagedTask(LayoutContent);
             }
         }
        #endregion

        #region 平板项集合

        private Collection<DSkinIpadViewItem> _items;

        /// <summary>
        /// 平板项集合
        /// </summary>
        [Description("平板项集合"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
         Category("DSkin")]
        public Collection<DSkinIpadViewItem> Items
        {
            get
            {
                if (_items == null)
                {
                    _items = new Collection<DSkinIpadViewItem>();
                    _items.ItemAdded += ItemAdded;
                    _items.ItemRemoved += ItemRemoved;
                }
                return _items;
            }
        }

        private void ItemAdded(object sender, CollectionEventArgs<DSkinIpadViewItem> e)
        {
            e.Item.MouseEventBubble = false;
            e.Item.NewTabPageIndex = Items.Count - 1;
            DSkinIpadViewItem beforeItem = FindItemFroNewPageIndex(e.Item.NewTabPageIndex - 1);
            if (beforeItem != null)
            {
                e.Item.Top = beforeItem.Top + beforeItem.Height;
            }
            else
            {
                e.Item.Top = 0;
            }
            
            _container.Controls.Add(e.Item);
            this.InnerDuiControl.ManagedTask(LayoutContent);
            //if ((!DesignMode) && (TabControl != null))
            //{
            //    AddItem(e.Item);
            //}
            e.Item.MouseClick += Item_MouseClick;
        }

        private void ItemRemoved(object sender, CollectionEventArgs<DSkinIpadViewItem> e)
        {
            e.Item.MouseClick -= Item_MouseClick;
            _container.Controls.Remove(e.Item);
            this.InnerDuiControl.ManagedTask(LayoutContent);
        }

        private void Item_MouseClick(object sender, DuiMouseEventArgs e)
        {
            DSkinIpadViewItem item = sender as DSkinIpadViewItem;
            OnItemClick(new ItemClickEventArgs(Items.IndexOf(item), item, e.Button, e.Clicks, e.X, e.Y, e.Delta));
        }


        /// <summary>
        /// 移除标签，如果绑定了TabControl，同时也会移除对应的Page
        /// </summary>
        /// <param name="item"></param>
        public void RemoveItem(DSkinIpadViewItem item)
        {
            Items.Remove(item);
            //SetPosition();
        }

        #endregion

        #region 方法

        //public readonly object LayoutContentTask=new object();

        /// <summary>
        /// 刷新列表，对目标重新布局
        /// </summary>
        public void LayoutContent()
        {
            SetPosition();
        }

        /// <summary>
        /// 查找标签，根据最新下标查找
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public DSkinIpadViewItem FindItemFroNewPageIndex(int index)
        {
            for (int i = 0; i < Items.Count; i++)
            {
                if (Items[i].NewTabPageIndex == index)
                {
                    return Items[i];
                }
            }
            return null;
        }
        public DSkinIpadViewItem FindMoveItemToItem(DSkinIpadViewItem item)
        {
            Rectangle r = item.ClientRectangle;
            for (int i = 0; i < Items.Count; i++)
            {
                if (item == Items[i])
                {
                    continue;
                }
                Rectangle temp = Items[i].ClientRectangle;
                temp.Intersect(r);
                if ((temp.Width * temp.Height * 1.0) >= (r.Width * r.Height * 3.0 / 5.0))
                {
                    return Items[i];
                }
            }
            return null;
        }
        /// <summary>
        /// 获取标签在集合中的下标，非新下标
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int GetPageIndex(DSkinIpadViewItem item)
        {
            for (int i = 0; i < Items.Count; i++)
            {
                if (Items[i] == item)
                {
                    return i;
                }
            }
            return -1;
        }

        private int _pageIndex=0;

        public int PageIndex
        {
            get { return _pageIndex; }
            set
            {
                int move = 0;
                if (value>_pageIndex)
                {
                    move =- (value-_pageIndex)*_container.Width;
                }
                else if (value<_pageIndex )
                {
                    move=  (_pageIndex - value) * _container.Width;
                }
                _pageIndex = value;
                for (int i = 0; i < Items.Count; i++)
                {
                    Items[i].MoveToLeft = Items[i].Left+move;
                    Items[i].DoEffect(Items[i].Left, Items[i].MoveToLeft, 300, "Left", (a) => { });
                }
            }
        }

        /// <summary>
        /// 设置标签位置
        /// </summary>
        private void SetPosition()
        {
            //获取每页容纳的Item数量
            int matrixCount = MatrixRows * MatrixColumns;
            //计算总页数
            int pagecount = Items.Count/matrixCount + (Items.Count%matrixCount == 0 ? 0 : 1);
            //当前页下标
            int pageindex = 0;
            //第N行
            int rowindex = 0;
            //第N列
            int columnindex = 0;
            //添加按钮
             BottomContainer.Controls.Clear();
             int left = BottomContainer.Width / 2 - pagecount * (b_ottomBtnSize+4) / 2;
            
            for (int i = 0; i < pagecount; i++)
            {
                DuiButton btn = new DuiButton() { Text = "", IsPureColor = true, BaseColor = Color.White, Radius = b_ottomBtnSize, Size = new Size(b_ottomBtnSize, b_ottomBtnSize), Location = new Point(left, BottomContainer.Height / 2 - b_ottomBtnSize / 2), Tag = i.ToString() };
                left += b_ottomBtnSize + 4;
                btn.MouseClick += (s, e) =>
                {
                    ((DuiButton) s).BaseColor = Color.White;
                    PageIndex = int.Parse(((DuiButton) s).Tag.ToString());
                    for (int j = 0; j < BottomContainer.Controls.Count; j++)
                    {
                        if (BottomContainer.Controls[j] != ((DuiButton) s))
                        {
                            ((DuiButton) BottomContainer.Controls[j]).BaseColor = Color.Silver;
                        }
                    }
                };
                BottomContainer.Controls.Add(btn);
            }
            for (int i = 0; i < Items.Count; i++)
            {
                Items[i].Width = _container.Width / MatrixColumns;
                Items[i].Height = _container.Height / MatrixRows;
                columnindex = Items[i] .NewTabPageIndex% MatrixColumns;
                rowindex = (Items[i].NewTabPageIndex) / MatrixColumns % MatrixRows;
                pageindex = Items[i].NewTabPageIndex / matrixCount;
                Items[i].Location = new Point(pageindex * _container.Width + columnindex * Items[i].Width, rowindex * Items[i].Height);
                Items[i].OldLocation = new Point(columnindex * Items[i].Width, rowindex * Items[i].Height);
            }
            Invalidate();
        }
        #endregion

        #region 控件自定义事件

        public event EventHandler<ItemClickEventArgs> ItemClick;

        protected virtual void OnItemClick(ItemClickEventArgs e)
        {
            if (ItemClick != null)
            {
                ItemClick(this, e);
            }
        }

        #endregion
    }
}