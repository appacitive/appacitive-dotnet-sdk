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
    public class WcfPlatform : NetPlatform
    {
        public static readonly WcfPlatform Instance = new WcfPlatform();

        public override void InitializeContainer(IDependencyContainer container)
        {
            base.InitializeContainer(container);
            container.RegisterInstance<ISession, SessionCleanupProxy>(new SessionCleanupProxy(new InProcSession(), new TimeSpan(0,5,0), new TimeSpan(0,30,0)));
        }

        public override IApplicationState ApplicationState
        {
            get { return WcfApplicationState.Instance; }
        }
    }
}
