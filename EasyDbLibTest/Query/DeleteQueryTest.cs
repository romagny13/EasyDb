using System;
using EasyDbLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EasyDbLibTest.Query
{
    [TestClass]
    public class DeleteQueryTest
    {
        [TestMethod]
        public void TestDelete()
        {
            var queryBuilder = new QueryBuilder(new EasyDb());

            var result = queryBuilder.DeleteFrom("posts")
                .Where(Condition.Op("id", 1))
                .Build();

            Assert.AreEqual("delete from [posts] where [id]=@id", result);
        }

        [TestMethod]
        public void TestDeleteWithMultipleConditions()
        {
            var queryBuilder = new QueryBuilder(new EasyDb());

            var result = queryBuilder.DeleteFrom("posts")
                .Where(Condition.Op("id", 1).Or(Condition.Op("id",2)))
                .Build();

            Assert.AreEqual("delete from [posts] where [id]=@id or [id]=@id2", result);
        }
    }
}
