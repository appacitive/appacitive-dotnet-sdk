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
using System.Collections;
using Appacitive.Sdk.Internal;


namespace Appacitive.Sdk
{
    public abstract partial class Entity : INotifyPropertyChanged
    {
        public Entity(Entity existing)
            : this(existing.Type, existing.Id)
        {
            // Copy
            this.CreatedBy = existing.CreatedBy;
            this.LastUpdatedBy = existing.LastUpdatedBy;
            this.CreatedAt = existing.CreatedAt;
            this.LastUpdatedAt = existing.LastUpdatedAt;

            // Copy properties
            lock (_syncRoot)
            {
                foreach (var property in existing.Properties)
                    this[property.Key] = property.Value;
                foreach (var attr in existing.Attributes)
                {
                    this._currentAttributes[attr.Key] = attr.Value;
                    this._lastKnownAttributes[attr.Key] = attr.Value;
                }
                foreach (var tag in existing.Tags)
                {
                    this._currentTags.Add(tag);
                    this._lastKnownTags.Add(tag);
                }
            }
        }

        public Entity(string type)
        {
            this.Type = type;
        }

        public Entity(string type, string id)
            : this(type)
        {
            this.Id = id;
        }

        // Represents the saved state of the object
        private IDictionary<string, object> _currentFields = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        private IDictionary<string, object> _lastKnownFields = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        private IDictionary<string, string> _currentAttributes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        private IDictionary<string, string> _lastKnownAttributes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        private List<string> _currentTags = new List<string>();
        private List<string> _lastKnownTags = new List<string>();
        private readonly object _syncRoot = new object();
        private readonly object _eventSyncRoot = new object();

