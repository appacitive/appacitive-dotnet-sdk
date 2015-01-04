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
        /// Creates a new instance of an APConnection objects for use inside a batch api.
        /// This api will create a connection between a new object and a new object via its batch api reference.
        /// </summary>
        /// <param name="type">The name of the relation for which to create a new connection.</param>
        /// <param name="labelA">Label for the first endpoint in the connection.</param>
        /// <param name="objectA">The instance of the new object corresponding to the first endpoint in the connection.</param>
        /// <param name="labelB">Label for the second endpoint in the connection.</param>
        /// <param name="objectReferenceB">Batch object reference to the APObject associated with endpoint B.</param>
        public APConnection(string type, string labelA, APObject objectA, string labelB, APObjectReference objectReferenceB)
            : base(type)
        {
            this.Endpoints = new EndpointPair(new Endpoint(labelA, objectA),new Endpoint(labelB, objectReferenceB));
        }

        /// <summary>
        /// Creates a new instance of an APConnection objects for use inside a batch api.
        /// This api will create a connection between an existing object and a new object via its batch api reference.
        /// </summary>
        /// <param name="type">The name of the relation for which to create a new connection.</param>
        /// <param name="labelA">Label for the first endpoint in the connection.</param>
        /// <param name="objectIdA">Id of the existing object corresponding to the first endpoint in the connection.</param>
        /// <param name="labelB">Label for the second endpoint in the connection.</param>
        /// <param name="objectReferenceB">Batch object reference to the APObject associated with endpoint B.</param>
        public APConnection(string type, string labelA, string objectIdA, string labelB, APObjectReference objectReferenceB)
            : base(type)
        {
            this.Endpoints = new EndpointPair(new Endpoint(labelA, objectIdA), new Endpoint(labelB, objectReferenceB));
        }

        /// <summary>
        /// Creates a new instance of an APConnection objects for use inside a batch api.
        /// This api will create a connection between a two new object via their batch api reference.
        /// </summary>
        /// <param name="type">The name of the relation for which to create a new connection.</param>
        /// <param name="labelA">Label for the first endpoint in the connection.</param>
        /// <param name="objectReferenceA">Batch object reference to the APObject associated with endpoint A.</param>
        /// <param name="labelB">Label for the second endpoint in the connection.</param>
        /// <param name="objectReferenceB">Batch object reference to the APObject associated with endpoint B.</param>
        public APConnection(string type, string labelA, APObjectReference objectReferenceA, string labelB, APObjectReference objectReferenceB)
            : base(type)
        {
            this.Endpoints = new EndpointPair(new Endpoint(labelA, objectReferenceA), new Endpoint(labelB, objectReferenceB));
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
        public async Task<APObject> GetEndpointObjectAsync(string label, ApiOptions options = null)
        {
            if (string.Compare(this.Endpoints.EndpointA.Label, label, StringComparison.OrdinalIgnoreCase) == 0)
                return await this.Endpoints.EndpointA.GetObjectAsync(options);
            if (string.Compare(this.Endpoints.EndpointB.Label, label, StringComparison.OrdinalIgnoreCase) == 0)
                return await this.Endpoints.EndpointB.GetObjectAsync(options);
            throw new AppacitiveRuntimeException("Invalid label " + label);
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
            throw new AppacitiveRuntimeException("Invalid label " + label);
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
        /// <param name="forceUpdate">Setting this flag as True will force an update request even when the state of the object may not have changed locally.</param>
        /// <returns>Returns the saved connection object.</returns>
        public async Task<APConnection> SaveAsync(int specificRevision = 0, bool throwIfAlreadyExists = false, bool forceUpdate = false, ApiOptions options = null)
        {
            try
            {
                EnsureNoBatchReferences();
                await this.SaveEntityAsync(specificRevision, forceUpdate, options);
                return this;
            }
            catch (DuplicateObjectException)
            {
                if (throwIfAlreadyExists == true)
                    throw;
            }
            // Get existing connection.
            return await APConnections.GetAsync(this.Type, this.Endpoints.EndpointA.ObjectId, this.Endpoints.EndpointB.ObjectId);
        }

        private void EnsureNoBatchReferences()
        {
            // Ensure that the endpoints used in this connections are not object references to objects created in a batch api.
            if (this.Endpoints.HasBatchReference == true )
                throw new AppacitiveRuntimeException("Connections with batch object references can only be saved via the batch api.");
        }

        protected override async Task<Entity> FetchAsync(ApiOptions options = null)
        {
            return await APConnections.GetAsync(this.Type, this.Id, options);
        }

        protected async override Task<Entity> CreateNewAsync(ApiOptions options)
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
                    await endpoints[i].Content.SaveAsync(options: options);
                    // Update endpoint object ids
                    endpoints[i].ObjectId = endpoints[i].Content.Id;
                }
            }

            // Create a new object
            var request = new CreateConnectionRequest() { Connection = this };
            ApiOptions.Apply(request, options);
            var response = await request.ExecuteAsync();
            if (response.Status.IsSuccessful == false)
                throw response.Status.ToFault();
            Debug.Assert(response.Connection != null, "If status is successful, then created connection should not be null.");
            return response.Connection;
        }

        protected override void UpdateLastKnown(Entity entity, ApiOptions options)
        {
            base.UpdateLastKnown(entity, options);
            
            var other = entity as APConnection;
            if (other == null) return;
            this.Endpoints.EndpointA = other.Endpoints.EndpointA;
            this.Endpoints.EndpointB = other.Endpoints.EndpointB;
        }

        internal UpdateConnectionRequest CreateUpdateRequest(int specificRevision, ApiOptions options = null)
        {
            return CreateUpdateRequest(this.GetDeltaChanges(), specificRevision, options);
        }

        private UpdateConnectionRequest CreateUpdateRequest(EntityChanges changes, int specificRevision, ApiOptions options)
        {
            var request = new UpdateConnectionRequest { Id = this.Id, Type = this.Type, Revision = specificRevision };
            ApiOptions.Apply(request, options);
            request.Revision = specificRevision;
            if (changes.PropertyUpdates != null && changes.PropertyUpdates.Count > 0)
                changes.PropertyUpdates.For(x => request.PropertyUpdates[x.Key] = x.Value);
            if (changes.AttributeUpdates != null && changes.AttributeUpdates.Count > 0)
                changes.AttributeUpdates.For(x => request.AttributeUpdates[x.Key] = x.Value);
            if (changes.AddedTags != null)
                request.AddedTags.AddRange(changes.AddedTags);
            if (changes.RemovedTags != null)
                request.RemovedTags.AddRange(changes.RemovedTags);

            return request;
        }

        protected override async Task<Entity> UpdateAsync(EntityChanges changes, int specificRevision, ApiOptions options, bool forceUpdate)
        {
            var request = CreateUpdateRequest(changes, specificRevision, options);

            // Check if an update is needed.
            if (request.PropertyUpdates.Count == 0 &&
                request.AttributeUpdates.Count == 0 &&
                request.AddedTags.Count == 0         &&
                request.RemovedTags.Count == 0  &&
                forceUpdate == false )
                return this;

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
        /// <param name="objectId">The id of the existing APObject.</param>
        public Endpoint(string label, string objectId)
        {
            this.Label = label;
            this.ObjectId = objectId;
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

        /// <summary>
        /// Creates a new endpoint corresponding to a new APObject.
        /// </summary>
        /// <param name="label">The label for the endpoint.</param>
        /// <param name="objReference">Batch api reference to an APObject associated with the endpoint.</param>
        public Endpoint(string label, APObjectReference objReference)
        {
            this.Label = label;
            this.ObjectReference = objReference;
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

        internal APObjectReference ObjectReference { get; set; }

        internal string Type { get; set; }


        /// <summary>
        /// Gets the APObject instance associated with this endpoint.
        /// </summary>
        /// <returns>The APObject associated with this endpoint.</returns>
        /// <param name="options">Request specific api options. These will override the global settings for the app for this request.</param>
        public async Task<APObject> GetObjectAsync(ApiOptions options = null)
        {
            if (this.Content != null)
                return this.Content;
            else 
                return await APObjects.GetAsync(this.Type, this.ObjectId, options: options);
        }


        internal bool HasBatchReference
        {
            get
            {
                return this.ObjectReference != null;
            }
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

        public bool HasBatchReference 
        {
            get
            {
                return this.EndpointA.HasBatchReference == true || this.EndpointB.HasBatchReference == true;
            }
        }
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
        private APObjectReference EndpointAObjectReference { get; set; }
        private APObjectReference EndpointBObjectReference { get; set; }
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

        public FluentAPConnection FromBatchObjectReference(string endpointLabel, APObjectReference reference)
        {
            this.EndpointALabel = endpointLabel;
            this.EndpointAContent = null;
            this.EndpointAId = null;
            this.EndpointAObjectReference = reference;
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

        public APConnection ToBatchObjectReference(string endpointLabel, APObjectReference reference)
        {
            this.EndpointBLabel = endpointLabel;
            this.EndpointBContent = null;
            this.EndpointBId = null;
            this.EndpointBObjectReference = reference;
            return Build();
        }

        private APConnection Build()
        {
            APConnection conn = null;
            // Handle object references.
            if (this.EndpointAObjectReference != null && this.EndpointBObjectReference != null)
                return new APConnection(this.RelationName, this.EndpointALabel, this.EndpointAObjectReference, this.EndpointBLabel, this.EndpointBObjectReference);
            if (this.EndpointAObjectReference != null)
            {
                if( this.EndpointBContent != null )
                    return new APConnection(this.RelationName, this.EndpointBLabel, this.EndpointBContent, this.EndpointALabel, this.EndpointAObjectReference);
                else
                    return new APConnection(this.RelationName, this.EndpointBLabel, this.EndpointBId, this.EndpointALabel, this.EndpointAObjectReference);
            }
            if (this.EndpointBObjectReference != null)
            {
                if (this.EndpointAContent != null)
                    return new APConnection(this.RelationName, this.EndpointALabel, this.EndpointAContent, this.EndpointBLabel, this.EndpointBObjectReference);
                else
                    return new APConnection(this.RelationName, this.EndpointALabel, this.EndpointAId, this.EndpointBLabel, this.EndpointBObjectReference);
            }




            
            if (this.EndpointAContent != null && this.EndpointBContent != null)
                conn = new APConnection(this.RelationName, EndpointALabel, EndpointAContent, EndpointBLabel, EndpointBContent);
            else if (this.EndpointAContent == null && this.EndpointBContent != null)
                conn = new APConnection(this.RelationName, EndpointBLabel, EndpointBContent, EndpointALabel, EndpointAId);
            else if (this.EndpointAContent != null && this.EndpointBContent == null)
                conn = new APConnection(this.RelationName, EndpointALabel, EndpointAContent, EndpointBLabel, EndpointBId);
            else
                conn = new APConnection(this.RelationName, EndpointALabel, EndpointAId, EndpointBLabel, EndpointBId);
            conn.Endpoints.EndpointA.ObjectReference = this.EndpointAObjectReference;
            conn.Endpoints.EndpointB.ObjectReference = this.EndpointAObjectReference;
            return conn;
        }
    }
}
