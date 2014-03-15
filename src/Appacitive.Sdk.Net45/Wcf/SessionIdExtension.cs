using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Wcf
{
    public class SessionIdExtension : IExtension<OperationContext>
    {

        public SessionIdExtension(string sessionId)
        {
            this.SessionId = sessionId;
        }

        public string SessionId { get; set; }

        public void Attach(OperationContext owner)
        {   
        }

        public void Detach(OperationContext owner)
        {
        }
    }
}
