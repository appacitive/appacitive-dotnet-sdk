using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Services
{
    public class InvalidateUserSessionRequest : PostOperation<InvalidateUserSessionResponse>
    {
        
        public override byte[] ToBytes()
        {
            return null;
        }

        protected override string GetUrl()
        {
            return Urls.For.InvalidateUser(this.CurrentLocation, this.DebugEnabled, this.Verbosity);
        }
    }
}
