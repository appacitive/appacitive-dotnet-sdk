using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Services
{
    public class GetObjectRequest : GetOperation<GetObjectResponse>
    {
        public GetObjectRequest() :
            this(AppacitiveContext.ApiKey, AppacitiveContext.SessionToken, AppacitiveContext.Environment, AppacitiveContext.UserToken, AppacitiveContext.UserLocation, AppacitiveContext.EnableDebugging, AppacitiveContext.Verbosity)
        {
        }

        public GetObjectRequest(string apiKey, string sessionToken, Environment environment, string userToken = null, Geocode location = null, bool enableDebugging = false, Verbosity verbosity = Verbosity.Info) :
            base(apiKey, sessionToken, environment, userToken, location, enableDebugging, verbosity)
        {
        }

        public string Type { get; set; }

        public string Id { get; set; }

        public override byte[] ToBytes()
        {
            return null;
        }

        protected override string GetUrl()
        {
            return Urls.For.GetObject(this.Type, this.Id, this.CurrentLocation, this.DebugEnabled, this.Verbosity, this.Fields);
        }
    }
}
