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
    /// Represents a connection object on the Appacitive platform.
    /// </summary>
    public class APConnection : Entity
    {
        /// <summary>
        /// Creates a new instance of an APConnection for the given relation type.
        /// </summary>
        /// <param name="type">The name of the relation for which to create a new connection.</param>
        public APConnection(string type) : base(type) 
        {
            this.Endpoints = new EndpointPair(null, null);
        }

        /// <summary>
        /// Creates a new APConnection instance corresponding to an existing connection object.
        /// This does not retrieve the existing instance from the server side.
        /// </summary>
        /// <param name="type">The name of the relation for which to create a new connection.</param>
        /// <param name="id">The id of the existing connection object.</param>
        public APConnection(string type, string id) : base(type, id) 
        {
            this.Endpoints = new EndpointPair(null, null);
        }

        /// <summary>
        /// Creates a new instance of an APConnection object between two existing APObjects.
        /// </summary>
        /// <param name="type">The name of the relation for which to create a new connection.</param>
        /// <param name="labelA">Label for the first endpoint in the connection.</param>
        /// <param name="objectIdA">Id of the existing object corresponding to the first endpoint in the connection.</param>
        /// <param name="labelB">Label for the second endpoint in the connection.</param>
        /// <param name="objectIdB">Id of the existing object corresponding to the second endpoint in the connection.</param>
        public APConnection(string type, string labelA, string objectIdA, string labelB, string objectIdB) 
            : base(type)
        {
            var ep1 = new Endpoint(labelA, objectIdA);
            var ep2 = new Endpoint(labelB, objectIdB);
            this.Endpoints = new EndpointPair(ep1, ep2);
        }

        /// <summary>
        /// Creates a new instance of an APConnection object between a new and an existing APObject instance.
        /// </summary>
        /// <param name="type">The name of the relation for which to create a new connection.</param>
        /// <param name="labelA">Label for the first endpoint in the connection.</param>
        /// <param name="objectA">The instance of the new object corresponding to the first endpoint in the connection.</param>
        /// <param name="labelB">Label for the second endpoint in the connection.</param>
        /// <param name="objectIdB">Id of the existing object corresponding to the second endpoint in the connection.</param>
        public APConnection(string type, string labelA, APObject objectA, string labelB, string objectIdB)
            : base(type)
        {
            Endpoint ep1, ep2;
            if (objectA.IsNewInstance == false)
            {
                ep1 = new Endpoint(labelA, objectA.Id);
                ep2 = new Endpoint(labelB, objectIdB);
            }
            else
            {
                string nullId = null;
                ep1 = new Endpoint(labelA, nullId);
                ep2 = new Endpoint(labelB, objectIdB);
                ep1.Content = objectA;
            }
            this.Endpoints = new EndpointPair(ep1, ep2);
        }

        /// <summary>
        /// Creates a new instance of an APConnection object between two new APObject instances.
        /// </summary>
        /// <param name="type">The name of the relation for which to create a new connection.</param>
        /// <param name="labelA">Label for the first endpoint in the connection.</param>
        /// <param name="objectA">The instance of the new object corresponding to the first endpoint in the connection.</param>
        /// <param name="labelB">Label for the second endpoint in the connection.</param>
        /// <param name="objectB">The instance of the new object corresponding to the second endpoint in the connection.</param>
        public APConnection(string type, string labelA, APObject objectA, string labelB, APObject objectB)
            : base(type)
        {
            Endpoint ep1, ep2;
            string nullId = null;
            if (objectA.IsNewInstance == true)
            {
                ep1 = new Endpoint(labelA, nullId);
                ep1.Content = objectA;
            }
            else
                ep1 = new Endpoint(labelA, objectA.Id);

            if (objectB.IsNewInstance == true)
            {   
                ep2 = new Endpoint(labelB, nullId);
                ep2.Content = objectB;
            }
            else
                ep2 = new Endpoint(labelB, objectB.Id);

            this.Endpoints = new EndpointPair(ep1, ep2);
        }

        /// <summary>
        /// Fluent method to create a new APConnection instance for the given connection type.
        /// </summary>
        /// <param name="connectionType">The name of the relation type for which to create a new connection.</param>
        public static FluentAPConnection New(string connectionType)
        {
            return new FluentAPConnection(connectionType);
        }

        
        /// <summary>
        /// Indicates whether or not the current connection is associated with an existing connection object on the server side.
        /// </summary>
        internal bool IsNewInstance
        {
            get 
            {
                return string.IsNullOrWhiteSpace(this.Id) || this.Id == "0";
            }
        }

        /// <summary>
        /// Gets the pair of endpoints associated with this connection.
        /// </summary>
        public EndpointPair Endpoints { get; set; }

        /// <summary>
        /// Gets the APObject associated with the endpoint with the given label.
        /// </summary>
        /// <param name="label">Label of the endpoint</param>
        /// <returns>The APObject instance associated with the specified endpoint.</returns>
        public async Task<APObject> GetEndpointObjectAsync(string label)
        {
            if (string.Compare(this.Endpoints.EndpointA.Label, label, StringComparison.OrdinalIgnoreCase) == 0)
                return await this.Endpoints.EndpointA.GetObjectAsync();
            if (string.Compare(this.Endpoints.EndpointB.Label, label, StringComparison.OrdinalIgnoreCase) == 0)
                return await this.Endpoints.EndpointB.GetObjectAsync();
            throw new AppacitiveApiException("Invalid label " + label);
        }

        /// <summary>
        /// Gets the id of the APObject associated with this connection at the endpoint with the given label.
        /// </summary>
        /// <param name="label">The label of the endpoint</param>
        /// <returns>The id of the APObject for the given endpoint.</returns>
        public string GetEndpointId(string label)
        {
            if (string.Compare(this.Endpoints.EndpointA.Label, label, StringComparison.OrdinalIgnoreCase) == 0)
                return this.Endpoints.EndpointA.ObjectId;
            if (string.Compare(this.Endpoints.EndpointB.Label, label, StringComparison.OrdinalIgnoreCase) == 0)
                return this.Endpoints.EndpointB.ObjectId;
            throw new AppacitiveApiException("Invalid label " + label);
        }

        /// <summary>
        /// Creates or updates the current APConnection object on the server side.
        /// Incase any of the endpoints contains a new APObject, then that object will be created as well on the server side.
        /// </summary>
        /// <param name="specificRevision">
        /// Revision number for this connection instance. 
        /// Used for <a href="http://en.wikipedia.org/wiki/Multiversion_concurrency_control">Multiversion Concurrency Control</a>.
        /// If this version does not match on the server side, the Save operation will fail. Passing 0 disables the revision check.
        /// </param>
        /// <param name="throwIfAlreadyExists">Flag indicating that the operation should throw incase a connection is being created when it already exists on the server side. Passing false will return the existing instance of the connection.</param>
        /// <returns>Returns the saved connection object.</returns>
        public async Task<APConnection> SaveAsync(int specificRevision = 0, bool throwIfAlreadyExists = false)
        {
            try
            {
                await this.SaveEntityAsync(specificRevision);
                return this;
            }
            catch (AppacitiveApiException ex)
            {
                if (throwIfAlreadyExists == true)
                    throw;
                if (ex.Code != ErrorCodes.DuplicateConnection)
                    throw;
            }
            // Get existing connection.
            return await APConnections.GetAsync(this.Type, this.Endpoints.EndpointA.ObjectId, this.Endpoints.EndpointB.ObjectId);
        }

        protected async override Task<Entity> CreateNewAsync()
        {
            // Handling for special case when endpoint contains a new object or device.
            // Since these cannot be created on the fly when creating a new connection.
            var endpoints = this.Endpoints.ToArray();
            for (int i = 0; i < endpoints.Length; i++)
            {
                if (endpoints[i].CreateEndpoint == false) continue;
                if (endpoints[i].Content.Type.Equals("user", StringComparison.OrdinalIgnoreCase) == true ||
                    endpoints[i].Content.Type.Equals("device", StringComparison.OrdinalIgnoreCase) == true)
                {
                    // Create the object
                    await endpoints[i].Content.SaveAsync();
                    // Update endpoint object ids
                    endpoints[i].ObjectId = endpoints[i].Content.Id;
                }
            }

            // Create a new object
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
            var other = entity as APConnection;
            if (other == null) return;
            this.Endpoints.EndpointA = other.Endpoints.EndpointA;
            this.Endpoints.EndpointB = other.Endpoints.EndpointB;
        }

        protected override async Task<Entity> UpdateAsync(IDictionary<string, object> propertyUpdates, IDictionary<string, string> attributeUpdates, IEnumerable<string> addedTags, IEnumerable<string> removedTags, int specificRevision)
        {
            var request = new UpdateConnectionRequest{ Id = this.Id, Type = this.Type, Revision = specificRevision };
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
            Debug.Assert(response.Connection != null, "If status is successful, then updated connection should not be null.");
            return response.Connection;
        }

    }

    /// <summary>
    /// Represents an endpoint in an APConnection.
    /// </summary>
    public class Endpoint
    {
        /// <summary>
        /// Creates a new endpoint corresponding to an existing APObject.
        /// </summary>
        /// <param name="label">The label for the endpoint.</param>
        /// <param name="obectId">The id of the existing APObject.</param>
        public Endpoint(string label, string obectId)
        {
            this.Label = label;
            this.ObjectId = obectId;
        }

        /// <summary>
        /// Creates a new endpoint corresponding to a new APObject.
        /// </summary>
        /// <param name="label">The label for the endpoint.</param>
        /// <param name="content">The new APObject associated with the endpoint.</param>
        public Endpoint(string label, APObject content)
        {
            this.Label = label;
            this.Content = content; 
            if( content != null )
                this.ObjectId = content.Id;
        }

        internal bool CreateEndpoint
        {
            get { return string.IsNullOrWhiteSpace(this.ObjectId); }
        }

        internal APObject Content { get; set; }

        /// <summary>
        /// Gets the object id for the APObject associated with this endpoint.
        /// </summary>
        public string ObjectId { get; set; }

        /// <summary>
        /// Gets the label for this endpoint.
        /// </summary>
        public string Label { get; set; }

        internal string Type { get; set; }

        /// <summary>
        /// Gets the APObject instance associated with this endpoint.
        /// </summary>
        /// <returns>The APObject associated with this endpoint.</returns>
        public async Task<APObject> GetObjectAsync()
        {
            if (this.Content != null)
                return this.Content;
            else 
                return await APObjects.GetAsync(this.Type, this.ObjectId);
        }
    }

    /// <summary>
    /// Represents a pair of endpoints in an APConnection.
    /// </summary>
    public class EndpointPair
    {
        /// <summary>
        /// Creates a new EndpointPair with the given endpoints.
        /// </summary>
        /// <param name="ep1">Endpoint 1</param>
        /// <param name="ep2">Endpoint 2</param>
        public EndpointPair(Endpoint ep1, Endpoint ep2)
        {
            this.EndpointA = ep1;
            this.EndpointB = ep2;
        }

        /// <summary>
        /// Returns the two endpoints for this instance as an array.
        /// </summary>
        /// <returns>Array of endpoints.</returns>
        public Endpoint[] ToArray()
        {
            return new Endpoint[] { this.EndpointA, this.EndpointB };
        }

        /// <summary>
        /// Gets the endpoint with the given label.
        /// </summary>
        /// <param name="label">The label for the endpoint to get.</param>
        /// <returns>The endpoint with the matching label.</returns>
        public Endpoint this[string label]
        {
            get
            {
                if( string.Compare( this.EndpointA.Label, this.EndpointB.Label, StringComparison.OrdinalIgnoreCase) == 0 )
                    throw new AppacitiveRuntimeException("Cannot resolve endpoint as both endpoint labels are the same.");
                if (string.Compare(this.EndpointA.Label, label, StringComparison.OrdinalIgnoreCase) == 0)
                    return this.EndpointA;
                if (string.Compare(this.EndpointB.Label, label, StringComparison.OrdinalIgnoreCase) == 0)
                    return this.EndpointB;
                return null;
            }
        }

        internal Endpoint EndpointA { get; set; }

        internal Endpoint EndpointB { get; set; }
    }

    /// <summary>
    /// Fluent interface for creating a new instance of an APConnection.
    /// </summary>
    public class FluentAPConnection
    {
        internal FluentAPConnection(string relation)
        {
            this.RelationName = relation;
        }

        private string RelationName { get; set; }
        private string EndpointALabel { get; set; }
        private string EndpointAId { get; set; }
        private APObject EndpointAContent { get; set; }
        private string EndpointBLabel { get; set; }
        private string EndpointBId { get; set; }
        private APObject EndpointBContent { get; set; }


        /// <summary>
        /// Create a connection with a new APObject instance. 
        /// </summary>
        /// <param name="endpointLabel">The label for the endpoint.</param>
        /// <param name="obj">The instance of the new APObject.</param>
        public FluentAPConnection FromNewObject(string endpointLabel, APObject obj)
        {
            this.EndpointALabel = endpointLabel;
            this.EndpointAContent = obj;
            this.EndpointAId = null;
            return this;
        }

        /// <summary>
        /// Create a connection with an existing APObject.
        /// </summary>
        /// <param name="endpointLabel">The label for the endpoint.</param>
        /// <param name="objectId">The id of the existing APObject.</param>
        public FluentAPConnection FromExistingObject(string endpointLabel, string objectId)
        {
            this.EndpointALabel = endpointLabel;
            this.EndpointAContent = null;
            this.EndpointAId = objectId;
            return this;
        }

        /// <summary>
        /// Create a connection with a new APObject instance.
        /// </summary>
        /// <param name="endpointLabel">The label for the endpoint.</param>
        /// <param name="obj">The instance of the new APObject.</param>
        public APConnection ToNewObject(string endpointLabel, APObject obj)
        {
            this.EndpointBLabel = endpointLabel;
            this.EndpointBContent = obj;
            this.EndpointBId = null;
            return Build();
        }

        /// <summary>
        /// Create a connection with an existing APObject.
        /// </summary>
        /// <param name="endpointLabel">The label for the endpoint.</param>
        /// <param name="objectId">The id of the existing APObject.</param>
        public APConnection ToExistingObject(string endpointLabel, string objectId)
        {
            this.EndpointBLabel = endpointLabel;
            this.EndpointBContent = null;
            this.EndpointBId = objectId;
            return Build();
        }

        private APConnection Build()
        {
            APConnection conn = null;
            if (this.EndpointAContent != null && this.EndpointBContent != null)
                conn = new APConnection(this.RelationName, EndpointALabel, EndpointAContent, EndpointBLabel, EndpointBContent);
            else if (this.EndpointAContent == null && this.EndpointBContent != null)
                conn = new APConnection(this.RelationName, EndpointBLabel, EndpointBContent, EndpointALabel, EndpointAId);
            else if (this.EndpointAContent != null && this.EndpointBContent == null)
                conn = new APConnection(this.RelationName, EndpointALabel, EndpointAContent, EndpointBLabel, EndpointBId);
            else
                conn = new APConnection(this.RelationName, EndpointALabel, EndpointAId, EndpointBLabel, EndpointBId);
            return conn;
        }
    }
}
