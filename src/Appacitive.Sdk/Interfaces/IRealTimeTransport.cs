using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Realtime
{
    public interface IRealTimeTransport : IDisposable
    {
        Task Start();

        void Stop();

        Task SendAsync(string data);

        event Action<string> Received;
    }
}
