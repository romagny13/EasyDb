using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EasyDbLib.Tests.Condition
{
    [TestClass]
    public class CheckOpTests
    {

        [TestMethod]
        public void WithoutColumnName_ThrowException()
        {
            bool failed = false;
            try
            {
                var result = Check.Op("", 10);
            }
            catch (System.Exception)
            {
                failed = true;
            }

            Assert.IsTrue(failed);
        }


        [TestMethod]
        public void TestConditionOp_WithNoOperator_IsEquals()
        {
            var result = Check.Op("id", 10);
            Assert.AreEqual("id", result.ColumnName);
            Assert.AreEqual("=", result.Operator);
            Assert.AreEqual(10, result.Value);
        }

        [TestMethod]
        public void TestConditionOp_WithOperator()
        {
            var result = Check.Op("id", ">", 10);
            Assert.AreEqual("id", result.ColumnName);
            Assert.AreEqual(">", result.Operator);
            Assert.AreEqual(10, result.Value);
        }
    }


}
