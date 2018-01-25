using EasyDbLib.Tests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.Common;
using System.Threading.Tasks;

namespace EasyDbLib.Tests.Core
{

    [TestClass]
    public class EasyDbDeleteTests
    {
        [ClassInitialize()]
        public static void ClassInit(TestContext context)
        {
            InitDb.CreateDbLikeProperties();
            InitDb.CreateDbTestLikeMySql();
        }

        [TestMethod]
        public async Task Delete()
        {
            var db = new EasyDb();

            db.SetConnectionStringSettings(DbConstants.SqlDb1, DbConstants.SqlProviderName);

            db.DefaultMappingBehavior = DefaultMappingBehavior.CreateEmptyTable;

            var result = await db.DeleteAsync<User>(Check.Op("Id", 3));

            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public async Task Delete_WithCommand()
        {
            var db = new EasyDb();

            db.SetConnectionStringSettings(DbConstants.SqlDb1, DbConstants.SqlProviderName);

            db.DefaultMappingBehavior = DefaultMappingBehavior.CreateEmptyTable;

            int result = 0;
            using (var command = db.CreateSqlCommand("delete from [User] where Id=@id")
                .AddInParameter("@id", 4))
            {
                result = await db.DeleteAsync<User>(command);
            }

            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public async Task Delete_WithFactory()
        {
            var db = new EasyDb();

            db.SetConnectionStringSettings(DbConstants.SqlDb1, DbConstants.SqlProviderName);

            db.DefaultMappingBehavior = DefaultMappingBehavior.CreateEmptyTable;

            var post = new Post
            {
                Id = 1
            };

            int result = await db.DeleteAsync<Post>(new DeleteUserFactory(), post);

            Assert.AreEqual(1, result);
        }
    }

    public class DeleteUserFactory : IDeleteCommandFactory<Post>
    {
        public DbCommand CreateCommand(EasyDb db, Post model)
        {
            return db.CreateSqlCommand("delete from [Post] where Id=@id")
                 .AddInParameter("@id", model.Id);
        }
    }
}
