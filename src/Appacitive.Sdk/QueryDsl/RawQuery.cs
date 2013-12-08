using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk
{
    internal class RawQuery : IQuery
    {
        public RawQuery(string query)
        {
            this.Query = query;
        }

        public string Query { get; private set; }

        public string AsString()
        {
            return this.Query;
        }
    }
}
