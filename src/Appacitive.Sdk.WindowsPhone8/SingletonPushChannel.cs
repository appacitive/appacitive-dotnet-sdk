using Microsoft.Phone.Notification;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if WINDOWS_PHONE
namespace Appacitive.Sdk.WindowsPhone8
#elif WINDOWS_PHONE7
namespace Appacitive.Sdk.WindowsPhone7
#endif
{
    public class SingletonPushChannel : IDisposable, IPushChannel
    {
        private SingletonPushChannel() { }

        public event EventHandler<NotificationEventArgs> ShellToastNotificationReceived
        {
            add { _shellToastNotificationReceived += value; }
            remove { _shellToastNotificationReceived -= value; }
        }

        public event EventHandler<HttpNotificationEventArgs> HttpNotificationReceived
        {
            add { _httpNotificationReceived += value; }
            remove { _httpNotificationReceived -= value; }
        }

        private HttpNotificationChannel _channel = null;
        private readonly object _syncRoot = new object();
        private EventHandler<NotificationEventArgs> _shellToastNotificationReceived;
        private EventHandler<HttpNotificationEventArgs> _httpNotificationReceived;

        private HttpNotificationChannel GetChannel()
        {
            if (_channel == null)
            {
                lock (_syncRoot)
                {
                    if (_channel == null)
                        _channel = CreateNewChannel();
                }
            }
            return _channel;
        }

        private HttpNotificationChannel CreateNewChannel()
        {
            var channel = HttpNotificationChannel.Find(App.Current.AppId);
            if (channel == null)
            {
                channel = new HttpNotificationChannel(App.Current.AppId);
                SubscribeToEvents(channel);
                // Open the channel
                channel.Open();
                UpdateChannelUri(channel.ChannelUri);
                // Register for tile notifications
                var whitelistedDomains = App.Current.Settings.PushSettings.WhitelistedDomains;
                if (whitelistedDomains.Count == 0)
                    channel.BindToShellTile();
                else
                    channel.BindToShellTile(new Collection<Uri>(whitelistedDomains));
                // Register for shell notifications
                channel.BindToShellToast();
            }
            else
            {
                SubscribeToEvents(channel);
            }
            return channel;
        }

        private void SubscribeToEvents(HttpNotificationChannel channel)
        {
            // Subscribe to channel events.
            channel.ChannelUriUpdated += OnChannelUriUpdated;
            channel.ConnectionStatusChanged += OnConnectionStatusChanged;
            channel.ErrorOccurred += OnChannelErrorOccurred;
            // Subscribe to notifications.
            channel.HttpNotificationReceived += (s, e) =>
            {
                OnHttpNotificationReceived(e);
            };
            channel.ShellToastNotificationReceived += (s, e) =>
            {
                OnShellToastNotificationReceived(e);
            };
        }

        protected void OnShellToastNotificationReceived(NotificationEventArgs e)
        {
            var copy = _shellToastNotificationReceived;
            if (copy != null)
                copy(this, e);
        }

        protected void OnHttpNotificationReceived(HttpNotificationEventArgs e)
        {
            var copy = _httpNotificationReceived;
            if (copy != null)
                copy(this, e);
        }

        

        private void OnConnectionStatusChanged(object sender, NotificationChannelConnectionEventArgs e)
        {
            App.Debug.LogAsync(string.Format("Connection status changed to {0}.", e.ConnectionStatus));
        }

        void OnChannelErrorOccurred(object sender, NotificationChannelErrorEventArgs e)
        {
            bool reOpenConnection = e.ErrorType == ChannelErrorType.ChannelOpenFailed ||
                                    e.ErrorType == ChannelErrorType.PayloadFormatError;
            
            var currentChannel = sender as HttpNotificationChannel;
            if (reOpenConnection == true)
            {
                lock (_syncRoot)
                {
                    if (currentChannel == _channel)
                    {
                        _channel = CreateNewChannel();
                        CloseChannel(currentChannel);
                    }
                }
            }

            App.Debug.LogAsync(string.Format("Channel error- type:{0}, errorCode:{1}, message:{2}, addlData:{3}",
                e.ErrorType,
                e.ErrorCode,
                e.Message,
                e.ErrorAdditionalData
                ));
        }

        private void CloseChannel(HttpNotificationChannel channel)
        {
            try
            {
                if (channel != null)
                    channel.Close();
            }
            catch { }
        }

        void OnChannelUriUpdated(object sender, NotificationChannelUriEventArgs e)
        {
            UpdateChannelUri(e.ChannelUri);
        }

        private void UpdateChannelUri(Uri uri)
        {
            var existing = App.Current.GetCurrentDevice().Device.DeviceToken;
            var newToken = uri.ToString();
            if (string.Equals(newToken, existing) == false)
            {
                var current = App.Current.GetCurrentDevice();
                current.Device.DeviceToken = newToken;
                current.Device.SaveAsync();
            }
        }

        public void Dispose()
        {
            var channel = _channel;
            if (channel != null)
                channel.Dispose();
            _channel = null;
        }

        private static readonly IPushChannel _instance = new SingletonPushChannel();

        public static IPushChannel GetInstance()
        {
            return _instance;
        }
    }
}
