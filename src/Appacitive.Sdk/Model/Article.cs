using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Appacitive.Sdk
{
    public sealed partial class Article : INotifyPropertyChanged
    {
        public Article(string type)
        {
            this.Type = type;
            this.Tags = new List<string>();
        }

        // Represents the saved state of the article
        private ConcurrentDictionary<string, string> _currentFields = new ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        private ConcurrentDictionary<string, string> _lastKnownFields = new ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase);

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

        internal string SchemaId { get; set; }

        public string Id { get; internal set; }

        public string CreatedBy { get; set; }

        public string LastUpdatedBy { get; internal set; }

        public DateTime UtcCreateDate { get; internal set; }

        public DateTime UtcLastUpdated { get; internal set; }

        public IEnumerable<KeyValuePair<string, string>> Properties
        {
            get
            {
                return this.Clone(_currentFields).Where(x => x.Key.StartsWith("@") == false);
            }
        }

        public IEnumerable<KeyValuePair<string, string>> Attributes
        {
            get
            {
                return this.Clone(_currentFields).Where(x => x.Key.StartsWith("@") == true);
            }
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
                var oldValue = _currentFields[name];
                _currentFields[name] = value;
                // Raise property changed event
                if (oldValue != value)
                    FirePropertyChanged();
            }
        }

        public void Save()
        {
            if (string.IsNullOrWhiteSpace(this.Id) == true)
                CreateNew();
            else
                Update();
        }

        private void Update()
        {
            // 1. Get differences to be updates based on last known state
            var differences = GetDifferences(_currentFields, _lastKnownFields);
            if (differences == null || differences.Count == 0)
                return;

            // 2. Update the article.
            // var appacitiveClient = Appacitive.CreateClient();
            // var article = appacitiveClient.UpdateArticle(this.Id, this.Type, differences);

            // 3. Update the last known state based on the differences
            // UpdateLastKnown(article);
        }

        private void UpdateLastKnown(Article article)
        {
            this.Id = this.Id ?? article.Id;
            this.CreatedBy = article.CreatedBy;
            this.LastUpdatedBy = article.LastUpdatedBy;
            this.UtcCreateDate = article.UtcCreateDate;
            this.UtcLastUpdated = article.UtcLastUpdated;

            var newLastKnown = new ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            article.Properties.For(x => newLastKnown[x.Key] = x.Value);
            article.Attributes.For(x => newLastKnown["@" + x.Key] = x.Value);

            // Is this correct ?? 
            ConcurrentDictionary<string, string> oldLastKnown, afterExchange;
            do
            {
                // Get original value of last known
                oldLastKnown = _lastKnownFields;
                // Try update to new value
                afterExchange = Interlocked.CompareExchange(ref _lastKnownFields, newLastKnown, oldLastKnown);
            } while (afterExchange != oldLastKnown); // Keep repeating until successful. Ideally this should never fail.
        }

        private IDictionary<string, string> Clone(IDictionary<string, string> map)
        {
            lock (_syncRoot)
            {
                return new ConcurrentDictionary<string, string>(map, StringComparer.OrdinalIgnoreCase);
            }
        }

        private IDictionary<string, string> GetDifferences(ConcurrentDictionary<string, string> currentState, ConcurrentDictionary<string, string> lastKnownState)
        {
            var current = Clone(currentState);
            var lastKnown = Clone(lastKnownState);
            return new DictionaryDifference().GetDifferences(current, lastKnown);
        }

        private void CreateNew()
        {
            // Create a new article
            // var client = Appacitive.CreateClient();
            // var created = client.CreateArticle(this);
            // Update the last known
            // UpdateLastKnown(created);
        }

        public List<string> Tags { get; private set; }
    }

    public class DictionaryDifference
    {
        public IDictionary<string, string> GetDifferences(IDictionary<string, string> current, IDictionary<string, string> lastKnown)
        {
            var allKeys = current.Keys.Union(lastKnown.Keys).ToArray();
            var differences = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            // Get updates
            for (int i = 0; i < allKeys.Length; i++)
            {
                string oldValue, newValue;
                bool hasLastKnownValue = lastKnown.TryGetValue(allKeys[i], out oldValue);
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