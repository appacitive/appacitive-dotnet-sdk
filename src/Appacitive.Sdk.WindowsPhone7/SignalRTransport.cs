using Appacitive.Sdk.Realtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.WindowsPhone7
{
    public class SignalRTransport : IRealTimeTransport
    {
        private async void OnStatechange(StateChange obj)
        {
            await App.Debug.LogAsync(string.Format("State changed from {0} to {1}.", obj.OldState, obj.NewState));
        }

        private async void OnReconnecting()
        {
            await App.Debug.LogAsync("Reconnecting");
        }

        private async void OnReconnected()
        {
            await App.Debug.LogAsync("Reconnected");
        }

        private async void OnError(Exception obj)
        {
            await App.Debug.LogAsync(string.Format("Faulted with exception {0}.", obj.ToString()));
        }

        private async void OnConnectionSlow()
        {
            await App.Debug.LogAsync("Connection is slow");
        }

        private async void OnClosed()
        {
            await App.Debug.LogAsync("Connection closed");
        }

        private Connection2 _conn = null;

        public async Task SendAsync(string data)
        {
            if (_conn == null)
                throw new Exception("Connection is not started.");
            await _conn.Send(data);

        }

        private readonly object _eventLock = new object();
        private Action<string> _received;
        public event Action<string> Received
        {
            add
            {
                lock (_eventLock)
                {
                    _received += value;
                }
            }
            remove
            {
                lock (_eventLock)
                {
                    _received -= value;
                }
            }
        }

        public void Dispose()
        {
            if (_conn != null)
            {
                _conn.Stop();
                _conn = null;
            }
        }

        protected void OnReceived(string data)
        {
            var copy = _received;
            if (copy != null)
                copy(data);
        }

        public async Task Start()
        {
            if (_conn != null)
                return;
            _conn = new Connection2(Urls.RealTimeMessaging);
            _conn.Headers["ut"] = App.UserToken;
            _conn.Headers["ak"] = App.Apikey;
            _conn.Received += OnReceived;
            _conn.Closed += OnClosed;
            _conn.ConnectionSlow += OnConnectionSlow;
            _conn.Error += OnError;
            _conn.Reconnected += OnReconnected;
            _conn.Reconnecting += OnReconnecting;
            _conn.StateChanged += OnStatechange;
            await _conn.Start();
        }

        public void Stop()
        {
            if (_conn != null)
            {
                _conn.Stop();
                _conn = null;
            }
        }
    }
}
