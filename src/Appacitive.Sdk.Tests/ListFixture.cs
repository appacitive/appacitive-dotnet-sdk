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
    public class ListFixture
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
        public async Task GetListContentsTest()
        {
            var contents = await CannedLists.GetListItemsAsync("list");
            Assert.IsTrue(contents != null );
            Assert.IsTrue(contents.TotalRecords > 0 );
            Console.WriteLine("Total records: {0}", contents.TotalRecords );
            contents.ForEach( item => Console.WriteLine("{0}) {1} : {2}", item.Position, item.Name, item.Value));
        }

        #if MONO
		[Test]
		[Timeout(int.MaxValue)]
		#else
		[TestMethod]
		#endif
        public async Task GetListContentsWithPagingTest()
        {
            var contents = await CannedLists.GetListItemsAsync("list");
            Assert.IsTrue(contents != null);
            Assert.IsTrue(contents.TotalRecords > 0);
            
            var index = contents.TotalRecords - 1;
            var pagedContents = await CannedLists.GetListItemsAsync("list", index, 1);
            Assert.IsTrue(pagedContents != null);
            Assert.IsTrue(pagedContents.Count == 1);
            var item = pagedContents.Single();
            var itemAtIndex = contents[index-1];
            Assert.IsTrue(item.Name == itemAtIndex.Name);
            Assert.IsTrue(item.Value == itemAtIndex.Value);
            Assert.IsTrue(item.Description == itemAtIndex.Description);
            Assert.IsTrue(item.Position == itemAtIndex.Position);
        }

    }
}
