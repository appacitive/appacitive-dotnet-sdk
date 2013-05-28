using Appacitive.Sdk.Realtime;
using Appacitive.Sdk.Net45;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Aspnet
{
    public class AspnetApplicationHost : WindowsHost
    {
        public static new readonly IApplicationHost Instance = new AspnetApplicationHost();

        protected override void InitializeContainer(IDependencyContainer container)
        {
            // Add default winrt registration
            base.InitializeContainer(container);
            // Add aspnet specific registrations
            container.Register<IUserContext, AspnetUserContext>(() => UserContextViaHttpContext.Instance);
        }
    }
}
