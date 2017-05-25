using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EasyDbLib;

namespace EasyDbLibTest.Query
{
    [TestClass]
    public class ConditionAndParameterContainerTest
    {
        [TestMethod]
        public void TestCreateMain_WithOp()
        {
            var condition = Check.Op("id", 1);

            var container = new ConditionAndParameterContainer(condition);

            Assert.AreEqual("id", container.Main.Column);
            Assert.AreEqual(true, container.Main.IsConditionOp);
            Assert.AreEqual("@id", container.Main.ParameterName);
            Assert.AreEqual("=@id", container.Main.ValueString);
            Assert.AreEqual(1, container.Main.ParameterValue);
        }

        [TestMethod]
        public void TestCreateMain_WithLike()
        {
            var condition = Check.Like("name", "%a");

            var container = new ConditionAndParameterContainer(condition);

            Assert.AreEqual("name", container.Main.Column);
            Assert.AreEqual(false, container.Main.IsConditionOp);
            Assert.AreEqual("@name", container.Main.ParameterName);
            Assert.AreEqual(" like '%a'", container.Main.ValueString);
            Assert.AreEqual("%a", container.Main.ParameterValue);
        }

        [TestMethod]
        public void TestCreateMain_WithBetween()
        {
            var condition = Check.Between("range", 5,10);

            var container = new ConditionAndParameterContainer(condition);

            Assert.AreEqual("range", container.Main.Column);
            Assert.AreEqual(false, container.Main.IsConditionOp);
            Assert.AreEqual("@range", container.Main.ParameterName);
            Assert.AreEqual(" between 5 and 10", container.Main.ValueString);
            Assert.AreEqual(null, container.Main.ParameterValue);
        }

        [TestMethod]
        public void TestCreateMain_WithIsNull()
        {
            var condition = Check.IsNull("user_id");

            var container = new ConditionAndParameterContainer(condition);

            Assert.AreEqual("user_id", container.Main.Column);
            Assert.AreEqual(false, container.Main.IsConditionOp);
            Assert.AreEqual("@user_id", container.Main.ParameterName);
            Assert.AreEqual(" is null", container.Main.ValueString);
            Assert.AreEqual(null, container.Main.ParameterValue);
        }

        [TestMethod]
        public void TestCreateMain_WithIsNotNull()
        {
            var condition = Check.IsNotNull("user_id");

            var container = new ConditionAndParameterContainer(condition);

            Assert.AreEqual("user_id", container.Main.Column);
            Assert.AreEqual(false, container.Main.IsConditionOp);
            Assert.AreEqual("@user_id", container.Main.ParameterName);
            Assert.AreEqual(" is not null", container.Main.ValueString);
            Assert.AreEqual(null, container.Main.ParameterValue);
        }

        // subs

        [TestMethod]
        public void TestCreateSub()
        {
            var condition = Check.Op("id", 1).And(Check.Op("id",2));

            var container = new ConditionAndParameterContainer(condition);

            Assert.AreEqual("id", container.Main.Column);
            Assert.AreEqual(true, container.Main.IsConditionOp);
            Assert.AreEqual("@id", container.Main.ParameterName);
            Assert.AreEqual("=@id", container.Main.ValueString);
            Assert.AreEqual(1, container.Main.ParameterValue);

            var result = container.SubConditions[0];

            Assert.AreEqual("id", result.Column);
            Assert.AreEqual(true, result.IsConditionOp);
            Assert.AreEqual("@id2", result.ParameterName);
            Assert.AreEqual("=@id2", result.ValueString);
            Assert.AreEqual(2, result.ParameterValue);
            Assert.AreEqual("and", result.Op);
        }

        [TestMethod]
        public void TestCreateSub_WithOr()
        {
            var condition = Check.Op("id", 1).Or(Check.Op("id", 2));

            var container = new ConditionAndParameterContainer(condition);

            Assert.AreEqual("id", container.Main.Column);
            Assert.AreEqual(true, container.Main.IsConditionOp);
            Assert.AreEqual("@id", container.Main.ParameterName);
            Assert.AreEqual("=@id", container.Main.ValueString);
            Assert.AreEqual(1, container.Main.ParameterValue);

            var result = container.SubConditions[0];

            Assert.AreEqual("id", result.Column);
            Assert.AreEqual(true, result.IsConditionOp);
            Assert.AreEqual("@id2", result.ParameterName);
            Assert.AreEqual("=@id2", result.ValueString);
            Assert.AreEqual(2, result.ParameterValue);
            Assert.AreEqual("or", result.Op);
        }

        [TestMethod]
        public void TestCreateSub_Multiples()
        {
            var condition = Check.Op("id", 1).Or(Check.Op("id", 2)).Or(Check.Op("id", 3));

            var container = new ConditionAndParameterContainer(condition);

            Assert.AreEqual("id", container.Main.Column);
            Assert.AreEqual(true, container.Main.IsConditionOp);
            Assert.AreEqual("@id", container.Main.ParameterName);
            Assert.AreEqual("=@id", container.Main.ValueString);
            Assert.AreEqual(1, container.Main.ParameterValue);

            var result = container.SubConditions[0];

            Assert.AreEqual("id", result.Column);
            Assert.AreEqual(true, result.IsConditionOp);
            Assert.AreEqual("@id2", result.ParameterName);
            Assert.AreEqual("=@id2", result.ValueString);
            Assert.AreEqual(2, result.ParameterValue);
            Assert.AreEqual("or", result.Op);

            var result2 = container.SubConditions[1];

            Assert.AreEqual("id", result2.Column);
            Assert.AreEqual(true, result2.IsConditionOp);
            Assert.AreEqual("@id3", result2.ParameterName);
            Assert.AreEqual("=@id3", result2.ValueString);
            Assert.AreEqual(3, result2.ParameterValue);
            Assert.AreEqual("or", result2.Op);
        }

    }
}
