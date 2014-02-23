using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Services
{
    public class GetListContentRequest : GetOperation<GetListContentResponse>
    {
        public string Name { get; set; }

        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public override byte[] ToBytes()
        {
            return null;
        }

        protected override string GetUrl()
        {
            return Urls.For.GetListContent(this.Name, this.PageNumber, this.PageSize, this.CurrentLocation, this.DebugEnabled, this.Verbosity, this.Fields);
        }
    }
}
