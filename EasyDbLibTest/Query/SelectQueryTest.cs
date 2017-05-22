using System;
using EasyDbLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EasyDbLibTest
{
    [TestClass]
    public class SelectQueryTest
    {
        // select

        [TestMethod]
        public void TestSelectFrom()
        {
           var builder = new QueryBuilder(new EasyDb());
            var result = builder.Select("id", "firstname")
                .From("users")
                .Build();

            Assert.AreEqual("select [id],[firstname] from [users]", result);
        }

        [TestMethod]
        public void TestSelectWithAllColumns()
        {
            var builder = new QueryBuilder(new EasyDb());
            var result = builder.Select()
                .From("users")
                .Build();

            Assert.AreEqual("select * from [users]", result);
        }

        [TestMethod]
        public void TestSelectWithAllColumnsExplicit()
        {
            var builder = new QueryBuilder(new EasyDb());
            var result = builder.Select("*")
                .From("users")
                .Build();

            Assert.AreEqual("select * from [users]", result);
        }

        [TestMethod]
        public void TestFailedWithNoTable()
        {
            bool failed = false;
            var builder = new QueryBuilder(new EasyDb());
            try
            {
                var result = builder.Select("*")
                    .Build();
            }
            catch (Exception e)
            {
                failed = true;
            }

            Assert.IsTrue(failed);
        }

        // options

        [TestMethod]
        public void TestOption()
        {
            var builder = new QueryBuilder(new EasyDb());
            var result = builder.Select("id", "firstname")
                .Statements("DISTINCT")
                .From("users")
                .Build();

            Assert.AreEqual("select DISTINCT [id],[firstname] from [users]", result);
        }

        [TestMethod]
        public void TestOptions()
        {
            var builder = new QueryBuilder(new EasyDb());
            var result = builder.Select("id", "firstname")
                .Statements("DISTINCT", "HIGH_PRIORITY")
                .From("users")
                .Build();

            Assert.AreEqual("select DISTINCT HIGH_PRIORITY [id],[firstname] from [users]", result);
        }

        // top

        [TestMethod]
        public void TestTop()
        {
            var builder = new QueryBuilder(new EasyDb());
            var result = builder.Select("id", "firstname")
                .Top(10)
                .From("users")
                .Build();

            Assert.AreEqual("select TOP 10 [id],[firstname] from [users]", result);
        }

        [TestMethod]
        public void TestOptionsAndTop()
        {
            var builder = new QueryBuilder(new EasyDb());
            var result = builder.Select("id", "firstname")
                .Statements("DISTINCT", "HIGH_PRIORITY")
                .Top(10)
                .From("users")
                .Build();

            Assert.AreEqual("select DISTINCT HIGH_PRIORITY TOP 10 [id],[firstname] from [users]", result);
        }

        // from tables

        [TestMethod]
        public void TestWithMultipleTables()
        {
            var builder = new QueryBuilder(new EasyDb());
            var result = builder.Select()
                .From("posts","users")
                .Build();

            Assert.AreEqual("select * from [posts],[users]", result);
        }

        [TestMethod]
        public void TestWithMultipleTablesAndColumns()
        {
            var builder = new QueryBuilder(new EasyDb());
            var result = builder.Select("posts.id","title","content","users.id","users.name")
                .From("posts", "users")
                .Build();

            Assert.AreEqual("select [posts].[id],[title],[content],[users].[id],[users].[name] from [posts],[users]", result);
        }

        [TestMethod]
        public void TestWithMultipleTablesAndColumnsAndDbo()
        {
            var builder = new QueryBuilder(new EasyDb());
            var result = builder.Select("dbo.posts.id", "title", "content", "dbo.users.id", "users.name")
                .From("dbo.posts", "dbo.users")
                .Build();

            Assert.AreEqual("select [dbo].[posts].[id],[title],[content],[dbo].[users].[id],[users].[name] from [dbo].[posts],[dbo].[users]", result);
        }

        // where

        [TestMethod]
        public void TestWhereWithCondition()
        {
            var builder = new QueryBuilder(new EasyDb());
            var result = builder.Select("title", "content")
                .From("posts")
                .Where(Condition.Op("user_id",1))
                .Build();

            Assert.AreEqual("select [title],[content] from [posts] where [user_id]=@user_id", result);
        }

        [TestMethod]
        public void TestWhereWithConditions()
        {
            var builder = new QueryBuilder(new EasyDb());
            var result = builder.Select("title", "content")
                .From("posts")
                .Where(Condition.Op("user_id", 1).And(Condition.Op("posts.category id", 10)))
                .Build();

            Assert.AreEqual("select [title],[content] from [posts] where [user_id]=@user_id and [posts].[category id]=@category_id", result);
        }


        // order by

        [TestMethod]
        public void TestOrderBy()
        {
            var builder = new QueryBuilder(new EasyDb());
            var result = builder.Select("title", "content")
                .From("posts")
                .OrderBy("posts.title","created_at desc")
                .Build();

            Assert.AreEqual("select [title],[content] from [posts] order by [posts].[title],[created_at] desc", result);
        }

        [TestMethod]
        public void TestOrderBy_WithSort()
        {
            var builder = new QueryBuilder(new EasyDb());
            var result = builder.Select("title", "content")
                .From("posts")
                .OrderBy(Sort.Asc("posts.title"),Sort.Desc("created_at"))
                .Build();

            Assert.AreEqual("select [title],[content] from [posts] order by [posts].[title],[created_at] desc", result);
        }
    }
}
