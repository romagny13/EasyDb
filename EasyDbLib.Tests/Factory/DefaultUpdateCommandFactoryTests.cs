using EasyDbLib.Tests.Factory;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Data;

namespace EasyDbLib.Tests.Factory
{
    [TestClass]
    public class DefaultUpdateCommandFactoryTests
    {
        public DefaultUpdateCommandFactory GetService()
        {
            return new DefaultUpdateCommandFactory();
        }

        [TestMethod]
        public void GetQuery_WithNoMappingAndNoCondition()
        {
            var service = this.GetService();

            var db = new EasyDb();
            db.SetConnectionStringSettings(@"Data Source=(LocalDb)\MSSQLLocalDB;Initial Catalog=testdb;Integrated Security=True", "System.Data.SqlClient");
            service.SetDb(db);

            var model = new User { };
            var columnValues = DbHelper.GetUpdateColumnValues<User>(model);
            var columns = new List<string>(columnValues.Keys);

            var result = service.GetQuery<User>(columns, null, null);

            Assert.AreEqual("update [User] set [Id]=@id,[UserName]=@username,[RoleId]=@roleid", result);
        }

        [TestMethod]
        public void GetQuery_WithNoMappingAndCondition()
        {
            var service = this.GetService();

            var db = new EasyDb();
            db.SetConnectionStringSettings(@"Data Source=(LocalDb)\MSSQLLocalDB;Initial Catalog=testdb;Integrated Security=True", "System.Data.SqlClient");
            service.SetDb(db);

            var condition = Check.Op("Id", 10);
            var model = new User { };
            var columnValues = DbHelper.GetUpdateColumnValues<User>(model, null, condition);
            var columns = new List<string>(columnValues.Keys);

            var result = service.GetQuery<User>(columns, condition, null);

            Assert.AreEqual("update [User] set [UserName]=@username,[RoleId]=@roleid where [Id]=@id", result);
        }

        [TestMethod]
        public void GetQuery_WithMultipleCondition()
        {
            var service = this.GetService();

            var db = new EasyDb();
            db.SetConnectionStringSettings(@"Data Source=(LocalDb)\MSSQLLocalDB;Initial Catalog=testdb;Integrated Security=True", "System.Data.SqlClient");
            service.SetDb(db);

            var condition = Check.Op("Id", 10).And(Check.Op("RoleId", 5));
            var model = new User { };
            var columnValues = DbHelper.GetUpdateColumnValues<User>(model, null, condition);
            var columns = new List<string>(columnValues.Keys);

            var result = service.GetQuery<User>(columns, condition);

            Assert.AreEqual("update [User] set [UserName]=@username where [Id]=@id and [RoleId]=@roleid", result);
        }

        [TestMethod]
        public void GetQuery_WithMapping()
        {
            var service = this.GetService();

            var db = new EasyDb();
            db.SetConnectionStringSettings(@"Data Source=(LocalDb)\MSSQLLocalDB;Initial Catalog=testdb;Integrated Security=True", "System.Data.SqlClient");
            service.SetDb(db);

            var condition = Check.Op("Id", 10).And(Check.Op("RoleId", 5));
            var table = new Table<User>("users")
                .SetPrimaryKeyColumn("Id", p => p.Id)
                .SetColumn("Name", p => p.UserName);

            var model = new User { };
            var columnValues = DbHelper.GetUpdateColumnValues<User>(model, table, condition);
            var columns = new List<string>(columnValues.Keys);

            var result = service.GetQuery<User>(columns, condition, table);

            Assert.AreEqual("update [users] set [Name]=@name where [Id]=@id and [RoleId]=@roleid", result);
        }

        [TestMethod]
        public void GetQuery_WithMapping_Ignore()
        {
            var service = this.GetService();

            var db = new EasyDb();
            db.SetConnectionStringSettings(@"Data Source=(LocalDb)\MSSQLLocalDB;Initial Catalog=testdb;Integrated Security=True", "System.Data.SqlClient");
            service.SetDb(db);

            var condition = Check.Op("Id", 10);
            var table = new Table<User>("users")
                .SetPrimaryKeyColumn("Id", p => p.Id)
                .SetColumn("Name", p => p.UserName, false, true);

            var model = new User { };
            var columnValues = DbHelper.GetUpdateColumnValues<User>(model, table, condition);
            var columns = new List<string>(columnValues.Keys);

            var result = service.GetQuery<User>(columns, condition, table);

            Assert.AreEqual("update [users] set [RoleId]=@roleid where [Id]=@id", result);
        }

