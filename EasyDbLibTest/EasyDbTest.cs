using System;
using System.Data.Common;
using System.Threading.Tasks;
using EasyDbLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EasyDbLibTest
{
    [TestClass]
    public class EasyDbTest
    {
        private static string sqlServerConnectionString =
                @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\romag\Documents\Visual Studio 2017\Projects\EasyDbLib\EasyDbLibTest\SqlServerDb.mdf;Integrated Security=True;Connect Timeout=20"
            ;

        private static string sqlServerProviderName = "System.Data.SqlClient";

        [ClassInitialize()]
        public static void ClassInit(TestContext context)
        {
            var connection = DbProviderFactories.GetFactory(EasyDbTest.sqlServerProviderName).CreateConnection();
            connection.ConnectionString = EasyDbTest.sqlServerConnectionString;

            var command = connection.CreateCommand();
            connection.Open();
            command.CommandText = "DROP TABLE IF EXISTS dbo.people";
            command.ExecuteNonQuery();

            command.CommandText = @"CREATE TABLE [dbo].[people] (
                                        [id]    INT         IDENTITY (1, 1) NOT NULL,
                                        [first] NCHAR (255) NOT NULL,
                                        [last]  NCHAR (255) NOT NULL,
                                        [age]   INT         NULL,
                                        [email] NCHAR (100) NULL,
                                        PRIMARY KEY CLUSTERED ([Id] ASC)
                                    );";

            command.ExecuteNonQuery();
            connection.Close();
        }

        // get connection and open

        [TestMethod]
        public async Task TestGetNewConnection()
        {
            var db = new EasyDb();
            await db.GetNewConnection(sqlServerConnectionString, sqlServerProviderName)
                .OpenAsync();

            Assert.IsTrue(db.IsOpen());
        }

        [TestMethod]
        public async Task TestGetNewConnectionFromConfigFile_WithDefault()
        {
            var db = new EasyDb();
            await db.GetNewConnection()
                .OpenAsync();

            Assert.IsTrue(db.IsOpen());
        }

        [TestMethod]
        public async Task TestGetNewConnectionFromConfigFile_WithNamed()
        {
            var db = new EasyDb();
            await db.GetNewConnection("MyConnection")
                .OpenAsync();

            Assert.IsTrue(db.IsOpen());
        }

        // close

        [TestMethod]
        public async Task TestClose()
        {
            var db = new EasyDb();
            await db.GetNewConnection(sqlServerConnectionString, sqlServerProviderName)
                .OpenAsync();

            Assert.IsTrue(db.IsOpen());

            db.Close();

            Assert.IsFalse(db.IsOpen());
            Assert.IsTrue(db.IsClosed());
        }

        // read one

        [TestMethod]
        public async Task TestReadOneAsync_PropertyNotInTableAreIgnored()
        {
            var db = new EasyDb();
            db.GetNewConnection(sqlServerConnectionString, sqlServerProviderName);

            var result = await db.CreateCommand("select * from users where id=@id")
                .AddParameter("@id", 11)
                .ReadOneAsync<UserWithIgnoredProperty>();

            Assert.AreEqual(null, result.Age);
            Assert.AreEqual(11, result.Id);
            Assert.AreEqual("Marie", result.Name);
            Assert.AreEqual("marie@mail.com", result.Email);
            Assert.AreEqual(null, result.MyIgnoredProperty);
        }

        [TestMethod]
        public async Task TestReadOneAsync_ColumnNotInClassIsIgnored()
        {
            var db = new EasyDb();
            db.GetNewConnection(sqlServerConnectionString, sqlServerProviderName);

            var result = await db.CreateCommand("select * from users where id=@id")
                .AddParameter("@id", 11)
                .ReadOneAsync<UserWithIgnoredColumns>();

            Assert.AreEqual(11, result.Id);
            Assert.AreEqual("Marie", result.Name);
        }

        [TestMethod]
        public async Task TestReadOneAsync_WithNullableAndDbNull_ReturnsValuesAndNull()
        {
            var db = new EasyDb();
            db.GetNewConnection(sqlServerConnectionString, sqlServerProviderName);

            var result = await db.CreateCommand("select * from users where id=@id")
                .AddParameter("@id", 11)
                .ReadOneAsync<User>();

            Assert.AreEqual(null, result.Age);
            Assert.AreEqual(11, result.Id);
            Assert.AreEqual("Marie", result.Name);
            Assert.AreEqual("marie@mail.com", result.Email);
        }

        [TestMethod]
        public async Task TestReadOneAsync_WithNullableAndNotDbNull_ReturnsValues()
        {
            var db = new EasyDb();
            db.GetNewConnection(sqlServerConnectionString, sqlServerProviderName);

            var result = await db.CreateCommand("select * from users where id=@id")
                .AddParameter("@id", 12)
                .ReadOneAsync<User>();

            Assert.AreEqual(20, result.Age);
            Assert.AreEqual(12, result.Id);
            Assert.AreEqual("Pat", result.Name);
            Assert.AreEqual(null, result.Email);
        }

        [TestMethod]
        public async Task TestReadOneAsync_ConnectionIsClosed()
        {
            var db = new EasyDb();
            db.GetNewConnection(sqlServerConnectionString, sqlServerProviderName);

            var result = await db.CreateCommand("select * from users where id=@id")
                .AddParameter("@id", 12)
                .ReadOneAsync<User>();

            Assert.IsTrue(db.IsClosed());
        }

       

        // read all

        [TestMethod]
        public async Task TestReadAllAsync()
        {
            var db = new EasyDb();
            db.GetNewConnection(sqlServerConnectionString, sqlServerProviderName);

            var result = await db.CreateCommand("select * from users")
                 .ReadAllAsync<User>();

            Assert.AreEqual(2, result.Count);

            Assert.AreEqual(null, result[0].Age);
            Assert.AreEqual(11, result[0].Id);
            Assert.AreEqual("Marie", result[0].Name);
            Assert.AreEqual("marie@mail.com", result[0].Email);

            Assert.AreEqual(20, result[1].Age);
            Assert.AreEqual(12, result[1].Id);
            Assert.AreEqual("Pat", result[1].Name);
            Assert.AreEqual(null, result[1].Email);
        }

        [TestMethod]
        public async Task TestReadAllAsync_ConnectionIsClosed()
        {
            var db = new EasyDb();
            db.GetNewConnection(sqlServerConnectionString, sqlServerProviderName);

            var result = await db.CreateCommand("select * from users")
                .ReadAllAsync<User>();

            Assert.IsTrue(db.IsClosed());
        }

        // non query

        [TestMethod]
        public async Task TestNonQueryAsyncWithStringNull()
        {
            var db = new EasyDb();
            db.GetNewConnection(sqlServerConnectionString, sqlServerProviderName);

            var user = new P
            {
                first = "Marie",
                last = "Bellin",
                age = 20
            };

            var rowAffected = await db.CreateCommand("insert into [dbo].[people](first,last,age,email) values(@first,@last,@age,@email)")
                .AddParameter("@first", user.first)
                .AddParameter("@last", user.last)
                 .AddParameter("@age",user.age)
                .AddParameter("@email", user.email)
                .NonQueryAsync();

            Assert.AreEqual(1, rowAffected);

            var result = await db
                .CreateCommand("select * from people where id=@id")
                .AddParameter("@id", 1) // id 1
                .ReadOneAsync<P>();

            Assert.AreEqual(1, result.id);
            Assert.AreEqual(user.first, result.first);
            Assert.AreEqual(user.last, result.last);
            Assert.AreEqual(user.age, result.age);
            Assert.IsNull(result.email);
        }


        // scalar

        [TestMethod]
        public async Task TestScalarAsync()
        {
            var db = new EasyDb();
            db.GetNewConnection(sqlServerConnectionString, sqlServerProviderName);


            var user = new P
            {
                first = "Pat",
                last = "Prem",
                email = "pat@email.com"
            };

            var id = (int)await db.CreateCommand("insert into [dbo].[people](first,last,age,email) output INSERTED.Id values(@first,@last,@age,@email)")
                .AddParameter("@first", user.first)
                .AddParameter("@last", user.last)
                .AddParameter("@age", user.age)
                .AddParameter("@email", user.email)
                .ScalarAsync();

            Assert.AreEqual(2, id);

            var result = await db
                .CreateCommand("select * from people where id=@id")
                .AddParameter("@id", id) 
                .ReadOneAsync<P>();

            Assert.AreEqual(user.first, result.first);
            Assert.AreEqual(user.last, result.last);
            Assert.IsNull(result.age);
            Assert.AreEqual(user.email, result.email);
        }


        // handle exceptions

        [TestMethod]
        public async Task HandleExceptionOnConnect()
        {
            bool isCalled = false;
            EasyDbErrorEventArgs ex = null;
            var db = new EasyDb();
            db.GetNewConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\file\not\found.mdf;Integrated Security=True;Connect Timeout=20", sqlServerProviderName);

            db.OnError += (sender, e) =>
            {
                isCalled = true;
                ex = e;
            };

            await db.OpenAsync();

            Assert.IsTrue(isCalled);
            Assert.IsNotNull(ex);
            Assert.IsNotNull(ex.Exception);
            Assert.IsNotNull(ex.Connection);
            Assert.AreEqual(When.Open, ex.When);
        }

        [TestMethod]
        public async Task HandleExceptionReadAll()
        {
            bool isCalled = false;
            EasyDbErrorEventArgs ex = null;
            var db = new EasyDb();
            db.GetNewConnection(sqlServerConnectionString, sqlServerProviderName);

            db.OnError += (sender, e) =>
            {
                isCalled = true;
                ex = e;
            };

            await db.CreateCommand("select * notfound")
                .ReadAllAsync<P>();

            Assert.IsTrue(isCalled);
            Assert.IsNotNull(ex);
            Assert.IsNotNull(ex.Exception);
            Assert.IsNotNull(ex.Connection);
            Assert.IsNotNull(ex.Command);
            Assert.AreEqual(When.ReadAll, ex.When);
        }


        [TestMethod]
        public async Task HandleExceptionReadOne()
        {
            bool isCalled = false;
            EasyDbErrorEventArgs ex = null;
            var db = new EasyDb();
            db.GetNewConnection(sqlServerConnectionString, sqlServerProviderName);

            db.OnError += (sender, e) =>
            {
                isCalled = true;
                ex = e;
            };

            await db.CreateCommand("select * notfound")
                .ReadOneAsync<P>();

            Assert.IsTrue(isCalled);
            Assert.IsNotNull(ex);
            Assert.IsNotNull(ex.Exception);
            Assert.IsNotNull(ex.Connection);
            Assert.IsNotNull(ex.Command);
            Assert.AreEqual(When.ReadOne, ex.When);
        }

        [TestMethod]
        public async Task HandleExceptionNonQuery()
        {
            bool isCalled = false;
            EasyDbErrorEventArgs ex = null;
            var db = new EasyDb();
            db.GetNewConnection(sqlServerConnectionString, sqlServerProviderName);

            db.OnError += (sender, e) =>
            {
                isCalled = true;
                ex = e;
            };

            await db.CreateCommand("insert into notfound(name) values (@name)")
                .AddParameter("@name","Marie")
                .NonQueryAsync();

            Assert.IsTrue(isCalled);
            Assert.IsNotNull(ex);
            Assert.IsNotNull(ex.Exception);
            Assert.IsNotNull(ex.Connection);
            Assert.IsNotNull(ex.Command);
            Assert.AreEqual(When.NonQuery, ex.When);
        }

        [TestMethod]
        public async Task HandleExceptionScalar()
        {
            bool isCalled = false;
            EasyDbErrorEventArgs ex = null;
            var db = new EasyDb();
            db.GetNewConnection(sqlServerConnectionString, sqlServerProviderName);

            db.OnError += (sender, e) =>
            {
                isCalled = true;
                ex = e;
            };

            await db.CreateCommand("select count(*) from notfound where user_id=@user_id")
                .AddParameter("@user_id", 1)
                .ScalarAsync();

            Assert.IsTrue(isCalled);
            Assert.IsNotNull(ex);
            Assert.IsNotNull(ex.Exception);
            Assert.IsNotNull(ex.Connection);
            Assert.IsNotNull(ex.Command);
            Assert.AreEqual(When.Scalar, ex.When);
        }

        [TestMethod]
        public async Task TestThrowException_IfNoErrorSubscribers()
        {
            bool failed = false;
            var db = new EasyDb();
            db.GetNewConnection(sqlServerConnectionString, sqlServerProviderName);

            try
            {
                await db.CreateCommand("select count(*) from notfound where user_id=@user_id")
                    .AddParameter("@user_id", 1)
                    .ScalarAsync();

            }
            catch (Exception e)
            {
                failed = true;
            }

            Assert.IsTrue(failed);
        }

        // strategy

        [TestMethod]
        public async Task TestWithManualStrategy_DontOpenConnection()
        {
            bool failed = false;
            var db = new EasyDb();
            db.GetNewConnection(sqlServerConnectionString, sqlServerProviderName, ConnectionStrategy.Manual);

            try
            {
                var result = await db.CreateCommand("select * from users where id=@id")
                    .AddParameter("@id", 11)
                    .ReadOneAsync<UserWithIgnoredProperty>();
            }
            catch (Exception e)
            {
                failed = true;
            }

            Assert.IsTrue(failed);
        }

        [TestMethod]
        public async Task TestWithManualStrategy_DontCloseConnection()
        {
            var db = new EasyDb();
            db.GetNewConnection(sqlServerConnectionString, sqlServerProviderName, ConnectionStrategy.Manual);

           await db.OpenAsync();

            var result = await db.CreateCommand("select * from people").ReadAllAsync<User>();

            Assert.IsFalse(db.IsClosed());
        }

        [TestMethod]
        public async Task TestWitDefaultStrategy_OpenAndClose()
        {
            var db = new EasyDb();
            db.GetNewConnection(sqlServerConnectionString, sqlServerProviderName);

            var result = await db.CreateCommand("select * from users where id=@id")
                .AddParameter("@id", 11)
                .ReadOneAsync<UserWithIgnoredProperty>();

            Assert.IsTrue(db.IsClosed());
        }

        [TestMethod]
        public async Task TestCouldChangeStrategy()
        {
            var db = new EasyDb();
            db.GetNewConnection(sqlServerConnectionString, sqlServerProviderName,ConnectionStrategy.Manual);

            await db.OpenAsync();

            var result = await db.CreateCommand("select * from users where id=@id")
                .AddParameter("@id", 11)
                .ReadOneAsync<UserWithIgnoredProperty>();

            db.Close();

            db.ChangeConnectionStrategy(ConnectionStrategy.Default);

            result = await db.CreateCommand("select * from users where id=@id")
                .AddParameter("@id", 11)
                .ReadOneAsync<UserWithIgnoredProperty>();

            Assert.IsTrue(db.IsClosed());
        }

        [TestMethod]
        public async Task TestDontOpen_IfConnectionIsOpened()
        {
            var db = new EasyDb();
            db.GetNewConnection(sqlServerConnectionString, sqlServerProviderName);

            await db.OpenAsync();

            var result = await db.CreateCommand("select * from people")
                .ReadAllAsync<User>();
         

            Assert.IsTrue(db.IsClosed());
        }

        // stored

        [TestMethod]
        public async Task TestCreateStoredProcedure()
        {
            var db = new EasyDb();
            db.GetNewConnection(sqlServerConnectionString, sqlServerProviderName);

            var result = await db.CreateStoredProcedureCommand("GetUser")
                .AddParameter("@id", 11)
                .ReadOneAsync<User>();

            Assert.AreEqual("Marie", result.Name);
            Assert.AreEqual("marie@mail.com", result.Email);
        }
    }

    public class P
    {
        public int id { get; set; }
        public string first { get; set; }
        public string last { get; set; }
        public string email { get; set; }
        public int? age { get; set; }
    }

    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int? Age { get; set; }
    }

    public class UserWithIgnoredProperty
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int? Age { get; set; }
        public string MyIgnoredProperty { get; set; }
    }

    public class UserWithIgnoredColumns
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

}
