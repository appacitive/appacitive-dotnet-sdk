using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Services
{
    public class ArticleService : IArticleService
    {
        internal static ArticleService Instance = new ArticleService();

        public async Task<GetArticleResponse> GetArticleAsync(GetArticleRequest request)
        {
            byte[] bytes = null;
            bytes = await HttpOperation
                .WithUrl(Urls.For.GetArticle(request.Type, request.Id, request.CurrentLocation, request.DebugEnabled, request.Verbosity, request.Fields))
                .WithAppacitiveSession(request.SessionToken)
                .WithEnvironment(request.Environment)
                .WithUserToken(request.UserToken)
                .GetAsync();
            var response = GetArticleResponse.Parse(bytes);
            return response;
        }

        public async Task<CreateArticleResponse> CreateArticleAsync(CreateArticleRequest request)
        {
            
            var bytes = await HttpOperation
                            .WithUrl(Urls.For.CreateArticle(request.Article.Type, request.CurrentLocation, request.DebugEnabled, request.Verbosity, request.Fields))
                            .WithAppacitiveSession(request.SessionToken)
                            .WithEnvironment(request.Environment)
                            .WithUserToken(request.UserToken)
                            .PutAsyc(request.ToBytes());
            
            var response = CreateArticleResponse.Parse(bytes);
            return response;
        }

        public async Task<Status> DeleteArticleAsync(DeleteArticleRequest request)
        {
            byte[] bytes = null;
            bytes = await HttpOperation
                .WithUrl(Urls.For.DeleteArticle(request.Type, request.Id, request.CurrentLocation, request.DebugEnabled, request.Verbosity, request.Fields))
                .WithAppacitiveSession(request.SessionToken)
                .WithEnvironment(request.Environment)
                .WithUserToken(request.UserToken)
                .DeleteAsync();
            var response = Status.Parse(bytes);
            return response;
        }

        public async Task<UpdateArticleResponse> UpdateArticleAsync(UpdateArticleRequest request)
        {
            byte[] bytes = null;
            bytes = await HttpOperation
                        .WithUrl(Urls.For.UpdateArticle(request.Type, request.Id, request.CurrentLocation, request.DebugEnabled, request.Verbosity, request.Fields))
                        .WithAppacitiveSession(request.SessionToken)
                        .WithEnvironment(request.Environment)
                        .WithUserToken(request.UserToken)
                        .PostAsyc(request.ToBytes());
            var response = UpdateArticleResponse.Parse(bytes);
            return response;
        }

        public async Task<FindAllArticleResponse> FindAllAsync(FindAllArticleRequest request)
        {
            byte[] bytes = null;
            bytes = await HttpOperation
                        .WithUrl(Urls.For.FindAllArticles(request.Type, request.Query, request.PageNumber, request.PageSize, request.CurrentLocation, request.DebugEnabled, request.Verbosity, request.Fields))
                        .WithAppacitiveSession(request.SessionToken)
                        .WithEnvironment(request.Environment)
                        .WithUserToken(request.UserToken)
                        .GetAsync();
            var response = FindAllArticleResponse.Parse(bytes);
            return response;
        }

        public async Task<MultiGetArticleResponse> MultiGetArticleAsync(MultiGetArticleRequest request)
        {
            byte[] bytes = null;
            bytes = await HttpOperation
                .WithUrl(Urls.For.MultiGetArticle(request.Type, request.IdList, request.CurrentLocation, request.DebugEnabled, request.Verbosity, request.Fields))
                .WithAppacitiveSession(request.SessionToken)
                .WithEnvironment(request.Environment)
                .WithUserToken(request.UserToken)
                .GetAsync();
            var response = MultiGetArticleResponse.Parse(bytes);
            return response;
        }
    }
}
