using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Services
{
    public class FindAllConnectionsRequest : GetOperation<FindAllConnectionsResponse>
    {
        public string Type { get; set; }

        public string Query { get; set; }

        public string FreeTextExpression { get; set; }

        public string OrderBy { get; set; }

        public SortOrder SortOrder { get; set; }

        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        protected override string GetUrl()
        {
            return Urls.For.FindAllConnectionsAsync(this.Type, this.FreeTextExpression, this.Query, this.PageNumber, this.PageSize, this.OrderBy, this.SortOrder, this.CurrentLocation, this.DebugEnabled, this.Verbosity, this.Fields);
        }
    }
}
