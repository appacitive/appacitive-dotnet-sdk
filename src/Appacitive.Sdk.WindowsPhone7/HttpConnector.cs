using Appacitive.Sdk.Realtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Appacitive.Sdk.WindowsPhone7
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
            var request = HttpWebRequest.Create(url) as HttpWebRequest;

            //Dummy header
            request.Headers[HttpRequestHeader.IfModifiedSince] = DateTime.Now.ToString();

            // Write the headers
            if (headers != null)
            {
                foreach (var key in headers.Keys)
                    request.Headers[key] = headers[key];
            }
            request.Method = httpMethod;
            if (data != null)
            {
                using (var stream = await request.GetRequestStreamAsync())
                {
                    await stream.WriteAsync(data, 0, data.Length);
                }
            }

            var response = await request.GetResponseAsync();
            byte[] responseData = null;
            using (var responseStream = response.GetResponseStream())
            {
                using (var memStream = new MemoryStream())
                {
                    await responseStream.CopyToAsync(memStream);
                    responseData = memStream.ToArray();
                }
            }
            await LogTransaction(url, httpMethod, data, responseData, headers);
            return responseData;

        }

        private async Task LogTransaction(string url, string httpMethod, byte[] request, byte[] response, IDictionary<string, string> headers)
        {
            try
            {
                var buffer = new StringBuilder();
                buffer
                    .Append("Method: ").AppendLine(httpMethod)
                    .Append("Url: ").AppendLine(url);
                foreach (var key in headers.Keys)
                    buffer.Append(key).AppendLine(": ").AppendLine(headers[key]);
                buffer.Append("Request: ").AppendLine(request == null ? string.Empty : Encoding.UTF8.GetString(request, 0, request.Length));
                buffer.Append("Response: ").AppendLine(response == null ? string.Empty : Encoding.UTF8.GetString(response, 0, request.Length));
                await Debugger.Log(buffer.ToString());
            }
            catch { }
        }
    }
}
