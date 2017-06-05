using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EasyDbLib;
using System.Threading.Tasks;
using System.Data;

namespace EasyDbLibTest
{
    [TestClass]
    public class MySQLTest
    {
        [ClassInitialize()]
        public static void ClassInit(TestContext context)
        {
            Mapping.Clear();
        }
        
        public EasyDb GetDb()
        {
            var db = new EasyDb();
            db.SetConnectionStringSettings(@"server=localhost;database=easydbtests;uid=root", "MySql.Data.MySqlClient");
            return db;
        }

        [TestMethod]
        public async Task TestCreateCommand()
        {
            var db = this.GetDb();

           var result = await db.CreateCommand("SELECT * FROM `users` WHERE `id`=@id")
                .AddParameter("@id", 2)
                .ReadOneAsync<MySQLUser>();

            Assert.AreEqual(2, result.id);
            Assert.AreEqual("user2", result.username);
            Assert.AreEqual(30, result.age);
        }

        [TestMethod]
        public async Task TestCreateStoredProcedureCommand()
        {
            var db = this.GetDb();

            var result = await db.CreateStoredProcedureCommand("get_user")
                 .AddParameter("p_id", 2)
                 .ReadOneAsync<MySQLUser>();

            Assert.AreEqual(2, result.id);
            Assert.AreEqual("user2", result.username);
            Assert.AreEqual(30, result.age);
        }

        [TestMethod]
        public async Task TestCreateStoredProcedureCommand_WitOutputParameter()
        {
            var db = this.GetDb();


            var command = db.CreateStoredProcedureCommand("get_output_age")
               .AddParameter("p_id", 2)
               .AddOutputParameter("p_age");

            await command.ReadOneAsync<MySQLUser>();

            var result = command.GetParameter("p_age");
            Assert.AreEqual(30, result.Value);
        }

        [TestMethod]
        public async Task TestCreateStoredProcedureCommand_WithFunction()
        {
            var db = this.GetDb();


            var command = db.CreateCommand("SELECT `get_username_function`(@p0) AS `get_username_function`;")
               .AddParameter("@p0", 2)
               .AddParameter("get_username_function", null, ParameterDirection.ReturnValue);

           var result = await command.ScalarAsync();

            Assert.AreEqual("user2", result);
        }

        [TestMethod]
        public async Task TestSelectQuery()
        {
            var db = this.GetDb();

            Mapping.SetTable("users");

            var result = await db.Select<MySQLUser>(Mapping.GetTable("users"))
                .Where(Check.Op("id",2))
                .ReadOneAsync();

            Assert.AreEqual(2, result.id);
            Assert.AreEqual("user2", result.username);
            Assert.AreEqual(30, result.age);
        }

        [TestMethod]
        public async Task TestSelectQuery_WithLimit()
        {
            var db = this.GetDb();

            Mapping.SetTable("users");

            var result = await db.Select<MySQLUser>(Mapping.GetTable("users"))
                .Top(1)
                .ReadAllAsync();

            Assert.AreEqual(1, result.Count);

            Assert.AreEqual(1, result[0].id);
            Assert.AreEqual("user1", result[0].username);
            Assert.AreEqual(20, result[0].age);
        }


        [TestMethod]
        public async Task TestInsertQuery_WithFetch()
        {
            var db = this.GetDb();

            Mapping.SetTable("users")
                .SetPrimaryKeyColumn("id","id");

            var user = new MySQLUser
            {
               username="user5",
               age=30
            };

            var result = await db.InsertInto("users")
                .Values("username", user.username)
                .Values("age", user.age)
                .Fetch<MySQLUser>(Mapping.GetTable("users")); // caution: type returned with unsigned primary key

            Assert.IsTrue(result.id > 3);
            Assert.AreEqual(user.username, result.username);
            Assert.AreEqual(user.age, result.age);
        }


        [TestMethod]
        public async Task TestInsertQuery_WithLastInsertedId_ThenUpdate_ThenDelete()
        {
            var db = this.GetDb();

            Mapping.SetTable("users");

            var user = new MySQLUser
            {
                username = "user4",
                age = 20
            };

            var result = await db.InsertInto("users")
                .Values("username", user.username)
                .Values("age", user.age)
                .LastInsertedId<UInt64>(); // caution: type returned with unsigned primary key

            Assert.IsTrue(result > 3);

            await db.Update("users")
                .Set("username", "updated username")
                .Where(Check.Op("id", result))
                .NonQueryAsync();

            var updatedUser = await db.Select<MySQLUser>(Mapping.GetTable("users"))
             .Where(Check.Op("id", result))
             .ReadOneAsync();

            Assert.AreEqual("updated username", updatedUser.username);

            var deleted = await db.DeleteFrom("users")
             .Where(Check.Op("id", result))
             .NonQueryAsync();

            Assert.AreEqual(1, deleted);
        }

    }
    public class MySQLUser
    {
        public int id { get; set; }
        public string username { get; set; }
        public int? age { get; set; }
    }

}
