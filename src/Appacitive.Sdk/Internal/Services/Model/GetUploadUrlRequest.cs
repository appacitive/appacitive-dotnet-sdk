using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Services
{
    public class GetUploadUrlRequest : GetOperation<GetUploadUrlResponse>
    {
        public GetUploadUrlRequest() :
            this(AppacitiveContext.ApiKey, AppacitiveContext.SessionToken, AppacitiveContext.Environment, AppacitiveContext.UserToken, AppacitiveContext.UserLocation, AppacitiveContext.EnableDebugging, AppacitiveContext.Verbosity)
        {
        }

        public GetUploadUrlRequest(string apiKey, string sessionToken, Environment environment, string userToken = null, Geocode location = null, bool enableDebugging = false, Verbosity verbosity = Verbosity.Info) :
            base(apiKey, sessionToken, environment, userToken, location, enableDebugging, verbosity)
        {
        }

        public string MimeType { get; set; }

        public string FileName { get; set; }

        public int ExpiryInMinutes { get; set; }

        protected override string GetUrl()
        {
            return Urls.For.GetUploadUrl(this.MimeType, this.FileName, this.ExpiryInMinutes);
        }
    }

    
}
