using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Internal
{
    internal class MapDiff
    {
        public IDictionary<TKey, TValue> GetDifferences<TKey, TValue>(IDictionary<TKey, TValue> current, IDictionary<TKey, TValue> old, Func<TValue, TValue, bool> isEqual)
        {
            var allKeys = current.Keys.Union(old.Keys).ToArray();
            var differences = new Dictionary<TKey, TValue>();

            // Get updates
            for (int i = 0; i < allKeys.Length; i++)
            {
                TValue oldValue, newValue;
                bool hasLastKnownValue = old.TryGetValue(allKeys[i], out oldValue);
                bool hasCurrentValue = current.TryGetValue(allKeys[i], out newValue);
                // If value has changed
                if (hasLastKnownValue == true && hasCurrentValue == true && isEqual(oldValue, newValue) == false)
                    differences[allKeys[i]] = newValue;
                // If value has been added
                else if (hasLastKnownValue == false && hasCurrentValue == true)
                    differences[allKeys[i]] = newValue;
                // If value has been removed.. Should never happen
                if (hasLastKnownValue == true && hasCurrentValue == false)
                    differences[allKeys[i]] = default(TValue);
            }
            return differences;
        }
    }
}
