using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Services
{
    public class GetDownloadUrlRequest : GetOperation<GetDownloadUrlResponse>
    {
        public GetDownloadUrlRequest() :
            this(AppacitiveContext.ApiKey, AppacitiveContext.SessionToken, AppacitiveContext.Environment, AppacitiveContext.UserToken, AppacitiveContext.UserLocation, AppacitiveContext.EnableDebugging, AppacitiveContext.Verbosity)
        {
        }

        public GetDownloadUrlRequest(string apiKey, string sessionToken, Environment environment, string userToken = null, Geocode location = null, bool enableDebugging = false, Verbosity verbosity = Verbosity.Info) :
            base(apiKey, sessionToken, environment, userToken, location, enableDebugging, verbosity)
        {
        }

        public string FileName { get; set; }

        public int ExpiryInMinutes { get; set; }

        protected override string GetUrl()
        {
            return Urls.For.GetDownloadUrl(this.FileName, this.ExpiryInMinutes);
        }
    }

    
}
