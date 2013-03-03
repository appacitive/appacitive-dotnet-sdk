using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk
{
    public struct FileUrl
    {
        public FileUrl(string filename, string url) : this()
        {
            this.FileName = filename;
            this.Url = url;
        }

        public string FileName { get; private set; }

        public string Url { get; private set; }
    }
}
