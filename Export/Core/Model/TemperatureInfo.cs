using Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Model
{
    public class TemperatureInfo : BinableBase
    {
        private DateTime dateTime;
        public DateTime DateTime
        {
            get
            {
                return dateTime;
            }
            set
            {
                dateTime = value;
                OnPropertyChanged("DateTime");
            }
        }

        private string cameraName;
        public string CameraName
        {
            get
            {
                return cameraName;
            }
            set
            {
                cameraName = value;
                OnPropertyChanged("CameraName");
            }
        }

        private string zoneName;
        public string ZoneName
        {
            get
            {
                return zoneName;
            }
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

        private double minimumTemperature;
        public double MinimumTemperature
        {
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

        private int cameraId;
        public int CameraId
        {
            get
            {
                return cameraId;
            }
            set
            {
                cameraId = value;
                OnPropertyChanged("CameraId");
            }
        }

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
    }
}
