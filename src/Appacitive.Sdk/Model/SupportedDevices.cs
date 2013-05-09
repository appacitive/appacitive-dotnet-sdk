using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk
{
    internal static class SupportedDevices
    {
        static SupportedDevices()
        {
            RegisterMapping("ios", DeviceType.iOS);
            RegisterMapping("android", DeviceType.Android);
            RegisterMapping("wp7", DeviceType.WindowsPhone7);
            RegisterMapping("wp75", DeviceType.WindowsPhone7_5);
            RegisterMapping("wp8", DeviceType.WindowsPhone8);
        }

        private static void RegisterMapping(string str, DeviceType type)
        {
            StringToDeviceTypeMapping[str] = type;
            DeviceToStringMapping[type] = str;
        }

        private static readonly Dictionary<string, DeviceType> StringToDeviceTypeMapping = new Dictionary<string, DeviceType>(StringComparer.OrdinalIgnoreCase);
        private static readonly Dictionary<DeviceType, string> DeviceToStringMapping = new Dictionary<DeviceType, string>();

        public static DeviceType ResolveDeviceType(string str)
        {
            DeviceType type;
            if (StringToDeviceTypeMapping.TryGetValue(str, out type) == true)
                return type;
            throw new ArgumentException(str + " is not a valid device type.");
        }

        public static string ResolveDeviceTypeString(DeviceType type)
        {
            // Ideally this should never fail.
            return DeviceToStringMapping[type];
        }
    }
}
