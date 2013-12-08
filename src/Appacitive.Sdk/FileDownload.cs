﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Appacitive.Sdk.Services;
using Appacitive.Sdk.Realtime;


namespace Appacitive.Sdk
{
    public class FileDownload
    {
        public FileDownload(string filename)
        {
            this.FileName = filename;
            this.FileHandler = ObjectFactory.Build<IHttpFileHandler>();
        }

        public string FileName { get; private set; }

        public event EventHandler<DownloadCompletedEventArgs> DownloadCompleted
        {
            add { this.FileHandler.DownloadCompleted += value; }
            remove { this.FileHandler.DownloadCompleted -= value; }
        }

        public event EventHandler<DownloadProgressChangedEventArgs> DownloadProgressChanged
        {
            add { this.FileHandler.DownloadProgressChanged += value; }
            remove { this.FileHandler.DownloadProgressChanged -= value; }
        }

        public IHttpFileHandler FileHandler { get; set; }

        public async Task<string> GetDownloadUrl(int expiryTimeInMinutes = 5)
        {
            var request = new GetDownloadUrlRequest
            {
                FileName = this.FileName,
                ExpiryInMinutes = expiryTimeInMinutes
            };
            var response = await request.ExecuteAsync();
            return response.Url;
        }

        public async Task<byte[]> DownloadAsync()
        {
            try
            {
                var url = await this.GetDownloadUrl();
                return await this.FileHandler.DownloadAsync(url, null, "GET");
            }
            catch (WebException wex)
            {
                var response = wex.Response as HttpWebResponse;
                if (response != null && response.StatusCode == HttpStatusCode.NotFound)
                    throw new AppacitiveException("File not found.", wex);
                else throw;
            }
        }

        public async Task DownloadFileAsync(string file)
        {
            var url = await this.GetDownloadUrl();
            await this.FileHandler.DownloadAsync(url, null, "GET", file);
        }
    }
}
