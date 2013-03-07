using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Appacitive.Sdk.Services
{
    public class FindAllConectionsResponse : ApiResponse
    {
        [JsonProperty("connections")]
        public List<Connection> Connections { get; set; }

        [JsonProperty("paginginfo")]
        public PagingInfo PagingInfo { get; set; }

        internal static FindAllConectionsResponse Parse(byte[] bytes)
        {
            IJsonSerializer serializer = ObjectFactory.Build<IJsonSerializer>();
            return serializer.Deserialize<FindAllConectionsResponse>(bytes);
        }
    }

    
}
