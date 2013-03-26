using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Configuration;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Wcf
{
    public class AllowAsyncServiceBehaviorExtension : BehaviorExtensionElement
    {
        public override Type BehaviorType
        {
            get { return typeof(AllowAsyncService); }
        }

        protected override object CreateBehavior()
        {
            return new AllowAsyncService();
        }
    }
}
