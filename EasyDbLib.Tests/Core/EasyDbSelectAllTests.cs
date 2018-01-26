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
        public async Task SelectAll_WithCommandAndFactory()
        {
            var db = new EasyDb();
            db.SetConnectionStringSettings(DbConstants.SqlDb1, DbConstants.SqlProviderName);

            db.DefaultMappingBehavior = DefaultMappingBehavior.CreateEmptyTable;

            List<User> result = null;

            using (var command = db.CreateSqlCommand("select * from [User]"))
            {
                result = await db.SelectAllAsync<User>(command, (reader, idb) =>
                {
                    return new User
                    {
                        Id = (int)reader["Id"],
                        UserName = ((string)reader["UserName"]).Trim(),
                        Email = idb.CheckDBNullAndConvertTo<string>(reader["Email"])?.Trim(),
                        Age = idb.CheckDBNullAndConvertTo<int?>(reader["Age"])
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

        [TestMethod]
        public async Task SelectAll_WithCommandAndDefaultFactory()
        {
            var db = new EasyDb();
            db.SetConnectionStringSettings(DbConstants.SqlDb1, DbConstants.SqlProviderName);

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
        public async Task SelectAll_WithSelectionAllCommandFactory_And_ModelFactory_And_NullCriteria()
        {
            var db = new EasyDb();
            db.SetConnectionStringSettings(DbConstants.SqlDb1, DbConstants.SqlProviderName);

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
        public async Task SelectAll_WithSelectionAllCommandFactory_And_DefaultModelFactory_And_NullCriteria()
        {
            var db = new EasyDb();
            db.SetConnectionStringSettings(DbConstants.SqlDb1, DbConstants.SqlProviderName);

            db.DefaultMappingBehavior = DefaultMappingBehavior.CreateEmptyTable;

            var result = await db.SelectAllAsync<User>(new UserSelectionAllFactory());

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
            db.SetConnectionStringSettings(DbConstants.SqlDbLikeMySql, DbConstants.SqlProviderName);


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
            db.SetConnectionStringSettings(DbConstants.SqlDbLikeMySql, DbConstants.SqlProviderName);

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
        public async Task SelectAll_WithSelectionAllCommandFactory_And_ModelFactory_And_Criteria()
        {
            var db = new EasyDb();
            db.SetConnectionStringSettings(DbConstants.SqlDb1, DbConstants.SqlProviderName);

            db.DefaultMappingBehavior = DefaultMappingBehavior.CreateEmptyTable;

            var result = await db.SelectAllAsync<Post, int>(new PostSelectionAllFactory(), new PostModelFactory(),2);

            Assert.AreEqual(3, result.Count);

            Assert.AreEqual(2, result[0].Id);
            Assert.AreEqual("Post 2", result[0].Title);
            Assert.AreEqual("Content 2", result[0].Content);
            Assert.AreEqual(2, result[0].UserId);

            Assert.AreEqual(3, result[1].Id);
            Assert.AreEqual("Post 3", result[1].Title);
            Assert.AreEqual("Content 3", result[1].Content);
            Assert.AreEqual(2, result[1].UserId);

            Assert.AreEqual(4, result[2].Id);
            Assert.AreEqual("Post 4", result[2].Title);
            Assert.AreEqual("Content 4", result[2].Content);
            Assert.AreEqual(2, result[2].UserId);
        }

        [TestMethod]
        public async Task SelectAll_WithSelectionAllCommandFactory_And_DefaultModelFactory_And_Criteria()
        {
            var db = new EasyDb();
            db.SetConnectionStringSettings(DbConstants.SqlDb1, DbConstants.SqlProviderName);

            db.DefaultMappingBehavior = DefaultMappingBehavior.CreateEmptyTable;

            var result = await db.SelectAllAsync<Post, int>(new PostSelectionAllFactory(), 2);

            Assert.AreEqual(3, result.Count);

            Assert.AreEqual(2, result[0].Id);
            Assert.AreEqual("Post 2", result[0].Title);
            Assert.AreEqual("Content 2", result[0].Content);
            Assert.AreEqual(2, result[0].UserId);

            Assert.AreEqual(3, result[1].Id);
            Assert.AreEqual("Post 3", result[1].Title);
            Assert.AreEqual("Content 3", result[1].Content);
            Assert.AreEqual(2, result[1].UserId);

            Assert.AreEqual(4, result[2].Id);
            Assert.AreEqual("Post 4", result[2].Title);
            Assert.AreEqual("Content 4", result[2].Content);
            Assert.AreEqual(2, result[2].UserId);
        }

        [TestMethod]
        public async Task SelectAll_WithLimit()
        {
            var db = new EasyDb();
            db.SetConnectionStringSettings(DbConstants.SqlDb1, DbConstants.SqlProviderName);

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
            db.SetConnectionStringSettings(DbConstants.SqlDb1, DbConstants.SqlProviderName);

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
            db.SetConnectionStringSettings(DbConstants.SqlDb1, DbConstants.SqlProviderName);

            db.DefaultMappingBehavior = DefaultMappingBehavior.CreateEmptyTable;

            var result = await db.SelectAllAsync<User>(null, Check.Op("Age", ">", 26));

            Assert.AreEqual(1, result.Count);

            Assert.AreEqual("Pat", result[0].UserName);
            Assert.AreEqual(30, result[0].Age);
            Assert.AreEqual(null, result[0].Email);
        }

    }

    public class UserSelectionAllFactory : ISelectionAllCommandFactory<User, NullCriteria>
    {
        public DbCommand CreateCommand(EasyDb db, NullCriteria criteria)
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


    public class PostSelectionAllFactory : ISelectionAllCommandFactory<Post, int>
    {
        public DbCommand CreateCommand(EasyDb db, int criteria)
        {
            return db.CreateSqlCommand("select * from [Post] where [UserId]=@userid").AddInParameter("@userid", criteria);
        }
    }

    public class PostModelFactory : IModelFactory<Post>
    {
        public Post CreateModel(IDataReader reader, EasyDb db)
        {
            return new Post
            {
                Id = (int)reader["Id"],
                Title = ((string)reader["Title"]).Trim(),
                Content = ((string)reader["Content"]).Trim(),
                UserId = (int)reader["UserId"]
            };
        }
    }

}
