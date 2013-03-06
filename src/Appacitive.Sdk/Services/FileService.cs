using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Services
{
    public class FileService : IFileService
    {
        public static readonly IFileService Instance = new FileService();

        public async Task<GetUploadUrlResponse> GetUploadUrlAsync(GetUploadUrlRequest request)
        {
            var bytes = await HttpOperation
                .WithUrl(Urls.For.GetUploadUrl(request.MimeType, request.FileName, request.ExpiryInMinutes))
                .WithAppacitiveSession(request.SessionToken)
                .WithEnvironment(request.Environment)
                .WithUserToken(request.UserToken)
                .GetAsync();
            var response = GetUploadUrlResponse.Parse(bytes);
            return response;
        }

        public async Task<GetDownloadUrlResponse> GetDownloadUrlAsync(GetDownloadUrlRequest request)
        {
            var bytes = await HttpOperation
                .WithUrl(Urls.For.GetDownloadUrl(request.FileName, request.ExpiryInMinutes))
                .WithAppacitiveSession(request.SessionToken)
                .WithEnvironment(request.Environment)
                .WithUserToken(request.UserToken)
                .GetAsync();
            var response = GetDownloadUrlResponse.Parse(bytes);
            return response;
        }
    }
}
