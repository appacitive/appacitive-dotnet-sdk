using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Internal
{
    public class UserInfo
    {
        public UserInfo(IApplicationPlatform platform)
        {
            _platform = platform;
        }

        private IApplicationPlatform _platform;

        public APUser LoggedInUser
        {
            get
            {
                return _platform.ApplicationState.GetUser();
            }
            set
            {
                _platform.ApplicationState.SetUser(value);
            }
        }

        public string SessionToken
        {
            get
            {
                return _platform.ApplicationState.GetUserToken();
            }
            internal set
            {
                _platform.ApplicationState.SetUserToken(value);
            }
        }

        internal void Reset()
        {
            this.SetUser(null, null);
        }

        internal void SetUser(APUser user, string userToken)
        {
            this.LoggedInUser = user;
            this.SessionToken = userToken;
        }

        public bool IsLoggedIn
        {
            get
            {
                return this.LoggedInUser != null;
            }
        }
    }
}
