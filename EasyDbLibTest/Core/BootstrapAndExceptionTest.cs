using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EasyDbLib;
using System.Threading.Tasks;

namespace EasyDbLibTest.Core
{
    [TestClass]
    public class BootstrapAndExceptionTest
    {
        [ClassInitialize()]
        public static void ClassInit(TestContext context)
        {
            InitDb.CreateDbTest();
        }


        // bootstrap 


        [TestMethod]
        public void TestIsNotInitialized_WithoutConnectionString_Settings()
        {
            var db = new EasyDb();
            Assert.IsFalse(db.IsInitialized());
        }

        [TestMethod]
        public void TestIsInitialized_WithConnectionStringSettings()
        {
            var db = new EasyDb();
            db.SetConnectionStringSettings(InitDb.SqlConnectionString, InitDb.SqlProviderName);
            Assert.IsTrue(db.IsInitialized());
        }

        // throw exception

        [TestMethod]
        public void TestThrowException_WithoutConnectionStringSettingsOnCreateCommand()
        {
            bool failed = false;
            string message = "";
            var db = new EasyDb();
            try
            {
                db.CreateCommand("select * from [posts]");
            }
            catch (Exception e)
            {
                failed = true;
                message = e.Message;
            }
            Assert.IsTrue(failed);
            Assert.AreEqual("No connection string settings provided", message);
        }

        [TestMethod]
        public async Task TestThrowException_WithoutConnectionStringSettingsOnOpen()
        {
            bool failed = false;
            string message = "";
            var db = new EasyDb();
            try
            {
                await db.OpenAsync();
            }
            catch (Exception e)
            {
                failed = true;
                message = e.Message;
            }
            Assert.IsTrue(failed);
            Assert.AreEqual("No connection string settings provided", message);
        }

        [TestMethod]
        public void TestThrowException_WithoutConnectionStringSettingsOnClose()
        {
            bool failed = false;
            string message = "";
            var db = new EasyDb();
            try
            {
                db.Close();
            }
            catch (Exception e)
            {
                failed = true;
                message = e.Message;
            }
            Assert.IsTrue(failed);
            Assert.AreEqual("No connection string settings provided", message);
        }

        // handle exception

        [TestMethod]
        public void TestOnError_WithoutConnectionStringSettings_HandleExceptionOnCreateCommand()
        {
            bool isCalled = false;
            string message = "";
            EasyDbErrorEventArgs error = null;

            var db = new EasyDb();
            db.OnError += (sender, e) =>
            {
                isCalled = true;
                message = e.Message;
                error = e;
            };

            db.CreateCommand("select * from [posts]");
            Assert.IsTrue(isCalled);
            Assert.AreEqual("No connection string settings provided", message);
            Assert.AreEqual(When.CreateCommand, error.When);
            Assert.IsNull(error.Connection);
            Assert.IsNull(error.Command);
        }


        [TestMethod]
        public async Task TestOnError_WithoutConnectionStringSettings_HandleExceptionOnOpen()
        {
            bool isCalled = false;
            string message = "";
            EasyDbErrorEventArgs error = null;
            var db = new EasyDb();
            db.OnError += (sender, e) =>
            {
                isCalled = true;
                message = e.Message;
                error = e;
            };

            await db.OpenAsync();
            Assert.IsTrue(isCalled);
            Assert.AreEqual("No connection string settings provided", message);
            Assert.AreEqual(When.Open, error.When);
            Assert.IsNull(error.Connection);
            Assert.IsNull(error.Command);
        }

        [TestMethod]
        public void TestOnError_WithoutConnectionStringSettings_HandleExceptionOnClose()
        {
            bool isCalled = false;
            string message = "";
            EasyDbErrorEventArgs error = null;
            var db = new EasyDb();
            db.OnError += (sender, e) =>
            {
                isCalled = true;
                message = e.Message;
                error = e;
            };

            db.Close();
            Assert.IsTrue(isCalled);
            Assert.AreEqual("No connection string settings provided", message);
            Assert.AreEqual(When.Close, error.When);
            Assert.IsNull(error.Connection);
            Assert.IsNull(error.Command);
        }

        // connection

        [TestMethod]
        public void TestHandleExceptionOnConfigConnectionString()
        {
            bool isCalled = false;
            string message = "";
            EasyDbErrorEventArgs error = null;
            var db = new EasyDb();
            db.OnError += (sender, e) =>
            {
                isCalled = true;
                message = e.Message;
                error = e;
            };
            db.SetConnectionStringSettings("invalid", "invalid");
            Assert.IsTrue(isCalled);
            Assert.IsNotNull(message);
            Assert.AreEqual(When.Config, error.When);
            Assert.IsNull(error.Connection);
            Assert.IsNull(error.Command);
        }

