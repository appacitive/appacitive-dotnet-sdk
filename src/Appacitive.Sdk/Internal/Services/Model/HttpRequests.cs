using Appacitive.Sdk;
using Appacitive.Sdk.Realtime;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
            T response = null;
            Exception fault = null;
            byte[] responseBytes = null;
            long elasedTimeInMs = 0;
            var op = HttpOperation
                    .WithUrl(url)
                    .WithAppacitiveKeyOrSession(request.ApiKey, request.SessionToken, request.UseApiSession)
                    .WithEnvironment(request.Environment)
                    .WithUserToken(request.UserToken);
            try
            {   
#if !WINDOWS_PHONE7
                Stopwatch timer = Stopwatch.StartNew();
#else
                var startTime = DateTime.Now;
#endif
                responseBytes = await op.GetAsync();
#if !WINDOWS_PHONE7
                elasedTimeInMs = timer.ElapsedMilliseconds;
                timer.Stop();
                timer = null;
                
#else
                elasedTimeInMs = Convert.ToInt64(DateTime.Now.Subtract(startTime).TotalMilliseconds);
#endif
                response = Parse<T>(responseBytes);
            }
            catch (Exception ex)
            {
                fault = ex;
            }
            await TraceAsync(response == null ? string.Empty : response.Status.ReferenceId, 
                "GET", url, op.Headers, null, responseBytes, elasedTimeInMs,
                response,
                fault);
            if (fault != null)
                throw fault;
            return response;
        }

        public static async Task<T> DeleteAsync<T>(ApiRequest request, string url)
            where T : ApiResponse
        {
            T response = null;
            Exception fault = null;
            byte[] responseBytes = null;
            long elasedTimeInMs = 0;
            var op = HttpOperation
                    .WithUrl(url)
                    .WithAppacitiveKeyOrSession(request.ApiKey, request.SessionToken, request.UseApiSession)
                    .WithEnvironment(request.Environment)
                    .WithUserToken(request.UserToken);
            try
            {
#if !WINDOWS_PHONE7
                Stopwatch timer = Stopwatch.StartNew();
#else
                var startTime = DateTime.Now;
#endif
                responseBytes = await op.DeleteAsync();
#if !WINDOWS_PHONE7
                elasedTimeInMs = timer.ElapsedMilliseconds;
                timer.Stop();
                timer = null;

#else
                elasedTimeInMs = Convert.ToInt64(DateTime.Now.Subtract(startTime).TotalMilliseconds);
#endif

                response = Parse<T>(responseBytes);
            }
            catch (Exception ex)
            {
                fault = ex;
            }
            await TraceAsync(response == null ? string.Empty : response.Status.ReferenceId,
                "DELETE", url, op.Headers, null, responseBytes, elasedTimeInMs,
                response,
                fault);
            if (fault != null)
                throw fault;
            return response;
        }

        public static async Task<T> PutAsync<T>(ApiRequest request, string url)
            where T : ApiResponse
        {
            T response = null;
            Exception fault = null;
            byte[] responseBytes = null, requestBytes = null;
            long elasedTimeInMs = 0;
            var op = HttpOperation
                    .WithUrl(url)
                    .WithAppacitiveKeyOrSession(request.ApiKey, request.SessionToken, request.UseApiSession)
                    .WithEnvironment(request.Environment)
                    .WithUserToken(request.UserToken);
            try
            {
#if !WINDOWS_PHONE7
                Stopwatch timer = Stopwatch.StartNew();
#else
                var startTime = DateTime.Now;
#endif
                requestBytes = request.ToBytes();
                responseBytes = await op.PutAsyc(requestBytes);
#if !WINDOWS_PHONE7
                elasedTimeInMs = timer.ElapsedMilliseconds;
                timer.Stop();
                timer = null;

#else
                elasedTimeInMs = Convert.ToInt64(DateTime.Now.Subtract(startTime).TotalMilliseconds);
#endif          
                response = Parse<T>(responseBytes);
            }
            catch (Exception ex)
            {
                fault = ex;
            }
            await TraceAsync(response == null ? string.Empty : response.Status.ReferenceId,
                "PUT", url, op.Headers, requestBytes, responseBytes, elasedTimeInMs,
                response,
                fault);
            if (fault != null)
                throw fault;
            return response;
        }

        public static async Task<T> PostAsync<T>(ApiRequest request, string url)
            where T : ApiResponse
        {
            T response = null;
            Exception fault = null;
            byte[] responseBytes = null, requestBytes = null;
            long elasedTimeInMs = 0;
            var op = HttpOperation
                    .WithUrl(url)
                    .WithAppacitiveKeyOrSession(request.ApiKey, request.SessionToken, request.UseApiSession)
                    .WithEnvironment(request.Environment)
                    .WithUserToken(request.UserToken);
            try
            {
#if !WINDOWS_PHONE7
                Stopwatch timer = Stopwatch.StartNew();
#else
                var startTime = DateTime.Now;
#endif
                requestBytes = request.ToBytes();
                responseBytes = await op.PostAsyc(requestBytes);
#if !WINDOWS_PHONE7
                elasedTimeInMs = timer.ElapsedMilliseconds;
                timer.Stop();
                timer = null;

#else
                elasedTimeInMs = Convert.ToInt64(DateTime.Now.Subtract(startTime).TotalMilliseconds);
#endif          
                response = Parse<T>(responseBytes);
            }
            catch (Exception ex)
            {
                fault = ex;
            }
            await TraceAsync(response == null ? string.Empty : response.Status.ReferenceId,
                "POST", url, op.Headers, requestBytes, responseBytes, elasedTimeInMs,
                response,
                fault);
            if (fault != null)
                throw fault;
            return response;
        }


        
        private static T Parse<T>(byte[] bytes)
        {
            var serializer = ObjectFactory.Build<IJsonSerializer>();
            return serializer.Deserialize<T>(bytes);
        }

        private static async Task TraceAsync(string trxId, string operation, string url, IDictionary<string, string> headers, byte[] request, byte[] response, long responseTime, ApiResponse responseObj, Exception fault = null)
        {
            try
            {
                var logThisRequest = App.Debug.ApiLogging.ShouldLog(responseObj, responseTime);
                if (logThisRequest)
                {
                    var log = new JObject();
                    log["referenceId"] = new JValue(trxId);
                    log["method"] = new JValue(operation);
                    log["url"] = new JValue(url);
                    log["responseTime"] = new JValue(responseTime);
                    var headerJson = new JObject();
                    foreach (var key in headers.Keys)
                        headerJson[key] = new JValue(headers[key]);
                    log["headers"] = headerJson;

                    log["request"] = request == null ?
                            null :
                            new JRaw(Encoding.UTF8.GetString(request, 0, request.Length));

                    log["response"] = response == null ?
                            null :
                            new JRaw(Encoding.UTF8.GetString(response, 0, response.Length));
                    if (fault != null)
                        log["fault"] = fault.ToString();

                    await App.Debug.LogAsync(log.ToString(Formatting.Indented));
                }
            }
            catch { /* Suppress fault */ }
        }

        private static JObject GetJObject(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0 )
                return null;
            using (var memStream = new MemoryStream(bytes, false))
            {
                using (var streamReader = new StreamReader(memStream, Encoding.UTF8))
                {
                    using (var jsonReader = new JsonTextReader(streamReader))
                    {
                        return JObject.ReadFrom(jsonReader) as JObject;
                    }
                }
            }
        }
    }
}
