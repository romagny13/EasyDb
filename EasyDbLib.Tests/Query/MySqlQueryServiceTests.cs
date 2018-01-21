using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EasyDbLib;

namespace EasyDbLib.Tests
{
    [TestClass]
    public class MySqlQueryServiceTests
    {

        public MySqlQueryService GetService()
        {
            return new MySqlQueryService();
        }


        [TestMethod]
        public void WrapWithQuotes()
        {
            var service = this.GetService();
            Assert.AreEqual("`posts`", service.WrapWithQuotes("posts"));
            Assert.AreEqual("`posts`", service.WrapWithQuotes("`posts`"));
        }

        // limit

        [TestMethod]
        public void GetLimit()
        {
            var service = this.GetService();
            var result = service.GetLimit(10);

            Assert.AreEqual(" limit 10", result);
        }

        [TestMethod]
        public void GetLimit_WithNoValue_ReturnsEmptyString()
        {
            var service = this.GetService();
            var result = service.GetLimit(null);

            Assert.AreEqual("", result);
        }

        // format table column

        [TestMethod]
        public void FormatTableAndColumn()
        {
            var service = this.GetService();
            var result = service.FormatTableAndColumn("posts");

            Assert.AreEqual("`posts`", result);
        }

        [TestMethod]
        public void FormatTableAndColumn_MultiplesParts()
        {
            var service = this.GetService();
            var result = service.FormatTableAndColumn("posts.id");

            Assert.AreEqual("`posts`.`id`", result);
        }

        // select

        [TestMethod]
        public void GetSelect()
        {
            var service = this.GetService();
            var columns = new string[] { "title", "content" };

            var result = service.GetSelect(null, columns, "posts", null, null);

            Assert.AreEqual("select `title`,`content` from `posts`", result);
        }

        [TestMethod]
        public void GetSelect_FormatColumns()
        {
            var service = this.GetService();
            var columns = new string[] { "posts.title", "posts.content" };

            var result = service.GetSelect(null, columns, "test.posts", null, null);

            Assert.AreEqual("select `posts`.`title`,`posts`.`content` from `test`.`posts`", result);
        }

        [TestMethod]
        public void GetSelect_WithLimit()
        {
            var service = this.GetService();
            var columns = new string[] { "title", "content" };

            var result = service.GetSelect(10, columns, "posts", null, null);

            Assert.AreEqual("select `title`,`content` from `posts` limit 10", result);
        }

        [TestMethod]
        public void GetParameterName()
        {
            var service = this.GetService();
            Assert.AreEqual("@id", service.GetParameterName("Id"));
            Assert.AreEqual("@role_id", service.GetParameterName("Role Id"));
            Assert.AreEqual("@id", service.GetParameterName("`test`.`Id`"));
        }
    }
}
