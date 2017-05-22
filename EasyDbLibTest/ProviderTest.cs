using System;
using System.Threading.Tasks;
using EasyDbLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EasyDbLibTest
{
    [TestClass]
    public class ProviderTest
    {
        private string connectionString =
                @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=C:\Users\romag\Documents\Visual Studio 2017\Projects\EasyDbLib\EasyDbLibTest\NorthWind.mdb"
            ;

        private string providerName = "System.Data.OleDb";

        [TestMethod]
        public async Task TestAccess_GetData()
        {
            var db = new EasyDb();
            db.GetNewConnection(this.connectionString, this.providerName);

            var result = await db.CreateCommand("select * from Categories where id=@id")
                .AddParameter("@id", 1)
                .ReadOneAsync<Category>();

            Assert.AreEqual("Beverages", result.CategoryName);
        }

        [TestMethod]
        public async Task TestAccess_Scalar ()
        {
            var db = new EasyDb();
            db.GetNewConnection(this.connectionString, this.providerName);

            var result =(int) await db.CreateCommand("select count(*) from Categories")
                .ScalarAsync();

            Assert.IsTrue(result > 0);
        }

        [TestMethod]
        public void Test()
        {
            this.select("id","first");
        }

        public void select(params string[] columns)
        {
            var x = 10;
        }
    }

    public class Category
    {
        public string CategoryName { get; set; }
    }
}
