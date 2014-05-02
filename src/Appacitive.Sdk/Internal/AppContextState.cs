using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Internal
{
    internal class AppContextState : IAppContextState
    {
        public AppContextState(IPlatform platform, string apiKey, Environment environment, AppacitiveSettings settings)
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

        public UserInfo CurrentUser
        {
            get { return _user; }
        }

        public IPlatform Platform { get; private set; }

        public AppacitiveSettings Settings { get; private set; }

        public IDependencyContainer Container { get; private set; }

        public string AppId { get; private set; }
    }
}
