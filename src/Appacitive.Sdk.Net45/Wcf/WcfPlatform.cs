using Appacitive.Sdk.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if NET40
using Appacitive.Sdk.Net40;
#else
using Appacitive.Sdk.Net45;
using Appacitive.Sdk.Wcf;
#endif

namespace Appacitive.Sdk.Wcf
{
    public class WcfPlatform : NetPlatform, IApplicationPlatform
    {
        public static readonly WcfPlatform Instance = new WcfPlatform();

        protected override void InitializeContainer(IDependencyContainer container)
        {
            base.InitializeContainer(container);
            container.RegisterInstance<ISession, InProcSession>(new InProcSession());
        }

        public override IApplicationState ApplicationState
        {
            get { return WcfApplicationState.Instance; }
        }
    }
}
