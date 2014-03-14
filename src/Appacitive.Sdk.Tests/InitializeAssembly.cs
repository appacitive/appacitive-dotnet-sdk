using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

#if MONO
using NUnit.Framework;
using Appacitive.Sdk.Tests;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif
using Appacitive.Sdk;
using System.Diagnostics;




namespace Appacitive.Sdk.Tests
{

	#if MONO

	public static class OneTimeSetup
	{
		private static int _isRunOnce = 0;

		public static void Run()
		{
			if( Interlocked.CompareExchange(ref _isRunOnce, 1, 0) != 0 )
				return;
			#if __IOS__
			App.Initialize(Platforms.IOs, "appid", TestConfiguration.ApiKey, TestConfiguration.Environment);
			#endif
		}
	}

	#else
	

	[TestClass]
	public class InitializeAssembly
	{

		[AssemblyInitialize]
		public static void Init(TestContext context)
		{
			App.Initialize(Platforms.NonWeb, "appid", TestConfiguration.ApiKey, TestConfiguration.Environment);
			App.Debug.ApiLogging.LogEverything();
		}


	}

	#endif
	
}

