using Appacitive.Sdk.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Realtime
{
    public interface IRealTimeChannel
    {
        Task Start();

        void Stop();

        Task SendAsync(RealTimeMessage msg);

        event Action<RealTimeMessage> Receive;
    }
}
