using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Internal
{
    public class AppContext
    {
        public AppContext(Platform platform, string apiKey, Environment environment, AppacitiveSettings settings)
        {
            this.ApiKey = apiKey;
            this.Environment = environment;
            this.CurrentDevice = new DeviceInfo(platform);
            this.CurrentUser = new UserInfo(platform);
            this.Settings = settings ?? AppacitiveSettings.Default;
            this.Container = settings.Factory ?? AppacitiveSettings.Default.Factory;
        }

        internal string ApiKey { get; set; }

        internal Environment Environment { get; set; }

        public DeviceInfo CurrentDevice { get; private set; }

        public UserInfo CurrentUser { get; private set; }

        public Platform Platform { get; set; }

        public AppacitiveSettings Settings { get; private set; }

        internal IDependencyContainer Container { get; private set; }

        public string AppId { get; set; }
    }
}
