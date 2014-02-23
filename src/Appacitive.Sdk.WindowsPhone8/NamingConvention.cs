using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if WINDOWS_PHONE
namespace Appacitive.Sdk.WindowsPhone8
#elif WINDOWS_PHONE7
namespace Appacitive.Sdk.WindowsPhone7
#endif
{
    internal class NamingConvention
    {
        internal static string LocalUserKey()
        {
            return "{162DEEE2-E3F2-43A3-838E-41F4886FD714}";
        }

        internal static string LocalUserTokenKey()
        {
            return "{ECF94223-F5BC-4350-BFB4-5594FDADB63F}";
        }

        internal static string LocalDeviceKey()
        {
            return "{828F3081-B914-4851-8158-7906533FFCC5}";
        }
    }
}
