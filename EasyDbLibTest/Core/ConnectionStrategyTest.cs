using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using EasyDbLib;

namespace EasyDbLibTest.Core
{
    [TestClass]
    public class ConnectionStrategyTest
    {

        [ClassInitialize()]
        public static void ClassInit(TestContext context)
        {
            InitDb.CreateDbTest();
            Mapping.Clear();
        }

        // read one

        [TestMethod]
        public async Task TestReadOneAsync_ConnectionIsClosed()
        {
            var db = new EasyDb();
            db.SetConnectionStringSettings(InitDb.SqlConnectionString, InitDb.SqlProviderName);

            var result = await db.CreateCommand("select * from users where id=@id")
                .AddParameter("@id", 1)
                .ReadOneAsync<UserLikeTable>();

            Assert.IsTrue(db.IsClosed());
        }

        [TestMethod]
        public async Task TestReadOneAsync_ConnectionIsNotOpen()
        {
            bool failed = true;
            var db = new EasyDb();
            db.SetConnectionStringSettings(InitDb.SqlConnectionString, InitDb.SqlProviderName, ConnectionStrategy.Manual);


            try
            {
                var result = await db.CreateCommand("select * from users where id=@id")
                    .AddParameter("@id", 1)
                    .ReadOneAsync<UserLikeTable>();
            }
            catch (Exception)
            {
                failed = true;
            }
            Assert.IsTrue(failed);
        }


        [TestMethod]
        public async Task TestReadOneAsync_WithManual_ConnectionIsNotClosed()
        {
            var db = new EasyDb();
            db.SetConnectionStringSettings(InitDb.SqlConnectionString, InitDb.SqlProviderName, ConnectionStrategy.Manual);

            await db.OpenAsync();

            var result = await db.CreateCommand("select * from users where id=@id")
                  .AddParameter("@id", 1)
                  .ReadOneAsync<UserLikeTable>();

            Assert.IsFalse(db.IsClosed());
        }

        [TestMethod]
        public async Task TestWithManualStrategy_DontOpenConnection()
        {
            bool failed = false;
            var db = new EasyDb();
            db.SetConnectionStringSettings(InitDb.SqlConnectionString, InitDb.SqlProviderName, ConnectionStrategy.Manual);

            try
            {
                var result = await db.CreateCommand("select * from users").ReadAllAsync<UserLikeTable>();
            }
            catch (Exception e)
            {
                failed = true;
            }

            Assert.IsTrue(failed);
        }

        // strategy

        [TestMethod]
        public async Task TestWithManualStrategy_DontCloseConnection()
        {
            var db = new EasyDb();
            db.SetConnectionStringSettings(InitDb.SqlConnectionString, InitDb.SqlProviderName, ConnectionStrategy.Manual);

            await db.OpenAsync();

            var result = await db.CreateCommand("select * from users").ReadAllAsync<UserLikeTable>();

            Assert.IsFalse(db.IsClosed());
        }

        [TestMethod]
        public async Task TestWitDefaultStrategy_OpenAndClose()
        {
            var db = new EasyDb();
            db.SetConnectionStringSettings(InitDb.SqlConnectionString, InitDb.SqlProviderName);

            var result = await db.CreateCommand("select * from users")
                .ReadAllAsync<UserLikeTable>();

            Assert.IsTrue(db.IsClosed());
        }

        // change strategy

        [TestMethod]
        public async Task TestCouldChangeStrategy()
        {
            var db = new EasyDb();
            db.SetConnectionStringSettings(InitDb.SqlConnectionString, InitDb.SqlProviderName, ConnectionStrategy.Manual);

            await db.OpenAsync();

            var result = await db.CreateCommand("select * from users")
                .ReadAllAsync<UserLikeTable>();

            db.Close();

            db.SetConnectionStrategy(ConnectionStrategy.Default);

            result = await db.CreateCommand("select * from users")
               .ReadAllAsync<UserLikeTable>();

            Assert.IsTrue(db.IsClosed());
        }

        [TestMethod]
        public async Task TestDontOpen_IfConnectionIsOpened()
        {
            var db = new EasyDb();
            db.SetConnectionStringSettings(InitDb.SqlConnectionString, InitDb.SqlProviderName);

            await db.OpenAsync();

            var result = await db.CreateCommand("select * from users")
                .ReadAllAsync<UserLikeTable>();


            Assert.IsTrue(db.IsClosed());
        }


        // handle exceptions on read write commands methods

        [TestMethod]
        public async Task HandleExceptionOnConnect()
        {
            bool isCalled = false;
            EasyDbErrorEventArgs ex = null;
            var db = new EasyDb();
            db.SetConnectionStringSettings(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\file\not\found.mdf;Integrated Security=True;Connect Timeout=20", InitDb.SqlProviderName);

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
            db.SetConnectionStringSettings(InitDb.SqlConnectionString, InitDb.SqlProviderName);

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
            db.SetConnectionStringSettings(InitDb.SqlConnectionString, InitDb.SqlProviderName);

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
            db.SetConnectionStringSettings(InitDb.SqlConnectionString, InitDb.SqlProviderName);

            db.OnError += (sender, e) =>
            {
                isCalled = true;
                ex = e;
            };

            await db.CreateCommand("insert into notfound(name) values (@name)")
                .AddParameter("@name", "Marie")
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
            db.SetConnectionStringSettings(InitDb.SqlConnectionString, InitDb.SqlProviderName);

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
            db.SetConnectionStringSettings(InitDb.SqlConnectionString, InitDb.SqlProviderName);

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

    }
}
