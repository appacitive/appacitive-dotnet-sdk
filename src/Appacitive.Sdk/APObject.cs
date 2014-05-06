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
    /// <summary>
    /// Represents an instance of a type on the Appacitive platform.
    /// </summary>
    public partial class APObject : Entity, INotifyPropertyChanged
    {   
        /// <summary>
        /// Copy and create a new instance from an existing instance.
        /// </summary>
        /// <param name="existing">Existing object instance.</param>
        protected APObject(APObject existing) : base(existing)
        {   
        }

        /// <summary>
        /// Creates a new instance of APObject with the given type.
        /// </summary>
        /// <param name="type">Type (schema name) of the object.</param>
        public APObject(string type) : base(type)
        {   
        }

        /// <summary>
        /// Creates a new instance of APObject corresponding to an existing object.
        /// This does not get the existing object from the backend platform.
        /// </summary>
        /// <param name="type">Type (schema name) of the object.</param>
        /// <param name="id">If of the existing object instance.</param>
        public APObject(string type, string id)
            : base(type, id)
        {
        }

        /// <summary>
        /// Gets whether or not this object is a new instance or represents an existing instance.
        /// </summary>
        internal bool IsNewInstance
        {
            get { return string.IsNullOrWhiteSpace(this.Id) == true || this.Id == "0"; }
        }

        internal static readonly IEnumerable<string> AllFields = new string[0];

        /// <summary>
        /// Creates or updates this APObject instance. 
        /// </summary>
        /// <param name="specificRevision">
        /// Revision number for this connection instance. 
        /// Used for <a href="http://en.wikipedia.org/wiki/Multiversion_concurrency_control">Multiversion Concurrency Control</a>.
        /// If this version does not match on the server side, the Save operation will fail. Passing 0 disables the revision check.
        /// </param>
        /// <returns>The current instance of APObject updated with the changes applied.</returns>
        public virtual async Task<APObject> SaveAsync(int specificRevision = 0)
        {
            await this.SaveEntityAsync(specificRevision);
            return this;
        }

        protected override async Task<Entity> UpdateAsync(IDictionary<string, object> propertyUpdates, IDictionary<string, string> attributeUpdates, IEnumerable<string> addedTags, IEnumerable<string> removedTags, int specificRevision)
        {
            var request = new UpdateObjectRequest {Id = this.Id, Type = this.Type};
            request.Revision = specificRevision;
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

            var response = await request.ExecuteAsync();
            if (response.Status.IsSuccessful == false)
                throw response.Status.ToFault();

            // 3. Update the last known state based on the differences
            Debug.Assert(response.Object != null, "If status is successful, then updated object should not be null.");
            return response.Object;
        }

        protected override async Task<Entity> CreateNewAsync()
        {
            // Create a new object
            var response = await new CreateObjectRequest()
            {
                Object = this
            }.ExecuteAsync();
            if (response.Status.IsSuccessful == false)
                throw response.Status.ToFault();

            // 3. Update the last known state based on the differences
            Debug.Assert(response.Object != null, "If status is successful, then created object should not be null.");
            return response.Object;
        }

        /// <summary>
        /// Gets a paginated list of APObjects connected to this object via connections of the given type.
        /// </summary>
        /// <param name="connectionType">The type (relation name) of the connection.</param>
        /// <param name="query">Search query to further filter the list of connection objects.</param>
        /// <param name="label">Label of the endpoint to be queried. This is mandatory when the connection type being queried has endpoints with the same type but different labels.</param>
        /// <param name="fields">The fields to be returned for the matching objects.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="orderBy">The field on which to sort the results.</param>
        /// <param name="sortOrder">The sort order - Ascending or Descending.</param>
        /// <returns>A paginated list of APObjects.</returns>
        public async Task<PagedList<APObject>> GetConnectedObjectsAsync(string connectionType, string query = null, string label = null, IEnumerable<string> fields = null, int pageNumber = 1, int pageSize = 20, string orderBy = null, SortOrder sortOrder = SortOrder.Descending)
        {
            var request = new FindConnectedObjectsRequest
            {
                Relation = connectionType,
                ObjectId = this.Id,
                Object = this,
                Label = label,
                Query = query,
                Type = this.Type,
                ReturnEdge = false,
                PageNumber = pageNumber,
                PageSize = pageSize,
                SortOrder = sortOrder,
                OrderBy = orderBy
            };
            if (fields != null)
                request.Fields.AddRange(fields);
            var response = await request.ExecuteAsync();
            if (response.Status.IsSuccessful == false)
                throw response.Status.ToFault();

            IEnumerable<APObject> objects = response.Nodes.Select(n => n.Object);
            var list = new PagedList<APObject>()
            {
                PageNumber = response.PagingInfo.PageNumber,
                PageSize = response.PagingInfo.PageSize,
                TotalRecords = response.PagingInfo.TotalRecords,
                GetNextPage = async skip => await this.GetConnectedObjectsAsync(connectionType, query, label, fields, pageNumber + skip + 1, pageSize, orderBy, sortOrder)
            };
            list.AddRange(objects);
            return list;
        }

        /// <summary>
        /// Gets a paginated list of APConnections for the current object of the given connection type.
        /// </summary>
        /// <param name="connectionType">The type (relation name) of the connection.</param>
        /// <param name="query">Search query to further filter the list of connection objects.</param>
        /// <param name="label">Label of the endpoint to be queried. This is mandatory when the connection type being queried has endpoints with the same type but different labels.</param>
        /// <param name="fields">The fields to be returned for the matching objects.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="orderBy">The field on which to sort the results.</param>
        /// <param name="sortOrder">The sort order - Ascending or Descending.</param>
        /// <returns>A paginated list of APConnection objects.</returns>
        public async Task<PagedList<APConnection>> GetConnectionsAsync(string connectionType, string query = null, string label = null, IEnumerable<string> fields = null, int pageNumber = 1, int pageSize = 20, string orderBy = null, SortOrder sortOrder = SortOrder.Descending)
        {
            var request = new FindConnectedObjectsRequest
            {
                Relation = connectionType,
                ObjectId = this.Id,
                Object = this,
                Query = query,
                Label = label,
                Type = this.Type,
                ReturnEdge = true,
                PageNumber = pageNumber,
                PageSize = pageSize,
                SortOrder = sortOrder,
                OrderBy = orderBy
            };
            if (fields != null)
                request.Fields.AddRange(fields);
            var response = await request.ExecuteAsync();
            if (response.Status.IsSuccessful == false)
                throw response.Status.ToFault();

            var list = new PagedList<APConnection>
            {
                PageNumber = response.PagingInfo.PageNumber,
                PageSize = response.PagingInfo.PageSize,
                TotalRecords = response.PagingInfo.TotalRecords,
                GetNextPage = async skip => await GetConnectionsAsync(connectionType, query, null, fields, pageNumber + skip + 1, pageSize, orderBy, sortOrder)
            };
            list.AddRange(response.Nodes.Select(n => n.Connection));
            return list;
        }
    }
}