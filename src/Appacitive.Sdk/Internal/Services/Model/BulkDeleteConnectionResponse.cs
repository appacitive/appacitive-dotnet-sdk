using Appacitive.Sdk.Realtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Services
{
    public class BulkDeleteConnectionResponse : ApiResponse
    {
        public static BulkDeleteConnectionResponse Parse(byte[] bytes)
        {
            IJsonSerializer serializer = ObjectFactory.Build<IJsonSerializer>();
            return new BulkDeleteConnectionResponse() { Status = serializer.Deserialize<Status>(bytes) };
        }
    }
}
