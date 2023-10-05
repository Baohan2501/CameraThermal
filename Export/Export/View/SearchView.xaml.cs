using Export.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Export.View
{
    /// <summary>
    /// Interaction logic for SearchView.xaml
    /// </summary>
    public partial class SearchView : UserControl
    {
        SearchViewModel vm = null;
        public SearchView()
        {
            InitializeComponent();
            this.Loaded += SearchView_Loaded;
            this.Unloaded += SearchView_Unloaded;
        }

        private void SearchView_Unloaded(object sender, RoutedEventArgs e)
        {
            if (vm != null)
                vm.RefreshChart -= Vm_RefreshChart;
        }

        private void SearchView_Loaded(object sender, RoutedEventArgs e)
        {
            vm = this.DataContext as SearchViewModel;
            vm.RefreshChart += Vm_RefreshChart;
        }

        private void Vm_RefreshChart(object sender, EventArgs e)
        {
            Dispatcher.InvokeAsync(() =>
            {
                chartData.InvalidatePlot(true);
            });
        }
    }
}
