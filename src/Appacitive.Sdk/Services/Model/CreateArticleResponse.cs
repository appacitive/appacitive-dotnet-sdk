using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Appacitive.Sdk.Interfaces;

namespace Appacitive.Sdk.Services
{
    public class CreateArticleResponse : ApiResponse
    {
        [JsonProperty("article")]
        public Article Article { get; set; }

        public static CreateArticleResponse Parse(byte[] bytes)
        {
            IJsonSerializer serializer = ObjectFactory.Build<IJsonSerializer>();
            return serializer.Deserialize<CreateArticleResponse>(bytes);
        }
    }
}
