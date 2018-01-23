﻿using EasyDbLib.Tests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.Common;
using System.Threading.Tasks;

namespace EasyDbLib.Tests.Core
{

    [TestClass]
    public class EasyDbInsertTests
    {
        [ClassInitialize()]
        public static void ClassInit(TestContext context)
        {
            InitDb.CreateDbLikeProperties();
            InitDb.CreateDbTestLikeMySql();
        }

        [TestMethod]
        public async Task Insert()
        {
            var db = new EasyDb();

            db.SetConnectionStringSettings(DbConstants.SqlFile, DbConstants.SqlProviderName);

            db.DefaultMappingBehavior = DefaultMappingBehavior.CreateEmptyTable;

            var user = new User
            {
                UserName = "New User"
            };

            var result = await db.InsertAsync<User>(user);

            Assert.AreEqual(5, result);
            Assert.AreEqual(5, user.Id);
        }

        [TestMethod]
        public async Task Insert_WithCommand()
        {
            var db = new EasyDb();

            db.SetConnectionStringSettings(DbConstants.SqlFile, DbConstants.SqlProviderName);

            db.DefaultMappingBehavior = DefaultMappingBehavior.CreateEmptyTable;

            var user = new User
            {
                UserName = "New User"
            };

            object result = null;

            using (var command = db.CreateSqlCommand("insert into [User](UserName) output inserted.id values(@username)")
                .AddInParameter("@username", "New User 2"))
            {
                result = await db.InsertAsync<User>(command, user, (c, model, r) =>
                {
                    user.Id = (int)r;
                });
            }

            Assert.AreEqual(6, result);
            Assert.AreEqual(6, user.Id);
        }

        [TestMethod]
        public async Task Insert_WithFactory()
        {
            var db = new EasyDb();

            db.SetConnectionStringSettings(DbConstants.SqlFile, DbConstants.SqlProviderName);

            db.DefaultMappingBehavior = DefaultMappingBehavior.CreateEmptyTable;

            var user = new User
            {
                UserName = "New User"
            };

            var result = await db.InsertAsync<User>(new UserInsertFactory(), user);

            Assert.AreEqual(7, result);
            Assert.AreEqual(7, user.Id);
        }
    }

    public class UserInsertFactory : IInsertCommandFactory<User>
    {
        public DbCommand CreateCommand(EasyDb db, User model)
        {
            return db.CreateSqlCommand("insert into [User](UserName) output inserted.id values(@username)")
                 .AddInParameter("@username", model.UserName);
        }

        public void SetNewId(DbCommand command, User model, object result)
        {
            model.Id = (int)result;
        }
    }

}
