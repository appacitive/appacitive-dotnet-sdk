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
    public sealed partial class Article : INotifyPropertyChanged
    {
        public static Article Get(string type, string id)
        {
            var service = ObjectFactory.Build<IArticleService>();
            var response = service.GetArticle(new GetArticleRequest() { Id = id, Type = type });
            if (response.Status.IsSuccessful == false)
                throw response.Status.ToFault();
            Debug.Assert(response.Article != null, "For a successful get call, article should always be returned.");
            return response.Article;
        }

        public async static Task<Article> GetAsync(string type, string id)
        {
            var service = ObjectFactory.Build<IArticleService>();
            var response = await service.GetArticleAsync(new GetArticleRequest() { Id = id, Type = type });
            if (response.Status.IsSuccessful == false)
                throw response.Status.ToFault();
            Debug.Assert(response.Article != null, "For a successful get call, article should always be returned.");
            return response.Article;
        }

        public static void Delete(string type, string id)
        {
            var service = ObjectFactory.Build<IArticleService>();
            var status = service.DeleteArticle(new DeleteArticleRequest() { Id = id, Type = type });
            if (status.IsSuccessful == false)
                throw status.ToFault();
        }

        public async static Task DeleteAsync(string type, string id)
        {
            var service = ObjectFactory.Build<IArticleService>();
            var status = await service.DeleteArticleAsync(new DeleteArticleRequest() { Id = id, Type = type });
            if (status.IsSuccessful == false)
                throw status.ToFault();
        }
        

        public Article(string type)
        {
            this.Type = type;
        }

        public Article(string type, string id) : this(type)
        {
            this.Id = id;
        }

        // Represents the saved state of the article
        private ConcurrentDictionary<string, string> _currentFields = new ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        private ConcurrentDictionary<string, string> _lastKnownFields = new ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
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

        public string GetAttribute(string name)
        {
            return this["@" + name];
        }

        public void SetAttribute(string name, string value)
        {
            this["@" + name] = value;
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

        public void Save()
        {
            if (string.IsNullOrWhiteSpace(this.Id) == true)
                CreateNew();
            else
                Update();
        }

        public async void SaveAsync()
        {
            if (string.IsNullOrWhiteSpace(this.Id) == true)
                await CreateNewAsync();
            else
                await UpdateAsync();
        }

        private void Update()
        {
            // 1. Get differences to be updates based on last known state
            var differences = GetDifferences(_currentFields, _lastKnownFields);
            if (differences == null || differences.Count == 0)
                return;

            // 2. Update the article.
            var articleService = ObjectFactory.Build<IArticleService>();
            var request = new UpdateArticleRequest()
                {
                    SessionToken = AppacitiveContext.SessionToken,
                    Environment = AppacitiveContext.Environment,
                    UserToken = AppacitiveContext.UserToken,
                    Verbosity = AppacitiveContext.Verbosity,
                    Id = this.Id,
                    Type = this.Type
                };
            differences.For(x => request.PropertyUpdates[x.Key] = x.Value);
            // Add tags changes
            IEnumerable<string> addedTags, removedTags;
            _lastKnownTags.GetDifferences(_currentTags, out addedTags, out removedTags);
            request.AddedTags.AddRange(addedTags);
            request.RemovedTags.AddRange(removedTags);

            //  update the article
            var response = articleService.UpdateArticle(request);
            if (response.Status.IsSuccessful == false)
                throw response.Status.ToFault();
            
            // 3. Update the last known state based on the differences
            Debug.Assert(response.Article != null, "If status is successful, then updated article should not be null.");
            UpdateLastKnown(response.Article);
        }

        private async Task UpdateAsync()
        {
            // 1. Get differences to be updates based on last known state
            var differences = GetDifferences(_currentFields, _lastKnownFields);
            if (differences == null || differences.Count == 0)
                return;

            // 2. Update the article.
            var articleService = ObjectFactory.Build<IArticleService>();
            var request = new UpdateArticleRequest()
            {
                SessionToken = AppacitiveContext.SessionToken,
                Environment = AppacitiveContext.Environment,
                UserToken = AppacitiveContext.UserToken,
                Verbosity = AppacitiveContext.Verbosity,
                Id = this.Id,
                Type = this.Type
            };
            differences.For(x => request.PropertyUpdates[x.Key] = x.Value);

            // Add tags changes
            IEnumerable<string> addedTags, removedTags;
            _lastKnownTags.GetDifferences(_currentTags, out addedTags, out removedTags);
            request.AddedTags.AddRange(addedTags);
            request.RemovedTags.AddRange(removedTags);

            //  update the article
            var response = await articleService.UpdateArticleAsync(request);
            if (response.Status.IsSuccessful == false)
                throw response.Status.ToFault();

            // 3. Update the last known state based on the differences
            Debug.Assert(response.Article != null, "If status is successful, then updated article should not be null.");
            UpdateLastKnown(response.Article);
        }

        private void UpdateLastKnown(Article article)
        {
            lock (_syncRoot)
            {
                this.Id = this.Id ?? article.Id;
                this.CreatedBy = article.CreatedBy;
                this.LastUpdatedBy = article.LastUpdatedBy;
                this.UtcCreateDate = article.UtcCreateDate;
                this.UtcLastUpdated = article.UtcLastUpdated;

                var newLastKnown = new ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                article.Properties.For(x => newLastKnown[x.Key] = x.Value);
                article.Attributes.For(x => newLastKnown["@" + x.Key] = x.Value);
                _lastKnownFields = newLastKnown;
                _lastKnownTags = article.Tags.ToList();
            }
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
            var service = ObjectFactory.Build<IArticleService>();
            var response = service.CreateArticle(new CreateArticleRequest()
                {
                    Article = this
                });
            if (response.Status.IsSuccessful == false)
                throw response.Status.ToFault();

            // 3. Update the last known state based on the differences
            Debug.Assert(response.Article != null, "If status is successful, then created article should not be null.");
            // Update the last known
            UpdateLastKnown(response.Article);
        }

        private async Task CreateNewAsync()
        {
            // Create a new article
            var service = ObjectFactory.Build<IArticleService>();
            var response = await service.CreateArticleAsync(new CreateArticleRequest()
            {
                Article = this
            });
            if (response.Status.IsSuccessful == false)
                throw response.Status.ToFault();

            // 3. Update the last known state based on the differences
            Debug.Assert(response.Article != null, "If status is successful, then created article should not be null.");
            // Update the last known
            UpdateLastKnown(response.Article);
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
    }

    internal class DictionaryDifference
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