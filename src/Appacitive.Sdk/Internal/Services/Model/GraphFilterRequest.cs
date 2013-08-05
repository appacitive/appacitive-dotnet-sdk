using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Services
{
    public class GraphFilterRequest : PostOperation<GraphFilterResponse>
    {
        public GraphFilterRequest() :
            this(AppacitiveContext.ApiKey, AppacitiveContext.SessionToken, AppacitiveContext.Environment, AppacitiveContext.UserToken, AppacitiveContext.UserLocation, AppacitiveContext.EnableDebugging, AppacitiveContext.Verbosity)
        {
        }

        public GraphFilterRequest(string apiKey, string sessionToken, Environment environment, string userToken = null, Geocode location = null, bool enableDebugging = false, Verbosity verbosity = Verbosity.Info) :
            base(apiKey, sessionToken, environment, userToken, location, enableDebugging, verbosity)
        {
        }

        [JsonIgnore]
        public string Query { get; set; }

        [JsonProperty("placeholders")]
        public IDictionary<string, string> Placeholders { get; set; }
        
        protected override string GetUrl()
        {
            return Urls.For.GraphFilter(this.Query, this.CurrentLocation, this.DebugEnabled, this.Verbosity, this.Fields);
        }
    }
}
