using Appacitive.Sdk.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if WINDOWS_PHONE7
using Appacitive.Sdk.WindowsPhone7;
using System.Threading;
#else
using Appacitive.Sdk.WindowsPhone8;
using Microsoft.Phone.Notification;
using System.Threading;
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

        public APDevice CurrentDevice
        {
            get 
            { 
                var device = _platform.DeviceState.GetDevice();
                if (device == null)
                    throw new AppacitiveRuntimeException("The current device is not initialized. Use AppContext.DeviceContext.RegisterCurrentDeviceAsync() to initialize the device.");
                return device;
            }
        }

        private int _registrationInProgress = 0;
        public async Task RegisterCurrentDeviceAsync()
        {
            if (Interlocked.CompareExchange(ref _registrationInProgress, 1, 0) != 0)
                throw new AppacitiveRuntimeException("Current device is already being initialized.");
            try
            {
                /// Get device from local storage. 
                /// Incase one does not exist then we set it up.
                var currentDevice = _platform.DeviceState.GetDevice();
                if (currentDevice != null) return;
                var deviceState = _platform.DeviceState;

                var instanceId = Guid.NewGuid().ToString();
                var device = new APDevice(deviceState.DeviceType);
                device.SetAttribute("instance-id", instanceId);
                device.DeviceToken = "-";
                await device.SaveAsync();
                deviceState.SetDevice(device);
            }
            finally
            {
                _registrationInProgress = 0;
            }
        }

        public IPushChannel Notifications
        {
            get 
            {
                var device = _platform.DeviceState.GetDevice();
                if (device == null)
                    throw new AppacitiveRuntimeException("The current device is not initialized. Use App.DeviceContext.RegisterCurrentDeviceAsync() to initialize the device.");
                return SingletonPushChannel.GetInstance(); 
            }
        }
    }


    
    
}
