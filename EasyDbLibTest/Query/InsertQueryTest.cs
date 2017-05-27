using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EasyDbLib;
using System.Threading.Tasks;

namespace EasyDbLibTest.Query
{
    [TestClass]
    public class InsertQueryTest
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
        public void TestFailWithNoColumns()
        {
            bool failed = false;

            var db = this.GetDb();

            try
            {
                var result = db.InsertInto("people").GetQuery();
            }
            catch (Exception)
            {
                failed = true;
            }

            Assert.IsTrue(failed);
        }

        // query

        [TestMethod]
        public void TestQuery()
        {
            var db = this.GetDb();

            var user = new User
            {
                FirstName = "new firstname",
                LastName = "new lastname",
                Age = 20,
                Email ="new@domain.com"
            };

            var result = db.InsertInto("users")
                .Values("firstname", user.FirstName)
                .Values("lastname", user.LastName)
                .Values("age", user.Age)
                .Values("email", user.Email)
                .GetQuery();

            Assert.AreEqual("insert into [users] ([firstname],[lastname],[age],[email]) output inserted.id values (@firstname,@lastname,@age,@email)", result);
        }

        [TestMethod]
        public void TestQuery_WithNoInsertedId()
        {
            var db = this.GetDb();

            var user = new User
            {
                FirstName = "new firstname",
                LastName = "new lastname",
                Age = 20,
                Email = "new@domain.com"
            };

            var result = db.InsertInto("users")
                .Values("firstname", user.FirstName)
                .Values("lastname", user.LastName)
                .Values("age", user.Age)
                .Values("email", user.Email)
                .GetQuery(false);

            Assert.AreEqual("insert into [users] ([firstname],[lastname],[age],[email]) values (@firstname,@lastname,@age,@email)", result);
        }

        // create command

