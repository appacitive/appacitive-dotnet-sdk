using Appacitive.Sdk.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Appacitive.Sdk
{
    public interface IDevicePlatform : IApplicationPlatform
    {
        IDeviceState DeviceState { get; }
    }
}

