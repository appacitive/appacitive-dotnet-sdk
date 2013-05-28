using Appacitive.Sdk.Realtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Internal
{
    public class Subscription
    {
        public ITopic Topic { get; set; }

        public event Action<RealTimeMessage> Triggered;

        public void Notify(RealTimeMessage msg)
        {
            var copy = this.Triggered;
            if (copy != null)
                copy(msg);
        }

        public bool IsEmpty
        {
            get
            {
                var action = this.Triggered;
                if (action == null)
                    return true;
                var invList = action.GetInvocationList();
                return invList == null || invList.Length == 0;
            }
        }
    }

}
