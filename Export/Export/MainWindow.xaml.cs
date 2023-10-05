﻿using Core.Base;
using Core.Model;
using Export.ViewModel;
//using Export.Util;
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

namespace Export
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : WindowBase
    {
        MainWindowViewModel vm = null;
        public MainWindow()
        {
            InitializeComponent();
            vm = this.DataContext as MainWindowViewModel;
            vm.Close += Vm_Close;
            vm.CheckServiceConnected();
            vm.LoadConfig();
        }

        private void Vm_Close()
        {
            this.Close();
        }
    }
}
