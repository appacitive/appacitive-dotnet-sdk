using Appacitive.Sdk.Internal;
using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;

#if WINDOWS_PHONE
namespace Appacitive.Sdk.WindowsPhone8
#elif WINDOWS_PHONE7
namespace Appacitive.Sdk.WindowsPhone7
#endif
{
    public class IsolatedLocalStorage : ILocalStorage
    {
        public static readonly IsolatedLocalStorage Instance = new IsolatedLocalStorage();

        private readonly object _syncRoot = new object();

        public void SetValue(string key, string value)
        {
            var settings = IsolatedStorageSettings.ApplicationSettings;
            lock (_syncRoot)
            {
                settings[key] = value;
            }
        }

        public string GetValue(string key, string defaultValue = null)
        {
            string value = null;
            var settings = IsolatedStorageSettings.ApplicationSettings;
            lock (_syncRoot)
            {
                if (settings.TryGetValue<string>(key, out value) == true)
                    return value;
                else return defaultValue;
            }
        }


        public void Remove(string key)
        {
            var settings = IsolatedStorageSettings.ApplicationSettings;
            lock (_syncRoot)
            {
                settings.Remove(key);
            }
        }
    }
}
