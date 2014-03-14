using Appacitive.Sdk;
using Appacitive.Sdk.Internal;
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
        protected GetOperation() : base() { }

        protected abstract string GetUrl();

        public virtual async Task<TRs> ExecuteAsync()
        {
            return await ApiHttpClient.GetAsync<TRs>(this, GetUrl());
        }
    }

    public abstract class PostOperation<TRs> : ApiRequest
        where TRs : ApiResponse
    {
        public PostOperation() : base() { }

        protected abstract string GetUrl();

        public virtual async Task<TRs> ExecuteAsync()
        {
            return await ApiHttpClient.PostAsync<TRs>(this, GetUrl());
        }
    }

    public abstract class PutOperation<TRs> : ApiRequest
        where TRs : ApiResponse
    {

        public PutOperation() : base() { }

        protected abstract string GetUrl();

        public virtual async Task<TRs> ExecuteAsync()
        {
            return await ApiHttpClient.PutAsync<TRs>(this, GetUrl());
        }
    }

    public abstract class DeleteOperation<TRs> : ApiRequest
        where TRs : ApiResponse
    {
        public DeleteOperation() : base() { }

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
                    .WithApiKey(request.ApiKey)
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
                request, response,
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
                    .WithApiKey(request.ApiKey)
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
                request, response,
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
                    .WithApiKey(request.ApiKey)
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
                request, response,
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
                    .WithApiKey(request.ApiKey)
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
                request, response,
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

        private static async Task TraceAsync(string trxId, string operation, string url, IDictionary<string, string> headers, byte[] request, byte[] response, long responseTime, ApiRequest requestObj, ApiResponse responseObj, Exception fault = null)
        {
			try
            {
				var logThisRequest = App.Debug.ApiLogging.ShouldLog(requestObj, responseObj, responseTime);
				if( logThisRequest == false ) return;
				var buffer = new StringBuilder();
				using( var txtWriter = new StringWriter(buffer))
				{
					using( var jWriter = new JsonTextWriter(txtWriter))
					{
						jWriter.Formatting = Formatting.Indented;
						jWriter.WriteStartObject();
						// Write the properties
						jWriter.WriteProperty("referenceId", trxId, true);
						jWriter.WriteProperty("method", operation, true);
						jWriter.WriteProperty("url", url, true);
						jWriter.WriteProperty("responseTime");
						jWriter.WriteValue(responseTime);
						// Write headers
						jWriter.WritePropertyName("headers");
						jWriter.WriteStartObject();
						headers.For( h => jWriter.WriteProperty(h.Key, h.Value, true));
						jWriter.WriteEndObject();

						// Write the request and response
						jWriter.WriteProperty("request");
						if( request != null )
							jWriter.WriteRaw(Encoding.UTF8.GetString(request, 0, request.Length));
						else 
							jWriter.WriteNull();
						jWriter.WriteProperty("response");
						if( response != null )
							jWriter.WriteRaw(Encoding.UTF8.GetString(request, 0, response.Length));
						else 
							jWriter.WriteNull();
						jWriter.WriteEndObject();
					}
				}
				// Log the request.
				await App.Debug.LogAsync(buffer.ToString());
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
