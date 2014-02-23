using Appacitive.Sdk.Internal;
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
                SubscribeToDownloadEvents(client);
                if (headers != null)
                {
                    foreach (var header in headers)
                        client.Headers[header.Key] = header.Value;
                }
                
                var result = await client.DownloadDataTaskAsync(url);
                UnsubscribeFromDownloadEvents(client);
                return result;
                
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
                SubscribeToUploadEvents(client);
                if (headers != null)
                {
                    foreach (var header in headers)
                        client.Headers[header.Key] = header.Value;
                }
                await client.UploadDataTaskAsync(url, "PUT", data);
                UnsubscribeFromUploadEvents(client);
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


        private void UnsubscribeFromDownloadEvents(WebClient client)
        {
            client.DownloadDataCompleted -= client_DownloadDataCompleted;
            client.DownloadProgressChanged -= client_DownloadProgressChanged;
        }

        private void SubscribeToDownloadEvents(WebClient client)
        {
            client.DownloadDataCompleted += client_DownloadDataCompleted;
            client.DownloadProgressChanged += client_DownloadProgressChanged;
        }

        private void UnsubscribeFromUploadEvents(WebClient client)
        {
            client.UploadDataCompleted -= client_UploadDataCompleted;
            client.UploadProgressChanged -= client_UploadProgressChanged;
        }

        private void SubscribeToUploadEvents(WebClient client)
        {
            client.UploadDataCompleted += client_UploadDataCompleted;
            client.UploadProgressChanged += client_UploadProgressChanged;
        }

        void client_UploadProgressChanged(object sender, System.Net.UploadProgressChangedEventArgs e)
        {
            OnUploadProgressChanged(e);
        }

        void client_UploadDataCompleted(object sender, UploadDataCompletedEventArgs e)
        {
            OnUploadCompleted(e);
        }

        private void client_DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e)
        {
            OnDownloadCompleted(e);
        }

        private void client_DownloadProgressChanged(object sender, System.Net.DownloadProgressChangedEventArgs e)
        {
            OnDownloadProgressChanged(e);
        }

        private void OnDownloadCompleted(DownloadDataCompletedEventArgs e)
        {
            var copy = DownloadCompleted;
            if (copy != null)
                copy(this, new DownloadCompletedEventArgs(e.Result, e.Error, e.Cancelled, e.UserState));
        }

        private void OnUploadCompleted(UploadDataCompletedEventArgs e)
        {
            var copy = UploadCompleted;
            if (copy != null)
                copy(this, new UploadCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
        }

        private void OnDownloadProgressChanged(System.Net.DownloadProgressChangedEventArgs e)
        {
            var copy = DownloadProgressChanged;
            if (copy != null)
                copy(this, new DownloadProgressChangedEventArgs(e.BytesReceived, e.TotalBytesToReceive, e.ProgressPercentage, e.UserState));
        }

        private void OnUploadProgressChanged(System.Net.UploadProgressChangedEventArgs e)
        {
            var copy = UploadProgressChanged;
            if (copy != null)
                copy(this, new UploadProgressChangedEventArgs(e.BytesReceived, e.TotalBytesToReceive, e.BytesSent, e.TotalBytesToSend,
                    e.ProgressPercentage, e.UserState));
        }

        public event EventHandler<DownloadCompletedEventArgs> DownloadCompleted;

        public event EventHandler<DownloadProgressChangedEventArgs> DownloadProgressChanged;

        public event EventHandler<UploadCompletedEventArgs> UploadCompleted;

        public event EventHandler<UploadProgressChangedEventArgs> UploadProgressChanged;
    }
}
