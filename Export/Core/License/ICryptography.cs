using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.License
{
    public interface ICryptography
    {
        void CreateFileLicenseInfo(string pathfile, string publickey);
        void CreateFileLicenseActive(string pathfileKeyinfo, string pathfileLicenseInfo, string pathLicenseActive);
        bool CheckActiveLicense(string pathKeyLicense, string key);
        void CreateFileLicenseTrial(Action<bool> firstTime);
    }
}
