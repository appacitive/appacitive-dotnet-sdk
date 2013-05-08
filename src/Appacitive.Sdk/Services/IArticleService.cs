using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Services
{
    public interface IArticleService
    {
        Task<GetArticleResponse> GetArticleAsync(GetArticleRequest request);

        Task<MultiGetArticleResponse> MultiGetArticleAsync(MultiGetArticleRequest request);

        Task<CreateArticleResponse> CreateArticleAsync(CreateArticleRequest request);

        Task<Status> DeleteArticleAsync(DeleteArticleRequest request);

        Task<UpdateArticleResponse> UpdateArticleAsync(UpdateArticleRequest request);

        Task<FindAllArticleResponse> FindAllAsync(FindAllArticleRequest request);

        Task<BulkDeleteArticleResponse> BulkDeleteAsync(BulkDeleteArticleRequest request);
 
        
    }

    
}
