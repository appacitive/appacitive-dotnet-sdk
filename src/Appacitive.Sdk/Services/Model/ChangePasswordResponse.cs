using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Services
{
    public class ChangePasswordResponse : ApiResponse
    {
        public static ChangePasswordResponse Parse(byte[] bytes)
        {
            IJsonSerializer serializer = ObjectFactory.Build<IJsonSerializer>();
            return new ChangePasswordResponse() 
            {
                Status = serializer.Deserialize<Status>(bytes)
            };
        }
    }
}
