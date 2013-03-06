using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Appacitive.Sdk
{
    public interface IApplicationHost
    {
        void InitializeContainer(IDependencyContainer container);
    }
}
