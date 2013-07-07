using Appacitive.Sdk;
using Appacitive.Sdk.Realtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Services
{
    public abstract class GetOperation<TRs> : ApiRequest
        where TRs : ApiResponse
    {
        public GetOperation(string apiKey, string sessionToken, Environment environment, string userToken = null, Geocode location = null, bool enableDebugging = false, Verbosity verbosity = Verbosity.Info) :
            base(apiKey, sessionToken, environment, userToken, location, enableDebugging, verbosity)
        {
        }

        protected abstract string GetUrl();

        public virtual async Task<TRs> ExecuteAsync()
        {
            return await ApiHttpClient.GetAsync<TRs>(this, GetUrl());
        }
    }

    public abstract class PostOperation<TRs> : ApiRequest
        where TRs : ApiResponse
    {
        public PostOperation(string apiKey, string sessionToken, Environment environment, string userToken = null, Geocode location = null, bool enableDebugging = false, Verbosity verbosity = Verbosity.Info) :
            base(apiKey, sessionToken, environment, userToken, location, enableDebugging, verbosity)
        {
        }

        protected abstract string GetUrl();

        public virtual async Task<TRs> ExecuteAsync()
        {
            return await ApiHttpClient.PostAsync<TRs>(this, GetUrl());
        }
    }

    public abstract class PutOperation<TRs> : ApiRequest
        where TRs : ApiResponse
    {
        
        public PutOperation(string apiKey, string sessionToken, Environment environment, string userToken = null, Geocode location = null, bool enableDebugging = false, Verbosity verbosity = Verbosity.Info) :
            base(apiKey, sessionToken, environment, userToken, location, enableDebugging, verbosity)
        {   
        }

        protected abstract string GetUrl();

        public virtual async Task<TRs> ExecuteAsync()
        {
            return await ApiHttpClient.PutAsync<TRs>(this, GetUrl());
        }
    }

    public abstract class DeleteOperation<TRs> : ApiRequest
        where TRs : ApiResponse
    {
        public DeleteOperation(string apiKey, string sessionToken, Environment environment, string userToken = null, Geocode location = null, bool enableDebugging = false, Verbosity verbosity = Verbosity.Info) :
            base(apiKey, sessionToken, environment, userToken, location, enableDebugging, verbosity)
        {
        }

        protected abstract string GetUrl();

        public virtual async Task<TRs> ExecuteAsync()
        {
            return await ApiHttpClient.DeleteAsync<TRs>(this, GetUrl());
        }
    }


    internal class ApiHttpClient
    {
        public static async Task<T> GetAsync<T>(ApiRequest request, string url)
            where T : ApiResponse
        {
            var bytes = await HttpOperation
                .WithUrl(url)
                .WithAppacitiveKeyOrSession(request.ApiKey, request.SessionToken, request.UseApiSession)
                .WithEnvironment(request.Environment)
                .WithUserToken(request.UserToken)
                .GetAsync();
            return Parse<T>(bytes);
        }

        public static async Task<T> DeleteAsync<T>(ApiRequest request, string url)
            where T : ApiResponse
        {
            var bytes = await HttpOperation
                .WithUrl(url)
                .WithAppacitiveKeyOrSession(request.ApiKey, request.SessionToken, request.UseApiSession)
                .WithEnvironment(request.Environment)
                .WithUserToken(request.UserToken)
                .DeleteAsync();
            return Parse<T>(bytes);
        }

        public static async Task<T> PutAsync<T>(ApiRequest request, string url)
            where T : ApiResponse
        {
            var bytes = await HttpOperation
                .WithUrl(url)
                .WithAppacitiveKeyOrSession(request.ApiKey, request.SessionToken, request.UseApiSession)
                .WithEnvironment(request.Environment)
                .WithUserToken(request.UserToken)
                .PutAsyc(request.ToBytes());
            return Parse<T>(bytes);
        }

        public static async Task<T> PostAsync<T>(ApiRequest request, string url)
            where T : ApiResponse
        {
            var bytes = await HttpOperation
                .WithUrl(url)
                .WithAppacitiveKeyOrSession(request.ApiKey, request.SessionToken, request.UseApiSession)
                .WithEnvironment(request.Environment)
                .WithUserToken(request.UserToken)
                .PostAsyc(request.ToBytes());
            return Parse<T>(bytes);
        }

        private static T Parse<T>(byte[] bytes)
        {
            var serializer = ObjectFactory.Build<IJsonSerializer>();
            return serializer.Deserialize<T>(bytes);
        }
    }
}
