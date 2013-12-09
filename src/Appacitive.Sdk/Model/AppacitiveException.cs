using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Appacitive.Sdk.Services;

namespace Appacitive.Sdk
{
    [DataContract]
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

        [DataMember(Name="code")]
        public string Code { get; set; }

        [DataMember(Name = "faultType")]
        public string FaultType { get; set; }

        [DataMember(Name = "referenceId")]
        public string ReferenceId { get; set; }

        [DataMember(Name = "additionalMessages")]
        public string[] AdditionalMessages { get; set; }
    }
}
