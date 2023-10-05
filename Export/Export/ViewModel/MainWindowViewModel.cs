using Core.Base;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Collections.ObjectModel;
using Core.Model;
using Microsoft.Win32;
using System.IO;
using System.Xml.Serialization;
using Core.Util;
using Export.Service;
using Newtonsoft.Json;
using System.ServiceProcess;
using static Core.Util.Serializable;
using System.Threading;
using Core.SqlHelper;
using System.Data;

namespace Export.ViewModel
{
    public class MainWindowViewModel : ViewModelBaseExtend
    {
        public event Action Close;
        private const string ServiceName = "WindowThermalCameraService";

        #region Property
        public SqlHelper SqlHelper { set; get; } = new SqlHelper();

        private DateTime startDate = DateTime.Now;
        public DateTime StartDate
        {
            get
            {
                return startDate;
            }
            set
            {
                startDate = value;
                RaisePropertyChanged("StartDate");
            }
        }

        private DateTime endDate = DateTime.Now;
        public DateTime EndDate
        {
            get
            {
                return endDate;
            }
            set
            {
                endDate = value;
                RaisePropertyChanged("EndDate");
            }
        }

        //private ObservableCollection<ThermalInfo> thermalInfos = new ObservableCollection<ThermalInfo>();
        //public ObservableCollection<ThermalInfo> ThermalInfos
        //{
        //    get
        //    {
        //        return thermalInfos;
        //    }
        //    set
        //    {
        //        thermalInfos = value;
        //        RaisePropertyChanged("ThermalInfos");
        //    }
        //}

        //private ObservableCollection<Equipment> equipments = new ObservableCollection<Equipment>();
        //public ObservableCollection<Equipment> Equipments
        //{
        //    get
        //    {
        //        return equipments;
        //    }
        //    set
        //    {
        //        equipments = value;
        //        RaisePropertyChanged("Equipments");
        //    }
        //}

        private LiveViewModel currentItem = new LiveViewModel();
        public LiveViewModel CurrentItem
        {
            get
            {
                return currentItem;
            }
            set
            {
                currentItem = value;
                RaisePropertyChanged("CurrentItem");
            }
        }

        public List<CameraZone> CameraZones { get; set; }

        private ObservableCollection<SiteCamera> siteCameras;
        public ObservableCollection<SiteCamera> SiteCameras
        {
            get
            {
                return siteCameras;
            }
            set
            {
                siteCameras = value;
                RaisePropertyChanged("SiteCameras");
                if (SelectedTree == null && SiteCameras.Count > 0 && SiteCameras[0].CameraZones.Count > 0)
                    SelectedTree = SiteCameras[0].CameraZones[0];
            }
        }

        private CameraZone selectedCamera;
        public CameraZone SelectedCamera
        {
            get
            {
                return selectedCamera;
            }
            set
            {
                if (selectedCamera != value)
                {
                    selectedCamera = value;
                    RaisePropertyChanged("SelectedCamera");
                    RaisePropertyChanged("MainTitle");
                    Task.Run(async () =>
                    {
                        CurrentItem.Stop();
                        while(CurrentItem.IsRunning)
                        {
                            await Task.Delay(500);
                        }    
                        CurrentItem.Camera = SelectedCamera;
                        if (CurrentMode == Mode.Live)
                        {
                            CurrentItem.Reset();
                            CameraZone cameraZone = CameraZones.FirstOrDefault(p => p.CameraId == SelectedCamera.CameraId);
                            CurrentItem.Start(cameraZone);
                        }
                    });
                }
            }
        }

        public string MainTitle
        {
            get
            {
                if (SelectedCamera != null)
                    return CurrentMode == Mode.Live ? string.Format("Thermal camera- ({0})", SelectedCamera.CameraName) : IsLiveAlarm ? "Thermal camera" : string.Format("Thermal camera- ({0})", SelectedCamera.CameraName);
                return "Thermal camera";
            }
        }

        private object selectedTree;
        public object SelectedTree
        {
            get
            {
                return selectedTree;
            }
            set
            {
                selectedTree = value;
                RaisePropertyChanged("SelectedTree");

                if (value.GetType() == typeof(SiteCamera))
                {
                    SelectedCamera = ((SiteCamera)value).CameraZones[0];
                }
                else if (value.GetType() == typeof(CameraZone))
                {
                    SelectedCamera = ((CameraZone)value);
                }
            }
        }


