using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Services
{
    public class PagedUserList : List<User>
    {
        internal string Query { get; set; }

        public int PageNumber { get; set; }

        internal Func<int, Task<PagedUserList>> GetNextPage { get; set; }

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

        public async Task<PagedUserList> NextPageAsync(int skip = 0)
        {
            return await GetNextPage(skip);
            // return await Article.FindAllAsync(this.ArticleType, this.Query, this.PageNumber + 1, this.PageSize);
        }
    }
}
