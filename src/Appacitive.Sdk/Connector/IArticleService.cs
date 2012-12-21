using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Connector
{
    public interface IArticleService
    {
        GetArticleResponse GetArticle(GetArticleRequest request);

        Task<GetArticleResponse> GetArticleAsync(GetArticleRequest request);
    }

    
}
