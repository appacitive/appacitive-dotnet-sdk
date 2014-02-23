using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Services
{
    public class GetUserRequest : GetOperation<GetUserResponse>
    {
        
        public string UserIdType { get; set; }

        public string UserId { get; set; }

        protected override string GetUrl()
        {
            return Urls.For.GetUser(this.UserId, this.UserIdType, this.CurrentLocation, this.DebugEnabled, this.Verbosity, this.Fields);
        }
    }
}
