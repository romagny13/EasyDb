using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using EasyDbLib;
using System.Data;

namespace EasyDbLibTest.Core
{
    [TestClass]
    public class ReadTest
    {
        [ClassInitialize()]
        public static void ClassInit(TestContext context)
        {
            InitDb.CreateDbTest();
            Mapping.Clear();
        }

        // parameters

        [TestMethod]
        public void TestCreateCommand_WithParameter()
        {
            var db = new EasyDb();
            db.SetConnectionStringSettings(InitDb.SqlConnectionString, InitDb.SqlProviderName);

            var sql = "select * from users where id=@id";
            var result = db.CreateCommand(sql).AddParameter("@id",10);

            Assert.AreEqual(sql, result.Command.CommandText);
            Assert.AreEqual(CommandType.Text, result.Command.CommandType);
            Assert.AreEqual(1, result.Command.Parameters.Count);
            Assert.AreEqual("@id", result.Command.Parameters[0].ParameterName);
            Assert.AreEqual(10, result.Command.Parameters[0].Value);
            Assert.AreEqual(ParameterDirection.Input, result.Command.Parameters[0].Direction);
            Assert.AreEqual(DbType.Int32, result.Command.Parameters[0].DbType);
        }

        [TestMethod]
        public void TestCreateCommand_WithParameters()
        {
            var db = new EasyDb();
            db.SetConnectionStringSettings(InitDb.SqlConnectionString, InitDb.SqlProviderName);

            var sql = "select * from users where id=@id and category_id=@id2";
            var result = db.CreateCommand(sql).AddParameter("@id", 10).AddParameter("@id2",20);

            Assert.AreEqual(sql, result.Command.CommandText);
            Assert.AreEqual(CommandType.Text, result.Command.CommandType);

            Assert.AreEqual(2, result.Command.Parameters.Count);

            Assert.AreEqual("@id", result.Command.Parameters[0].ParameterName);
            Assert.AreEqual(10, result.Command.Parameters[0].Value);
            Assert.AreEqual(ParameterDirection.Input, result.Command.Parameters[0].Direction);
            Assert.AreEqual(DbType.Int32, result.Command.Parameters[0].DbType);

            Assert.AreEqual("@id2", result.Command.Parameters[1].ParameterName);
            Assert.AreEqual(20, result.Command.Parameters[1].Value);
            Assert.AreEqual(ParameterDirection.Input, result.Command.Parameters[1].Direction);
            Assert.AreEqual(DbType.Int32, result.Command.Parameters[1].DbType);
        }

        [TestMethod]
        public void TestAddParameter_WithDbType()
        {
            var db = new EasyDb();
            db.SetConnectionStringSettings(InitDb.SqlConnectionString, InitDb.SqlProviderName);

            var sql = "select * from users where id=@id";
            var result = db.CreateCommand(sql).AddParameter("@id", 10, DbType.Int16);

            Assert.AreEqual("@id", result.Command.Parameters[0].ParameterName);
            Assert.AreEqual(10, result.Command.Parameters[0].Value);
            Assert.AreEqual(ParameterDirection.Input, result.Command.Parameters[0].Direction);
            Assert.AreEqual(DbType.Int16, result.Command.Parameters[0].DbType);
        }

        [TestMethod]
        public void TestAddParameter_WithDirection()
        {
            var db = new EasyDb();
            db.SetConnectionStringSettings(InitDb.SqlConnectionString, InitDb.SqlProviderName);

            var sql = "select * from users where id=@id";
            var result = db.CreateCommand(sql).AddParameter("@id", 10, ParameterDirection.Output);

            Assert.AreEqual("@id", result.Command.Parameters[0].ParameterName);
            Assert.AreEqual(10, result.Command.Parameters[0].Value);
            Assert.AreEqual(ParameterDirection.Output, result.Command.Parameters[0].Direction);
            Assert.AreEqual(DbType.Int32, result.Command.Parameters[0].DbType);
        }

        [TestMethod]
        public void TestAddParameter_WithDbTypeAndDirection()
        {
            var db = new EasyDb();
            db.SetConnectionStringSettings(InitDb.SqlConnectionString, InitDb.SqlProviderName);

            var sql = "select * from users where id=@id";
            var result = db.CreateCommand(sql).AddParameter("@id", 10, DbType.Int16, ParameterDirection.Output);

            Assert.AreEqual("@id", result.Command.Parameters[0].ParameterName);
            Assert.AreEqual(10, result.Command.Parameters[0].Value);
            Assert.AreEqual(ParameterDirection.Output, result.Command.Parameters[0].Direction);
            Assert.AreEqual(DbType.Int16, result.Command.Parameters[0].DbType);
        }


        // read all

