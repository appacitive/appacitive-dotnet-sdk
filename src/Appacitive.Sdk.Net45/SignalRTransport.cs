using Appacitive.Sdk.Realtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;
using Connection2 = Microsoft.AspNet.SignalR.Client.Connection;

namespace Appacitive.Sdk.Net45
{
    public class SignalRTransport : IRealTimeTransport
    {
        private async void OnStatechange(StateChange obj)
        {
            await Debugger.Log(string.Format("State changed from {0} to {1}.", obj.OldState, obj.NewState));
        }

        private async void OnReconnecting()
        {
            await Debugger.Log("Reconnecting");
        }

        private async void OnReconnected()
        {
            await Debugger.Log("Reconnected");
        }

        private async void OnError(Exception obj)
        {
            await Debugger.Log(string.Format("Faulted with exception {0}.", obj.ToString()));
        }

        private async void OnConnectionSlow()
        {
            await Debugger.Log("Connection is slow");
        }

        private async void OnClosed()
        {
            await Debugger.Log("Connection closed");
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
