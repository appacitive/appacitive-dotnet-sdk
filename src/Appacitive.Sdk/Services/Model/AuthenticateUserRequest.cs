using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace Appacitive.Sdk.Services
{
    public class AuthenticateUserRequest : ApiRequest
    {
        public AuthenticateUserRequest() :
            this(AppacitiveContext.SessionToken, AppacitiveContext.Environment, AppacitiveContext.UserToken, AppacitiveContext.UserLocation, AppacitiveContext.EnableDebugging, AppacitiveContext.Verbosity)
        {   
        }

        public AuthenticateUserRequest(string sessionToken, Environment environment, string userToken = null, Geocode location = null, bool enableDebugging = false, Verbosity verbosity = Verbosity.Info) :
            base(sessionToken, environment, userToken, location, enableDebugging, verbosity)
        {
            this.Attributes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; set; }

        [JsonProperty("attempts", NullValueHandling = NullValueHandling.Ignore)]
        public int MaxAttempts { get; set; }

        [JsonProperty("expiry", NullValueHandling = NullValueHandling.Ignore)]
        public int TimeoutInSeconds { get; set; }

        public IDictionary<string, string> Attributes { get; private set; }

        public string this[string attribute]
        {
            get
            {
                string value = null;
                if (this.Attributes.TryGetValue(attribute, out value) == true)
                    return value;
                else return null;
            }
            set
            {
                this.Attributes[attribute] = value;
            }
        }
    }
}
