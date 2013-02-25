using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Appacitive.Sdk.Services;

namespace Appacitive.Sdk
{
    public class Connect
    {
        internal Connect(string relation)
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


        public Connect FromNewArticle(string endpointLabel, Article article )
        {
            this.EndpointALabel = endpointLabel;
            this.EndpointAContent = article;
            this.EndpointAId = null;
            return this;
        }

        public Connect FromExistingArticle(string endpointLabel, string articleId )
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
        }

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
            if (articleA.IsNewInstance == false)
            {
                this.EndpointA = new Endpoint(labelA, articleA.Id);
                this.EndpointB = new Endpoint(labelA, ArticleIdB);
            }
            else
            {
                this.EndpointA = new Endpoint(labelA, null);
                this.EndpointB = new Endpoint(labelA, ArticleIdB);
                this.EndpointA.Content = articleA;
            }
        }

        public Connection(string type, string labelA, Article articleA, string labelB, Article articleB)
            : base(type)
        {
            if (articleA.IsNewInstance == true)
            {
                this.EndpointA = new Endpoint(labelA, null);
                this.EndpointA.Content = articleA;
            }
            else
                this.EndpointA = new Endpoint(labelA, articleA.Id);

            if (articleB.IsNewInstance == true)
            {
                this.EndpointB = new Endpoint(labelA, null);
                this.EndpointB.Content = articleB;
            }
            else
                this.EndpointB = new Endpoint(labelA, articleB.Id);
        }

        public static Connect Create(string relationName)
        {
            return new Connect(relationName);
        }

        public async static Task<Connection> Get(string relation, string id)
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

        internal bool IsNewInstance
        {
            get 
            {
                return string.IsNullOrWhiteSpace(this.Id) || this.Id == "0";
            }
        }

        public Endpoint EndpointA { get; set; }

        public Endpoint EndpointB { get; set; }

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

        protected async override Task<Entity> CreateNewAsync()
        {
            // Create a new article
            IConnectionService service = ObjectFactory.Build<IConnectionService>();
            var response = await service.CreateConnectionAsync(new CreateConnectionRequest()
            {
                Connection = this
            });
            if (response.Status.IsSuccessful == false)
                throw response.Status.ToFault();
            Debug.Assert(response.Connection != null, "If status is successful, then created connection should not be null.");
            return response.Connection;
        }

        protected override void UpdateState(Entity entity)
        {
            var other = entity as Connection;
            if (other == null) return;
            this.EndpointA = other.EndpointA;
            this.EndpointB = other.EndpointB;
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

        internal Article Content { get; set; }

        public string ArticleId { get; set; }

        public string Label { get; set; }
    }
}
