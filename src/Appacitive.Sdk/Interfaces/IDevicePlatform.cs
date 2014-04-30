using Appacitive.Sdk.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Appacitive.Sdk
{
    /// <summary>
    /// Represents the device based hosting platform runtime environment inside which the SDK is running.
    /// </summary>
    public interface IDevicePlatform : IApplicationPlatform
    {
        /// <summary>
        /// The device specific data used by the SDK.
        /// </summary>
        IDeviceState DeviceState { get; }
    }
}