        [TestMethod]
        public void TestCreateCommand()
        {
            var db = this.GetDb();

            var user = new User
            {
                FirstName = "new firstname",
                LastName = "new lastname",
                Age = 20,
                Email = "new@domain.com"
            };

            var result = db.InsertInto("users")
                .Values("firstname", user.FirstName)
                .Values("lastname", user.LastName)
                .Values("age", user.Age)
                .Values("email", user.Email)
                .CreateCommand();

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

        [TestMethod]
        public void TestCreateCommand_WithNullableAndString()
        {
            var db = this.GetDb();

            var user = new User
            {
                FirstName = "new firstname",
                LastName = "new lastname"
            };

            var result = db.InsertInto("users")
                .Values("firstname", user.FirstName)
                .Values("lastname", user.LastName)
                .Values("age", user.Age)
                .Values("email", user.Email)
                .CreateCommand();

            Assert.AreEqual(4, result.Command.Parameters.Count);

            Assert.AreEqual("@firstname", result.Command.Parameters[0].ParameterName);
            Assert.AreEqual(user.FirstName, result.Command.Parameters[0].Value);

            Assert.AreEqual("@lastname", result.Command.Parameters[1].ParameterName);
            Assert.AreEqual(user.LastName, result.Command.Parameters[1].Value);

            Assert.AreEqual("@age", result.Command.Parameters[2].ParameterName);
            Assert.AreEqual(DBNull.Value, result.Command.Parameters[2].Value);

            Assert.AreEqual("@email", result.Command.Parameters[3].ParameterName);
            Assert.AreEqual(DBNull.Value, result.Command.Parameters[3].Value);
        }

        // non query

        [TestMethod]
        public async Task TestNonQueryAsync()
        {
            var db = this.GetDb();

            var user = new User
            {
                FirstName = "new firstname",
                LastName = "new lastname",
                Age = 20,
                Email = "new@domain.com"
            };

            var result = await db.InsertInto("users")
                .Values("firstname", user.FirstName)
                .Values("lastname", user.LastName)
                .Values("age", user.Age)
                .Values("email", user.Email)
                .NonQueryAsync();

            Assert.AreEqual(1, result);
        }

        // last inserted id


        [TestMethod]
        public async Task TestLastInsertedId_WithInt()
        {
            var db = this.GetDb();

            var user = new User
            {
                FirstName = "new firstname",
                LastName = "new lastname",
                Age = 20,
                Email = "new@domain.com"
            };

            var result = await db.InsertInto("users")
                .Values("firstname", user.FirstName)
                .Values("lastname", user.LastName)
                .Values("age", user.Age)
                .Values("email", user.Email)
                .LastInsertedId<int>();

            Assert.IsTrue(result > 2);
        }

        [TestMethod]
        public async Task TestLastInsertedId_WithGuid()
        {
            var db = this.GetDb();

            var result = await db
                .InsertInto("UsersWithGuid")
                .Values("name", "test first")
                .Values("email", "test@domain.com")
                .LastInsertedId<Guid>();

            Assert.IsTrue(Guid.TryParse(result.ToString(), out Guid check)); 
        }

        // fetch

        [TestMethod]
        public async Task TestFetch()
        {
            var db = this.GetDb();

            Mapping.SetTable("users").SetPrimaryKeyColumn("id", "id");

            var user = new User
            {
                FirstName = "new firstname",
                LastName = "new lastname",
                Age = 20,
                Email = "new@domain.com"
            };

            var result = await db.InsertInto("users")
                .Values("firstname", user.FirstName)
                .Values("lastname", user.LastName)
                .Values("age", user.Age)
                .Values("email", user.Email)
                .Fetch<UserLikeTable>(Mapping.GetTable("users"));

            Assert.IsNotNull(result);
            Assert.IsTrue(result.id > 2);
            Assert.AreEqual(user.FirstName, result.firstname);
            Assert.AreEqual(user.LastName, result.lastname);
            Assert.AreEqual(user.Age, result.age);
            Assert.AreEqual(user.Email, result.email);
        }

        [TestMethod]
        public async Task TestWithNullable()
        {
            var db = this.GetDb();

            Mapping.SetTable("users").SetPrimaryKeyColumn("id", "id");

            var user = new User
            {
                FirstName = "new firstname",
                LastName = "new lastname",
            };

            var result = await db.InsertInto("users")
                .Values("firstname", user.FirstName)
                .Values("lastname", user.LastName)
                .Values("age", user.Age)
                .Values("email", user.Email)
                .Fetch<UserLikeTable>(Mapping.GetTable("users"));

            Assert.IsNotNull(result);
            Assert.IsTrue(result.id > 2);
            Assert.AreEqual(user.FirstName, result.firstname);
            Assert.AreEqual(user.LastName, result.lastname);
            Assert.AreEqual(null, result.age);
            Assert.AreEqual(null, result.email);
        }

        // with mapping

        [TestMethod]
        public async Task TestFetch_WithMapping()
        {
            var db = this.GetDb();

            Mapping.SetTable("users")
                .SetPrimaryKeyColumn("id", "Id")
                .SetColumn("firstname","FirstName")
                .SetColumn("lastname", "LastName")
                .SetColumn("age", "Age")
                .SetColumn("email", "Email");

            var user = new User
            {
                FirstName = "new firstname",
                LastName = "new lastname",
                Age = 20,
                Email = "new@domain.com"
            };

            var result = await db.InsertInto("users")
                .Values("firstname", user.FirstName)
                .Values("lastname", user.LastName)
                .Values("age", user.Age)
                .Values("email", user.Email)
                .Fetch<User>(Mapping.GetTable("users"));

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Id > 2);
            Assert.AreEqual(user.FirstName, result.FirstName);
            Assert.AreEqual(user.LastName, result.LastName);
            Assert.AreEqual(user.Age, result.Age);
            Assert.AreEqual(user.Email, result.Email);
        }

    }
}
