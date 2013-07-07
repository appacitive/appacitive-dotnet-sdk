using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Services
{
    public class MultiGetArticleRequest : GetOperation<MultiGetArticleResponse>
    {
        public MultiGetArticleRequest() :
            this(AppacitiveContext.ApiKey, AppacitiveContext.SessionToken, AppacitiveContext.Environment, AppacitiveContext.UserToken, AppacitiveContext.UserLocation, AppacitiveContext.EnableDebugging, AppacitiveContext.Verbosity)
        {
            this.IdList = new List<string>();
        }

        public MultiGetArticleRequest(string apiKey, string sessionToken, Environment environment, string userToken = null, Geocode location = null, bool enableDebugging = false, Verbosity verbosity = Verbosity.Info) :
            base(apiKey, sessionToken, environment, userToken, location, enableDebugging, verbosity)
        {
            this.IdList = new List<string>();
        }

        public string Type { get; set; }

        public List<string> IdList { get; private set; }

        protected override string GetUrl()
        {
            return Urls.For.MultiGetArticle(this.Type, this.IdList, this.CurrentLocation, this.DebugEnabled, this.Verbosity, this.Fields);
        }
    }
}
