using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EasyDbLib.Tests.Condition
{
    [TestClass]
    public class CheckLikeTests
    {
        [TestMethod]
        public void WithoutColumnName_ThrowException()
        {
            bool failed = false;
            try
            {
                var result = Check.Like("", "%abc");
            }
            catch (System.Exception)
            {
                failed = true;
            }

            Assert.IsTrue(failed);
        }

        [TestMethod]
        public void CheckLike()
        {
            var result = Check.Like("id","%abc");
            Assert.AreEqual("id", result.ColumnName);
            Assert.AreEqual("%abc", result.Value);
        }
    }
}
