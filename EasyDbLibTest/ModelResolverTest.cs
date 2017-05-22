using System;
using EasyDbLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace EasyDbLibTest
{
    [TestClass]
    public class ModelResolverTest
    {
        [TestMethod]
        public void TestCreateModelInstance()
        {
            var resolver = new ModelResolver();
            var user = resolver.CreateModelInstance(typeof(User));

            Assert.IsNotNull(user);
            Assert.AreEqual(typeof(User), user.GetType());
        }

        [TestMethod]
        public void TestCreateModelInstance_NoDefaultCtor_Fail()
        {
            bool failed = false;
            var resolver = new ModelResolver();
            try
            {
                var user = resolver.CreateModelInstance(typeof(UserWithNoDefaultCtor));
            }
            catch (Exception e)
            {
                failed = true;
            }
            Assert.IsTrue(failed);
        }

        // resolve property type

        [TestMethod]
        public void TestResolvePropertyType_ReturnsPropertyType()
        {
            var resolver = new ModelResolver();
            var propertyInfo = resolver.GetPropertyInfo(typeof(MyModel), "MyInt");

            var result = resolver.ResolvePropertyType(propertyInfo);

            Assert.AreEqual(typeof(int), result);
        }

        [TestMethod]
        public void TestResolvePropertyType_ReturnsTargetTypeOfNullable()
        {
            var resolver = new ModelResolver();
            var propertyInfo = resolver.GetPropertyInfo(typeof(MyModelWithNullable), "MyNullable");

            var result = resolver.ResolvePropertyType(propertyInfo);

            Assert.AreEqual(typeof(int), result);
        }


        // get converted value

        [TestMethod]
        public void TestGetConvertedValue()
        {
            var resolver = new ModelResolver();
            int myInt = 10;
            var result = resolver.GetConvertedValue(myInt, typeof(string));

            Assert.AreEqual(typeof(string), result.GetType());
            Assert.AreEqual("10", result);
        }

        [TestMethod]
        public void TestGetConvertedValue_WithNull_ReturnsNull()
        {
            var resolver = new ModelResolver();

            var result = resolver.GetConvertedValue(null, typeof(string));

            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public void TestGetConvertedValue_ConvertStringToInt()
        {
            var resolver = new ModelResolver();
            string myString = "10";
            var result = resolver.GetConvertedValue(myString, typeof(int));

            Assert.AreEqual(typeof(int), result.GetType());
            Assert.AreEqual(10, result);
        }

        // trim end string

        [TestMethod]
        public void TestCanTrimValue()
        {
            var resolver = new ModelResolver();

            Assert.IsTrue(resolver.CanTrimValue(typeof(string), "my value        "));
        }

        [TestMethod]
        public void TestCanTrimValue_WithStringNullReturnsFalse()
        {
            var resolver = new ModelResolver();

            Assert.IsFalse(resolver.CanTrimValue(typeof(string), null));
        }

        [TestMethod]
        public void TestCanTrimValue_NotString_ReturnsFalse()
        {
            var resolver = new ModelResolver();

            Assert.IsFalse(resolver.CanTrimValue(typeof(int), 10));
        }

        [TestMethod]
        public void TestTrimEndString()
        {
            var resolver = new ModelResolver();

            var myString = "my value             ";
            var result = resolver.TrimEndString(myString);

            Assert.AreEqual("my value", result);
        }


        // set value

        [TestMethod]
        public void TestSetValue()
        {
            var resolver = new ModelResolver();

            var model = (MyModel) resolver.CreateModelInstance(typeof(MyModel));
            var propertyInfo = resolver.GetPropertyInfo(model.GetType(), "MyInt");

            resolver.SetValue(model, propertyInfo, 10);

            Assert.AreEqual(10, model.MyInt);
        }

        [TestMethod]
        public void TestSetValue_WithStringNull()
        {
            var resolver = new ModelResolver();

            var model = (MyModel) resolver.CreateModelInstance(typeof(MyModel));
            var propertyInfo = resolver.GetPropertyInfo(model.GetType(), "MyString");

            resolver.SetValue(model, propertyInfo, null);

            Assert.AreEqual(null, model.MyString);
        }

        [TestMethod]
        public void TestSetValue_WithNullable()
        {
            var resolver = new ModelResolver();

            var model = (MyModelWithNullable) resolver.CreateModelInstance(typeof(MyModelWithNullable));
            var propertyInfo = resolver.GetPropertyInfo(model.GetType(), "MyNullable");

            resolver.SetValue(model, propertyInfo, null);

            Assert.IsFalse(model.MyNullable.HasValue);
            Assert.AreEqual(null, model.MyNullable);
        }

        [TestMethod]
        public void TestSetValue_WithNullableAndValue()
        {
            var resolver = new ModelResolver();

            var model = (MyModelWithNullable) resolver.CreateModelInstance(typeof(MyModelWithNullable));
            var propertyInfo = resolver.GetPropertyInfo(model.GetType(), "MyNullable");

            resolver.SetValue(model, propertyInfo, 10);

            Assert.IsTrue(model.MyNullable.HasValue);
            Assert.AreEqual(10, model.MyNullable);
        }

        // Check and set value

        [TestMethod]
        public void TestCheckAndSetValue()
        {
            var resolver = new ModelResolver();

            var model = (MyModel) resolver.CreateModelInstance(typeof(MyModel));
            var propertyInfo = resolver.GetPropertyInfo(model.GetType(), "MyInt");

            resolver.CheckAndSetValue(model, "MyInt", propertyInfo, 10);

            Assert.AreEqual(10, model.MyInt);
        }

        [TestMethod]
        public void TestCheckAndSetValue_Convert()
        {
            var resolver = new ModelResolver();

            var model = (MyModel) resolver.CreateModelInstance(typeof(MyModel));
            var propertyInfo = resolver.GetPropertyInfo(model.GetType(), "MyIntAsString");

            resolver.CheckAndSetValue(model, "MyIntAsString", propertyInfo, 10);

            Assert.AreEqual("10", model.MyIntAsString);
        }


        // resolve property

        [TestMethod]
        public void TestResolveProperty()
        {
            var resolver = new ModelResolver();

            var model = (MyModel) resolver.CreateModelInstance(typeof(MyModel));

            resolver.ResolveProperty(model, "MyInt", 10);

            Assert.AreEqual(10, model.MyInt);
        }

        [TestMethod]
        public void TestResolveProperty_ConvertColmunValueToPropertyType()
        {
            var resolver = new ModelResolver();

            var model = (MyModel) resolver.CreateModelInstance(typeof(MyModel));

            resolver.ResolveProperty(model, "MyInt", "10");

            Assert.AreEqual(10, model.MyInt);
        }


        // mapping

        [TestMethod]
        public void TestResolveProperty_WithMapping()
        {
            var resolver = new ModelResolver();
            var mapping = new Mapping();
            mapping.Add("id", "Id")
                .Add("first", "FirstName");

            var model = (MyModelMapped) resolver.CreateModelInstance(typeof(MyModelMapped));


            resolver.ResolveProperty(model, "first", "Marie", mapping);

            Assert.AreEqual("Marie", model.FirstName);
        }

        [TestMethod]
        public void TestResolveProperty_WithMappingConvertStringToInt()
        {
            var resolver = new ModelResolver();
            var mapping = new Mapping();
            mapping.Add("id", "Id")
                .Add("first", "FirstName");

            var model = (MyModelMapped) resolver.CreateModelInstance(typeof(MyModelMapped));


            resolver.ResolveProperty(model, "id", "10", mapping);

            Assert.AreEqual(10, model.Id);
        }

        [TestMethod]
        public void TestResolveProperty_WithMappingConvertIntToString()
        {
            var resolver = new ModelResolver();
            var mapping = new Mapping();
            mapping.Add("id", "Id")
                .Add("first", "FirstName");

            var model = (MyModelMapped) resolver.CreateModelInstance(typeof(MyModelMapped));


            resolver.ResolveProperty(model, "first", 10, mapping);

            Assert.AreEqual("10", model.FirstName);
        }

        [TestMethod]
        public void TestColumnIsIgnored()
        {
            var resolver = new ModelResolver();
            var mapping = new Mapping();
            mapping.Add("id", "Id")
                .Add("first", "FirstName", true);

            var model = (MyModelMapped)resolver.CreateModelInstance(typeof(MyModelMapped));


            resolver.ResolveProperty(model, "first", "Marie", mapping);

            Assert.AreEqual(null, model.FirstName);
        }

        [TestMethod]
        public void TestColumnIsIgnoredWIthInt()
        {
            var resolver = new ModelResolver();
            var mapping = new Mapping();
            mapping.Add("id", "Id", true)
                .Add("first", "FirstName");

            var model = (MyModelMapped)resolver.CreateModelInstance(typeof(MyModelMapped));

            resolver.ResolveProperty(model, "id", 10, mapping);

            Assert.AreEqual(default(int), model.Id);
        }

        [TestMethod]
        public void TestColumnDontIgnoredWOtherColumns()
        {
            var resolver = new ModelResolver();
            var mapping = new Mapping();
            mapping.Add("id", "Id", true)
                .Add("first", "FirstName");

            var model = (MyModelMapped)resolver.CreateModelInstance(typeof(MyModelMapped));

            resolver.ResolveProperty(model, "id", 10, mapping);

            Assert.AreEqual(default(int), model.Id);

            resolver.ResolveProperty(model, "first", "Marie", mapping);

            Assert.AreEqual("Marie", model.FirstName);

        }

        [TestMethod]
        public void TestColumnCouldChangeIgnored()
        {
            var resolver = new ModelResolver();
            var mapping = new Mapping();
            mapping.Add("id", "Id")
                .Add("first", "FirstName", true);

            var model = (MyModelMapped)resolver.CreateModelInstance(typeof(MyModelMapped));

            resolver.ResolveProperty(model, "first", "Marie", mapping);

            Assert.AreEqual(null, model.FirstName);

            mapping.Get("first").Ignore = false;

            resolver.ResolveProperty(model, "first", "Marie", mapping);

            Assert.AreEqual("Marie", model.FirstName);
        }

        // resolve model

        [TestMethod]
        public void TestResolve()
        {
            var resolver = new ModelResolver();

            var data = new Dictionary<string, FakeContainerItem>
            {
                ["id"] = new FakeContainerItem {IsDBNull = false, Name = "id", Value = 1},
                ["first"] = new FakeContainerItem {IsDBNull = false, Name = "first", Value = "Marie"}
            };
            var reader = new MyReaderContainer(data);

            var result = (FakeModel) resolver.Resolve(typeof(FakeModel), reader);

            Assert.AreEqual(1, result.id);
            Assert.AreEqual("Marie", result.first);
        }

        [TestMethod]
        public void TestResolveConvertIntToString()
        {
            var resolver = new ModelResolver();

            var data = new Dictionary<string, FakeContainerItem>
            {
                ["id"] = new FakeContainerItem { IsDBNull = false, Name = "id", Value = 1 },
                ["first"] = new FakeContainerItem { IsDBNull = false, Name = "first", Value = 10 }
            };
            var reader = new MyReaderContainer(data);

            var result = (FakeModel)resolver.Resolve(typeof(FakeModel), reader);

            Assert.AreEqual("10", result.first);
        }

        [TestMethod]
        public void TestResolveConvertStringToInt()
        {
            var resolver = new ModelResolver();

            var data = new Dictionary<string, FakeContainerItem>
            {
                ["id"] = new FakeContainerItem { IsDBNull = false, Name = "id", Value = "10" },
                ["first"] = new FakeContainerItem { IsDBNull = false, Name = "first", Value ="Marie" }
            };
            var reader = new MyReaderContainer(data);

            var result = (FakeModel)resolver.Resolve(typeof(FakeModel), reader);

            Assert.AreEqual(10, result.id);
        }

        [TestMethod]
        public void TestResolve_WithMapping()
        {
            var resolver = new ModelResolver();

            var data = new Dictionary<string, FakeContainerItem>
            {
                ["id"] = new FakeContainerItem { IsDBNull = false, Name = "id", Value = 1 },
                ["first"] = new FakeContainerItem { IsDBNull = false, Name = "first", Value = "Marie" }
            };
            var reader = new MyReaderContainer(data);

            var mapping = new Mapping();
            mapping.Add("id", "Id")
                .Add("first", "FirstName");

            var result = (FakeModelMapped)resolver.Resolve(typeof(FakeModelMapped), reader,mapping);

            Assert.AreEqual(1, result.Id);
            Assert.AreEqual("Marie", result.FirstName);
        }
    }

    public class FakeModelMapped
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
    }

    public class FakeModel
    {
        public int id { get; set; }
        public string first { get; set; }
    }

    public class FakeContainerItem
    {
        public string Name { get; set; }
        public object Value { get; set; }
        public bool IsDBNull { get; set; }
    }

    public class MyReaderContainer :IReaderContainer
    {
        public Dictionary<string, FakeContainerItem> data;

        public MyReaderContainer(Dictionary<string,FakeContainerItem> data)
        {
            this.data = data;
        }

        public int FieldCount => this.data.Count;

        public string GetName(int index)
        {
            return this.data.Keys.ElementAt(index);
        }

        public object GetValue(int index)
        {
            return this.data.Values.ElementAt(index).Value;
        }

        public bool IsDBNull(int index)
        {
            return this.data.Values.ElementAt(index).IsDBNull;
        }
    }

    public class MyModelMapped
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
    }

    public class MyModel
    {
        public int MyInt { get; set; }
        public string MyString { get; set; }
        public string MyIntAsString { get; set; }
    }

    public class MyModelWithNullable
    {
        public int? MyNullable { get; set; }
    }

    public class UserWithNoDefaultCtor
    {
        public UserWithNoDefaultCtor(string name)
        {
                
        }
    }

}
