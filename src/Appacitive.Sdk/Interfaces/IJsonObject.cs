using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Internal
{
    /// <summary>
    /// Represents an object which can be converted to Json.
    /// </summary>
    public interface IJsonObject
    {
        /// <summary>
        /// Returns the json representation of this object.
        /// </summary>
        /// <returns>Json string.</returns>
        string AsString();
    }
}
