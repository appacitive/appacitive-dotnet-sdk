using Appacitive.Sdk.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Services
{
    public class BulkDeleteArticleResponse : ApiResponse
    {
        public static BulkDeleteArticleResponse Parse(byte[] bytes)
        {
            IJsonSerializer serializer = ObjectFactory.Build<IJsonSerializer>();
            return new BulkDeleteArticleResponse() { Status = serializer.Deserialize<Status>(bytes) };
        }
    }
}
