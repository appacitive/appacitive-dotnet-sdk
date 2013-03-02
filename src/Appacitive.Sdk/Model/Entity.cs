using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Appacitive.Sdk.Services;

namespace Appacitive.Sdk
{
    public abstract partial class Entity : DynamicObject, INotifyPropertyChanged
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
        private ConcurrentDictionary<string, string> _currentFields = new ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        private ConcurrentDictionary<string, string> _lastKnownFields = new ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        private ConcurrentDictionary<string, string> _currentAttributes = new ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        private ConcurrentDictionary<string, string> _lastKnownAttributes = new ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
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

        public IEnumerable<KeyValuePair<string, string>> Properties
        {
            get
            {
                return this.Clone(_currentFields);
            }
        }

        public IEnumerable<KeyValuePair<string, string>> Attributes
        {
            get
            {
                return this.Clone(_currentAttributes);
            }
        }

        public string GetAttribute(string name)
        {
            string value;
            if (_currentAttributes.TryGetValue(name, out value) == true)
                return value;
            else return null;
        }

        public void SetAttribute(string name, string value)
        {
            _currentAttributes[name] = value;
        }

        public void RemoveAttribute(string name)
        {
            string value;
            _currentFields.TryRemove("@" + name, out value);
        }

        private void FirePropertyChanged([CallerMemberName] string propertyName = "")
        {
            var privateCopy = _properyChanged;
            if (privateCopy != null)
                privateCopy(this, new PropertyChangedEventArgs(propertyName));
        }

        public string this[string name]
        {
            get
            {
                string value;
                if (_currentFields.TryGetValue(name, out value) == true)
                    return value;
                else return null;
            }
            set
            {
                var oldValue = this[name];
                _currentFields[name] = value;
                // Raise property changed event
                if (oldValue != value)
                    FirePropertyChanged();
            }
        }

        public async Task SaveAsync()
        {
            if (string.IsNullOrWhiteSpace(this.Id) == true)
                await CreateNewEntityAsync();
            else
                await UpdateEntityAsync();
        }

        private async Task CreateNewEntityAsync()
        {
            // create new
            var entity = await CreateNewAsync();
            UpdateLastKnown(entity);
        }
        
        private async Task UpdateEntityAsync()
        {
            // 1. Get property differences
            var propertyDifferences = _currentFields.GetModifications(_lastKnownFields);

            // 2. Get attribute differences
            var attributeDifferences = _currentAttributes.GetModifications(_lastKnownAttributes);

            // 2. Get tags changes
            IEnumerable<string> addedTags, removedTags;
            _lastKnownTags.GetDifferences(_currentTags, out addedTags, out removedTags);

            // 3. update the article
            var updated = await UpdateAsync(propertyDifferences, attributeDifferences, addedTags, removedTags);
            if (updated != null)
            {
                // 4. Update the last known state based on the differences
                UpdateLastKnown(updated);
            }
        }

        private void UpdateLastKnown(Entity entity)
        {   
            var newLastKnownFields = new ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var newCurrentFields = new ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            entity.Properties.For(x => 
                {
                    newLastKnownFields[x.Key] = x.Value;
                    newCurrentFields[x.Key] = x.Value;
                });
            var newLastKnownAttributes = new ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var newCurrentAttributes = new ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
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

        private IDictionary<string, string> Clone(IDictionary<string, string> map)
        {
            lock (_syncRoot)
            {
                return new ConcurrentDictionary<string, string>(map, StringComparer.OrdinalIgnoreCase);
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

        protected abstract Task<Entity> UpdateAsync(IDictionary<string, string> propertyUpdates, IDictionary<string, string> attributeUpdates, IEnumerable<string> addedTags, IEnumerable<string> removedTags);
    }

    internal class DictionaryDifference
    {
        public IDictionary<string, string> GetDifferences(IDictionary<string, string> current, IDictionary<string, string> old)
        {
            var allKeys = current.Keys.Union(old.Keys).ToArray();
            var differences = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            // Get updates
            for (int i = 0; i < allKeys.Length; i++)
            {
                string oldValue, newValue;
                bool hasLastKnownValue = old.TryGetValue(allKeys[i], out oldValue);
                bool hasCurrentValue = current.TryGetValue(allKeys[i], out newValue);

                // If value has changed
                if (hasLastKnownValue == true && hasCurrentValue == true && oldValue != newValue)
                    differences[allKeys[i]] = newValue;
                // If value has been added
                else if (hasLastKnownValue == false && hasCurrentValue == true)
                    differences[allKeys[i]] = newValue;
                // If value has been removed.. Should never happen
                if (hasLastKnownValue == true && hasCurrentValue == false)
                    differences[allKeys[i]] = null;
            }
            return differences;
        }
    }
}