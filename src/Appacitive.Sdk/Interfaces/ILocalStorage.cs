using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Internal
{
    public interface ILocalStorage
    {
        void SetValue(string key, string value);

        string GetValue(string key, string defaultValue = null);

        void Remove(string key);
    }
}
