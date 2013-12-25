using Appacitive.Sdk.Realtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Appacitive.Sdk.Services
{
    public class CreateUserRequest : PutOperation<CreateUserResponse>
    {
        public CreateUserRequest() :
            this(AppacitiveContext.ApiKey, AppacitiveContext.SessionToken, AppacitiveContext.Environment, AppacitiveContext.UserToken, AppacitiveContext.UserLocation, AppacitiveContext.EnableDebugging, AppacitiveContext.Verbosity)
        {
        }

        public CreateUserRequest(string apiKey, string sessionToken, Environment environment, string userToken = null, Geocode location = null, bool enableDebugging = false, Verbosity verbosity = Verbosity.Info) :
            base(apiKey, sessionToken, environment, userToken, location, enableDebugging, verbosity)
        {
        }

        public APUser User { get; set; }

        public override byte[] ToBytes()
        {
            var serializer = ObjectFactory.Build<IJsonSerializer>();
            return serializer.Serialize(this.User);
        }

        protected override string GetUrl()
        {
            return Urls.For.CreateUser(this.CurrentLocation, this.DebugEnabled, this.Verbosity, this.Fields);
        }
    }
}
