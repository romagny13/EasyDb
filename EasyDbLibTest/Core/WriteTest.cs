using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.Common;
using EasyDbLib;
using System.Threading.Tasks;
using System.Data;

namespace EasyDbLibTest
{
    [TestClass]
    public class WriteTest
    {
        [ClassInitialize()]
        public static void ClassInit(TestContext context)
        {
            InitDb.CreateDbTest();
            Mapping.Clear();
        }

        // create valid commands

        [TestMethod]
        public void TestCreateCommand_WithOneParameter_HaveParameter()
        {
            var db = new EasyDb();
            db.SetConnectionStringSettings(InitDb.SqlConnectionString, InitDb.SqlProviderName);
            var sql = "insert into [categories] (name) values(@name)";
            var result = db.CreateCommand(sql)
                .AddParameter("@name", "Web");

            Assert.IsNotNull(result.Command);

            Assert.AreEqual(sql,result.Command.CommandText);
            Assert.AreEqual(CommandType.Text, result.Command.CommandType);

            Assert.AreEqual(1, result.Command.Parameters.Count);

            Assert.AreEqual("@name", result.Command.Parameters[0].ParameterName);
            Assert.AreEqual("Web", result.Command.Parameters[0].Value);
        }

        [TestMethod]
        public void TestCreateCommand_WithParameters_HaveParameters()
        {
            var db = new EasyDb();
            db.SetConnectionStringSettings(InitDb.SqlConnectionString, InitDb.SqlProviderName);

            var user = new User
            {
                FirstName = "Alexandra",
                LastName = "Bellin",
                Age = 20,
                Email = "alex@domain.com"
            };

            var sql = "insert into [users] (firstname,lastname,age,email) values(@firstname,@lastname,@age,@email)";
            var result = db.CreateCommand(sql)
                .AddParameter("@firstname", user.FirstName)
                .AddParameter("@lastname", user.LastName)
                .AddParameter("@age", user.Age)
                .AddParameter("@email", user.Email);

            Assert.IsNotNull(result.Command);

            Assert.AreEqual(sql, result.Command.CommandText);
            Assert.AreEqual(CommandType.Text, result.Command.CommandType);

            Assert.AreEqual(4, result.Command.Parameters.Count);

            Assert.AreEqual("@firstname", result.Command.Parameters[0].ParameterName);
            Assert.AreEqual(user.FirstName, result.Command.Parameters[0].Value);

            Assert.AreEqual("@lastname", result.Command.Parameters[1].ParameterName);
            Assert.AreEqual(user.LastName, result.Command.Parameters[1].Value);

            Assert.AreEqual("@age", result.Command.Parameters[2].ParameterName);
            Assert.AreEqual(user.Age, result.Command.Parameters[2].Value);

            Assert.AreEqual("@email", result.Command.Parameters[3].ParameterName);
            Assert.AreEqual(user.Email, result.Command.Parameters[3].Value);
        }


        // insert

        [TestMethod]
        public async Task TestCreateCommand_InsertWithNonQueryAsync()
        {
            var db = new EasyDb();
            db.SetConnectionStringSettings(InitDb.SqlConnectionString, InitDb.SqlProviderName);
            var sql = "insert into [categories] (name) values(@name)";
            var result = await db.CreateCommand(sql)
                .AddParameter("@name", "PHP")
                .NonQueryAsync();

            Assert.AreEqual(1,result);
        }

        [TestMethod]
        public async Task TestCreateCommand_InsertMultipleWithNonQueryAsync_ReturnsRowAffected()
        {
            var db = new EasyDb();
            db.SetConnectionStringSettings(InitDb.SqlConnectionString, InitDb.SqlProviderName);
            var sql = "insert into [categories] (name) values(@name);insert into [categories] (name) values(@name2)";
            var result = await db.CreateCommand(sql)
                .AddParameter("@name", "Java")
                .AddParameter("@name2", ".Net")
                .NonQueryAsync();

            Assert.AreEqual(2, result);
        }

        [TestMethod]
        public async Task TestInsert_WithScalarAsync_InsertAndReturnsLastInsertedId()
        {
            var db = new EasyDb();
            db.SetConnectionStringSettings(InitDb.SqlConnectionString, InitDb.SqlProviderName);

            var user = new User
            {
                FirstName = "Pato",
                LastName = "Premo",
                Age = 30
            };

            var sql = "insert into [users] (firstname,lastname,age,email) output inserted.id values(@firstname,@lastname,@age,@email)";
            var lastInsertedId = await db.CreateCommand(sql)
                .AddParameter("@firstname", user.FirstName)
                .AddParameter("@lastname", user.LastName)
                .AddParameter("@age", user.Age)
                .AddParameter("@email", user.Email)
                .ScalarAsync();

            Assert.AreEqual(3, lastInsertedId); // 3

            var result = await db.CreateCommand("select * from [users] where [id]=@id")
                .AddParameter("@id", lastInsertedId)
                .ReadOneAsync<UserLikeTable>();

            Assert.AreEqual(lastInsertedId, result.id);
            Assert.AreEqual(user.FirstName, result.firstname);
            Assert.AreEqual(user.LastName, result.lastname);
            Assert.AreEqual(30, result.age);
            Assert.AreEqual(null, result.email);
        }

        [TestMethod]
        public async Task TestInsert_WithNullable_ReplaceNullByDBNullValue()
        {
            var db = new EasyDb();
            db.SetConnectionStringSettings(InitDb.SqlConnectionString, InitDb.SqlProviderName);

            var user = new User
            {
                FirstName = "Ken",
                LastName = "Nuke",
                Email = "ken@domain.com"
            };

            var sql = "insert into [users] (firstname,lastname,age,email) output inserted.id values(@firstname,@lastname,@age,@email)";
            var id = await db.CreateCommand(sql)
                .AddParameter("@firstname", user.FirstName)
                .AddParameter("@lastname", user.LastName)
                .AddParameter("@age", user.Age)
                .AddParameter("@email", user.Email)
                .ScalarAsync();

            var result = await db.CreateCommand("select * from [users] where [id]=@id")
                .AddParameter("@id", id) 
                .ReadOneAsync<UserLikeTable>();

            Assert.AreEqual(user.FirstName, result.firstname);
            Assert.AreEqual(user.LastName, result.lastname);
            Assert.AreEqual(null, result.age);
            Assert.AreEqual(user.Email, result.email);
        }

    }

}
