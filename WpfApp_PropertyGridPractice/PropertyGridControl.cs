using DevExpress.Xpf.PropertyGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp_PropertyGridPractice
{
    public class PropertyGridControl : DevExpress.Xpf.PropertyGrid.PropertyGridControl
    {
        public PropertyGridControl()
        {
        }

        protected override CategoriesShowMode OnShowCategoriesChanging(CategoriesShowMode newValue)
        {
            if (newValue == CategoriesShowMode.Hidden)
                SortMode = DevExpress.Xpf.PropertyGrid.PropertyGridSortMode.Ascending;
            else
                SortMode = DevExpress.Xpf.PropertyGrid.PropertyGridSortMode.NoSort;

            return base.OnShowCategoriesChanging(newValue);
        }
    }
}
