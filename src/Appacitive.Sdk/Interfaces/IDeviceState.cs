using Appacitive.Sdk.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Internal
{
    /// <summary>
    /// Represents the device specific state needed by the SDK.
    /// </summary>
    public interface IDeviceState
    {

        /// <summary>
        /// Gets the current device on which the app is running.
        /// </summary>
        /// <returns>The current device</returns>
        APDevice GetDevice();

        /// <summary>
        /// Sets the current device on which the app is running.
        /// </summary>
        /// <param name="device">APDevice object representing the current device.</param>
        void SetDevice(APDevice device);

        /// <summary>
        /// Gets the device type for the current device.
        /// </summary>
        DeviceType DeviceType { get; }

        /// <summary>
        /// Gets whether network connectivity is available.
        /// </summary>
        /// <returns>True or false.</returns>
        bool IsNetworkAvailable();

        /// <summary>
        /// Gets local storage handle for the current phone device.
        /// </summary>
        ILocalStorage LocalStorage { get; }
    }
}

