using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Appacitive.Sdk.Services;
using System.IO;
using Appacitive.Sdk.Internal;
using Appacitive.Sdk.Interfaces;

namespace Appacitive.Sdk.Internal
{
    internal static class InternalApp
    {
        private static readonly int NOT_INITIALIZED = 0;
        private static readonly int INITIALIZED = 1;
        private static int _isInitialized = NOT_INITIALIZED;
        public static void Initialize(IPlatform platform, string appId, string apikey, Environment environment, AppacitiveSettings settings = null)
        {
            // Setup container.
            // Setup current device.
            // Setup debugging.
            settings = settings ?? AppacitiveSettings.Default;
            if (Interlocked.CompareExchange(ref _isInitialized, INITIALIZED, NOT_INITIALIZED) == NOT_INITIALIZED)
                InitOnce(platform, appId, apikey, environment, settings);
        }

        private static void InitOnce(IPlatform platform, string appId, string apikey, Environment environment, AppacitiveSettings settings)
        {
            var context = new AppContextState(platform, apikey, environment, settings);
            // Register defaults
            DefaultRegistrations.ConfigureContainer(context.Container);
            // Setup platform specific registrations
            platform.Initialize(context);
            _context = context;
        }

        

        private static IAppContextState _context = null;
        public static IAppContextState Current
        {
            get
            {
                if (_context == null)
                    throw new AppacitiveRuntimeException("Appacitive runtime is not initialized.");
                else return _context;
            }
        }

        public static readonly TypeMapper Types = new TypeMapper();
        public static readonly Debugger Debug = new Debugger();

        public static async Task<UserSession> LoginAsync(Credentials credentials)
        {
            var userSession = await credentials.AuthenticateAsync();
            _context.CurrentUser.SetUser(userSession.LoggedInUser, userSession.UserToken);
            return userSession;
        }

        public static async Task LogoutAsync()
        {
            var userToken = _context.CurrentUser.SessionToken;
            if (string.IsNullOrWhiteSpace(userToken) == false)
                await UserSession.InvalidateAsync(userToken);
            _context.CurrentUser.Reset();
        }
    }
}
