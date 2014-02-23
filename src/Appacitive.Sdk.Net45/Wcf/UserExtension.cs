using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Wcf
{
    public class UserExtension : IExtension<OperationContext>
    {

        public UserExtension(APUser user)
        {
            this.User = user;
        }

        public APUser User { get; set; }

        public void Attach(OperationContext owner)
        {
        }

        public void Detach(OperationContext owner)
        {
        }
    }
}
