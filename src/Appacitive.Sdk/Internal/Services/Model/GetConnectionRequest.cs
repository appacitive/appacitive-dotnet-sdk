using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Services
{
    public class GetConnectionRequest : GetOperation<GetConnectionResponse>
    {
        public GetConnectionRequest() :
            this(AppacitiveContext.ApiKey, AppacitiveContext.SessionToken, AppacitiveContext.Environment, AppacitiveContext.UserToken, AppacitiveContext.UserLocation, AppacitiveContext.EnableDebugging, AppacitiveContext.Verbosity)
        {
        }

        public GetConnectionRequest(string apiKey, string sessionToken, Environment environment, string userToken = null, Geocode location = null, bool enableDebugging = false, Verbosity verbosity = Verbosity.Info) :
            base(apiKey, sessionToken, environment, userToken, location, enableDebugging, verbosity)
        {
        }

        public string Relation { get; set; }

        public string Id { get; set; }

        protected override string GetUrl()
        {
            return Urls.For.GetConnection(this.Relation, this.Id, this.CurrentLocation, this.DebugEnabled, this.Verbosity, this.Fields);
        }
    }
}
