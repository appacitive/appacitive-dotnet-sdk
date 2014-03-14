using System;
using Appacitive.Sdk.Internal;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Appacitive.Sdk.iOS
{
	public class TraceWriter : ITraceWriter
	{
		public Task WritelineAsync (string trace)
		{
			Trace.WriteLine (trace);
			return Task.FromResult(true);
		}
	}
}

