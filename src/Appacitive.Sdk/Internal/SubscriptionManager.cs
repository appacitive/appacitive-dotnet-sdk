using Appacitive.Sdk.Realtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Internal
{
    internal class SingletonSubscriptionManager : ISubscriptionManager
    {
        public static readonly ISubscriptionManager Instance = new SingletonSubscriptionManager();

        private SingletonSubscriptionManager()
        {
        }

        private IDictionary<string, Subscription> _subscriptions = new Dictionary<string, Subscription>(StringComparer.OrdinalIgnoreCase);
        private readonly object _syncRoot = new object();

        public void Subscribe(Subscription subscription)
        {
            AddSubscription(subscription);
        }

        public void Unsubscribe(ITopic topic)
        {
            RemoveSubscription(topic);
        }

        private void AddSubscription(Subscription sub)
        {
            lock (_syncRoot)
            {
                if (_subscriptions.ContainsKey(sub.Topic.Key) == true)
                    throw new Exception("Multiple subscriptions for the same topic are not supported.");
                _subscriptions[sub.Topic.Key] = sub;
            }
        }

        private void RemoveSubscription(ITopic topic)
        {
            lock (_syncRoot)
            {
                if (_subscriptions.ContainsKey(topic.Key) == false)
                    return;
                _subscriptions.Remove(topic.Key);
            }
        }

        public Subscription Get(ITopic topic)
        {
            lock (_syncRoot)
            {
                Subscription sub = null;
                if (_subscriptions.TryGetValue(topic.Key, out sub) == true)
                    return sub;
                else return null;
            }
        }
    }
}