        private Mode currentMode = Mode.Live;
        public Mode CurrentMode
        {
            get
            {
                return currentMode;
            }
            set
            {
                currentMode = value;
                RaisePropertyChanged("CurrentMode");
                RaisePropertyChanged("MainTitle");
                //if (CurrentItem != null)
                //    CurrentItem.CurrentMode = CurrentMode;
            }
        }

        private bool isMinute = true;
        public bool IsMinute
        {
            get
            {
                return isMinute;
            }
            set
            {
                isMinute = value;
                RaisePropertyChanged("IsMinute");
            }
        }
        private bool isHours;
        public bool IsHours
        {
            get
            {
                return isHours;
            }
            set
            {
                isHours = value;
                RaisePropertyChanged("IsHours");
            }
        }
        private bool isDay;
        public bool IsDay
        {
            get
            {
                return isDay;
            }
            set
            {
                isDay = value;
                RaisePropertyChanged("IsDay");
            }
        }

        private bool isLiveAlarm;
        public bool IsLiveAlarm
        {
            get
            {
                return isLiveAlarm;
            }
            set
            {
                isLiveAlarm = value;
                RaisePropertyChanged("IsLiveAlarm");
                RaisePropertyChanged("MainTitle");
                if (CurrentItem != null && CurrentItem is SearchViewModel)
                    ((SearchViewModel)CurrentItem).IsLiveAlarm = value;
            }
        }

        private bool isLoading;
        public bool IsLoading
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
            // LoadConfig();
        }

        #endregion

        #region ICommand
        private ICommand searchCommand;
        public ICommand SearchCommand
        {
            get
            {
                if (searchCommand == null)
                    searchCommand = new RelayCommand(() => OnSearchComand());
                return searchCommand;
            }
        }

        private ICommand exportCommand;
        public ICommand ExportCommand
        {
            get
            {
                if (exportCommand == null)
                    exportCommand = new RelayCommand(() => OnExportCommand());
                return exportCommand;
            }
        }

        private ICommand liveCommand;
        public ICommand LiveCommand
        {
            get
            {
                if (liveCommand == null)
                    liveCommand = new RelayCommand(() => OnLiveCommand());
                return liveCommand;
            }
        }

        private ICommand historyCommand;
        public ICommand HistoryCommand
        {
            get
            {
                if (historyCommand == null)
                    historyCommand = new RelayCommand(() => OnHistoryCommand());
                return historyCommand;
            }
        }

        #endregion

        #region Public function
        public void OnHistoryCommand()
        {
            if (CameraZones != null)
            {
                Task.Run(() =>
                {
                    if (CurrentItem != null)
                        CurrentItem.Stop();
                    CurrentItem = new SearchViewModel();
                    CurrentItem.Camera = SelectedCamera == null ? CameraZones[0] : SelectedCamera;
                    if (IsLiveAlarm)
                        IsLiveAlarm = false;
                    CurrentMode = Mode.Search;
                });
            }
        }

        public void LoadConfig()
        {
            Task.Run(async () =>
            {
                string strdata = await CameraServiceSingleton.Instance.GetSiteCamerasAsync();
                SqlHelper = new SqlHelper(await CameraServiceSingleton.Instance.GetSqlConnectionAsync());
                SiteCameras = JsonConvert.DeserializeObject<ObservableCollection<SiteCamera>>(strdata);
                CameraZones = SiteCameras.SelectMany(p => p.CameraZones).ToList();
                OnLiveCommand();
            });
        }

        public void CheckServiceConnected()
        {
            ServiceController service = ServiceController.GetServices().FirstOrDefault(s => s.ServiceName == ServiceName);
            if (service == null || service.Status != ServiceControllerStatus.Running)
            {
                Core.Message.Control.MessageBoxView.Show("Information", "Cannot connect to the service, you should check status of the service before run application", System.Windows.MessageBoxButton.OK, Core.Message.Control.MessageBoxImage.Information);
                if (Close != null)
                    Close();
            }
        }

        #endregion

