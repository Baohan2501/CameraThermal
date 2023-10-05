using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Model
{
    [Serializable]
    public class LicenseKeyTrialModel : LicenseKeyModel
    {
        /// <summary>
        /// TrialDay
        /// </summary>
        public int TrialDay { set; get; }
        /// <summary>
        /// StartDate
        /// </summary>
        public DateTime StartDate { set; get; }

        /// <summary>
        /// EndDate
        /// </summary>
        public DateTime EndDate { set; get; }
    }
}
