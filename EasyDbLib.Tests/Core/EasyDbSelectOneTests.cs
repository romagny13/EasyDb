using EasyDbLib.Tests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;

namespace EasyDbLib.Tests.Core
{
    [TestClass]
    public class EasyDbSelectOneTests
    {
        [ClassInitialize()]
        public static void ClassInit(TestContext context)
        {
            InitDb.CreateDbLikeProperties();
            InitDb.CreateDbTestLikeMySql();
        }

        [TestMethod]
        public async Task SelectOne()
        {
            var db = new EasyDb();
            db.SetConnectionStringSettings(DbConstants.SqlFile, DbConstants.SqlProviderName);

            db.DefaultMappingBehavior = DefaultMappingBehavior.CreateEmptyTable;

            User result = null;

            using (var command = db.CreateSqlCommand("select * from [User] where [Id]=@id").AddInParameter("@id", 1))
            {
                result = await db.SelectOneAsync<User>(command);
            }

            Assert.AreEqual("Marie", result.UserName);
            Assert.AreEqual(null, result.Age);
            Assert.AreEqual("marie@domain.com", result.Email);
        }

        [TestMethod]
        public async Task SelectOne_WithoutCompleteMappingAndeIgnoreCase_Fail()
        {
            var db = new EasyDb();
            db.SetConnectionStringSettings(DbConstants.SqlFileLikeMySql, DbConstants.SqlProviderName);


            var table = db.SetTable<User>("users")
                .SetPrimaryKeyColumn("id", p => p.Id);

            table.IgnoreCase = false;

            User result = null;

            using (var command = db.CreateSqlCommand("select * from [users] where [Id]=@id").AddInParameter("@id", 2))
            {
                result = await db.SelectOneAsync<User>(command);
            }

            Assert.AreEqual(null, result.UserName);
            Assert.AreEqual(null, result.Age);
            Assert.AreEqual(null, result.Email);
        }

        [TestMethod]
        public async Task SelectOne_WithCompleteMappingAndeIgnoreCase_Success()
        {
            var db = new EasyDb();
            db.SetConnectionStringSettings(DbConstants.SqlFileLikeMySql, DbConstants.SqlProviderName);

            var table = db.SetTable<User>("users")
                .SetPrimaryKeyColumn("id", p => p.Id)
                .SetColumn("username", p => p.UserName)
                .SetColumn("age", p => p.Age)
                .SetColumn("email", p => p.Email);

            table.IgnoreCase = false;

            User result = null;

            using (var command = db.CreateSqlCommand("select * from [users] where [id]=@id").AddInParameter("@id", 2))
            {
                result = await db.SelectOneAsync<User>(command);
            }

            Assert.AreEqual("Pat", result.UserName);
            Assert.AreEqual(30, result.Age);
            Assert.AreEqual(null, result.Email);
        }


        [TestMethod]
        public async Task SelectOne_WithCondition()
        {
            var db = new EasyDb();
            db.SetConnectionStringSettings(DbConstants.SqlFile, DbConstants.SqlProviderName);

            db.DefaultMappingBehavior = DefaultMappingBehavior.CreateEmptyTable;

            var result = await db.SelectOneAsync<User>(Check.Op("Age", ">", 26));

            Assert.AreEqual("Pat", result.UserName);
            Assert.AreEqual(30, result.Age);
            Assert.AreEqual(null, result.Email);
        }

        [TestMethod]
        public async Task SelectOne_WithSelectionOneCommandFactory()
        {
            var db = new EasyDb();
            db.SetConnectionStringSettings(DbConstants.SqlFile, DbConstants.SqlProviderName);

            db.DefaultMappingBehavior = DefaultMappingBehavior.CreateEmptyTable;

            var user = new User
            {
                Id = 2
            };

            var result = await db.SelectOneAsync<User>(new UserSelectionOneFactory(),user, new UserModelFactory());

            Assert.AreEqual("Pat", result.UserName);
            Assert.AreEqual(30, result.Age);
            Assert.AreEqual(null, result.Email);
        }

        [TestMethod]
        public async Task SelectOne_WithModelFactory()
        {
            var db = new EasyDb();
            db.SetConnectionStringSettings(DbConstants.SqlFile, DbConstants.SqlProviderName);

            db.DefaultMappingBehavior = DefaultMappingBehavior.CreateEmptyTable;

            Post result = null;

            using (var command = db.CreateSqlCommand("select a.Title, a.Content, a.UserId, b.UserName from [Post] as a,[User] as b where a.UserId=b.Id and a.Id=@id").AddInParameter("@id", 2))
            {
                result = await db.SelectOneAsync<Post>(command, (reader, idb) =>
                {
                    return new Post
                    {
                        Title = ((string)reader["Title"]).Trim(),
                        Content = ((string)reader["Content"]).Trim(),
                        UserId = (int)reader["UserId"],
                        User = new User
                        {
                            UserName = ((string)reader["UserName"]).Trim()
                        }
                    };
                });
            }
            Assert.AreEqual("Post 2", result.Title);
            Assert.AreEqual("Content 2", result.Content);
            Assert.AreEqual("Pat", result.User.UserName);
        }

    }

    public class UserSelectionOneFactory : ISelectionOneCommandFactory<User>
    {
        public DbCommand CreateCommand(EasyDb db, User model)
        {
            return db.CreateSqlCommand("select * from [User] where Id=@id").AddInParameter("@id", model.Id);
        }
    }


}
