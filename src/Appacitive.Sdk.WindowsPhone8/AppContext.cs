using Appacitive.Sdk.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk
{
    public static class AppContext
    {
        /// <summary>
        /// Initialize the Appacitive SDK with a customized platform implementation for windows phone app.
        /// </summary>
        /// <param name="platform">Custom device platform implementation</param>
        /// <param name="appId">App id for the app on appacitive.</param>
        /// <param name="apikey">API key for the app on appacitive.</param>
        /// <param name="environment">The environment to use - sandbox or live.</param>
        /// <param name="settings">Additional settings for initializing the SDK.</param>
        public static void Initialize(IDevicePlatform platform, string appId, string apikey, Environment environment, AppacitiveSettings settings = null)
        {
            InternalApp.Initialize(platform, appId, apikey, environment, settings);
        }

        /// <summary>
        /// Initialize the SDK for a windows phone app.
        /// </summary>
        /// <param name="appId">App id for the app on appacitive.</param>
        /// <param name="apikey">API key for the app on appacitive.</param>
        /// <param name="environment">The environment to use - sandbox or live.</param>
        /// <param name="settings">Additional settings for initializing the SDK.</param>
        public static void Initialize(string appId, string apikey, Environment environment, AppacitiveSettings settings = null)
        {
            #if WINDOWS_PHONE
            InternalApp.Initialize(Platforms.WP8, appId, apikey, environment, settings);
            #elif WINDOWS_PHONE7
            InternalApp.Initialize(Platforms.WP7, appId, apikey, environment, settings);
            #endif
        }

        /// <summary>
        /// The state of the Appacitive app context.
        /// </summary>
        public static IAppContextState State
        {
            get { return InternalApp.Current; }
        }

        /// <summary>
        /// Custom type mapping for subclasses of APObject, APUser and APConnection.
        /// </summary>
        public static TypeMapper Types
        {
            get { return InternalApp.Types; }
        }

        /// <summary>
        /// Debug and tracing setup for SDK.
        /// </summary>
        public static Debugger Debug
        {
            get { return InternalApp.Debug; }
        }

        /// <summary>
        /// Login a user inside an app.
        /// </summary>
        /// <param name="credentials">The login credentials for the user.</param>
        /// <returns>User session for the logged in user.</returns>
        public static async Task<UserSession> LoginAsync(Credentials credentials)
        {
            return await InternalApp.LoginAsync(credentials);
        }

        /// <summary>
        /// Logout the currently logged in user.
        /// </summary>
        /// <returns></returns>
        public static async Task LogoutAsync()
        {
            await InternalApp.LogoutAsync();
        }

        /// <summary>
        /// The current logged in user information.
        /// </summary>
        public static UserInfo UserContext
        {
            get
            {
                return InternalApp.Current.CurrentUser;
            }
        }

        /// <summary>
        /// The current device information.
        /// </summary>
        public static DeviceInfo DeviceContext
        {
            get 
            {
                var devicePlatform = AppContext.State.Platform as IDevicePlatform;
                if (devicePlatform != null)
                    return new DeviceInfo(devicePlatform);
                else 
                {
                    var platform = AppContext.State.Platform;
                    var name = platform == null ? "NULL" : platform.GetType().Name;
                    throw new AppacitiveRuntimeException(string.Format("The currently configured platform [{0}] is not a phone device platform.", name));
                    
                }
            }
        }


    }
}
