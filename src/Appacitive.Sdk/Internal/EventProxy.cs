using Appacitive.Sdk.Realtime;
using Appacitive.Sdk.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Internal
{
    internal static class EventProxy
    {
        public static readonly object _lock = new object();

        internal static void Add(ITopic topic, Action<RealTimeMessage> handler)
        {
            if (handler == null) throw new ArgumentException("Event handler cannot be null.");
            var subscriptionManager = ObjectFactory.Build<ISubscriptionManager>();
            lock (_lock)
            {
                // Get existing
                var sub = subscriptionManager.Get(topic);
                // Create if not exists.
                if (sub == null)
                {
                    sub = new Subscription { Topic = topic };
                    sub.Triggered += handler;
                    subscriptionManager.Subscribe(sub);
                }
                else
                {
                    sub.Triggered += handler;
                }
                
            }

        }

        internal static void Remove(ITopic topic, Action<RealTimeMessage> handler)
        {
            var subscriptionManager = ObjectFactory.Build<ISubscriptionManager>();
            lock (_lock)
            {
                var sub = subscriptionManager.Get(topic);
                if (sub == null)
                    return;
                sub.Triggered -= handler;
                if (sub.IsEmpty == true)
                    subscriptionManager.Unsubscribe(topic);
            }
        }
    }
}
