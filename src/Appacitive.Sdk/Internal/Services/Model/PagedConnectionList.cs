using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Services.Model
{
    public class PagedConnectionList : List<APConnection>
    {
        internal string Type { get; set; }

        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public int TotalRecords { get; set; }

        internal Func<int, Task<PagedConnectionList>> GetNextPage { get; set; }

        public bool IsLastPage
        {
            get
            {
                var currentIndex = (this.PageNumber - 1) * PageSize + this.Count;
                return currentIndex > (this.TotalRecords / this.PageSize) * this.PageSize;
            }
        }

        public async Task<PagedConnectionList> NextPageAsync(int skipPages = 0)
        {
            return await GetNextPage(skipPages);
        }
    }
}
