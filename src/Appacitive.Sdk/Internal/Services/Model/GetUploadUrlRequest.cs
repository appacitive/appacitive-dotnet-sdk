using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Services
{
    public class GetUploadUrlRequest : GetOperation<GetUploadUrlResponse>
    {
        public GetUploadUrlRequest()
            : base()
        {
        }

        public string MimeType { get; set; }

        public string FileName { get; set; }

        public int ExpiryInMinutes { get; set; }

        protected override string GetUrl()
        {
            return Urls.For.GetUploadUrl(this.MimeType, this.FileName, this.ExpiryInMinutes);
        }
    }

    
}
