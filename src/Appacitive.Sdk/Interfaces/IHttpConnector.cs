using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk
{
    public interface IHttpConnector
    {
        Task<byte[]> Get(string url, IDictionary<string, string> headers);

        Task<byte[]> Delete(string url, IDictionary<string, string> headers);

        Task<byte[]> Put(string url, IDictionary<string, string> headers, byte[] data);

        Task<byte[]> Post(string url, IDictionary<string, string> headers, byte[] data);
    }
}
