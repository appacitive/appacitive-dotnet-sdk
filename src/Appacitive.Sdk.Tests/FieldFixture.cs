using Appacitive.Sdk.Internal;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Tests
{
    [TestClass]
    public class FieldFixture
    {
        [TestMethod]
        public void ToStringTest()
        {
            var property = Field.Property("property");
            var attribute = Field.Attribute("attribute");
            var aggregate = Field.Aggregate("aggregate");
            Assert.AreEqual("*property", property.ToString());
            Assert.AreEqual("@attribute", attribute.ToString());
            Assert.AreEqual("$aggregate", aggregate.ToString());
        }

    }
}
