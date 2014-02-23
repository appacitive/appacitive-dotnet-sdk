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

namespace Appacitive.Sdk
{
    public static class App
    {
        private static readonly int NOT_INITIALIZED = 0;
        private static readonly int INITIALIZED = 1;
        private static int _isInitialized = NOT_INITIALIZED;
        public static void Initialize(Platform platform, string appId, string apikey, Environment environment, AppacitiveSettings settings = null)
        {
            // Setup container.
            // Setup current device.
            // Setup debugging.
            settings = settings ?? AppacitiveSettings.Default;
            if (Interlocked.CompareExchange(ref _isInitialized, INITIALIZED, NOT_INITIALIZED) == NOT_INITIALIZED)
                InitOnce(platform, appId, apikey, environment, settings);
        }

        private static void InitOnce(Platform platform, string appId, string apikey, Environment environment, AppacitiveSettings settings)
        {
            _context = new AppContext(platform, apikey, environment, settings);
            // Register defaults
            DefaultRegistrations.ConfigureContainer(_context.Container);
            // Setup platform specific registrations
            platform.InitializeContainer(_context.Container);
        }

        private static AppContext _context = null;
        public static AppContext Current
        {
            get
            {
                if (_context == null)
                    throw new AppacitiveRuntimeException("Appacitive runtime is not initialized.");
                else return _context;
            }
        }

        public static readonly Debugger Debug = new Debugger();

        public static async Task<UserSession> LoginAsync(Credentials credentials)
        {
            var userSession = await credentials.AuthenticateAsync();
            _context.CurrentUser.SetCurrentUser(userSession.LoggedInUser, userSession.UserToken);
            return userSession;
        }

        public static async Task LogoutAsync()
        {
            var userToken = _context.CurrentUser.UserToken;
            if (string.IsNullOrWhiteSpace(userToken) == false)
                await UserSession.InvalidateAsync(userToken);
            _context.CurrentUser.Reset();
        }
    }

}
