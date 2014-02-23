using Appacitive.Sdk.Internal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Services
{
    public class RegisterDeviceRequest : PutOperation<RegisterDeviceResponse>
    {
        public APDevice Device { get; set; }

        public override byte[] ToBytes()
        {
            var serializer = ObjectFactory.Build<IJsonSerializer>();
            return serializer.Serialize(this.Device);
        }

        protected override string GetUrl()
        {
            return Urls.For.RegisterDevice(this.CurrentLocation, this.DebugEnabled, this.Verbosity, this.Fields);
        }
    }
}
