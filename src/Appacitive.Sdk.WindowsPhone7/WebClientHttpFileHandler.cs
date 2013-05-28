using Appacitive.Sdk.Realtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.IO.IsolatedStorage;

namespace Appacitive.Sdk.WindowsPhone7
{
    public class WebClientHttpFileHandler : IHttpFileHandler
    {
        public async Task<byte[]> DownloadAsync(string url, IDictionary<string, string> headers, string method)
        {
            var client = new WebClient();
            if (headers != null)
            {
                foreach (var header in headers)
                    client.Headers[header.Key] = header.Value;
            }
            var downloadStream = await client.OpenReadTaskAsync(url);
            byte[] buffer = new byte[1024];
            using (MemoryStream stream = new MemoryStream())
            {
                while (downloadStream.Read(buffer, 0, buffer.Length) > 0)
                {
                    stream.Write(buffer, 0, buffer.Length);
                }
                await stream.FlushAsync();
                return stream.ToArray();
            }

        }

        public async Task DownloadAsync(string url, IDictionary<string, string> headers, string method, string saveAs)
        {
            // Download the data
            var data = await this.DownloadAsync(url, headers, method);

            // Get the local folder.
            var localStorage = IsolatedStorageFile.GetUserStoreForApplication();

            // Write the file
            using (var memStream = new MemoryStream(data))
            {
                using (var stream = new IsolatedStorageFileStream(saveAs, FileMode.CreateNew, FileAccess.Write, FileShare.None, localStorage))
                {
                    await memStream.CopyToAsync(stream);
                    await stream.FlushAsync();
                }
            }
        }

        public async Task UploadAsync(string url, IDictionary<string, string> headers, string method, byte[] data)
        {
            var client = new WebClient();
            if (headers != null)
            {
                foreach (var header in headers)
                    client.Headers[header.Key] = header.Value;
            }
            await client.UploadData(url, method, data);
        }

        public async Task UploadAsync(string url, IDictionary<string, string> headers, string method, string file)
        {
            byte[] data = null;

            // Get the local folder.
            var localStorage = IsolatedStorageFile.GetUserStoreForApplication();

            using (var fileStream = new IsolatedStorageFileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read, localStorage))
            {
                using (var memStream = new MemoryStream())
                {
                    await fileStream.CopyToAsync(memStream);
                    await fileStream.FlushAsync();
                    await memStream.FlushAsync();
                    data = memStream.ToArray();
                }
            }
            await UploadAsync(url, headers, method, data);
        }
    }
}
