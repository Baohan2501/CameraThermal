using Core.Base;
using Core.Model;
using Export.Service;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using Newtonsoft.Json;
using OxyPlot.Legends;
using OxyPlot;
using OxyPlot.Axes;

namespace Export.ViewModel
{
    public class LiveViewModel : ViewModelBaseExtend
    {
        private int maxItemliveInChar = 200;
        private int maxItemliveInGrid = 1000;
        public event EventHandler RefreshChart;
        #region Property
        public bool IsRunning { get; set; } = false;
            

        private CameraZone camera = new CameraZone();
        public CameraZone Camera
        {
            get
            {
                return camera;
            }
            set
            {
                if (camera != null)
                    camera.ZoneChange -= Camera_ZoneChange;
                camera = value;

                if (camera != null)
                {
                    camera.Zones.ToList().ForEach(p => p.IsSelected = true);
                    camera.ZoneChange += Camera_ZoneChange;
                }

                RaisePropertyChanged("Camera");
            }
        }

        private void Camera_ZoneChange(object sender, EventArgs e)
        {
            if (PlotModel != null)
            {
                //Show/Hide Series chart
                foreach (var item in PlotModel.Series)
                {
                    if (Camera.Zones.Any(p => !p.IsSelected && p.ZoneId == (int)item.Tag))
                        item.IsVisible = false;
                    else
                        item.IsVisible = true;
                }
                if (RefreshChart != null)
                    RefreshChart(null, null);
            }
        }

        private ObservableCollection<TemperatureInfo> temperatureInfos = new ObservableCollection<TemperatureInfo>();
        public ObservableCollection<TemperatureInfo> TemperatureInfos
        {
            get
            {
                return temperatureInfos;
            }
            set
            {
                temperatureInfos = value;
                RaisePropertyChanged("TemperatureInfos");
            }
        }

        private ObservableCollection<TemperatureInfo> liveAlarmTemperatureInfos = new ObservableCollection<TemperatureInfo>();
        public virtual ObservableCollection<TemperatureInfo> LiveAlarmTemperatureInfos
        {
            get
            {
                return liveAlarmTemperatureInfos;
            }
            set
            {
                liveAlarmTemperatureInfos = value;
                RaisePropertyChanged("LiveAlarmTemperatureInfos");
            }
        }
        private PlotModel plotModel;// = new PlotModel();
        public PlotModel PlotModel
        {
            get
            {
                return plotModel;
            }
            set
            {
                plotModel = value;
                RaisePropertyChanged("PlotModel");
            }
        }

        private readonly List<OxyColor> colors = new List<OxyColor>
                                            {
                                                OxyColors.Green,
                                                OxyColors.IndianRed,
                                                OxyColors.Coral,
                                                OxyColors.Chartreuse,
                                                OxyColors.Azure,
                                                OxyColors.Black
                                            };

        private readonly List<MarkerType> markerTypes = new List<MarkerType>
                                                   {
                                                       MarkerType.Plus,
                                                       MarkerType.Star,
                                                       MarkerType.Diamond,
                                                       MarkerType.Triangle,
                                                       MarkerType.Cross,
                                                       MarkerType.Square,
                                                   };

        #endregion

        #region ICommand
        private ICommand exportToExcelCommand;
        public ICommand ExportToExcelCommand
        {
            get
            {
                if (exportToExcelCommand == null)
                    exportToExcelCommand = new RelayCommand<string>((obj) => OnExportToExcelCommand(obj));
                return exportToExcelCommand;
            }
        }

        #endregion

        #region Contructor
        public LiveViewModel()
        {
            IsRunning = true;
        }
        #endregion

        #region Public

        public void Start(CameraZone cameraZone)
        {
            IsRunning = true;
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                if (TemperatureInfos != null)
                    TemperatureInfos.Clear();
                if (LiveAlarmTemperatureInfos != null)
                    LiveAlarmTemperatureInfos.Clear();
            });

