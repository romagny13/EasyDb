using EasyDb;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace EasyDbTests
{
    [TestClass]
    public class ReadWriteTests
    {
        public ReadWriteTests()
        {
            Db.Default.Initialize(TestConstants.SqlClientConnectionString, "System.Data.SqlClient");
        }

        [TestMethod]
        public void Should_Insert()
        {
            var result = Db.Default.CreateCommand("insert into [dbo].[Users](Name,Email,Age,Birth) values(@name,@email,@age,@birth)")
                  .AddParameter("@name", "user2")
                  .AddParameter("@email", "user2@mail.com")
                  .AddParameter("@age", 30)
                  .AddParameter("@birth", DateTime.Now.AddYears(-30))
                  .NonQuery();

            Assert.AreEqual(result, 1);
        }

        [TestMethod]
        public void Should_Insert_And_Get_Last_Inserted_Id()
        {
            // insert into [dbo].[Users](Name,Email,Age,Birth) values(@name,@email,@age,@birth);SELECT SCOPE_IDENTITY()
            var result = Db.Default.CreateCommand("insert into [dbo].[Users](Name,Email,Age,Birth) output INSERTED.Id values(@name,@email,@age,@birth)")
                         .AddParameter("@name", "user4")
                         .AddParameter("@email", "user4@mail.com")
                         .AddParameter("@age", 30)
                         .AddParameter("@birth", DateTime.Now.AddYears(-30))
                         .Scalar();

            //int key;
            //if(int.TryParse(result.ToString(),out key))
            //{
            //    //  key
            //}

            var key = Helper.TryParseToInt(result);
            Assert.IsTrue(key.HasValue && key > 0);
        }


        [TestMethod]
        public async Task Should_Insert_Null_With_DbNull_Value_On_Nullable_Column()
        {
            var result = await Db.Default.CreateCommand("insert into [dbo].[Users](Name,Email,Age,Birth) values(@name,@email,@age,@birth)")
                  .AddParameter("@name", "user4")
                  .AddParameter("@email", "user4@mail.com")
                  .AddParameter("@age", DBNull.Value)
                  .AddParameter("@birth", DBNull.Value)
                  .NonQueryAsync();

            Assert.AreEqual(result, 1);
        }

        [TestMethod]
        public void Should_Map_One_With_Map_Attribute()
        {
            var user = Db.Default.CreateCommand("select * from [dbo].[Users] where id=@id")
                .AddParameter("@id", 11)
                .ReadOneMapTo<User>();

            Assert.AreEqual(user.UserName, "Marie");
            Assert.AreEqual(user.Email, "marie@mail.com");
        }

        [TestMethod]
        public void Should_Create_Command_With_Stored_Procedure()
        {
            var user = Db.Default.CreateStoredProcedureCommand("GetUser")
                .AddParameter("@id", 11)
                .ReadOneMapTo<User>();

            Assert.AreEqual(user.UserName, "Marie");
            Assert.AreEqual(user.Email, "marie@mail.com");
        }

        [TestMethod]
        public void Should_Map_All_With_Map_Attribute()
        {
            var users = Db.Default.CreateCommand("select * from [dbo].[Users]")
                .ReadAllMapTo<User>();

            Assert.IsTrue(users.Count() > 0);
        }

        [TestMethod]
        public void Each_Db_Have_Own_Connection()
        {
            var db = new Db();
            var db2 = new Db();

            db.Initialize(TestConstants.SqlClientConnectionString, "System.Data.SqlClient", ConnectionStrategyType.Manual);
            db2.Initialize(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=C:\Users\romag\Documents\Visual Studio 2015\Projects\EasyDb\EasyDbTests\NorthWind.mdb", "System.Data.OleDb");

            db.Open();

            Assert.AreEqual(db.Connection.ConnectionString, TestConstants.SqlClientConnectionString);
            Assert.AreEqual(db.Connection.Provider, "System.Data.SqlClient");
            Assert.AreEqual(db.Connection.ConnectionStrategy, ConnectionStrategyType.Manual);
            Assert.AreEqual(db.Connection.State, System.Data.ConnectionState.Open);

            Assert.AreEqual(db2.Connection.ConnectionString, @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=C:\Users\romag\Documents\Visual Studio 2015\Projects\EasyDb\EasyDbTests\NorthWind.mdb");
            Assert.AreEqual(db2.Connection.Provider, "System.Data.OleDb");
            Assert.AreEqual(db2.Connection.ConnectionStrategy, ConnectionStrategyType.Automatic);
            Assert.AreEqual(db2.Connection.State, System.Data.ConnectionState.Closed);

            db.Close();
        }

        [TestMethod]
        public void Cannot_Execute_Command_With_Manual_Strategy_And_Connection_Closed()
        {
            bool fail = false;
            var db = new Db();
            db.Initialize(TestConstants.SqlClientConnectionString, "System.Data.SqlClient",ConnectionStrategyType.Manual);

            try
            {
                var users = db.CreateCommand("select * from [dbo].[Users]")
                              .ReadAllMapTo<User>();
            }
            catch (Exception)
            {
                fail = true;
            }
          
            Assert.IsTrue(fail);
        }

        [TestMethod]
        public void Should_Delete()
        {
            try
            {
                using (var scope = new TransactionScope())
                {
                    var result = Db.Default.CreateCommand("delete from [dbo].[Users] where Name=@name")
                          .AddParameter("@name", "user2")
                          .NonQuery();

                    var r2 = Db.Default.CreateCommand("delete from [dbo].[Users] where Name=@name")
                       .AddParameter("@name", "user4")
                       .NonQuery();

                    scope.Complete();

                    Assert.IsTrue(result > 0);
                }
            }
            catch (Exception ex)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public async Task Should_Insert_And_Delete_Async()
        {
            var result = await Db.Default.CreateCommand("insert into [dbo].[Users](Name,Email,Age,Birth) values(@name,@email,@age,@birth)")
                 .AddParameter("@name", "user3")
                 .AddParameter("@email", "user3@mail.com")
                 .AddParameter("@age", 20)
                 .AddParameter("@birth", DateTime.Now.AddYears(-20))
                 .NonQueryAsync();

            Assert.AreEqual(result, 1);

            var result2 = await Db.Default.CreateCommand("delete from [dbo].[Users] where Name=@name")
                 .AddParameter("@name", "user3")
                 .NonQueryAsync();

            Assert.AreEqual(result2, 1);
        }

        [TestMethod]
        public async Task Cannot_Delete_Not_Existing_Async()
        {
            var result = await Db.Default.CreateCommand("delete from [dbo].[Users] where id=@id")
                 .AddParameter("@id", 500000)
                 .NonQueryAsync();

            Assert.AreEqual(result, 0);
        }

        [TestMethod]
        public void Should_Map_One_With_Multiple_Attributes()
        {
            var user = Db.Default.CreateCommand("select * from [dbo].[Users] where id=@id")
                .AddParameter("@id", 11)
                .ReadOneMapTo<UserComplex>();

            Assert.AreEqual(user.UserId, 11);
            Assert.AreEqual(user.UserName, "Marie");
            Assert.AreEqual(user.UserEmail, "marie@mail.com");
            Assert.AreEqual(user.UserAge, null);
        }
    }
    [DataContract]
    public class UserComplex
    {
        
        [DataMember]
        [Map(ColumnName = "Id")]
        public int UserId { get; set; }

        [DataMember]
        [Map(ColumnName = "Name")]
        public string UserName { get; set; }

        [Map(ColumnName = "Email")]
        public string UserEmail { get; set; }

        [Map(ColumnName = "Age")]
        public int? UserAge { get; set; }
    }
}
