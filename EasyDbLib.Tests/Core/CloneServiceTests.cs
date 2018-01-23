using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EasyDbLib.Tests.Core
{
    [TestClass]
    public class CloneServiceTests
    {
        [TestMethod]
        public void Clone_SimpleObject()
        {
            var service = new CloneService();

            var value = new SimpleObject
            {
                MyInt = 10,
                MyBool = true,
                MyString = "my value",
                MyChar = 'c',
                MyDateTime = new DateTime(2000, 12, 12),
                MyGuid = new Guid("2cb3f7e5-47fb-456f-a234-9d8909f69b2c")
            };

            var result = service.DeepClone(value);

            value.MyInt = 100;
            value.MyBool = false;
            value.MyString = "my new value";
            value.MyChar = 'e';
            value.MyDateTime = new DateTime(1900, 11, 11);
            value.MyGuid = Guid.NewGuid();

            Assert.AreEqual(10, result.MyInt);
            Assert.AreEqual(true, result.MyBool);
            Assert.AreEqual("my value", result.MyString);
            Assert.AreEqual('c', result.MyChar);
            Assert.AreEqual(new DateTime(2000, 12, 12), result.MyDateTime);
            Assert.AreEqual(new Guid("2cb3f7e5-47fb-456f-a234-9d8909f69b2c"), result.MyGuid);
        }

        [TestMethod]
        public void Clone_ComplexObject()
        {
            var service = new CloneService();
            var value = new ComplexObject
            {
                MyUser = new MyUser { Name = "Marie" },
                MyList = new List<string> { "a", "b" },
                MyArray = new string[] { "a", "b" },
                MyEnumValue = MyEnum.val2,
                MyCol = new ObservableCollection<MyUser>(new List<MyUser>
                {
                    new MyUser{ Name="Marie col"}
                }),
                MyDict = new Dictionary<string, string>
                {
                    {"key1","value 1" }
                }
            };

            var result = service.DeepClone(value);

            value.MyUser = new MyUser { Name = "Pat" };
            value.MyList.Add("c");
            value.MyArray[0] = "a!";
            value.MyEnumValue = MyEnum.val1;

            value.MyCol.Add(new MyUser { Name = "Pat col" });
            value.MyDict.Add("key3", "value 3");

            Assert.AreEqual("Marie", result.MyUser.Name);
            Assert.AreEqual(2, result.MyList.Count);
            Assert.AreEqual("a", result.MyArray[0]);
            Assert.AreEqual(MyEnum.val2, result.MyEnumValue);

            Assert.AreEqual(1, result.MyCol.Count);
            Assert.AreEqual(1, result.MyDict.Count);
        }

        [TestMethod]
        public void Clone_ObjectWithNullable()
        {
            var service = new CloneService();

            var value = new WithNullables
            {
                MyInt = 10,
                MyBool = true,
                MyChar = 'c',
                MyDateTime = new DateTime(2000, 12, 12),
                MyGuid = new Guid("2cb3f7e5-47fb-456f-a234-9d8909f69b2c")
            };

            var result = service.DeepClone(value);

            value.MyInt = 100;
            value.MyBool = false;
            value.MyChar = 'e';
            value.MyDateTime = new DateTime(1900, 11, 11);
            value.MyGuid = Guid.NewGuid();

            Assert.AreEqual(10, result.MyInt);
            Assert.AreEqual(true, result.MyBool);
            Assert.AreEqual('c', result.MyChar);
            Assert.AreEqual(new DateTime(2000, 12, 12), result.MyDateTime);
            Assert.AreEqual(new Guid("2cb3f7e5-47fb-456f-a234-9d8909f69b2c"), result.MyGuid);
        }

        [TestMethod]
        public void Clone_ObjectWithNullableNulls()
        {
            var service = new CloneService();

            var value = new WithNullables {};

            var result = service.DeepClone(value);

            value.MyInt = 100;
            value.MyBool = false;
            value.MyChar = 'e';
            value.MyDateTime = new DateTime(1900, 11, 11);
            value.MyGuid = Guid.NewGuid();

            Assert.AreEqual(null, result.MyInt);
            Assert.AreEqual(null, result.MyBool);
            Assert.AreEqual(null, result.MyChar);
            Assert.AreEqual(null, result.MyDateTime);
            Assert.AreEqual(null, result.MyGuid);
        }
    }


    public class SimpleObject 
    {
        public int MyInt { get; set; }
        public bool MyBool { get; set; }
        public string MyString { get; set; }
        public char MyChar { get; set; }
        public DateTime MyDateTime { get; set; }
        public Guid MyGuid { get; set; }
    }

    public class WithNullables 
    {
        public int? MyInt { get; set; }
        public bool? MyBool { get; set; }
        public char? MyChar { get; set; }
        public DateTime? MyDateTime { get; set; }
        public Guid? MyGuid { get; set; }
    }

    public class MyUser
    {
        public string Name { get; set; }
    }

    public class ComplexObject 
    {
        public MyUser MyUser { get; set; }
        public List<string> MyList { get; set; }
        public string[] MyArray { get; set; }
        public MyEnum MyEnumValue { get; set; }
        public ObservableCollection<MyUser> MyCol { get; set; }
        public Dictionary<string, string> MyDict { get; set; }
    }

    public enum MyEnum
    {
        val1,
        val2
    }
}
