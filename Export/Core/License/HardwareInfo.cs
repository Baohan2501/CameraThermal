using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Core.License
{
    public class HardwareInfo
    {
        [DllImport("kernel32.dll")]
        private static extern long GetVolumeInformation(
                                                        string PathName,
                                                        StringBuilder VolumeNameBuffer,
                                                        UInt32 VolumeNameSize,
                                                        ref UInt32 VolumeSerialNumber,
                                                        ref UInt32 MaximumComponentLength,
                                                        ref UInt32 FileSystemFlags,
                                                        StringBuilder FileSystemNameBuffer,
                                                        UInt32 FileSystemNameSize);

        #region "Get Info Hardware"
        private string GetProcessorId()
        {

            ManagementClass mc = new ManagementClass("win32_processor");
            ManagementObjectCollection moc = mc.GetInstances();
            String Id = String.Empty;
            foreach (ManagementObject mo in moc)
            {

                Id = mo.Properties["processorID"].Value.ToString();
                break;
            }
            return Id;

        }
        /// <summary>
        /// Retrieving HDD Serial No.
        /// </summary>
        /// <returns></returns>
        private string GetHDDSerialNo()
        {
            ManagementClass mangnmt = new ManagementClass("Win32_LogicalDisk");
            ManagementObjectCollection mcol = mangnmt.GetInstances();
            string result = "";
            foreach (ManagementObject strt in mcol)
            {
                result += Convert.ToString(strt["VolumeSerialNumber"]);
            }
            return result;
        }
        /// <summary>
        /// Retrieving System MAC Address.
        /// </summary>
        /// <returns></returns>
        private string GetMACAddress()
        {
            ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection moc = mc.GetInstances();
            string MACAddress = String.Empty;
            foreach (ManagementObject mo in moc)
            {
                if (MACAddress == String.Empty)
                {
                    if ((bool)mo["IPEnabled"] == true) MACAddress = mo["MacAddress"].ToString();
                }
                mo.Dispose();
            }

            MACAddress = MACAddress.Replace(":", "");
            return MACAddress;
        }

        public string GetHardwareInfo()
        {
            string key = GetProcessorId() + "_ _" + GetHDDSerialNo() + "_ _" + GetMACAddress();
            return key;
        }
        /// <summary>
        /// get serial number sdcard
        /// </summary>
        /// <param name="driverName">driver name (E:)</param>
        /// <returns></returns>
        public string GetSerialSDCard(string driverName)
        {
            uint serial_number = 0;
            uint max_component_length = 0;
            StringBuilder sb_volume_name = new StringBuilder(256);
            UInt32 file_system_flags = new UInt32();
            StringBuilder sb_file_system_name = new StringBuilder(256);

            if (GetVolumeInformation(driverName, sb_volume_name,
                (UInt32)sb_volume_name.Capacity, ref serial_number,
                ref max_component_length, ref file_system_flags,
                sb_file_system_name,
                (UInt32)sb_file_system_name.Capacity) > 0)
            {
                return serial_number.ToString();
            }
            return string.Empty;
        }


        #endregion
    }
}

