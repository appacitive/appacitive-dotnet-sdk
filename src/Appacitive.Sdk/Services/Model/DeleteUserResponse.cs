using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Appacitive.Sdk.Interfaces;

namespace Appacitive.Sdk.Services
{
    public class DeleteUserResponse : ApiResponse
    {
        public static DeleteUserResponse Parse(byte[] bytes)
        {
            IJsonSerializer serializer = ObjectFactory.Build<IJsonSerializer>();
            return new DeleteUserResponse() { Status = serializer.Deserialize<Status>(bytes) };
        }
    }
}
