using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Wcf
{
    public class InProcSession : ISession
    {
        private static ConcurrentDictionary<string, object> _sessionStore = new ConcurrentDictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        
        
        public object this[string key]
        {
            get
            {
                object value = null;
                if (_sessionStore.TryGetValue(key, out value) == true)
                    return value;
                else return null;
                
            }
            set
            {
                if (value != null)
                    _sessionStore[key] = value;
                else 
                    Remove(key);
            }
        }

        public void Remove(string key)
        {
            object value = null;
            _sessionStore.TryRemove(key, out value);
        }
    }
}
