using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.License
{
    public class CryptographyFactory
    {
        public static ICryptography CreateCryptography(string type)
        {
            switch (type)
            {
                case "RSA":
                    return new RSACryptography();
                    break;
                default:
                    return new RSACryptography();
                    break;
            }
        }
    }
}
