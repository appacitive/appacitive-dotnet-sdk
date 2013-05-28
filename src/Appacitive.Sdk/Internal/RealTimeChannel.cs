using Appacitive.Sdk.Realtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Internal
{
    public class RealTimeChannel : IRealTimeChannel
    {
        public IRealTimeTransport Transport { get; set; }

        public async Task SendAsync(RealTimeMessage msg)
        {
            await this.Transport.SendAsync(msg.ToString());
        }

        private readonly object _syncRoot = new object();
        private Action<RealTimeMessage> _onReceive;
        public event Action<RealTimeMessage> Receive
        {
            add 
            {
                lock (_syncRoot)
                {
                    _onReceive += value;
                }
            }
            remove 
            {
                if (_onReceive == null) return;
                lock (_syncRoot)
                {
                    _onReceive -= value;
                }
            }
        }

        public async Task Start()
        {
            if (this.Transport != null)
                return;
            this.Transport = ObjectFactory.Build<IRealTimeTransport>();
            this.Transport.Received += OnReceive;
            await this.Transport.Start();
        }

        private void OnReceive(string obj)
        {
            var copy = _onReceive;
            if (copy == null)
                return;
            var serializer = ObjectFactory.Build<IJsonSerializer>();
            var message = serializer.Deserialize<RealTimeMessage>(Encoding.UTF8.GetBytes(obj));
            if( message != null )
                copy(message);
        }

        public void Stop()
        {
            if (this.Transport != null)
            {
                this.Transport.Stop();
                this.Transport = null;
            }
        }
    }
}
