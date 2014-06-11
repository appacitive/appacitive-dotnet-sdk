using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Services
{
    public class GetDownloadUrlRequest : GetOperation<GetDownloadUrlResponse>
    {
        public string FileName { get; set; }

        public int ExpiryInMinutes { get; set; }

        public long CacheControlMaxAge { get; set; }

        protected override string GetUrl()
        {
            return Urls.For.GetDownloadUrl(this.FileName, this.ExpiryInMinutes, this.CacheControlMaxAge);
        }
    }

    
}
