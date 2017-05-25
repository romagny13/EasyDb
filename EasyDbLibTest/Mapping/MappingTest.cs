using System;
using EasyDbLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;

namespace EasyDbLibTest
{
    [TestClass]
    public class MappingTest
    {

        // check

        [TestMethod]
        public void TestGet_WitoutTable_ThrowException()
        {
            bool failed = false;
            try
            {
                Mapping.GetTable("notfound");
            }
            catch (Exception)
            {
                failed = true;
            }

            Assert.IsTrue(failed);
        }

        [TestMethod]
        public void TestInvalidPropertyName_Fail()
        {
            bool failed = false;
            try
            {
                Mapping.AddTable("users").AddColumn("firstname","inv@alid");
            }
            catch (Exception)
            {
                failed = true;
            }

            Assert.IsTrue(failed);
        }

        [TestMethod]
        public void TestInvalidPropertyNameAndDbType_Fail()
        {
            bool failed = false;
            try
            {
                Mapping.AddTable("users").AddColumn("firstname", "inv@alid",DbType.String);
            }
            catch (Exception)
            {
                failed = true;
            }

            Assert.IsTrue(failed);
        }

        [TestMethod]
        public void TestValidPropertyName_Success()
        {
            bool failed = false;
            try
            {
                Mapping.AddTable("users").AddColumn("firstname", "Valid", DbType.String);
            }
            catch (Exception)
            {
                failed = true;
            }

            Assert.IsFalse(failed);
        }

        [TestMethod]
        public void TestInvalidPropertyName_WithPrimaryKey_Fail()
        {
            bool failed = false;
            try
            {
                Mapping.AddTable("users").AddPrimaryKeyColumn("firstname", "inv@alid");
            }
            catch (Exception)
            {
                failed = true;
            }

            Assert.IsTrue(failed);
        }

        [TestMethod]
        public void TestInvalidPropertyName_WithPrimaryKeyAndDbType_Fail()
        {
            bool failed = false;
            try
            {
                Mapping.AddTable("users").AddPrimaryKeyColumn("firstname", "inv@alid", DbType.String);
            }
            catch (Exception)
            {
                failed = true;
            }

            Assert.IsTrue(failed);
        }

        [TestMethod]
        public void TestInvalidPropertyName_WithValidPrimaryKey_Success()
        {
            bool failed = false;
            try
            {
                Mapping.AddTable("users").AddPrimaryKeyColumn("firstname", "valid", DbType.String);
            }
            catch (Exception)
            {
                failed = true;
            }

            Assert.IsFalse(failed);
        }

        [TestMethod]
        public void TestInvalidPropertyName_WithForeignKey_Fail()
        {
            bool failed = false;
            try
            {
                Mapping.AddTable("posts").AddForeignKeyColumn("user_id", "inv@alid","users","id");
            }
            catch (Exception)
            {
                failed = true;
            }

            Assert.IsTrue(failed);
        }

        [TestMethod]
        public void TestInvalidPropertyName_WithForeignKeyAndDbType_Fail()
        {
            bool failed = false;
            try
            {
                Mapping.AddTable("posts").AddForeignKeyColumn("user_id", "inv@alid", "users", "id", DbType.Int16);
            }
            catch (Exception)
            {
                failed = true;
            }

            Assert.IsTrue(failed);
        }

        [TestMethod]
        public void TestInvalidPropertyName_WithValidForeignKey_Success()
        {
            bool failed = false;
            try
            {
                Mapping.AddTable("posts").AddForeignKeyColumn("user_id", "valid", "users", "id", DbType.Int16);
            }
            catch (Exception)
            {
                failed = true;
            }

            Assert.IsFalse(failed);
        }

        // has

        [TestMethod]
        public void TestAddMapping_HasTable()
        {
            Mapping
                .AddTable("products")
                .AddColumn("id", "Id")
                .AddColumn("name", "ProductName");

            Mapping
                .AddTable("users")
                .AddColumn("id", "Id")
                .AddColumn("first", "FirstNAme");


            Assert.IsTrue(Mapping.HasTable("products"));
            Assert.IsTrue(Mapping.HasTable("users"));
            Assert.IsFalse(Mapping.HasTable("categories"));
        }


        [TestMethod]
        public void TestTables_HasColumns()
        {

            Mapping
                .AddTable("products")
                .AddColumn("id", "Id")
                .AddColumn("name", "ProductName");

            Mapping
                .AddTable("users")
                .AddColumn("id", "Id")
                .AddColumn("first", "FirstNAme");

            Assert.IsTrue(Mapping.GetTable("products").HasColumn("id"));
            Assert.IsTrue(Mapping.GetTable("products").HasColumn("name"));

            Assert.IsTrue(Mapping.GetTable("users").HasColumn("id"));
            Assert.IsTrue(Mapping.GetTable("users").HasColumn("first"));
        }

        [TestMethod]
        public void TestKeyColumns()
        {
            Mapping
                .AddTable("posts")
                .AddPrimaryKeyColumn("id", "Id")
                .AddColumn("title", "Title")
                .AddForeignKeyColumn("user_id", "UserId", "users", "id");

            Assert.AreEqual(typeof(PrimaryKeyColumn), Mapping.GetTable("posts").GetColumn("id").GetType());
            Assert.AreEqual(typeof(Column), Mapping.GetTable("posts").GetColumn("title").GetType());
            Assert.AreEqual(typeof(ForeignKeyColumn), Mapping.GetTable("posts").GetColumn("user_id").GetType());
        }

