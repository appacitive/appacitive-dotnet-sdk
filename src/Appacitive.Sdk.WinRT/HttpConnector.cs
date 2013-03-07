using Appacitive.Sdk.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
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
            var client = new HttpClient();
            HttpResponseMessage response = null;
            ByteArrayContent content = null;
            if (data != null)
            {
                content = new ByteArrayContent(data);
                if (headers != null)
                {
                    foreach (var key in headers.Keys)
                        content.Headers.Add(key, headers[key]);
                }
            }
            else
            {
                if (headers != null)
                {
                    foreach (var key in headers.Keys)
                        client.DefaultRequestHeaders.Add(key, headers[key]);
                }
            }

            switch (httpMethod)
            {
                case "GET":
                    response = await client.GetAsync(url);
                    break;
                case "PUT":
                    response = await client.PutAsync(url, content);
                    break;
                case "POST":
                    response = await client.PostAsync(url, content);
                    break;
                case "DELETE":
                    response = await client.DeleteAsync(url);
                    break;
            }
            return await response.Content.ReadAsByteArrayAsync();
        }
    }
}
