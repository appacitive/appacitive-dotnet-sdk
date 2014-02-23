using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Internal
{
    public class UserInfo
    {
        public UserInfo(Platform platform)
        {
            _platform = platform;
        }

        private Platform _platform;

        public APUser GetLoggedInUser()
        {
            return _platform.ContextService.GetUser();
        }

        internal void SetLoggedInUser(APUser user)
        {
            _platform.ContextService.SetUser(user);
        }


        public string UserToken
        {
            get
            {
                return _platform.ContextService.GetUserToken();
            }
            internal set
            {
                _platform.ContextService.SetUserToken(value);
            }
        }

        internal void Reset()
        {
            this.SetCurrentUser(null, null);
        }

        internal void SetCurrentUser(APUser user, string userToken)
        {
            this.SetLoggedInUser(user);
            this.UserToken = userToken;
        }
    }
}
