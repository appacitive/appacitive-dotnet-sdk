using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Internal
{
    /// <summary>
    /// Interface for the http connector for handling HTTP operations.
    /// </summary>
    public interface IHttpConnector
    {
        /// <summary>
        /// Gets the response for given HTTP GET request.
        /// </summary>
        /// <param name="url">Request url</param>
        /// <param name="headers">Request headers</param>
        /// <returns>Response data.</returns>
        Task<byte[]> GetAsync(string url, IDictionary<string, string> headers);

        /// <summary>
        /// Gets the response for given HTTP DELETE request.
        /// </summary>
        /// <param name="url">Request url</param>
        /// <param name="headers">Request headers</param>
        /// <returns>Response data.</returns>
        Task<byte[]> DeleteAsync(string url, IDictionary<string, string> headers);

        /// <summary>
        /// Gets the response for given HTTP PUT request.
        /// </summary>
        /// <param name="url">Request url</param>
        /// <param name="headers">Request headers</param>
        /// <param name="data">Request data</param>
        /// <returns>Response data.</returns>
        Task<byte[]> PutAsync(string url, IDictionary<string, string> headers, byte[] data);

        /// <summary>
        /// Gets the response for given HTTP POST request.
        /// </summary>
        /// <param name="url">Request url</param>
        /// <param name="headers">Request headers</param>
        /// <param name="data">Request data</param>
        /// <returns>Response data.</returns>
        Task<byte[]> PostAsync(string url, IDictionary<string, string> headers, byte[] data);
    }
}
