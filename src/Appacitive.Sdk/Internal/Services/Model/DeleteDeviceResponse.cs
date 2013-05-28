using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Appacitive.Sdk.Realtime;

namespace Appacitive.Sdk.Services
{
    public class DeleteDeviceResponse : ApiResponse
    {
        public static DeleteDeviceResponse Parse(byte[] bytes)
        {
            IJsonSerializer serializer = ObjectFactory.Build<IJsonSerializer>();
            return new DeleteDeviceResponse() { Status = serializer.Deserialize<Status>(bytes) };
        }
    }
}
