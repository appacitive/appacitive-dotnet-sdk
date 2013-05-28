using Appacitive.Sdk.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Realtime
{
    public interface ISubscriptionManager
    {
        void Subscribe(Subscription subscription);

        void Unsubscribe(ITopic topic);

        Subscription Get(ITopic topic);
    }
}
