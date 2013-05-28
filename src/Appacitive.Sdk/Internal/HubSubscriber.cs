using Appacitive.Sdk.Realtime;
using Appacitive.Sdk.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Internal
{
    public class HubSubscriber
    {
        public event Action<RealTimeMessage> Joined
        {
            add { EventProxy.Add(JoinedHubTopic.Instance, value); }
            remove { EventProxy.Remove(JoinedHubTopic.Instance, value); }
        }

        public event Action<RealTimeMessage> Left
        {
            add { EventProxy.Add(LeftHubTopic.Instance, value); }
            remove { EventProxy.Remove(LeftHubTopic.Instance, value); }
        }
    }
}