        [TestMethod]
        public void GetQuery_WithMapping_IgnoreDatabaseGeneratedColumn()
        {
            var service = this.GetService();

            var db = new EasyDb();
            db.SetConnectionStringSettings(@"Data Source=(LocalDb)\MSSQLLocalDB;Initial Catalog=testdb;Integrated Security=True", "System.Data.SqlClient");
            service.SetDb(db);

            var condition = Check.Op("Id", 10);
            var table = new Table<User>("users")
                .SetPrimaryKeyColumn("Id", p => p.Id)
                .SetColumn("Name", p => p.UserName, true);

            var model = new User { };
            var columnValues = DbHelper.GetUpdateColumnValues<User>(model, table, condition);
            var columns = new List<string>(columnValues.Keys);

            var result = service.GetQuery<User>(columns, condition, table);

            Assert.AreEqual("update [users] set [RoleId]=@roleid where [Id]=@id", result);
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
                RoleId = 1
            };

            db.DefaultMappingBehavior = DefaultMappingBehavior.CreateEmptyTable;

            var result = service.GetCommand<User>(model, Check.Op("Id", 10));

            Assert.AreEqual("update [User] set [UserName]=@username,[RoleId]=@roleid where [Id]=@id", result.CommandText);

            Assert.AreEqual(CommandType.Text, result.CommandType);
            Assert.AreEqual(3, result.Parameters.Count);

            Assert.AreEqual("@username", result.Parameters[0].ParameterName);
            Assert.AreEqual("Marie", result.Parameters[0].Value);

            Assert.AreEqual("@roleid", result.Parameters[1].ParameterName);
            Assert.AreEqual(1, result.Parameters[1].Value);

            Assert.AreEqual("@id", result.Parameters[2].ParameterName);
            Assert.AreEqual(10, result.Parameters[2].Value);
        }

        [TestMethod]
        public void GetCommand_WithMultipleCondition()
        {
            var service = this.GetService();

            var db = new EasyDb();
            db.SetConnectionStringSettings(@"Data Source=(LocalDb)\MSSQLLocalDB;Initial Catalog=testdb;Integrated Security=True", "System.Data.SqlClient");
            service.SetDb(db);

            var model = new User
            {
                UserName = "Marie",
                RoleId = 1
            };

            db.DefaultMappingBehavior = DefaultMappingBehavior.CreateEmptyTable;

            var result = service.GetCommand<User>(model, Check.Op("Id", 10).And(Check.Op("RoleId",5)));

            Assert.AreEqual("update [User] set [UserName]=@username where [Id]=@id and [RoleId]=@roleid", result.CommandText);

            Assert.AreEqual(CommandType.Text, result.CommandType);
            Assert.AreEqual(3, result.Parameters.Count);

            Assert.AreEqual("@username", result.Parameters[0].ParameterName);
            Assert.AreEqual("Marie", result.Parameters[0].Value);

            Assert.AreEqual("@id", result.Parameters[1].ParameterName);
            Assert.AreEqual(10, result.Parameters[1].Value);

            Assert.AreEqual("@roleid", result.Parameters[2].ParameterName);
            Assert.AreEqual(5, result.Parameters[2].Value);
        }

        [TestMethod]
        public void GetQuery_WithMappingAndNoColumnToSet_ThrowException()
        {
            var service = this.GetService();

            var db = new EasyDb();
            db.SetConnectionStringSettings(@"Data Source=(LocalDb)\MSSQLLocalDB;Initial Catalog=testdb;Integrated Security=True", "System.Data.SqlClient");

            db.SetTable<User>("User")
               .SetPrimaryKeyColumn("Id", p => p.Id)
               .SetColumn("UserName", p => p.UserName, true);

            service.SetDb(db);

            var model = new User { };

            bool failed = false;

            try
            {
                var result = service.GetCommand<User>(model, Check.Op("Id", 10).And(Check.Op("RoleId", 5)));
            }
            catch (System.Exception)
            {
                failed = true;
            }

            Assert.IsTrue(failed);
        }
    }


}
