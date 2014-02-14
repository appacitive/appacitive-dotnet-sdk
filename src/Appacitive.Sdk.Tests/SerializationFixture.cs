using Appacitive.Sdk.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Tests
{
    [TestClass]
    public class SerializationFixture
    {
        [TestMethod]
        public void ObjectSerializerSelectionTest()
        {
            ObjectConverter converter = new ObjectConverter();
            Assert.IsTrue( converter.CanConvert(typeof(APObject)));
            Assert.IsTrue( converter.CanConvert(typeof(CustomObject2)));
        }

        
    }

    public class CustomObject2 : APObject
    {
        public CustomObject2() : base("object")
        {
        }
    }
}
