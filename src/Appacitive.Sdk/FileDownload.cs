using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Appacitive.Sdk.Services;
using Appacitive.Sdk.Internal;


namespace Appacitive.Sdk
{
    /// <summary>
    /// Helper class for managing file downloads from the Appacitive file storage.
    /// </summary>
    public class FileDownload
    {
        /// <summary>
        /// Creates a new instance of the file download helper class with the given file name.
        /// </summary>
        /// <param name="filename">The name of the file to be downloaded. This name must match the name on the appacitive file store.</param>
        /// <param name="options">Request specific api options. These will override the global settings for the app for this request.</param>
        public FileDownload(string filename, ApiOptions options = null)
        {
            this.FileName = filename;
            this.FileHandler = ObjectFactory.Build<IHttpFileHandler>();
        }

        /// <summary>
        /// The name of the file to be downloaded.
        /// </summary>
        public string FileName { get; private set; }
        private ApiOptions Options { get; set; }

        /// <summary>
        /// Occurs then the download for the specified file has been completed. 
        /// </summary>
        public event EventHandler<DownloadCompletedEventArgs> DownloadCompleted
        {
            
            add { this.FileHandler.DownloadCompleted += value; }
            remove { this.FileHandler.DownloadCompleted -= value; }
        }

        /// <summary>
        /// Occurs as the progress of the file download changes.
        /// </summary>
        public event EventHandler<DownloadProgressChangedEventArgs> DownloadProgressChanged
        {
            add { this.FileHandler.DownloadProgressChanged += value; }
            remove { this.FileHandler.DownloadProgressChanged -= value; }
        }

        /// <summary>
        /// Gets and sets the file download handler instance.
        /// </summary>
        public IHttpFileHandler FileHandler { get; set; }


        /// <summary>
        /// Returns the public download url for the file associated with this download instance.
        /// Generating a public url for a file will flag the file as a public file.
        /// </summary>
        /// <param name="cacheControlMaxAgeInSeconds">The value to be set as the cache-control max age for the file.</param>
        /// <returns>Public (non-expiring) download url for the file.</returns>
        public async Task<string> GetPublicUrlAsync(long cacheControlMaxAgeInSeconds = 2592000 )
        {
            return await this.GetDownloadUrlAsync(-1, cacheControlMaxAgeInSeconds);
        }

        /// <summary>
        /// Returns a limited time validity download url for the file associated with this FileDownload object.
        /// </summary>
        /// <param name="expiryTimeInMinutes">The validity interval for the download url (in minutes)."</param>
        /// <param name="cacheControlMaxAgeInSeconds">The value to be returned in the cache-control header for the file.</param>
        /// <returns>Download url.</returns>
        public async Task<string> GetDownloadUrlAsync(int expiryTimeInMinutes = 5, long cacheControlMaxAgeInSeconds = 2592000)
        {
            var request = new GetDownloadUrlRequest
            {
                FileName = this.FileName,
                ExpiryInMinutes = expiryTimeInMinutes,
                CacheControlMaxAge = cacheControlMaxAgeInSeconds
            };
            ApiOptions.Apply(request, this.Options);
            var response = await request.ExecuteAsync();
            return response.Url;
        }

        /// <summary>
        /// Downloads the file associated with this FileDownload instance.
        /// </summary>
        /// <returns>The specified file contents as a byte array.</returns>
        public async Task<byte[]> DownloadAsync()
        {
            try
            {
                var url = await this.GetDownloadUrlAsync();
                return await this.FileHandler.DownloadAsync(url, null, "GET");
            }
            catch (WebException wex)
            {
                var response = wex.Response as HttpWebResponse;
                if (response != null && response.StatusCode == HttpStatusCode.NotFound)
                    throw new AppacitiveRuntimeException("File not found.", wex);
                else throw;
            }
        }

        /// <summary>
        /// Downloads the file associated with this FileDownload instance and saves the file to disk.
        /// </summary>
        /// <param name="saveAs">File path to save the downloaded file to.</param>
        public async Task DownloadFileAsync(string saveAs)
        {
            var url = await this.GetDownloadUrlAsync();
            await this.FileHandler.DownloadAsync(url, null, "GET", saveAs);
        }
    }
}
