using Appacitive.Sdk.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
        public void ArticleSerializerSelectionTest()
        {
            ArticleConverter converter = new ArticleConverter();
            Assert.IsTrue( converter.CanConvert(typeof(Article)));
            Assert.IsTrue( converter.CanConvert(typeof(ObjectArticle)));
        }
    }

    public class ObjectArticle : Article
    {
        public ObjectArticle() : base("object")
        {
        }
    }
}
