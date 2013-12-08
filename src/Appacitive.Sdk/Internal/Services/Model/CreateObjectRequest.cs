using Appacitive.Sdk.Realtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Services
{
    public class CreateObjectRequest : PutOperation<CreateObjectResponse>
    {
        public CreateObjectRequest() :
            this(AppacitiveContext.ApiKey, AppacitiveContext.SessionToken, AppacitiveContext.Environment, AppacitiveContext.UserToken, AppacitiveContext.UserLocation, AppacitiveContext.EnableDebugging, AppacitiveContext.Verbosity)
        {
        }

        public CreateObjectRequest(string apiKey, string sessionToken, Environment environment, string userToken = null, Geocode location = null, bool enableDebugging = false, Verbosity verbosity = Verbosity.Info) :
            base(apiKey, sessionToken, environment, userToken, location, enableDebugging, verbosity)
        {
        }

        public APObject Object { get; set; }

        public override byte[] ToBytes()
        {
            var serializer = ObjectFactory.Build<IJsonSerializer>();
            return serializer.Serialize(this.Object);
        }

        protected override string GetUrl()
        {
            return Urls.For.CreateObject(this.Object.Type, this.CurrentLocation, this.DebugEnabled, this.Verbosity, this.Fields);
        }
    }
}
