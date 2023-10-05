using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.License
{
    public class LicenseManagement : ILicenseManagement
    {
        string currentPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
        Serializition file = new Serializition();
        HardwareInfo hware = new HardwareInfo();
        ICryptography cryptography = CryptographyFactory.CreateCryptography("RSA");
        public ResultAction CheckActiveLicense()
        {
            if (File.Exists(currentPath + "//License.dll"))
            {
                var licenseInfo = file.ReadFromBinaryFile<object>(currentPath + "//License.dll");
                //if license key trial
                if (licenseInfo != null && licenseInfo is Model.LicenseKeyTrialModel)
                {
                    if (
                        ((Model.LicenseKeyTrialModel)licenseInfo).HardwareInfo != null
                        && ((Model.LicenseKeyTrialModel)licenseInfo).HardwareInfo.Equals(hware.GetHardwareInfo())
                        )
                    {
                        ((Model.LicenseKeyTrialModel)licenseInfo).StartDate = ((Model.LicenseKeyTrialModel)licenseInfo).StartDate < DateTime.Now ? DateTime.Now : ((Model.LicenseKeyTrialModel)licenseInfo).StartDate;
                        //expired date trial
                        if (((Model.LicenseKeyTrialModel)licenseInfo).EndDate.Subtract(((Model.LicenseKeyTrialModel)licenseInfo).StartDate).TotalMinutes > 0)
                        {
                            return ResultAction.License;
                        }
                        else
                        {
                            return ResultAction.UnActive;
                        }
                       
                    }
                    else
                    {
                        return ResultAction.Trial;
                    }
                }
                else if (licenseInfo != null && licenseInfo is Model.LicenseKeyModel)
                {
                    //Check license key of this pc
                    if (((Model.LicenseKeyModel)licenseInfo).HardwareInfo != null && ((Model.LicenseKeyModel)licenseInfo).HardwareInfo.Equals(hware.GetHardwareInfo()))
                    {
                        //action.Invoke(true, ((Model.LicenseKeyModel)licenseInfo).IsCheckExportFile, ((Model.LicenseKeyModel)licenseInfo).IsCheckFlipper);//license trial is  expire
                        return ResultAction.License;
                    }
                    
                }
            }
            return ResultAction.Trial;
        }
    }

    public enum ResultAction
    {
        Trial,
        License,
        UnActive
    }
}
