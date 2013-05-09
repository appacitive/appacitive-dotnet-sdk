﻿using Appacitive.Sdk.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Services
{
    public class RegisterDeviceResponse : ApiResponse
    {
        [JsonProperty("device")]
        public Device Device { get; set; }

        public static RegisterDeviceResponse Parse(byte[] data)
        {
            if (data == null || data.Length == 0)
                return null;
            var serializer = ObjectFactory.Build<IJsonSerializer>();
            return serializer.Deserialize<RegisterDeviceResponse>(data);
        }
    }
}
