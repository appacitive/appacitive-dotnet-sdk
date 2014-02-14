using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Internal
{
    public static class LocalSettings
    {
        public static string DeviceId 
        {
            get
            {
                var localStorage = ObjectFactory.Build<ILocalStorage>();
                return localStorage.GetValue("local.deviceid");
            }
            set
            {
                var localStorage = ObjectFactory.Build<ILocalStorage>();
                localStorage.SetValue("local.deviceid", value);
            }
        }


        public static Timezone LocalTimezone
        {
            get
            {
                TimeZoneInfo tz = TimeZoneInfo.Local;
                return Timezone.Create(tz.BaseUtcOffset.Hours, (uint)(tz.BaseUtcOffset.Minutes));
            }
        }


        
    }


}
