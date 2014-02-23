using Appacitive.Sdk.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Appacitive.Sdk.Services
{
    public class CreateUserRequest : PutOperation<CreateUserResponse>
    {
        public APUser User { get; set; }

        public override byte[] ToBytes()
        {
            var serializer = ObjectFactory.Build<IJsonSerializer>();
            return serializer.Serialize(this.User);
        }

        protected override string GetUrl()
        {
            return Urls.For.CreateUser(this.CurrentLocation, this.DebugEnabled, this.Verbosity, this.Fields);
        }
    }
}
