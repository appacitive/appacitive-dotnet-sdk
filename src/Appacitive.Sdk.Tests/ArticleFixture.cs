using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Appacitive.Sdk.Tests
{
    [TestClass]
    public class ArticleFixture
    {
        [TestMethod]
        public void ArticleNumericPropertyTest()
        {
            dynamic score = new Article("score");
            score.Total = 50;
            // Add decimal
            score.Total = 100.0m + score.Total;
            decimal decimalTotal = score.Total;
            Assert.IsTrue(decimalTotal == 150.0m, "Decimal additional failed.");

            // Add float
            score.Total = 50;
            score.Total = 100.0f + score.Total;
            float floatTotal = score.Total;
            Assert.IsTrue(floatTotal == 150.0f, "Float additional failed.");
        }

        [TestMethod]
        public void ArticleStringPropertyTest()
        {
            dynamic movie = new Article("movie");
            movie.name = "100 days";
            movie.@rating = "1";
            movie.@rating++;
            int into10 = movie.rating * 10;
        }

    }
}
