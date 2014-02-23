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
            _user = new UserInfo(platform as IApplicationPlatform);
            this.Settings = settings ?? AppacitiveSettings.Default;
            this.Container = settings.Factory ?? AppacitiveSettings.Default.Factory;
        }

        public string ApiKey { get; private set; }

        public Environment Environment { get; private set; }

        private UserInfo _user;

        public UserInfo GetCurrentUser()
        {
            return _user;
        }

        public Platform Platform { get; private set; }

        public AppacitiveSettings Settings { get; private set; }

        internal IDependencyContainer Container { get; private set; }

        public string AppId { get; private set; }
    }
}
