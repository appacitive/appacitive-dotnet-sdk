using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Appacitive.Sdk.Services
{
    public class AtomicCountersRequest : PostOperation<UpdateObjectResponse>
    {
        public AtomicCountersRequest()
            : base()
        {   
        }

        public string Property { get; set; }

        public string Type { get; set; }

        public string Id { get; set; }

        public uint IncrementBy { get; set; }

        public uint DecrementBy { get; set; }

        protected override string GetUrl()
        {
            this.Fields.Add(this.Property);
            return Urls.For.UpdateObject(this.Type, this.Id, 0, false, this.CurrentLocation, this.DebugEnabled, this.Verbosity, this.Fields);
        }
    }
}
