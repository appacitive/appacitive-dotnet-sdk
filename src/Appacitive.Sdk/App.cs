using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Appacitive.Sdk.Services;

namespace Appacitive.Sdk
{
    public static class App
    {
        public static void Initialize(IApplicationHost host, string apiKey, Environment environment, AppacitiveSettings settings = null)
        {
            settings = settings ?? AppacitiveSettings.Default;
            // Set the api key
            AppacitiveContext.ApiKey = apiKey;
            // Set the environment
            AppacitiveContext.Environment = environment;
            // Set the factory
            AppacitiveContext.ObjectFactory = settings.Factory ?? AppacitiveSettings.Default.Factory;
            // Initialize host
            host.InitializeContainer(AppacitiveContext.ObjectFactory);
        }

        public static void SetLoggedInUser(string userToken)
        {
            AppacitiveContext.UserToken = userToken;
        }
    }

    public class AppacitiveSettings
    {
        internal static readonly AppacitiveSettings Default = new AppacitiveSettings
        {
            // Register defaults
            Factory = GetDefaultContainer()
        };

        public IDependencyContainer Factory { get; set; }

        private static IDependencyContainer GetDefaultContainer()
        {
            InProcContainer.Instance
                            .Register<IJsonSerializer, JsonDotNetSerializer>(() => new JsonDotNetSerializer())
                            .Register<IFileService, FileService>(() => FileService.Instance)
                            .Register<IConnectionService, ConnectionService>(() => ConnectionService.Instance)
                            .Register<ISessionService, SessionService>(() => SessionService.Instance)
                            .Register<IArticleService, ArticleService>(() => ArticleService.Instance)
                            .Register<IUserService, UserService>(() => UserService.Instance)
                            ;
            return InProcContainer.Instance;
        }
    }

    public static class AppacitiveContext
    {
        public static string ApiKey { get; set; }

        public static Environment Environment { get; set; }

        private static object _syncRoot = new object();
        private static string _sessionToken;

        public static string SessionToken
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_sessionToken) == true)
                {
                    lock (_syncroot)
                    {
                        if (string.IsNullOrWhiteSpace(_sessionToken) == true)
                        {
                            CreateSessionAsync().Wait();
                        }
                    }
                }
                return _sessionToken;
            }
        }

        public static string UserToken { get; internal set; }

        public static Geocode UserLocation { get; internal set; }

        public static IDependencyContainer ObjectFactory { get; internal set; }

        internal static void MarkSessionInvalid()
        {
            _sessionToken = null;
        }

        private static object _syncroot = new object();

        private static async Task CreateSessionAsync()
        {
            //TODO: Add validation and failure handling
            var request = new CreateSessionRequest() { ApiKey = AppacitiveContext.ApiKey };
            var service = ObjectFactory.Build<ISessionService>();
            var response = await service.CreateSessionAsync(request);
            _sessionToken = response.Session.SessionKey;
        }

        public static Verbosity Verbosity { get; set; }

        public static bool EnableDebugging { get; set; }
    }

    public enum Environment
    {
        Sandbox,
        Live
    }

    

    public enum Verbosity
    {
        Info,
        Verbose
    }
}
