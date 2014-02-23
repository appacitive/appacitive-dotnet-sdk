using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Services
{
    public class DeleteConnectionRequest : DeleteOperation<DeleteConnectionResponse>
    {
        public string Relation { get; set; }

        public string Id { get; set; }

        protected override string GetUrl()
        {
            return Urls.For.GetConnection(this.Relation, this.Id, this.CurrentLocation, this.DebugEnabled, this.Verbosity, this.Fields);
        }
    }
}
