using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk
{
    public sealed class UserInfo
    {

        private EventHandler<UserLoggedInEventArgs> _userLoggedIn;
        private EventHandler _userLoggedOut;
        private readonly object _mutex = new object();

        /// <summary>
        /// Event raised whenever a user logs into the app.
        /// </summary>
        public event EventHandler<UserLoggedInEventArgs> UserLoggedIn
        {
            add
            {
                lock (_mutex)
                {
                    _userLoggedIn += value;
                }
            }
            remove
            {
                lock (_mutex)
                {
                    _userLoggedIn -= value;
                }
            }
        }

        /// <summary>
        /// Event raised when a user logs out of the app.
        /// </summary>
        public event EventHandler UserLoggedOut
        {
            add
            {
                lock (_mutex)
                {
                    _userLoggedOut += value;
                }
            }
            remove
            {
                lock (_mutex)
                {
                    _userLoggedOut -= value;
                }
            }
        }

        private void OnUserLoggedIn(APUser user, string session)
        {
            var copy = _userLoggedIn;
            if (copy != null)
                copy(this, new UserLoggedInEventArgs(user, session));
        }

        private void OnUserLoggedOut()
        {
            var copy = _userLoggedOut;
            if (copy != null)
                copy(this, EventArgs.Empty);
        }

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
            private set
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
            private set
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
            if (user == null)
                OnUserLoggedOut();
            else
                OnUserLoggedIn(user, userToken);
        }

        public bool IsLoggedIn
        {
            get
            {
                return this.LoggedInUser != null;
            }
        }
    }

    public class UserLoggedInEventArgs : EventArgs
    {
        public UserLoggedInEventArgs(APUser user, string sessionToken)
        {
            this.User = user;
            this.SessionToken = sessionToken;
        }

        public APUser User { get; private set; }

        public string SessionToken { get; private set; }
    }
}
