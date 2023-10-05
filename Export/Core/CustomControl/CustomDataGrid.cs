using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Core.CustomControl
{
    public class CustomDataGrid : DataGrid//DataGridControl
    {
        public CustomDataGrid()
        {
            this.CanUserAddRows = false;
            this.CanUserResizeColumns = true;
            this.CanUserSortColumns = true;
            this.AutoGenerateColumns = false;
            this.Style = Application.Current.FindResource("CustomDataGrid") as Style;
            this.GridLinesVisibility = DataGridGridLinesVisibility.All;
            EnableColumnVirtualization = true;
            EnableRowVirtualization = true;
            this.RowHeight = 25;
            this.Loaded += CustomDataGrid_Loaded;
            this.SelectionChanged += CustomDataGrid_SelectionChanged;
        }

        void CustomDataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            this.RowHeaderWidth = 0;
        }
        private void CustomDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.SelectedItemsList = this.SelectedItems;
        }

        #region SelectedItemsList

        public IList SelectedItemsList
        {
            get { return (IList)GetValue(SelectedItemsListProperty); }
            set { SetValue(SelectedItemsListProperty, value); }
        }

        public static readonly DependencyProperty SelectedItemsListProperty =
                DependencyProperty.Register("SelectedItemsList", typeof(IList), typeof(CustomDataGrid), new PropertyMetadata(null));



        #endregion SelectedItemsList
    }
}
