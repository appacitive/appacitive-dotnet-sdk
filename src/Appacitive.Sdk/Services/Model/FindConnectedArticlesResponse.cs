using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Appacitive.Sdk.Interfaces;

namespace Appacitive.Sdk.Services
{
    public class FindConnectedArticlesResponse : ApiResponse
    {
        [JsonProperty("paginginfo")]
        public PagingInfo PagingInfo { get; set; }

        [JsonProperty("connections")]
        public List<Connection> Connections { get; set; }

        public static FindConnectedArticlesResponse Parse(byte[] bytes)
        {
            IJsonSerializer serializer = ObjectFactory.Build<IJsonSerializer>();
            return serializer.Deserialize<FindConnectedArticlesResponse>(bytes);
        }
    }
}
