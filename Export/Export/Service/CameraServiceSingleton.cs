using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Export.Service
{
    public class CameraServiceSingleton
    {
        private static CameraService.ServiceClient instance;

        private CameraServiceSingleton()
        {

        }
        public static CameraService.ServiceClient Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new CameraService.ServiceClient();
                }
                return instance;
            }
        }

        public static void Reset()
        {
            instance = new CameraService.ServiceClient();
        }
    }
}
