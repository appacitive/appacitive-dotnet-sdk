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
            DynamicValue value = new DynamicValue("test");
            Assert.IsTrue(value == "test", "Equality test Value == <string> test failed.");
            // RHS
            Assert.IsTrue("test" == value, "Equality test <string> == Value test failed.");
            // Value
            var value2 = new DynamicValue("test");
            Assert.IsTrue(value == value2, "Equality test Value == Value test failed.");
        }

        [TestMethod]
        public void StringInEqualityOperatorTest()
        {
            // LHS 
            DynamicValue value = new DynamicValue("test1");
            Assert.IsTrue(value != "test", "Equality test Value == <string> test failed.");
            // RHS
            Assert.IsTrue("test" != value, "Equality test <string> == Value test failed.");
            // Value
            var value2 = new DynamicValue("test");
            Assert.IsTrue(value != value2, "Equality test Value == Value test failed.");
        }

        [TestMethod]
        public void IntegerEqualityOperatorTest()
        {
            // As integer
            // LHS 
            int original = 10;
            int compareTo = 10;
            DynamicValue value = new DynamicValue(original);
            Assert.IsTrue(value == compareTo, "Equality test Value == <int> test failed.");
            // RHS
            Assert.IsTrue(compareTo == value, "Equality test <int> == Value test failed.");
            // Value
            var value2 = new DynamicValue(original);
            Assert.IsTrue(value == value2, "Equality test Value == Value test failed.");
        }

        [TestMethod]
        public void IntegerInEqualityOperatorTest()
        {
            // LHS 
            DynamicValue value = new DynamicValue(10);
            Assert.IsTrue(value != 12, "Equality test Value == <int> test failed.");
            // RHS
            Assert.IsTrue(12 != value, "Equality test <int> == Value test failed.");
            // Value
            var value2 = new DynamicValue(12);
            Assert.IsTrue(value != value2, "Equality test Value == Value test failed.");
        }

        [TestMethod]
        public void LongEqualityOperatorTest()
        {
            // As integer
            // LHS 
            DynamicValue value = new DynamicValue(10L);
            Assert.IsTrue(value == 10L, "Equality test Value == <long> test failed.");
            // RHS
            Assert.IsTrue(10L == value, "Equality test <long> == Value test failed.");
            // Value
            var value2 = new DynamicValue(10L);
            Assert.IsTrue(value == value2, "Equality test Value == Value test failed.");
        }

        [TestMethod]
        public void LongInEqualityOperatorTest()
        {
            // LHS 
            DynamicValue value = new DynamicValue(10L);
            Assert.IsTrue(value != 12L, "Equality test Value == <long> test failed.");
            // RHS
            Assert.IsTrue(12L != value, "Equality test <long> == Value test failed.");
            // Value
            var value2 = new DynamicValue(12L);
            Assert.IsTrue(value != value2, "Equality test Value == Value test failed.");
        }

        [TestMethod]
        public void BoolEqualityOperatorTest()
        {
            // As integer
            // LHS 
            DynamicValue value = new DynamicValue(true);
            Assert.IsTrue(value == true, "Equality test Value == <bool> test failed.");
            // RHS
            Assert.IsTrue(true == value, "Equality test <bool> == Value test failed.");
            // Value
            var value2 = new DynamicValue(true);
            Assert.IsTrue(value == value2, "Equality test Value == Value test failed.");
        }

        [TestMethod]
        public void BoolInEqualityOperatorTest()
        {
            // LHS 
            DynamicValue value = new DynamicValue(true);
            Assert.IsTrue(value != false, "Equality test Value == <bool> test failed.");
            // RHS
            Assert.IsTrue(false != value, "Equality test <bool> == Value test failed.");
            // Value
            var value2 = new DynamicValue(false);
            Assert.IsTrue(value != value2, "Equality test Value == Value test failed.");
        }

        [TestMethod]
        public void DecimalEqualityOperatorTest()
        {
            // As integer
            // LHS 
            DynamicValue value = new DynamicValue(10M);
            Assert.IsTrue(value == 10M, "Equality test Value == <Decimal> test failed.");
            // RHS
            Assert.IsTrue(10M == value, "Equality test <Decimal> == Value test failed.");
            // Value
            var value2 = new DynamicValue(10M);
            Assert.IsTrue(value == value2, "Equality test Value == Value test failed.");
        }

        [TestMethod]
        public void DecimalInEqualityOperatorTest()
        {
            // LHS 
            DynamicValue value = new DynamicValue(10M);
            Assert.IsTrue(value != 12M, "Equality test Value == <Decimal> test failed.");
            // RHS
            Assert.IsTrue(12M != value, "Equality test <Decimal> == Value test failed.");
            // Value
            var value2 = new DynamicValue(12M);
            Assert.IsTrue(value != value2, "Equality test Value == Value test failed.");
        }

        [TestMethod]
        public void DoubleEqualityOperatorTest()
        {
            // As integer
            // LHS 
            DynamicValue value = new DynamicValue(10d);
            Assert.IsTrue(value == 10d, "Equality test Value == <Double> test failed.");
            // RHS
            Assert.IsTrue(10d == value, "Equality test <Double> == Value test failed.");
            // Value
            var value2 = new DynamicValue(10d);
            Assert.IsTrue(value == value2, "Equality test Value == Value test failed.");
        }

        [TestMethod]
        public void DoubleInEqualityOperatorTest()
        {
            // LHS 
            DynamicValue value = new DynamicValue(10d);
            Assert.IsTrue(value != 12d, "Equality test Value == <Double> test failed.");
            // RHS
            Assert.IsTrue(12d != value, "Equality test <Double> == Value test failed.");
            // Value
            var value2 = new DynamicValue(12d);
            Assert.IsTrue(value != value2, "Equality test Value == Value test failed.");
        }

        [TestMethod]
        public void FloatEqualityOperatorTest()
        {
            // As integer
            // LHS 
            DynamicValue value = new DynamicValue(10f);
            Assert.IsTrue(value == 10f, "Equality test Value == <Float> test failed.");
            // RHS
            Assert.IsTrue(10f == value, "Equality test <Float> == Value test failed.");
            // Value
            var value2 = new DynamicValue(10f);
            Assert.IsTrue(value == value2, "Equality test Value == Value test failed.");
        }

        [TestMethod]
        public void FloatInEqualityOperatorTest()
        {
            // LHS 
            DynamicValue value = new DynamicValue(10f);
            Assert.IsTrue(value != 12f, "Equality test Value == <Float> test failed.");
            // RHS
            Assert.IsTrue(12f != value, "Equality test <Float> == Value test failed.");
            // Value
            var value2 = new DynamicValue(12f);
            Assert.IsTrue(value != value2, "Equality test Value == Value test failed.");
        }

        [TestMethod]
        public void DateTimeEqualityOperatorTest()
        {
            var date1 = new DateTime(2012, 11, 10, 10, 10, 10);
            var date2 = new DateTime(2012, 11, 10, 10, 10, 10);
            DynamicValue value = new DynamicValue(date1);
            Assert.IsTrue(value == date2, "Equality test Value == <DateTime> test failed.");
            // RHS
            Assert.IsTrue(date2 == value, "Equality test <DateTime> == Value test failed.");
            // Value
            var value2 = new DynamicValue(date2);
            Assert.IsTrue(value == value2, "Equality test Value == Value test failed.");
        }

        [TestMethod]
        public void DateTimeInEqualityOperatorTest()
        {
            var date1 = new DateTime(2012, 11, 10, 10, 10, 10);
            var date2 = new DateTime(2012, 12, 10, 10, 10, 10);
            // LHS
            DynamicValue value = new DynamicValue(date1);
            Assert.IsTrue(value != date2, "Equality test Value == <Float> test failed.");
            // RHS
            Assert.IsTrue(date2 != value, "Equality test <Float> == Value test failed.");
            // Value
            var value2 = new DynamicValue(date2);
            Assert.IsTrue(value != value2, "Equality test Value == Value test failed.");
        }

        [TestMethod]
        public void ImplicitIntConversionTest()
        {
            var data = 10;
            var value = new DynamicValue(data);
            int result = value;
            Assert.IsTrue(result == data, "Value to " + data.GetType().Name + " conversion failed.");
            DynamicValue value2 = data;
            Assert.IsTrue(data.ToString() == value2.StringValue, data.GetType().Name + " to value conversion failed.");
        }


        [TestMethod]
        public void ImplicitLongConversionTest()
        {
            var data = 10L;
            var value = new DynamicValue(data);
            long result = value;
            Assert.IsTrue(result == data, "Value to " + data.GetType().Name + " conversion failed.");
            DynamicValue value2 = data;
            Assert.IsTrue(data.ToString() == value2.StringValue, data.GetType().Name + " to value conversion failed.");
        }

        [TestMethod]
        public void ImplicitDecimalConversionTest()
        {
            var data = 10M;
            var value = new DynamicValue(data);
            decimal result = value;
            Assert.IsTrue(result == data, "Value to " + data.GetType().Name + " conversion failed.");
            DynamicValue value2 = data;
            Assert.IsTrue(data.ToString() == value2.StringValue, data.GetType().Name + " to value conversion failed.");
        }

        [TestMethod]
        public void ImplicitFloatConversionTest()
        {
            var data = 10f;
            var value = new DynamicValue(data);
            float result = value;
            Assert.IsTrue(result == data, "Value to " + data.GetType().Name + " conversion failed.");
            DynamicValue value2 = data;
            Assert.IsTrue(data.ToString() == value2.StringValue, data.GetType().Name + " to value conversion failed.");
        }

        [TestMethod]
        public void ImplicitDoubleConversionTest()
        {
            var data = 10d;
            var value = new DynamicValue(data);
            double result = value;
            Assert.IsTrue(result == data, "Value to " + data.GetType().Name + " conversion failed.");
            DynamicValue value2 = data;
            Assert.IsTrue(data.ToString() == value2.StringValue, data.GetType().Name + " to value conversion failed.");
        }

        [TestMethod]
        public void ImplicitDateConversionTest()
        {
            var value = new DynamicValue("2010-10-10");
            DateTime date = value;
            Assert.IsTrue(date == new DateTime(2010,10,10));
        }

        [TestMethod]
        public void ImplicitTimeConversionTest()
        {
            var value = new DynamicValue("10:45:35.1234567");
            DateTime date = value;
        }
    }
}
