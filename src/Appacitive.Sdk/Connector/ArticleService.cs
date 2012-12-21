using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Connector
{
    public class ArticleService : IArticleService
    {
        public GetArticleResponse GetArticle(GetArticleRequest request)
        {
            var bytes = HttpClient
                .WithUrl(Urls.For.GetArticle(request.Type, request.Id, request.CurrentLocation, request.DebugEnabled, request.Verbosity))
                .WithHeader("Appacitive-Session", AppacitiveContext.SessionToken)
                .WithHeader("Appacitive-Environment", AppacitiveContext.Environment.ToString())
                .WithHeader("Appacitive-Auth", AppacitiveContext.UserToken)
                .Get();
            return GetArticleResponse.Parse(bytes);
        }

        public async Task<GetArticleResponse> GetArticleAsync(GetArticleRequest request)
        {
            var bytes = await HttpClient
                .WithUrl(Urls.For.GetArticle(request.Type, request.Id, request.CurrentLocation, request.DebugEnabled, request.Verbosity))
                .WithHeader("Appacitive-Session", AppacitiveContext.SessionToken)
                .WithHeader("Appacitive-Environment", AppacitiveContext.Environment.ToString())
                .WithHeader("Appacitive-Auth", AppacitiveContext.UserToken)
                .GetAsync();
            return GetArticleResponse.Parse(bytes);
        }
    }

    
}
