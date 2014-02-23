using Appacitive.Sdk.Realtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Appacitive.Sdk.Internal;

namespace Appacitive.Sdk.Aspnet
{
    public class AspnetPlatform : Net40Platform
    {
        public static new readonly Platform Instance = new AspnetPlatform();

        public override void InitializeContainer(IDependencyContainer container)
        {
            // Add default winrt registration
            base.InitializeContainer(container);
            // Add aspnet specific registrations
            container.Register<Platform, AspnetPlatform>("aspnet", () => AspnetPlatform.Instance);
        }

        public override IContextService ContextService
        {
            get
            {
                return AspnetContextService.Instance;
            }
        }

        public override ILocalStorage LocalStorage
        {
            get { throw new NotSupportedException("Local storage is supported on ASP.net platform."); }
        }
        
    }
}
