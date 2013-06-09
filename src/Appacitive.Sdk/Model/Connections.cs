using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Appacitive.Sdk.Services;

namespace Appacitive.Sdk
{
    public static partial class Connections
    {   
        public async static Task<Connection> GetAsync(string relation, string endpointArticleId1, string endpointArticleId2)
        {
            IConnectionService connService = ObjectFactory.Build<IConnectionService>();
            var response = await connService.GetConnectionByEndpointAsync(new GetConnectionByEndpointRequest
            {
                Relation = relation,
                ArticleId1 = endpointArticleId1,
                ArticleId2 = endpointArticleId2
            });
            if (response.Status.IsSuccessful == false)
                throw response.Status.ToFault();
            else return response.Connections.SingleOrDefault();
        }

        public async static Task<Connection> GetAsync(string relation, string id)
        {
            IConnectionService connService = ObjectFactory.Build<IConnectionService>();
            var response = await connService.GetConnectionAsync(new GetConnectionRequest
                                                        {
                                                            Relation = relation,
                                                            Id = id
                                                        });
            if (response.Status.IsSuccessful == false)
                throw response.Status.ToFault();
            else return response.Connection;
        }

        public async static Task DeleteAsync(string relation, string id)
        {
            IConnectionService connService = ObjectFactory.Build<IConnectionService>();
            var response = await connService.DeleteConnectionAsync (new DeleteConnectionRequest
            {
                Relation = relation,
                Id = id
            });
            if (response.Status.IsSuccessful == false)
                throw response.Status.ToFault();
        }

        public async static Task<PagedList<Connection>> FindAllAsync(string type, string query = null, IEnumerable<string> fields = null, int page = 1, int pageSize = 20, string orderBy = null, SortOrder sortOrder = SortOrder.Descending)
        {
            var service = ObjectFactory.Build<IConnectionService>();
            var request = new FindAllConnectionsRequest()
            {
                Type = type,
                Query = query,
                PageNumber = page,
                PageSize = pageSize,
                OrderBy = orderBy,
                SortOrder = sortOrder
            };
            
            var response = await service.FindAllConnectionsAsync(request);
            if (response.Status.IsSuccessful == false)
                throw response.Status.ToFault();
            var connections = new PagedList<Connection>()
            {
                PageNumber = response.PagingInfo.PageNumber,
                PageSize = response.PagingInfo.PageSize,
                TotalRecords = response.PagingInfo.TotalRecords,
                GetNextPage = async skip => await FindAllAsync(type, query, fields, page + skip + 1, pageSize)
            };
            connections.AddRange(response.Connections);
            return connections;

        }

        public async static Task MultiDeleteAsync(string connectionType, params string[] connectionIds)
        {
            IConnectionService connService = ObjectFactory.Build<IConnectionService>();
            var response = await connService.BulkDeleteAsync(new BulkDeleteConnectionRequest { Type = connectionType, ConnectionIds = new List<string>(connectionIds) } );
            if (response.Status.IsSuccessful == false)
                throw response.Status.ToFault();
        }
    }
}
