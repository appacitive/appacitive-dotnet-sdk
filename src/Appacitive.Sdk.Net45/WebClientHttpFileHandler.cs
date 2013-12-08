using Appacitive.Sdk.Realtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Net45
{
    public class WebClientHttpFileHandler : IHttpFileHandler
    {
        public async Task<byte[]> DownloadAsync(string url, IDictionary<string, string> headers, string method)
        {
            using (var client = new WebClient())
            {
                if (headers != null)
                {
                    foreach (var header in headers)
                        client.Headers[header.Key] = header.Value;
                }
                return await client.DownloadDataTaskAsync(url);
            }
        }

        public async Task DownloadAsync(string url, IDictionary<string, string> headers, string method, string saveAs)
        {
            // Download the data
            var data = await this.DownloadAsync(url, headers, method);
            // Write the file
            using (var memStream = new MemoryStream(data))
            {
                using (var stream = new FileStream(saveAs, FileMode.CreateNew, FileAccess.Write, FileShare.None, (int)1024, true))
                {
                    await memStream.CopyToAsync(stream);
                    await stream.FlushAsync();
                }
            }
        }

        public async Task UploadAsync(string url, IDictionary<string, string> headers, string method, byte[] data)
        {
            using (var client = new WebClient())
            {   
                if (headers != null)
                {
                    foreach (var header in headers)
                        client.Headers[header.Key] = header.Value;
                }
                await client.UploadDataTaskAsync(url, "PUT", data);
            }
        }




        public async Task UploadAsync(string url, IDictionary<string, string> headers, string method, string file)
        {
            byte[] data = null;
            using (var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read, 1024, true))
            {
                using (var memStream = new MemoryStream())
                {
                    await fileStream.CopyToAsync(memStream);
                    data = memStream.ToArray();
                }
            }
            await UploadAsync(url, headers, method, data);
        }
    }
}
