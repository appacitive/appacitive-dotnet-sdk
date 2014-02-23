using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Services
{
    public class DeleteDeviceRequest : DeleteOperation<DeleteObjectResponse>
    {
        public string Id { get; set; }

        protected override string GetUrl()
        {
            return Urls.For.GetDevice(this.Id, this.CurrentLocation, this.DebugEnabled, this.Verbosity, this.Fields);
        }
    }
}
