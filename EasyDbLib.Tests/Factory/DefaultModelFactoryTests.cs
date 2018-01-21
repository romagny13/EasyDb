using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using EasyDbLib.Tests.Factory;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EasyDbLib.Tests
{
    [TestClass]
    public class DefaultModelFactoryTests
    {
        public DefaultModelFactory GetService()
        {
            return new DefaultModelFactory();
        }

        [TestMethod]
        public void CreateModelInstance()
        {
            var service = this.GetService();
            var user = service.CreateModelInstance(typeof(User));

            Assert.IsNotNull(user);
            Assert.AreEqual(typeof(User), user.GetType());
        }

        [TestMethod]
        public void CreateModelInstance_NoDefaultCtor_Fail()
        {
            bool failed = false;
            var service = this.GetService();
            try
            {
                var user = service.CreateModelInstance(typeof(UserWithNoDefaultCtor));
            }
            catch (Exception e)
            {
                failed = true;
            }
            Assert.IsTrue(failed);
        }

        // resolve property type

        [TestMethod]
        public void ResolvePropertyType_ReturnsPropertyType()
        {
            var service = this.GetService();
            var property = service.GetProperty(typeof(MyModel), "MyInt", false);

            var result = service.ResolvePropertyType(property);

            Assert.AreEqual(typeof(int), result);
        }

        [TestMethod]
        public void ResolvePropertyType_AddToCache()
        {
            var service = this.GetService();

            Assert.IsFalse(service.IsInCache(typeof(MyModel), "MyInt"));

            var property = service.GetProperty(typeof(MyModel), "MyInt", false);

            var result = service.ResolvePropertyType(property);

            Assert.IsTrue(service.IsInCache(typeof(MyModel), "MyInt"));
        }

        [TestMethod]
        public void ResolvePropertyType_WithIgnoreCase_ReturnsPropertyType()
        {
            var service = this.GetService();
            var property = service.GetProperty(typeof(MyModel), "myint", true);

            var result = service.ResolvePropertyType(property);

            Assert.AreEqual(typeof(int), result);
        }

        [TestMethod]
        public void ResolvePropertyType_ReturnsTargetTypeOfNullable()
        {
            var service = this.GetService();
            var property = service.GetProperty(typeof(MyModelWithNullable), "MyNullable", false);

            var result = service.ResolvePropertyType(property);

            Assert.AreEqual(typeof(int), result);
        }


        // get converted value

        [TestMethod]
        public void GetConvertedValue()
        {
            var service = this.GetService();
            int myInt = 10;
            var result = service.GetConvertedValue(myInt, typeof(string));

            Assert.AreEqual(typeof(string), result.GetType());
            Assert.AreEqual("10", result);
        }

        [TestMethod]
        public void GetConvertedValue_WithNull_ReturnsNull()
        {
            var service = this.GetService();

            var result = service.GetConvertedValue(null, typeof(string));

            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public void GetConvertedValue_ConvertStringToInt()
        {
            var service = this.GetService();
            string myString = "10";
            var result = service.GetConvertedValue(myString, typeof(int));

            Assert.AreEqual(typeof(int), result.GetType());
            Assert.AreEqual(10, result);
        }

        // trim end string

        [TestMethod]
        public void CanTrimValue()
        {
            var service = this.GetService();

            Assert.IsTrue(service.CanTrimValue(typeof(string), "my value        "));
        }

        [TestMethod]
        public void CanTrimValue_WithStringNullReturnsFalse()
        {
            var service = this.GetService();

            Assert.IsFalse(service.CanTrimValue(typeof(string), null));
        }

        [TestMethod]
        public void CanTrimValue_NotString_ReturnsFalse()
        {
            var service = this.GetService();

            Assert.IsFalse(service.CanTrimValue(typeof(int), 10));
        }

        [TestMethod]
        public void TrimEndString()
        {
            var service = this.GetService();

            var myString = "my value             ";
            var result = service.TrimEndString(myString);

            Assert.AreEqual("my value", result);
        }


        // set value

        [TestMethod]
        public void SetValue()
        {
            var service = this.GetService();

            var model = (MyModel)service.CreateModelInstance(typeof(MyModel));
            var property = service.GetProperty(model.GetType(), "MyInt", false);

            service.SetValue(model, property, 10);

            Assert.AreEqual(10, model.MyInt);
        }

        [TestMethod]
        public void SetValue_WithStringNull()
        {
            var service = this.GetService();

            var model = (MyModel)service.CreateModelInstance(typeof(MyModel));
            var property = service.GetProperty(model.GetType(), "MyString", false);

            service.SetValue(model, property, null);

            Assert.AreEqual(null, model.MyString);
        }

        [TestMethod]
        public void SetValue_WithNullable()
        {
            var service = this.GetService();

            var model = (MyModelWithNullable)service.CreateModelInstance(typeof(MyModelWithNullable));
            var property = service.GetProperty(model.GetType(), "MyNullable", false);

            service.SetValue(model, property, null);

            Assert.IsFalse(model.MyNullable.HasValue);
            Assert.AreEqual(null, model.MyNullable);
        }

        [TestMethod]
        public void SetValue_WithNullableAndValue()
        {
            var service = this.GetService();

            var model = (MyModelWithNullable)service.CreateModelInstance(typeof(MyModelWithNullable));
            var property = service.GetProperty(model.GetType(), "MyNullable", false);

            service.SetValue(model, property, 10);

            Assert.IsTrue(model.MyNullable.HasValue);
            Assert.AreEqual(10, model.MyNullable);
        }

        // Check and set value

        [TestMethod]
        public void TryConvertAndSetValue()
        {
            var service = this.GetService();

            var model = (MyModel)service.CreateModelInstance(typeof(MyModel));
            var property = service.GetProperty(model.GetType(), "MyInt", false);

            service.TryConvertAndSetValue(model, "MyInt", property, 10);

            Assert.AreEqual(10, model.MyInt);
        }

        [TestMethod]
        public void TryConvertAndSetValue_Convert()
        {
            var service = this.GetService();

            var model = (MyModel)service.CreateModelInstance(typeof(MyModel));
            var property = service.GetProperty(model.GetType(), "MyIntAsString", false);

            service.TryConvertAndSetValue(model, "MyIntAsString", property, 10);

            Assert.AreEqual("10", model.MyIntAsString);
        }

        // resolve property

        [TestMethod]
        public void ResolveProperty()
        {
            var service = this.GetService();

            var model = (MyModel)service.CreateModelInstance(typeof(MyModel));

            service.ResolveProperty(model, "MyInt", 10);

            Assert.AreEqual(10, model.MyInt);
        }

        [TestMethod]
        public void ResolveProperty_ConvertColmunValueToPropertyType()
        {
            var service = this.GetService();

            var model = (MyModel)service.CreateModelInstance(typeof(MyModel));

            service.ResolveProperty(model, "MyInt", "10");

            Assert.AreEqual(10, model.MyInt);
        }


        // mapping

        [TestMethod]
        public void ResolveProperty_WithMapping()
        {
            var service = this.GetService();

            var table = new Table<MyModelMapped>("users")
                .SetColumn("id", p => p.Id)
                .SetColumn("first", p => p.FirstName);

            var model = (MyModelMapped)service.CreateModelInstance(typeof(MyModelMapped));

            service.ResolveProperty(model, "first", "Marie", table);

            Assert.AreEqual("Marie", model.FirstName);
        }

        [TestMethod]
        public void ResolveProperty_WithMapping_ConvertStringToInt()
        {
            var service = this.GetService();

            var table = new Table<MyModelMapped>("users")
                         .SetColumn("id", p => p.Id)
                         .SetColumn("first", p => p.FirstName);

            var model = (MyModelMapped)service.CreateModelInstance(typeof(MyModelMapped));


            service.ResolveProperty(model, "id", "10", table);

            Assert.AreEqual(10, model.Id);
        }

        [TestMethod]
        public void ResolveProperty_WithMapping_ConvertIntToString()
        {
            var service = this.GetService();

            var table = new Table<MyModelMapped>("users")
               .SetColumn("id", p => p.Id)
               .SetColumn("first", p => p.FirstName);

            var model = (MyModelMapped)service.CreateModelInstance(typeof(MyModelMapped));


            service.ResolveProperty(model, "first", 10, table);

            Assert.AreEqual("10", model.FirstName);
        }

        [TestMethod]
        public void ResolveProperty_WithColumnIsIgnored()
        {
            var service = this.GetService();

            var table = new Table<MyModelMapped>("users")
                .SetColumn("id", p => p.Id)
                .SetColumn("first", p => p.FirstName, false, true);

            var model = (MyModelMapped)service.CreateModelInstance(typeof(MyModelMapped));


            service.ResolveProperty(model, "first", "Marie", table);

            Assert.AreEqual(null, model.FirstName);
        }

        [TestMethod]
        public void ResolveProperty_IsIgnoredWIthInt()
        {
            var service = this.GetService();

            var table = new Table<MyModelMapped>("users")
                .SetColumn("id", p => p.Id, false, true)
                .SetColumn("first", p => p.FirstName);

            var model = (MyModelMapped)service.CreateModelInstance(typeof(MyModelMapped));

            service.ResolveProperty(model, "id", 10, table);

            Assert.AreEqual(default(int), model.Id);
        }

        [TestMethod]
        public void ResolveProperty_DontIgnoredOtherColumns()
        {
            var service = this.GetService();

            var table = new Table<MyModelMapped>("users")
                 .SetColumn("id", p => p.Id, false, true)
                 .SetColumn("first", p => p.FirstName);

            var model = (MyModelMapped)service.CreateModelInstance(typeof(MyModelMapped));

            service.ResolveProperty(model, "id", 10, table);

            Assert.AreEqual(default(int), model.Id);

            service.ResolveProperty(model, "first", "Marie", table);

            Assert.AreEqual("Marie", model.FirstName);

        }

        [TestMethod]
        public void ResolveProperty_ColumnCouldChangeIgnored()
        {
            var service = this.GetService();

            var table = new Table<MyModelMapped>("users")
                 .SetColumn("id", p => p.Id)
                 .SetColumn("first", p => p.FirstName, false, true);

            var model = (MyModelMapped)service.CreateModelInstance(typeof(MyModelMapped));

            service.ResolveProperty(model, "first", "Marie", table);

            Assert.AreEqual(null, model.FirstName);

            table.MappingByColumnName["first"].IsIgnored = false;

            service.ResolveProperty(model, "first", "Marie", table);

            Assert.AreEqual("Marie", model.FirstName);
        }

        // resolve model

        [TestMethod]
        public void CreateModel()
        {
            var service = this.GetService();

            var db = new EasyDb();

            var data = new Dictionary<string, FakeContainerItem>
            {
                ["id"] = new FakeContainerItem { IsDBNull = false, Name = "id", Value = 1 },
                ["first"] = new FakeContainerItem { IsDBNull = false, Name = "first", Value = "Marie" }
            };
            var reader = new MyReaderContainer(data);

            var result = (FakeModel)service.CreateModel<FakeModel>(reader, db);

            Assert.AreEqual(1, result.id);
            Assert.AreEqual("Marie", result.first);
        }

        [TestMethod]
        public void CreateModel_ConvertIntToString()
        {
            var service = this.GetService();

            var db = new EasyDb();

            var data = new Dictionary<string, FakeContainerItem>
            {
                ["id"] = new FakeContainerItem { IsDBNull = false, Name = "id", Value = 1 },
                ["first"] = new FakeContainerItem { IsDBNull = false, Name = "first", Value = 10 }
            };
            var reader = new MyReaderContainer(data);

            var result = (FakeModel)service.CreateModel<FakeModel>(reader, db);

            Assert.AreEqual("10", result.first);
        }

        [TestMethod]
        public void CreateModel_ConvertStringToInt()
        {
            var service = this.GetService();

            var db = new EasyDb();

            var data = new Dictionary<string, FakeContainerItem>
            {
                ["id"] = new FakeContainerItem { IsDBNull = false, Name = "id", Value = "10" },
                ["first"] = new FakeContainerItem { IsDBNull = false, Name = "first", Value = "Marie" }
            };
            var reader = new MyReaderContainer(data);

            var result = (FakeModel)service.CreateModel<FakeModel>(reader, db);

            Assert.AreEqual(10, result.id);
        }

        [TestMethod]
        public void CreateModel_WithMapping()
        {
            var service = this.GetService();

            var db = new EasyDb();

            db.SetTable<FakeModelMapped>("users")
                  .SetColumn("id", p => p.Id)
                  .SetColumn("first", p => p.FirstName);

            var data = new Dictionary<string, FakeContainerItem>
            {
                ["id"] = new FakeContainerItem { IsDBNull = false, Name = "id", Value = 1 },
                ["first"] = new FakeContainerItem { IsDBNull = false, Name = "first", Value = "Marie" }
            };
            var reader = new MyReaderContainer(data);

            var result = (FakeModelMapped)service.CreateModel<FakeModelMapped>(reader, db);

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

    public class FakeContainerItem
    {
        public string Name { get; set; }
        public object Value { get; set; }
        public bool IsDBNull { get; set; }
    }

    public class MyReaderContainer : IDataReader
    {
        public Dictionary<string, FakeContainerItem> dataByColumnName;

        public MyReaderContainer(Dictionary<string, FakeContainerItem> data)
        {
            this.dataByColumnName = data;
        }

        public object this[int i] => ((FakeContainerItem)dataByColumnName.ElementAt(i).Value).Value;

        public object this[string name] => ((FakeContainerItem)dataByColumnName[name].Value).Value;

        public int Depth => dataByColumnName.Count;

        public bool IsClosed => throw new NotImplementedException();

        public int RecordsAffected => throw new NotImplementedException();

        public int FieldCount => this.dataByColumnName.Count;

        public void Close()
        {

        }

        public void Dispose()
        {

        }

        public bool GetBoolean(int i)
        {
            return (bool)this.dataByColumnName.ElementAt(i).Value.Value;
        }

        public byte GetByte(int i)
        {
            return (byte)this.dataByColumnName.ElementAt(i).Value.Value;
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            return (long)this.dataByColumnName.ElementAt(i).Value.Value;
        }

        public char GetChar(int i)
        {
            return (char)this.dataByColumnName.ElementAt(i).Value.Value;
        }

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        public IDataReader GetData(int i)
        {
            throw new NotImplementedException();
        }

        public string GetDataTypeName(int i)
        {
            throw new NotImplementedException();
        }

        public DateTime GetDateTime(int i)
        {
            throw new NotImplementedException();
        }

        public decimal GetDecimal(int i)
        {
            throw new NotImplementedException();
        }

        public double GetDouble(int i)
        {
            return (double)this.dataByColumnName.ElementAt(i).Value.Value;
        }

        public Type GetFieldType(int i)
        {
            throw new NotImplementedException();
        }

        public float GetFloat(int i)
        {
            throw new NotImplementedException();
        }

        public Guid GetGuid(int i)
        {
            return (Guid)this.dataByColumnName.ElementAt(i).Value.Value;
        }

        public short GetInt16(int i)
        {
            return (short)this.dataByColumnName.ElementAt(i).Value.Value;
        }

        public int GetInt32(int i)
        {
            return (int)this.dataByColumnName.ElementAt(i).Value.Value;
        }

        public long GetInt64(int i)
        {
            return (long)this.dataByColumnName.ElementAt(i).Value.Value;
        }

        public string GetName(int i)
        {
            return this.dataByColumnName.Keys.ElementAt(i);
        }

        public int GetOrdinal(string name)
        {
            throw new NotImplementedException();
        }

        public DataTable GetSchemaTable()
        {
            throw new NotImplementedException();
        }

        public string GetString(int i)
        {
            return (string)this.dataByColumnName.ElementAt(i).Value.Value;
        }

        public object GetValue(int i)
        {
            return this.dataByColumnName.Values.ElementAt(i).Value;
        }

        public int GetValues(object[] values)
        {
            throw new NotImplementedException();
        }

        public bool IsDBNull(int i)
        {
            return this.dataByColumnName.Values.ElementAt(i).IsDBNull;
        }

        public bool NextResult()
        {
            throw new NotImplementedException();
        }

        public bool Read()
        {
            throw new NotImplementedException();
        }
    }
}
