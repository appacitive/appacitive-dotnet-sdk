using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Services
{
    public interface IArticleService
    {
        GetArticleResponse GetArticle(GetArticleRequest request);

        Task<GetArticleResponse> GetArticleAsync(GetArticleRequest request);

        CreateArticleResponse CreateArticle(CreateArticleRequest request);

        Task<CreateArticleResponse> CreateArticleAsync(CreateArticleRequest request);

        Status DeleteArticle(DeleteArticleRequest request);

        Task<Status> DeleteArticleAsync(DeleteArticleRequest request);

        UpdateArticleResponse UpdateArticle(UpdateArticleRequest request);

        Task<UpdateArticleResponse> UpdateArticleAsync(UpdateArticleRequest request);
    }

    
}
