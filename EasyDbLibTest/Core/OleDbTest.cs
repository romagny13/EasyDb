using System;
using System.Threading.Tasks;
using EasyDbLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EasyDbLibTest
{
    [TestClass]
    public class OleDbTest
    {
        private string connectionString =
                @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=C:\Users\romag\Documents\Visual Studio 2017\Projects\EasyDbLib\EasyDbLibTest\NorthWind.mdb"
            ;

        private string providerName = "System.Data.OleDb";

        [TestMethod]
        public async Task TestGet()
        {
            var db = new EasyDb();
            db.SetConnectionStringSettings(this.connectionString, this.providerName);

            var result = await db.CreateCommand("select * from `Categories` where `CategoryID`=@id")
                .AddParameter("@id", 3)
                .ReadOneAsync<OleDbCategory>();

            Assert.AreEqual("Confections", result.CategoryName);
        }


        [TestMethod]
        public async Task TestInsert()
        {
            var db = new EasyDb();
            db.SetConnectionStringSettings(this.connectionString, this.providerName);

            var category = new OleDbCategory
            {
                CategoryID = 9,
                CategoryName = "My category",
                Description = "My description"
            };

            var rowAffected = await db.CreateCommand("insert into Categories (CategoryID,CategoryName,Description) values (@id,@name,@description)")
                .AddParameter("@id", category.CategoryID)
                .AddParameter("@name", category.CategoryName)
                .AddParameter("@description", category.Description)
                .NonQueryAsync();

            Assert.AreEqual(1, rowAffected);

            var result = await db.CreateCommand("select * from `Categories` where `CategoryID`=@id")
              .AddParameter("@id", category.CategoryID)
              .ReadOneAsync<OleDbCategory>();

            Assert.AreEqual(category.CategoryID, result.CategoryID);
            Assert.AreEqual(category.CategoryName, result.CategoryName);
            Assert.AreEqual(category.Description, result.Description);
        }


        [TestMethod]
        public async Task TestUpdate()
        {
            var db = new EasyDb();
            db.SetConnectionStringSettings(this.connectionString, this.providerName);

            var category = new OleDbCategory
            {
                CategoryID = 9,
                CategoryName = "My updated category",
                Description = "My updated description"
            };

            var rowAffected = await db.CreateCommand("update Categories set CategoryName=@name,Description=@description where CategoryID=@id")
                .AddParameter("@name", category.CategoryName)
                .AddParameter("@description", category.Description)
                .AddParameter("@id", category.CategoryID)
                .NonQueryAsync();

            Assert.AreEqual(1, rowAffected);

            var result = await db.CreateCommand("select * from `Categories` where `CategoryID`=@id")
               .AddParameter("@id", category.CategoryID)
               .ReadOneAsync<OleDbCategory>();

            Assert.AreEqual(category.CategoryID, result.CategoryID);
            Assert.AreEqual(category.CategoryName, result.CategoryName);
            Assert.AreEqual(category.Description, result.Description);
        }


        [TestMethod]
        public async Task TestDelete()
        {
            var db = new EasyDb();
            db.SetConnectionStringSettings(this.connectionString, this.providerName);

            var category = new OleDbCategory
            {
                CategoryID = 9
            };

            var result = await db.CreateCommand("delete from Categories where CategoryID=@id")
                .AddParameter("@id", category.CategoryID)
                .NonQueryAsync();

            Assert.AreEqual(1, result);
        }

        // query

        [TestMethod]
        public async Task TestQueries()
        {

            await TestSelectQuery();

            await TestInsertQuery();

            await TestUpdateQuery();

            await TestDeleteQuery();

        }


        public async Task TestSelectQuery()
        {
            var db = new EasyDb();
            db.SetConnectionStringSettings(this.connectionString, this.providerName);

            Mapping.SetTable("Categories");

            var result = await db.Select<OleDbCategory>(Mapping.GetTable("Categories"))
                .Where(Check.Op("CategoryID", 3))
                .ReadOneAsync();

            Assert.AreEqual("Confections", result.CategoryName);
        }

        public async Task TestInsertQuery()
        {
            var db = new EasyDb();
            db.SetConnectionStringSettings(this.connectionString, this.providerName);

            var category = new OleDbCategory
            {
                CategoryID = 10,
                CategoryName = "My category",
                Description = "My description"
            };

            var rowAfftected = await db.InsertInto("Categories")
                .Values("CategoryID", category.CategoryID)
               .Values("CategoryName", category.CategoryName)
               .Values("Description", category.Description)
               .NonQueryAsync();

            Assert.AreEqual(1, rowAfftected);

            Mapping.SetTable("Categories");

            var result = await db.Select<OleDbCategory>(Mapping.GetTable("Categories"))
           .Where(Check.Op("CategoryID", 10))
           .ReadOneAsync();

            Assert.AreEqual(category.CategoryID, result.CategoryID);
            Assert.AreEqual(category.CategoryName, result.CategoryName);
            Assert.AreEqual(category.Description, result.Description);
        }


        public async Task TestUpdateQuery()
        {
            var db = new EasyDb();
            db.SetConnectionStringSettings(this.connectionString, this.providerName);

            var category = new OleDbCategory
            {
                CategoryID = 10,
                CategoryName = "My updated category",
                Description = "My updated description"
            };

            var rowAffected = await db.Update("Categories")
                .Set("CategoryName", category.CategoryName)
                .Set("Description", category.Description)
                .Where(Check.Op("CategoryID", category.CategoryID))
                .NonQueryAsync();

            Assert.AreEqual(1, rowAffected);


            var result = await db.Select<OleDbCategory>(Mapping.GetTable("Categories"))
              .Where(Check.Op("CategoryID", category.CategoryID))
              .ReadOneAsync();

            Assert.AreEqual(category.CategoryID, result.CategoryID);
            Assert.AreEqual(category.CategoryName, result.CategoryName);
            Assert.AreEqual(category.Description, result.Description);
        }


        public async Task TestDeleteQuery()
        {
            var db = new EasyDb();
            db.SetConnectionStringSettings(this.connectionString, this.providerName);

            var category = new OleDbCategory
            {
                CategoryID = 10
            };

            var result = await db.DeleteFrom("Categories")
                .Where(Check.Op("CategoryID", category.CategoryID))
                .NonQueryAsync();

            Assert.AreEqual(1, result);
        }


        public async Task TestAccess_Scalar()
        {
            var db = new EasyDb();
            db.SetConnectionStringSettings(this.connectionString, this.providerName);

            var result = (int)await db.CreateCommand("select count(*) from Categories")
                .ScalarAsync();

            Assert.IsTrue(result > 0);
        }      
    }

    public class OleDbCategory
    {
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
    }

}

