using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Appacitive.Sdk.Services;

namespace Appacitive.Sdk
{
    public class ConnectionBuilder
    {
        internal ConnectionBuilder(string relation)
        {
            this.RelationName = relation;
        }

        private string RelationName {get; set;}
        private string EndpointALabel {get; set;}
        private string EndpointAId {get; set;}
        private Article EndpointAContent {get; set;}
        private string EndpointBLabel {get; set;}
        private string EndpointBId {get; set;}
        private Article EndpointBContent {get; set;}


        public ConnectionBuilder FromNewArticle(string endpointLabel, Article article )
        {
            this.EndpointALabel = endpointLabel;
            this.EndpointAContent = article;
            this.EndpointAId = null;
            return this;
        }

        public ConnectionBuilder FromExistingArticle(string endpointLabel, string articleId )
        {
            this.EndpointALabel = endpointLabel;
            this.EndpointAContent = null;
            this.EndpointAId = articleId;
            return this;
        }

        public Connection ToNewArticle( string endpointLabel, Article article )
        {
            this.EndpointBLabel = endpointLabel;
            this.EndpointBContent = article;
            this.EndpointBId = null;
            return Build();
        }

        public Connection ToExistingArticle( string endpointLabel, string articleId )
        {
            this.EndpointBLabel = endpointLabel;
            this.EndpointBContent = null;
            this.EndpointBId = articleId;
            return Build();
        }

        private Connection Build()
        {
            Connection conn = null;
            if( this.EndpointAContent != null && this.EndpointBContent != null )
                conn = new Connection(this.RelationName, EndpointALabel, EndpointAContent, EndpointBLabel, EndpointBContent );
            else if( this.EndpointAContent == null && this.EndpointBContent != null )
                conn = new Connection(this.RelationName, EndpointBLabel, EndpointBContent, EndpointALabel, EndpointAId);
            else if (this.EndpointAContent != null && this.EndpointBContent == null)
                conn = new Connection(this.RelationName, EndpointALabel, EndpointAContent, EndpointBLabel, EndpointBId);
            else 
                conn = new Connection(this.RelationName, EndpointALabel, EndpointAId, EndpointBLabel, EndpointBId);
            return conn;
        }

        
    }

    public class Connection : Entity
    {
        public Connection(string type) : base(type) 
        {
            this.Endpoints = new EndpointPair(null, null);
        }

        public Connection(string type, string id) : base(type, id) 
        {
            this.Endpoints = new EndpointPair(null, null);
        }

        public Connection(string type, string labelA, string articleIdA, string labelB, string ArticleIdB) 
            : base(type)
        {
            var ep1 = new Endpoint(labelA, articleIdA);
            var ep2 = new Endpoint(labelB, ArticleIdB);
            this.Endpoints = new EndpointPair(ep1, ep2);
        }

        public Connection(string type, string labelA, Article articleA, string labelB, string ArticleIdB)
            : base(type)
        {
            Endpoint ep1, ep2;
            if (articleA.IsNewInstance == false)
            {
                ep1 = new Endpoint(labelA, articleA.Id);
                ep2 = new Endpoint(labelB, ArticleIdB);
            }
            else
            {
                string nullId = null;
                ep1 = new Endpoint(labelA, nullId);
                ep2 = new Endpoint(labelB, ArticleIdB);
                ep1.Content = articleA;
            }
            this.Endpoints = new EndpointPair(ep1, ep2);
        }

        public Connection(string type, string labelA, Article articleA, string labelB, Article articleB)
            : base(type)
        {
            Endpoint ep1, ep2;
            string nullId = null;
            if (articleA.IsNewInstance == true)
            {
                ep1 = new Endpoint(labelA, nullId);
                ep1.Content = articleA;
            }
            else
                ep1 = new Endpoint(labelA, articleA.Id);

            if (articleB.IsNewInstance == true)
            {   
                ep2 = new Endpoint(labelB, nullId);
                ep2.Content = articleB;
            }
            else
                ep2 = new Endpoint(labelB, articleB.Id);

            this.Endpoints = new EndpointPair(ep1, ep2);
        }

        public static ConnectionBuilder New(string relationName)
        {
            return new ConnectionBuilder(relationName);
        }

        
        internal bool IsNewInstance
        {
            get 
            {
                return string.IsNullOrWhiteSpace(this.Id) || this.Id == "0";
            }
        }

        public EndpointPair Endpoints { get; set; }

