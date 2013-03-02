using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Appacitive.Sdk.Services;

namespace Appacitive.Sdk
{
    public partial class Article : Entity, INotifyPropertyChanged
    {
        public Article(string type) : base(type)
        {   
        }

        public Article(string type, string id)
            : base(type, id)
        {
        }

        public static readonly IEnumerable<string> AllFields = new string[0];

        public async static Task<Article> GetAsync(string type, string id, IEnumerable<string> fields = null)
        {
            var service = ObjectFactory.Build<IArticleService>();
            var request = new GetArticleRequest() { Id = id, Type = type, };
            if( fields != null )
                request.Fields.AddRange(fields);
            var response = await service.GetArticleAsync(request);
            if (response.Status.IsSuccessful == false)
                throw response.Status.ToFault();
            Debug.Assert(response.Article != null, "For a successful get call, article should always be returned.");
            return response.Article;
        }

        public async static Task<PagedArticleList> FindAllAsync(string type, string query = null, IEnumerable<string> fields = null, int page = 1, int pageSize = 20)
        {
            var service = ObjectFactory.Build<IArticleService>();
            var request = new FindAllArticleRequest() { Type = type, Query = query, PageNumber = page, PageSize = pageSize };
            var response = await service.FindAllAsync(request);
            if (response.Status.IsSuccessful == false)
                throw response.Status.ToFault();
            var articles = new PagedArticleList()
            {
                PageNumber = response.PagingInfo.PageNumber,
                PageSize = response.PagingInfo.PageSize,
                TotalRecords = response.PagingInfo.TotalRecords,
                GetNextPage = async skip => await FindAllAsync(type, query, fields, page+skip+1, pageSize)
            };
            articles.AddRange(response.Articles);
            return articles;
            
        }

        public async static Task DeleteAsync(string type, string id)
        {
            var service = ObjectFactory.Build<IArticleService>();
            var status = await service.DeleteArticleAsync(new DeleteArticleRequest()
            { 
                Id = id, 
                Type = type 
            });
            if (status.IsSuccessful == false)
                throw status.ToFault();
        }


        internal string SchemaId { get; set; }

        internal bool IsNewInstance
        {
            get { return string.IsNullOrWhiteSpace(this.Id) == true || this.Id == "0"; }
        }

        protected override async Task<Entity> UpdateAsync(IDictionary<string, string> propertyUpdates, IDictionary<string, string> attributeUpdates, IEnumerable<string> addedTags, IEnumerable<string> removedTags)
        {
            var articleService = ObjectFactory.Build<IArticleService>();
            var request = new UpdateArticleRequest()
            {
                SessionToken = AppacitiveContext.SessionToken,
                Environment = AppacitiveContext.Environment,
                UserToken = AppacitiveContext.UserToken,
                Verbosity = AppacitiveContext.Verbosity,
                Id = this.Id,
                Type = this.Type
            };

            if (propertyUpdates != null && propertyUpdates.Count > 0)
                propertyUpdates.For(x => request.PropertyUpdates[x.Key] = x.Value);
            if (attributeUpdates != null && attributeUpdates.Count > 0)
                attributeUpdates.For(x => request.AttributeUpdates[x.Key] = x.Value);
            if (addedTags != null)
                request.AddedTags.AddRange(addedTags);
            if (removedTags != null)
                request.RemovedTags.AddRange(removedTags);

            // Check if an update is needed.
            if (request.PropertyUpdates.Count == 0 &&
                request.AttributeUpdates.Count == 0 &&
                request.AddedTags.Count == 0 &&
                request.RemovedTags.Count == 0)
                return null;

            var response = await articleService.UpdateArticleAsync(request);
            if (response.Status.IsSuccessful == false)
                throw response.Status.ToFault();

            // 3. Update the last known state based on the differences
            Debug.Assert(response.Article != null, "If status is successful, then updated article should not be null.");
            return response.Article;
        }

        protected override async Task<Entity> CreateNewAsync()
        {
            // Create a new article
            var service = ObjectFactory.Build<IArticleService>();
            var response = await service.CreateArticleAsync(new CreateArticleRequest() 
            {
                Article = this
            });
            if (response.Status.IsSuccessful == false)
                throw response.Status.ToFault();

            // 3. Update the last known state based on the differences
            Debug.Assert(response.Article != null, "If status is successful, then created article should not be null.");
            return response.Article;
        }

        public async Task<PagedArticleList> GetConnectedArticlesAsync(string relation, IQuery query = null, int pageNumber = 1, int pageSize = 20)
        {
            string queryString = query == null ? null : query.ToString();
            return await Article.GetConnectedArticlesAsync(relation, this.Id, queryString, pageNumber, pageSize);
        }

        public async Task<PagedArticleList> GetConnectedArticlesAsync(string relation, string query, int pageNumber = 1, int pageSize = 20)
        {
            return await Article.GetConnectedArticlesAsync(relation, this.Id, query, pageNumber, pageSize);
        }

        public async static Task<PagedArticleList> GetConnectedArticlesAsync(string relation, string articleId, IQuery query = null, int pageNumber = 1, int pageSize = 20)
        {
            string queryString = query == null ? null : query.ToString();
            return await Article.GetConnectedArticlesAsync(relation, articleId, queryString, pageNumber, pageSize);
        }

        public async static Task<PagedArticleList> GetConnectedArticlesAsync(string relation, string articleId, string query = null, int pageNumber = 1, int pageSize = 20)
        {
            IConnectionService connService = ObjectFactory.Build<IConnectionService>();
            var response = await connService.FindConnectedArticlesAsync(
                new FindConnectedArticlesRequest
                {
                    Relation = relation, ArticleId = articleId, Query = query,
                    PageNumber = pageNumber, PageSize = pageSize
                });
            if (response.Status.IsSuccessful == false)
                throw response.Status.ToFault();

            Func<int, Task<PagedArticleList>> nextPage = async skip => 
                {
                    return await GetConnectedArticlesAsync(relation, articleId, query, pageNumber + skip + 1, pageSize);
                };

            var articles = response.Connections.Select(c =>
                {
                    if (c.EndpointA.ArticleId == articleId)
                        return c.EndpointB.Content;
                    else 
                        return c.EndpointA.Content;
                });

            var list = new PagedArticleList()
            {
                PageNumber = response.PagingInfo.PageNumber,
                PageSize = response.PagingInfo.PageSize,
                TotalRecords = response.PagingInfo.TotalRecords,
                GetNextPage = nextPage
            };
            list.AddRange(articles);
            return list;
        }
    }
}