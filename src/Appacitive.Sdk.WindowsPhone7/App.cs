using Appacitive.Sdk.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk
{
    public static class App
    {
        public static void Initialize(IPlatform platform, string appId, string apikey, Environment environment, AppacitiveSettings settings = null)
        {
            InternalApp.Initialize(platform, appId, apikey, environment, settings);
        }
        public static void Initialize(string appId, string apikey, Environment environment, AppacitiveSettings settings = null)
        {
            InternalApp.Initialize(Platforms.WP7 , appId, apikey, environment, settings);
        }

        public static AppContext Context
        {
            get { return InternalApp.Current; }
        }

        public static TypeMapper Types
        {
            get { return InternalApp.Types; }
        }

        public static Debugger Debug
        {
            get { return InternalApp.Debug; }
        }

        public static async Task<UserSession> LoginAsync(Credentials credentials)
        {
            return await InternalApp.LoginAsync(credentials);
        }

        public static async Task LogoutAsync()
        {
            await InternalApp.LogoutAsync();
        }

        public static UserInfo UserContext
        {
            get
            {
                return InternalApp.Current.CurrentUser;
            }
        }

        public static DeviceInfo DeviceContext
        {
            get
            {
                var devicePlatform = App.Context.Platform as IDevicePlatform;
                if (devicePlatform != null)
                    return new DeviceInfo(devicePlatform);
                else
                {
                    var platform = App.Context.Platform;
                    var name = platform == null ? "NULL" : platform.GetType().Name;
                    throw new AppacitiveRuntimeException(string.Format("The currently configured platform [{0}] is not a phone device platform.", name));

                }
            }
        }
    }
}
