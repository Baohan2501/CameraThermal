using Core.Base;
using GalaSoft.MvvmLight.Command;
using MonitorService.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MonitorService.ViewModel
{
    public class MainWindowViewModel : ViewModelBaseExtend
    {
        private DataConfigView dataConfigView;
        private ServiceController sc = new ServiceController("WindowThermalCameraService");
        #region Property
        private ServiceStatus serviceStatus;
        public ServiceStatus ServiceStatus
        {
            get
            {
                return serviceStatus;
            }
            set
            {
                serviceStatus = value;
                RaisePropertyChanged("ServiceStatus");
            }
        }

        private bool isVisible = true;
        public bool IsVisible
        {
            get
            {
                return isVisible;
            }
            set
            {
                isVisible = value;
                RaisePropertyChanged("IsVisible");
            }
        }

        private System.Windows.Visibility isLoading = System.Windows.Visibility.Collapsed;
        public System.Windows.Visibility IsLoading
        {
            get
            {
                return isLoading;
            }
            set
            {
                isLoading = value;
                RaisePropertyChanged("IsLoading");
            }
        }

        #endregion

        #region Contructor
        public MainWindowViewModel()
        {
            LoadstatusSevice(sc);
        }

        #endregion

        #region ICommand
        private ICommand restartCommand;
        public ICommand RestartCommand
        {
            get
            {
                if (restartCommand == null)
                    restartCommand = new RelayCommand(() => OnRestartCommand());
                return restartCommand;
            }
        }

        private ICommand startCommand;
        public ICommand StartCommand
        {
            get
            {
                if (startCommand == null)
                    startCommand = new RelayCommand(() => OnStartCommand());
                return startCommand;
            }
        }

        private ICommand configDataCommand;
        public ICommand ConfigDataCommand
        {
            get
            {
                if (configDataCommand == null)
                    configDataCommand = new RelayCommand(() => OnConfigDataCommand());
                return configDataCommand;
            }
        }
        

        #endregion

        #region Public function

        #endregion

        #region Private function
        private void OnStartCommand()
        {
            
            if (ServiceStatus == ServiceStatus.Running && sc != null)
            {
                if (Core.Message.Control.MessageBoxView.Show("Question", "Do you want to stop the service ?", System.Windows.MessageBoxButton.YesNo, Core.Message.Control.MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    Task.Run(()=> {
                        IsLoading = Visibility.Visible;
                        sc.Stop();
                        sc.WaitForStatus(ServiceControllerStatus.Stopped);
                        ServiceStatus = ServiceStatus.Stop;
                        IsLoading = Visibility.Collapsed;
                    });
                }
            }
            else if (ServiceStatus == ServiceStatus.Stop && sc != null)
            {
                Task.Run(() =>
                {
                    IsLoading = Visibility.Visible;
                    sc.Start();
                    sc.WaitForStatus(ServiceControllerStatus.Running);
                    ServiceStatus = ServiceStatus.Running;
                    IsLoading = Visibility.Collapsed;
                });
            }
        }

        private void LoadstatusSevice(ServiceController sc)
        {
            Task.Run(() =>
            {
                IsLoading = Visibility.Visible;
                while (ServiceStatus != ServiceStatus.Running && ServiceStatus != ServiceStatus.Stop)
                {
                    switch (sc.Status)
                    {
                        case ServiceControllerStatus.Running:
                            ServiceStatus = ServiceStatus.Running;
                            break;
                        case ServiceControllerStatus.Stopped:
                            ServiceStatus = ServiceStatus.Stop;
                            break;
                        default:
                            ServiceStatus = ServiceStatus.None;
                            break;
                    }
                    Task.Delay(200);
                    sc.Refresh();
                }
                IsLoading = Visibility.Collapsed;
            });

        }

        private void OnRestartCommand()
        {
            if (Core.Message.Control.MessageBoxView.Show("Question", "Do you want to restart the service ?", MessageBoxButton.YesNo, Core.Message.Control.MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                IsLoading = Visibility.Visible;
                Task.Run(() =>
                {
                    ServiceStatus = ServiceStatus.Stopping;

                    //Stop service
                    IsLoading = Visibility.Visible;
                    sc.Stop();
                    sc.WaitForStatus(ServiceControllerStatus.Stopped);
                    ServiceStatus = ServiceStatus.Stop;

                    //Start service
                    sc.Start();
                    sc.WaitForStatus(ServiceControllerStatus.Running);
                    ServiceStatus = ServiceStatus.Running;
                    IsLoading = Visibility.Collapsed;
                });
            }
        }

        private void OnConfigDataCommand()
        {
            dataConfigView = new DataConfigView();
            dataConfigView.ShowDialog();
        }

        #endregion
    }
    public enum ServiceStatus
    {
        None,
        Stop,
        Stopping,
        Running
    }

}
