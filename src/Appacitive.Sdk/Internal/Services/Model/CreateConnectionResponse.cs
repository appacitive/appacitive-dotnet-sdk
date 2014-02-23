﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Appacitive.Sdk.Internal;


namespace Appacitive.Sdk.Services
{
    public class CreateConnectionResponse : ApiResponse
    {
        [JsonProperty("connection")]
        public APConnection Connection { get; set; }

        public static CreateConnectionResponse Parse(byte[] bytes)
        {
            IJsonSerializer serializer = ObjectFactory.Build<IJsonSerializer>();
            return serializer.Deserialize<CreateConnectionResponse>(bytes);
        }
    }
}
