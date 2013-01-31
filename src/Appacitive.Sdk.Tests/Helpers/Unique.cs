using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Tests
{
    internal static class Unique
    {

        public static string String
        {
            get
            {
                Guid guid = Guid.NewGuid();
                string str = Convert.ToBase64String(guid.ToByteArray());
                str = str.Replace("=", "");
                str = str.Replace("+", "");
                str = str.Replace("/", "");
                return str;
            }
        }
    }
}
