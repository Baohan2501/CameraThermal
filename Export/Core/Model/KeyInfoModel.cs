using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Model
{
    [Serializable]
    public class KeyInfoModel
    {
        /// <summary>
        /// Name company custommer
        /// </summary>
        public string Name { set; get; }

        /// <summary>
        /// Public  key
        /// </summary>
        public string PublicKey { set; get; }

        /// <summary>
        /// Private key
        /// </summary>
        public string PrivateKey { set; get; }

        /// <summary>
        /// NumberDayTrial
        /// </summary>
        public int NumberTrialDay { set; get; }
        
    }
}
