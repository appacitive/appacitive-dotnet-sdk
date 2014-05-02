using Appacitive.Sdk.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

#if WINDOWS_PHONE
namespace Appacitive.Sdk.WindowsPhone8
#elif WINDOWS_PHONE7
namespace Appacitive.Sdk.WindowsPhone7
#endif
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
            if( IsNetworkAvailable() == false)
                throw new AppacitiveRuntimeException("Network is not available.");

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
            return responseData;

        }

        private bool IsNetworkAvailable()
        {
            var devicePlatform = AppContext.State.Platform as IDevicePlatform;
            if (devicePlatform == null)
                return true;
            else return devicePlatform.DeviceState.IsNetworkAvailable();
        }
    }
}
