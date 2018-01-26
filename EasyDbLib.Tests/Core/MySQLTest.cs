using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EasyDbLib;
using System.Threading.Tasks;
using System.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EasyDbLib.Tests.Core
{
    [TestClass]
    public class MySQLTest
    {
        public EasyDb GetDb()
        {
            var db = new EasyDb();
            db.SetConnectionStringSettings(@"server=localhost;database=testdb;uid=root", "MySql.Data.MySqlClient");
            db.DefaultMappingBehavior = DefaultMappingBehavior.Reflection;
            return db;
        }

        [TestMethod]
        public async Task TestCreateCommand()
        {
            var db = this.GetDb();
    
            var result = await db.SelectOneAsync<MySQLUser>(Check.Op("id", 2));

            Assert.AreEqual(2, result.id);
            Assert.AreEqual("user2", result.username);
            Assert.AreEqual(30, result.age);
        }

        [TestMethod]
        public async Task TestCreateStoredProcedureCommand()
        {
            var db = this.GetDb();

            var command = db.CreateStoredProcedureCommand<MySQLUser>("get_user")
                 .AddInParameter("p_id", 2);

            var result = await db.SelectOneAsync<MySQLUser>(command);

            Assert.AreEqual(2, result.id);
            Assert.AreEqual("user2", result.username);
            Assert.AreEqual(30, result.age);
        }

        [TestMethod]
        public async Task TestCreateStoredProcedureCommand_WitOutputParameter()
        {
            var db = this.GetDb();

            var command = db.CreateStoredProcedureCommand<MySQLUser>("get_output_age")
               .AddInParameter("p_id", 2)
               .AddOutParameter("p_age");

            await db.SelectOneAsync<MySQLUser>(command);

            var result = command.Parameters["p_age"];
            Assert.AreEqual(30, result.Value);
        }

        [TestMethod]
        public async Task TestCreateStoredProcedureCommand_WithFunction()
        {
            var db = this.GetDb();

            var command = db.CreateSqlCommand("SELECT `get_username_function`(@p0) AS `get_username_function`;")
               .AddInParameter("@p0", 2)
               .AddParameter("get_username_function", null, ParameterDirection.ReturnValue);

            var result = await db.ExecuteScalarAsync(command);

            Assert.AreEqual("user2", result);
        }

        [TestMethod]
        public async Task TestSelectQuery_WithLimit()
        {
            var db = this.GetDb();

            var result = await db.SelectAllAsync<MySQLUser>(1);

            Assert.AreEqual(1, result.Count);

            Assert.AreEqual(1, result[0].id);
            Assert.AreEqual("user1", result[0].username);
            Assert.AreEqual(20, result[0].age);
        }


        [TestMethod]
        public async Task TestCrud()
        {
            var db = this.GetDb();

            var user = new MySQLUser
            {
                username = "user5",
                age = 30
            };

            var result = await db.InsertAsync<MySQLUser>(user); // caution: type returned with unsigned primary key

            Assert.IsTrue(Convert.ToInt32(result) > 2);
            Assert.IsTrue(user.id > 2);


            // update

            var r2 = await db.UpdateAsync<MySQLUser>(user, Check.Op("id", result));

            Assert.AreEqual(1, r2);

            var r3 = await db.DeleteAsync<MySQLUser>(user,Check.Op("id", result));
            Assert.AreEqual(1, r3);
        }      

    }

    [Table("users")]
    public class MySQLUser
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        public string username { get; set; }

        public int? age { get; set; }
    }

}
