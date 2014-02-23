using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Services
{
    public class MultiGetObjectsRequest : GetOperation<MultiGetObjectsResponse>
    {
        public MultiGetObjectsRequest() : base()
        {
            this.IdList = new List<string>();
        }

        public string Type { get; set; }

        public List<string> IdList { get; private set; }

        protected override string GetUrl()
        {
            return Urls.For.MultiGetObjects(this.Type, this.IdList, this.CurrentLocation, this.DebugEnabled, this.Verbosity, this.Fields);
        }
    }
}
