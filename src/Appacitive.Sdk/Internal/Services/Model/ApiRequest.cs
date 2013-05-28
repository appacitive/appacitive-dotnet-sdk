using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Appacitive.Sdk.Realtime;

namespace Appacitive.Sdk.Services
{
    public abstract class ApiRequest
    {
        protected ApiRequest(string apiKey, string sessionToken, Environment environment, string userToken = null, Geocode location = null, bool enableDebugging = false, Verbosity verbosity = Verbosity.Info)
        {
            this.ApiKey = apiKey;
            this.UseApiSession = string.IsNullOrWhiteSpace(sessionToken) ? false : true;
            this.SessionToken = sessionToken;
            this.CurrentLocation = location;
            this.Verbosity = verbosity;
            this.UserToken = userToken;
            this.Environment = environment;
            this.Fields = new List<string>();
        }

        [JsonIgnore]
        public string ApiKey { get; set; }

        [JsonIgnore]
        public bool UseApiSession { get; private set; }

        [JsonIgnore]
        public string SessionToken { get; set; }

        [JsonIgnore]
        public List<string> Fields { get; private set; }

        [JsonIgnore]
        public string UserToken { get; set; }

        [JsonIgnore]
        public Environment Environment { get; set; }

        [JsonIgnore]
        public bool DebugEnabled { get; set; }

        [JsonIgnore]
        public Geocode CurrentLocation { get; set; }

        [JsonIgnore]
        public Verbosity Verbosity { get; set; }

        public virtual byte[] ToBytes()
        {
            IJsonSerializer serializer = ObjectFactory.Build<IJsonSerializer>();
            return serializer.Serialize(this);
        }
    }
}
