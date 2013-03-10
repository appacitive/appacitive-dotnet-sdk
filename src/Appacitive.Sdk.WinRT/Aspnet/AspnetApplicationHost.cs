using Appacitive.Sdk.Interfaces;
using Appacitive.Sdk.WinRT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Aspnet
{
    public class AspnetApplicationHost : WindowsHost
    {
        protected override void InitializeContainer(IDependencyContainer container)
        {
            // Add default winrt registration
            base.InitializeContainer(container);
            // Add aspnet specific registrations
            container.Register<IUserContext, AspnetUserContext>(() => new AspnetUserContext());
        }
    }
}
