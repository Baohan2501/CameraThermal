using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.License
{
    public interface ILicenseManagement
    {
        ResultAction CheckActiveLicense();
    }
}
