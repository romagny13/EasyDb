using EasyDbLib.Tests.Factory;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;

namespace EasyDbLib.Tests
{
    [TestClass]
    public class DefaultSelectionAllCommandFactoryTests
    {
        public DefaultSelectionAllCommandFactory GetService()
        {
            return new DefaultSelectionAllCommandFactory();
        }

        [TestMethod]
        public void GetQuery()
        {
            var service = this.GetService();

            var db = new EasyDb();
            db.SetConnectionStringSettings(@"Data Source=(LocalDb)\MSSQLLocalDB;Initial Catalog=testdb;Integrated Security=True", "System.Data.SqlClient");
            service.SetDb(db);

            var result = service.GetQuery<User>(null, null, null);

            Assert.AreEqual("select [Id],[UserName],[RoleId] from [User]", result);
        }

        [TestMethod]
        public void GetQuery_WithMapping()
        {
            var service = this.GetService();

            var table = new Table<User>("users")
             .SetColumn("Name", p => p.UserName)
             .SetColumn("RoleId", p => p.RoleId, false, true);

            var db = new EasyDb();
            db.SetConnectionStringSettings(@"Data Source=(LocalDb)\MSSQLLocalDB;Initial Catalog=testdb;Integrated Security=True", "System.Data.SqlClient");
            service.SetDb(db);

            var condition = Check.Op("Id", 10);

            var result = service.GetQuery<User>(null, condition, null, table);

            Assert.AreEqual("select [Id],[Name] from [users] where [Id]=@id", result);
        }

        [TestMethod]
        public void GetQuery_WithLimit()
        {
            var service = this.GetService();

            var db = new EasyDb();
            db.SetConnectionStringSettings(@"Data Source=(LocalDb)\MSSQLLocalDB;Initial Catalog=testdb;Integrated Security=True", "System.Data.SqlClient");
            service.SetDb(db);

            var result = service.GetQuery<User>(10, null, null);

            Assert.AreEqual("select top 10 [Id],[UserName],[RoleId] from [User]", result);
        }

        [TestMethod]
        public void GetQuery_WithLimitAndCondition()
        {
            var service = this.GetService();

            var db = new EasyDb();
            db.SetConnectionStringSettings(@"Data Source=(LocalDb)\MSSQLLocalDB;Initial Catalog=testdb;Integrated Security=True", "System.Data.SqlClient");
            service.SetDb(db);

            var condition = Check.Op("Id", 10);

            var result = service.GetQuery<User>(10, condition, null);

            Assert.AreEqual("select top 10 [Id],[UserName],[RoleId] from [User] where [Id]=@id", result);
        }

        [TestMethod]
        public void GetQuery_WithLimitAndConditionAndSorts()
        {
            var service = this.GetService();

            var db = new EasyDb();
            db.SetConnectionStringSettings(@"Data Source=(LocalDb)\MSSQLLocalDB;Initial Catalog=testdb;Integrated Security=True", "System.Data.SqlClient");
            service.SetDb(db);

            var condition = Check.Op("Id", 10);

            var result = service.GetQuery<User>(10, condition, new string[] { "Id", "RoleId DESC" });

            Assert.AreEqual("select top 10 [Id],[UserName],[RoleId] from [User] where [Id]=@id order by [Id],[RoleId] DESC", result);
        }

        [TestMethod]
        public void GetQuery_WithLimitMySQL()
        {
            var service = this.GetService();

            var db = new EasyDb();
            db.SetConnectionStringSettings(@"server=localhost;database=demo;uid=root", "MySql.Data.MySqlClient");
            service.SetDb(db);

            var result = service.GetQuery<User>(10, null, null);

            Assert.AreEqual("select `Id`,`UserName`,`RoleId` from `User` limit 10", result);
        }

        [TestMethod]
        public void GetQuery_WithSortsMySQL()
        {
            var service = this.GetService();

            var db = new EasyDb();
            db.SetConnectionStringSettings(@"server=localhost;database=demo;uid=root", "MySql.Data.MySqlClient");
            service.SetDb(db);

            var result = service.GetQuery<User>(null, null, new string[] { "Id", "RoleId DESC" });

            Assert.AreEqual("select `Id`,`UserName`,`RoleId` from `User` order by `Id`,`RoleId` DESC", result);
        }

        [TestMethod]
        public void GetQuery_WithLimitAndConditionAndSortsMySQL()
        {
            var service = this.GetService();

            var db = new EasyDb();
            db.SetConnectionStringSettings(@"server=localhost;database=demo;uid=root", "MySql.Data.MySqlClient");
            service.SetDb(db);

            var condition = Check.Op("Id", 10);

            var result = service.GetQuery<User>(10, condition, new string[] { "Id", "RoleId DESC" });

            Assert.AreEqual("select `Id`,`UserName`,`RoleId` from `User` where `Id`=@id order by `Id`,`RoleId` DESC limit 10", result);
        }

        [TestMethod]
        public void GetCommand()
        {
            var service = this.GetService();

            var db = new EasyDb();
            db.SetConnectionStringSettings(@"Data Source=(LocalDb)\MSSQLLocalDB;Initial Catalog=testdb;Integrated Security=True", "System.Data.SqlClient");
            service.SetDb(db);

            var result = service.GetCommand<User>(10, Check.Op("Id", 10), new string[] { "Id", "RoleId DESC" });

            Assert.AreEqual("select top 10 [Id],[UserName],[RoleId] from [User] where [Id]=@id order by [Id],[RoleId] DESC", result.CommandText);
            Assert.AreEqual(CommandType.Text, result.CommandType);

            Assert.AreEqual(1, result.Parameters.Count);

            Assert.AreEqual("@id", result.Parameters[0].ParameterName);
            Assert.AreEqual(10, result.Parameters[0].Value);
        }

        [TestMethod]
        public void GetCommand_WithMultipleParameters()
        {
            var service = this.GetService();

            var db = new EasyDb();
            db.SetConnectionStringSettings(@"Data Source=(LocalDb)\MSSQLLocalDB;Initial Catalog=testdb;Integrated Security=True", "System.Data.SqlClient");
            service.SetDb(db);

            var result = service.GetCommand<User>(null, Check.Op("Id", 10).And(Check.Op("RoleId", 5)), null);

            Assert.AreEqual("select [Id],[UserName],[RoleId] from [User] where [Id]=@id and [RoleId]=@roleid", result.CommandText);
            Assert.AreEqual(CommandType.Text, result.CommandType);

            Assert.AreEqual(2, result.Parameters.Count);

            Assert.AreEqual("@id", result.Parameters[0].ParameterName);
            Assert.AreEqual(10, result.Parameters[0].Value);

            Assert.AreEqual("@roleid", result.Parameters[1].ParameterName);
            Assert.AreEqual(5, result.Parameters[1].Value);
        }
    }
}
