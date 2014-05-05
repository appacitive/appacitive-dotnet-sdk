using Appacitive.Sdk.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if WINDOWS_PHONE
namespace Appacitive.Sdk.WindowsPhone8
#elif WINDOWS_PHONE7
namespace Appacitive.Sdk.WindowsPhone7
#endif
{
    public class WPApplicationState : IApplicationState
    {
        public WPApplicationState(ILocalStorage storage, IJsonSerializer serializer)
        {
            this.LocalStorage = storage;
            this.Serializer = serializer;
        }

        public ILocalStorage LocalStorage { get; private set; }
        public IJsonSerializer Serializer { get; private set; }

        private APUser _localUser = null;
        public APUser GetUser()
        {
            /*
             If local user is not available, check local store and get from there.
             If not available in local store then return null.
             */
            if (_localUser == null)
                _localUser = GetLocalUser();
            return _localUser;
        }

        private APUser GetLocalUser()
        {
            var userJson = this.LocalStorage.GetValue(NamingConvention.LocalUserKey());
            if (string.IsNullOrWhiteSpace(userJson) == true)
                return null;
            APUser user = null;
            if (this.Serializer.TryDeserialize(userJson, out user) == true)
                return user;
            else return null;
        }


        private string _userToken = null;
        public string GetUserToken()
        {
            if (_userToken == null)
                _userToken = GetLocalUserToken();
            return _userToken;
        }

        private string GetLocalUserToken()
        {
            return this.LocalStorage.GetValue(NamingConvention.LocalUserTokenKey());
        }

        
        public void SetUser(APUser user)
        {
            if (user == null)
            {
                this.LocalStorage.Remove(NamingConvention.LocalUserKey());
            }
            else
            {
                var bytes = this.Serializer.Serialize(user);
                var json = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
                this.LocalStorage.SetValue(NamingConvention.LocalUserKey(), json);
            }
            _localUser = user;
        }

        public void SetUserToken(string value)
        {
            if (string.IsNullOrWhiteSpace(value) == true )
            {
                this.LocalStorage.Remove(NamingConvention.LocalUserTokenKey());
            }
            else
            {
                this.LocalStorage.SetValue(NamingConvention.LocalUserTokenKey(), value);
            }
        }
    }
}