            ResetCharting();
            Task.Run(async () =>
            {
                while (IsRunning)
                {
                    TemperatureResponse data = await CameraServiceSingleton.Instance.GetDataLiveAsync(Camera.CameraId);
                        //TemperatureResponse data = strData != null ? JsonConvert.DeserializeObject<TemperatureResponse>(strData) : null;
                        if (data != null && cameraZone != null)
                    {
                        foreach (var zone in data.Success.GetZoneStatusSuccess)
                        {
                            TemperatureInfo temperature = new TemperatureInfo()
                            {
                                CameraId = data.Success.CameraId,
                                DateTime = data.Success.DateTime,
                                CameraName = cameraZone.CameraName
                            };

                            temperature.AverageTemperature = zone.AverageTemperature;
                            temperature.MinimumTemperature = zone.MinimumTemperature;
                            temperature.ZoneId = zone.ZoneId;
                            AlarmZoneSetting alarmZone = cameraZone.Zones.FirstOrDefault(p => p.ZoneId == zone.ZoneId);
                            temperature.ZoneName = alarmZone != null ? alarmZone.ZoneName : string.Empty;
                            System.Windows.Application.Current.Dispatcher.Invoke(() =>
                            {
                                if (PlotModel != null && PlotModel.Series != null)
                                {
                                    if (PlotModel.Series.Any(p => (int)p.Tag == zone.ZoneId))
                                    {
                                        var lineSerie = PlotModel.Series[zone.ZoneId] as OxyPlot.Series.LineSeries;
                                        if (lineSerie != null)
                                        {
                                            if (lineSerie.Points.Count > maxItemliveInChar)
                                                lineSerie.Points.RemoveAt(0);
                                            lineSerie.Points.Add(new DataPoint(DateTimeAxis.ToDouble(data.Success.DateTime), zone.AverageTemperature));
                                        }
                                    }
                                    else
                                    {
                                        var lineSeria = new OxyPlot.Series.LineSeries()
                                        {
                                            StrokeThickness = 2,
                                            MarkerSize = 3,
                                            MarkerStroke = colors[zone.ZoneId],
                                            MarkerType = markerTypes[zone.ZoneId],
                                            CanTrackerInterpolatePoints = false,
                                            Title = temperature.ZoneName,
                                            Tag = zone.ZoneId
                                        };
                                        lineSeria.Points.Add(new DataPoint(DateTimeAxis.ToDouble(data.Success.DateTime), zone.AverageTemperature));
                                        PlotModel.Series.Add(lineSeria);
                                    }

                                    if (TemperatureInfos.Count >= maxItemliveInGrid)
                                        TemperatureInfos.RemoveAt(maxItemliveInGrid - 1);
                                    TemperatureInfos.Insert(0, temperature);

                                        //insert Live alarm 

                                        if (zone.IsAlarm)
                                    {
                                        if (LiveAlarmTemperatureInfos.Count >= 200)
                                            LiveAlarmTemperatureInfos.RemoveAt(199);
                                        LiveAlarmTemperatureInfos.Insert(0, temperature);
                                    }
                                }
                            });
                        }
                        if (RefreshChart != null)
                            RefreshChart(null, null);
                    }
                    await Task.Delay(10000);
                }
            });

        }


        public void Stop()
        {
            IsRunning = false;
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                LiveAlarmTemperatureInfos.Clear();
                TemperatureInfos.Clear();
                ResetCharting();
                GC.Collect();
            });

        }

        public void DrawChartHistory()
        {
            ResetCharting();
            if (TemperatureInfos != null)
            {
                var dataPer = TemperatureInfos.GroupBy(p => p.ZoneId).OrderBy(m => m.Key).ToList();

                foreach (var item in dataPer)
                {
                    var lineSeria = new OxyPlot.Series.LineSeries()
                    {
                        StrokeThickness = 2,
                        MarkerSize = 3,
                        MarkerStroke = colors[item.Key],
                        MarkerType = markerTypes[item.Key],
                        CanTrackerInterpolatePoints = false,
                        Title = Camera.Zones.FirstOrDefault(p => p.ZoneId == item.Key).ZoneName,
                        Tag = item.Key
                    };
                    item.ToList().ForEach(d => lineSeria.Points.Add(new DataPoint(DateTimeAxis.ToDouble(d.DateTime), d.AverageTemperature)));
                    PlotModel.Series.Add(lineSeria);
                }

                Camera_ZoneChange(null, null);
            }
        }

        public virtual void SetupModel()
        {
            PlotModel.Legends.Add(new Legend());
            string formatX = "HH:mm:ss";
            PlotModel.Legends[0].LegendTitle = "Zones";
            PlotModel.Legends[0].LegendOrientation = LegendOrientation.Horizontal;
            PlotModel.Legends[0].LegendPlacement = LegendPlacement.Outside;
            PlotModel.Legends[0].LegendPosition = LegendPosition.TopRight;
            PlotModel.Legends[0].LegendBackground = OxyColor.FromAColor(200, OxyColors.White);
            PlotModel.Legends[0].LegendBorder = OxyColors.Black;
            var dateAxis = new DateTimeAxis() { Title = "DateTime", Angle = -45, Position = AxisPosition.Bottom, StringFormat = formatX, MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot, IntervalLength = 60 };
            PlotModel.Axes.Add(dateAxis);
            var valueAxis = new LinearAxis() { Position = AxisPosition.Left, Minimum = 0, Maximum = 60, MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot, Title = "AverageTemperature" };
            PlotModel.Axes.Add(valueAxis);
        }

        private void OnExportToExcelCommand(string isLiveAlarm)
        {
            try
            {
                Core.Util.ExportExcel exportExcel = new Core.Util.ExportExcel();
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Excel file|*.xlsx";
                saveFileDialog.Title = "Save an Excel file";
                saveFileDialog.ShowDialog();
                if (!string.IsNullOrEmpty(saveFileDialog.FileName))
                {
                    if (isLiveAlarm.ToLower() == "true")
                        exportExcel.ExportTemperature(LiveAlarmTemperatureInfos.ToList(), saveFileDialog.FileName);
                    else
                        exportExcel.ExportTemperature(TemperatureInfos.ToList(), saveFileDialog.FileName);
                    Core.Message.Control.MessageBoxView.Show("Information", "Save file success!");
                }
            }
            catch (Exception ex)
            {
                Core.Log.LogManager.Error("Exception export to excel :" + ex.Message);
            }

        }

        #endregion

        #region Private

        private void ResetCharting()
        {
            if (PlotModel != null)
            {
                PlotModel.Series.Clear();
            }
            PlotModel = new PlotModel();
            SetupModel();
        }

        public void Reset()
        {
            LiveAlarmTemperatureInfos = new ObservableCollection<TemperatureInfo>();
            TemperatureInfos = new ObservableCollection<TemperatureInfo>();
            ResetCharting();
        }
        #endregion
    }

    public enum Mode
    {
        Live,
        Search
    }
}