        // get primary keys

        [TestMethod]
        public void TestGetPrimaryKeys()
        {
            Mapping
                .AddTable("posts")
                .AddPrimaryKeyColumn("id", "Id")
                .AddColumn("title", "Title")
                .AddForeignKeyColumn("user_id", "UserId", "users", "id");

            var result = Mapping.GetTable("posts").GetPrimaryKeys();

            Assert.AreEqual(1, result.Length);
            Assert.AreEqual("id", result[0].ColumnName);
            Assert.AreEqual("Id", result[0].PropertyName);
            Assert.AreEqual(typeof(PrimaryKeyColumn), result[0].GetType());
        }

        [TestMethod]
        public void TestGetMultiplePrimaryKeys()
        {
            Mapping
                .AddTable("posts")
                .AddPrimaryKeyColumn("id", "Id")
                .AddPrimaryKeyColumn("id2", "Id2")
                .AddColumn("title", "Title")
              .AddForeignKeyColumn("user_id", "UserId", "users", "id");

            var result = Mapping.GetTable("posts").GetPrimaryKeys();

            Assert.AreEqual(2, result.Length);

            Assert.AreEqual("id", result[0].ColumnName);
            Assert.AreEqual("Id", result[0].PropertyName);
            Assert.AreEqual(typeof(PrimaryKeyColumn), result[0].GetType());

            Assert.AreEqual("id2", result[1].ColumnName);
            Assert.AreEqual("Id2", result[1].PropertyName);
            Assert.AreEqual(typeof(PrimaryKeyColumn), result[1].GetType());
        }

        // get foreign keys

        [TestMethod]
        public void TestGetForeignKeys()
        {
            Mapping
                .AddTable("posts")
                .AddPrimaryKeyColumn("id", "Id")
                .AddColumn("title", "Title")
                .AddForeignKeyColumn("user_id", "UserId", "users", "id");

            var result = Mapping.GetTable("posts").GetForeignKeys("users");

            Assert.AreEqual(1, result.Length);
            Assert.AreEqual("user_id", result[0].ColumnName);
            Assert.AreEqual("UserId", result[0].PropertyName);
            Assert.AreEqual(typeof(ForeignKeyColumn), result[0].GetType());
        }

        [TestMethod]
        public void TestGetMultipleForeignKeys()
        {
            Mapping
                .AddTable("posts")
                .AddPrimaryKeyColumn("id", "Id")
                .AddColumn("title", "Title")
                .AddForeignKeyColumn("user_id", "UserId", "users", "id")
                .AddForeignKeyColumn("user_id2", "UserId2", "users", "id2");

            var result = Mapping.GetTable("posts").GetForeignKeys("users");

            Assert.AreEqual(2, result.Length);

            Assert.AreEqual("user_id", result[0].ColumnName);
            Assert.AreEqual("UserId", result[0].PropertyName);
            Assert.AreEqual(typeof(ForeignKeyColumn), result[0].GetType());

            Assert.AreEqual("user_id2", result[1].ColumnName);
            Assert.AreEqual("UserId2", result[1].PropertyName);
            Assert.AreEqual(typeof(ForeignKeyColumn), result[1].GetType());
        }

        [TestMethod]
        public void TestGetOnlyForeignKeysForTableReferenced()
        {
            Mapping
                .AddTable("posts")
                .AddPrimaryKeyColumn("id", "Id")
                .AddColumn("title", "Title")
                .AddForeignKeyColumn("category_id", "CategoryId", "categories", "id")
                .AddForeignKeyColumn("user_id", "UserId", "users", "id")
                .AddForeignKeyColumn("user_id2", "UserId2", "users", "id2");

            var result = Mapping.GetTable("posts").GetForeignKeys("categories");

            Assert.AreEqual(1, result.Length);

            Assert.AreEqual("category_id", result[0].ColumnName);
            Assert.AreEqual("CategoryId", result[0].PropertyName);
            Assert.AreEqual(typeof(ForeignKeyColumn), result[0].GetType());

            var result2 = Mapping.GetTable("posts").GetForeignKeys("users");

            Assert.AreEqual("user_id", result2[0].ColumnName);
            Assert.AreEqual("UserId", result2[0].PropertyName);
            Assert.AreEqual(typeof(ForeignKeyColumn), result[0].GetType());

            Assert.AreEqual("user_id2", result2[1].ColumnName);
            Assert.AreEqual("UserId2", result2[1].PropertyName);
            Assert.AreEqual(typeof(ForeignKeyColumn), result2[1].GetType());

        }

        [TestMethod]
        public void TestDbType()
        {
            Mapping
                .AddTable("posts")
                .AddColumn("id", "Id", DbType.Int16)
                .AddColumn("title", "Title", DbType.String);

            Assert.AreEqual(DbType.Int16, Mapping.GetTable("posts").GetColumn("id").DbType);
            Assert.AreEqual(DbType.String, Mapping.GetTable("posts").GetColumn("title").DbType);
        }

        [TestMethod]
        public void TestDbType_WithoutValue_IsNull()
        {
            Mapping
                .AddTable("posts")
                .AddColumn("id", "Id")
                .AddColumn("title", "Title");

            Assert.AreEqual(null, Mapping.GetTable("posts").GetColumn("id").DbType);
            Assert.AreEqual(null, Mapping.GetTable("posts").GetColumn("title").DbType);
        }
    }

    public class UserMapping
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
    }
}
