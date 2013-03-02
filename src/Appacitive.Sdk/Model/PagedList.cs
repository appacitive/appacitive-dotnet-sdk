using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk
{
    public class PagedList<T> : List<T>
    {
        internal Func<int, Task<PagedList<T>>> GetNextPage { get; set; }

        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public int TotalRecords { get; set; }

        public bool IsLastPage
        {
            get
            {
                var currentIndex = (this.PageNumber - 1) * PageSize + this.Count;
                return currentIndex > (this.TotalRecords / this.PageSize) * this.PageSize;
            }
        }

        public async Task<PagedList<T>> NextPageAsync(int skip = 0)
        {
            return await GetNextPage(skip);
        }

    }
}
