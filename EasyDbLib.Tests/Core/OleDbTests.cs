using System;
using System.Threading.Tasks;
using EasyDbLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EasyDbLib.Tests
{
    [TestClass]
    public class OleDbTests
    {

        public EasyDb GetService()
        {
            var db = new EasyDb();
            db.SetConnectionStringSettings(DbConstants.OleDbConnectionString, DbConstants.OleDbProviderName);

            var table = db.SetTable<OleDbCategory>("Categories")
                .SetPrimaryKeyColumn("CategoryID", p => p.CategoryID, false);

            return db;
        }


        [TestMethod]
        public async Task TestGet()
        {
            var db = GetService();

            var command = db.CreateSqlCommand("select * from `Categories` where `CategoryID`=@id")
                 .AddInParameter("@id", 3);

            var result = await db.SelectOneAsync<OleDbCategory>(command);

            Assert.AreEqual("Confections", result.CategoryName);
        }


        [TestMethod]
        public async Task TestSelectQuery()
        {
            var db = GetService();

            var result = await db.SelectOneAsync<OleDbCategory>(Check.Op("CategoryID", 3));

            Assert.AreEqual("Confections", result.CategoryName);
        }

        [TestMethod]
        public async Task TestAccess_Scalar()
        {
            var db = GetService();

            var result = (int)await db.CountAsync<OleDbCategory>(null);

            Assert.IsTrue(result > 0);
        }


        [TestMethod]
        public async Task ExecuteTests()
        {
            await TestInsert();

            await TestUpdate();

            await TestDelete();
        }

        public async Task TestInsert()
        {
            var db = GetService();

            var category = new OleDbCategory
            {
                CategoryID = 9,
                CategoryName = "My category",
                Description = "My description"
            };

            var result = await db.InsertAsync<OleDbCategory>(category);

            Assert.AreEqual(1, result);
        }


        public async Task TestUpdate()
        {
            var db = GetService();

            var category = new OleDbCategory
            {
                CategoryID = 9,
                CategoryName = "My updated category",
                Description = "My updated description"
            };

            var result = await db.UpdateAsync<OleDbCategory>(category, Check.Op("CategoryID", category.CategoryID));

            Assert.AreEqual(1, result);
        }


        public async Task TestDelete()
        {
            var db = GetService();

            var result = await db.DeleteAsync<OleDbCategory>(Check.Op("CategoryID", 9));

            Assert.AreEqual(1, result);
        }
      
    }

    public class OleDbCategory
    {
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
    }

}

