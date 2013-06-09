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
            
            Article obj = new Article("object");
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
            Article obj = new Article("object");
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
            Article obj = new Article("object");
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
            var obj = new Article("object");
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
            var obj = new Article("object");
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
            Article obj = new Article("object");
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
            Article obj = new Article("object");
            obj.AddItems("field1", 5, 6);
            Assert.IsTrue(obj.GetList<string>("field1").Count() == 2);
        }

        [TestMethod]
        public void ItemAddTestWithDuplication()
        {
            Article obj = new Article("object");
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
            dynamic article = new Article("object");
            AssertIndexerGetSet<int>(article, "intfield", 10);
            AssertIndexerGetSet<uint>(article, "intfield", 10);
            AssertIndexerGetSet<long>(article, "intfield", 10);
            AssertIndexerGetSet<float>(article, "decimalfield", 10.0f);
            AssertIndexerGetSet<double>(article, "decimalfield", 10.0);
            AssertIndexerGetSet<decimal>(article, "decimalfield", 10.0m);
            AssertIndexerGetSet<string>(article, "strinfield", "test value");
            AssertIndexerGetSet<bool>(article, "boolfield", true);
            AssertIndexerGetSet<char>(article, "stringfield", 'x');
            AssertIndexerGetSet<DateTime>(article, "datetimefield", DateTime.Now);
        }


        [TestMethod]
        public void MultiValuedGetSetTest()
        {
            dynamic article = new Article("object");
            AssertMultiValuedGetSet<int>(article, "field1", new int[] { 1, 2, 3, 4, 5, 6 });
            AssertMultiValuedGetSet<uint>(article, "field2", new uint[] { 1, 2, 3, 4, 5, 6 });
            AssertMultiValuedGetSet<long>(article, "field3", new long[] { 1, 2, 3, 4, 5, 6 });
            AssertMultiValuedGetSet<ulong>(article, "field4", new ulong[] { 1, 2, 3, 4, 5, 6 });

            AssertMultiValuedGetSet<float>(article, "field5", new float[] { 1, 2, 3, 4, 5, 6 });
            AssertMultiValuedGetSet<double>(article, "field6", new double[] { 1, 2, 3, 4, 5, 6 });
            AssertMultiValuedGetSet<decimal>(article, "field7", new decimal[] { 1, 2, 3, 4, 5, 6 });

            AssertMultiValuedGetSet<string>(article, "field8", new string[] { "a", "b", "c" });
            AssertMultiValuedGetSet<bool>(article, "field9", new bool[] { true, true, false, true, false});
            AssertMultiValuedGetSet<DateTime>(article, "field10", new DateTime[] { DateTime.Now, DateTime.Now, DateTime.Now });
            AssertMultiValuedGetSet<char>(article, "field11", new char[] { 'a', 'b', 'c' });
        }

        private void AssertMultiValuedGetSet<T>(dynamic article, string field, IEnumerable<T> values)
        {
            Console.WriteLine("Testing for Article.GetList<{0}>() and Article.SetList<{0}>().", typeof(T).Name);
            var initial = values.ToList();
            article.SetList<T>(field, initial);
            List<T> other = new List<T>();
            other.AddRange(article.GetList<T>(field));
            Assert.IsTrue(initial.Count == other.Count);
            for (int i = 0; i < initial.Count; i++)
                Assert.IsTrue(initial[i].Equals(other[i]));
            
        }

        [TestMethod]
        public void EntityGetSetTest()
        {
            dynamic article = new Article("object");
            AssertGetSet<int>(article, "field1", 10);
            AssertGetSet<uint>(article, "field2", 10);
            AssertGetSet<long>(article, "field3", 10);
            AssertGetSet<float>(article, "field4", 10.0f);
            AssertGetSet<double>(article, "field5", 10.0d);
            AssertGetSet<decimal>(article, "field6", 10.0m);
            AssertGetSet<string>(article, "field7", "test string");
            AssertGetSet<char>(article, "field8", 'x');
            AssertGetSet<DateTime>(article, "field9", DateTime.Now);
        }

        [TestMethod]
        public void TryGetSetMultiValuedPropertyTest()
        {
            var article = new Article("object");
            try
            {
                article.Set<int[]>("field1", new[] { 1, 3, 4, 5, 6 });
                Assert.Fail("Multivalue set should not be allowed on Article.Set<T>().");
            }
            catch (ArgumentException)
            {
            }

            try
            {
                var values = article.Get<int[]>("field1");
                Assert.Fail("Multivalue get should not be allowed on Article.Get<T>().");
            }
            catch (ArgumentException)
            {
            }
        }

        private void AssertIndexerGetSet<T>(dynamic article, string field, T value)
        {
            article["field"] = value;
            Assert.IsTrue(article["field"].GetValue<T>() == value);
        }

        private void AssertGetSet<T>(dynamic article, string field, T value)
        {
            article.Set<T>(field, value);
            Assert.IsTrue(article.Get<T>(field) == value);
        }
        
    }
}
