using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

#if WINDOWS_PHONE
namespace Appacitive.Sdk.WindowsPhone8
#elif WINDOWS_PHONE7
namespace Appacitive.Sdk.WindowsPhone7
#endif
{
    internal static class TaskExtensions
    {
        public static Task<Stream> GetRequestStreamAsync(this WebRequest request)
        {
            var taskSource = new TaskCompletionSource<Stream>();
            request.BeginGetRequestStream(ar =>
            {
                try
                {
                    var webRequest = ar.AsyncState as WebRequest;
                    var stream = webRequest.EndGetRequestStream(ar);
                    taskSource.TrySetResult(stream);
                }
                catch (Exception ex)
                {
                    taskSource.TrySetException(ex);
                }
            }, request);
            return taskSource.Task;
        }

        public static Task<WebResponse> GetResponseAsync(this WebRequest request)
        {
            var taskSource = new TaskCompletionSource<WebResponse>();
            request.BeginGetResponse(ar =>
            {
                try
                {
                    var webRequest = ar.AsyncState as WebRequest;
                    var response = webRequest.EndGetResponse(ar);
                    taskSource.TrySetResult(response);
                }
                catch (WebException wex)
                {
                    taskSource.TrySetResult(wex.Response);
                }
                catch (Exception ex)
                {
                    taskSource.TrySetException(ex);
                }
            }, request);
            return taskSource.Task;
        }


        public static Task<Stream> OpenReadTaskAsync(this WebClient client, Uri uri)
        {
            var completionSource = new TaskCompletionSource<Stream>();
            OpenReadCompletedEventHandler handler = null;
            handler = (s, e) =>
             {
                 if (e.Error != null)
                     completionSource.TrySetException(e.Error);
                 else
                     completionSource.TrySetResult(e.Result);
                 
                 // Unsubscribe the handler
                 var webClient = e.UserState as WebClient;
                 if( handler != null )
                    webClient.OpenReadCompleted -= handler;
             };
            // Subscribe the handler.
            client.OpenReadCompleted += handler;
            client.OpenReadAsync(uri, client);
            return completionSource.Task;
        }

        const int BLOCK_SIZE = 4096;
        public static Task UploadDataAsync(this WebClient client, string address, string method, byte[] data)
        {
            var source = new TaskCompletionSource<bool>();
            client.AllowWriteStreamBuffering = true;
            OpenWriteCompletedEventHandler openWriteCompletedHandler = (s, args) =>
            {
                using (var stream = new MemoryStream(data))
                {
                    using (BinaryReader br = new BinaryReader(stream))
                    {
                        using (BinaryWriter bw = new BinaryWriter(args.Result))
                        {
                            long bCount = 0;
                            long fileSize = stream.Length;
                            byte[] bytes = new byte[BLOCK_SIZE];
                            do
                            {
                                bytes = br.ReadBytes(BLOCK_SIZE);
                                bCount += bytes.Length;
                                bw.Write(bytes);
                            } while (bCount < fileSize);
                        }
                    }
                }
            };

            WriteStreamClosedEventHandler streamClosedHandler = null;
            streamClosedHandler = (s, args) =>
            {
                if (args.Error != null)
                    source.TrySetException(args.Error);
                else
                    source.TrySetResult(true);

                // Unsubscribe the handlers
                client.OpenWriteCompleted -= openWriteCompletedHandler;
                client.WriteStreamClosed -= streamClosedHandler;
            };

            client.OpenWriteCompleted += openWriteCompletedHandler;
            client.WriteStreamClosed += streamClosedHandler;
            client.OpenWriteAsync(new Uri(address), method);
            return source.Task;
        }
    }
}
