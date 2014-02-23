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
    public class WPContextService : IContextService
    {
        public WPContextService(ILocalStorage storage, IJsonSerializer serializer)
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
            if (_localUser != null)
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

        private APDevice _localDevice = null;
        public APDevice GetDevice()
        {
            if (_localDevice != null)
                _localDevice = GetLocalDevice();
            return _localDevice;
        }

        private APDevice GetLocalDevice()
        {
            var json = this.LocalStorage.GetValue(NamingConvention.LocalDeviceKey());
            if (string.IsNullOrWhiteSpace(json) == true)
                return null;
            APDevice device = null;
            if (this.Serializer.TryDeserialize(json, out device) == true)
                return device;
            else return null;
        }

        public void SetDevice(APDevice device)
        {
            if (device == null)
            {
                this.LocalStorage.Remove(NamingConvention.LocalDeviceKey());
            }
            else
            {
                var bytes = this.Serializer.Serialize(device);
                var json = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
                this.LocalStorage.SetValue(NamingConvention.LocalDeviceKey(), json);
            }
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
