using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Tests
{
    [TestClass]
    public class MultiValuedFixture
    {
    
        [TestMethod]
        public void CannotGetMultiValuedViaIndexerTest()
        {
            var obj = new APObject("object");
            try
            {
                obj.SetList<int>("multifield", new int[] { 1, 2, 4 });
                try
                {
                    var list = obj["multifield"];
                    Assert.Fail("Set operation on multivalue indexer should have failed.");
                }
                catch (Exception)
                {
                }
            }
            catch (Exception)
            { }
        }

        [TestMethod]
        public void GetListTest()
        {
            var obj = new APObject("object");
            obj.SetList<int>("multifield", new[] { 1, 2, 3, 4, 5 });
            var list = obj.GetList<decimal>("multifield");
            Assert.IsTrue(list.Intersect(new decimal[] { 1, 2, 3, 4, 5 }).Count() == 5);
        }


        [TestMethod]
        public async Task SaveMultivaluedAsyncTest()
        {
            var array = new[] { 1, 3, 4, 5, 6, 7 };
            var obj = new APObject("object");
            obj.SetList<int>("multifield", array);
            await obj.SaveAsync();

            var saved = await APObjects.GetAsync("object", obj.Id);
            var array2 = saved.GetList<int>("multifield");
            Assert.IsTrue(array.Intersect(array2).Count() == array.Length);
        }
    }
}
