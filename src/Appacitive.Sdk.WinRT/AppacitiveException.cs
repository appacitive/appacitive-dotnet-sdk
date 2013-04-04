using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Appacitive.Sdk.Services;

namespace Appacitive.Sdk.WinRT
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

        public string Code { get; set; }

        public string FaultType { get; set; }

        public string ReferenceId { get; set; }

        public string[] AdditionalMessages { get; set; }
    }
}
