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
            Console.WriteLine(query.ToString());
        }

        [TestMethod]
        public void AggregatedQueryTest()
        {
            var query = 
                BooleanOperator.And( new [] {
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
                BooleanOperator.Or( new [] 
                { 
                    Query.Property("name").StartsWith("x"),
                    BooleanOperator.And(new[] 
                    {
                        Query.Property("name").IsEqualTo("nikhil"),
                        Query.Property("age").IsGreaterThanEqualTo(10),
                    }),
                    Query.Property("location").WithinCircle(new Geocode(10, 10), 15, DistanceUnit.Kilometers),
                    Query.Property("location").WithinPolygon( 
                    new [] 
                    {
                        new Geocode(10,10),
                        new Geocode(13,14),
                        new Geocode(20,20)
                    })
                });
            Console.WriteLine(query.ToString());
        }

    }
}
