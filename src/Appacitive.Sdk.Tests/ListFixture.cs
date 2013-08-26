using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Appacitive.Sdk.Tests
{
    [TestClass]
    public class ListFixture
    {
        [TestMethod]
        public async Task GetListContentsTest()
        {
            var contents = await CannedLists.GetListItemsAsync("list");
            Assert.IsTrue(contents != null );
            Assert.IsTrue(contents.TotalRecords > 0 );
            Console.WriteLine("Total records: {0}", contents.TotalRecords );
            contents.ForEach( item => Console.WriteLine("{0}) {1} : {2}", item.Position, item.Name, item.Value));
        }

        [TestMethod]
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
