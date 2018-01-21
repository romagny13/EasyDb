using EasyDbLib.Tests.Factory;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Data;

namespace EasyDbLib.Tests
{
    [TestClass]
    public class DefaultInsertCommandFactoryTests
    {
        public DefaultInsertCommandFactory GetService()
        {
            return new DefaultInsertCommandFactory();
        }

        [TestMethod]
        public void GetQuery_WithNoMapping_NoIgnoreKeyResult()
        {
            var service = this.GetService();

            var db = new EasyDb();
            db.SetConnectionStringSettings(@"Data Source=(LocalDb)\MSSQLLocalDB;Initial Catalog=testdb;Integrated Security=True", "System.Data.SqlClient");
            service.SetDb(db);

            var model = new User
            {
                UserName = "Marie",
                RoleId = 10
            };

            var table = db.TryGetTable<User>();
            var columnValues = DbHelper.GetInsertColumnValues<User>(model, table);
            var columns = new List<string>(columnValues.Keys);

            var result = service.GetQuery<User>(columns, table);

            Assert.AreEqual("insert into [User] ([Id],[UserName],[RoleId]) values (@id,@username,@roleid)", result);
        }

        [TestMethod]
        public void GetQuery_WithDefaults()
        {
            var service = this.GetService();

            var db = new EasyDb();

            db.DefaultMappingBehavior = DefaultMappingBehavior.CreateEmptyTable;

            db.SetConnectionStringSettings(@"Data Source=(LocalDb)\MSSQLLocalDB;Initial Catalog=testdb;Integrated Security=True", "System.Data.SqlClient");
            service.SetDb(db);

            var model = new User
            {
                UserName = "Marie",
                RoleId = 10
            };

            var table = db.TryGetTable<User>();
            var columnValues = DbHelper.GetInsertColumnValues<User>(model, table);
            var columns = new List<string>(columnValues.Keys);

            var result = service.GetQuery<User>(columns, table);

            Assert.AreEqual("insert into [User] ([UserName],[RoleId]) output inserted.id values (@username,@roleid)", result);
        }

        [TestMethod]
        public void GetQuery_WithMapping()
        {
            var service = this.GetService();

            var db = new EasyDb();
            db.SetConnectionStringSettings(@"Data Source=(LocalDb)\MSSQLLocalDB;Initial Catalog=testdb;Integrated Security=True", "System.Data.SqlClient");
            service.SetDb(db);

            var model = new User
            {
                UserName = "Marie",
                RoleId = 10
            };

            var table = new Table<User>("users")
                .SetPrimaryKeyColumn("Id", p => p.Id);

            var columnValues = DbHelper.GetInsertColumnValues<User>(model, table);
            var columns = new List<string>(columnValues.Keys);

            var result = service.GetQuery<User>(columns, table);

            Assert.AreEqual("insert into [users] ([UserName],[RoleId]) output inserted.id values (@username,@roleid)", result);
        }

        [TestMethod]
        public void GetCommand()
        {
            var service = this.GetService();

            var db = new EasyDb();
            db.SetConnectionStringSettings(@"Data Source=(LocalDb)\MSSQLLocalDB;Initial Catalog=testdb;Integrated Security=True", "System.Data.SqlClient");
            service.SetDb(db);

            var model = new User
            {
                UserName = "Marie",
                RoleId = 10
            };

            db.DefaultMappingBehavior = DefaultMappingBehavior.CreateEmptyTable;

            var result = service.GetCommand<User>(model);

            Assert.AreEqual("insert into [User] ([UserName],[RoleId]) output inserted.id values (@username,@roleid)", result.CommandText);

            Assert.AreEqual(CommandType.Text, result.CommandType);
            Assert.AreEqual(2, result.Parameters.Count);

            Assert.AreEqual("@username", result.Parameters[0].ParameterName);
            Assert.AreEqual("Marie", result.Parameters[0].Value);

            Assert.AreEqual("@roleid", result.Parameters[1].ParameterName);
            Assert.AreEqual(10, result.Parameters[1].Value);
        }


    }
}
