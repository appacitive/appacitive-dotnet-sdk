using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Appacitive.Sdk.Services
{
    public class UpdateListItemsRequest : PostOperation<UpdateObjectResponse>
    {
        public UpdateListItemsRequest()
            : base()
        {
            this.ItemsToAdd = new List<string>();
            this.ItemsToRemove = new List<string>();
        }

        public string Property { get; set; }

        public string Type { get; set; }

        public string Id { get; set; }

        public bool AddUniquely { get; set;}

        public List<string> ItemsToAdd { get; private set; }

        public List<string> ItemsToRemove { get; private set; }

        protected override string GetUrl()
        {
            this.Fields.Add(this.Property);
            return Urls.For.UpdateObject(this.Type, this.Id, 0, false, this.CurrentLocation, this.DebugEnabled, this.Verbosity, this.Fields);
        }
    }
}
