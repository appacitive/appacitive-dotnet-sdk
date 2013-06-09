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
        public Entity(string type)
        {
            this.Type = type;
        }

        public Entity(string type, string id)
            : this(type)
        {
            this.Id = id;
        }
        
        // Represents the saved state of the article
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

        public string Type { get; private set; }

        public string Id { get; internal set; }

        public string CreatedBy { get; set; }

        public int Revision { get; set; }

        public string LastUpdatedBy { get; internal set; }

        public DateTime UtcCreateDate { get; internal set; }

        public DateTime UtcLastUpdated { get; internal set; }

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

        public void SetAttribute(string name, string value)
        {
            WriteAttribute(name, value);
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
                var oldValue = this[name];
                // Value is null
                if (value == null || value is NullValue)
                    WriteField(name, null);
                else if (value is SingleValue)
                    WriteField(name, ((SingleValue)value).Value);
                else if(value is MultiValue )
                    WriteField(name, ((MultiValue)value).GetValues<string>().ToList());

                // Raise property changed event
                if( oldValue.Equals(value) == false )
                    FirePropertyChanged(name);
            }
        }

        public async Task SaveAsync(int specificRevision = 0)
        {
            if (string.IsNullOrWhiteSpace(this.Id) == true)
                await CreateNewEntityAsync();
            else
                await UpdateEntityAsync(specificRevision);
        }

        private async Task CreateNewEntityAsync()
        {
            // create new
            var entity = await CreateNewAsync();
            UpdateLastKnown(entity);
        }
        
        private async Task UpdateEntityAsync(int specificRevision)
        {
            // 1. Get property differences
            var propertyDifferences = _currentFields.GetModifications(_lastKnownFields, (x, y) =>
                {
                    var strX = x.AsString();
                    var strY = y.AsString();
                    return strX == strY;
                });

            // 2. Get attribute differences
            var attributeDifferences = _currentAttributes.GetModifications(_lastKnownAttributes, (x,y) => x == y );

            // 2. Get tags changes
            IEnumerable<string> addedTags, removedTags;
            _lastKnownTags.GetDifferences(_currentTags, out addedTags, out removedTags);

            // 3. update the article
            var updated = await UpdateAsync(propertyDifferences, attributeDifferences, addedTags, removedTags, specificRevision);
            if (updated != null)
            {
                // 4. Update the last known state based on the differences
                UpdateLastKnown(updated);
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

        private void UpdateLastKnown(Entity entity)
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
                this.UtcCreateDate = entity.UtcCreateDate;
                this.UtcLastUpdated = entity.UtcLastUpdated;
                _lastKnownFields = newLastKnownFields;
                _lastKnownAttributes = newLastKnownAttributes;
                _currentFields = newCurrentFields;
                _currentAttributes = newCurrentAttributes;
                _lastKnownTags = entity.Tags.ToList();
                UpdateState(entity);
            }
        }

        protected virtual void UpdateState(Entity entity)
        {
        }

        private void WriteField(string name, object value)
        {
            lock (_syncRoot)
            {
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

        private void WriteAttribute(string name, string value)
        {
            lock (_syncRoot)
            {
                _currentAttributes[name] = value;
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

        public void AddTags(IEnumerable<string> tags)
        {
            lock( _syncRoot )
            {
                tags.For( x => 
                 {
                     if( _currentTags.Contains(x) == false )
                         _currentTags.Add(x);
                 });
            }
            
        }

        public void AddTag( string tag )
        {
            lock( _syncRoot )
            {
                if( _currentTags.Contains(tag) == false )
                    _currentTags.Add(tag);
            }
        }

        protected abstract Task<Entity> CreateNewAsync();

        protected abstract Task<Entity> UpdateAsync(IDictionary<string, object> propertyUpdates, IDictionary<string, string> attributeUpdates, IEnumerable<string> addedTags, IEnumerable<string> removedTags, int specificRevision);

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
    }
}