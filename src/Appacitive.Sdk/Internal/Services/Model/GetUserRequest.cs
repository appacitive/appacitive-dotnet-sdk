using Appacitive.Sdk.Realtime;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Services
{
    public class GetUserRequest : GetOperation<GetUserResponse>
    {
        public GetUserRequest() :
            this(AppacitiveContext.ApiKey, AppacitiveContext.SessionToken, AppacitiveContext.Environment, AppacitiveContext.UserToken, AppacitiveContext.UserLocation, AppacitiveContext.EnableDebugging, AppacitiveContext.Verbosity)
        {
        }

        public GetUserRequest(string apiKey, string sessionToken, Environment environment, string userToken = null, Geocode location = null, bool enableDebugging = false, Verbosity verbosity = Verbosity.Info) :
            base(apiKey, sessionToken, environment, userToken, location, enableDebugging, verbosity)
        {
            this.UserIdType = string.Empty; // Nikhil: String.empty indicates default type is id. This should probably be changed.
        }

        public string UserIdType { get; set; }

        public string UserId { get; set; }

        protected override string GetUrl()
        {
            return Urls.For.GetUser(this.UserId, this.UserIdType, this.CurrentLocation, this.DebugEnabled, this.Verbosity, this.Fields);
        }
    }
}
