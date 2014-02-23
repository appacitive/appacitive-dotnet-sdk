﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Appacitive.Sdk.Internal;


namespace Appacitive.Sdk.Services
{
    public abstract class ApiRequest
    {
        protected ApiRequest()
        {
            var appContext = App.Current;
            this.ApiKey = appContext.ApiKey;
            var user = appContext.CurrentUser.GetLoggedInUser();
            if (user != null && user.Location != null)
                this.CurrentLocation = user.Location;
            this.UserToken = appContext.CurrentUser.UserToken;
            this.Environment = appContext.Environment;
            this.DebugEnabled = App.Debug.IsProfilingEnabled;
            this.Verbosity = App.Debug.Verbosity;
            this.Fields = new List<string>();
        }

        [JsonIgnore]
        public string ApiKey { get; set; }

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
