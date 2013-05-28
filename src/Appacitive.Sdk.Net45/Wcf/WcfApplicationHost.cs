using Appacitive.Sdk.Realtime;
using Appacitive.Sdk.Net45;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Wcf
{
    public class WcfApplicationHost : WindowsHost
    {

        public static new readonly IApplicationHost Instance = new WcfApplicationHost();

        protected override void InitializeContainer(IDependencyContainer container)
        {
            // Add default winrt registration
            base.InitializeContainer(container);
            // Add aspnet specific registrations
            container.Register<IUserContext, WcfUserContext>(() => WcfUserContext.Instance);
        }
    }
}
