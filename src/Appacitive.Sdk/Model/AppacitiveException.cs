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
    public abstract class BaseAppacitiveException : Exception
    {
        protected BaseAppacitiveException()
            : base()
        {
        }

        protected BaseAppacitiveException(string message)
            : base(message)
        {
        }

        protected BaseAppacitiveException(string message, Exception innerEx)
            : base(message, innerEx)
        {
        }
    }

    [DataContract]
    public class AppacitiveRuntimeException : BaseAppacitiveException
    {
        public AppacitiveRuntimeException()
            : base()
        {
        }

        public AppacitiveRuntimeException(string message)
            : base(message)
        {
        }

        public AppacitiveRuntimeException(string message, Exception innerEx)
            : base(message, innerEx)
        {
        }
    }

    [DataContract]
    public class AppacitiveApiException : BaseAppacitiveException
    {
        public AppacitiveApiException()
            : base()
        {
        }
        
        public AppacitiveApiException(string message)
            : base(message)
        {
        }

        public AppacitiveApiException(string message, Exception innerEx)
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
