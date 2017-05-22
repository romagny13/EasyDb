using System;
using EasyDbLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EasyDbLibTest
{
    [TestClass]
    public class MappingTest
    {
        [TestMethod]
        public void TestRegisterMapping()
        {
            var mapping = new Mapping();
            mapping.Add("id", "Id")
                .Add("first", "FirstName");

            Assert.IsTrue(mapping.Has("id"));
            Assert.IsTrue(mapping.Has("first"));
        }

        [TestMethod]
        public void TestGetValues()
        {
            var mapping = new Mapping();
            mapping.Add("id", "Id")
                .Add("first", "FirstName");

            var idMapping = mapping.Get("id");
            var firstNameMapping = mapping.Get("first");

            Assert.AreEqual("id", idMapping.ColumnName);
            Assert.AreEqual("Id", idMapping.PropertyName);

            Assert.AreEqual("first", firstNameMapping.ColumnName);
            Assert.AreEqual("FirstName", firstNameMapping.PropertyName);
        }

        [TestMethod]
        public void TestMappingWithMappingContainer()
        {
            MappingContainer.Default
                .Add("products", Mapping.Create().Add("id", "Id").Add("name", "productName"))
                .Add("users", Mapping.Create().Add("id", "Id").Add("first", "FirstName"));

            Assert.IsTrue(MappingContainer.Default.Has("products"));
            Assert.IsTrue(MappingContainer.Default.Has("users"));
        }

        [TestMethod]
        public void TestGetMapping_WithContainer()
        {
            MappingContainer.Default
                .Add("products", Mapping.Create().Add("id", "Id").Add("name", "productName"))
                .Add("users", Mapping.Create().Add("id", "Id").Add("first", "FirstName"));

            var mapping = MappingContainer.Default.Get("users");

            Assert.IsTrue(mapping.Has("id"));
            Assert.IsTrue(mapping.Has("first"));

            var idMapping = mapping.Get("id");
            var firstNameMapping = mapping.Get("first");

            Assert.AreEqual("id", idMapping.ColumnName);
            Assert.AreEqual("Id", idMapping.PropertyName);

            Assert.AreEqual("first", firstNameMapping.ColumnName);
            Assert.AreEqual("FirstName", firstNameMapping.PropertyName);
        }

        //[TestMethod]
        //public void TestNull()
        //{
        //    var user = new P
        //    {
        //        first = "Marie",
        //        last = "Bellin"
        //    };

        //    Assert.IsTrue(IsNullable(user.age.GetType()));
        //}


        //public bool IsNullable(Type type)
        //{
        //    return Nullable.GetUnderlyingType(type) != null;
        //}
    }

    public class UserMapping
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
    }
}
