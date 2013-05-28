using Appacitive.Sdk.Realtime;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Services
{
    public class InvalidateUserSessionResponse : ApiResponse
    {
        public static InvalidateUserSessionResponse Parse(byte[] bytes)
        {
            IJsonSerializer serializer = ObjectFactory.Build<IJsonSerializer>();
            return new InvalidateUserSessionResponse { Status = serializer.Deserialize<Status>(bytes) };
        }
    }
}
