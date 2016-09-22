using EasyDb;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;


namespace EasyDbTests
{
    [TestClass]
    public class TableWithGuidTests
    {
        [TestMethod]
        public void Should_Get_New_Guid_Key()
        {
            var db = new Db();
            db.Initialize(TestConstants.SqlClientConnectionString, "System.Data.SqlClient");

            var newGuid = db.CreateCommand("insert into [dbo].[UsersWithGuid](Name,Email) output INSERTED.Id values(@name,@email)")
                           .AddParameter("@name", "user1")
                           .AddParameter("@email", "user1@mail.com")
                           .Scalar();

            Assert.IsFalse(string.IsNullOrEmpty(newGuid.ToString()));
        }

        [TestMethod]
        public void Should_Map_One_With_Guid()
        {
            var db = new Db();
            db.Initialize(TestConstants.SqlClientConnectionString, "System.Data.SqlClient");

            var user = db.CreateCommand("select * from [dbo].[UsersWithGuid] where id=@id")
                .AddParameter("@id", "39544cae-1342-4ab4-9d3c-921ffd93ac62")
                .ReadOneMapTo<UserWithGuid>();

            Assert.AreEqual(user.UserName, "Marie");
        }

        [TestMethod]
        public void Should_Read_One_With_Guid()
        {
            var db = new Db();
            db.Initialize(TestConstants.SqlClientConnectionString, "System.Data.SqlClient");

            var user = db.CreateCommand("select * from [dbo].[UsersWithGuid] where id=@id")
                .AddParameter("@id", "39544cae-1342-4ab4-9d3c-921ffd93ac62")
                .ReadOne<UserWithGuid>((reader) =>
                {
                    return new UserWithGuid
                    {
                        Id = (Guid)reader["Id"],
                        UserName = (string)reader["Name"]
                    };
                });

            Assert.AreEqual(user.UserName.TrimEnd(), "Marie");
        }
    }


    public class UserWithGuid
    {
        public Guid Id { get; set; }
        [MapAttribute(ColumnName = "Name")]
        public string UserName { get; set; }
        public string Email { get; set; }
    }

}
