using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Services
{
    public class BulkDeleteObjectRequest : PostOperation<BulkDeleteObjectResponse>
    {
        public BulkDeleteObjectRequest() :
            this(AppacitiveContext.ApiKey, AppacitiveContext.SessionToken, AppacitiveContext.Environment, AppacitiveContext.UserToken, AppacitiveContext.UserLocation, AppacitiveContext.EnableDebugging, AppacitiveContext.Verbosity)
        {
        }

        public BulkDeleteObjectRequest(string apiKey, string sessionToken, Environment environment, string userToken = null, Geocode location = null, bool enableDebugging = false, Verbosity verbosity = Verbosity.Info) :
            base(apiKey, sessionToken, environment, userToken, location, enableDebugging, verbosity)
        {
        }

        [JsonIgnore]
        public string Type { get; set; }

        [JsonProperty("idlist")]
        public List<string> ObjectIds { get; set; }



        protected override string GetUrl()
        {
            return Urls.For.BulkDeleteObjects(this.Type, this.CurrentLocation, this.DebugEnabled, this.Verbosity, this.Fields);
        }
    }
}
