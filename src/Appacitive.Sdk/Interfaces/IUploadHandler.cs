﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Internal
{
    public interface IHttpFileHandler
    {
        Task<byte[]> DownloadAsync(string url, IDictionary<string, string> headers, string method);

        Task DownloadAsync(string url, IDictionary<string, string> headers, string method, string saveAs);

        Task UploadAsync(string url, IDictionary<string, string> headers, string method, byte[] data);

        Task UploadAsync(string url, IDictionary<string, string> headers, string method, string file);

        event EventHandler<DownloadCompletedEventArgs> DownloadCompleted;

        event EventHandler<DownloadProgressChangedEventArgs> DownloadProgressChanged;

        event EventHandler<UploadCompletedEventArgs> UploadCompleted;

        event EventHandler<UploadProgressChangedEventArgs> UploadProgressChanged;
        

    }
}
