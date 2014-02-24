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
            var conn = APConnection.New("link").FromExistingObject("parent", parent.Id).ToNewObject("child", child);
            await conn.SaveAsync();

            // Run filter
            var results = await Graph.Query("sample_filter", new Dictionary<string, string> { { "search_value", unique } });
            Assert.IsTrue(results.Count == 1);
            Assert.IsTrue(results[0] == parent.Id);
        }

        

        [TestMethod]
        public async Task ProjectWithArgsTest()
        {
            // Create sample data 
            string val1 = Unique.String, val2 = Unique.String;
            var root = await ObjectHelper.CreateNewAsync();
            var level1Child = ObjectHelper.NewInstance();
            level1Child.Set<string>("stringfield", val1);
            var level1Edge = APConnection.New("link").FromExistingObject("parent", root.Id).ToNewObject("child", level1Child);
            await level1Edge.SaveAsync();

            var level2Child = ObjectHelper.NewInstance();
            level2Child.Set<string>("stringfield", val2);
            var level2Edge = APConnection.New("link").FromExistingObject("parent", level1Child.Id).ToNewObject("child", level2Child);
            await level2Edge.SaveAsync();

            // Run filter
            var results = await Graph.Select("sample_project", 
                new [] { root.Id },
                new Dictionary<string, string> { { "level1_filter", val1 }, { "level2_filter", val2 } });

            Assert.IsTrue(results.Count == 1);
            Assert.IsTrue(results[0].Object != null);
            Assert.IsTrue(results[0].Object.Id  == root.Id);

            var level1Children = results[0].GetChildren("level1_children");
            Assert.IsTrue(level1Children.Count == 1);
            Assert.IsTrue(level1Children[0].Object != null);
            Assert.IsTrue(level1Children[0].Object.Id == level1Child.Id);
            Assert.IsTrue(level1Children[0].Connection != null);
            Assert.IsTrue(level1Children[0].Connection.Id == level1Edge.Id);
            Assert.IsTrue(level1Children[0].Connection.Endpoints["parent"].ObjectId == root.Id);
            Assert.IsTrue(level1Children[0].Connection.Endpoints["child"].ObjectId == level1Child.Id);

            var level2Children = level1Children[0].GetChildren("level2_children");
            Assert.IsTrue(level2Children.Count == 1);
            Assert.IsTrue(level2Children[0].Object != null);
            Assert.IsTrue(level2Children[0].Object.Id == level2Child.Id);
            Assert.IsTrue(level2Children[0].Connection != null);
            Assert.IsTrue(level2Children[0].Connection.Id == level2Edge.Id);
            Assert.IsTrue(level2Children[0].Connection.Endpoints["parent"].ObjectId == level1Child.Id);
            Assert.IsTrue(level2Children[0].Connection.Endpoints["child"].ObjectId == level2Child.Id);
        }
        
    }
}
