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
    public class GeocodeFixture
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
        public void GeocodeParsingTest()
        {
            var geo = new Geocode(10.0m, 10.0m);
            var valid = new[] 
            { 
                "10,10",        // without decimal points
                "10.0,10.0",    // normal
                " 10.0,10.0",   // with leading spaces
                "10.0,10.0",    // with trailing spaces
                "10.0 ,10.0 ",  // with leading space before comma
                "10.0, 10.0"    // with trailing space after comma
            };
            Array.ForEach(valid, value =>
                {
                    Geocode geocode = null;
                    Assert.IsTrue(Geocode.TryParse(value, out geocode), "Valid value {0} was not parsed correctly.", value);
                    Assert.IsNotNull(geocode, "Geocode parsed for {0} was null.", value);
                    Assert.IsTrue(geocode.Equals(geo), "Expected {0} but received {1}.", geo.ToString(), geocode.ToString());
                });
        }

        #if MONO
		[Test]
		[Timeout(int.MaxValue)]
		#else
		[TestMethod]
		#endif
        public void InvalidGeocodeTest()
        {
            var invalid = new[] 
            { 
                null,               // null
                string.Empty,       // empty
                " ",                // space
                "10,10,123",        // incorrect number of args
                "abc,abc",          // non numeric values
                "91,100",           // incorrect latitude
                "75,190",           // incorrect longitude
            };
            Array.ForEach(invalid, value =>
            {
                Geocode geocode = null;
                Assert.IsFalse(Geocode.TryParse(value, out geocode), "Invalid value {0} parsed successfully as {1}", value, geocode);
                Assert.IsNull(geocode, "Invalid value {0} parsed successfully as {1}", value, geocode);
            });
        }

        #if MONO
		[Test]
		[Timeout(int.MaxValue)]
		#else
		[TestMethod]
		#endif
        public void GeocodeEqualityTest()
        {
            var geo1 = new Geocode(10m, 10.5m);
            var geo2 = new Geocode(20m / 2, 21m / 2);
            var geo3 = new Geocode(10m, 11m);       // different latitude
            var geo4 = new Geocode(12m, 10.5m);     // different longitude

			Assert.AreEqual(geo1, geo2);
			Assert.AreNotEqual(geo1, geo3);
			Assert.AreNotEqual(geo1, geo4);


        }

        #if MONO
		[Test]
		[Timeout(int.MaxValue)]
		#else
		[TestMethod]
		#endif
        public void GetHashCodeTest()
        {
            var map = new Dictionary<Geocode, int>();
            var geoKey1 = new Geocode(10.5m, 10.6m);
            map[geoKey1] = 10;
            var geoKey2 = new Geocode(10.5m, 10.6m);
            int result;
            Assert.IsTrue(map.TryGetValue(geoKey2, out result));
            Assert.AreEqual(10, result);
            
        }
    }
}
