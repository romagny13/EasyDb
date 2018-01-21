using EasyDbLib.Tests.Factory;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;

namespace EasyDbLib.Tests
{
    [TestClass]
    public class DefaultSelectionOneCommandFactoryTests
    {
        public DefaultSelectionOneCommandFactory GetService()
        {
            return new DefaultSelectionOneCommandFactory();
        }

        [TestMethod]
        public void GetQuery()
        {
            var service = this.GetService();

            var db = new EasyDb();
            db.SetConnectionStringSettings(@"Data Source=(LocalDb)\MSSQLLocalDB;Initial Catalog=testdb;Integrated Security=True", "System.Data.SqlClient");
            service.SetDb(db);

            var condition = new ConditionAndParameterContainer(Check.Op("Id", 10), QueryServiceFactory.Current);

            var result = service.GetQuery<User>(condition);

            Assert.AreEqual("select [Id],[UserName],[RoleId] from [User] where [Id]=@id", result);
        }

        [TestMethod]
        public void GetQuery_WithMapping()
        {
            var service = this.GetService();

            var db = new EasyDb();
            db.SetConnectionStringSettings(@"Data Source=(LocalDb)\MSSQLLocalDB;Initial Catalog=testdb;Integrated Security=True", "System.Data.SqlClient");
            service.SetDb(db);

            var table = new Table<User>("users")
               .SetColumn("Name", p => p.UserName)
               .SetColumn("RoleId", p => p.RoleId, false, true);

            var condition = new ConditionAndParameterContainer(Check.Op("Id", 10), QueryServiceFactory.Current);

            var result = service.GetQuery<User>(condition, table);

            Assert.AreEqual("select [Id],[Name] from [users] where [Id]=@id", result);
        }

        [TestMethod]
        public void GetQuery_WithBetween()
        {
            var service = this.GetService();

            var db = new EasyDb();
            db.SetConnectionStringSettings(@"Data Source=(LocalDb)\MSSQLLocalDB;Initial Catalog=testdb;Integrated Security=True", "System.Data.SqlClient");
            service.SetDb(db);

            var condition = new ConditionAndParameterContainer(Check.Between("Id", 10, 20), QueryServiceFactory.Current);

            var result = service.GetQuery<User>(condition);

            Assert.AreEqual("select [Id],[UserName],[RoleId] from [User] where [Id] between 10 and 20", result);
        }

        [TestMethod]
        public void GetQuery_WithLike()
        {
            var service = this.GetService();

            var db = new EasyDb();
            db.SetConnectionStringSettings(@"Data Source=(LocalDb)\MSSQLLocalDB;Initial Catalog=testdb;Integrated Security=True", "System.Data.SqlClient");
            service.SetDb(db);

            var condition = new ConditionAndParameterContainer(Check.Like("Id", "%abc"), QueryServiceFactory.Current);

            var result = service.GetQuery<User>(condition);

            Assert.AreEqual("select [Id],[UserName],[RoleId] from [User] where [Id] like '%abc'", result);
        }

        [TestMethod]
        public void GetQuery_WithIsNull()
        {
            var service = this.GetService();

            var db = new EasyDb();
            db.SetConnectionStringSettings(@"Data Source=(LocalDb)\MSSQLLocalDB;Initial Catalog=testdb;Integrated Security=True", "System.Data.SqlClient");
            service.SetDb(db);

            var condition = new ConditionAndParameterContainer(Check.IsNull("Id"), QueryServiceFactory.Current);

            var result = service.GetQuery<User>(condition);

            Assert.AreEqual("select [Id],[UserName],[RoleId] from [User] where [Id] is null", result);
        }

        [TestMethod]
        public void GetQuery_WithIsNotNull()
        {
            var service = this.GetService();

            var db = new EasyDb();
            db.SetConnectionStringSettings(@"Data Source=(LocalDb)\MSSQLLocalDB;Initial Catalog=testdb;Integrated Security=True", "System.Data.SqlClient");
            service.SetDb(db);

            var condition = new ConditionAndParameterContainer(Check.IsNotNull("Id"), QueryServiceFactory.Current);

            var result = service.GetQuery<User>(condition);

            Assert.AreEqual("select [Id],[UserName],[RoleId] from [User] where [Id] is not null", result);
        }

        [TestMethod]
        public void GetQuery_WithMultipleCondition()
        {
            var service = this.GetService();

            var db = new EasyDb();
            db.SetConnectionStringSettings(@"Data Source=(LocalDb)\MSSQLLocalDB;Initial Catalog=testdb;Integrated Security=True", "System.Data.SqlClient");
            service.SetDb(db);

            var condition = new ConditionAndParameterContainer(Check.Op("Id", 10).And(Check.Op("RoleId", 5)), QueryServiceFactory.Current);

            var result = service.GetQuery<User>(condition);

            Assert.AreEqual("select [Id],[UserName],[RoleId] from [User] where [Id]=@id and [RoleId]=@roleid", result);
        }

        [TestMethod]
        public void GetQuery_WithNoConditionMySQL()
        {
            var service = this.GetService();

            var db = new EasyDb();
            db.SetConnectionStringSettings(@"server=localhost;database=demo;uid=root", "MySql.Data.MySqlClient");
            service.SetDb(db);

            var result = service.GetQuery<User>(null);

            Assert.AreEqual("select `Id`,`UserName`,`RoleId` from `User`", result);
        }

        [TestMethod]
        public void GetQuery_WithMySQL()
        {
            var service = this.GetService();

            var db = new EasyDb();
            db.SetConnectionStringSettings(@"server=localhost;database=demo;uid=root", "MySql.Data.MySqlClient");
            service.SetDb(db);

            var condition = new ConditionAndParameterContainer(Check.Op("Id", 10).And(Check.Op("RoleId", 5)), QueryServiceFactory.Current);

            var result = service.GetQuery<User>(condition);

            Assert.AreEqual("select `Id`,`UserName`,`RoleId` from `User` where `Id`=@id and `RoleId`=@roleid", result);
        }


        [TestMethod]
        public void GetCommand()
        {
            var service = this.GetService();

            var db = new EasyDb();
            db.SetConnectionStringSettings(@"Data Source=(LocalDb)\MSSQLLocalDB;Initial Catalog=testdb;Integrated Security=True", "System.Data.SqlClient");
            service.SetDb(db);

            var result = service.GetCommand<User>(Check.Op("Id", 10));

            Assert.AreEqual("select [Id],[UserName],[RoleId] from [User] where [Id]=@id", result.CommandText);
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

            var result = service.GetCommand<User>(Check.Op("Id", 10).And(Check.Op("RoleId", 5)));

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
