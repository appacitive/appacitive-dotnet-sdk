using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Appacitive.Sdk.Services
{
    public class ChangePasswordRequest : ApiRequest
    {
        public ChangePasswordRequest() :
            this(AppacitiveContext.SessionToken, AppacitiveContext.Environment, AppacitiveContext.UserToken, AppacitiveContext.UserLocation, AppacitiveContext.EnableDebugging, AppacitiveContext.Verbosity)
        {
        }

        public ChangePasswordRequest(string sessionToken, Environment environment, string userToken = null, Geocode location = null, bool enableDebugging = false, Verbosity verbosity = Verbosity.Info) :
            base(sessionToken, environment, userToken, location, enableDebugging, verbosity)
        {
            this.IdType = string.Empty; // Nikhil: String.empty indicates default type is id. This should probably be changed.
        }

        [JsonIgnore]
        public string UserId { get; set; }

        [JsonIgnore]
        public string IdType { get; set; }

        [JsonProperty("oldpassword")]
        public string OldPassword { get; set; }

        [JsonProperty("newpassword")]
        public string NewPassword { get; set; }
    }

    
}
