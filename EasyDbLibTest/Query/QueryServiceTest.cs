using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EasyDbLib;

namespace EasyDbLibTest
{
    [TestClass]
    public class QueryServiceTest
    {
        // wrap quotes

        [TestMethod]
        public void TestWrapWithQuotes()
        {

            var service = new QueryService();
            var result = service.WrapWithQuotes("posts");

            Assert.AreEqual("[posts]", result);
        }

        // statements

        [TestMethod]
        public void TestStatement()
        {
            var service = new QueryService();
            var result = service.GetStatements(new string[] { "distinct" });

            Assert.AreEqual(" distinct", result);
        }

        [TestMethod]
        public void TestStatements()
        {
            var service = new QueryService();
            var result = service.GetStatements(new string[] { "distinct", "high_priority" });

            Assert.AreEqual(" distinct high_priority", result);
        }

        [TestMethod]
        public void TestStatements_WithEmptyArray_ReturnsEmptyString()
        {
            var service = new QueryService();
            var result = service.GetStatements(new string[] { });

            Assert.AreEqual("", result);
        }

        // limit

        [TestMethod]
        public void TestGetLimit()
        {
            var service = new QueryService();
            var result = service.GetLimit(10);

            Assert.AreEqual(" top 10", result);
        }

        [TestMethod]
        public void TestGetLimit_WithNoValue_ReturnsEmptyString()
        {
            var service = new QueryService();
            var result = service.GetLimit(null);

            Assert.AreEqual("", result);
        }

        // format table column

        [TestMethod]
        public void TestFormatTableAndColumn()
        {
            var service = new QueryService();
            var result = service.FormatTableAndColumn("posts");

            Assert.AreEqual("[posts]", result);
        }

        [TestMethod]
        public void TestFormatTableAndColumn_MultiplesParts()
        {
            var service = new QueryService();
            var result = service.FormatTableAndColumn("dbo.posts.id");

            Assert.AreEqual("[dbo].[posts].[id]", result);
        }

        // get columns

        [TestMethod]
        public void TestGetColumns_WithNoColumns_ReturnsAll()
        {
            var service = new QueryService();

            Mapping.AddTable("posts");

            var result = service.GetColumns(Mapping.GetTable("posts"));

            Assert.AreEqual(" *", result);
        }

        [TestMethod]
        public void TestGetColumns_WithOnlyPk_ReturnsAll()
        {
            var service = new QueryService();

            Mapping.AddTable("posts").AddPrimaryKeyColumn("id", "id");

            var result = service.GetColumns(Mapping.GetTable("posts"));

            Assert.AreEqual(" *", result);
        }

        [TestMethod]
        public void TestGetColumns_WithOnlyFk_ReturnsAll()
        {
            var service = new QueryService();

            Mapping.AddTable("posts").AddForeignKeyColumn("user_id", "UserId", "users", "id");

            var result = service.GetColumns(Mapping.GetTable("posts"));

            Assert.AreEqual(" *", result);
        }

        [TestMethod]
        public void TestGetColumns_WithOnlyKeys_ReturnsAll()
        {
            var service = new QueryService();

            Mapping.AddTable("posts")
                .AddPrimaryKeyColumn("id", "id")
                .AddForeignKeyColumn("user_id", "UserId", "users", "id");

            var result = service.GetColumns(Mapping.GetTable("posts"));

            Assert.AreEqual(" *", result);
        }

        [TestMethod]
        public void TestGetColumns_WithColumns_ReturnsColumns()
        {
            var service = new QueryService();

            Mapping.AddTable("posts")
                .AddColumn("title", "Title")
                .AddColumn("content", "Content");

            var result = service.GetColumns(Mapping.GetTable("posts"));

            Assert.AreEqual(" [title],[content]", result);
        }

        [TestMethod]
        public void TestGetColumns_WithColumnsAndKeys_ReturnsColumns()
        {
            var service = new QueryService();

            Mapping.AddTable("posts")
                .AddPrimaryKeyColumn("id", "id")
                .AddColumn("title", "Title")
                .AddColumn("content", "Content")
                .AddForeignKeyColumn("user_id", "UserId", "users", "id");


            var result = service.GetColumns(Mapping.GetTable("posts"));

            Assert.AreEqual(" [id],[title],[content],[user_id]", result);
        }

        [TestMethod]
        public void TestGetColumns_FormatColumns()
        {
            var service = new QueryService();

            Mapping.AddTable("posts")
                .AddPrimaryKeyColumn("posts.id", "id")
                .AddColumn("posts.title", "Title")
                .AddColumn("posts.content", "Content")
                .AddForeignKeyColumn("users.user_id", "UserId", "users", "id");


            var result = service.GetColumns(Mapping.GetTable("posts"));

            Assert.AreEqual(" [posts].[id],[posts].[title],[posts].[content],[users].[user_id]", result);
        }

