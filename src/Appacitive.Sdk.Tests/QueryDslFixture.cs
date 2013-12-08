using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Appacitive.Sdk.Tests
{
    [TestClass]
    public class QueryDslFixture
    {

        [TestMethod]
        public void SimpleQueryTest()
        {
            var query = Query.Property("name").IsEqualTo("nikhil");
            var query2 = Query.Property("__id").IsEqualTo("12345");
            Console.WriteLine(query.ToString());
            Console.WriteLine(query2.ToString());
        }

        [TestMethod]
        public void AggregatedQueryTest()
        {
            var query =
                Query.And(new[] {
                    Query.Property("name").IsEqualTo("nikhil"),
                    Query.Property("age").IsGreaterThanEqualTo(10),
                });
            Console.WriteLine(query.ToString());
        }

        [TestMethod]
        public void RadialQueryTest()
        {
            var query = Query.Property("location").WithinCircle(new Geocode(10, 10), 15);
            Console.WriteLine(query);
        }

        [TestMethod]
        public void DatetimeQueryTest()
        {
            Console.WriteLine("Date time query");
            var query = Query.Property("datetime").IsGreaterThan(DateTime.Now);
            Console.WriteLine(query);
            Console.WriteLine("Date query");
            query = Query.Property("datetime").IsGreaterThanDate(DateTime.Today);
            Console.WriteLine(query);
            Console.WriteLine("Time query");
            query = Query.Property("datetime").IsGreaterThanTime(DateTime.Now);
            Console.WriteLine(query);
        }

        [TestMethod]
        public void QueryWithSingleQuoteShouldBeEscapedTest()
        {
            var query = Query.Property("string_field").IsEqualTo("steve's house").ToString();
            var expected = @"*string_field == '" + Uri.EscapeDataString(@"steve\'s house") + "'";
            Assert.AreEqual(query, expected);
        }


        [TestMethod]
        public void NonLiteralsInStringQueryShouldBeEscapedTest()
        {
            var query = Query.Property("string_field").IsEqualTo("hello world").ToString();
            var expected = @"*string_field == '" + Uri.EscapeDataString("hello world") + "'";
            Assert.AreEqual(query, expected);
        }


        [TestMethod]
        public void PolygonQueryTest()
        {
            var query = Query.Property("location").WithinPolygon( 
                new [] 
                {
                    new Geocode(10,10),
                    new Geocode(13,14),
                    new Geocode(20,20)
                });
            Console.WriteLine(query);
        }

        [TestMethod]
        public void NestedQueryTest()
        {
            var query =
                Query.Or(
                    Query.Property("name").StartsWith("x"),
                    Query.And(
                        Query.Property("name").IsEqualTo("nikhil"),
                        Query.Property("age").IsGreaterThanEqualTo(10)
                        ),
                    Query.Property("location").WithinCircle(new Geocode(10, 10), 15, DistanceUnit.Kilometers),
                    Query.Property("location").WithinPolygon(
                        new Geocode(10,10),
                        new Geocode(13,14),
                        new Geocode(20,20)
                        )
                );
            Console.WriteLine(query.ToString());
        }


        [TestMethod]
        public async Task MatchQueryTest()
        {
            var propertyValue = Unique.String;
            var attrValue = Unique.String;
            var obj = new APObject("object");
            obj.Set<string>("stringfield", propertyValue);
            obj.SetAttribute("test_attribute", attrValue);
            await obj.SaveAsync();

            var propertyQuery = Query.Property("stringfield").FreeTextMatches(propertyValue).ToString();
            var attrQuery = Query.Attribute("test_attribute").FreeTextMatches(attrValue).ToString();
            var result1 = await APObjects.FindAllAsync("object", propertyQuery);
            var result2 = await APObjects.FindAllAsync("object", attrQuery);
            Assert.IsNotNull(result1);
            Assert.IsTrue(result1.Count == 1);
            Assert.IsNotNull(result2);
            Assert.IsTrue(result2.Count == 1);
            Assert.IsTrue(result1.Single().Id == obj.Id);
            Assert.IsTrue(result2.Single().Id == obj.Id);
        }
    }
}
