using System;
using System.Linq;
using EasyDbLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EasyDbLibTest
{
    [TestClass]
    public class ConditionTest
    {
       QueryBuilderService service = new QueryBuilderService();

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

        [TestMethod]
        public void TestConditionOp()
        {
            var condition = Condition.Op("id", 10);
            var result = service.GetConditionString(condition);
            Assert.AreEqual("[id]=@id", result);
        }

        [TestMethod]
        public void TestConditionOp_WithTableAndColumn()
        {
            var condition = Condition.Op("posts.id", 10);
            var result = service.GetConditionString(condition);
            Assert.AreEqual("[posts].[id]=@id", result);
        }

        [TestMethod]
        public void TestConditionOp_WithSpaceInColumnName()
        {
            var condition = Condition.Op("my id", 10);
            var result = service.GetConditionString(condition);
            Assert.AreEqual("[my id]=@my_id", result);
        }

        [TestMethod]
        public void TestConditionBetween()
        {
            var condition = Condition.Between("value",1,10);
            var result = service.GetConditionString(condition);
            Assert.AreEqual("[value] between 1 and 10", result);
        }

        [TestMethod]
        public void TestConditionLike()
        {
            var condition = Condition.Like("value", "%a");
            var result = service.GetConditionString(condition);
            Assert.AreEqual("[value] like '%a'", result);
        }

        [TestMethod]
        public void TestOrderedCondition_WithOperator()
        {
            var condition = Condition.Op("a", "<", 10).And(Condition.Op("b", ">", 20));
            var result = service.GetConditionAndOrderedConditionString(condition);
            Assert.AreEqual("[a]<@a and [b]>@b", result);
        }

        [TestMethod]
        public void TestOrderedCondition_WithMultipleConditions()
        {
           var condition = Condition.Op("a", "<", 10).And(Condition.Op("b", ">", 20)).Or(Condition.Between("c", 60, 100));
            var result = service.GetConditionAndOrderedConditionString(condition);
            Assert.AreEqual("[a]<@a and [b]>@b or [c] between 60 and 100", result);
        }
    }
}
