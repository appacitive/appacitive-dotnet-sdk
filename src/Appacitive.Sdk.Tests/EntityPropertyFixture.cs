using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Tests
{
    [TestClass]
    public class EntityPropertyFixture
    {
        [TestMethod]
        public void ItemAddTest()
        {
            
            APObject obj = new APObject("object");
            obj.SetList<int>("field1", new[] { 1, 2, 3, 4, 5, 6 });
            var field1 = obj.GetList<int>("field1");
            Assert.IsFalse(field1.Contains(10));
            Assert.IsFalse(field1.Contains(11));
            obj.AddItems("field1", 10, 11);
            field1 = obj.GetList<int>("field1");
            Assert.IsTrue(field1.Contains(10));
            Assert.IsTrue(field1.Contains(11));
        }

        [TestMethod]
        public void ItemAddTestWithoutDuplication()
        {
            APObject obj = new APObject("object");
            obj.SetList<int>("field1", new[] { 1, 2, 3, 4, 5, 6 });
            var field1 = obj.GetList<int>("field1");
            Assert.IsTrue(field1.Count() == 6);
            obj.AddItems("field1", false, 5, 6);
            field1 = obj.GetList<int>("field1");
            Assert.IsTrue(field1.Count() == 6);
        }

        [TestMethod]
        public void RemoveFirstOccurenceOfItemTest()
        {
            APObject obj = new APObject("object");
            obj.SetList<int>("field1", new [] { 1,2,3,1,2,3 });
            var removed = obj.RemoveItems("field1", 1, true);
            Assert.IsTrue(removed == true);
            var list = obj.GetList<int>("field1").ToList();
            Assert.IsTrue(list.Count == 5);
            Assert.IsTrue(list.Count(x => x == 1) == 1 );
        }


        [TestMethod]
        public void InvalidAddItemsTest()
        {
            var obj = new APObject("object");
            obj["age"] = 10;
            try
            {
                obj.AddItems("age", 2, 3);
                Assert.Fail("Add item to integer property should have failed.");
            }
            catch (ArgumentException)
            {
            }

        }

        [TestMethod]
        public void InvalidRemoveItemsTest()
        {
            var obj = new APObject("object");
            obj["age"] = 10;
            try
            {
                obj.RemoveItems("age", 2);
                Assert.Fail("Add item to integer property should have failed.");
            }
            catch (ArgumentException)
            {
            }

        }

        [TestMethod]
        public void RemoveAllOccurencesOfItemTest()
        {
            APObject obj = new APObject("object");
            obj.SetList<int>("field1", new[] { 1, 2, 3, 1, 2, 3 });
            var removed = obj.RemoveItems("field1", 1, false);
            Assert.IsTrue(removed == true);
            var list = obj.GetList<int>("field1").ToList();
            Assert.IsTrue(list.Count == 4);
            Assert.IsTrue(list.Count(x => x == 1) == 0);
        }

        [TestMethod]
        public void AddItemsToNullPropertyTest()
        {
            APObject obj = new APObject("object");
            obj.AddItems("field1", 5, 6);
            Assert.IsTrue(obj.GetList<string>("field1").Count() == 2);
        }

        [TestMethod]
        public void ItemAddTestWithDuplication()
        {
            APObject obj = new APObject("object");
            obj.SetList<int>("field1", new[] { 1, 2, 3, 4, 5, 6 });
            var field1 = obj.GetList<int>("field1");
            Assert.IsTrue(field1.Count() == 6);
            obj.AddItems("field1", true, 5, 6);
            field1 = obj.GetList<int>("field1");
            Assert.IsTrue(field1.Count() == 8);
        }

        [TestMethod]
        public void EntityIndexerReadWriteTest()
        {
            dynamic apObject = new APObject("object");
            AssertIndexerGetSet<int>(apObject, "intfield", 10);
            AssertIndexerGetSet<uint>(apObject, "intfield", 10);
            AssertIndexerGetSet<long>(apObject, "intfield", 10);
            AssertIndexerGetSet<float>(apObject, "decimalfield", 10.0f);
            AssertIndexerGetSet<double>(apObject, "decimalfield", 10.0);
            AssertIndexerGetSet<decimal>(apObject, "decimalfield", 10.0m);
            AssertIndexerGetSet<string>(apObject, "strinfield", "test value");
            AssertIndexerGetSet<bool>(apObject, "boolfield", true);
            AssertIndexerGetSet<char>(apObject, "stringfield", 'x');
            AssertIndexerGetSet<DateTime>(apObject, "datetimefield", DateTime.Now);
        }


        [TestMethod]
        public void MultiValuedGetSetTest()
        {
            dynamic apObject = new APObject("object");
            AssertMultiValuedGetSet<int>(apObject, "field1", new int[] { 1, 2, 3, 4, 5, 6 });
            AssertMultiValuedGetSet<uint>(apObject, "field2", new uint[] { 1, 2, 3, 4, 5, 6 });
            AssertMultiValuedGetSet<long>(apObject, "field3", new long[] { 1, 2, 3, 4, 5, 6 });
            AssertMultiValuedGetSet<ulong>(apObject, "field4", new ulong[] { 1, 2, 3, 4, 5, 6 });

            AssertMultiValuedGetSet<float>(apObject, "field5", new float[] { 1, 2, 3, 4, 5, 6 });
            AssertMultiValuedGetSet<double>(apObject, "field6", new double[] { 1, 2, 3, 4, 5, 6 });
            AssertMultiValuedGetSet<decimal>(apObject, "field7", new decimal[] { 1, 2, 3, 4, 5, 6 });

            AssertMultiValuedGetSet<string>(apObject, "field8", new string[] { "a", "b", "c" });
            AssertMultiValuedGetSet<bool>(apObject, "field9", new bool[] { true, true, false, true, false});
            AssertMultiValuedGetSet<DateTime>(apObject, "field10", new DateTime[] { DateTime.Now, DateTime.Now, DateTime.Now });
            AssertMultiValuedGetSet<char>(apObject, "field11", new char[] { 'a', 'b', 'c' });
        }

        private void AssertMultiValuedGetSet<T>(dynamic apObject, string field, IEnumerable<T> values)
        {
            Console.WriteLine("Testing for Object.GetList<{0}>() and Object.SetList<{0}>().", typeof(T).Name);
            var initial = values.ToList();
            apObject.SetList<T>(field, initial);
            List<T> other = new List<T>();
            other.AddRange(apObject.GetList<T>(field));
            Assert.IsTrue(initial.Count == other.Count);
            for (int i = 0; i < initial.Count; i++)
                Assert.IsTrue(initial[i].Equals(other[i]));
            
        }

        [TestMethod]
        public void EntityGetSetTest()
        {
            dynamic apObject = new APObject("object");
            AssertGetSet<int>(apObject, "field1", 10);
            AssertGetSet<uint>(apObject, "field2", 10);
            AssertGetSet<long>(apObject, "field3", 10);
            AssertGetSet<float>(apObject, "field4", 10.0f);
            AssertGetSet<double>(apObject, "field5", 10.0d);
            AssertGetSet<decimal>(apObject, "field6", 10.0m);
            AssertGetSet<string>(apObject, "field7", "test string");
            AssertGetSet<char>(apObject, "field8", 'x');
            AssertGetSet<DateTime>(apObject, "field9", DateTime.Now);
        }

        [TestMethod]
        public void TryGetSetMultiValuedPropertyTest()
        {
            var apObject = new APObject("object");
            try
            {
                apObject.Set<int[]>("field1", new[] { 1, 3, 4, 5, 6 });
                Assert.Fail("Multivalue set should not be allowed on Object.Set<T>().");
            }
            catch (ArgumentException)
            {
            }

            try
            {
                var values = apObject.Get<int[]>("field1");
                Assert.Fail("Multivalue get should not be allowed on Object.Get<T>().");
            }
            catch (ArgumentException)
            {
            }
        }

        [TestMethod]
        public void TestGeocodeSet()
        {
            var apObject = new APObject("object");
            apObject.Set("geofield", new Geocode(80.0m, 81.0m));
            var value = apObject.Get<string>("geofield");
            Assert.IsTrue(value == "80.0,81.0");
        }

        [TestMethod]
        public void TestGeocodeGet()
        {
            var apObject = new APObject("object");
            apObject.Set("geofield", new Geocode(80.0m, 81.0m));
            var geo = apObject.Get<Geocode>("geofield");
            Assert.IsTrue(geo.ToString() == "80.0,81.0");
        }

        private void AssertIndexerGetSet<T>(dynamic apObject, string field, T value)
        {
            apObject["field"] = value;
            Assert.IsTrue(apObject["field"].GetValue<T>() == value);
        }

        private void AssertGetSet<T>(dynamic apObject, string field, T value)
        {
            apObject.Set<T>(field, value);
            Assert.IsTrue(apObject.Get<T>(field) == value);
        }
        
    }
}
