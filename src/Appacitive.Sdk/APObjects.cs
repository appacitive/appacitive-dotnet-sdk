using Appacitive.Sdk.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk
{
    /// <summary>
    /// Helper class which contains static lookup methods for APObject objects.
    /// </summary>
    public static partial class APObjects
    {
        /// <summary>
        /// Gets an existing APObject by type and id.
        /// </summary>
        /// <param name="type">Object type (schema name).</param>
        /// <param name="id">Object id.</param>
        /// <param name="fields">The object fields to be retrieved.</param>
        /// <param name="options">Request specific api options. These will override the global settings for the app for this request.</param>
        /// <returns>The matching APObject instance.</returns>
        public async static Task<APObject> GetAsync(string type, string id, IEnumerable<string> fields = null, ApiOptions options = null)
        {
            var request = new GetObjectRequest() { Id = id, Type = type, };
            if (fields != null)
                request.Fields.AddRange(fields);
            ApiOptions.Apply(request, options);
            var response = await request.ExecuteAsync();
            if (response.Status.IsSuccessful == false)
                throw response.Status.ToFault();
            Debug.Assert(response.Object != null, "For a successful get call, object should always be returned.");
            return response.Object;
        }

        /// <summary>
        /// Gets a list of existing objects by id list.
        /// </summary>
        /// <param name="type">Object type.</param>
        /// <param name="idList">Id list of the objects to be fetched.</param>
        /// <param name="fields">The object fields to be retrieved.</param>
        /// <returns>List of matching APObject objects.</returns>
        public async static Task<IEnumerable<APObject>> MultiGetAsync(string type, IEnumerable<string> idList, IEnumerable<string> fields = null)
        {
            var request = new MultiGetObjectsRequest() { Type = type, };
            request.IdList.AddRange(idList);
            if (fields != null)
                request.Fields.AddRange(fields);
            var response = await request.ExecuteAsync();
            if (response.Status.IsSuccessful == false)
                throw response.Status.ToFault();
            return response.Objects;
        }

        /// <summary>
        /// Gets a paginated list of APObjects matching the given search criteria.
        /// </summary>
        /// <param name="type">The object type.</param>
        /// <param name="query">The search query for objects to be found.</param>
        /// <param name="fields">The object fields to be returned for the matching list of objects.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="orderBy">The object field on which the results should be sorted.</param>
        /// <param name="sortOrder">The sort order.</param>
        /// <param name="options">Request specific api options. These will override the global settings for the app for this request.</param>
        /// <returns>Paginated list of APObject objects matching the given search criteria.</returns>
        public async static Task<PagedList<APObject>> FindAllAsync(string type, IQuery query = null, IEnumerable<string> fields = null, int pageNumber = 1, int pageSize = 20, string orderBy = null, SortOrder sortOrder = SortOrder.Descending, ApiOptions options = null)
        {
            query = query ?? Query.None;
            var request = new FindAllObjectsRequest()
            {
                Type = type,
                Query = query.AsString().Escape(),
                PageNumber = pageNumber,
                PageSize = pageSize,
                OrderBy = orderBy,
                SortOrder = sortOrder
            };
            if( fields != null )
                request.Fields.AddRange(fields);
            ApiOptions.Apply(request, options);
            var response = await request.ExecuteAsync();
            if (response.Status.IsSuccessful == false)
                throw response.Status.ToFault();
            var objects = new PagedList<APObject>()
            {
                PageNumber = response.PagingInfo.PageNumber,
                PageSize = response.PagingInfo.PageSize,
                TotalRecords = response.PagingInfo.TotalRecords,
                GetNextPage = async skip => await FindAllAsync(type, query, fields, pageNumber + skip + 1, pageSize, orderBy, sortOrder, options)
            };
            objects.AddRange(response.Objects);
            return objects;

        }

        /// <summary>
        /// Gets a paginated list of APObjects matching the given search criteria.
        /// </summary>
        /// <param name="type">The object type.</param>
        /// <param name="query">The search query for objects to be found.</param>
        /// <param name="fields">The object fields to be returned for the matching list of objects.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="orderBy">The object field on which the results should be sorted.</param>
        /// <param name="sortOrder">The sort order.</param>
        /// <param name="options">Request specific api options. These will override the global settings for the app for this request.</param>
        /// <returns>Paginated list of APObject objects matching the given search criteria.</returns>
        public async static Task<PagedList<APObject>> FindAllAsync(string type, string freeTextExpression, IQuery query = null, IEnumerable<string> fields = null, int pageNumber = 1, int pageSize = 20, string orderBy = null, SortOrder sortOrder = SortOrder.Descending, ApiOptions options = null)
        {
            query = query ?? Query.None;
            var request = new FindAllObjectsRequest()
            {
                Type = type,
                FreeTextExpression = freeTextExpression,
                Query = query.AsString().Escape(),
                PageNumber = pageNumber,
                PageSize = pageSize,
                OrderBy = orderBy,
                SortOrder = sortOrder
            };
            if (fields != null)
                request.Fields.AddRange(fields);
            ApiOptions.Apply(request, options);
            var response = await request.ExecuteAsync();
            if (response.Status.IsSuccessful == false)
                throw response.Status.ToFault();
            var objects = new PagedList<APObject>()
            {
                PageNumber = response.PagingInfo.PageNumber,
                PageSize = response.PagingInfo.PageSize,
                TotalRecords = response.PagingInfo.TotalRecords,
                GetNextPage = async skip => await FindAllAsync(type, query, fields, pageNumber + skip + 1, pageSize, orderBy, sortOrder, options)
            };
            objects.AddRange(response.Objects);
            return objects;

        }

        /// <summary>
        /// Gets a paginated list of APObjects matching the given freetext search expression.
        /// The provided expression is matched with the entire object instead of a particular field.
        /// </summary>
        /// <param name="type">The object type</param>
        /// <param name="freeTextExpression">Freetext expression.</param>
        /// <param name="fields">The specific object fields to be retrieved. </param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="orderBy">The object field on which the results should be sorted.</param>
        /// <param name="sortOrder">The sort order.</param>
        /// <param name="options">Request specific api options. These will override the global settings for the app for this request.</param>
        /// <returns>Paginated list of APObject objects matching the given search criteria.</returns>
        public async static Task<PagedList<APObject>> FreeTextSearchAsync(string type, string freeTextExpression, IEnumerable<string> fields = null, int pageNumber = 1, int pageSize = 20, string orderBy = null, SortOrder sortOrder = SortOrder.Descending, ApiOptions options = null)
        {
            var request = new FreeTextSearchObjectsRequest()
            {
                Type = type,
                FreeTextExpression = freeTextExpression,
                PageNumber = pageNumber,
                PageSize = pageSize,
                OrderBy = orderBy,
                SortOrder = sortOrder
            };
            if (fields != null)
                request.Fields.AddRange(fields);
            ApiOptions.Apply(request, options);
            var response = await request.ExecuteAsync();
            if (response.Status.IsSuccessful == false)
                throw response.Status.ToFault();
            var objects = new PagedList<APObject>()
            {
                PageNumber = response.PagingInfo.PageNumber,
                PageSize = response.PagingInfo.PageSize,
                TotalRecords = response.PagingInfo.TotalRecords,
                GetNextPage = async skip => await FreeTextSearchAsync(type, freeTextExpression, fields, pageNumber + skip + 1, pageSize, orderBy, sortOrder, options)
            };
            objects.AddRange(response.Objects);
            return objects;

        }

        /// <summary>
        /// Deletes an existing object by type and id.
        /// </summary>
        /// <param name="type">Object type.</param>
        /// <param name="id">Object id for the object to be deleted.</param>
        /// <param name="deleteConnections">Flag indicating if the delete should also delete any existing connections with the object.
        /// If <code>deleteConnections=False</code> and any existing connections are present, then this operation will fail.</param>
        /// <param name="options">Request specific api options. These will override the global settings for the app for this request.</param>
        public async static Task DeleteAsync(string type, string id, bool deleteConnections = false, ApiOptions options = null)
        {
            var request = new DeleteObjectRequest()
            {
                Id = id,
                Type = type,
                DeleteConnections = deleteConnections
            };
            ApiOptions.Apply(request, options);
            var response = await request.ExecuteAsync();
            if (response.Status.IsSuccessful == false)
                throw response.Status.ToFault();
        }

        /// <summary>
        /// Delete multiple objects by id list. This method will only work for objects without any connections.
        /// </summary>
        /// <param name="type">Object id</param>
        /// <param name="ids">List of object ids to be deleted.</param>
        public async static Task MultiDeleteAsync(string type, params string[] ids)
        {
            ApiOptions options = null;
            await MultiDeleteAsync(type, options, ids);
        }


        /// <summary>
        /// Delete multiple objects by id list. This method will only work for objects without any connections.
        /// </summary>
        /// <param name="type">Object id</param>
        /// <param name="ids">List of object ids to be deleted.</param>
        /// <param name="options">Request specific api options. These will override the global settings for the app for this request.</param>
        public async static Task MultiDeleteAsync(string type, ApiOptions options, params string[] ids)
        {
            var request = new BulkDeleteObjectRequest
                {
                    Type = type,
                    ObjectIds = ids.ToList()
                };
            ApiOptions.Apply(request, options);
            var response = await request.ExecuteAsync();
            if (response.Status.IsSuccessful == false)
                throw response.Status.ToFault();
        }

        /// <summary>
        /// Gets a paginated list of APObjects connected to the given object via connections of the given type.
        /// </summary>
        /// <param name="objectId">The object id to be queried.</param>
        /// <param name="type">The object type (schema name).</param>
        /// <param name="connectionType">The type (relation name) of the connection.</param>
        /// <param name="query">Search query to further filter the list of connection objects.</param>
        /// <param name="label">Label of the endpoint to be queried. This is mandatory when the connection type being queried has endpoints with the same type but different labels.</param>
        /// <param name="fields">The fields to be returned for the matching objects.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="orderBy">The field on which to sort the results.</param>
        /// <param name="sortOrder">The sort order - Ascending or Descending.</param>
        /// <param name="options">Request specific api options. These will override the global settings for the app for this request.</param>
        /// <returns>A paginated list of APObjects.</returns>
        public async static Task<PagedList<APObject>> GetConnectedObjectsAsync(string relation, string type, string objectId, IQuery query = null, string label = null, IEnumerable<string> fields = null, int pageNumber = 1, int pageSize = 20, string orderBy = null, SortOrder sortOrder = SortOrder.Ascending, ApiOptions options = null)
        {
            query = query ?? Query.None;
            return await new APObject(type, objectId).GetConnectedObjectsAsync(relation, query.AsString(), label, fields, pageNumber, pageSize, orderBy, sortOrder, options);
        }

        /// <summary>
        /// Gets a paginated list of APConnections for the given object of the given connection type.
        /// </summary>
        /// <param name="objectId">The object id to be queried.</param>
        /// <param name="type">The object type (schema name).</param>
        /// <param name="connectionType">The type (relation name) of the connection.</param>
        /// <param name="query">Search query to further filter the list of connection objects.</param>
        /// <param name="label">Label of the endpoint to be queried. This is mandatory when the connection type being queried has endpoints with the same type but different labels.</param>
        /// <param name="fields">The fields to be returned for the matching objects.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="orderBy">The field on which to sort the results.</param>
        /// <param name="sortOrder">The sort order - Ascending or Descending.</param>
        /// <param name="options">Request specific api options. These will override the global settings for the app for this request.</param>
        /// <returns>A paginated list of APConnection objects.</returns>
        public async static Task<PagedList<APConnection>> GetConnectionsAsync(string relation, string schemaType, string objectId, IQuery query = null, string label = null, IEnumerable<string> fields = null, int pageNumber = 1, int pageSize = 20, string orderBy = null, SortOrder sortOrder = SortOrder.Ascending, ApiOptions options = null)
        {
            query = query ?? Query.None;
            return await new APObject(schemaType, objectId).GetConnectionsAsync(relation, query.AsString(), label, fields, pageNumber, pageSize, orderBy, sortOrder, options);
        }
    }
}
