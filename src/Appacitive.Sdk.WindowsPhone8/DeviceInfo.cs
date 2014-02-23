using Appacitive.Sdk.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if WINDOWS_PHONE7
using Appacitive.Sdk.WindowsPhone7;
#else
using Appacitive.Sdk.WindowsPhone8;
#endif

namespace Appacitive.Sdk
{
    public class DeviceInfo
    {
        public DeviceInfo(IDevicePlatform platform)
        {
            _platform = platform;
        }

        private IDevicePlatform _platform;

        public APDevice Device
        {
            get { return _platform.DeviceState.GetDevice(); }
        }

        internal void Setup()
        {
            // If a device is not registered, then setup one.
            var state = _platform.DeviceState;
            var device = state.GetDevice();
            if (device == null)
            {
                var id = Guid.NewGuid().ToString();
                var newDevice = new APDevice(state.DeviceType);
                newDevice.SetAttribute("instance-id", id);
                newDevice.DeviceToken = "-";
                state.SetDevice(newDevice);
                newDevice.SaveAsync().ContinueWith(t =>
                {
                    var currentDevice = state.GetDevice();
                    // Ensure the same instance.
                    if (currentDevice.GetAttribute("instance-id") == id)
                    {
                        state.SetDevice(t.Result as APDevice);
                    }
                }, TaskContinuationOptions.OnlyOnRanToCompletion);
            }
        }


        public IPushChannel Channel
        {
            get
            {
                return SingletonPushChannel.GetInstance();
            }
        }

    }

    public static class DeviceInfoExtensions
    {
        public static DeviceInfo GetCurrentDevice(this AppContext context)
        {
            var devicePlatform = context.Platform as IDevicePlatform;
            if (devicePlatform != null)
                return new DeviceInfo(devicePlatform);
            else return null;
        }
    }
}