        [TestMethod]
        public void TestHandleExceptionOnConfigConnectionString_WithConfurationFile()
        {
            bool isCalled = false;
            string message = "";
            EasyDbErrorEventArgs error = null;
            var db = new EasyDb();
            db.OnError += (sender, e) =>
            {
                isCalled = true;
                message = e.Message;
                error = e;
            };
            db.SetConnectionStringSettings("invalid");
            Assert.IsTrue(isCalled);
            Assert.IsNotNull(message);
            Assert.AreEqual(When.Config, error.When);
            Assert.IsNull(error.Connection);
            Assert.IsNull(error.Command);
        }

        // open

        [TestMethod]
        public async Task TestGetNewConnection()
        {
            var db = new EasyDb();
            await db.SetConnectionStringSettings(InitDb.SqlConnectionString, InitDb.SqlProviderName)
                .OpenAsync();

            Assert.IsTrue(db.IsOpen());
        }

        [TestMethod]
        public async Task TestGetNewConnectionFromConfigFile_WithDefault()
        {
            var db = new EasyDb();
            await db.SetConnectionStringSettings()
                .OpenAsync();

            Assert.IsTrue(db.IsOpen());
        }

        [TestMethod]
        public async Task TestGetNewConnectionFromConfigFile_WithNamed()
        {
            var db = new EasyDb();
            await db.SetConnectionStringSettings("MyConnection")
                .OpenAsync();

            Assert.IsTrue(db.IsOpen());
        }

        // close

        [TestMethod]
        public async Task TestClose()
        {
            var db = new EasyDb();
            await db.SetConnectionStringSettings(InitDb.SqlConnectionString, InitDb.SqlProviderName)
                .OpenAsync();

            Assert.IsTrue(db.IsOpen());

            db.Close();

            Assert.IsFalse(db.IsOpen());
            Assert.IsTrue(db.IsClosed());
        }


        // create command

        [TestMethod]
        public void TestCreateCommand_WithWhitespace_ThrowException()
        {
            bool failed = false;
            string message = "";
            var db = new EasyDb();
            db.SetConnectionStringSettings(InitDb.SqlConnectionString, InitDb.SqlProviderName);

            try
            {
                db.CreateCommand("  ");
            }
            catch (Exception e)
            {
                failed = true;
                message = e.Message;
            }
            Assert.IsTrue(failed);
            Assert.AreEqual("Invalid command text", message);
        }

        [TestMethod]
        public void TestCreateCommand_WithNull_ThrowException()
        {
            bool failed = false;
            string message = "";
            var db = new EasyDb();
            db.SetConnectionStringSettings(InitDb.SqlConnectionString, InitDb.SqlProviderName);

            try
            {
                db.CreateCommand(null);
            }
            catch (Exception e)
            {
                failed = true;
                message = e.Message;
            }
            Assert.IsTrue(failed);
            Assert.AreEqual("Invalid command text", message);
        }

        // handle

        [TestMethod]
        public void TestCreateCommand_WithWhitespace_HandleException()
        {
            bool isCalled = false;
            string message = "";
            EasyDbErrorEventArgs error = null;
            var db = new EasyDb();
            db.SetConnectionStringSettings(InitDb.SqlConnectionString, InitDb.SqlProviderName);
            db.OnError += (sender, e) =>
            {
                isCalled = true;
                message = e.Message;
                error = e;
            };

            db.CreateCommand("  ");
            Assert.IsTrue(isCalled);
            Assert.AreEqual("Invalid command text", message);
            Assert.AreEqual(When.CreateCommand, error.When);
            Assert.IsNotNull(error.Connection);
            Assert.IsNull(error.Command);
        }

        [TestMethod]
        public void TestCreateCommand_WithNull_HandleException()
        {
            bool isCalled = false;
            string message = "";
            EasyDbErrorEventArgs error = null;
            var db = new EasyDb();
            db.SetConnectionStringSettings(InitDb.SqlConnectionString, InitDb.SqlProviderName);
            db.OnError += (sender, e) =>
            {
                isCalled = true;
                message = e.Message;
                error = e;
            };

            db.CreateCommand(null);
            Assert.IsTrue(isCalled);
            Assert.AreEqual("Invalid command text", message);
            Assert.AreEqual(When.CreateCommand, error.When);
            Assert.IsNotNull(error.Connection);
            Assert.IsNull(error.Command);
        }
    }
}
