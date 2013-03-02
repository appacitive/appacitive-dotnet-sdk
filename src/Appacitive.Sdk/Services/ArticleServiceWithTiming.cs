using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Services
{
    public class ArticleServiceWithTiming : IArticleService
    {
        public ArticleServiceWithTiming( IArticleService inner )
        {
            this.Inner = inner;
        }

        public IArticleService Inner { get; set; }

        public Task<GetArticleResponse> GetArticleAsync(GetArticleRequest request)
        {
            return ExecuteWithTiming(request, this.Inner.GetArticleAsync);
        }

        
        public Task<CreateArticleResponse> CreateArticleAsync(CreateArticleRequest request)
        {
            return ExecuteWithTiming(request, this.Inner.CreateArticleAsync);
        }

        private TRs ExecuteWithTiming<TRq, TRs>(TRq request, Func<TRq, TRs> api)
            where TRs : ApiResponse
            where TRq : ApiRequest
        {
            TRs response = null;
            var timeTaken = Measure.TimeFor(() =>
            {
                response = api(request);
            });
            response.TimeTaken = timeTaken;
            return response;
        }

        public Task<TRs> ExecuteWithTiming<TRq, TRs>(TRq request, Func<TRq, Task<TRs>> asyncApi)
            where TRs : ApiResponse
            where TRq : ApiRequest
        {
            var timer = Stopwatch.StartNew();
            var task = asyncApi(request);
            return task.ContinueWith(t =>
            {
                TRs response = t.Result;
                response.TimeTaken = (decimal)timer.ElapsedTicks / Stopwatch.Frequency;
                return t.Result;
            });
        }
        
        public Task<Status> DeleteArticleAsync(DeleteArticleRequest request)
        {
            return this.Inner.DeleteArticleAsync(request);
        }


        public Task<UpdateArticleResponse> UpdateArticleAsync(UpdateArticleRequest request)
        {
            return ExecuteWithTiming(request, this.Inner.UpdateArticleAsync);
        }


        public Task<FindAllArticleResponse> FindAllAsync(FindAllArticleRequest request)
        {
            return ExecuteWithTiming(request, this.Inner.FindAllAsync);
        }
    }
}
