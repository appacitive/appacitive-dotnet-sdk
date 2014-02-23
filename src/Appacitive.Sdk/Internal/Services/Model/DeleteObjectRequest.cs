using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Services
{
    public class DeleteObjectRequest : DeleteOperation<DeleteObjectResponse>
    {
        public string Type { get; set; }

        public string Id { get; set; }

        public bool DeleteConnections { get; set; }

        public override byte[] ToBytes()
        {
            return null;
        }

        protected override string GetUrl()
        {
            return Urls.For.DeleteObject(this.Type, this.Id, this.DeleteConnections, this.CurrentLocation, this.DebugEnabled, this.Verbosity, this.Fields);
        }
    }
}
