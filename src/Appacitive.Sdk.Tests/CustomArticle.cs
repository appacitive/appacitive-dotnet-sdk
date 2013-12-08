using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk.Tests
{
    public class CustomObject : APObject
    {
        public CustomObject()
            : base("object")
        {
        }
    }

    [TestClass]
    public class Test
    {
        [TestMethod]
        public void Run()
        {
            var list = new List<int>();
            var type = list.GetType();
            Console.WriteLine(type);
        }
    }

    
}
