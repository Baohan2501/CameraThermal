using Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Model
{
    public class Zone:BinableBase
    {
        private int zoneid;
        public int ZoneId
        {
            get
            {
                return zoneid;
            }
            set
            {
                zoneid = value;
                OnPropertyChanged("ZoneId");
            }
        }

        private string zonename;
        public string ZoneName
        {
            get
            {
                return zonename;
            }
            set
            {
                zonename = value;
                OnPropertyChanged("ZoneName");
            }
        }

        private double maxalarmtemperature;
        public double MaxAlarmTemperature
        {
            get
            {
                return maxalarmtemperature;
            }
            set
            {
                maxalarmtemperature = value;
                OnPropertyChanged("MaxAlarmTemperature");
            }
        }

        private double minimumtemperature;
        public double MinimumTemperature
        {
            get
            {
                return minimumtemperature;
            }
            set
            {
                minimumtemperature = value;
                OnPropertyChanged("MinimumTemperature");
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

       

        private bool isSelected = false;
        public bool IsSelcted
        {
            get
            {
                return isSelected;
            }
            set
            {
                isSelected = value;
                OnPropertyChanged("IsSelcted");
            }
        }
    }
}
