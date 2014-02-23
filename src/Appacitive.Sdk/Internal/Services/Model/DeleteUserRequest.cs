using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Services
{
    public class DeleteUserRequest : DeleteOperation<DeleteUserResponse>
    {
        public bool DeleteConnections { get; set; }

        public string UserIdType { get; set; }

        public string UserId { get; set; }

        protected override string GetUrl()
        {
            return Urls.For.DeleteUser(this.UserId, this.UserIdType, this.DeleteConnections, this.CurrentLocation, this.DebugEnabled, this.Verbosity, this.Fields);
        }
    }
}
