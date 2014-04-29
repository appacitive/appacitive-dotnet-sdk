using Appacitive.Sdk.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk
{
    public interface IPlatform
    {
        void InitializeContainer(IDependencyContainer container);

        void Init(AppContext context);
    }
}
