using Appacitive.Sdk.Realtime;
using Appacitive.Sdk.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Internal
{
    public class MessageSubscriber
    {
        public event Action<RealTimeMessage> NewMessage
        {
            add { EventProxy.Add(NewMessageTopic.Instance, value); }
            remove { EventProxy.Remove(NewMessageTopic.Instance, value); }
        }
    }
}
