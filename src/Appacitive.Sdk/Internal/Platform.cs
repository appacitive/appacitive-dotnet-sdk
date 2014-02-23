using Appacitive.Sdk.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk
{
    public abstract class Platform
    {
        public abstract void InitializeContainer(IDependencyContainer container);

        public abstract bool IsNetworkAvailable();

        public abstract IContextService ContextService { get; }

        public abstract ILocalStorage LocalStorage { get; }
    }
}
