using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace EasyDbLib.Tests.Core
{
    [TestClass]
    public class EasyDbTransactionOperationTests
    {
        [ClassInitialize()]
        public static void ClassInit(TestContext context)
        {
            InitDb.CreateDbLikeProperties();
            InitDb.CreateDbTestLikeMySql();
        }

        [TestMethod]
        public async Task DoTransaction()
        {
            var db = new EasyDb();

            db.SetConnectionStringSettings(DbConstants.SqlDb1, DbConstants.SqlProviderName);

            db.DefaultMappingBehavior = DefaultMappingBehavior.CreateEmptyTable;

            var result = await db.ExecuteTransactionFactoryAsync(new MyTransactionOperations1());

            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task PendingTransaction_Fail()
        {
            var db = new EasyDb();

            db.SetConnectionStringSettings(DbConstants.SqlDb1, DbConstants.SqlProviderName);

            db.DefaultMappingBehavior = DefaultMappingBehavior.CreateEmptyTable;

            var result = await db.ExecuteTransactionFactoryAsync(new MyTransactionOperations2());

            Assert.IsFalse(result);
        }
    }

    public class MyTransactionOperations1 : TransactionFactory
    {
        public override async Task ExecuteAsync(EasyDb db)
        {
            try
            {
                await db.InsertAsync<User>(new User { UserName = "u1" });
                await db.InsertAsync<User>(new User { UserName = "u2" });
                await db.InsertAsync<User>(new User { UserName = "u3" });
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
    }

    public class MyTransactionOperations2 : TransactionFactory
    {
        public override async Task ExecuteAsync(EasyDb db)
        {
            try
            {
                await db.InsertAsync<User>(new User { UserName = "u4" });

                var user = await db.SelectOneAsync<User>(Check.Op("Id", 1));
                // constraint on user permission
                await db.DeleteAsync<User>(user, Check.Op("Id", 1));
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
    }

}
