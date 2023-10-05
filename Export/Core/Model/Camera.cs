using Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace Core.Model
{
    [Serializable]
    public class Camera:BinableBase
    {
        private int cameraid;
        public int CameraId
        {
            get
            {
                return cameraid;
            }
            set
            {
                cameraid = value;
                OnPropertyChanged("CameraId");
            }
        }

        private string cameraname;
        public string CameraName
        {
            get
            {
                return cameraname;
            }
            set
            {
                cameraname = value;
                OnPropertyChanged("CameraName");
            }
        }

        private string ipaddress;
        public string IPAddress
        {
            get
            {
                return ipaddress;
            }
            set
            {
                ipaddress = value;
                OnPropertyChanged("IPAddress");
            }
        }

        private string user;
        public string User
        {
            get
            {
                return user;
            }
            set
            {
                user = value;
                OnPropertyChanged("User");
            }
        }

        private string password;
        public string Password
        {
            get
            {
                return password;
            }
            set
            {
                password = value;
                OnPropertyChanged("Password");
            }
        }

        public string Model { get; set; }
    }
}