        #region Private function
        private void OnSearchComand()
        {
            string cameraIdStr = string.Empty;
            Core.Log.LogManager.Info("Start Search");
            Task.Run(async () =>
            {
                try
                {
                    CurrentItem.Reset();
                    IsLoading = true;
                    if (IsLiveAlarm)
                    {
                        List<int> cameraIds = new List<int>();
                        SiteCameras.ToList().ForEach(p => cameraIds.AddRange(p.CameraZones.Where(g => g.IsSelected).Select(x => x.CameraId).ToList()));
                        cameraIdStr = string.Join(",", cameraIds);
                    }
                    else
                    {
                        cameraIdStr = SelectedCamera.CameraId.ToString();
                    }
                    var data = await CameraServiceSingleton.Instance.GetDataAsync(cameraIdStr, StartDate, EndDate, IsLiveAlarm ? 3 : (isMinute ? 0 : (isHours ? 1 : 2)));
                    List<TemperatureInfo> temperatureInfos = data != null ? data.ToList() : null;
                    if (temperatureInfos != null && temperatureInfos.Count > 0)
                    {
                        temperatureInfos = temperatureInfos.OrderByDescending(p => p.DateTime).ThenBy(g => g.ZoneId).ToList();
                        foreach (var item in temperatureInfos)
                        {
                            item.CameraName = SelectedCamera.CameraName;
                            AlarmZoneSetting alarmZone = SelectedCamera.Zones.FirstOrDefault(p => p.ZoneId == item.ZoneId);
                            item.ZoneName = alarmZone != null ? alarmZone.ZoneName : string.Empty;
                        }
                        if (IsLiveAlarm)
                        {
                            CurrentItem.LiveAlarmTemperatureInfos = new ObservableCollection<TemperatureInfo>(temperatureInfos);
                        }
                        else
                        {
                            CurrentItem.TemperatureInfos = new ObservableCollection<TemperatureInfo>(temperatureInfos);
                            System.Windows.Application.Current.Dispatcher.Invoke(() =>
                            {
                                CurrentItem.DrawChartHistory();
                            });
                        }
                    }
                    IsLoading = false;
                }
                catch (Exception ex)
                {
                    IsLoading = false;
                    Core.Log.LogManager.Error("Search data :" + ex.Message);
                }
                finally
                {
                    GC.Collect();
                }
            });
        }

        private void OnExportCommand()
        {
            //Export.Util.ExportUtil exportUtil = new Util.ExportUtil();
            //SaveFileDialog saveFileDialog = new SaveFileDialog();
            //saveFileDialog.Filter = "Excel |*.xlsx";
            //if (saveFileDialog.ShowDialog() == true)
            //{
            //    exportUtil.Export(saveFileDialog.FileName, ThermalInfos.ToList());
            //    System.Windows.MessageBox.Show("Export success !");
            //}
        }

        private void OnLiveCommand()
        {
            if (CameraZones != null)
            {
                if (CurrentItem is SearchViewModel)
                    CurrentItem = new LiveViewModel();
                if (SelectedCamera == null)
                    SelectedCamera = CameraZones[0];
                CurrentItem.Camera = SelectedCamera;
                CameraZone cameraZone = CameraZones.FirstOrDefault(p => p.CameraId == SelectedCamera.CameraId);
                CurrentItem.Start(cameraZone);
                CurrentMode = Mode.Live;
            }
        }


        private List<TemperatureInfo> GetData(string cameraId, DateTime startDate, DateTime endDate, int type)
        {
            List<TemperatureInfo> results = new List<TemperatureInfo>();
            try
            {
                CoreParameter[] coreParameter = new CoreParameter[4];
                string sqlQuery = string.Empty;
                coreParameter[0] = new CoreParameter("@cameraId", cameraId);
                coreParameter[1] = new CoreParameter("@startDate", startDate);
                coreParameter[2] = new CoreParameter("@endDate", endDate);
                coreParameter[3] = new CoreParameter("@type", type);
                DataTable data = SqlHelper.LoadTableFromStoreProcedure("GetDataThermalCameraInfo", coreParameter);
                results = Core.Util.Serializable.ConvertDataTable<TemperatureInfo>(data);
                Core.Log.LogManager.Info("Data count: " + results.Count);
                Core.Log.LogManager.Info("***** camera: " + cameraId);
                data.Clear();
                return results;
            }
            catch (Exception ex)
            {
                Core.Log.LogManager.Info("Exception Get data from database  : " + ex.Message);
                return null;
            }
        }
        #endregion
    }
}
