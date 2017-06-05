using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EasyDbLib;

namespace EasyDbLibTest.Query
{
    [TestClass]
    public class MySQLQueryServiceTest
    {

        public MySQLQueryService GetService()
        {
            return new MySQLQueryService();
        }

        // limit

        [TestMethod]
        public void TestGetLimit()
        {
            var service = this.GetService();
            var result = service.GetLimit(10);

            Assert.AreEqual(" limit 10", result);
        }

        [TestMethod]
        public void TestGetLimit_WithNoValue_ReturnsEmptyString()
        {
            var service = this.GetService();
            var result = service.GetLimit(null);

            Assert.AreEqual("", result);
        }

        // format table column

        [TestMethod]
        public void TestFormatTableAndColumn()
        {
            var service = this.GetService();
            var result = service.FormatTableAndColumn("posts");

            Assert.AreEqual("`posts`", result);
        }

        [TestMethod]
        public void TestFormatTableAndColumn_MultiplesParts()
        {
            var service = this.GetService();
            var result = service.FormatTableAndColumn("posts.id");

            Assert.AreEqual("`posts`.`id`", result);
        }

        // select

        [TestMethod]
        public void TestGetSelectFormOrderBy()
        {
            var service = this.GetService();

            Mapping.SetTable("posts")
               .SetColumn("title", "Title")
               .SetColumn("content", "Content");

            var result = service.GetSelectFromOrderBy(Mapping.GetTable("posts"), new string[] { }, null, null, new string[] { });

            Assert.AreEqual("select `title`,`content` from `posts`", result);
        }

        [TestMethod]
        public void TestGetSelectFormOrderBy_WithLimit()
        {
            var service = this.GetService();

            Mapping.SetTable("posts")
               .SetColumn("title", "Title")
               .SetColumn("content", "Content");

            var result = service.GetSelectFromOrderBy(Mapping.GetTable("posts"), new string[] { },10, null, new string[] { });

            Assert.AreEqual("select `title`,`content` from `posts` limit 10", result);
        }

        [TestMethod]
        public void TestGetSelectFormOrderBy_WithWhreOrderByAndLimit_LimitIsLastParameter()
        {
            var service = this.GetService();

            Mapping.SetTable("posts")
               .SetColumn("title", "Title")
               .SetColumn("content", "Content");

            var condition = new ConditionAndParameterContainer(Check.Op("id", 1));
            var result = service.GetSelectFromOrderBy(Mapping.GetTable("posts"), new string[] { "distinct" }, 10, condition , new string[] { "title" });

            Assert.AreEqual("select distinct `title`,`content` from `posts` where `id`=@id order by `title` limit 10", result);
        }
    }
}
