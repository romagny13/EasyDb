using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EasyDbLib.Tests.Core
{
    [TestClass]
    public class EasyDbQueryServiceTests
    {
        [ClassInitialize()]
        public static void ClassInit(TestContext context)
        {
            QueryServiceFactory.Reset();
        }

        [TestMethod]
        public void SetConnectionStringSettings_ChangeQueryService()
        {
            var db = new EasyDb();

            //Assert.IsNull(QueryServiceFactory.Current);

            // My SQL
            db.SetConnectionStringSettings(@"server=localhost;database=demo;uid=root", "MySql.Data.MySqlClient");

            Assert.AreEqual(typeof(MySqlQueryService), QueryServiceFactory.Current.GetType());

            db.SetConnectionStringSettings(@"Data Source=(LocalDb)\MSSQLLocalDB;Initial Catalog=testdb;Integrated Security=True", "System.Data.SqlClient");

            Assert.AreEqual(typeof(SqlQueryService), QueryServiceFactory.Current.GetType());
        }
    }
}
