using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Internal
{
    public class DeviceInfo
    {
        public DeviceInfo(Platform platform)
        {
            _platform = platform;
        }

        private Platform _platform;

        public APDevice Device
        {
            get { return _platform.ContextService.GetDevice(); }
        }

        #if (WINDOWS_PHONE7 || WINDOWS_PHONE8)
        
        #endif

    }
}
