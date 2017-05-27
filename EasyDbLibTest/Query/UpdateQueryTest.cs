using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EasyDbLib;
using System.Threading.Tasks;
using System.Data;

namespace EasyDbLibTest.Query
{
    [TestClass]
    public class UpdateQueryTest
    {

        [ClassInitialize()]
        public static void ClassInit(TestContext context)
        {
            InitDb.CreateDbTest();
            Mapping.Clear();
        }

        public EasyDb GetDb()
        {
            var db = new EasyDb();
            db.SetConnectionStringSettings(InitDb.SqlConnectionString, InitDb.SqlProviderName);
            return db;
        }

        // check

        [TestMethod]
        public void TestOneClauseWhere()
        {
            bool failed = false;
            var db = this.GetDb();

            var user = new User
            {
                Id = 2,
                FirstName = "updated firstname",
                LastName = "updated lastname",
            };

            try
            {
                var result = db
                  .Update("users")
                  .Set("firstname", user.FirstName)
                  .Set("lastname", user.LastName)
                  .Where(Check.Op("id", user.Id))
                  .Where(Check.Op("id", 2))
                  .GetQuery();
            }
            catch (Exception)
            {
                failed = true;
            }
            Assert.IsTrue(failed);
        }

        // query

        [TestMethod]
        public void TestUpdate_Query()
        {
            var db = this.GetDb();

            var user = new User
            {
                Id = 2,
                FirstName = "updated firstname",
                LastName = "updated lastname",
            };

            var result = db
                 .Update("users")
                 .Set("firstname", user.FirstName)
                 .Set("lastname", user.LastName)
                 .Where(Check.Op("id", user.Id))
                 .GetQuery();

            Assert.AreEqual("update [users] set [firstname]=@firstname,[lastname]=@lastname where [id]=@id", result);
        }

        // create command 

        [TestMethod]
        public void TestUdpate_CreateCommand_AddParameters()
        {
            var db = this.GetDb();
            var user = new User
            {
                Id = 2,
                FirstName = "updated firstname",
                LastName = "updated lastname",
            };

            var result = db
                 .Update("users")
                 .Set("firstname", user.FirstName)
                 .Set("lastname", user.LastName)
                 .Where(Check.Op("id", user.Id))
                 .CreateCommand();

            Assert.AreEqual("update [users] set [firstname]=@firstname,[lastname]=@lastname where [id]=@id",result.Command.CommandText);
            Assert.AreEqual(CommandType.Text, result.Command.CommandType);

            Assert.AreEqual("@firstname", result.Command.Parameters[0].ParameterName);
            Assert.AreEqual(user.FirstName, result.Command.Parameters[0].Value);

            Assert.AreEqual("@lastname", result.Command.Parameters[1].ParameterName);
            Assert.AreEqual(user.LastName, result.Command.Parameters[1].Value);

            Assert.AreEqual(3, result.Command.Parameters.Count);
            Assert.AreEqual("@id", result.Command.Parameters[2].ParameterName);
            Assert.AreEqual(user.Id, result.Command.Parameters[2].Value);
        }

        [TestMethod]
        public void TestUdpate_DontDuplicateParameter()
        {

            var db = this.GetDb();
            var user = new User
            {
                Id = 2,
                FirstName = "updated firstname",
                LastName = "updated lastname",
            };

            var result = db
                 .Update("users")
                 .Set("firstname", user.FirstName)
                 .Set("lastname", user.LastName)
                 .Where(Check.Op("lastname", "old value"))
                 .CreateCommand();

            Assert.AreEqual(2, result.Command.Parameters.Count);
        }

        [TestMethod]
        public void TestUdpate_CreateCommand_WithNullableAndString()
        {
            var db = this.GetDb();

            var user = new User
            {
                Id = 2,
                FirstName = "updated firstname",
                LastName = "updated lastname",
            };

            var result = db
                 .Update("users")
                 .Set("firstname", user.FirstName)
                 .Set("lastname", user.LastName)
                 .Set("age", user.Age)
                 .Set("email",user.Email)
                 .Where(Check.Op("id", user.Id))
                 .CreateCommand();

            Assert.AreEqual("update [users] set [firstname]=@firstname,[lastname]=@lastname,[age]=@age,[email]=@email where [id]=@id", result.Command.CommandText);
            Assert.AreEqual(CommandType.Text, result.Command.CommandType);

            Assert.AreEqual(5, result.Command.Parameters.Count);

            Assert.AreEqual("@firstname", result.Command.Parameters[0].ParameterName);
            Assert.AreEqual(user.FirstName, result.Command.Parameters[0].Value);

            Assert.AreEqual("@lastname", result.Command.Parameters[1].ParameterName);
            Assert.AreEqual(user.LastName, result.Command.Parameters[1].Value);

            Assert.AreEqual("@age", result.Command.Parameters[2].ParameterName);
            Assert.AreEqual(DBNull.Value, result.Command.Parameters[2].Value);

            Assert.AreEqual("@email", result.Command.Parameters[3].ParameterName);
            Assert.AreEqual(DBNull.Value, result.Command.Parameters[3].Value);

            Assert.AreEqual("@id", result.Command.Parameters[4].ParameterName);
            Assert.AreEqual(user.Id, result.Command.Parameters[4].Value);

        }

        // execute

        [TestMethod]
        public async Task TestUpdate()
        {
            var db = this.GetDb();

            var user = new User
            {
                Id = 2,
                FirstName = "updated firstname",
                LastName = "updated lastname",
            };

            var rowAffected = await db
                 .Update("users")
                 .Set("firstname", user.FirstName)
                 .Set("lastname", user.LastName)
                 .Set("age", user.Age)
                 .Set("email", user.Email)
                 .Where(Check.Op("id", user.Id))
                 .NonQueryAsync();

            Assert.AreEqual(1, rowAffected);

            var result = await db.CreateCommand("select * from users where id=@id")
                .AddParameter("@id", user.Id)
                .ReadOneAsync<UserLikeTable>();

            Assert.AreEqual(user.Id, result.id);
            Assert.AreEqual(user.FirstName, result.firstname);
            Assert.AreEqual(user.LastName, result.lastname);
            Assert.AreEqual(null, result.age);
            Assert.AreEqual(null, result.email);
        }       

       
    }
}
