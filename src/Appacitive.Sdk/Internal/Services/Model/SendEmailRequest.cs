using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Appacitive.Sdk.Internal;



namespace Appacitive.Sdk.Services
{
    public class SendEmailRequest : PostOperation<SendEmailResponse>
    {
        public Email Email { get; set; }

        public override byte[] ToBytes()
        {
            var serializer = ObjectFactory.Build<IJsonSerializer>();
            return serializer.Serialize(this.Email);
        }

        protected override string GetUrl()
        {
            return Urls.For.SendEmail(this.CurrentLocation, this.DebugEnabled, this.Verbosity, this.Fields);
        }
    }
}
