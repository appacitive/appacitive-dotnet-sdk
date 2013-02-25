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

        public static Article Get(string type, string id)
        {
            var service = ObjectFactory.Build<IArticleService>();
            var response = service.GetArticle(new GetArticleRequest() 
            {
                Id = id, 
                Type = type 
            });
            if (response.Status.IsSuccessful == false)
                throw response.Status.ToFault();
            Debug.Assert(response.Article != null, "For a successful get call, article should always be returned.");
            return response.Article;
        }

        public async static Task<Article> GetAsync(string type, string id)
        {
            var service = ObjectFactory.Build<IArticleService>();
            var response = await service.GetArticleAsync(new GetArticleRequest()
            {
                Id = id, 
                Type = type 
            });
            if (response.Status.IsSuccessful == false)
                throw response.Status.ToFault();
            Debug.Assert(response.Article != null, "For a successful get call, article should always be returned.");
            return response.Article;
        }

        public async static Task<PagedArticleList> FindAllAsync(string type, int page = 1, int pageSize = 20)
        {
            string query = null;
            return await Article.FindAllAsync(type, query, page, pageSize);
        }

        public async static Task<PagedArticleList> FindAllAsync(string type, IQuery query, int page = 1, int pageSize = 20)
        {
            return await Article.FindAllAsync(type, query.AsString(), page, pageSize);
        }

        public async static Task<PagedArticleList> FindAllAsync(string type, string query, int page = 1, int pageSize = 20)
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
                Query = query,
                ArticleType = type
            };
            articles.AddRange(response.Articles);
            return articles;
            
        }

        public static void Delete(string type, string id)
        {
            var service = ObjectFactory.Build<IArticleService>();
            var status = service.DeleteArticle(new DeleteArticleRequest()
            {
                Id = id, 
                Type = type 
            });
            if (status.IsSuccessful == false)
                throw status.ToFault();
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

        protected override Entity Update(IDictionary<string, string> propertyUpdates, IDictionary<string, string> attributeUpdates, IEnumerable<string> addedTags, IEnumerable<string> removedTags)
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

            if( propertyUpdates != null && propertyUpdates.Count > 0 )
                propertyUpdates.For(x => request.PropertyUpdates[x.Key] = x.Value);
            if (attributeUpdates != null && attributeUpdates.Count > 0)
                attributeUpdates.For(x => request.AttributeUpdates[x.Key] = x.Value);
            if( addedTags != null )
                request.AddedTags.AddRange(addedTags);
            if( removedTags != null )
                request.RemovedTags.AddRange(removedTags);

            // Check if an update is needed.
            if (request.PropertyUpdates.Count == 0 &&
                request.AttributeUpdates.Count == 0 &&
                request.AddedTags.Count == 0 &&
                request.RemovedTags.Count == 0)
                return null;

            var response = articleService.UpdateArticle(request);
            if (response.Status.IsSuccessful == false)
                throw response.Status.ToFault();

            // 3. Update the last known state based on the differences
            Debug.Assert(response.Article != null, "If status is successful, then updated article should not be null.");
            return response.Article;
        }

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

        protected override Entity CreateNew()
        {
            // Create a new article
            var service = ObjectFactory.Build<IArticleService>();
            var response = service.CreateArticle(new CreateArticleRequest() 
                {
                    Article = this
                });
            if (response.Status.IsSuccessful == false)
                throw response.Status.ToFault();

            // 3. Update the last known state based on the differences
            
            Debug.Assert(response.Article != null, "If status is successful, then created article should not be null.");
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

        
    }
}