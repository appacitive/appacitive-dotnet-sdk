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
            var timer = Stopwatch.StartNew();
            bytes = await HttpClient
                .WithUrl(Urls.For.GetArticle(request.Type, request.Id, request.CurrentLocation, request.DebugEnabled, request.Verbosity, request.Fields))
                .WithAppacitiveSession(request.SessionToken)
                .WithEnvironment(request.Environment)
                .WithUserToken(request.UserToken)
                .GetAsync();
            timer.Stop();
            decimal timeTaken = (decimal)timer.ElapsedTicks / Stopwatch.Frequency;
            var response = GetArticleResponse.Parse(bytes);
            response.TimeTaken = timeTaken;
            return response;
        }

        public async Task<CreateArticleResponse> CreateArticleAsync(CreateArticleRequest request)
        {
            var timer = Stopwatch.StartNew();
            var bytes = await HttpClient
                            .WithUrl(Urls.For.CreateArticle(request.Article.Type, request.CurrentLocation, request.DebugEnabled, request.Verbosity, request.Fields))
                            .WithAppacitiveSession(request.SessionToken)
                            .WithEnvironment(request.Environment)
                            .WithUserToken(request.UserToken)
                            .PutAsyc(request.ToBytes());
            timer.Stop();
            var response = CreateArticleResponse.Parse(bytes);
            response.TimeTaken = (decimal)timer.ElapsedTicks / Stopwatch.Frequency;
            return response;
        }

        public async Task<Status> DeleteArticleAsync(DeleteArticleRequest request)
        {
            byte[] bytes = null;
            bytes = await HttpClient
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
            bytes = await HttpClient
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
            bytes = await HttpClient
                        .WithUrl(Urls.For.FindAllArticles(request.Type, request.Query, request.PageNumber, request.PageSize, request.CurrentLocation, request.DebugEnabled, request.Verbosity, request.Fields))
                        .WithAppacitiveSession(request.SessionToken)
                        .WithEnvironment(request.Environment)
                        .WithUserToken(request.UserToken)
                        .GetAsync();
            var response = FindAllArticleResponse.Parse(bytes);
            return response;
        }
    }
}
