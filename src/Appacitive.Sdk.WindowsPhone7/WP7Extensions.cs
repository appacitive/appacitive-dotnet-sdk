using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.WindowsPhone7
{
    internal static class WP7Extensions
    {
        public static Task UploadData(this WebClient client, string address, string method, byte[] data)
        {
            var taskComplete = new TaskCompletionSource<string>();
            client.OpenWriteCompleted += (a, e) =>
            {
                if (e.Error != null) taskComplete.TrySetException(e.Error);

                var uState = e.UserState as byte[];
                Stream outputStream = e.Result;
                outputStream.Write(uState, 0, uState.Length);
                outputStream.Flush();
                outputStream.Close();

                taskComplete.TrySetResult(e.Result.ToString());
            };
            client.OpenWriteAsync(new Uri(address), method, data);
            return taskComplete.Task;
        }
    }
}
