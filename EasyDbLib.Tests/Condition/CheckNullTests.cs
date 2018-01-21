using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EasyDbLib.Tests.Condition
{
    [TestClass]
    public class CheckNullTests
    {
        [TestMethod]
        public void WithoutColumnName_ThrowException()
        {
            bool failed = false;
            try
            {
                var result = Check.IsNull("");
            }
            catch (System.Exception)
            {
                failed = true;
            }

            Assert.IsTrue(failed);
        }

        [TestMethod]
        public void CheckIsNull()
        {
            var result = Check.IsNull("id");
            Assert.AreEqual("id", result.ColumnName);
            Assert.IsTrue(result.ValueIsNull);
        }

        [TestMethod]
        public void CheckIsNotNull()
        {
            var result = Check.IsNotNull("id");
            Assert.AreEqual("id", result.ColumnName);
            Assert.IsFalse(result.ValueIsNull);
        }
    }
}
