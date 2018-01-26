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

            db.DefaultMappingBehavior = DefaultMappingBehavior.CreateEmptyTable;

            db.SetTable<User>("User")
                .SetPrimaryKeyColumn("Id", p => p.Id);

            db.SetConnectionStringSettings(DbConstants.SqlDb1, DbConstants.SqlProviderName);

            var model = await db.SelectOneAsync<User>(Check.Op("Id", 2));
            model.UserName += " updated";
            var result = await db.UpdateAsync<User>(model, Check.Op("Id", model.Id));

            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public async Task Update_WithFactory()
        {
            var db = new EasyDb();

            db.SetConnectionStringSettings(DbConstants.SqlDb1, DbConstants.SqlProviderName);

            db.DefaultMappingBehavior = DefaultMappingBehavior.CreateEmptyTable;

            var model = await db.SelectOneAsync<User>(Check.Op("Id", 4));
            model.UserName += " updated";
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
