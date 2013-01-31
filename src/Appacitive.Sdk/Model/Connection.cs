using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk
{
    public class Connection : Entity
    {
        public Connection(string type) : base(type) { }

        public Connection(string type, string id) : base(type, id) { }

        public Connection(string type, string labelA, string articleIdA, string labelB, string ArticleIdB) 
            : base(type)
        {
            this.EndpointA = new Endpoint(labelA, articleIdA);
            this.EndpointB = new Endpoint(labelA, ArticleIdB);
        }

        public Connection(string type, string labelA, Article articleA, string labelB, string ArticleIdB)
            : base(type)
        {
            this.EndpointA = new Endpoint(labelA, null);
            this.EndpointB = new Endpoint(labelA, ArticleIdB);
            this.EndpointAContent = articleA;
        }

        public Connection(string type, string labelA, Article articleA, string labelB, Article articleB)
            : base(type)
        {
            this.EndpointA = new Endpoint(labelA, null);
            this.EndpointB = new Endpoint(labelA, null);
            this.EndpointAContent = articleA;
            this.EndpointBContent = articleB;
        }

        public Endpoint EndpointA { get; set; }

        public Endpoint EndpointB { get; set; }

        internal Article EndpointAContent { get; set; }

        internal Article EndpointBContent { get; set; }

        public string RelationId { get; set; }

        internal bool CreateEndpointA
        {
            get { return string.IsNullOrWhiteSpace(this.EndpointA.ArticleId); }
        }

        internal bool CreateEndpointB
        {
            get { return string.IsNullOrWhiteSpace(this.EndpointB.ArticleId); }
        }

        protected override Entity CreateNew()
        {
            throw new NotImplementedException();
        }

        protected override Task<Entity> CreateNewAsync()
        {
            throw new NotImplementedException();
        }

        protected override Entity Update(IDictionary<string, string> propertyUpdates, IDictionary<string, string> attributeUpdates, IEnumerable<string> addedTags, IEnumerable<string> removedTags)
        {
            throw new NotImplementedException();
        }

        protected override Task<Entity> UpdateAsync(IDictionary<string, string> propertyUpdates, IDictionary<string, string> attributeUpdates, IEnumerable<string> addedTags, IEnumerable<string> removedTags)
        {
            throw new NotImplementedException();
        }
    }

    public class Endpoint
    {
        public Endpoint(string label, string articleId)
        {
            this.Label = label;
            this.ArticleId = articleId;
        }

        public string ArticleId { get; set; }

        public string Label { get; set; }
    }
}
