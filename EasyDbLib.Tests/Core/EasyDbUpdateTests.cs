using EasyDbLib.Tests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.Common;
using System.Threading.Tasks;

namespace EasyDbLib.Tests.Core
{
    [TestClass]
    public class EasyDbUpdateTests
    {
        [ClassInitialize()]
        public static void ClassInit(TestContext context)
        {
            InitDb.CreateDbLikeProperties();
            InitDb.CreateDbTestLikeMySql();
        }

        [TestMethod]
        public async Task Update()
        {
            var db = new EasyDb();

            db.SetConnectionStringSettings(DbConstants.SqlFile, DbConstants.SqlProviderName);

            db.DefaultMappingBehavior = DefaultMappingBehavior.CreateEmptyTable;

            var model = new User
            {
                Id = 2,
                UserName = "updated"
            };

            var result = await db.UpdateAsync<User>(model, Check.Op("Id", model.Id));

            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public async Task Update_WithCommand()
        {
            var db = new EasyDb();

            db.SetConnectionStringSettings(DbConstants.SqlFile, DbConstants.SqlProviderName);

            db.DefaultMappingBehavior = DefaultMappingBehavior.CreateEmptyTable;

            int result = 0;
            using (var command = db.CreateSqlCommand("update [User] set UserName=@username where Id=@id")
                 .AddInParameter("@username", "updated")
                .AddInParameter("@id", 3))
            {
                result = await db.UpdateAsync<User>(command);
            }

            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public async Task Update_WithFactory()
        {
            var db = new EasyDb();

            db.SetConnectionStringSettings(DbConstants.SqlFile, DbConstants.SqlProviderName);

            db.DefaultMappingBehavior = DefaultMappingBehavior.CreateEmptyTable;

            var model = new User
            {
                Id = 4,
                UserName = "updated"
            };

            int result = await db.UpdateAsync<User>(new UserUdpateFactory(), model);

            Assert.AreEqual(1, result);
        }
    }

    public class UserUdpateFactory : IUpdateCommandFactory<User>
    {
        public DbCommand CreateCommand(EasyDb db, User model)
        {
            return db.CreateSqlCommand("update [User] set UserName=@username where Id=@id")
                    .AddInParameter("@username", model.UserName)
                   .AddInParameter("@id", model.Id);
        }
    }
}
