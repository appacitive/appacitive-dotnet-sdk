using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk
{
    [Serializable]
    public class AppacitiveException : Exception
    {
        public AppacitiveException()
            : base()
        {
        }
        
        public AppacitiveException(string message)
            : base(message)
        {
        }

        public AppacitiveException(string message, Exception innerEx)
            : base(message, innerEx)
        {
        }

        public AppacitiveException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
