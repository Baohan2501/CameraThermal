using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Model
{
    [Serializable]
    public class CustomerKeyInfoModel
    {
        /// <summary>
        /// Name company custommer
        /// </summary>
        public string Name { set; get; }

        /// <summary>
        /// Public /Private key
        /// </summary>
        public string Key { set; get; }
    }
}
