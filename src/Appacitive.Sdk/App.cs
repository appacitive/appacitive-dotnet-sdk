using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Appacitive.Sdk.Services;
using Appacitive.Sdk.Realtime;
using System.IO;
using Appacitive.Sdk.Internal;

namespace Appacitive.Sdk
{
    public static class App
    {
        private static int _initialized = 0;
        private static object _syncRoot = new object();
        public static void Initialize(IApplicationHost host, string apiKey, Environment environment, AppacitiveSettings settings = null)
        {
            if (Interlocked.CompareExchange(ref _initialized, 1, 0) == 0)
            {
                settings = settings ?? AppacitiveSettings.Default;
                // Set the api key
                AppacitiveContext.ApiKey = App.Apikey = apiKey;
                // Set the environment
                AppacitiveContext.Environment = App.Environment = environment;
                // Use api session
                AppacitiveContext.UseApiSession = settings.UseApiSession;
                // Set the factory
                AppacitiveContext.ObjectFactory = settings.Factory ?? AppacitiveSettings.Default.Factory;
                // Register defaults
                RegisterDefaults(AppacitiveContext.ObjectFactory);
                // Initialize host
                host.InitializeContainer(AppacitiveContext.ObjectFactory);
                // Setup real time connections
                App.EnableRealtime = settings.EnableRealTimeSupport;
                if (settings.EnableRealTimeSupport == true)
                    StartRealTime().Wait();
                
            }
        }

        private static int _isShutdown = 0;
        public static void Shutdown()
        {
            // Clean up app resources
            // Shutdown any real time connections
            if (Interlocked.CompareExchange(ref _isShutdown, 1, 0) == 0)
            {
                if (App.Channel != null)
                {
                    App.Channel.Stop();
                    App.Channel = null;
                }
            }
        }

        private static IRealTimeChannel Channel { get; set; }

        public static IDependencyContainer Factory
        {
            get
            {
                return AppacitiveContext.ObjectFactory;
            }
        }

        public static bool EnableRealtime {get; set;}

        internal static async Task StartRealTime()
        {
            if (App.Channel != null)
                return;
            // Setup the real time channel
            var factory = ObjectFactory.Build<IRealTimeChannelFactory>();
            var channel = factory.CreateChannel();
            // Setup downstream messages
            channel.Receive += m =>
                {
                    var msg = m as IDownstreamMessage;
                    if (msg == null) return;
                    var topics = msg.GetTopics();
                    var subManager = ObjectFactory.Build<ISubscriptionManager>();
                    foreach (var topic in topics)
                    {
                        var sub = subManager.Get(topic);
                        if (sub != null)
                            sub.Notify(m);
                    }
                };
            await channel.Start();
            App.Channel = channel;
        }

        private static void RegisterDefaults(IDependencyContainer dependencyContainer)
        {
            dependencyContainer
                    .Register<ISubscriptionManager, SingletonSubscriptionManager>( () => SingletonSubscriptionManager.Instance )
                    .Register<IRealTimeChannelFactory, RealTimeChannelFactory>( () => new RealTimeChannelFactory() )
                    .Register<IRealTimeChannel, RealTimeChannel>(  () => new RealTimeChannel() )
                    .Register<IUserContext, StaticUserContext>(() => new StaticUserContext())
                    .Register<IJsonSerializer, JsonDotNetSerializer>(() => new JsonDotNetSerializer())
                    .Register<IExceptionFactory, ServiceExceptionFactory>( () => ServiceExceptionFactory.Instance )
                    ;
        }

        public static async Task<UserSession> LoginAsync(Credentials credentials)
        {
            var session = await credentials.AuthenticateAsync();
            App.UserToken = session.UserToken;
            return session;
        }

        public static async Task Logout()
        {
            var token = App.UserToken;
            if (string.IsNullOrWhiteSpace(App.UserToken) == true) return;
            await UserSession.InvalidateAsync(token);
            App.UserToken = null;
        }

        public static bool IsAuthenticated
        {
            get
            {
                return string.IsNullOrWhiteSpace(App.UserToken) == false;
            }
        }

        internal static async Task SendMessageAsync(RealTimeMessage msg)
        {
            if (App.EnableRealtime == false)
                throw new Exception("Real time support has not been enabled on the SDK. Initialize the application with AppacitiveSettings.EnableRealTimeSupport as true.");
            if (App.Channel == null)
                throw new Exception("Real time infrastucture not initialized. Make sure that App has been initialized.");
            await App.Channel.SendAsync(msg);

        }

        public static string UserToken
        {
            get
            {
                var userContext = ObjectFactory.Build<IUserContext>();
                return userContext.GetUserToken();
            }
            set
            {
                var userContext = ObjectFactory.Build<IUserContext>();
                userContext.SetUserToken(value);
            }
        }

        public static string Apikey { get; private set; }

        public static Environment Environment { get; private set; }

        public static class Debug
        {
            public static bool IsEnabled = false;

            public static TextWriter Out { get; set; }
        }
    }


    public static class Debugger
    {
        public static async Task Log(string data)
        {
            try
            {
                if (App.Debug.IsEnabled == false)
                    return;
                var tw = App.Debug.Out;
                if (tw != null)
                    await tw.WriteLineAsync(data);
                await WriteDelimiter(tw);
                await tw.FlushAsync();
            }
            catch { }
        }

        public static async Task Log(byte[] bytes)
        {
            try
            {
                if (App.Debug.IsEnabled == false)
                    return;
                var tw = App.Debug.Out;
                if (tw != null)
                    await tw.WriteLineAsync(Encoding.UTF8.GetString(bytes, 0, bytes.Length));
                await WriteDelimiter(tw);
                await tw.FlushAsync();
            }
            catch { }
        }

        private async static Task WriteDelimiter(TextWriter writer)
        {
            await writer.WriteLineAsync();
            await writer.WriteLineAsync();
        }
    }
    
    public class AppacitiveSettings
    {
        internal static readonly AppacitiveSettings Default = new AppacitiveSettings();

        public AppacitiveSettings()
        {
            this.Factory = GetDefaultContainer();
            this.UseApiSession = false;
            this.EnableRealTimeSupport = false;
        }

        public IDependencyContainer Factory { get; set; }

        public bool UseApiSession { get; set; }

        public bool EnableRealTimeSupport { get; set; }

        private static IDependencyContainer GetDefaultContainer()
        {
            return InProcContainer.Instance;
        }
    }

    

}
