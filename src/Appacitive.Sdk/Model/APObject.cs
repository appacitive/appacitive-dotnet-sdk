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
    public partial class APObject : Entity, INotifyPropertyChanged
    {   
        protected APObject(APObject existing) : base(existing)
        {
            // Copy
            this.SchemaId = existing.SchemaId;
        }

        public APObject(string type) : base(type)
        {   
        }

        public APObject(string type, string id)
            : base(type, id)
        {
        }

        internal string SchemaId { get; set; }

        internal bool IsNewInstance
        {
            get { return string.IsNullOrWhiteSpace(this.Id) == true || this.Id == "0"; }
        }

        public static readonly IEnumerable<string> AllFields = new string[0];

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

        public async Task<PagedList<APObject>> GetConnectedObjectsAsync(string relation, string query = null, string label = null, IEnumerable<string> fields = null, int pageNumber = 1, int pageSize = 20, string orderBy = null, SortOrder sortOrder = SortOrder.Descending)
        {
            var request = new FindConnectedObjectsRequest
            {
                Relation = relation,
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
                GetNextPage = async skip => await this.GetConnectedObjectsAsync(relation, query, label, fields, pageNumber + skip + 1, pageSize)
            };
            list.AddRange(objects);
            return list;
        }

        public async Task<PagedList<Connection>> GetConnectionsAsync(string relation, string query = null, string label = null, IEnumerable<string> fields = null, int pageNumber = 1, int pageSize = 20, string orderBy = null, SortOrder sortOrder = SortOrder.Descending)
        {
            var request = new FindConnectedObjectsRequest
            {
                Relation = relation,
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

            var list = new PagedList<Connection>
            {
                PageNumber = response.PagingInfo.PageNumber,
                PageSize = response.PagingInfo.PageSize,
                TotalRecords = response.PagingInfo.TotalRecords,
                GetNextPage = async skip => await GetConnectionsAsync(relation, query, null, fields, pageNumber + skip + 1, pageSize)
            };
            list.AddRange(response.Nodes.Select(n => n.Connection));
            return list;
        }
    }
}