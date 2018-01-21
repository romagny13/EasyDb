using EasyDbLib.Tests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace EasyDbLib.Tests.Core
{

    [TestClass]
    public class EasyDbSelectAllTests
    {
        [ClassInitialize()]
        public static void ClassInit(TestContext context)
        {
            InitDb.CreateDbLikeProperties();
            InitDb.CreateDbTestLikeMySql();
        }

        [TestMethod]
        public async Task SelectAll()
        {
            var db = new EasyDb();
            db.SetConnectionStringSettings(DbConstants.SqlFile, DbConstants.SqlProviderName);

            db.DefaultMappingBehavior = DefaultMappingBehavior.CreateEmptyTable;

            List<User> result = null;

            using (var command = db.CreateSqlCommand("select * from [User]"))
            {
                result = await db.SelectAllAsync<User>(command);
            }

            Assert.AreEqual(4, result.Count);

            Assert.AreEqual("Marie", result[0].UserName);
            Assert.AreEqual(null, result[0].Age);
            Assert.AreEqual("marie@domain.com", result[0].Email);

            Assert.AreEqual("Pat", result[1].UserName);
            Assert.AreEqual(30, result[1].Age);
            Assert.AreEqual(null, result[1].Email);
        }

        [TestMethod]
        public async Task SelectAll_WithoutCompleteMappingAndeIgnoreCase_Fail()
        {
            var db = new EasyDb();
            db.SetConnectionStringSettings(DbConstants.SqlFileLikeMySql, DbConstants.SqlProviderName);


            var table = db.SetTable<User>("users")
                .SetPrimaryKeyColumn("id", p => p.Id);

            table.IgnoreCase = false;

            List<User> result = null;

            using (var command = db.CreateSqlCommand("select * from [users]"))
            {
                result = await db.SelectAllAsync<User>(command);
            }

            Assert.AreEqual(4, result.Count);

            Assert.AreEqual(null, result[0].UserName);
            Assert.AreEqual(null, result[0].Age);
            Assert.AreEqual(null, result[0].Email);

            Assert.AreEqual(null, result[1].UserName);
            Assert.AreEqual(null, result[1].Age);
            Assert.AreEqual(null, result[1].Email);
        }

        [TestMethod]
        public async Task SelectAll_WithCompleteMappingAndeIgnoreCase_Success()
        {
            var db = new EasyDb();
            db.SetConnectionStringSettings(DbConstants.SqlFileLikeMySql, DbConstants.SqlProviderName);

            var table = db.SetTable<User>("users")
                .SetPrimaryKeyColumn("id", p => p.Id)
                .SetColumn("username", p => p.UserName)
                .SetColumn("age", p => p.Age)
                .SetColumn("email", p => p.Email);

            table.IgnoreCase = false;

            List<User> result = null;

            using (var command = db.CreateSqlCommand("select * from [users]"))
            {
                result = await db.SelectAllAsync<User>(command);
            }

            Assert.AreEqual(4, result.Count);

            Assert.AreEqual("Marie", result[0].UserName);
            Assert.AreEqual(null, result[0].Age);
            Assert.AreEqual("marie@domain.com", result[0].Email);

            Assert.AreEqual("Pat", result[1].UserName);
            Assert.AreEqual(30, result[1].Age);
            Assert.AreEqual(null, result[1].Email);
        }


        [TestMethod]
        public async Task SelectAll_WithLimit()
        {
            var db = new EasyDb();
            db.SetConnectionStringSettings(DbConstants.SqlFile, DbConstants.SqlProviderName);

            db.DefaultMappingBehavior = DefaultMappingBehavior.CreateEmptyTable;

            var result = await db.SelectAllAsync<User>(2, null, null); // var result = await db.SelectAllAsync<User>(2, null, new string[] { "UserName" });

            Assert.AreEqual(2, result.Count);

            Assert.AreEqual("Marie", result[0].UserName);
            Assert.AreEqual(null, result[0].Age);
            Assert.AreEqual("marie@domain.com", result[0].Email);

            Assert.AreEqual("Pat", result[1].UserName);
            Assert.AreEqual(30, result[1].Age);
            Assert.AreEqual(null, result[1].Email);
        }


        [TestMethod]
        public async Task SelectAll_WithSorts()
        {
            var db = new EasyDb();
            db.SetConnectionStringSettings(DbConstants.SqlFile, DbConstants.SqlProviderName);

            db.DefaultMappingBehavior = DefaultMappingBehavior.CreateEmptyTable;

            var result = await db.SelectAllAsync<User>(null, null, new string[] { "UserName" });

            Assert.AreEqual(4, result.Count);

            Assert.AreEqual("Deb", result[0].UserName);
            Assert.AreEqual(null, result[0].Age);
            Assert.AreEqual("deb@domain.com", result[0].Email);

            Assert.AreEqual("Ken", result[1].UserName);
            Assert.AreEqual(25, result[1].Age);
            Assert.AreEqual(null, result[1].Email);
        }


        [TestMethod]
        public async Task SelectAll_WithCondition()
        {
            var db = new EasyDb();
            db.SetConnectionStringSettings(DbConstants.SqlFile, DbConstants.SqlProviderName);

            db.DefaultMappingBehavior = DefaultMappingBehavior.CreateEmptyTable;

            var result = await db.SelectAllAsync<User>(null, Check.Op("Age", ">", 26));

            Assert.AreEqual(1, result.Count);

            Assert.AreEqual("Pat", result[0].UserName);
            Assert.AreEqual(30, result[0].Age);
            Assert.AreEqual(null, result[0].Email);
        }

        [TestMethod]
        public async Task SelectAll_WithSelectionAllCommandFactory()
        {
            var db = new EasyDb();
            db.SetConnectionStringSettings(DbConstants.SqlFile, DbConstants.SqlProviderName);

            db.DefaultMappingBehavior = DefaultMappingBehavior.CreateEmptyTable;

            var result = await db.SelectAllAsync<User>(new UserSelectionAllFactory(), new UserModelFactory());

            Assert.AreEqual(4, result.Count);

            Assert.AreEqual("Marie", result[0].UserName);
            Assert.AreEqual(null, result[0].Age);
            Assert.AreEqual("marie@domain.com", result[0].Email);

            Assert.AreEqual("Pat", result[1].UserName);
            Assert.AreEqual(30, result[1].Age);
            Assert.AreEqual(null, result[1].Email);
        }

        [TestMethod]
        public async Task SelectAll_WithModelFactory()
        {
            var db = new EasyDb();
            db.SetConnectionStringSettings(DbConstants.SqlFile, DbConstants.SqlProviderName);

            db.DefaultMappingBehavior = DefaultMappingBehavior.CreateEmptyTable;

            List<User> result = null;

            using (var command = db.CreateSqlCommand("select * from [User]"))
            {
                result = await db.SelectAllAsync<User>(command, (reader, idb) =>
                {
                    return new User
                    {
                        UserName = ((string)reader["UserName"]).Trim(),
                        Email = db.CheckDBNullAndConvertTo<string>(reader["Email"])?.Trim(),
                        Age = db.CheckDBNullAndConvertTo<int?>(reader["Age"])
                    };
                });
            }

            Assert.AreEqual(4, result.Count);

            Assert.AreEqual("Marie", result[0].UserName);
            Assert.AreEqual(null, result[0].Age);
            Assert.AreEqual("marie@domain.com", result[0].Email);

            Assert.AreEqual("Pat", result[1].UserName);
            Assert.AreEqual(30, result[1].Age);
            Assert.AreEqual(null, result[1].Email);
        }

    }

    public class UserSelectionAllFactory : ISelectionAllCommandFactory<User>
    {
        public DbCommand CreateCommand(EasyDb db)
        {
            return db.CreateSqlCommand("select * from [User]");
        }
    }

    public class UserModelFactory : IModelFactory<User>
    {
        //public User CreateModel(IDataReader reader)
        //{
        //    return new User
        //    {
        //        UserName = ((string)reader["UserName"]).Trim(),
        //        Email = reader["Email"] == DBNull.Value ? default(string) : ((string)reader["Email"]).Trim(),
        //        Age = reader["Age"] == DBNull.Value ? default(int?) : (int)reader["Age"]
        //    };
        //}

        public User CreateModel(IDataReader reader, EasyDb db)
        {
            // db => model
            return new User
            {
                UserName = ((string)reader["UserName"]).Trim(),
                Email = db.CheckDBNullAndConvertTo<string>(reader["Email"])?.Trim(),
                Age = db.CheckDBNullAndConvertTo<int?>(reader["Age"])
            };
        }
    }



}
