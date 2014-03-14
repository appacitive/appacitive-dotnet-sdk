using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
#if MONO
using NUnit.Framework;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

namespace Appacitive.Sdk.Tests
{
	#if MONO
	[TestFixture]
	#else
	[TestClass]
	#endif
    public class HttpFixture
    {
		#if MONO
		[TestFixtureSetUp]
		public void Setup()
		{
			OneTimeSetup.Run ();
		}
		#endif

        #if MONO
		[Test]
		[Timeout(int.MaxValue)]
		#else
		[TestMethod]
		#endif
		public async Task GetTest()
        {
			var content = await HttpOperation
				.WithUrl("http://www.google.co.in")
				.GetAsync();
			Assert.IsNotNull(content);
			Assert.IsTrue(content.Length > 0);
			Console.WriteLine(Encoding.UTF8.GetString(content));
        }
    }
}
