using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EasyDbLib;

namespace EasyDbLibTest.Core
{
    [TestClass]
    public class NameCheckerTest
    {
     
        [TestMethod]
        public void TestValidPropertyName()
        {
            Assert.IsTrue(NameChecker.CheckPropertyName("valid_table"));
        }

        [TestMethod]
        public void TestValidPropertyName_WithArobase()
        {
            Assert.IsTrue(NameChecker.CheckPropertyName("@valid_table"));
        }

        [TestMethod]
        public void TestValidPropertyName_WithNumberAtTheEnd()
        {
            Assert.IsTrue(NameChecker.CheckPropertyName("@valid_table123"));
        }

        [TestMethod]
        public void TestValidPropertyName_WithNumberAtStart_Fail()
        {
            Assert.IsFalse(NameChecker.CheckPropertyName("123valid_table"));
        }

        [TestMethod]
        public void TestValidPropertyName_WithArobaseNotAtStart_Fail()
        {
            Assert.IsFalse(NameChecker.CheckPropertyName("valid_@table"));
        }
    }
}
