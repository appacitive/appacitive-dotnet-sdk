using Appacitive.Sdk.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Appacitive.Sdk
{
    public class APBatch
    {
        private Dictionary<string, APObject> _objectsToCreate = new Dictionary<string, APObject>();
        private Dictionary<string, APConnection> _connectionsToCreate = new Dictionary<string, APConnection>();
        private Dictionary<string, IObjectUpdateRequest> _objectsToUpdate = new Dictionary<string, IObjectUpdateRequest>();
        private Dictionary<string, UpdateConnectionRequest> _connectionsToUpdate = new Dictionary<string, UpdateConnectionRequest>();
        private List<Tuple<EntityInfo, bool>> _objectsToDelete = new List<Tuple<EntityInfo, bool>>();
        private List<EntityInfo> _connectionsToDelete = new List<EntityInfo>();

        private List<APObject> _updatedObjects = new List<APObject>();
        private List<APConnection> _updatedConnections = new List<APConnection>();
        private List<APObject> _createdObjects = new List<APObject>();
        private List<APConnection> _createdConnections = new List<APConnection>();
        private List<EntityInfo> _deletedObjects = new List<EntityInfo>();
        private List<EntityInfo> _deletedConnections = new List<EntityInfo>();

        private int _isExecuted = 0;

        public APBatch AddObjectsToCreate(params APObject[] objects)
        {
            if (objects == null) return this;
            foreach (var obj in objects)
            {
                if (obj == null) continue;
                obj.Id = string.Empty;
                _objectsToCreate[GenerateName()] = obj;
            }
            return this;
        }

        public APBatch AddConnectionsToCreate(params APConnection[] connections)
        {
            if (connections == null) return this;
            foreach (var connection in connections)
            {
                if (connection == null) continue;
                connection.Id = string.Empty;
                _connectionsToCreate[GenerateName()] = connection;
            }
            return this;
        }

        public APBatch AddObjectToUpdate(string id, APObject obj, int specificRevision = 0)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");
            if (string.IsNullOrWhiteSpace(id) == true)
                throw new ArgumentNullException("id");

            obj.Id = id;
            _objectsToUpdate[GenerateName()] = GetUpdateRequest(obj, specificRevision);
            return this;
        }

        public APBatch AddConnectionToUpdate(string id, APConnection conn, int specificRevision = 0)
        {
            if (conn == null)
                throw new ArgumentNullException("conn");
            if (string.IsNullOrWhiteSpace(id) == true)
                throw new ArgumentNullException("id");

            conn.Id = id;
            _connectionsToUpdate[GenerateName()] = GetUpdateRequest(conn, specificRevision);
            return this;
        }

        private UpdateConnectionRequest GetUpdateRequest(APConnection conn, int specificRevision)
        {
            return conn.CreateUpdateRequest(specificRevision);
        }

        public APBatch AddObjectToDelete(string type, string id, bool deleteEdge = true)
        {
            if (string.IsNullOrWhiteSpace(type) == true)
                throw new ArgumentNullException("type");
            if (string.IsNullOrWhiteSpace(id) == true)
                throw new ArgumentNullException("id");

            this._objectsToDelete.Add(new Tuple<EntityInfo, bool>(new EntityInfo { Type = type, Id = id }, deleteEdge));
            return this;
        }

        public APBatch AddConnectionToDelete(string type, string id)
        {
            if (string.IsNullOrWhiteSpace(type) == true)
                throw new ArgumentNullException("type");
            if (string.IsNullOrWhiteSpace(id) == true)
                throw new ArgumentNullException("id");
            this._connectionsToDelete.Add(new EntityInfo { Type = type, Id = id });
            return this;
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

            foreach (var item in _objectsToCreate)
            {
                var match = response.AffectedObjects.SingleOrDefault(x => x.Name.Equals(item.Key));
                if (match != null)
                    _createdObjects.Add(match.Object);
            }
            _updatedObjects.AddRange(response.AffectedObjects.Select( x => x.Object).Except(_createdObjects));

            foreach( var item in _connectionsToCreate )
            {
                var match = response.AffectedConnections.SingleOrDefault(x => x.Name.Equals(item.Key));
                if (match != null)
                    _createdConnections.Add(match.Connection);
            }
            _updatedConnections.AddRange(response.AffectedConnections.Select(x => x.Connection).Except(_createdConnections));

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

        public IEnumerable<APObject> CreatedObjects
        {
            get { return _createdObjects; }
        }

        public IEnumerable<APConnection> CreatedConnections
        {
            get { return _createdConnections; }
        }

        public IEnumerable<APObject> UpdatedObjects
        {
            get { return _updatedObjects; }
        }

        public IEnumerable<APConnection> UpdatedConnections
        {
            get { return _updatedConnections; }
        }

        public IEnumerable<EntityInfo> DeletedObjects
        {
            get { return _deletedObjects; }
        }

        public IEnumerable<EntityInfo> DeletedConnections
        {
            get { return _deletedConnections; }
        }
    }
}
