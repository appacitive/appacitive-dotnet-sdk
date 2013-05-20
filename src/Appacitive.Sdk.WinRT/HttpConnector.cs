using Appacitive.Sdk.Interfaces;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.WinRT
{
    public class HttpConnector : IHttpConnector
    {
        public async Task<byte[]> GetAsync(string url, IDictionary<string, string> headers)
        {
            
            return await ExecuteAsync("GET", url, headers, null);
        }

        public async Task<byte[]> DeleteAsync(string url, IDictionary<string, string> headers)
        {
            return await ExecuteAsync("DELETE", url, headers, null);
        }

        public async Task<byte[]> PutAsync(string url, IDictionary<string, string> headers, byte[] data)
        {
            return await ExecuteAsync("PUT", url, headers, data);
        }

        public async Task<byte[]> PostAsync(string url, IDictionary<string, string> headers, byte[] data)
        {
            return await ExecuteAsync("POST", url, headers, data);
        }

        private static readonly byte[] Empty = new byte[0];
        private async Task<byte[]> ExecuteAsync(string httpMethod, string url, IDictionary<string, string> headers, byte[] data)
        {
            await Debugger.Log(string.Format("{0} {1}", httpMethod, url));
            var client = new HttpClient();
            var request = new HttpRequestMessage(GetHttpMethod(httpMethod), url);
            if (data != null)
            {
                request.Content = new ByteArrayContent(data);
                await Debugger.Log("Request data:");
                await Debugger.Log(data);
            }
            if (headers != null)
            {
                foreach (var key in headers.Keys)
                    request.Headers.Add(key, headers[key]);
            }
            HttpResponseMessage response = await client.SendAsync(request);
            var responseData = await response.Content.ReadAsByteArrayAsync();
            await Debugger.Log("Response data:");
            await Debugger.Log(responseData);
            return responseData;
        }

        private static readonly Dictionary<string, HttpMethod> HttpMethods = new Dictionary<string, HttpMethod>(StringComparer.OrdinalIgnoreCase)
        {
            { "GET", HttpMethod.Get },
            { "POST", HttpMethod.Post },
            { "PUT", HttpMethod.Put },
            { "DELETE", HttpMethod.Delete },
            { "HEAD", HttpMethod.Head },
            { "OPTIONS", HttpMethod.Options },
        };

        private HttpMethod GetHttpMethod(string httpMethod)
        {
            return HttpMethods[httpMethod];
        }
    }
}
