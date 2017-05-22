using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EasyDbLib;

namespace EasyDbLibTest
{
    [TestClass]
    public class InsertQueryTests
    {
        [TestMethod]
        public void TestFaildWithoutColumns()
        {
            bool failed = false;
            var queryBuilder = new QueryBuilder(new EasyDb());

            try
            {
                queryBuilder.InsertInto("posts").build();
            }
            catch (Exception)
            {
                failed = true;
            }
            Assert.IsTrue(failed);
        }

        [TestMethod]
        public void TestBuild()
        {
            var queryBuilder = new QueryBuilder(new EasyDb());

            var result = queryBuilder.InsertInto("posts").
               Columns("title", "content")
               .build();

            Assert.AreEqual("insert into [posts] ([title],[content]) values (@title,@content)", result);
        }

        [TestMethod]
        public void TestFormatColumns()
        {
            var queryBuilder = new QueryBuilder(new EasyDb());

            var result = queryBuilder.InsertInto("dbo.posts").
               Columns("dbo.title", "dbo.my content")
               .build();

            Assert.AreEqual("insert into [dbo].[posts] ([dbo].[title],[dbo].[my content]) values (@title,@my_content)", result);
        }
    }
}
