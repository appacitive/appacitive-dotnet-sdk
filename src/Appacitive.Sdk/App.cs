using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Appacitive.Sdk.Services;
using Appacitive.Sdk.Interfaces;

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
            // Use api session
            AppacitiveContext.UseApiSession = settings.UseApiSession;
            // Set the factory
            AppacitiveContext.ObjectFactory = settings.Factory ?? AppacitiveSettings.Default.Factory;
            // Register defaults
            RegisterDefaults(AppacitiveContext.ObjectFactory);
            // Initialize host
            host.InitializeContainer(AppacitiveContext.ObjectFactory);
        }

        public static IDependencyContainer Factory
        {
            get
            {
                return AppacitiveContext.ObjectFactory;
            }
        }

        private static void RegisterDefaults(IDependencyContainer dependencyContainer)
        {
            dependencyContainer
                            .Register<IUserContext, StaticUserContext>(() => new StaticUserContext())
                            .Register<IJsonSerializer, JsonDotNetSerializer>(() => new JsonDotNetSerializer())
                            .Register<IFileService, FileService>(() => FileService.Instance)
                            .Register<IConnectionService, ConnectionService>(() => ConnectionService.Instance)
                            .Register<ISessionService, SessionService>(() => SessionService.Instance)
                            .Register<IArticleService, ArticleService>(() => ArticleService.Instance)
                            .Register<IUserService, UserService>(() => UserService.Instance)
                            .Register<IDeviceService, DeviceService>( () => DeviceService.Instance)
                            .Register<IExceptionFactory, ServiceExceptionFactory>( () => ServiceExceptionFactory.Instance )
                            ;
        }

        public static string GetLoggedInUser()
        {
            var userContext = ObjectFactory.Build<IUserContext>();
            return userContext.GetUserToken();
        }

        public static void SetLoggedInUser(string userToken)
        {
            var userContext = ObjectFactory.Build<IUserContext>();
            userContext.SetUserToken(userToken);
        }
    }

    
    public class AppacitiveSettings
    {
        internal static readonly AppacitiveSettings Default = new AppacitiveSettings
        {
            // Register defaults
            Factory = GetDefaultContainer(),
            UseApiSession = false
        };

        public IDependencyContainer Factory { get; set; }

        public bool UseApiSession { get; set; }

        private static IDependencyContainer GetDefaultContainer()
        {
            return InProcContainer.Instance;
        }
    }

    

}
