using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Wcf
{
    public class UserTokenExtension : IExtension<OperationContext>
    {

        public UserTokenExtension(string token)
        {
            this.UserToken = token;
        }

        public string UserToken { get; set; }

        public void Attach(OperationContext owner)
        {   
        }

        public void Detach(OperationContext owner)
        {
        }
    }
}
