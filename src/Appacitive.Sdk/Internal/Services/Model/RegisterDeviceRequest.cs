﻿using Appacitive.Sdk.Realtime;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Services
{
    public class RegisterDeviceRequest : PutOperation<RegisterDeviceResponse>
    {
        public RegisterDeviceRequest() :
            this(AppacitiveContext.ApiKey, AppacitiveContext.SessionToken, AppacitiveContext.Environment, AppacitiveContext.UserToken, AppacitiveContext.UserLocation, AppacitiveContext.EnableDebugging, AppacitiveContext.Verbosity)
        {
        }

        public RegisterDeviceRequest(string apiKey, string sessionToken, Environment environment, string userToken = null, Geocode location = null, bool enableDebugging = false, Verbosity verbosity = Verbosity.Info) :
            base(apiKey, sessionToken, environment, userToken, location, enableDebugging, verbosity)
        {
        }

        public APDevice Device { get; set; }

        public override byte[] ToBytes()
        {
            var serializer = ObjectFactory.Build<IJsonSerializer>();
            return serializer.Serialize(this.Device);
        }

        protected override string GetUrl()
        {
            return Urls.For.RegisterDevice(this.CurrentLocation, this.DebugEnabled, this.Verbosity, this.Fields);
        }
    }
}
