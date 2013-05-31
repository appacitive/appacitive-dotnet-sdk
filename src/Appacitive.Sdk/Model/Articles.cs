using Appacitive.Sdk.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk
{
    public static partial class Articles
    {
        public async static Task<Article> GetAsync(string type, string id, IEnumerable<string> fields = null)
        {
            var service = ObjectFactory.Build<IArticleService>();
            var request = new GetArticleRequest() { Id = id, Type = type, };
            if (fields != null)
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

        public async static Task<PagedList<Article>> FindAllAsync(string type, string query = null, IEnumerable<string> fields = null, int page = 1, int pageSize = 20, string orderBy = null, SortOrder sortOrder = SortOrder.Descending)
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
                GetNextPage = async skip => await FindAllAsync(type, query, fields, page + skip + 1, pageSize, orderBy, sortOrder)
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

        public async static Task<PagedList<Article>> GetConnectedArticlesAsync(string relation, string articleId, string query = null, string label = null, IEnumerable<string> fields = null, int pageNumber = 1, int pageSize = 20)
        {
            IConnectionService connService = ObjectFactory.Build<IConnectionService>();
            var request = new FindConnectedArticlesRequest
            {
                Relation = relation,
                ArticleId = articleId,
                Label = label,
                Query = query,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            if (fields != null)
                request.Fields.AddRange(fields);
            var response = await connService.FindConnectedArticlesAsync(request);
            if (response.Status.IsSuccessful == false)
                throw response.Status.ToFault();

            IEnumerable<Article> articles = null;
            // if label is specified, then get the labelled endpoint
            if (string.IsNullOrWhiteSpace(label) == false)
                articles = response.Connections.Select(c => c.Endpoints[label].Content);
            else
                articles = response.Connections.Select(c =>
                {
                    if (c.Endpoints.EndpointA.ArticleId == articleId)
                        return c.Endpoints.EndpointB.Content;
                    else
                        return c.Endpoints.EndpointA.Content;
                });

            var list = new PagedList<Article>()
            {
                PageNumber = response.PagingInfo.PageNumber,
                PageSize = response.PagingInfo.PageSize,
                TotalRecords = response.PagingInfo.TotalRecords,
                GetNextPage = async skip => await GetConnectedArticlesAsync(relation, articleId, query, label, fields, pageNumber + skip + 1, pageSize)
            };
            list.AddRange(articles);
            return list;
        }

        public async static Task<PagedList<Connection>> GetConnectionsAsync(string relation, string articleId, string query = null, string label = null, IEnumerable<string> fields = null, int pageNumber = 1, int pageSize = 20)
        {
            IConnectionService connService = ObjectFactory.Build<IConnectionService>();
            var request = new FindConnectedArticlesRequest
            {
                Relation = relation,
                ArticleId = articleId,
                Query = query,
                Label = label,
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
                GetNextPage = async skip => await GetConnectionsAsync(relation, articleId, query, null, fields, pageNumber + skip + 1, pageSize)
            };
            list.AddRange(response.Connections);
            return list;
        }
    }
}
