using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EasyDbLib;
using System.Threading.Tasks;

namespace EasyDbLibTest.Query
{
    [TestClass]
    public class InsertUpdateDeleteQueriesTest
    {
        private static string sqlServerConnectionString =
                @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\romag\Documents\Visual Studio 2017\Projects\EasyDbLib\EasyDbLibTest\SqlServerDb.mdf;Integrated Security=True;Connect Timeout=20"
            ;

        private static string sqlServerProviderName = "System.Data.SqlClient";

        // insert

        // query

        [TestMethod]
        public void TestFailWithNoColumns()
        {
            bool failed = false;
            EasyDb.Default.SetConnectionSettings(sqlServerConnectionString, sqlServerProviderName);

            try
            {
                var result = EasyDb.Default.InsertInto("people").GetQuery();
            }
            catch (Exception)
            {
                failed = true;
            }

            Assert.IsTrue(failed);
        }

        [TestMethod]
        public void TestBuildQuery()
        {
            EasyDb.Default.SetConnectionSettings(sqlServerConnectionString, sqlServerProviderName);

            var result = EasyDb.Default
                .InsertInto("people")
                .Columns("first", "last", "age", "email")
                .Values("test first", "test last", 30, "test@domain.com")
                .GetQuery();

            Assert.AreEqual("insert into [people] ([first],[last],[age],[email]) output INSERTED.Id values (@first,@last,@age,@email)", result);
        }

        // add data

        [TestMethod]
        public async Task TestFailWithNoValues()
        {
            bool failed = false;
            EasyDb.Default.SetConnectionSettings(sqlServerConnectionString, sqlServerProviderName);

            try
            {
                var result = await EasyDb.Default.InsertInto("people")
                    .Columns("first", "last", "age", "email")
                    .LastInsertedId<int>();
            }
            catch (Exception)
            {
                failed = true;
            }

            Assert.IsTrue(failed);
        }

        [TestMethod]
        public async Task TestFailWithNotSameNumberOfColumnANdValues()
        {
            bool failed = false;
            EasyDb.Default.SetConnectionSettings(sqlServerConnectionString, sqlServerProviderName);

            try
            {
                var result = await EasyDb.Default.InsertInto("people")
                    .Columns("first", "last", "age", "email")
                    .Values("test first", "test last")
                    .LastInsertedId<int>();
            }
            catch (Exception)
            {
                failed = true;
            }

            Assert.IsTrue(failed);
        }


        // non query

        [TestMethod]
        public async Task TestNonQueryAsync()
        {
            EasyDb.Default.SetConnectionSettings(sqlServerConnectionString, sqlServerProviderName);

            var result = await EasyDb.Default
                .InsertInto("people")
                .Columns("first", "last", "age", "email")
                .Values("test first", "test last", 30, "test@domain.com")
                 .NonQueryAsync();

            Assert.AreEqual(1, result);
        }

        // last inserted id


        [TestMethod]
        public async Task TestLastInsertedId_WithInt()
        {
            EasyDb.Default.SetConnectionSettings(sqlServerConnectionString, sqlServerProviderName);

            var result = await EasyDb.Default
                .InsertInto("people")
                .Columns("first", "last", "age", "email")
                .Values("test first", "test last", 30, "test@domain.com")
                 .LastInsertedId<int>();

            Assert.IsTrue(result > 0);
        }

        [TestMethod]
        public async Task TestLastInsertedId_WithGuid()
        {
            EasyDb.Default.SetConnectionSettings(sqlServerConnectionString, sqlServerProviderName);

            var result = await EasyDb.Default
                .InsertInto("UsersWithGuid")
                .Columns("name","email")
                .Values("test first", "test@domain.com")
                .LastInsertedId<Guid>();

            Assert.IsNotNull(result);
        }

        // fetch

        [TestMethod]
        public async Task TestFetch()
        {
            EasyDb.Default.SetConnectionSettings(sqlServerConnectionString, sqlServerProviderName);

            Mapping.AddTable("people").AddPrimaryKeyColumn("id", "id");

            var result = await EasyDb.Default
                .InsertInto("people")
                .Columns("first", "last", "age", "email")
                .Values("test first", "test last", 30, "test@domain.com")
                 .Fetch<P>(Mapping.GetTable("people"));

            Assert.IsNotNull(result);
            Assert.IsTrue(result.id > 0);
            Assert.AreEqual("test first", result.first);
            Assert.AreEqual("test last", result.last);
            Assert.AreEqual(30, result.age);
            Assert.AreEqual("test@domain.com", result.email);
        }

        // update

        [TestMethod]
        public void TestUpdate_Query()
        {
            EasyDb.Default.SetConnectionSettings(sqlServerConnectionString, sqlServerProviderName);

            var result = EasyDb.Default
                .Update("people")
                .Set("first","updated first")
                .Set("last", "updated last")
                .Where(Condition.Op("id", 4))
                .GetQuery();

            Assert.AreEqual("update [people] set [first]='updated first',[last]='updated last' where [id]=@id", result); ;
        }

        [TestMethod]
        public async Task TestUpdate()
        {
            EasyDb.Default.SetConnectionSettings(sqlServerConnectionString, sqlServerProviderName);

            var result = await EasyDb.Default
                .Update("people")
                .Set("first", "updated first")
                .Set("last", "updated last")
                .Where(Condition.Op("id", 4))
                .NonQueryAsync();

            Assert.AreEqual(1, result); ;
        }


        // delete

        [TestMethod]
        public void TestDelete_Query()
        {
            EasyDb.Default.SetConnectionSettings(sqlServerConnectionString, sqlServerProviderName);

            var result = EasyDb.Default
                .DeleteFrom("people")
                .Where(Condition.Op("id", 3))
                .GetQuery();

            Assert.AreEqual("delete from [people] where [id]=@id", result);;
        }

        [TestMethod]
        public void TestDelete_WhithSameColumNames()
        {
            EasyDb.Default.SetConnectionSettings(sqlServerConnectionString, sqlServerProviderName);

            var result = EasyDb.Default
                .DeleteFrom("people")
                .Where(Condition.Op("id", 3).Or(Condition.Op("id",4)))
                .GetQuery();

            Assert.AreEqual("delete from [people] where [id]=@id or [id]=@id2", result); ;
        }


        [TestMethod]
        public async Task TestDelete()
        {
            EasyDb.Default.SetConnectionSettings(sqlServerConnectionString, sqlServerProviderName);

            var result = await EasyDb.Default
                .DeleteFrom("people")
                .Where(Condition.Op("id", 3))
                .NonQueryAsync();

            Assert.AreEqual(1, result); ;
        }
    }
}
