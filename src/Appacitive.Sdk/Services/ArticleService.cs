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

        public GetArticleResponse GetArticle(GetArticleRequest request)
        {
            byte[] bytes = null;
            var timeTaken = Measure.TimeFor(() =>
                {
                    bytes = HttpClient
                        .WithUrl(Urls.For.GetArticle(request.Type, request.Id, request.CurrentLocation, request.DebugEnabled, request.Verbosity))
                        .WithAppacitiveSession(request.SessionToken)
                        .WithEnvironment(request.Environment)
                        .WithUserToken(request.UserToken)
                        .Get();
                });
            var response = GetArticleResponse.Parse(bytes);
            response.TimeTaken = timeTaken;
            return response;
        }

        public async Task<GetArticleResponse> GetArticleAsync(GetArticleRequest request)
        {
            byte[] bytes = null;
            var timer = Stopwatch.StartNew();
            bytes = await HttpClient
                .WithUrl(Urls.For.GetArticle(request.Type, request.Id, request.CurrentLocation, request.DebugEnabled, request.Verbosity))
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

        public CreateArticleResponse CreateArticle(CreateArticleRequest request)
        {
            byte[] bytes = null;

            bytes = HttpClient
                        .WithUrl(Urls.For.CreateArticle(request.Article.Type, request.CurrentLocation, request.DebugEnabled, request.Verbosity))
                        .WithAppacitiveSession(request.SessionToken)
                        .WithEnvironment(request.Environment)
                        .WithUserToken(request.UserToken)
                        .Put(request.ToBytes());
            var response = CreateArticleResponse.Parse(bytes);
            return response;
        }

        public async Task<CreateArticleResponse> CreateArticleAsync(CreateArticleRequest request)
        {
            var timer = Stopwatch.StartNew();
            var bytes = await HttpClient
                            .WithUrl(Urls.For.CreateArticle(request.Article.Type, request.CurrentLocation, request.DebugEnabled, request.Verbosity))
                            .WithAppacitiveSession(request.SessionToken)
                            .WithEnvironment(request.Environment)
                            .WithUserToken(request.UserToken)
                            .PutAsyc(request.ToBytes());
            timer.Stop();
            var response = CreateArticleResponse.Parse(bytes);
            response.TimeTaken = (decimal)timer.ElapsedTicks / Stopwatch.Frequency;
            return response;
        }


        public Status DeleteArticle(DeleteArticleRequest request)
        {
            byte[] bytes = null;
            bytes = HttpClient
                .WithUrl(Urls.For.DeleteArticle(request.Type, request.Id, request.CurrentLocation, request.DebugEnabled, request.Verbosity))
                .WithAppacitiveSession(request.SessionToken)
                .WithEnvironment(request.Environment)
                .WithUserToken(request.UserToken)
                .Delete(request.ToBytes());
            var response = Status.Parse(bytes);
            return response;
        }

        public async Task<Status> DeleteArticleAsync(DeleteArticleRequest request)
        {
            byte[] bytes = null;
            bytes = await HttpClient
                .WithUrl(Urls.For.DeleteArticle(request.Type, request.Id, request.CurrentLocation, request.DebugEnabled, request.Verbosity))
                .WithAppacitiveSession(request.SessionToken)
                .WithEnvironment(request.Environment)
                .WithUserToken(request.UserToken)
                .DeleteAsync(request.ToBytes());
            var response = Status.Parse(bytes);
            return response;
        }


        public UpdateArticleResponse UpdateArticle(UpdateArticleRequest request)
        {
            byte[] bytes = null;
            bytes = HttpClient
                        .WithUrl(Urls.For.UpdateArticle(request.Type, request.Id, request.CurrentLocation, request.DebugEnabled, request.Verbosity))
                        .WithAppacitiveSession(request.SessionToken)
                        .WithEnvironment(request.Environment)
                        .WithUserToken(request.UserToken)
                        .Post(request.ToBytes());
            var response = UpdateArticleResponse.Parse(bytes);
            return response;
        }

        public async Task<UpdateArticleResponse> UpdateArticleAsync(UpdateArticleRequest request)
        {
            byte[] bytes = null;
            bytes = await HttpClient
                        .WithUrl(Urls.For.UpdateArticle(request.Type, request.Id, request.CurrentLocation, request.DebugEnabled, request.Verbosity))
                        .WithAppacitiveSession(request.SessionToken)
                        .WithEnvironment(request.Environment)
                        .WithUserToken(request.UserToken)
                        .PostAsyc(request.ToBytes());
            var response = UpdateArticleResponse.Parse(bytes);
            return response;
        }
        
    }
}
