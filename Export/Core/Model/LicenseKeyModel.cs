using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Model
{
    [Serializable]
    public class LicenseKeyModel
    {
        /// <summary>
        /// LicenseKey
        /// </summary>
        public string LicenseKey { set; get; }
        /// <summary>
        /// HardwareInfo
        /// </summary>
        public string HardwareInfo { set; get; }


    }
}
