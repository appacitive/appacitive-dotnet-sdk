using System;
using Appacitive.Sdk.Internal;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace Appacitive.Sdk.iOS
{
	public class CachedLocalStorage : ILocalStorage
	{
		public CachedLocalStorage (ILocalStorage storage)
		{
			_storage = storage;
		}

		private ILocalStorage _storage;
		private static ConcurrentDictionary<string, string> _cache = new ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase);

		public void SetValue (string key, string value)
		{
			string outValue;
			if (value == null) 
			{
				if (_cache.ContainsKey (key) == true)
					_cache.TryRemove(key, out outValue);
			} 
			else 
			{
				_cache [key] = value;
			}
			_storage.SetValue (key, value);
		}

		public string GetValue (string key, string defaultValue = null)
		{
			string value = null;
			if (_cache.TryGetValue (key, out value) == true)
				return value;
			// Get value from backing storage.
			value = _storage.GetValue(key, defaultValue);
			if (value == null)
				return defaultValue;
			_cache [key] = value;
			return value;
		}

		public void Remove (string key)
		{
			string value;
			_cache.TryRemove(key, out value);
			_storage.Remove (key);
		}

	}
}

