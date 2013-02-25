using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Appacitive.Sdk.Services
{
    public class DeleteConnectionResponse : ApiResponse
    {
        public static DeleteConnectionResponse Parse(byte[] bytes)
        {
            IJsonSerializer serializer = ObjectFactory.Build<IJsonSerializer>();
            return new DeleteConnectionResponse() { Status = serializer.Deserialize<Status>(bytes) };
        }
    }
}
