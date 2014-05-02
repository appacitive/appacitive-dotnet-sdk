using Appacitive.Sdk.Internal;
using System;
namespace Appacitive.Sdk
{
    public interface IAppContextState
    {
        /// <summary>
        /// The api key for the application.
        /// </summary>
        string ApiKey { get; }
        
        /// <summary>
        /// The app id for the application on Appacitive.
        /// </summary>
        string AppId { get; }
        
        /// <summary>
        /// The current logged in user.
        /// </summary>
        UserInfo CurrentUser { get; }
        
        /// <summary>
        /// The sandbox or live environment.
        /// </summary>
        Environment Environment { get; }

        /// <summary>
        /// The runtime platform in which the app is running. 
        /// </summary>
        IPlatform Platform { get; }
        
        /// <summary>
        /// Additional settings for the SDK.
        /// </summary>
        AppacitiveSettings Settings { get; }
        
        /// <summary>
        /// The dependency container instance used by the sdk.
        /// </summary>
        IDependencyContainer Container { get;  }
    }
}
