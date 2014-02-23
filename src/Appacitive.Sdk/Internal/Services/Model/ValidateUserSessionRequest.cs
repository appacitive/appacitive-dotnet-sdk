using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Services
{
    public class ValidateUserSessionRequest : PostOperation<ValidateUserSessionResponse>
    {
        public override byte[] ToBytes()
        {
            return null;
        }

        protected override string GetUrl()
        {
            return Urls.For.ValidateUserSession(this.CurrentLocation, this.DebugEnabled, this.Verbosity);
        }
    }
}
