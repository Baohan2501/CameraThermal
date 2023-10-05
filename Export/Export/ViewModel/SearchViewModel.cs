using Core.Base;
using Core.Model;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Legends;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Export.ViewModel
{
    public class SearchViewModel : LiveViewModel
    {
        #region Property
        public override ObservableCollection<TemperatureInfo> LiveAlarmTemperatureInfos
        {
            get
            {
                return base.LiveAlarmTemperatureInfos;
            }
            set
            {
                base.LiveAlarmTemperatureInfos = value;
                RaisePropertyChanged("LiveAlarmTemperatureInfos");
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
                RaisePropertyChanged("IsShowNormal");
            }
        }

        public bool IsShowNormal
        {
            get
            {
                return !IsLiveAlarm;
            }
        }

        public override void SetupModel()
        {
            PlotModel.Legends.Add(new Legend());
            string formatX = "dd/MM/yyyy HH:mm:ss";
            PlotModel.Legends[0].LegendTitle = "Zones";
            PlotModel.Legends[0].LegendOrientation = LegendOrientation.Horizontal;
            PlotModel.Legends[0].LegendPlacement = LegendPlacement.Outside;
            PlotModel.Legends[0].LegendPosition = LegendPosition.TopRight;
            PlotModel.Legends[0].LegendBackground = OxyColor.FromAColor(200, OxyColors.White);
            PlotModel.Legends[0].LegendBorder = OxyColors.Black;
            var dateAxis = new DateTimeAxis() {Position=AxisPosition.Bottom,Title="DateTime",StringFormat=formatX, MajorGridlineStyle = LineStyle.Automatic,Angle=-45, MinorGridlineStyle = LineStyle.Dot, IntervalLength = 20 };
            PlotModel.Axes.Add(dateAxis);
            var valueAxis = new LinearAxis() {Position=AxisPosition.Left,Minimum=0, Maximum = 60, MajorGridlineStyle = LineStyle.Automatic, MinorGridlineStyle = LineStyle.Dot, Title = "AverageTemperature" };
            PlotModel.Axes.Add(valueAxis);
        }

        #endregion
    }
}
