using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Services
{
    public class PagedArticleList : List<Article>
    {
        internal string ArticleType { get; set; }

        internal string Query { get; set; }

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

        public async Task<PagedArticleList> NextPageAsync()
        {
            return await Article.FindAllAsync(this.ArticleType, this.Query, this.PageNumber + 1, this.PageSize);
        }
    }
}
