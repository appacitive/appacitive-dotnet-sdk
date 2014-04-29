using Appacitive.Sdk.Internal;
using Appacitive.Sdk.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk
{
    public class Debugger
    {
        public Debugger()
        {
            this.ApiLogging = new ApiLogging();
            this.Verbosity = Sdk.Verbosity.Info;
        }

        public ApiLogging ApiLogging { get; private set; }

        internal bool IsProfilingEnabled { get; private set; }

        internal Verbosity Verbosity { get; set; }

        public void EnableProfiling(Verbosity verbosity = Sdk.Verbosity.Info)
        {
            this.IsProfilingEnabled = true;
            this.Verbosity = verbosity;
        }

        public void DisableProfiling()
        {
            this.IsProfilingEnabled = false;
        }

        public async Task LogAsync(string data)
        {
            try
            {
                var tracer = ObjectFactory.Build<ITraceWriter>();
                if (tracer == null)
                    return;
                await tracer.WritelineAsync(data);
            }
            catch { }
        }

        private async Task WriteDelimiter(TextWriter writer)
        {
            await writer.WriteLineAsync();
            await writer.WriteLineAsync();
        }
    }

    public class ApiLogging
    {
        internal ApiLogging()
        {
            this.ApiLogFlags = ApiLogFlags.None;
        }

        internal ApiLogFlags ApiLogFlags { get; private set; }
        private int _slowLogThresholdInMilliSeconds = 500;
        private Func<ApiRequest, ApiResponse, bool> _condition = null;

        internal bool IsSlowLog(long timeTaken)
        {
            return timeTaken > _slowLogThresholdInMilliSeconds;
        }

        public void DisableLogging()
        {
            this.ApiLogFlags = ApiLogFlags.None;
        }

        public ApiLogging LogEverything()
        {
            SetDebugFlag(ApiLogFlags.Everything, true);
            return this;
        }

        public ApiLogging LogIf(Func<ApiRequest, ApiResponse, bool> condition)
        {
            this._condition = condition;
            SetDebugFlag(ApiLogFlags.Conditional, true);
            return this;
        }

        public ApiLogging LogFailures()
        {
            SetDebugFlag(ApiLogFlags.FailedCalls);
            return this;
        }

        public ApiLogging LogSlowCalls(int logCallsSlowerThanInMs)
        {
            SetDebugFlag(ApiLogFlags.SlowLogs);
            _slowLogThresholdInMilliSeconds = logCallsSlowerThanInMs;
            return this;
        }

        public bool MatchLogLevel(ApiLogFlags level)
        {
            return (this.ApiLogFlags & level) == level;
        }

        private void SetDebugFlag( ApiLogFlags flag, bool replaceFlagValue = false )
        {
            if( this.ApiLogFlags == ApiLogFlags.None || replaceFlagValue == true || this.ApiLogFlags == ApiLogFlags.Everything )
                this.ApiLogFlags = flag;
            else 
                this.ApiLogFlags = this.ApiLogFlags | flag;
        }


        internal bool ShouldLog(ApiRequest request, ApiResponse response, long responseTimeInMs)
        {
            if (InternalApp.Debug.ApiLogging.MatchLogLevel(ApiLogFlags.None) == true)
                return false;
            else if (InternalApp.Debug.ApiLogging.MatchLogLevel(ApiLogFlags.Everything) == true)
                return true;
            else if (InternalApp.Debug.ApiLogging.MatchLogLevel(ApiLogFlags.Conditional) == true)
            {
                if (_condition != null && _condition(request, response) == true)
                    return true;
            }
            else
            {
                if (InternalApp.Debug.ApiLogging.MatchLogLevel(ApiLogFlags.FailedCalls))
                {
                    if (response == null) return true;
                    if (response.Status == null) return true;
                    if (response.Status.IsSuccessful == false) return true;
                }
                if (InternalApp.Debug.ApiLogging.MatchLogLevel(ApiLogFlags.SlowLogs) && responseTimeInMs > _slowLogThresholdInMilliSeconds)
                    return true;
            }
            return false;
        }

    }
}
