using Core.Base;
using Core.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Export.ViewModel
{
    public class HistoryViewModel : ViewModelBaseExtend
    {
        #region Property
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

        #endregion

        #region ICommand

        #endregion

        #region Contructor
        public HistoryViewModel()
        {
        }
        #endregion

        #region Public


        #endregion

        #region Private



        #endregion
    }
}
