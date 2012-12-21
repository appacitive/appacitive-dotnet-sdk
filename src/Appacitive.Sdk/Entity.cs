using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk
{
    public class Entity
    {
        private ConcurrentDictionary<string, string> _current = new ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        
        private ConcurrentDictionary<string, string> _lastKnown = new ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        
    }
}
