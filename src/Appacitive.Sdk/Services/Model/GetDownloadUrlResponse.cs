using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Appacitive.Sdk.Interfaces;

namespace Appacitive.Sdk.Services
{
    public class GetDownloadUrlResponse : ApiResponse
    {
        [JsonProperty("uri")]
        public string Url { get; set; }

        public static GetDownloadUrlResponse Parse(byte[] bytes)
        {
            IJsonSerializer serializer = ObjectFactory.Build<IJsonSerializer>();
            return serializer.Deserialize<GetDownloadUrlResponse>(bytes);
        }
    }
}
