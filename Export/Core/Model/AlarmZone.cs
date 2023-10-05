using Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Core.Model
{
    [Serializable]
    public class AlarmZone : BinableBase
    {
        private int zoneId;
        public int ZoneId
        {
            get
            {
                return zoneId;
            }
            set
            {
                zoneId = value;
                OnPropertyChanged("ZoneId");
            }
        }
        private string zoneName;
        public string ZoneName
        {
            get { return zoneName; }
            set
            {
                zoneName = value; 
                OnPropertyChanged("ZoneName");
            }
        }
       
        private double averageTemperature;
        public double AverageTemperature
        {
            get
            {
                return averageTemperature;
            }
            set
            {
                averageTemperature = value;
                OnPropertyChanged("AverageTemperature");
            }
        }
        private double minimumTemperature;
        public double MinimumTemperature {
            get
            {
                return minimumTemperature;
            }
            set
            {
                minimumTemperature = value;
                OnPropertyChanged("MinimumTemperature");
            }
        }
        private double maximumTemperature;
        public double MaximumTemperature
        {
            get
            {
                return maximumTemperature;
            }
            set
            {
                maximumTemperature = value;
                OnPropertyChanged("MaximumTemperature");
            }
        }
     

        private bool isAlarm=false;
        public bool IsAlarm
        {
            get
            {
                return isAlarm;
            }
            set
            {
                isAlarm = value;
                OnPropertyChanged("IsAlarm");
            }
        }
    }

    
    [Serializable]
    public class AlarmZoneSetting : BinableBase
    {
        private int zoneId;
        public int ZoneId
        {
            get
            {
                return zoneId;
            }
            set
            {
                zoneId = value;
                OnPropertyChanged("ZoneId");
            }
        }
        private string zoneName;
        public string ZoneName
        {
            get { return zoneName; }
            set
            {
                zoneName = value;
                OnPropertyChanged("ZoneName");
            }
        }
        private double maxAlarmTemperature;
        public double MaxAlarmTemperature
        {
            get { return maxAlarmTemperature; }
            set
            {
                maxAlarmTemperature = value;
                OnPropertyChanged("MaxAlarmTemperature");
            }
        }

        private bool isSelected = true;
        public bool IsSelected
        {
            get
            {
                return isSelected;
            }
            set
            {
                if (isSelected !=value)
                {
                    isSelected = value;
                    OnPropertyChanged("IsSelected");
                }    
            }
        }
    }
}
