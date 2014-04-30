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
    /// Represents a desktop or server side hosting platform runtime environment inside which the SDK is running.
    /// </summary>
    public interface IApplicationPlatform : IPlatform
    {
        /// <summary>
        /// The application data used by the SDK.
        /// </summary>
        IApplicationState ApplicationState { get; }
    }
}

