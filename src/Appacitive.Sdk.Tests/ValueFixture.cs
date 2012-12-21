using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Appacitive.Sdk.Tests
{
    [TestClass]
    public class ValueFixture 
    {
        [TestMethod]
        public void StringEqualityOperatorTest()
        {
            // LHS 
            Value value = new Value("test");
            Assert.IsTrue(value == "test", "Equality test Value == <string> test failed.");
            // RHS
            Assert.IsTrue("test" == value, "Equality test <string> == Value test failed.");
            // Value
            var value2 = new Value("test");
            Assert.IsTrue(value == value2, "Equality test Value == Value test failed.");
        }

        [TestMethod]
        public void StringInEqualityOperatorTest()
        {
            // LHS 
            Value value = new Value("test1");
            Assert.IsTrue(value != "test", "Equality test Value == <string> test failed.");
            // RHS
            Assert.IsTrue("test" != value, "Equality test <string> == Value test failed.");
            // Value
            var value2 = new Value("test");
            Assert.IsTrue(value != value2, "Equality test Value == Value test failed.");
        }

        [TestMethod]
        public void IntegerEqualityOperatorTest()
        {
            // As integer
            // LHS 
            int original = 10;
            int compareTo = 10;
            Value value = new Value(original);
            Assert.IsTrue(value == compareTo, "Equality test Value == <int> test failed.");
            // RHS
            Assert.IsTrue(compareTo == value, "Equality test <int> == Value test failed.");
            // Value
            var value2 = new Value(original);
            Assert.IsTrue(value == value2, "Equality test Value == Value test failed.");
        }

        [TestMethod]
        public void IntegerInEqualityOperatorTest()
        {
            // LHS 
            Value value = new Value(10);
            Assert.IsTrue(value != 12, "Equality test Value == <int> test failed.");
            // RHS
            Assert.IsTrue(12 != value, "Equality test <int> == Value test failed.");
            // Value
            var value2 = new Value(12);
            Assert.IsTrue(value != value2, "Equality test Value == Value test failed.");
        }

        [TestMethod]
        public void LongEqualityOperatorTest()
        {
            // As integer
            // LHS 
            Value value = new Value(10L);
            Assert.IsTrue(value == 10L, "Equality test Value == <long> test failed.");
            // RHS
            Assert.IsTrue(10L == value, "Equality test <long> == Value test failed.");
            // Value
            var value2 = new Value(10L);
            Assert.IsTrue(value == value2, "Equality test Value == Value test failed.");
        }

        [TestMethod]
        public void LongInEqualityOperatorTest()
        {
            // LHS 
            Value value = new Value(10L);
            Assert.IsTrue(value != 12L, "Equality test Value == <long> test failed.");
            // RHS
            Assert.IsTrue(12L != value, "Equality test <long> == Value test failed.");
            // Value
            var value2 = new Value(12L);
            Assert.IsTrue(value != value2, "Equality test Value == Value test failed.");
        }

        [TestMethod]
        public void BoolEqualityOperatorTest()
        {
            // As integer
            // LHS 
            Value value = new Value(true);
            Assert.IsTrue(value == true, "Equality test Value == <bool> test failed.");
            // RHS
            Assert.IsTrue(true == value, "Equality test <bool> == Value test failed.");
            // Value
            var value2 = new Value(true);
            Assert.IsTrue(value == value2, "Equality test Value == Value test failed.");
        }

        [TestMethod]
        public void BoolInEqualityOperatorTest()
        {
            // LHS 
            Value value = new Value(true);
            Assert.IsTrue(value != false, "Equality test Value == <bool> test failed.");
            // RHS
            Assert.IsTrue(false != value, "Equality test <bool> == Value test failed.");
            // Value
            var value2 = new Value(false);
            Assert.IsTrue(value != value2, "Equality test Value == Value test failed.");
        }

        [TestMethod]
        public void DecimalEqualityOperatorTest()
        {
            // As integer
            // LHS 
            Value value = new Value(10M);
            Assert.IsTrue(value == 10M, "Equality test Value == <Decimal> test failed.");
            // RHS
            Assert.IsTrue(10M == value, "Equality test <Decimal> == Value test failed.");
            // Value
            var value2 = new Value(10M);
            Assert.IsTrue(value == value2, "Equality test Value == Value test failed.");
        }

        [TestMethod]
        public void DecimalInEqualityOperatorTest()
        {
            // LHS 
            Value value = new Value(10M);
            Assert.IsTrue(value != 12M, "Equality test Value == <Decimal> test failed.");
            // RHS
            Assert.IsTrue(12M != value, "Equality test <Decimal> == Value test failed.");
            // Value
            var value2 = new Value(12M);
            Assert.IsTrue(value != value2, "Equality test Value == Value test failed.");
        }

        [TestMethod]
        public void DoubleEqualityOperatorTest()
        {
            // As integer
            // LHS 
            Value value = new Value(10d);
            Assert.IsTrue(value == 10d, "Equality test Value == <Double> test failed.");
            // RHS
            Assert.IsTrue(10d == value, "Equality test <Double> == Value test failed.");
            // Value
            var value2 = new Value(10d);
            Assert.IsTrue(value == value2, "Equality test Value == Value test failed.");
        }

        [TestMethod]
        public void DoubleInEqualityOperatorTest()
        {
            // LHS 
            Value value = new Value(10d);
            Assert.IsTrue(value != 12d, "Equality test Value == <Double> test failed.");
            // RHS
            Assert.IsTrue(12d != value, "Equality test <Double> == Value test failed.");
            // Value
            var value2 = new Value(12d);
            Assert.IsTrue(value != value2, "Equality test Value == Value test failed.");
        }

        [TestMethod]
        public void FloatEqualityOperatorTest()
        {
            // As integer
            // LHS 
            Value value = new Value(10f);
            Assert.IsTrue(value == 10f, "Equality test Value == <Float> test failed.");
            // RHS
            Assert.IsTrue(10f == value, "Equality test <Float> == Value test failed.");
            // Value
            var value2 = new Value(10f);
            Assert.IsTrue(value == value2, "Equality test Value == Value test failed.");
        }

        [TestMethod]
        public void FloatInEqualityOperatorTest()
        {
            // LHS 
            Value value = new Value(10f);
            Assert.IsTrue(value != 12f, "Equality test Value == <Float> test failed.");
            // RHS
            Assert.IsTrue(12f != value, "Equality test <Float> == Value test failed.");
            // Value
            var value2 = new Value(12f);
            Assert.IsTrue(value != value2, "Equality test Value == Value test failed.");
        }

        [TestMethod]
        public void DateTimeEqualityOperatorTest()
        {
            var date1 = new DateTime(2012, 11, 10, 10, 10, 10);
            var date2 = new DateTime(2012, 11, 10, 10, 10, 10);
            Value value = new Value(date1);
            Assert.IsTrue(value == date2, "Equality test Value == <DateTime> test failed.");
            // RHS
            Assert.IsTrue(date2 == value, "Equality test <DateTime> == Value test failed.");
            // Value
            var value2 = new Value(date2);
            Assert.IsTrue(value == value2, "Equality test Value == Value test failed.");
        }

        [TestMethod]
        public void DateTimeInEqualityOperatorTest()
        {
            var date1 = new DateTime(2012, 11, 10, 10, 10, 10);
            var date2 = new DateTime(2012, 12, 10, 10, 10, 10);
            // LHS
            Value value = new Value(date1);
            Assert.IsTrue(value != date2, "Equality test Value == <Float> test failed.");
            // RHS
            Assert.IsTrue(date2 != value, "Equality test <Float> == Value test failed.");
            // Value
            var value2 = new Value(date2);
            Assert.IsTrue(value != value2, "Equality test Value == Value test failed.");
        }


    }
}
