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
        /// Initialize the Appacitive SDK with a customized platform implementation for server or desktop.
        /// </summary>
        /// <param name="platform">Custom platform implementation</param>
        /// <param name="appId">App id for the app on appacitive.</param>
        /// <param name="apikey">API key for the app on appacitive.</param>
        /// <param name="environment">The environment to use - sandbox or live.</param>
        /// <param name="settings">Additional settings for initializing the SDK.</param>
        public static void Initialize(IApplicationPlatform platform, string appId, string apikey, Environment environment, AppacitiveSettings settings = null)
        {
            InternalApp.Initialize(platform, appId, apikey, environment, settings);
        }

        /// <summary>
        /// Initialize the SDK for desktop or server hosted application.
        /// </summary>
        /// <param name="appId">App id for the app on appacitive.</param>
        /// <param name="apikey">API key for the app on appacitive.</param>
        /// <param name="environment">The environment to use - sandbox or live.</param>
        /// <param name="settings">Additional settings for initializing the SDK.</param>
        public static void Initialize(string appId, string apikey, Environment environment, AppacitiveSettings settings = null)
        {
            InternalApp.Initialize(Platforms.NonWeb, appId, apikey, environment, settings);
        }

        /// <summary>
        /// Initialize the SDK for running inside a WCF application.
        /// </summary>
        /// <param name="appId">App id for the app on appacitive.</param>
        /// <param name="apikey">API key for the app on appacitive.</param>
        /// <param name="environment">The environment to use - sandbox or live.</param>
        /// <param name="settings">Additional settings for initializing the SDK.</param>
        public static void InitializeForWcf(string appId, string apikey, Environment environment, AppacitiveSettings settings = null)
        {
            InternalApp.Initialize(Platforms.Wcf, appId, apikey, environment, settings);
        }

        /// <summary>
        /// Initialize the SDK for use inside an ASP.net application.
        /// </summary>
        /// <param name="appId">App id for the app on appacitive.</param>
        /// <param name="apikey">API key for the app on appacitive.</param>
        /// <param name="environment">The environment to use - sandbox or live.</param>
        /// <param name="settings">Additional settings for initializing the SDK.</param>
        public static void InitializeForAspnet(string appId, string apikey, Environment environment, AppacitiveSettings settings = null)
        {
            InternalApp.Initialize(Platforms.Aspnet, appId, apikey, environment, settings);
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
        /// Check if the current user session is still valid.
        /// </summary>
        /// <returns>True or false.</returns>
        public static async Task<bool> IsUserLoggedInAsync()
        {
            if (AppContext.UserContext == null) 
                return false;
            var sessionToken = AppContext.UserContext.SessionToken;
            if (string.IsNullOrWhiteSpace(sessionToken) == true)
                return false;
            return await UserSession.IsValidAsync(sessionToken);
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
    }
}
