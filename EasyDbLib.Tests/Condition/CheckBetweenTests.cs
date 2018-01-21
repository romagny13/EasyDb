using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EasyDbLib.Tests.Condition
{
    [TestClass]
    public class CheckBetweenTests
    {
        [TestMethod]
        public void WithoutColumnName_ThrowException()
        {
            bool failed = false;
            try
            {
                var result = Check.Between("", 10, 20);
            }
            catch (System.Exception)
            {
                failed = true;
            }

            Assert.IsTrue(failed);
        }

        [TestMethod]
        public void CheckBetween()
        {
            var result = Check.Between("id", 10, 20);
            Assert.AreEqual("id", result.ColumnName);
            Assert.AreEqual(10, result.Value1);
            Assert.AreEqual(20, result.Value2);
        }
    }
}
