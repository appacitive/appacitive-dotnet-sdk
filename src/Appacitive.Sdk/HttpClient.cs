using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Appacitive.Sdk
{
    public class HttpOperation
    {
        public HttpOperation(string url )
        {
            this.Url = url;
            this.ContentType = "application/json; charset=utf-8";
            this.Headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            this.Connector = ObjectFactory.Build<IHttpConnector>();
        }

        public string Url { get; set; }

        public string ContentType { get; set; }

        public IDictionary<string, string> Headers { get; private set; }

        public IHttpConnector Connector { get; set; }

        public HttpOperation WithHeader(string header, string value)
        {
            if( value != null )
                this.Headers[header] = value;
            return this;
        }

        public static HttpOperation WithUrl(string url)
        {
            return new HttpOperation(url);
        }

        public async Task<byte[]> GetAsync()
        {
            return await this.Connector.Get(this.Url, this.Headers);
        }

        public async Task<byte[]> PostAsyc(byte[] data)
        {
            return await this.Connector.Post(this.Url, this.Headers, data);
        }

        public async Task<byte[]> PutAsyc(byte[] data)
        {
            return await this.Connector.Put(this.Url, this.Headers, data);
        }

        public async Task<byte[]> DeleteAsync()
        {
            return await this.Connector.Delete(this.Url, this.Headers);
        }
    }
}
