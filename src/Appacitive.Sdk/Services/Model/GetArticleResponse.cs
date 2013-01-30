using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Appacitive.Sdk.Services
{
    public class GetArticleResponse : ApiResponse
    {
        [JsonProperty("article")]
        public Article Article { get; set; }

        public static GetArticleResponse Parse(byte[] bytes)
        {
            IJsonSerializer serializer = ObjectFactory.Build<IJsonSerializer>();
            return serializer.Deserialize<GetArticleResponse>(bytes);
        }
    }
}
