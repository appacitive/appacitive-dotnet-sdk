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
        public void Initialize(AppContext context)
        {
            InitializeContainer(context.Container);
            Init(context);
        }



        protected abstract void InitializeContainer(IDependencyContainer container);

        protected virtual void Init(AppContext context)
        {
        }
    }
}
