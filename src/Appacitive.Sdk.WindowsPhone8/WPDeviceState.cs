using Appacitive.Sdk.Internal;
using Microsoft.Phone.Net.NetworkInformation;
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
    public class WPDeviceState : IDeviceState
    {
        public WPDeviceState(ILocalStorage storage, IJsonSerializer serializer)
        {
            this.LocalStorage = storage;
            this.Serializer = serializer;
        }

        public ILocalStorage LocalStorage { get; private set; }
        public IJsonSerializer Serializer { get; private set; }

        private APDevice _localDevice = null;
        public APDevice GetDevice()
        {
            if (_localDevice != null)
                _localDevice = GetLocalDevice();
            return _localDevice;
        }

        private APDevice GetLocalDevice()
        {
            var json = this.LocalStorage.GetValue(NamingConvention.LocalDeviceKey());
            if (string.IsNullOrWhiteSpace(json) == true)
                return null;
            APDevice device = null;
            if (this.Serializer.TryDeserialize(json, out device) == true)
                return device;
            else return null;
        }

        public void SetDevice(APDevice device)
        {
            if (device == null)
            {
                this.LocalStorage.Remove(NamingConvention.LocalDeviceKey());
            }
            else
            {
                var bytes = this.Serializer.Serialize(device);
                var json = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
                this.LocalStorage.SetValue(NamingConvention.LocalDeviceKey(), json);
            }
        }

        public DeviceType DeviceType
        {
            get 
            {
                var version = System.Environment.OSVersion.Version;
                if (version.Major >= 8)
                    return Sdk.DeviceType.WindowsPhone8;
                else if (version.Major == 7 && version.Minor < 5)
                    return Sdk.DeviceType.WindowsPhone7;
                else if (version.Major == 7 && version.Minor == 5)
                    return Sdk.DeviceType.WindowsPhone7_5;
                else throw new AppacitiveRuntimeException("Unsupported OS version " + version.ToString());
                
                
            }
        }


        public bool IsNetworkAvailable()
        {
            return NetworkInterface.GetIsNetworkAvailable();
        }
    }
}
