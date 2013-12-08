using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Services
{
    [JsonConverter(typeof(StatusResponseConverter<DeleteObjectResponse>))]
    public class DeleteObjectResponse : ApiResponse
    {
    }
}
