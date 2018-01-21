using System;
using EasyDbLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EasyDbLib.Tests
{
    [TestClass]
    public class QueryServiceFactoryTests
    {
        [TestMethod]
        public void GetFail_WithNoProvider()
        {

            bool failed = false;
            try
            {
                QueryServiceFactory.GetQueryService("");
            }
            catch (Exception e)
            {
                failed = true;
            }
            Assert.IsTrue(failed);
        }

        [TestMethod]
        public void TestRegisterDefaults()
        {
            Assert.IsTrue(QueryServiceFactory.HasServiceFor("System.Data.SqlClient"));

            Assert.IsTrue(QueryServiceFactory.HasServiceFor("System.Data.OleDb"));

            var sqlService = QueryServiceFactory.GetQueryService("System.Data.SqlClient");

            var oledbService = QueryServiceFactory.GetQueryService("System.Data.OleDb");

            Assert.AreEqual(typeof(SqlQueryService), sqlService.GetType());
            Assert.AreEqual(typeof(OleDbQueryService), oledbService.GetType());
        }

        [TestMethod]
        public void TestRegisterNew()
        {
            QueryServiceFactory.RegisterQueryService("MyProvider", new MyCustomQueryService1());

            Assert.IsTrue(QueryServiceFactory.HasServiceFor("MyProvider"));

            var service = QueryServiceFactory.GetQueryService("MyProvider");

            Assert.AreEqual(typeof(MyCustomQueryService1), service.GetType());
        }

        [TestMethod]
        public void Register_WithNoProvider_Fail()
        {

            bool failed = false;
            try
            {
                QueryServiceFactory.RegisterQueryService("", new SqlQueryService());
            }
            catch (Exception e)
            {
                failed = true;
            }
            Assert.IsTrue(failed);
        }

        [TestMethod]
        public void Register_WithNoService_Fail()
        {

            bool failed = false;
            try
            {
                QueryServiceFactory.RegisterQueryService("System.Data.SqlClient", null);
            }
            catch (Exception e)
            {
                failed = true;
            }
            Assert.IsTrue(failed);
        }

        [TestMethod]
        public void TestSet()
        {
            QueryServiceFactory.RegisterQueryService("System.Data.SqlClient", new MyCustomQueryService());

            Assert.IsTrue(QueryServiceFactory.HasServiceFor("System.Data.SqlClient"));

            var service = QueryServiceFactory.GetQueryService("System.Data.SqlClient");

            Assert.AreEqual(typeof(MyCustomQueryService), service.GetType());
        }

        [TestMethod]
        public void Get_WithNoRegistered_Fail()
        {

            bool failed = false;
            try
            {
                QueryServiceFactory.GetQueryService("MyFake");
            }
            catch (Exception e)
            {
                failed = true;
            }
            Assert.IsTrue(failed);
        }

    }

    public class MyCustomQueryService : SqlQueryService
    {
        //public MyCustomQueryService()
        //    : base("`", "`")
        //{ }

        // override methods ... example 
    }

    public class MyCustomQueryService1 : SqlQueryService
    { }
}
