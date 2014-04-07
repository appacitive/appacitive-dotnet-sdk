using Appacitive.Sdk.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk
{
    public class ApiOptions
    {
        internal static T Apply<T>(T request, ApiOptions options)
            where T : ApiRequest
        {
            if (options == null)
                return request;
            if (string.IsNullOrWhiteSpace(options.ApiKey) == false)
                request.ApiKey = options.ApiKey;
            if (string.IsNullOrWhiteSpace(options.UserToken) == false)
                request.UserToken = options.UserToken;
            return request;
        }

        public string ApiKey { get; set; }

        public string UserToken { get; set; }
    }
}
