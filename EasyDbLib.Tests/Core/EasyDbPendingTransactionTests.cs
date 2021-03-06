﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;

namespace EasyDbLib.Tests.Core
{

    [TestClass]
    public class EasyDbPendingTransactionTests
    {
        [ClassInitialize()]
        public static void ClassInit(TestContext context)
        {
            InitDb.CreateDbLikeProperties();
            InitDb.CreateDbTestLikeMySql();
        }

        [TestMethod]
        public async Task PendingTransaction()
        {
            var db = new EasyDb();

            db.SetConnectionStringSettings(DbConstants.SqlDb1, DbConstants.SqlProviderName);

            db.DefaultMappingBehavior = DefaultMappingBehavior.CreateEmptyTable;

            db.AddPendingOperation(() => db.InsertAsync<User>(new User { UserName = "u1" }));
            db.AddPendingOperation(() => db.InsertAsync<User>(new User { UserName = "u2" }));
            db.AddPendingOperation(() => db.InsertAsync<User>(new User { UserName = "u3" }));

            Assert.AreEqual(3, db.PendingOperations.Count);

            var result = await db.ExecutePendingOperationsAsync();

            Assert.IsTrue(result);
            Assert.AreEqual(0, db.PendingOperations.Count);
        }

        [TestMethod]
        public async Task PendingTransaction_Fail()
        {
            var db = new EasyDb();

            db.SetConnectionStringSettings(DbConstants.SqlDb1, DbConstants.SqlProviderName);

            db.DefaultMappingBehavior = DefaultMappingBehavior.CreateEmptyTable;

            db.AddPendingOperation(() => db.InsertAsync<User>(new User { UserName = "u4" }));

            // constraint on user permission
            var user = await db.SelectOneAsync<User>(Check.Op("Id", 1));
            db.AddPendingOperation(() => db.DeleteAsync<User>(user,Check.Op("Id",1)));

            Assert.AreEqual(2, db.PendingOperations.Count);

            var result = await db.ExecutePendingOperationsAsync();

           

            Assert.IsFalse(result);
            Assert.AreEqual(2, db.PendingOperations.Count);
        }
    }

   
}
