using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Appacitive.Sdk
{
    public class HttpClient
    {
        public HttpClient(string url )
        {
            this.Url = url;
            this.ContentType = "application/json";
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

        public byte[] Get()
        {   
            return Execute("GET", null);
        }

        public byte[] Post(byte[] data)
        {
            return Execute("POST", data);
        }

        public byte[] Put(byte[] data)
        {
            return Execute("PUT", data);
        }

        public async Task<byte[]> PostAsyc(byte[] data)
        {
            return await ExecuteAsync("POST", data);
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

        private async Task<byte[]> ExecuteAsync(string httpMethod, byte[] data)
        {   
            var request = CreateRequest(httpMethod);
            if (data != null && data.Length > 0)
            {
                var requestStream = await request.GetRequestStreamAsync();
                using (requestStream)
                {
                    await requestStream.WriteAsync(data, 0, data.Length);
                }
            }

            var response = await request.GetResponseAsync();
            using (response)
            {
                using (var responseStream = response.GetResponseStream())
                {
                    using (var buffer = new MemoryStream())
                    {
                        await responseStream.CopyToAsync(buffer, 4096);
                        return buffer.ToArray();
                    }
                }
            }

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
