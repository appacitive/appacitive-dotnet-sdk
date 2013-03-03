using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Services
{
    public interface IFileService
    {
        Task<GetUploadUrlResponse> GetUploadUrlAsync(GetUploadUrlRequest request);

        Task<GetDownloadUrlResponse> GetDownloadUrlAsync(GetDownloadUrlRequest request);
    }
}
