using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EasyDb;

namespace EasyDbTests
{

    public class TestConstants
    {
        private TestConstants()
        { }
        public const string SqlClientConnectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=c:\users\romag\documents\visual studio 2015\Projects\EasyDb\EasyDbTests\SqlServerDb.mdf;Integrated Security=True";
    }

    [TestClass]
    public class ProviderTests
    {
        [TestMethod]
        public void Should_Connect_With_SqlClient()
        {
            var db = new Db();
            db.Initialize(TestConstants.SqlClientConnectionString, "System.Data.SqlClient");

            var user = db.CreateCommand("select * from [dbo].[Users] where id=@id")
                .AddParameter("@id", 11)
                .ReadOneMapTo<User>();

            Assert.AreEqual(user.UserName, "Marie");
        }

        [TestMethod]
        public void Should_Connect_With_OleDb()
        {
            var db = new Db();
            db.Initialize(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=C:\Users\romag\Documents\Visual Studio 2015\Projects\EasyDb\EasyDbTests\NorthWind.mdb", "System.Data.OleDb");

            var count = db.CreateCommand("select count(*) from Categories")
                .Scalar();

            var result = Helper.TryParseToInt(count);
            Assert.IsTrue(result.HasValue && result > 0);
        }

        [TestMethod]
        public void Should_Get_Data_With_OleDb()
        {
            var db = new Db();
            db.Initialize(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=C:\Users\romag\Documents\Visual Studio 2015\Projects\EasyDb\EasyDbTests\NorthWind.mdb", "System.Data.OleDb");

            var category = db.CreateCommand("select * from Categories where id=@id")
                .AddParameter("@id",1)
                .ReadOneMapTo<Category>();

            Assert.AreEqual(category.CategoryName.Trim(), "Beverages");
        }

        [TestMethod]
        public void Should_Get_Data_With_MySQL()
        {
            var db = new Db();
            db.Initialize(@"server=localhost;user id=root;database=mydb", "MySql.Data.MySqlClient");

            var user = db.CreateCommand("select * from users where id=@id")
                .AddParameter("@id", 1)
                .ReadOneMapTo<MySQLUser>();

            Assert.AreEqual(user.UserName.Trim(), "Marie");
        }
    }

    public class Category
    {
        public string CategoryName { get; set; }
    }

    public class Helper
    {
        public static int? TryParseToInt(object result)
        {
            int response = default(int);
            if (int.TryParse(result.ToString(), out response))
            {
                return response;
            }
            else
            {
                return null;
            }
        }
    }

    public class MySQLUser
    {
        [Map(ColumnName = "id")]
        public int Id { get; set; }
        [Map(ColumnName = "name")]
        public string UserName { get; set; }
        [Map(ColumnName = "email")]
        public string Email { get; set; }
    }

    public class User
    {
        public int Id { get; set; }
        [MapAttribute(ColumnName = "Name")]
        public string UserName { get; set; }
        public string Email { get; set; }
        public int? Age { get; set; }
        public Item Item { get; set; }
        public override string ToString()
        {
            return UserName;
        }
    }
    public class Item
    {

    }
}
