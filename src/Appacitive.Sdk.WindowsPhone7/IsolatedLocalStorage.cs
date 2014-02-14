using Appacitive.Sdk.Internal;
using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;

namespace Appacitive.Sdk.WindowsPhone7
{
    public class IsolatedLocalStorage : ILocalStorage
    {   
        public void SetValue(string key, string value)
        {
            var settings = IsolatedStorageSettings.ApplicationSettings;
            settings[key] = value;
        }

        public string GetValue(string key, string defaultValue = null)
        {
            string value = null;
            var settings = IsolatedStorageSettings.ApplicationSettings;
            if (settings.TryGetValue<string>(key, out value) == true)
                return value;
            else return defaultValue;
        }
    }
}
