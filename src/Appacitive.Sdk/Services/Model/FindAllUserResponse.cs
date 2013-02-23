using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Appacitive.Sdk.Services
{
    public class FindAllUsersResponse : ApiResponse
    {
        [JsonProperty("articles")]
        public List<User> Articles { get; set; }

        [JsonProperty("paginginfo")]
        public PagingInfo PagingInfo { get; set; }

        internal static FindAllUsersResponse Parse(byte[] bytes)
        {
            IJsonSerializer serializer = ObjectFactory.Build<IJsonSerializer>();
            return serializer.Deserialize<FindAllUsersResponse>(bytes);
        }
    }
}
