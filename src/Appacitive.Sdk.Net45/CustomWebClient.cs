using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk
{
    /// <summary>
    /// Overriding the existing webclient to ensure keep alive is false.
    /// This is because it causes connections to be closed from the remote endpoint during uploads / downloads.
    /// </summary>
    internal class CustomWebClient : WebClient
    {
        protected override WebRequest GetWebRequest(Uri address)
        {
            var request = base.GetWebRequest(address);
            var httpRequest = request as HttpWebRequest;
            if (httpRequest != null)
            {
                httpRequest.KeepAlive = false;
                httpRequest.ProtocolVersion = HttpVersion.Version10;
            }
            return request;
        }
    }
}
