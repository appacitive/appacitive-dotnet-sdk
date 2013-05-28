using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Realtime
{
    public interface IHttpConnector
    {
        Task<byte[]> GetAsync(string url, IDictionary<string, string> headers);

        Task<byte[]> DeleteAsync(string url, IDictionary<string, string> headers);

        Task<byte[]> PutAsync(string url, IDictionary<string, string> headers, byte[] data);

        Task<byte[]> PostAsync(string url, IDictionary<string, string> headers, byte[] data);
    }
}
