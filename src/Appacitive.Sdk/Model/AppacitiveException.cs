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
    public abstract class AppacitiveApiException : BaseAppacitiveException
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

        [DataMember(Name = "code")]
        public abstract string Code { get; }

        [DataMember(Name = "referenceId")]
        public string ReferenceId { get; set; }

        [DataMember(Name = "additionalMessages")]
        public string[] AdditionalMessages { get; set; }
    }

    [DataContract]
    public class BadRequestException : AppacitiveApiException
    {
        public BadRequestException()
            : base()
        {
        }

        public BadRequestException(string message)
            : base(message)
        {
        }

        public BadRequestException(string message, Exception innerEx)
            : base(message, innerEx)
        {
        }

        public override string Code
        {
            get { return "400"; }
        }
    }

    [DataContract]
    public class AccessDeniedException : AppacitiveApiException
    {
        public AccessDeniedException()
            : base()
        {
        }

        public AccessDeniedException(string message)
            : base(message)
        {
        }

        public AccessDeniedException(string message, Exception innerEx)
            : base(message, innerEx)
        {
        }

        public override string Code
        {
            get { return "401"; }
        }
    }

    [DataContract]
    public class InvalidSubscriptionException : AppacitiveApiException
    {
        public InvalidSubscriptionException()
            : base()
        {
        }

        public InvalidSubscriptionException(string message)
            : base(message)
        {
        }

        public InvalidSubscriptionException(string message, Exception innerEx)
            : base(message, innerEx)
        {
        }

        public override string Code
        {
            get { return "402"; }
        }
    }

    [DataContract]
    public class UsageLimitExceededException : AppacitiveApiException
    {
        public UsageLimitExceededException()
            : base()
        {
        }

        public UsageLimitExceededException(string message)
            : base(message)
        {
        }

        public UsageLimitExceededException(string message, Exception innerEx)
            : base(message, innerEx)
        {
        }

        public override string Code
        {
            get { return "403"; }
        }
    }

    [DataContract]
    public class ObjectNotFoundException : AppacitiveApiException
    {
        public ObjectNotFoundException()
            : base()
        {
        }

        public ObjectNotFoundException(string message)
            : base(message)
        {
        }

        public ObjectNotFoundException(string message, Exception innerEx)
            : base(message, innerEx)
        {
        }

        public override string Code
        {
            get { return "404"; }
        }
    }

    [DataContract]
    public class DuplicateObjectException : AppacitiveApiException
    {
        public DuplicateObjectException()
            : base()
        {
        }

        public DuplicateObjectException(string message)
            : base(message)
        {
        }

        public DuplicateObjectException(string message, Exception innerEx)
            : base(message, innerEx)
        {
        }

        public override string Code
        {
            get { return "435"; }
        }
    }

    [DataContract]
    public class UpdateConflictException : AppacitiveApiException
    {
        public UpdateConflictException()
            : base()
        {
        }

        public UpdateConflictException(string message)
            : base(message)
        {
        }

        public UpdateConflictException(string message, Exception innerEx)
            : base(message, innerEx)
        {
        }

        public override string Code
        {
            get { return "409"; }
        }
    }

    [DataContract]
    public class PreconditionFailedException : AppacitiveApiException
    {
        public PreconditionFailedException()
            : base()
        {
        }

        public PreconditionFailedException(string message)
            : base(message)
        {
        }

        public PreconditionFailedException(string message, Exception innerEx)
            : base(message, innerEx)
        {
        }

        public override string Code
        {
            get { return "412"; }
        }
    }

    [DataContract]
    public class ApiAuthenticationFailureException : AppacitiveApiException
    {
        public ApiAuthenticationFailureException()
            : base()
        {
        }

        public ApiAuthenticationFailureException(string message)
            : base(message)
        {
        }

        public ApiAuthenticationFailureException(string message, Exception innerEx)
            : base(message, innerEx)
        {
        }

        public override string Code
        {
            get { return "420"; }
        }
    }

    [DataContract]
    public class UserAuthenticationFailureException : AppacitiveApiException
    {
        public UserAuthenticationFailureException()
            : base()
        {
        }

        public UserAuthenticationFailureException(string message)
            : base(message)
        {
        }

        public UserAuthenticationFailureException(string message, Exception innerEx)
            : base(message, innerEx)
        {
        }

        public override string Code
        {
            get { return "421"; }
        }
    }

    [DataContract]
    public class IncorrectConfigurationException : AppacitiveApiException
    {
        public IncorrectConfigurationException()
            : base()
        {
        }

        public IncorrectConfigurationException(string message)
            : base(message)
        {
        }

        public IncorrectConfigurationException(string message, Exception innerEx)
            : base(message, innerEx)
        {
        }

        public override string Code
        {
            get { return "436"; }
        }
    }

    [DataContract]
    public class InternalServerException : AppacitiveApiException
    {
        public InternalServerException()
            : base()
        {
        }

        public InternalServerException(string message)
            : base(message)
        {
        }

        public InternalServerException(string message, Exception innerEx)
            : base(message, innerEx)
        {
        }

        public override string Code
        {
            get { return "500"; }
        }
    }

    [DataContract]
    public class DataAccessException : AppacitiveApiException
    {
        public DataAccessException()
            : base()
        {
        }

        public DataAccessException(string message)
            : base(message)
        {
        }

        public DataAccessException(string message, Exception innerEx)
            : base(message, innerEx)
        {
        }

        public override string Code
        {
            get { return "512"; }
        }
    }

    [DataContract]
    public class UnExpectedSystemException : AppacitiveApiException
    {
        public UnExpectedSystemException()
            : base()
        {
            _code = "500";
        }

        public UnExpectedSystemException(string code, string message)
            : base(message)
        {
            _code = code ?? "500";
        }


        public UnExpectedSystemException(string message, Exception innerEx)
            : base(message, innerEx)
        {
            _code = "500";
        }

        private string _code;
        public override string Code
        {
            get { return _code; } 
        }
    }
}
