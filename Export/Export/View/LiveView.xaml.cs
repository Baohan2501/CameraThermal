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
    /// Interaction logic for LiveView.xaml
    /// </summary>
    public partial class LiveView : UserControl
    {
        LiveViewModel vm = null;
        public LiveView()
        {
            InitializeComponent();
            this.Loaded += LiveView_Loaded;
            this.Unloaded += LiveView_Unloaded;
        }

        private void LiveView_Unloaded(object sender, RoutedEventArgs e)
        {
            if (vm != null)
                vm.RefreshChart -= Vm_RefreshChart;
        }

        private void LiveView_Loaded(object sender, RoutedEventArgs e)
        {
            vm = this.DataContext as LiveViewModel;
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
