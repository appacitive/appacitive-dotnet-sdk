using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Appacitive.Sdk.Services;

namespace Appacitive.Sdk
{
    /// <summary>
    /// Helper class which contains lookup methods for APConnections.
    /// </summary>
    public static partial class APConnections
    {   
        /// <summary>
        /// Gets an existing APConnection by its endpoints and type.
        /// </summary>
        /// <param name="type">Type of the connection.</param>
        /// <param name="endpointObjectId1">Id of the object for endpoint 1.</param>
        /// <param name="endpointObjectId2">Id of the object for endpoint 2.</param>
        /// <returns>The matching APConnection object.</returns>
        public async static Task<APConnection> GetAsync(string type, string endpointObjectId1, string endpointObjectId2)
        {   
            var response = await (new GetConnectionByEndpointRequest
            {
                Relation = type,
                ObjectId1 = endpointObjectId1,
                ObjectId2 = endpointObjectId2
            }).ExecuteAsync();
            if (response.Status.IsSuccessful == false)
                throw response.Status.ToFault();
            else
                return response.Connection;
            
        }

        /// <summary>
        /// Gets an existing APConnection by type and id.
        /// </summary>
        /// <param name="type">The type (relation name) of the connection.</param>
        /// <param name="id">The id of the connection.</param>
        /// <returns>The matching APConnection object.</returns>
        public async static Task<APConnection> GetAsync(string type, string id)
        {
            var response = await (new GetConnectionRequest
                                                        {
                                                            Relation = type,
                                                            Id = id
                                                        }).ExecuteAsync();
            if (response.Status.IsSuccessful == false)
                throw response.Status.ToFault();
            else return response.Connection;
        }

        /// <summary>
        /// Deletes the provided APConnection object.
        /// </summary>
        /// <param name="type">The type (relation name) of the connection.</param>
        /// <param name="id">The id of the connection.</param>
        public async static Task DeleteAsync(string type, string id)
        {
            var response = await (new DeleteConnectionRequest
            {
                Relation = type,
                Id = id
            }).ExecuteAsync();
            if (response.Status.IsSuccessful == false)
                throw response.Status.ToFault();
        }

        /// <summary>
        /// Finds a paginated list of APConnections for the given type and search criteria.
        /// </summary>
        /// <param name="type">The type (relation name) of the connection.</param>
        /// <param name="query">The search query</param>
        /// <param name="fields">The specific fields of the conection to be retrieved.</param>
        /// <param name="page">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="orderBy">The field on which to sort.</param>
        /// <param name="sortOrder">Sort order - Ascending or Descending.</param>
        /// <returns>A paginated list of APConnections for the given search criteria.</returns>
        public async static Task<PagedList<APConnection>> FindAllAsync(string type, IQuery query = null, IEnumerable<string> fields = null, int page = 1, int pageSize = 20, string orderBy = null, SortOrder sortOrder = SortOrder.Descending)
        {
            query = query ?? Query.None;
            var request = new FindAllConnectionsRequest()
            {
                Type = type,
                Query = query.AsString().Escape(),
                PageNumber = page,
                PageSize = pageSize,
                OrderBy = orderBy,
                SortOrder = sortOrder
            };

            var response = await request.ExecuteAsync();
            if (response.Status.IsSuccessful == false)
                throw response.Status.ToFault();
            var connections = new PagedList<APConnection>()
            {
                PageNumber = response.PagingInfo.PageNumber,
                PageSize = response.PagingInfo.PageSize,
                TotalRecords = response.PagingInfo.TotalRecords,
                GetNextPage = async skip => await FindAllAsync(type, query, fields, page + skip + 1, pageSize, orderBy, sortOrder)
            };
            connections.AddRange(response.Connections);
            return connections;

        }

        /// <summary>
        /// Deletes multiple APConnection objects by id list.
        /// </summary>
        /// <param name="type">The type (relation name) of the connection.</param>
        /// <param name="connectionIds">Array of ids corresponding to the APConnection objects to be deleted.</param>
        public async static Task MultiDeleteAsync(string type, params string[] connectionIds)
        {
            var response = await (new BulkDeleteConnectionRequest { Type = type, ConnectionIds = new List<string>(connectionIds) }).ExecuteAsync();
            if (response.Status.IsSuccessful == false)
                throw response.Status.ToFault();
        }
    }
}
