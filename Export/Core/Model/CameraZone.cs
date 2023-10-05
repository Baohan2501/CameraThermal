using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Core.Model
{
    [Serializable]
    public class CameraZone : Camera
    {
        [field: NonSerialized]
        public event EventHandler ZoneChange;
        public CameraZone()
        {
            Zones.CollectionChanged += Zones_CollectionChanged;
        }

        private void Zones_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (INotifyPropertyChanged item in e.NewItems)
                {
                    item.PropertyChanged += CameraZone_PropertyChanged;
                }
            }
            if (e.OldItems != null)
            {
                foreach (INotifyPropertyChanged item in e.OldItems)
                {
                    item.PropertyChanged -= CameraZone_PropertyChanged;
                }
            }
        }

        private void CameraZone_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsSelected")
            {
                if (ZoneChange != null)
                    ZoneChange(null, null);
            }
        }

        private ObservableCollection<AlarmZoneSetting> zones = new ObservableCollection<AlarmZoneSetting>();
        public ObservableCollection<AlarmZoneSetting> Zones
        {
            get
            {
                return zones;
            }
            set
            {
                zones = value;
                OnPropertyChanged("Zones");
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
                isSelected = value;
                OnPropertyChanged("IsSelected");
            }
        }
    }
}