        [TestMethod]
        public async Task TestReadAllAsync()
        {
            var db = new EasyDb();
            db.SetConnectionStringSettings(InitDb.SqlConnectionString, InitDb.SqlProviderName);

            var result = await db
                .CreateCommand("select * from users")
                .ReadAllAsync<UserLikeTable>();

            Assert.AreEqual(null, result[0].age);
            Assert.AreEqual(1, result[0].id);
            Assert.AreEqual("Marie", result[0].firstname);
            Assert.AreEqual("Bellin", result[0].lastname);
            Assert.AreEqual("marie@domain.com", result[0].email);

            Assert.AreEqual(30, result[1].age);
            Assert.AreEqual(2, result[1].id);
            Assert.AreEqual("Pat", result[1].firstname);
            Assert.AreEqual("Prem", result[1].lastname);
            Assert.AreEqual(null, result[1].email);
        }

        // read one

        [TestMethod]
        public async Task TestReadOneAsync_WithMapping()
        {
            var db = new EasyDb();
            db.SetConnectionStringSettings(InitDb.SqlConnectionString, InitDb.SqlProviderName);

            Mapping
                .SetTable("users")
                .SetColumn("id", "Id")
                .SetColumn("firstname", "FirstName")
                .SetColumn("lastname", "LastName")
                .SetColumn("age", "Age")
                .SetColumn("email", "Email");

            var user = new User
            {
                Id = 2,
                FirstName = "Pat",
                LastName = "Prem",
                Age = 30
            };

            var result = await db.CreateCommand("select * from [users] where [id]=@id")
                .AddParameter("@id", user.Id)
                .ReadOneAsync<User>(Mapping.GetTable("users"));

            Assert.AreEqual(user.Id, result.Id);
            Assert.AreEqual(user.FirstName, result.FirstName);
            Assert.AreEqual(user.LastName, result.LastName);
            Assert.AreEqual(user.Age, result.Age);
            Assert.AreEqual(null, result.Email);
        }

        [TestMethod]
        public async Task TestReadOneAsync_WithDbType()
        {
            var db = new EasyDb();
            db.SetConnectionStringSettings(InitDb.SqlConnectionString, InitDb.SqlProviderName);

            var result = await db.CreateCommand("select * from [users] where [id]=@id")
                   .AddParameter("@id", 1, DbType.Int16)
                .ReadOneAsync<UserLikeTable>();

            Assert.IsNotNull(result);
            Assert.AreEqual("Marie", result.firstname);
        }

        [TestMethod]
        public async Task TestReadOneAsync_WithInvalidDbType_Fail()
        {
            bool failed = false;
            var db = new EasyDb();
            db.SetConnectionStringSettings(InitDb.SqlConnectionString, InitDb.SqlProviderName);

            try
            {
                var result = await db.CreateCommand("select * from [users] where [id]=@id")
                      .AddParameter("@id", 1, DbType.Date)
                      .ReadOneAsync<UserLikeTable>();
            }
            catch (Exception)
            {
                failed = true;
            }

            Assert.IsTrue(failed);
        }

        [TestMethod]
        public async Task TestReadOneNotExist_RetrunsNull()
        {
            var db = new EasyDb();
            db.SetConnectionStringSettings(InitDb.SqlConnectionString, InitDb.SqlProviderName);

            var result = await db.CreateCommand("select * from [users] where [id]=@id")
                     .AddParameter("@id", 1000)
                     .ReadOneAsync<UserLikeTable>();

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task TestReadOneAsync_ColumnNotInClassIsIgnored()
        {
            var db = new EasyDb();
            db.SetConnectionStringSettings(InitDb.SqlConnectionString, InitDb.SqlProviderName);

            var result = await db.CreateCommand("select * from users where id=@id")
                .AddParameter("@id", 1)
                .ReadOneAsync<DbUserWithIgnoredColumns>();

            Assert.AreEqual(1, result.id);
            Assert.AreEqual("Marie", result.firstname);
        }

        [TestMethod]
        public async Task TestReadOneAsync_WithNullableAndDbNull_ReturnsValuesAndNull()
        {
            var db = new EasyDb();
            db.SetConnectionStringSettings(InitDb.SqlConnectionString, InitDb.SqlProviderName);

            var result = await db.CreateCommand("select * from users where id=@id")
                .AddParameter("@id", 1)
                .ReadOneAsync<UserLikeTable>();

            Assert.AreEqual(null, result.age);
            Assert.AreEqual(1, result.id);
            Assert.AreEqual("Marie", result.firstname);
            Assert.AreEqual("Bellin", result.lastname);
            Assert.AreEqual("marie@domain.com", result.email);
        }

        [TestMethod]
        public async Task TestReadOneAsync_WithNullableAndNotDbNull_ReturnsValues()
        {
            var db = new EasyDb();
            db.SetConnectionStringSettings(InitDb.SqlConnectionString, InitDb.SqlProviderName);

            var result = await db.CreateCommand("select * from users where id=@id")
                .AddParameter("@id", 2)
                .ReadOneAsync<UserLikeTable>();

            Assert.AreEqual(30, result.age);
            Assert.AreEqual(2, result.id);
            Assert.AreEqual("Pat", result.firstname);
            Assert.AreEqual("Prem", result.lastname);
            Assert.AreEqual(null, result.email);
        }
    }
}
