using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Services
{
    public class ValidateUserSessionRequest : PostOperation<ValidateUserSessionResponse>
    {
        public ValidateUserSessionRequest() :
            this(AppacitiveContext.ApiKey, AppacitiveContext.SessionToken, AppacitiveContext.Environment, AppacitiveContext.UserToken, AppacitiveContext.UserLocation, AppacitiveContext.EnableDebugging, AppacitiveContext.Verbosity)
        {
        }

        public ValidateUserSessionRequest(string apiKey, string sessionToken, Environment environment, string userToken = null, Geocode location = null, bool enableDebugging = false, Verbosity verbosity = Verbosity.Info) :
            base(apiKey, sessionToken, environment, userToken, location, enableDebugging, verbosity)
        {
        }

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
