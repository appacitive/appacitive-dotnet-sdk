using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Tests
{
    [TestClass]
    public class GraphSearchFixture
    {
        [TestMethod]
        public async Task FilterWithArgsTest()
        {
            var parent = await ObjectHelper.CreateNewAsync();
            var unique = Unique.String;
            var child = ObjectHelper.NewInstance();
            child.Set<string>("stringfield", unique);
            var conn = Connection.New("link").FromExistingArticle("parent", parent.Id).ToNewArticle("child", child);
            await conn.SaveAsync();

            // Run filter
            var results = await Graph.Filter("sample_filter", new Dictionary<string, string> { { "search_value", unique } });
            Assert.IsTrue(results.Count == 1);
            Assert.IsTrue(results[0] == parent.Id);
        }

        [TestMethod]
        public async Task FilterWithQueryObjectTest()
        {
            var parent = await ObjectHelper.CreateNewAsync();
            var unique = Unique.String;
            var child = ObjectHelper.NewInstance();
            child.Set<string>("stringfield", unique);
            var conn = Connection.New("link").FromExistingArticle("parent", parent.Id).ToNewArticle("child", child);
            await conn.SaveAsync();

            // Run filter
            var results = await Graph.Filter("sample_filter", new { search_value = unique } );
            Assert.IsTrue(results.Count == 1);
            Assert.IsTrue(results[0] == parent.Id);
        }

    }
}
