using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk
{
    public class EntityChanges
    {
        public EntityChanges()
        {
            this.PropertyUpdates = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            this.AttributeUpdates = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            this.AddedTags = new List<string>();
            this.RemovedTags = new List<string>();
        }

        public Dictionary<string, object> PropertyUpdates { get; set;}
        public Dictionary<string, string> AttributeUpdates { get; set; }
        public List<string> AddedTags { get; set; }
        public List<string> RemovedTags { get; set; }
    }
}