        public async Task<Article> GetEndpointArticleAsync(string label)
        {
            if (string.Compare(this.Endpoints.EndpointA.Label, label, StringComparison.OrdinalIgnoreCase) == 0)
                return await this.Endpoints.EndpointA.GetArticleAsync();
            if (string.Compare(this.Endpoints.EndpointB.Label, label, StringComparison.OrdinalIgnoreCase) == 0)
                return await this.Endpoints.EndpointB.GetArticleAsync();
            throw new AppacitiveException("Invalid label " + label);
        }

        public string GetEndpointId(string label)
        {
            if (string.Compare(this.Endpoints.EndpointA.Label, label, StringComparison.OrdinalIgnoreCase) == 0)
                return this.Endpoints.EndpointA.ArticleId;
            if (string.Compare(this.Endpoints.EndpointB.Label, label, StringComparison.OrdinalIgnoreCase) == 0)
                return this.Endpoints.EndpointB.ArticleId;
            throw new AppacitiveException("Invalid label " + label);
        }

        public string RelationId { get; set; }

        protected async override Task<Entity> CreateNewAsync()
        {
            // Handling for special case when endpoint contains a new article or device.
            // Since these cannot be created on the fly when creating a new connection.
            var endpoints = this.Endpoints.ToArray();
            for (int i = 0; i < endpoints.Length; i++)
            {
                if (endpoints[i].CreateEndpoint == false) continue;
                if (endpoints[i].Content.Type.Equals("user", StringComparison.OrdinalIgnoreCase) == true ||
                    endpoints[i].Content.Type.Equals("device", StringComparison.OrdinalIgnoreCase) == true)
                {
                    // Create the article
                    await endpoints[i].Content.SaveAsync();
                    // Update endpoint articleid
                    endpoints[i].ArticleId = endpoints[i].Content.Id;
                }
            }

            // Create a new article
            var response = await (new CreateConnectionRequest()
            {
                Connection = this
            }).ExecuteAsync();
            if (response.Status.IsSuccessful == false)
                throw response.Status.ToFault();
            Debug.Assert(response.Connection != null, "If status is successful, then created connection should not be null.");
            return response.Connection;
        }

        protected override void UpdateState(Entity entity)
        {
            var other = entity as Connection;
            if (other == null) return;
            this.Endpoints.EndpointA = other.Endpoints.EndpointA;
            this.Endpoints.EndpointB = other.Endpoints.EndpointB;
        }

        protected override async Task<Entity> UpdateAsync(IDictionary<string, object> propertyUpdates, IDictionary<string, string> attributeUpdates, IEnumerable<string> addedTags, IEnumerable<string> removedTags, int specificRevision)
        {
            var request = new UpdateConnectionRequest{ Id = this.Id, Type = this.Type };
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
            Debug.Assert(response.Connection != null, "If status is successful, then updated connection should not be null.");
            return response.Connection;
        }

    }

    public class Endpoint
    {
        public Endpoint(string label, string articleId)
        {
            this.Label = label;
            this.ArticleId = articleId;
        }

        public Endpoint(string label, Article content)
        {
            this.Label = label;
            this.Content = content;
        }

        internal bool CreateEndpoint
        {
            get { return string.IsNullOrWhiteSpace(this.ArticleId); }
        }

        internal Article Content { get; set; }

        public string ArticleId { get; set; }

        public string Label { get; set; }

        public string Type { get; set; }

        public async Task<Article> GetArticleAsync()
        {
            if (this.Content != null)
                return this.Content;
            else 
                return await Articles.GetAsync(this.Type, this.ArticleId);
        }
    }

    public class EndpointPair
    {
        public EndpointPair(Endpoint ep1, Endpoint ep2)
        {
            this.EndpointA = ep1;
            this.EndpointB = ep2;
        }

        public Endpoint[] ToArray()
        {
            return new Endpoint[] { this.EndpointA, this.EndpointB };
        }

        public Endpoint this[string name]
        {
            get
            {
                if( string.Compare( this.EndpointA.Label, this.EndpointB.Label, StringComparison.OrdinalIgnoreCase) == 0 )
                    throw new Exception("Cannot resolve endpoint as both endpoint labels are the same.");
                if( string.Compare( this.EndpointA.Label, name, StringComparison.OrdinalIgnoreCase) == 0 )
                    return this.EndpointA;
                if( string.Compare( this.EndpointB.Label, name, StringComparison.OrdinalIgnoreCase) == 0 )
                    return this.EndpointB;
                return null;
            }
        }

        internal Endpoint EndpointA { get; set; }

        internal Endpoint EndpointB { get; set; }
    }
}
