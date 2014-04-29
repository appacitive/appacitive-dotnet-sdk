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
            InternalApp.Initialize(Platforms.NonWeb, appId, apikey, environment, settings);
        }

        public static void InitializeForWcf(string appId, string apikey, Environment environment, AppacitiveSettings settings = null)
        {
            InternalApp.Initialize(Platforms.Wcf, appId, apikey, environment, settings);
        }

        public static void InitializeForAspnet(string appId, string apikey, Environment environment, AppacitiveSettings settings = null)
        {
            InternalApp.Initialize(Platforms.Aspnet, appId, apikey, environment, settings);
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
    }
}
