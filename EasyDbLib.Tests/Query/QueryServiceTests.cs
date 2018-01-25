using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EasyDbLib.Tests
{
    [TestClass]
    public class QueryServiceTests
    {

        public SqlQueryService GetService()
        {
            return new SqlQueryService();
        }

        // wrap quotes

        [TestMethod]
        public void WrapWithQuotes()
        {
            var service = this.GetService();
            Assert.AreEqual("[posts]", service.WrapWithQuotes("posts"));
            Assert.AreEqual("[posts]", service.WrapWithQuotes("[posts]"));
        }

        // limit

        [TestMethod]
        public void GetLimit()
        {
            var service = this.GetService();
            var result = service.GetLimit(10);

            Assert.AreEqual(" top 10", result);
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

            Assert.AreEqual("[posts]", result);
        }

        [TestMethod]
        public void FormatTableAndColumn_MultiplesParts()
        {
            var service = this.GetService();
            var result = service.FormatTableAndColumn("dbo.posts.id");

            Assert.AreEqual("[dbo].[posts].[id]", result);
        }

        // get columns

        [TestMethod]
        public void GetColumns_WithNoColumns_ReturnsEmpty()
        {
            var service = this.GetService();

            var columns = new string[] { };

            var result = service.GetColumns(columns);

            Assert.AreEqual(" ", result);
        }


        [TestMethod]
        public void GetColumns_WithColumns_ReturnsColumns()
        {
            var service = this.GetService();

            var columns = new string[] { "title", "content" };

            var result = service.GetColumns(columns);

            Assert.AreEqual(" [title],[content]", result);
        }

        [TestMethod]
        public void GetColumns_FormatColumns()
        {
            var service = this.GetService();

            var columns = new string[] { "posts.title", "posts.content" };

            var result = service.GetColumns(columns);

            Assert.AreEqual(" [posts].[title],[posts].[content]", result);
        }

        // is valid sort

        [TestMethod]
        public void IsValidSort()
        {
            var service = this.GetService();

            var result = service.IsValidSort("desc");

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsValidSort_WithAsc()
        {
            var service = this.GetService();

            var result = service.IsValidSort("asc");

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsValidSort_WithInvalid_ReturnsFalse()
        {
            var service = this.GetService();

            var result = service.IsValidSort("invalid");

            Assert.IsFalse(result);
        }

        // select

        [TestMethod]
        public void GetSelect()
        {
            var service = this.GetService();

            var columns = new string[] { "title", "content" };

            var result = service.GetSelect(null, columns);

            Assert.AreEqual("select [title],[content]", result);
        }

        [TestMethod]
        public void GetSelect_WithLimit()
        {
            var service = this.GetService();

            var columns = new string[] { "title", "content" };

            var result = service.GetSelect(10, columns);

            Assert.AreEqual("select top 10 [title],[content]", result);
        }

        [TestMethod]
        public void GetSelect_WithKeysAndFormattedColumns()
        {
            var service = this.GetService();

            var columns = new string[] { "posts.title", "posts.content" };

            var result = service.GetSelect(10, columns);

            Assert.AreEqual("select top 10 [posts].[title],[posts].[content]", result);
        }

        [TestMethod]
        public void GetSelect_WithNoColumn_ReturnsAll()
        {
            var service = this.GetService();

            var columns = new string[] { };

            var result = service.GetSelect(10, columns);

            Assert.AreEqual("select top 10 *", result);
        }

        // from

        [TestMethod]
        public void GetFrom()
        {
            var service = this.GetService();

            var result = service.GetFrom("posts");

            Assert.AreEqual(" from [posts]", result);
        }

        [TestMethod]
        public void GetFrom_FormatTable()
        {
            var service = this.GetService();

            var result = service.GetFrom("dbo.posts");

            Assert.AreEqual(" from [dbo].[posts]", result);
        }

        // join order by 

        [TestMethod]
        public void JoinOrder()
        {
            var service = this.GetService();

            var result = service.JoinOrderByStringValue("created_at desc");

            Assert.AreEqual("[created_at] desc", result);
        }

        [TestMethod]
        public void JoinOrder_WithAsc()
        {
            var service = this.GetService();

            var result = service.JoinOrderByStringValue("created_at asc");

            Assert.AreEqual("[created_at] asc", result);
        }

        [TestMethod]
        public void JoinOrder_WithNoDirection()
        {
            var service = this.GetService();

            var result = service.JoinOrderByStringValue("created_at");

            Assert.AreEqual("[created_at]", result);
        }

        [TestMethod]
        public void JoinOrder_FormatColumn()
        {
            var service = this.GetService();

            var result = service.JoinOrderByStringValue("posts.created_at desc");

            Assert.AreEqual("[posts].[created_at] desc", result);
        }

        [TestMethod]
        public void JoinOrder_FaildWithInvalidSort()
        {
            bool failed = true;
            var service = this.GetService();

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
        public void JoinOrder_FaildWithInvalidParts()
        {
            bool failed = true;
            var service = this.GetService();

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
        public void GetSorts()
        {
            var service = this.GetService();

            var result = service.GetSorts(new string[] { "posts.created_at desc", "title" });

            Assert.AreEqual("[posts].[created_at] desc,[title]", result);
        }

        // get order by

        [TestMethod]
        public void GetOrderBy()
        {
            var service = this.GetService();

            var result = service.GetOrderBy(new string[] { "posts.created_at desc", "title" });

            Assert.AreEqual(" order by [posts].[created_at] desc,[title]", result);
        }

        [TestMethod]
        public void GetOrderBy_WithNull_ReturnsEmptyString()
        {
            var service = this.GetService();

            var result = service.GetOrderBy(null);

            Assert.AreEqual("", result);
        }

        [TestMethod]
        public void GetOrderBy_WithNoSorts_ReturnsEmptyString()
        {
            var service = this.GetService();

            var result = service.GetOrderBy(new string[] { });

            Assert.AreEqual("", result);
        }

        // where condition

        [TestMethod]
        public void ConditionOp()
        {
            var service = this.GetService();

            Assert.AreEqual("[id]=@id", service.GetConditionOp(Check.Op("id", 10)));
            Assert.AreEqual("[id]>@id", service.GetConditionOp(Check.Op("id", ">", 10)));
        }

        [TestMethod]
        public void ConditionLike()
        {
            var service = this.GetService();

            Assert.AreEqual("[id] like 'abc%'", service.GetLike(Check.Like("id", "abc%")));
            Assert.AreEqual("lower([id]) like 'abc%'", service.GetLike(Check.Like("id", "abc%", true)));
        }

        [TestMethod]
        public void ConditionBetween()
        {
            var service = this.GetService();

            Assert.AreEqual("[id] between 10 and 20", service.GetBetween(Check.Between("id", 10, 20)));
        }

        [TestMethod]
        public void ConditionNull()
        {
            var service = this.GetService();

            Assert.AreEqual("[id] is null", service.GetNull(Check.IsNull("id")));
            Assert.AreEqual("[id] is not null", service.GetNull(Check.IsNotNull("id")));
        }

        // get where

        [TestMethod]
        public void GetWhere()
        {
            var service = this.GetService();
            var condition = Check.Op("a", "<", 10).And(Check.Op("b", ">", 20)).Or(Check.Between("c", 60, 100));
            var result = service.GetWhere(condition);
            Assert.AreEqual(" where [a]<@a and [b]>@b or [c] between 60 and 100", result);
        }

        [TestMethod]
        public void GetWhere_ReturnsUniques()
        {
            var service = this.GetService();
            var condition = Check.Op("a", "<", 10)
                .Or(Check.Op("a", ">", 20))
                .Or(Check.Op("a", 30))
                .Or(Check.Op("a", 40));
            var result = service.GetWhere(condition);
            Assert.AreEqual(" where [a]<@a or [a]>@a2 or [a]=@a3 or [a]=@a4", result);
        }

        [TestMethod]
        public void GetWhere_ReturnsUniques2()
        {
            var service = this.GetService();
            var condition = Check.Op("b", "<", 10)
                .Or(Check.Op("a", ">", 20))
                .Or(Check.Op("b", 30))
                .Or(Check.Op("a", 40));
            var result = service.GetWhere(condition);
            Assert.AreEqual(" where [b]<@b or [a]>@a or [b]=@b2 or [a]=@a2", result);
        }

        [TestMethod]
        public void GetWhere_ReturnsUniques3()
        {
            var service = this.GetService();
            var condition = Check.Op("a", "<", 10)
                .Or(Check.Op("a", ">", 20))
                .Or(Check.Op("b", 30))
                .Or(Check.Op("b", 40));
            var result = service.GetWhere(condition);
            Assert.AreEqual(" where [a]<@a or [a]>@a2 or [b]=@b or [b]=@b2", result);
        }


        [TestMethod]
        public void GetWhere_WithNull_ReturnsEmptyString()
        {
            var service = this.GetService();
            var result = service.GetWhere(null);
            Assert.AreEqual("", result);
        }


        [TestMethod]
        public void GetParameterName()
        {
            var service = this.GetService();
            Assert.AreEqual("@id", service.GetParameterName("Id"));
            Assert.AreEqual("@role_id", service.GetParameterName("Role Id"));
            Assert.AreEqual("@id", service.GetParameterName("[test].[Id]"));
        }
    }
}
