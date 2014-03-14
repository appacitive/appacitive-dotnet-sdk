using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    public class EntityFixture
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
        public async Task EntityCreateAndUpdatedDateTimeTest()
        {
            var obj = new APObject("object");
            obj.Set<string>("stringfield", Unique.String);
            await obj.SaveAsync();

            var createDuration = obj.CreatedAt.Subtract(DateTime.Now).Duration();
            Assert.IsTrue(createDuration.Minutes < 2);
            var updateDuration = obj.LastUpdatedAt.Subtract(DateTime.Now).Duration();
            Assert.IsTrue(updateDuration.Minutes < 2);
        }

        #if MONO
		[Test]
		[Timeout(int.MaxValue)]
		#else
		[TestMethod]
		#endif
        public async Task EntityDateTimePropertyTest()
        {
            var dateTime = DateTime.Now;
            var obj1 = new APObject("object");
            obj1.Set<DateTime>("datetimefield", dateTime);
            await obj1.SaveAsync();

            var obj2 = new APObject("object");
            obj2.Set<DateTime>("datetimefield", dateTime.ToUniversalTime());
            await obj2.SaveAsync();

            var obj1Copy = await APObjects.GetAsync("object", obj1.Id);
            var obj2Copy = await APObjects.GetAsync("object", obj2.Id);

            Assert.IsTrue(obj1Copy.Get<DateTime>("datetimefield") == dateTime);
            Assert.IsTrue(obj2Copy.Get<DateTime>("datetimefield") == dateTime);
        }
    }
}
