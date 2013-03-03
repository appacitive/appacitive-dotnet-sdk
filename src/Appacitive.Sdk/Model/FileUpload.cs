using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Appacitive.Sdk.Services;


namespace Appacitive.Sdk
{
    public class FileUpload
    {
        public FileUpload(string mimeType, string filename = null, int expiryInMinutes = 5)
        {
            this.MimeType = mimeType;
            this.FileName = filename;
        }

        public string MimeType { get; private set; }

        public string FileName { get; private set; }

        public async Task<string> UploadAsync(byte[] data)
        {
            var fileUrl = await GetUploadUrlAsync();
            var uri = new Uri(fileUrl.Url);
            var client = new WebClient();
            // Set content type
            client.Headers[HttpRequestHeader.ContentType] = this.MimeType;
            await client.UploadDataTaskAsync(uri, "PUT", data);
            return fileUrl.FileName;
        }

        public async Task<string> UploadFileAsync(string file)
        {
            byte[] data = null;
            // Read file contents
            using (var memStream = new MemoryStream())
            {
                using (var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read, 1024, true))
                {
                    await fileStream.CopyToAsync(memStream);
                }
                data = memStream.ToArray();
            }

            var fileUrl = await GetUploadUrlAsync();
            var uri = new Uri(fileUrl.Url);
            var client = new WebClient();
            // Set content type
            client.Headers[HttpRequestHeader.ContentType] = this.MimeType;
            await client.UploadDataTaskAsync(uri, "PUT", data);
            return fileUrl.FileName;
        }

        public async Task<FileUrl> GetUploadUrlAsync(int expiryInMinutes = 5)
        {
            var request = new GetUploadUrlRequest
            {
                MimeType = this.MimeType,
                FileName = this.FileName,
                ExpiryInMinutes = expiryInMinutes
            };
            IFileService fileService = ObjectFactory.Build<IFileService>();
            var response = await fileService.GetUploadUrlAsync(request);
            return new FileUrl(response.Filename, response.Url);
        }
    }
}
