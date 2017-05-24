using System;
using EasyDbLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EasyDbLibTest
{
    [TestClass]
    public class QueryServiceFactoryTest
    {
        [TestMethod]
        public void TestRegisterDefaults()
        {
            Assert.IsTrue(QueryServiceFactory.Has("System.Data.SqlClient"));
            Assert.IsTrue(QueryServiceFactory.Has("System.Data.OleDb"));

            var sqlService = QueryServiceFactory.Get("System.Data.SqlClient");
            var oledbService = QueryServiceFactory.Get("System.Data.OleDb");

            Assert.AreEqual(typeof(QueryService), sqlService.GetType());
            Assert.AreEqual(typeof(QueryService), oledbService.GetType());
        }

        [TestMethod]
        public void TestRegisterNew()
        {
            QueryServiceFactory.Set("MySql.Data.MySqlClient", new QueryService());

            Assert.IsTrue(QueryServiceFactory.Has("MySql.Data.MySqlClient"));

            var service = QueryServiceFactory.Get("System.Data.SqlClient");

            Assert.AreEqual(typeof(QueryService), service.GetType());
        }

        [TestMethod]
        public void TestReplace()
        {
            QueryServiceFactory.Set("System.Data.SqlClient", new MyCustomQueryService());

            Assert.IsTrue(QueryServiceFactory.Has("System.Data.SqlClient"));

            var service = QueryServiceFactory.Get("System.Data.SqlClient");

            Assert.AreEqual(typeof(MyCustomQueryService), service.GetType());
        }

        [TestMethod]
        public void TestFailWithNoRegistered()
        {

            bool failed = false;
            try
            {
                QueryServiceFactory.Get("MyFake");
            }
            catch (Exception e)
            {
                failed = true;
            }
            Assert.IsTrue(failed);
        }

    }

    public class MyCustomQueryService : QueryService
    {
        //public MyCustomQueryService()
        //    : base("`", "`")
        //{ }
    }
}