        // is valid sort

        [TestMethod]
        public void TestIsValidSort()
        {
            var service = new QueryService();

            var result = service.IsValidSort("desc");

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TestIsValidSort_WithAsc()
        {
            var service = new QueryService();

            var result = service.IsValidSort("asc");

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TestIsValidSort_WithInvalid_ReturnsFalse()
        {
            var service = new QueryService();

            var result = service.IsValidSort("invalid");

            Assert.IsFalse(result);
        }

        // select

        [TestMethod]
        public void TestGetSelect()
        {
            var service = new QueryService();

            Mapping.AddTable("posts")
               .AddColumn("title", "Title")
               .AddColumn("content", "Content");

            var result = service.GetSelect(new string[] { },null, Mapping.GetTable("posts"));

            Assert.AreEqual("select [title],[content]", result);
        }

        [TestMethod]
        public void TestGetSelect_WithLimit()
        {
            var service = new QueryService();

            Mapping.AddTable("posts")
               .AddColumn("title", "Title")
               .AddColumn("content", "Content");

            var result = service.GetSelect(new string[] { }, 10, Mapping.GetTable("posts"));

            Assert.AreEqual("select top 10 [title],[content]", result);
        }

        [TestMethod]
        public void TestGetSelect_WithKeysAndFormattedColumns()
        {
            var service = new QueryService();

            Mapping.AddTable("posts")
              .AddPrimaryKeyColumn("posts.id", "id")
              .AddColumn("posts.title", "Title")
              .AddColumn("posts.content", "Content")
              .AddForeignKeyColumn("users.user_id", "UserId", "users", "id");

            var result = service.GetSelect(new string[] { }, 10, Mapping.GetTable("posts"));

            Assert.AreEqual("select top 10 [posts].[id],[posts].[title],[posts].[content],[users].[user_id]", result);
        }

        [TestMethod]
        public void TestGetSelect_WithNoColumn_ReturnsAll()
        {
            var service = new QueryService();

            Mapping.AddTable("posts");

            var result = service.GetSelect(new string[] { }, 10, Mapping.GetTable("posts"));

            Assert.AreEqual("select top 10 *", result);
        }

        [TestMethod]
        public void TestGetSelect_WithStatements()
        {
            var service = new QueryService();

            Mapping.AddTable("posts")
               .AddColumn("title", "Title")
               .AddColumn("content", "Content");

            var result = service.GetSelect(new string[] { "distinct", "high_priority" }, null, Mapping.GetTable("posts"));

            Assert.AreEqual("select distinct high_priority [title],[content]", result);
        }

        [TestMethod]
        public void TestGetSelect_WithStatementsAndLimit()
        {
            var service = new QueryService();

            Mapping.AddTable("posts")
               .AddColumn("title", "Title")
               .AddColumn("content", "Content");

            var result = service.GetSelect(new string[] { "distinct", "high_priority" }, 10, Mapping.GetTable("posts"));

            Assert.AreEqual("select distinct high_priority top 10 [title],[content]", result);
        }

        [TestMethod]
        public void TestGetSelect_WithStatementsAndLimitAndNoColumns()
        {
            var service = new QueryService();

            Mapping.AddTable("posts");

            var result = service.GetSelect(new string[] { "distinct", "high_priority" }, 10, Mapping.GetTable("posts"));

            Assert.AreEqual("select distinct high_priority top 10 *", result);
        }

        // from

        [TestMethod]
        public void TestGetFrom()
        {
            var service = new QueryService();

            var result = service.GetFrom("posts");

            Assert.AreEqual(" from [posts]", result);
        }

        [TestMethod]
        public void TestGetFrom_FormatTable()
        {
            var service = new QueryService();

            var result = service.GetFrom("dbo.posts");

            Assert.AreEqual(" from [dbo].[posts]", result);
        }

        // join order by 

        [TestMethod]
        public void TestJoinOrder()
        {
            var service = new QueryService();

            var result = service.JoinOrderByStringValue("created_at desc");

            Assert.AreEqual("[created_at] desc", result);
        }

        [TestMethod]
        public void TestJoinOrder_WithAsc()
        {
            var service = new QueryService();

            var result = service.JoinOrderByStringValue("created_at asc");

            Assert.AreEqual("[created_at] asc", result);
        }

        [TestMethod]
        public void TestJoinOrder_WithNoDirection()
        {
            var service = new QueryService();

            var result = service.JoinOrderByStringValue("created_at");

            Assert.AreEqual("[created_at]", result);
        }

        [TestMethod]
        public void TestJoinOrder_FormatColumn()
        {
            var service = new QueryService();

            var result = service.JoinOrderByStringValue("posts.created_at desc");

            Assert.AreEqual("[posts].[created_at] desc", result);
        }

        [TestMethod]
        public void TestJoinOrder_FaildWithInvalidSort()
        {
            bool failed = true;
            var service = new QueryService();

            try
            {
                var result = service.JoinOrderByStringValue("created_at invalid");
            }
            catch (Exception)
            {
                failed = true;
            }

            Assert.IsTrue(failed);
        }

        [TestMethod]
        public void TestJoinOrder_FaildWithInvalidParts()
        {
            bool failed = true;
            var service = new QueryService();

            try
            {
                var result = service.JoinOrderByStringValue("created_at desc desc desc");
            }
            catch (Exception)
            {
                failed = true;
            }

            Assert.IsTrue(failed);
        }

        // get sorts

        [TestMethod]
        public void TestGetSorts()
        {
            var service = new QueryService();

            var result = service.GetSorts(new string[]{ "posts.created_at desc", "title"});

            Assert.AreEqual("[posts].[created_at] desc,[title]", result);
        }

        // get order by

        [TestMethod]
        public void TestGetOrderBy()
        {
            var service = new QueryService();

            var result = service.GetOrderBy(new string[] { "posts.created_at desc", "title" });

            Assert.AreEqual(" order by [posts].[created_at] desc,[title]", result);
        }

        [TestMethod]
        public void TestGetOrderBy_WithNull_ReturnsEmptyString()
        {
            var service = new QueryService();

            var result = service.GetOrderBy(null);

            Assert.AreEqual("", result);
        }

        [TestMethod]
        public void TestGetOrderBy_WithNoSorts_ReturnsEmptyString()
        {
            var service = new QueryService();

            var result = service.GetOrderBy(new string[] { });

            Assert.AreEqual("", result);
        }

        // where condition

        [TestMethod]
        public void TestConditionOp_WithNoOperator_IsEquals()
        {
            var result = Condition.Op("id", 10);
            Assert.AreEqual("id", result.Column);
            Assert.AreEqual("=", result.Operator);
            Assert.AreEqual(10, result.Value);
        }

        [TestMethod]
        public void TestConditionOp_WithOperator()
        {
            var result = Condition.Op("id", ">", 10);
            Assert.AreEqual("id", result.Column);
            Assert.AreEqual(">", result.Operator);
            Assert.AreEqual(10, result.Value);
        }

        // get where

        [TestMethod]
        public void TestGetWhere()
        {
            var service = new QueryService();
            var condition = Condition.Op("a", "<", 10).And(Condition.Op("b", ">", 20)).Or(Condition.Between("c", 60, 100));
            var container = new ConditionAndParameterContainer(condition);
            var result = service.GetWhere(container);
            Assert.AreEqual(" where [a]<@a and [b]>@b or [c] between 60 and 100", result);
        }

        [TestMethod]
        public void TestGetWhere_RetrunsUniques()
        {
            var service = new QueryService();
            var condition = Condition.Op("a", "<", 10).And(Condition.Op("a", ">", 20));
            var container = new ConditionAndParameterContainer(condition);
            var result = service.GetWhere(container);
            Assert.AreEqual(" where [a]<@a and [a]>@a2", result);
        }


        [TestMethod]
        public void TestGetWhere_WithNull_ReturnsEmptyString()
        {
            var service = new QueryService();
            var result = service.GetWhere(null);
            Assert.AreEqual("", result);
        }

        // where has one

        [TestMethod]
        public void TestGetWhereHasOne()
        {
            var service = new QueryService();

            Mapping.AddTable("posts").AddForeignKeyColumn("user_id", "UserId", "users", "id");

            var result = service.GetWhereHasOne(Mapping.GetTable("posts").GetForeignKeys("users"));

            Assert.AreEqual(" where [id]=@user_id", result);
        }

        [TestMethod]
        public void TestGetWhereHasOne_WithMultipleForeignKeys()
        {
            var service = new QueryService();

            Mapping.AddTable("posts")
                .AddForeignKeyColumn("user_id", "UserId", "users", "id")
                .AddForeignKeyColumn("user_id2", "UserId2", "users", "id2");

            var result = service.GetWhereHasOne(Mapping.GetTable("posts").GetForeignKeys("users"));

            Assert.AreEqual(" where [id]=@user_id and [id2]=@user_id2", result);
        }

    }
}
