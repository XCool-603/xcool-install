using System;
using System.ComponentModel.Design;
using System.Drawing;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms.Design;
using System.Windows.Forms;
using DSkin.Common;
using DSkin.Controls;

namespace DSkin.Design
{
    public class DSkinIpadViewDesigner : ControlDesigner
    {
        private DesignerActionListCollection actionLists;

        // 下拉菜单
        public override DesignerActionListCollection ActionLists
        {
            get
            {
                if (null == actionLists)
                {
                    actionLists = new DesignerActionListCollection();
                    actionLists.Add(
                        new DSkinIpadViewDesignerActionList(this.Component));
                }
                return actionLists;
            }
        }
    }

    class DSkinIpadViewDesignerActionList : DesignerActionList
    {
        public DSkinIpadViewDesignerActionList(IComponent c)
            : base(c)
        {

        }

        public Collection<DSkinIpadViewItem> Items
        {
            get { return (Collection<DSkinIpadViewItem>)GetPropertyByName("Items").GetValue(this.Component); }
        }

        private PropertyDescriptor GetPropertyByName(String propName)
        {
            PropertyDescriptor prop = TypeDescriptor.GetProperties(this.Component)[propName];
            if (null == prop)
                throw new ArgumentException(
                     "property not found!",
                      propName);
            else
                return prop;
        }
    }
}
