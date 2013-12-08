using Appacitive.Sdk.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk
{
    public static partial class APObjects
    {
        public async static Task<APObject> GetAsync(string type, string id, IEnumerable<string> fields = null)
        {
            var request = new GetObjectRequest() { Id = id, Type = type, };
            if (fields != null)
                request.Fields.AddRange(fields);
            var response = await request.ExecuteAsync();
            if (response.Status.IsSuccessful == false)
                throw response.Status.ToFault();
            Debug.Assert(response.Object != null, "For a successful get call, object should always be returned.");
            return response.Object;
        }

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

        public async static Task<PagedList<APObject>> FindAllAsync(string type, string query = null, IEnumerable<string> fields = null, int page = 1, int pageSize = 20, string orderBy = null, SortOrder sortOrder = SortOrder.Descending)
        {
            var request = new FindAllObjectsRequest()
            {
                Type = type,
                Query = query,
                PageNumber = page,
                PageSize = pageSize,
                OrderBy = orderBy,
                SortOrder = sortOrder
            };
            if( fields != null )
                request.Fields.AddRange(fields);
            var response = await request.ExecuteAsync();
            if (response.Status.IsSuccessful == false)
                throw response.Status.ToFault();
            var objects = new PagedList<APObject>()
            {
                PageNumber = response.PagingInfo.PageNumber,
                PageSize = response.PagingInfo.PageSize,
                TotalRecords = response.PagingInfo.TotalRecords,
                GetNextPage = async skip => await FindAllAsync(type, query, fields, page + skip + 1, pageSize, orderBy, sortOrder)
            };
            objects.AddRange(response.Objects);
            return objects;

        }

        public async static Task<PagedList<APObject>> FreeTextSearchAsync(string type, string freeTextExpression, IEnumerable<string> fields = null, int page = 1, int pageSize = 20, string orderBy = null, SortOrder sortOrder = SortOrder.Descending)
        {
            var request = new FreeTextSearchObjectsRequest()
            {
                Type = type,
                FreeTextExpression = freeTextExpression,
                PageNumber = page,
                PageSize = pageSize,
                OrderBy = orderBy,
                SortOrder = sortOrder
            };
            if (fields != null)
                request.Fields.AddRange(fields);
            var response = await request.ExecuteAsync();
            if (response.Status.IsSuccessful == false)
                throw response.Status.ToFault();
            var objects = new PagedList<APObject>()
            {
                PageNumber = response.PagingInfo.PageNumber,
                PageSize = response.PagingInfo.PageSize,
                TotalRecords = response.PagingInfo.TotalRecords,
                GetNextPage = async skip => await FreeTextSearchAsync(type, freeTextExpression, fields, page + skip + 1, pageSize, orderBy, sortOrder)
            };
            objects.AddRange(response.Objects);
            return objects;

        }

        public async static Task DeleteAsync(string type, string id, bool deleteConnections = false)
        {
            var response = await new DeleteObjectRequest()
            {
                Id = id,
                Type = type,
                DeleteConnections = deleteConnections
            }.ExecuteAsync();
            if (response.Status.IsSuccessful == false)
                throw response.Status.ToFault();
        }

        public async static Task MultiDeleteAsync(string type, params string[] ids)
        {
            var response = await new BulkDeleteObjectRequest
                {
                    Type = type,
                    ObjectIds = ids.ToList()
                }.ExecuteAsync();
            if (response.Status.IsSuccessful == false)
                throw response.Status.ToFault();
        }


        public async static Task<PagedList<APObject>> GetConnectedObjectsAsync(string relation, string type, string objectId, string query = null, string label = null, IEnumerable<string> fields = null, int pageNumber = 1, int pageSize = 20)
        {
            return await new APObject(type, objectId).GetConnectedObjectsAsync(relation, query, label, fields, pageNumber, pageSize);
        }

        public async static Task<PagedList<APConnection>> GetConnectionsAsync(string relation, string schemaType, string objectId, string query = null, string label = null, IEnumerable<string> fields = null, int pageNumber = 1, int pageSize = 20)
        {
            return await new APObject(schemaType, objectId).GetConnectionsAsync(relation, query, label, fields, pageNumber, pageSize);
        }
    }
}
