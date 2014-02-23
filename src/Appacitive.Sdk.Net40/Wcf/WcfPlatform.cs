using Appacitive.Sdk.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Wcf
{
    public class WcfPlatform : Net40Platform
    {
        public static new readonly Platform Instance = new WcfPlatform();

        public override void InitializeContainer(IDependencyContainer container)
        {
            base.InitializeContainer(container);
            container.Register<Platform, WcfPlatform>("wcf", () => WcfPlatform.Instance);
        }

        public override IContextService ContextService
        {
            get 
            {
                return WcfContextService.Instance;
            }
        }

        public override ILocalStorage LocalStorage
        {
            get { throw new NotSupportedException("Local storage is supported on WCF platform."); }
        }
    }
}
