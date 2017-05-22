using System;
using EasyDbLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EasyDbLibTest.Query
{
    [TestClass]
    public class UpdateQueryTest
    {
        [TestMethod]
        public void TestUpdate()
        {

            var queryBuilder = new QueryBuilder(new EasyDb());

            var result = queryBuilder.Update("posts")
                .Set("title","new title")
                .Set("user_id",10)
                .Where(Condition.Op("id",1))
                .Build();

            Assert.AreEqual("update [posts] set [title]='new title',[user_id]=10 where [id]=@id", result);
        }

        [TestMethod]
        public void TestMFormattableAndColumns()
        {

            var queryBuilder = new QueryBuilder(new EasyDb());

            var result = queryBuilder.Update("dbo.posts")
                .Set("posts.title", "new title")
                .Set("posts.user_id", 10)
                .Where(Condition.Op("my id", 1))
                .Build();

            Assert.AreEqual("update [dbo].[posts] set [posts].[title]='new title',[posts].[user_id]=10 where [my id]=@my_id", result);
        }
    }
}
