using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Core.Model
{
    [Serializable]
    public class Success
    {
        public List<AlarmZone> GetZoneStatusSuccess { get; set; }
        public int CameraId { get; set; }

        public DateTime DateTime { get; set; }
    }

    [Serializable]
    public class TemperatureResponse
    {
        private string guiID = Guid.NewGuid().ToString();
        public string GuiID
        {
            get
            {
                return guiID;
            }
            set
            {
                guiID = value;
            }
        }
        public Success Success { get; set; }
    }
    [XmlRoot(ElementName = "ArrayOfCamera")]
    [Serializable]
    public class ArrayOfCamera
    {
        public List<SiteCamera> SiteCameras { get; set; }

        public string Server { get; set; }

        public string DBName { get; set; }

        public string User { get; set; }

        public string Password { get; set; }

        public int Interval { get; set; }
    }


    [XmlRoot(ElementName = "SiteCamera")]
    [Serializable]
    public class SiteCamera
    {

        [XmlElement(ElementName = "SiteName")]
        public string SiteName { get; set; }

        public List<CameraZone> CameraZones { get; set; }
    }
}
