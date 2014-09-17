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
        /// <param name="options">Request specific api options. These will override the global settings for the app for this request.</param>
        /// <returns>The matching APConnection object.</returns>
        public async static Task<APConnection> GetAsync(string type, string endpointObjectId1, string endpointObjectId2, ApiOptions options = null)
        {
            var request = new GetConnectionByEndpointRequest
            {
                Relation = type,
                ObjectId1 = endpointObjectId1,
                ObjectId2 = endpointObjectId2
            };
            ApiOptions.Apply(request, options);
            var response = await request.ExecuteAsync();
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
        /// <param name="options">Request specific api options. These will override the global settings for the app for this request.</param>
        /// <returns>The matching APConnection object.</returns>
        public async static Task<APConnection> GetAsync(string relation, string id, ApiOptions options = null)
        {
            var request = new GetConnectionRequest { Relation = relation, Id = id };
            ApiOptions.Apply(request, options);
            var response = await request.ExecuteAsync();
            if (response.Status.IsSuccessful == false)
                throw response.Status.ToFault();
            else return response.Connection;
        }

        /// <summary>
        /// Deletes the provided APConnection object.
        /// </summary>
        /// <param name="type">The type (relation name) of the connection.</param>
        /// <param name="id">The id of the connection.</param>
        /// <param name="options">Request specific api options. These will override the global settings for the app for this request.</param>
        public async static Task DeleteAsync(string type, string id, ApiOptions options = null)
        {
            var request = new DeleteConnectionRequest
            {
                Relation = type,
                Id = id
            };
            ApiOptions.Apply(request, options);
            var response = await request.ExecuteAsync();
            if (response.Status.IsSuccessful == false)
                throw response.Status.ToFault();
        }

        /// <summary>
        /// Finds a paginated list of APConnections for the given type and search criteria.
        /// </summary>
        /// <param name="type">The type (relation name) of the connection.</param>
        /// <param name="freeTextExpression">Free text search expression over all fields of the given type.</param>
        /// <param name="query">The search query</param>
        /// <param name="fields">The specific fields of the conection to be retrieved.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="orderBy">The field on which to sort.</param>
        /// <param name="sortOrder">Sort order - Ascending or Descending.</param>
        /// <param name="options">Request specific api options. These will override the global settings for the app for this request.</param>
        /// <returns>A paginated list of APConnections for the given search criteria.</returns>
        public async static Task<PagedList<APConnection>> FindAllAsync(string type, string freeTextExpression, IQuery query = null, IEnumerable<string> fields = null, int pageNumber = 1, int pageSize = 20, string orderBy = null, SortOrder sortOrder = SortOrder.Descending, ApiOptions options = null)
        {
            query = query ?? Query.None;
            var request = new FindAllConnectionsRequest()
            {
                Type = type,
                FreeTextExpression = freeTextExpression,
                Query = query.AsString().Escape(),
                PageNumber = pageNumber,
                PageSize = pageSize,
                OrderBy = orderBy,
                SortOrder = sortOrder
            };
            ApiOptions.Apply(request, options);
            var response = await request.ExecuteAsync();
            if (response.Status.IsSuccessful == false)
                throw response.Status.ToFault();
            var connections = new PagedList<APConnection>()
            {
                PageNumber = response.PagingInfo.PageNumber,
                PageSize = response.PagingInfo.PageSize,
                TotalRecords = response.PagingInfo.TotalRecords,
                GetNextPage = async skip => await FindAllAsync(type, freeTextExpression, query, fields, pageNumber + skip + 1, pageSize, orderBy, sortOrder, options)
            };
            connections.AddRange(response.Connections);
            return connections;

        }

        /// <summary>
        /// Finds a paginated list of APConnections for the given type and search criteria.
        /// </summary>
        /// <param name="type">The type (relation name) of the connection.</param>
        /// <param name="query">The search query</param>
        /// <param name="fields">The specific fields of the conection to be retrieved.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="orderBy">The field on which to sort.</param>
        /// <param name="sortOrder">Sort order - Ascending or Descending.</param>
        /// <param name="options">Request specific api options. These will override the global settings for the app for this request.</param>
        /// <returns>A paginated list of APConnections for the given search criteria.</returns>
        public async static Task<PagedList<APConnection>> FindAllAsync(string type, IQuery query = null, IEnumerable<string> fields = null, int pageNumber = 1, int pageSize = 20, string orderBy = null, SortOrder sortOrder = SortOrder.Descending, ApiOptions options = null)
        {
            return await FindAllAsync(type, null, query, fields, pageNumber, pageSize, orderBy, sortOrder, options);
        }

        /// <summary>
        /// Deletes multiple APConnection objects by id list.
        /// </summary>
        /// <param name="type">The type (relation name) of the connection.</param>
        /// <param name="connectionIds">Array of ids corresponding to the APConnection objects to be deleted.</param>
        public async static Task MultiDeleteAsync(string type, params string[] connectionIds)
        {
            await MultiDeleteAsync(type, null, connectionIds);
        }

        /// <summary>
        /// Deletes multiple APConnection objects by id list.
        /// </summary>
        /// <param name="type">The type (relation name) of the connection.</param>
        /// <param name="connectionIds">Array of ids corresponding to the APConnection objects to be deleted.</param>
        /// <param name="options">Request specific api options. These will override the global settings for the app for this request.</param>
        public async static Task MultiDeleteAsync(string type, ApiOptions options, params string[] connectionIds)
        {
            var request = new BulkDeleteConnectionRequest { Type = type, ConnectionIds = new List<string>(connectionIds) };
            ApiOptions.Apply(request, options);
            var response = await request.ExecuteAsync();
            if (response.Status.IsSuccessful == false)
                throw response.Status.ToFault();
        }
    }
}
