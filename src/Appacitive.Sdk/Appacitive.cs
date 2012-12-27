using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Appacitive.Sdk.Services;

namespace Appacitive.Sdk
{
    public static class App
    {
        public static void Initialize(string apiKey, Environment environment, AppacitiveSettings settings = null)
        {
            RegisterDefaults();
            settings = settings ?? AppacitiveSettings.Default;
            // Set the api key
            AppacitiveContext.ApiKey = apiKey;
            // Set the environment
            AppacitiveContext.Environment = environment;
            // Set the factory
            var factory = settings.Factory ?? AppacitiveSettings.Default.Factory;
            AppacitiveContext.ObjectFactory = factory;
        }

        private static void RegisterDefaults()
        {
            InProcContainer.Instance
                            .Register<ISessionService, SessionService>( () => SessionService.Instance )
                            .Register<IArticleService, ArticleService>( () => new ArticleServiceWithTiming(ArticleService.Instance) )
                            .Register<IJsonSerializer, JsonDotNetSerializer>(() => new JsonDotNetSerializer());
        }
    }

    public class AppacitiveSettings
    {
        internal static readonly AppacitiveSettings Default = new AppacitiveSettings
        {
            // Register defaults
            Factory = InProcContainer.Instance
        };

        public IObjectFactory Factory { get; set; }
    }

    public static class AppacitiveContext
    {
        public static string ApiKey { get; set; }

        public static Environment Environment { get; set; }

        private static ReaderWriterLockSlim _sessionLock = new ReaderWriterLockSlim();
        private static string _sessionToken;
        public static string SessionToken
        {
            get
            {
                _sessionLock.EnterUpgradeableReadLock();
                try
                {
                    if (string.IsNullOrWhiteSpace(_sessionToken) == true)
                    {
                        _sessionLock.EnterWriteLock();
                        try
                        {
                            if (string.IsNullOrWhiteSpace(_sessionToken) == true)
                                CreateSession();
                        }
                        finally
                        {
                            _sessionLock.ExitWriteLock();
                        }
                    }
                    return _sessionToken;
                }
                finally
                {
                    _sessionLock.ExitUpgradeableReadLock();
                }
            }
        }

        public static string UserToken { get; set; }

        public static Geocode UserLocation { get; set; }

        public static IObjectFactory ObjectFactory { get; set; }

        internal static void MarkSessionInvalid()
        {
            _sessionToken = null;
        }

        private static object _syncroot = new object();

        private static void CreateSession()
        {
            //TODO: Add validation and failure handling
            var request = new CreateSessionRequest() { ApiKey = AppacitiveContext.ApiKey };
            var service = ObjectFactory.Build<ISessionService>();
            var response = service.CreateSession(request);
            _sessionToken = response.Session.SessionKey;
        }

        public static Verbosity Verbosity { get; set; }
    }

    public enum Environment
    {
        Sandbox,
        Live
    }

    public class Geocode
    {
        public decimal Latitude { get; set; }

        public decimal Longitude { get; set; }
    }

    public enum Verbosity
    {
        Info,
        Verbose
    }
}
