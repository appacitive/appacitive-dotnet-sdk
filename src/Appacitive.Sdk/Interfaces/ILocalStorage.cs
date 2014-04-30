using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Internal
{
    /// <summary>
    /// Interface for the local storage on a device.
    /// </summary>
    public interface ILocalStorage
    {
        /// <summary>
        /// Saves the given key value pair to the device's local storage.
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        void SetValue(string key, string value);

        /// <summary>
        /// Gets the value for the given key from the device's local storage.
        /// </summary>
        /// <param name="key">Key to lookup.</param>
        /// <param name="defaultValue">Default value to return incase the key is not found.</param>
        string GetValue(string key, string defaultValue = null);

        /// <summary>
        /// Removes the data for the given key from the device's local storage.
        /// </summary>
        /// <param name="key">Key to remove.</param>
        void Remove(string key);
    }
}
