using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HttpClient2 = System.Net.Http.HttpClient;

namespace Appacitive.Sdk
{
    public class HttpClient
    {
        public HttpClient(string url )
        {
            this.Url = url;
            this.ContentType = "application/json; charset=utf-8";
            this.Headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        public string Url { get; set; }

        public string ContentType { get; set; }

        public HttpClient WithHeader(string header, string value)
        {
            if( value != null )
                this.Headers[header] = value;
            return this;
        }

        public static HttpClient WithUrl(string url)
        {
            return new HttpClient(url);
        }

        public IDictionary<string, string> Headers { get; private set; }

        public async Task<byte[]> GetAsync()
        {
            return await ExecuteAsync("GET", null);
        }

        public async Task<Stream> GetAsStreamAsync()
        {
            return await ExecuteAsStreamAsync("GET", null);
        }

        public async Task<byte[]> PostAsyc(byte[] data)
        {
            return await ExecuteAsync("POST", data);
        }

        public async Task<Stream> PostAsStreamAsync(byte[] data)
        {
            return await ExecuteAsStreamAsync("POST", data);
        }

        
        public async Task<byte[]> PutAsyc(byte[] data)
        {
            return await ExecuteAsync("PUT", data);
        }

        private byte[] Execute(string httpMethod, byte[] data)
        {
            var request = CreateRequest(httpMethod);
            if (data != null && data.Length > 0)
            {
                var requestStream = request.GetRequestStream();
                using (requestStream)
                {
                    requestStream.Write(data, 0, data.Length);
                }
            }

            var response = request.GetResponse();
            using (response)
            {
                using (var responseStream = response.GetResponseStream())
                {
                    using (var buffer = new MemoryStream())
                    {
                        responseStream.CopyTo(buffer);
                        return buffer.ToArray();
                    }
                }
            }
        }

        private static readonly byte[] Empty = new byte[0];
        private async Task<byte[]> ExecuteAsync(string httpMethod, byte[] data)
        {
            var client = new HttpClient2();
            HttpResponseMessage response = null;
            ByteArrayContent content = null;
            if (data != null)
            {
                content = new ByteArrayContent(data);
                this.Headers.For(h => content.Headers.Add(h.Key, h.Value));
            }
            else
            {
                this.Headers.For(h => client.DefaultRequestHeaders.Add(h.Key, h.Value));
            }
            
            switch (httpMethod)
            {
                case "GET":
                    response = await client.GetAsync(this.Url);
                    break;
                case "PUT":
                    response = await client.PutAsync(this.Url, content);
                    break;
                case "POST":
                    response = await client.PostAsync(this.Url, content);
                    break;
                case "DELETE":
                    response = await client.DeleteAsync(this.Url);
                    break;
            }
            return await response.Content.ReadAsByteArrayAsync();
        }

        private async Task<Stream> ExecuteAsStreamAsync(string httpMethod, byte[] data)
        {
            var client = new HttpClient2();
            HttpResponseMessage response = null;
            ByteArrayContent content = null;
            if (data != null)
            {
                content = new ByteArrayContent(data);
                this.Headers.For(h => content.Headers.Add(h.Key, h.Value));
            }
            else
            {
                this.Headers.For(h => client.DefaultRequestHeaders.Add(h.Key, h.Value));
            }

            switch (httpMethod)
            {
                case "GET":
                    response = await client.GetAsync(this.Url);
                    break;
                case "PUT":
                    response = await client.PutAsync(this.Url, content);
                    break;
                case "POST":
                    response = await client.PostAsync(this.Url, content);
                    break;
                case "DELETE":
                    response = await client.DeleteAsync(this.Url);
                    break;
            }
            return await response.Content.ReadAsStreamAsync();
        }

        private HttpWebRequest CreateRequest(string method)
        {
            var request = WebRequest.Create(this.Url) as HttpWebRequest;
            // Set http method
            request.Method = method;
            // set the content type
            if (method != "GET")
            {
                if (string.IsNullOrWhiteSpace(this.ContentType) == true)
                    request.ContentType = "application/json";
                else
                    request.ContentType = this.ContentType;
            }
            // set the headers
            foreach (var header in this.Headers.Keys)
                request.Headers[header] = this.Headers[header];
            return request;
        }

        public byte[] Delete()
        {
            return Execute("DELETE", null);
        }

        public Task<byte[]> DeleteAsync()
        {
            return ExecuteAsync("DELETE", null);
        }
    }
}
