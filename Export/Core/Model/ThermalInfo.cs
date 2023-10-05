using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Core.Model
{
    public class ThermalInfo
    {
        private DateTime inspectionDate;
        public DateTime InspectionDate
        {
            get
            {
                return inspectionDate;
            }
            set
            {
                inspectionDate = value;
            }
        }

        private string equipment;
        public string Equipment
        {
            get
            {
                return equipment;
            }
            set
            {
                equipment = value;
            }
        }

        private string potentialProblem;
        public string PotentialProblem
        {
            get
            {
                return potentialProblem;
            }
            set
            {
                potentialProblem = value;
            }
        }
        private double emissivity;
        public double Emissivity
        {
            get
            {
                return emissivity;
            }
            set
            {
                emissivity = value;
            }
        }

        private string cameraManufacturer;
        public string CameraManufacturer
        {
            get
            {
                return cameraManufacturer;
            }
            set
            {
                cameraManufacturer = value;
            }
        }

        private double hotImageMarker;
        public double HotImageMarker
        {
            get
            {
                return hotImageMarker;
            }
            set
            {
                hotImageMarker = value;
            }
        }

        private string location;
        public string Location
        {
            get
            {
                return location;
            }
            set
            {
                location = value;
            }
        }

        private string equipmentName;
        public string EquipmentName
        {
            get
            {
                return equipmentName;
            }
            set
            {
                equipmentName = value;
            }
        }

        private double depairPriority;
        public double RepairPriority
        {
            get
            {
                return depairPriority;
            }
            set
            {
                depairPriority = value;
            }
        }

        private double reflectedTemperature;
        public double ReflectedTemperature
        {
            get
            {
                return reflectedTemperature;
            }
            set
            {
                reflectedTemperature = value;
            }
        }

        private string camera;
        public string Camera
        {
            get
            {
                return camera;
            }
            set
            {
                camera = value;
            }
        }

        private double coldImageMarker;
        public double ColdImageMarker
        {
            get
            {
                return coldImageMarker;
            }
            set
            {
                coldImageMarker = value;
            }
        }

        private System.Drawing.Image imageData;
        public System.Drawing.Image ImageData
        {
            get
            {
                return imageData;
            }
            set
            {
                imageData = value;
            }
        }
    }
}

