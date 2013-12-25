using Appacitive.Sdk.Realtime;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Services
{
    public class InitiateResetPasswordRequest : PostOperation<InitiateResetPasswordResponse>
    {
        public InitiateResetPasswordRequest() :
            this(AppacitiveContext.ApiKey, AppacitiveContext.SessionToken, AppacitiveContext.Environment, AppacitiveContext.UserToken, AppacitiveContext.UserLocation, AppacitiveContext.EnableDebugging, AppacitiveContext.Verbosity)
        {
        }

        public InitiateResetPasswordRequest(string apiKey, string sessionToken, Environment environment, string userToken = null, Geocode location = null, bool enableDebugging = false, Verbosity verbosity = Verbosity.Info) :
            base(apiKey, sessionToken, environment, userToken, location, enableDebugging, verbosity)
        {   
        }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("subject")]
        public string EmailSubject { get; set; }

        protected override string GetUrl()
        {
            return Urls.For.InitiateResetPassword(this.CurrentLocation, this.DebugEnabled, this.Verbosity);
        }
    }

    public class InitiateResetPasswordResponse : ApiResponse
    {   
    }
}
