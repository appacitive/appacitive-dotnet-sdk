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
    public class FileUpload
    {
        public FileUpload(string mimeType, string filename = null, int expiryInMinutes = 5)
        {
            this.MimeType = mimeType;
            this.FileName = filename;
            this.FileHandler = ObjectFactory.Build<IHttpFileHandler>();
            
        }

        public string MimeType { get; private set; }

        public string FileName { get; private set; }

        public IHttpFileHandler FileHandler { get; set; }

        public event EventHandler<UploadCompletedEventArgs> UploadCompleted
        {
            add { this.FileHandler.UploadCompleted += value; }
            remove { this.FileHandler.UploadCompleted -= value; }
        }

        public event EventHandler<UploadProgressChangedEventArgs> UploadProgressChanged
        {
            add { this.FileHandler.UploadProgressChanged += value; }
            remove { this.FileHandler.UploadProgressChanged -= value; }
        }

        public async Task<string> UploadAsync(byte[] data)
        {
            var result = await this.GetUploadUrlAsync();
            var headers = new Dictionary<string, string> { {"Content-Type", this.MimeType } };
            await this.FileHandler.UploadAsync(result.Url, headers, "PUT", data);
            return result.FileName;
        }

        public async Task<string> UploadFileAsync(string file)
        {
            var result = await this.GetUploadUrlAsync();
            var headers = new Dictionary<string, string> { { "Content-Type", this.MimeType } };
            await this.FileHandler.UploadAsync(result.Url, headers, "PUT", file);
            return result.FileName;
        }

        public async Task<FileUrl> GetUploadUrlAsync(int expiryInMinutes = 5)
        {
            var request = new GetUploadUrlRequest
            {
                MimeType = this.MimeType,
                FileName = this.FileName,
                ExpiryInMinutes = expiryInMinutes
            };
            var response = await request.ExecuteAsync();
            return new FileUrl(response.Filename, response.Url);
        }
    }
}
