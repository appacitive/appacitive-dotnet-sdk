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
        protected Article(Article existing) : base(existing)
        {
            // Copy
            this.SchemaId = existing.SchemaId;
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

        protected override async Task<Entity> UpdateAsync(IDictionary<string, object> propertyUpdates, IDictionary<string, string> attributeUpdates, IEnumerable<string> addedTags, IEnumerable<string> removedTags, int specificRevision)
        {
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

            var response = await request.ExecuteAsync();
            if (response.Status.IsSuccessful == false)
                throw response.Status.ToFault();

            // 3. Update the last known state based on the differences
            Debug.Assert(response.Article != null, "If status is successful, then updated article should not be null.");
            return response.Article;
        }

        protected override async Task<Entity> CreateNewAsync()
        {
            // Create a new article
            var response = await new CreateArticleRequest()
            {
                Article = this
            }.ExecuteAsync();
            if (response.Status.IsSuccessful == false)
                throw response.Status.ToFault();

            // 3. Update the last known state based on the differences
            Debug.Assert(response.Article != null, "If status is successful, then created article should not be null.");
            return response.Article;
        }

        public async Task<PagedList<Article>> GetConnectedArticlesAsync(string relation, string query = null, string label = null, IEnumerable<string> fields = null, int pageNumber = 1, int pageSize = 20)
        {
            return await Articles.GetConnectedArticlesAsync(relation, this.Id, query, label, fields, pageNumber, pageSize);
        }

        public async Task<PagedList<Connection>> GetConnectionsAsync(string relation, string query = null, string label = null, IEnumerable<string> fields = null, int pageNumber = 1, int pageSize = 20)
        {
            return await Articles.GetConnectionsAsync(relation, this.Id, query, label, fields, pageNumber, pageSize);
        }
    }
}