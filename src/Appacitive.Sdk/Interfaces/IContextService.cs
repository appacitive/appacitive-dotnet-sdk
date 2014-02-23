using Appacitive.Sdk.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Internal
{
    public interface IApplicationPlatform
    {
        IApplicationState ApplicationState { get; }
    }

    public interface IDevicePlatform : IApplicationPlatform
    {
        IDeviceState DeviceState { get; }
    }

    public interface IApplicationState
    {
        APUser GetUser();

        string GetUserToken();

        void SetUser(APUser user);

        void SetUserToken(string value);
    }

    public interface IDeviceState
    {
        APDevice GetDevice();

        void SetDevice(APDevice device);

        DeviceType DeviceType { get; }

        bool IsNetworkAvailable();

        ILocalStorage LocalStorage { get; }
    }
}

