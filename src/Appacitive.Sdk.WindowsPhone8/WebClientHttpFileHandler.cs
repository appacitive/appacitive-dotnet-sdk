
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.IO.IsolatedStorage;
using Appacitive.Sdk.Internal;

#if WINDOWS_PHONE
namespace Appacitive.Sdk.WindowsPhone8
#elif WINDOWS_PHONE7
namespace Appacitive.Sdk.WindowsPhone7
#endif
{
    public class WebClientHttpFileHandler : IHttpFileHandler
    {
        public async Task<byte[]> DownloadAsync(string url, IDictionary<string, string> headers, string method)
        {
            var client = new WebClient();
            SubscribeToDownloadEvents(client);
            try
            {
                if (headers != null)
                {
                    foreach (var header in headers)
                        client.Headers[header.Key] = header.Value;
                }
                var downloadStream = await client.OpenReadTaskAsync(new Uri(url));
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
            finally
            {
                UnsubscribeFromDownloadEvents(client);
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
            SubscribeToUploadEvents(client);
            try
            {
                if (headers != null)
                {
                    foreach (var header in headers)
                        client.Headers[header.Key] = header.Value;
                }
                await client.UploadDataAsync(url, method, data);
            }
            finally
            {
                UnsubscribeFromUploadEvents(client);
            }
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


        private void UnsubscribeFromDownloadEvents(WebClient client)
        {
            client.DownloadProgressChanged -= client_DownloadProgressChanged;
        }

        private void SubscribeToDownloadEvents(WebClient client)
        {
            client.DownloadProgressChanged += client_DownloadProgressChanged;
        }

        private void UnsubscribeFromUploadEvents(WebClient client)
        {
            client.UploadProgressChanged -= client_UploadProgressChanged;
        }

        private void SubscribeToUploadEvents(WebClient client)
        {
            client.UploadProgressChanged += client_UploadProgressChanged;
        }

        void client_UploadProgressChanged(object sender, System.Net.UploadProgressChangedEventArgs e)
        {
            OnUploadProgressChanged(e);
        }

        private void client_DownloadProgressChanged(object sender, System.Net.DownloadProgressChangedEventArgs e)
        {
            OnDownloadProgressChanged(e);
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

        public event EventHandler<DownloadCompletedEventArgs> DownloadCompleted
        {
            add { throw new NotSupportedException("DownloadCompleted event is not supported on WP7."); }
            remove { throw new NotSupportedException("DownloadCompleted event is not supported on WP7."); }
        }

        public event EventHandler<UploadCompletedEventArgs> UploadCompleted
        {   
            add { throw new NotSupportedException("UploadCompleted event is not supported on WP7."); }
            remove { throw new NotSupportedException("UploadCompleted event is not supported on WP7."); }
        }

        public event EventHandler<DownloadProgressChangedEventArgs> DownloadProgressChanged;

        

        public event EventHandler<UploadProgressChangedEventArgs> UploadProgressChanged;
    }
}