        PropertyChangedEventHandler _properyChanged;
        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                lock (_eventSyncRoot)
                {
                    _properyChanged += value;
                }
            }
            remove
            {
                lock (_eventSyncRoot)
                {
                    _properyChanged -= value;
                }
            }
        }

        public string Type { get; internal set; }

        public string Id { get; internal set; }

        public string CreatedBy { get; set; }

        public int Revision { get; set; }

        public string LastUpdatedBy { get; internal set; }

        public DateTime CreatedAt { get; internal set; }

        public DateTime LastUpdatedAt { get; internal set; }

        public IEnumerable<KeyValuePair<string, Value>> Properties
        {
            get
            {
                IDictionary<string, object> clone = null;
                lock (_syncRoot)
                {
                    clone = new Dictionary<string, object>(_currentFields, StringComparer.OrdinalIgnoreCase);
                }
                return clone.Select(x => new KeyValuePair<string, Value>(x.Key, Value.FromObject(x.Value)));
            }
        }

        public bool HasProperty(string propertyName)
        {
            lock (_syncRoot)
            {
                return _currentFields.ContainsKey(propertyName);
            }
        }

        public IEnumerable<KeyValuePair<string, string>> Attributes
        {
            get
            {
                lock (_syncRoot)
                {
                    return new Dictionary<string, string>(_currentAttributes, StringComparer.OrdinalIgnoreCase);
                }
            }
        }
        public async Task AddItemsAsync<T>(string property, params T[] values)
        {
            ApiOptions options = null;
            await AddItemsAsync(property, false, options, values);
        }

        public async Task AddItemsAsync<T>(string property, ApiOptions options, params T[] values)
        {
            await AddItemsAsync(property, false, options, values);
        }
        public async Task AddItemsAsync<T>(string property, bool addUniquely, params T[] values)
        {
            ApiOptions options = null;
            await AddItemsAsync(property, addUniquely, options, values);
        }
        public async Task AddItemsAsync<T>(string property, bool addUniquely, ApiOptions options, params T[] values)
        {
            var request = new UpdateListItemsRequest { Type = this.Type, Id = this.Id, Property = property, AddUniquely = addUniquely };
            ApiOptions.Apply(request, options);
            request.ItemsToAdd.AddRange(values.Where(x => x != null).Select(x => x.ToString()));
            var response = await request.ExecuteAsync();
            if (response.Status.IsSuccessful == false)
                throw response.Status.ToFault();
            var updated = response.Object.GetList<T>(property);
            this.SetList(property, updated, true);
        }

        public async Task RemoveItemsAsync<T>(string property, params T[] values)
        {
            ApiOptions options = null;
            await RemoveItemsAsync(property, options, values);
        }

        public async Task RemoveItemsAsync<T>(string property, ApiOptions options, params T[] values)
        {
            var request = new UpdateListItemsRequest { Type = this.Type, Id = this.Id, Property = property };
            ApiOptions.Apply(request, options);
            request.ItemsToRemove.AddRange(values.Where(x => x != null).Select(x => x.ToString()));
            var response = await request.ExecuteAsync();
            if (response.Status.IsSuccessful == false)
                throw response.Status.ToFault();
            var updated = response.Object.GetList<T>(property);
            this.SetList(property, updated, true);
        }


        /// <summary>
        /// Allows for atomically adding items without duplication to a multi valued property.
        /// </summary>
        /// <typeparam name="T">Type of the value</typeparam>
        /// <param name="property">The multi valued property</param>
        /// <param name="values">Item values</param>
        public void AddItems<T>(string property, params T[] values)
        {
            AddItems<T>(property, false, values);
        }

        /// <summary>
        /// Allows for atomically adding an item to a multi valued property.
        /// </summary>
        /// <typeparam name="T">Type of the value</typeparam>
        /// <param name="property">The multi valued property</param>
        /// <param name="values">Item values</param>
        /// <param name="allowDuplicates">Allow duplicate entries to be added.</param>
        public void AddItems<T>(string property, bool allowDuplicates, params T[] values)
        {
            // Validate type
            if (values == null)
                throw new ArgumentException("Cannot add null item to multi valued property.");
            Guard.ValidateAllowedPrimitiveTypes(typeof(T));
            List<string> items = null;
            lock (_syncRoot)
            {
                object value = null;
                // Set atomically if null.
                if (_currentFields.TryGetValue(property, out value) == true)
                {
                    if (value != null && value.IsMultiValued() == false)
                        throw new ArgumentException("Existing value of property " + property + " is not multi valued.");
                    items = value as List<string>;
                }
                if (items == null)
                {
                    items = new List<string>();
                    _currentFields[property] = items;
                }
                for (int i = 0; i < values.Length; i++)
                {
                    var content = values[i].AsString();
                    if (allowDuplicates == true)
                        items.Add(content);
                    else if (items.Contains(content) == false)
                        items.Add(content);
                }
            }
        }

        /// <summary>
        /// Allows for removing a value from a multi valued property.
        /// </summary>
        /// <typeparam name="T">Type of the value to remove.</typeparam>
        /// <param name="property">Name of the property.</param>
        /// <param name="item">The item value to remove.</param>
        /// <returns>Returns true of the item exists in the collection.</returns>
        public bool RemoveItems<T>(string property, T item, bool removeFirstOccurenceOnly = false)
        {
            // Validate type
            if (item == null)
                throw new ArgumentException("Cannot remove null item to multi valued property.");
            Guard.ValidateAllowedPrimitiveTypes(typeof(T));
            var strContent = item.AsString();
            List<string> items = null;
            lock (_syncRoot)
            {
                object value = null;
                if (_currentFields.TryGetValue(property, out value) == true)
                {
                    if (value != null && value.IsMultiValued() == false)
                        throw new ArgumentException("Existing value of property " + property + " is not multi valued.");
                    items = value as List<string>;
                }
                if (items == null)
                    return false;
                else if (removeFirstOccurenceOnly == true)
                    return items.Remove(strContent);
                else
                    return items.RemoveAllOccurences(strContent);
            }
        }

        public string GetAttribute(string name)
        {
            return ReadAttribute(name);
        }

        internal void SetAttribute(string name, string value, bool updateLastKnown)
        {
            lock (_syncRoot)
            {
                if (updateLastKnown) _lastKnownAttributes[name] = value;
                _currentAttributes[name] = value;
            }
        }

        public void SetAttribute(string name, string value)
        {
            SetAttribute(name, value, false);
        }

        public void RemoveAttribute(string name)
        {
            lock (_syncRoot)
            {
                _currentAttributes.Remove(name);
            }
        }

        protected void FirePropertyChanged(string propertyName)
        {
            var privateCopy = _properyChanged;
            if (privateCopy != null)
                privateCopy(this, new PropertyChangedEventArgs(propertyName));
        }

        internal void SetField(string name, Value value, bool updateLastKnown)
        {
            var oldValue = this[name];

            // Value is null
            if (value == null || value is NullValue)
                WriteField(name, null, updateLastKnown);
            else if (value is SingleValue)
                WriteField(name, ((SingleValue)value).Value, updateLastKnown);
            else if (value is MultiValue)
                WriteField(name, ((MultiValue)value).GetValues<string>().ToList(), updateLastKnown);

            // Raise property changed event
            if (oldValue.Equals(value) == false)
                FirePropertyChanged(name);
        }

        /// <summary>
        /// Updates this object with its latest state on the server.
        /// </summary>
        /// <param name="options">Request specific api options. These will override the global settings for the app for this request.</param>
        public async Task RefreshAsync(ApiOptions options = null)
        {
            if (string.IsNullOrWhiteSpace(this.Id) == true || this.Id == "0") 
                throw new AppacitiveRuntimeException("Cannot refresh object from server as it has not been saved on the server yet.");
            // Fetch the object
            var stateFromServer = await this.FetchAsync(options);
            // Update the last known state.
            UpdateLastKnown(stateFromServer, options);
        }

        protected abstract Task<Entity> FetchAsync(ApiOptions options = null);

        public Value this[string name]
        {
            get
            {
                // Read value
                var value = ReadField(name);
                if (value == null)
                    return NullValue.Instance;
                else if (value.IsMultiValued() == true)
                    return new MultiValue(value as IEnumerable);
                else return new SingleValue(value);
            }
            set
            {
                SetField(name, value, false);
            }
        }

        protected async Task SaveEntityAsync(int specificRevision = 0, bool forceUpdate = false, ApiOptions options = null)
        {
            if (string.IsNullOrWhiteSpace(this.Id) == true)
                await CreateNewEntityAsync(options);
            else
                await UpdateEntityAsync(specificRevision, forceUpdate, options);
        }

        private async Task CreateNewEntityAsync(ApiOptions options )
        {
            // create new
            var entity = await CreateNewAsync(options);
            UpdateLastKnown(entity, options);
        }


        private async Task UpdateEntityAsync(int specificRevision, bool forceUpdate, ApiOptions options)
        {
            // 1. Get property differences
            var propertyDifferences = _currentFields.GetModifications(_lastKnownFields, (x, y) =>
                {
                    var strX = x.AsString();
                    var strY = y.AsString();
                    return strX == strY;
                });

            // 2. Get attribute differences
            var attributeDifferences = _currentAttributes.GetModifications(_lastKnownAttributes, (x, y) => x == y);

            // 2. Get tags changes
            IEnumerable<string> addedTags, removedTags;
            _lastKnownTags.GetDifferences(_currentTags, out addedTags, out removedTags);

            // 3. update the object
            var updated = await UpdateAsync(propertyDifferences, attributeDifferences, addedTags, removedTags, specificRevision,  forceUpdate, options);
            if (updated != null)
            {
                // 4. Update the last known state based on the differences
                UpdateLastKnown(updated, options);
            }
        }

        private object ExtractValue(Value value)
        {
            if (value is NullValue)
                return null;
            if (value is SingleValue)
                return value.GetValue<string>();
            if (value is MultiValue)
                return value.GetValues<string>().ToList();
            else throw new ArgumentException(value.GetType().Name + " is not a supported value type.");
        }

        protected virtual void UpdateLastKnown(Entity entity, ApiOptions options )
        {
            var newLastKnownFields = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            var newCurrentFields = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            entity.Properties.For(x =>
                {
                    newLastKnownFields[x.Key] = newCurrentFields[x.Key] = ExtractValue(x.Value);
                });
            var newLastKnownAttributes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var newCurrentAttributes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            entity.Attributes.For(x =>
                {
                    newLastKnownAttributes[x.Key] = x.Value;
                    newCurrentAttributes[x.Key] = x.Value;
                });



            lock (_syncRoot)
            {
                this.Id = this.Id ?? entity.Id;
                this.CreatedBy = entity.CreatedBy;
                this.LastUpdatedBy = entity.LastUpdatedBy;
                this.CreatedAt = entity.CreatedAt;
                this.LastUpdatedAt = entity.LastUpdatedAt;
                this.Revision = entity.Revision;
                _lastKnownFields = newLastKnownFields;
                _lastKnownAttributes = newLastKnownAttributes;
                _currentFields = newCurrentFields;
                _currentAttributes = newCurrentAttributes;
                _lastKnownTags = entity.Tags.ToList();
            }
        }

        

        private void WriteField(string name, object value, bool updateLastKnown)
        {
            lock (_syncRoot)
            {
                if (updateLastKnown) _lastKnownFields[name] = value;
                _currentFields[name] = value;
            }
        }

        private object ReadField(string name)
        {
            object result = null;
            lock (_syncRoot)
            {
                if (_currentFields.TryGetValue(name, out result) == true)
                    return result;
                else return null;
            }
        }

        private string ReadAttribute(string name)
        {
            string result = null;
            lock (_syncRoot)
            {
                if (_currentAttributes.TryGetValue(name, out result) == true)
                    return result;
                else return null;
            }
        }

        public IEnumerable<string> Tags
        {
            get { return _currentTags; }
        }

        public void RemoveTags(IEnumerable<string> tags)
        {
            lock (_syncRoot)
            {
                tags.For(t =>
                    {
                        _currentTags.Remove(t);
                    });
            }
        }

        public void RemoveTag(string tag)
        {
            lock (_syncRoot)
            {
                _currentTags.Remove(tag);
            }
        }

        internal void AddTags(IEnumerable<string> tags, bool updateLastKnown)
        {
            lock (_syncRoot)
            {
                tags.For(x => AddTag(x, updateLastKnown));
            }
        }

        public void AddTags(IEnumerable<string> tags)
        {
            AddTags(tags, false);
        }

        internal void AddTag(string tag, bool updateLastKnown)
        {
            lock (_syncRoot)
            {
                if (updateLastKnown && _lastKnownTags.Contains(tag) == false) _lastKnownTags.Add(tag);
                if (_currentTags.Contains(tag) == false) _currentTags.Add(tag);
            }
        }

        public void AddTag(string tag)
        {
            AddTag(tag, false);
        }

        protected abstract Task<Entity> CreateNewAsync(ApiOptions options);

        protected abstract Task<Entity> UpdateAsync(IDictionary<string, object> propertyUpdates, IDictionary<string, string> attributeUpdates, IEnumerable<string> addedTags, IEnumerable<string> removedTags, int specificRevision, bool forceUpdate, ApiOptions options);

        public void ClearList(string property)
        {
            lock (_syncRoot)
            {
                object value = null;
                if (_currentFields.TryGetValue(property, out value) == true)
                {
                    if (value != null && value.IsMultiValued() == false)
                        throw new ArgumentException("Existing value for property " + property + " is not multi valued.");
                }
                _currentFields[property] = new List<string>();
            }
        }

        public async Task IncrementAsync(string property, uint increment, ApiOptions options = null)
        {
            var request = new AtomicCountersRequest { Type = this.Type, Id = this.Id, IncrementBy = increment, Property = property, };
            ApiOptions.Apply(request, options);
            var response = await request.ExecuteAsync();
            if (response.Status.IsSuccessful == false)
                throw response.Status.ToFault();
            var newValue = response.Object.Get<string>(property);
            this.Set<string>(property, newValue, true);
        }

        public async Task DecrementAsync(string property, uint decrement, ApiOptions options = null)
        {
            var request = new AtomicCountersRequest { Type = this.Type, Id = this.Id, DecrementBy = decrement, Property = property };
            ApiOptions.Apply(request, options);
            var response = await request.ExecuteAsync();
            if (response.Status.IsSuccessful == false)
                throw response.Status.ToFault();
            var newValue = response.Object.Get<string>(property);
            this.Set<string>(property, newValue, true);
        }

        public string ToJson()
        {
            var serializer = ObjectFactory.Build<IJsonSerializer>();
            return serializer.SerializeAsString(this);
        }

        internal static Entity CreateInstance(Type objectType)
        {
            throw new NotImplementedException();
        }
    }
}