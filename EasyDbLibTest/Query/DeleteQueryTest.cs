using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EasyDbLib;
using System.Threading.Tasks;
using System.Data;

namespace EasyDbLibTest.Query
{
    [TestClass]
    public class DeleteQueryTest
    {

        [ClassInitialize()]
        public static void ClassInit(TestContext context)
        {
            InitDb.CreateDbTest();
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

            try
            {
                var result = db
                 .DeleteFrom("users")
                 .Where(Check.Op("id", 2))
                 .Where(Check.Op("id",3))
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
        public void TestQuery()
        {
            var db = this.GetDb();

            var result = db
                .DeleteFrom("users")
                .Where(Check.Op("id", 2))
                .GetQuery();

            Assert.AreEqual("delete from [users] where [id]=@id", result);
        }


        [TestMethod]
        public void TesWhithSameColumNames()
        {
            var db = this.GetDb();

            var result = db
                .DeleteFrom("users")
                .Where(Check.Op("id", 3).Or(Check.Op("id", 4)))
                .GetQuery();

            Assert.AreEqual("delete from [users] where [id]=@id or [id]=@id2", result); ;
        }

        // create command

        [TestMethod]
        public void TestCreateCommand()
        {
            var db = this.GetDb();

            var result = db
                .DeleteFrom("users")
                .Where(Check.Op("id", 2))
                .CreateCommand();

            Assert.AreEqual("delete from [users] where [id]=@id", result.Command.CommandText);
            Assert.AreEqual(CommandType.Text, result.Command.CommandType);

            Assert.AreEqual(1, result.Command.Parameters.Count);

            Assert.AreEqual("@id", result.Command.Parameters[0].ParameterName);
            Assert.AreEqual(2, result.Command.Parameters[0].Value);
        }

        [TestMethod]
        public void TestCreateCommand_WithSameParameters_DontDuplicate()
        {
            var db = this.GetDb();

            var result = db
                .DeleteFrom("users")
                .Where(Check.Op("id", 3).Or(Check.Op("id", 4)))
                .CreateCommand();

            Assert.AreEqual("delete from [users] where [id]=@id or [id]=@id2", result.Command.CommandText);
            Assert.AreEqual(CommandType.Text, result.Command.CommandType);

            Assert.AreEqual(2, result.Command.Parameters.Count);

            Assert.AreEqual("@id", result.Command.Parameters[0].ParameterName);
            Assert.AreEqual(3, result.Command.Parameters[0].Value);

            Assert.AreEqual("@id2", result.Command.Parameters[1].ParameterName);
            Assert.AreEqual(4, result.Command.Parameters[1].Value);
        }

        // execute

        [TestMethod]
        public async Task TestDelete()
        {
            var db = this.GetDb();

            var result = await db
                .DeleteFrom("posts")
                .Where(Check.Op("id", 2))
                .NonQueryAsync();

            Assert.AreEqual(1, result);
        }
    }
}
