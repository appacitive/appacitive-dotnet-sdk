using Appacitive.Sdk.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Appacitive.Sdk
{
    /// <summary>
    /// Class to execute batch operations against the appacitive api platform.
    /// </summary>
    public class APBatch
    {
        private Dictionary<string, APObject> _objectsToCreate = new Dictionary<string, APObject>();
        private Dictionary<string, APConnection> _connectionsToCreate = new Dictionary<string, APConnection>();
        private Dictionary<string, IObjectUpdateRequest> _objectsToUpdate = new Dictionary<string, IObjectUpdateRequest>();
        private Dictionary<string, UpdateConnectionRequest> _connectionsToUpdate = new Dictionary<string, UpdateConnectionRequest>();
        private List<Tuple<EntityInfo, bool>> _objectsToDelete = new List<Tuple<EntityInfo, bool>>();
        private List<EntityInfo> _connectionsToDelete = new List<EntityInfo>();

        private Dictionary<APObjectReference, APObject> _updatedObjects = new Dictionary<APObjectReference, APObject>();
        private Dictionary<APConnectionReference, APConnection> _updatedConnections = new Dictionary<APConnectionReference, APConnection>();
        private Dictionary<APObjectReference, APObject> _createdObjects = new Dictionary<APObjectReference, APObject>();
        private Dictionary<APConnectionReference, APConnection> _createdConnections = new Dictionary<APConnectionReference, APConnection>();
        private List<EntityInfo> _deletedObjects = new List<EntityInfo>();
        private List<EntityInfo> _deletedConnections = new List<EntityInfo>();
        private int _isExecuted = 0;

        /// <summary>
        /// Add a new object to be created when the batch is executed.
        /// </summary>
        /// <param name="obj">New object to be created</param>
        /// <returns>Returns a reference to the object inside the batch. This can be used to get the created object from the batch once it is executed.</returns>
        public APObjectReference AddObjectToCreate(APObject obj)
        {
            // Check if already added and return existing handle.
            var existing = _objectsToCreate.Where(x => Object.ReferenceEquals(x.Value, obj) == true).Select(x => x.Key).SingleOrDefault();
            if (existing != null)
                return new APObjectReference(existing);
            else
            {
                if (string.IsNullOrWhiteSpace(obj.Id) == false)
                    throw new AppacitiveRuntimeException("Cannot add pre created objects inside a batch create operation.");
                var handle = new APObjectReference(GenerateName());
                _objectsToCreate[handle.ObjectHandle] = obj;
                return handle;
            }
        }

        /// <summary>
        /// Add a new connection to be created when the batch is executed.
        /// </summary>
        /// <param name="connection">New connection to be created</param>
        /// <returns>Returns a reference to the connection inside the batch. This can be used to get the created connection from the batch once it is executed.</returns>
        public APConnectionReference AddConnectionToCreate( APConnection connection)
        {
            // Check if already added and return existing handle.
            var existing = _connectionsToCreate.Where(x => Object.ReferenceEquals(x.Value, connection) == true).Select(x => x.Key).SingleOrDefault();
            if (existing != null)
                return new APConnectionReference(existing);
            else
            {
                var handle = new APConnectionReference(GenerateName());
                connection.Id = string.Empty;
                _connectionsToCreate[handle.ObjectHandle] = connection;
                return handle;
            }
            
        }

        /// <summary>
        /// Add an existing object to the batch to be updated when the batch is executed.
        /// </summary>
        /// <param name="id">Id of the object to be updated.</param>
        /// <param name="obj">The object to be updated with the updated fields.</param>
        /// <param name="specificRevision">Any specific revision</param>
        /// <returns>Returns a reference to the object inside the batch. This can be used to get the updated object from the batch once it is executed.</returns>
        public APObjectReference AddObjectToUpdate(string id, APObject obj, int specificRevision = 0)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");
            if (string.IsNullOrWhiteSpace(id) == true)
                throw new ArgumentNullException("id");
            var handle = new APObjectReference(id);
            obj.Id = id;
            _objectsToUpdate[handle.ObjectHandle] = GetUpdateRequest(obj, specificRevision);
            return handle;
        }

        /// <summary>
        /// Add an existing connection to the batch to be updated when the batch is executed.
        /// </summary>
        /// <param name="id">Id of the connection to be updated.</param>
        /// <param name="conn">The connection to be updated with the updated fields.</param>
        /// <param name="specificRevision">Any specific revision</param>
        /// <returns></returns>
        public APConnectionReference AddConnectionToUpdate(string id, APConnection conn, int specificRevision = 0)
        {
            if (conn == null)
                throw new ArgumentNullException("conn");
            if (string.IsNullOrWhiteSpace(id) == true)
                throw new ArgumentNullException("id");

            conn.Id = id;
            var handle = new APConnectionReference(id);
            _connectionsToUpdate[handle.ObjectHandle] = GetUpdateRequest(conn, specificRevision);
            return handle;
        }

        private UpdateConnectionRequest GetUpdateRequest(APConnection conn, int specificRevision)
        {
            return conn.CreateUpdateRequest(specificRevision);
        }

        /// <summary>
        /// Add an existing object to be deleted when the batch api is executed.
        /// </summary>
        /// <param name="type">Type of the object</param>
        /// <param name="id">The id of the object</param>
        /// <param name="deleteEdge">Boolean flag indicating if the delete operation should delete existing connections if any.</param>
        public void AddObjectToDelete(string type, string id, bool deleteEdge = true)
        {
            if (string.IsNullOrWhiteSpace(type) == true)
                throw new ArgumentNullException("type");
            if (string.IsNullOrWhiteSpace(id) == true)
                throw new ArgumentNullException("id");

            this._objectsToDelete.Add(new Tuple<EntityInfo, bool>(new EntityInfo { Type = type, Id = id }, deleteEdge));
        }

        /// <summary>
        /// Add an existing connection to be deleted when the batch api is executed.
        /// </summary>
        /// <param name="type">Type of the connection</param>
        /// <param name="id">The id of the connection</param>
        public void AddConnectionToDelete(string type, string id)
        {
            if (string.IsNullOrWhiteSpace(type) == true)
                throw new ArgumentNullException("type");
            if (string.IsNullOrWhiteSpace(id) == true)
                throw new ArgumentNullException("id");
            this._connectionsToDelete.Add(new EntityInfo { Type = type, Id = id });
        }

        public async Task ExecuteAsync(ApiOptions options = null)
        {
            if (Interlocked.CompareExchange(ref _isExecuted, 1, 0) != 0)
                throw new AppacitiveRuntimeException("ExecuteAsync cannot be invoked multiple times on an APBatch instance.");
            // Execute the batch
            var request = new MultiCallerRequest();
            ApiOptions.Apply(request, options);
            // To create
            _objectsToCreate.For(x => request.ObjectSaveCommands.Add(new ObjectCreateCommand { Name = x.Key, Object = x.Value }));
            _connectionsToCreate.For(x => request.ConnectionSaveCommands.Add(new ConnectionCreateCommand{ Name = x.Key, Connection = x.Value }));
            // To update
            _objectsToUpdate.For(x => request.ObjectSaveCommands.Add(new ObjectUpdateCommand { Name = x.Key, Object = x.Value, Version = x.Value.Revision.ToString() }));
            _connectionsToUpdate.For(x => request.ConnectionSaveCommands.Add(new ConnectionUpdateCommand { Name = x.Key, Connection = x.Value, Version = x.Value.Revision.ToString() }));
            // To delete
            _objectsToDelete.For(x => request.ObjectDeleteCommands.Add(new ObjectDeleteCommand { Type = x.Item1.Type, Id = x.Item1.Id, DeleteConnection = x.Item2 }));
            _connectionsToDelete.For(x => request.ConnectionDeleteCommands.Add(new ConnectionDeleteCommand { Type = x.Type, Id = x.Id }));
            var response = await request.ExecuteAsync();
            if (response.Status.IsSuccessful == false)
                throw response.Status.ToFault();
            foreach (var item in response.AffectedObjects)
            {
                var handle = new APObjectReference(item.Name);
                if (_objectsToCreate.ContainsKey(item.Name) == true)
                    _createdObjects[handle] = item.Object;
                else
                    _updatedObjects[handle] = item.Object;
            }

            foreach (var item in response.AffectedConnections)
            {
                var handle = new APConnectionReference(item.Name);
                if (_connectionsToCreate.ContainsKey(item.Name) == true)
                    _createdConnections[handle] = item.Connection;
                else
                    _updatedConnections[handle] = item.Connection;
            }


            if (response.DeletedConnections != null)
                _deletedConnections.AddRange(response.DeletedConnections.Select(x => new EntityInfo { Type = x.Type, Id = x.Id }));
            if (response.DeletedObjects != null)
                _deletedObjects.AddRange(response.DeletedObjects.Select(x => new EntityInfo { Type = x.Type, Id = x.Id }));
        }

        private string GenerateName()
        {
            return Guid.NewGuid().ToString();
        }

        internal static IObjectUpdateRequest GetUpdateRequest(APObject obj, int specificRevision = 0)
        {
            if (obj is APDevice)
                return (obj as APDevice).CreateUpdateRequest(specificRevision);
            else if (obj is APUser)
                return (obj as APUser).CreateUpdateRequest(specificRevision);
            else
                return obj.CreateUpdateRequest(specificRevision);
        }

        /// <summary>
        /// Map of all newly created objects keyed by the batch object references.
        /// </summary>
        public IDictionary<APObjectReference, APObject> CreatedObjects
        {
            get { return _createdObjects; }
        }

        /// <summary>
        /// Map of all newly created connections keyed by their batch connection references.
        /// </summary>
        public IDictionary<APConnectionReference, APConnection> CreatedConnections
        {
            get { return _createdConnections; }
        }

        /// <summary>
        /// Map of all updated objects keyed by the batch object references.
        /// </summary>
        public IDictionary<APObjectReference, APObject> UpdatedObjects
        {
            get { return _updatedObjects; }
        }

        /// <summary>
        /// Map of all updated connections keyed by their batch connection references.
        /// </summary>
        public IDictionary<APConnectionReference, APConnection> UpdatedConnections
        {
            get { return _updatedConnections; }
        }

        /// <summary>
        /// List of all objects deleted when this batch is executed.
        /// </summary>
        public IEnumerable<EntityInfo> DeletedObjects
        {
            get { return _deletedObjects; }
        }

        /// <summary>
        /// List of all connections deleted when this batch is executed.
        /// </summary>
        public IEnumerable<EntityInfo> DeletedConnections
        {
            get { return _deletedConnections; }
        }
    }

    public class APObjectReference
    {
        internal APObjectReference(string handle)
        {
            this.ObjectHandle = handle;
        }

        public string ObjectHandle { get; private set; }

        public override string ToString()
        {
            return this.ObjectHandle;
        }

        public override bool Equals(object obj)
        {
            if (obj is APObjectReference)
            {
                return StringComparer.OrdinalIgnoreCase.Equals(this.ObjectHandle, (obj as APObjectReference).ObjectHandle);
            }
            else return false;
        }

        public override int GetHashCode()
        {
            return StringComparer.OrdinalIgnoreCase.GetHashCode(this.ObjectHandle);
        }
    }

    public class APConnectionReference
    {
        internal APConnectionReference(string handle)
        {
            this.ObjectHandle = handle;
        }

        public string ObjectHandle { get; private set; }

        public override string ToString()
        {
            return this.ObjectHandle;
        }

        public override bool Equals(object obj)
        {
            if (obj is APConnectionReference)
            {
                return StringComparer.OrdinalIgnoreCase.Equals(this.ObjectHandle, (obj as APConnectionReference).ObjectHandle);
            }
            else return false;
        }

        public override int GetHashCode()
        {
            return StringComparer.OrdinalIgnoreCase.GetHashCode(this.ObjectHandle);
        }
    }
}
