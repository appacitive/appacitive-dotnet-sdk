using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
        
        public Article(Article existing) : base(existing.Type, existing.Id)
        {
            // Copy
            this.CreatedBy = existing.CreatedBy;
            this.LastUpdatedBy = existing.LastUpdatedBy;
            this.UtcCreateDate = existing.UtcCreateDate;
            this.UtcLastUpdated = existing.UtcLastUpdated;
            this.SchemaId = existing.SchemaId;
            
            // Copy properties
            foreach (var property in existing.Properties)
                this[property.Key] = property.Value;
            foreach (var attr in existing.Attributes)
                this.SetAttribute(attr.Key, attr.Value);
        }

        public Article(string type) : base(type)
        {   
        }

        public Article(string type, string id)
            : base(type, id)
        {
        }

        internal string SchemaId { get; set; }

        internal bool IsNewInstance
        {
            get { return string.IsNullOrWhiteSpace(this.Id) == true || this.Id == "0"; }
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

        public async static Task<IEnumerable<Article>> MultiGetAsync(string type, IEnumerable<string> idList, IEnumerable<string> fields = null)
        {
            var service = ObjectFactory.Build<IArticleService>();
            var request = new MultiGetArticleRequest() { Type = type, };
            request.IdList.AddRange(idList);
            if (fields != null)
                request.Fields.AddRange(fields);
            var response = await service.MultiGetArticleAsync(request);
            if (response.Status.IsSuccessful == false)
                throw response.Status.ToFault();
            return response.Articles;
        }

        public async static Task<PagedList<Article>> FindAllAsync(string type, string query = null, IEnumerable<string> fields = null, int page = 1, int pageSize = 20, string orderBy = null, SortOrder sortOrder = SortOrder.Descending )
        {
            var service = ObjectFactory.Build<IArticleService>();
            var request = new FindAllArticleRequest()
            {
                Type = type,
                Query = query,
                PageNumber = page,
                PageSize = pageSize,
                OrderBy = orderBy,
                SortOrder = sortOrder
            };
            var response = await service.FindAllAsync(request);
            if (response.Status.IsSuccessful == false)
                throw response.Status.ToFault();
            var articles = new PagedList<Article>()
            {
                PageNumber = response.PagingInfo.PageNumber,
                PageSize = response.PagingInfo.PageSize,
                TotalRecords = response.PagingInfo.TotalRecords,
                GetNextPage = async skip => await FindAllAsync(type, query, fields, page+skip+1, pageSize, orderBy, sortOrder)
            };
            articles.AddRange(response.Articles);
            return articles;
            
        }

        public async static Task DeleteAsync(string type, string id, bool deleteConnections = false)
        {
            var service = ObjectFactory.Build<IArticleService>();
            var status = await service.DeleteArticleAsync(new DeleteArticleRequest()
            { 
                Id = id, 
                Type = type,
                DeleteConnections = deleteConnections
            });
            if (status.IsSuccessful == false)
                throw status.ToFault();
        }

        public async static Task MultiDeleteAsync(string type, params string[] ids)
        {
            var service = ObjectFactory.Build<IArticleService>();
            var response = await service.BulkDeleteAsync(
                new BulkDeleteArticleRequest
                {
                    Type = type,
                    ArticleIds = ids.ToList()
                });
            if (response.Status.IsSuccessful == false)
                throw response.Status.ToFault();
        }

        protected override async Task<Entity> UpdateAsync(IDictionary<string, string> propertyUpdates, IDictionary<string, string> attributeUpdates, IEnumerable<string> addedTags, IEnumerable<string> removedTags, int specificRevision)
        {
            var articleService = ObjectFactory.Build<IArticleService>();
            var request = new UpdateArticleRequest {Id = this.Id, Type = this.Type};
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

        public async Task<PagedList<Article>> GetConnectedArticlesAsync(string relation, string query = null, IEnumerable<string> fields = null, int pageNumber = 1, int pageSize = 20)
        {
            return await Article.GetConnectedArticlesAsync(relation, this.Id, query, fields, pageNumber, pageSize);
        }

        public async static Task<PagedList<Article>> GetConnectedArticlesAsync(string relation, string articleId, string query = null, IEnumerable<string> fields = null, int pageNumber = 1, int pageSize = 20)
        {
            IConnectionService connService = ObjectFactory.Build<IConnectionService>();
            var request = new FindConnectedArticlesRequest
            {
                Relation = relation,
                ArticleId = articleId,
                Query = query,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            if (fields != null)
                request.Fields.AddRange(fields);
            var response = await connService.FindConnectedArticlesAsync(request);
            if (response.Status.IsSuccessful == false)
                throw response.Status.ToFault();

            var articles = response.Connections.Select(c =>
                {
                    if (c.EndpointA.ArticleId == articleId)
                        return c.EndpointB.Content;
                    else 
                        return c.EndpointA.Content;
                });

            var list = new PagedList<Article>()
            {
                PageNumber = response.PagingInfo.PageNumber,
                PageSize = response.PagingInfo.PageSize,
                TotalRecords = response.PagingInfo.TotalRecords,
                GetNextPage = async skip => await GetConnectedArticlesAsync(relation, articleId, query, fields, pageNumber + skip + 1, pageSize)
            };
            list.AddRange(articles);
            return list;
        }

        public async Task<PagedList<Connection>> GetConnectionsAsync(string relation, string query = null, IEnumerable<string> fields = null, int pageNumber = 1, int pageSize = 20)
        {
            return await Article.GetConnectionsAsync(relation, this.Id, query, fields, pageNumber, pageSize);
        }

        public async static Task<PagedList<Connection>> GetConnectionsAsync(string relation, string articleId, string query = null, IEnumerable<string> fields = null, int pageNumber = 1, int pageSize = 20)
        {
            IConnectionService connService = ObjectFactory.Build<IConnectionService>();
            var request = new FindConnectedArticlesRequest
            {
                Relation = relation,
                ArticleId = articleId,
                Query = query,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            if (fields != null)
                request.Fields.AddRange(fields);
            var response = await connService.FindConnectedArticlesAsync(request);
            if (response.Status.IsSuccessful == false)
                throw response.Status.ToFault();

            var list = new PagedList<Connection>
            {
                PageNumber = response.PagingInfo.PageNumber,
                PageSize = response.PagingInfo.PageSize,
                TotalRecords = response.PagingInfo.TotalRecords,
                GetNextPage = async skip => await GetConnectionsAsync(relation, articleId, query, fields, pageNumber + skip + 1, pageSize)
            };
            list.AddRange(response.Connections);
            return list;
        }
    }
}