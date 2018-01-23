using EasyDbLib.Tests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EasyDbLib.Tests.Core
{
    [TestClass]
    public class EasyDbInterceptionTests
    {
        [ClassInitialize()]
        public static void ClassInit(TestContext context)
        {
            InitDb.CreateDbLikeProperties();
            InitDb.CreateDbTestLikeMySql();
        }

        public EasyDb GetService()
        {
            var db = new EasyDb();
            db.SetConnectionStringSettings(DbConstants.SqlFile, DbConstants.SqlProviderName);
            return db;
        }

        [TestMethod]
        public async Task Register_Unregister()
        {
            var service = GetService();

            var i1 = new MyInterceptorOne();
            var i2 = new MyInterceptorTwo();
            service.AddInterceptor(i1);
            service.AddInterceptor(i2);

            Assert.AreEqual(2, service.Interceptors.Count);

            service.RemoveInterceptor(i2);

            Assert.AreEqual(1, service.Interceptors.Count);

            Assert.AreEqual("MyInterceptorOne", service.Interceptors[0].GetType().Name);

            service.RemoveInterceptor(i1);

            Assert.AreEqual(0, service.Interceptors.Count);
        }

        [TestMethod]
        public async Task Interceptor_AreNotified_OnSelectAllExecuting()
        {
            var service = GetService();

            var i1 = new MyInterceptorOne();
            var i2 = new MyInterceptorTwo();
            service.AddInterceptor(i1);
            service.AddInterceptor(i2);

            await service.SelectAllAsync<User>();

            Assert.AreEqual(true, i1.SelectAllCalled);
            Assert.AreEqual("select [Id],[UserName],[Age],[Email] from [User]", i1.SelectAllCommand.CommandText);

            Assert.AreEqual(true, i2.SelectAllCalled);
            Assert.AreEqual("select [Id],[UserName],[Age],[Email] from [User]", i2.SelectAllCommand.CommandText);
        }

        [TestMethod]
        public async Task Interceptor_AreNotified_OnSelectAllExecuted()
        {
            var service = GetService();

            var i1 = new MyInterceptorOne();
            var i2 = new MyInterceptorTwo();
            service.AddInterceptor(i1);
            service.AddInterceptor(i2);

            await service.SelectAllAsync<User>();

            var result = ((DbInterceptionContext<List<User>>)i1.SelectAllExecutedContext).Result;
            Assert.AreEqual(4, result.Count);

            Assert.AreEqual(true, i1.SelectAllExecutedCalled);
            Assert.AreEqual("select [Id],[UserName],[Age],[Email] from [User]", i1.SelectAllExecutedCommand.CommandText);
            Assert.IsNotNull(i1.SelectAllExecutedContext);

            Assert.AreEqual(true, i2.SelectAllExecutedCalled);
            Assert.AreEqual("select [Id],[UserName],[Age],[Email] from [User]", i2.SelectAllExecutedCommand.CommandText);
            Assert.IsNotNull(i2.SelectAllExecutedContext);
        }

        [TestMethod]
        public async Task Interceptor_AreNotified_OnSelectOneExecuting()
        {
            var service = GetService();

            var i1 = new MyInterceptorOne();
            var i2 = new MyInterceptorTwo();
            service.AddInterceptor(i1);
            service.AddInterceptor(i2);


            await service.SelectOneAsync<User>(Check.Op("Id", 2));

            Assert.AreEqual(true, i1.SelectOneCalled);
            Assert.IsNotNull(i1.SelectOneCommand);

            Assert.AreEqual(true, i2.SelectOneCalled);
            Assert.IsNotNull(i2.SelectOneCommand);
        }

        [TestMethod]
        public async Task Interceptor_AreNotified_OnSelectOneExecuted()
        {
            var service = GetService();

            var i1 = new MyInterceptorOne();
            var i2 = new MyInterceptorTwo();
            service.AddInterceptor(i1);
            service.AddInterceptor(i2);

            await service.SelectOneAsync<User>(Check.Op("Id", 2));

            var result = ((DbInterceptionContext<User>)i1.SelectOneExecutedContext).Result;

            Assert.AreEqual(true, i1.SelectOneExecutedCalled);
            Assert.IsNotNull(i1.SelectOneExecutedCommand);
            Assert.IsNotNull(i1.SelectOneExecutedContext);
            Assert.AreEqual("Pat", result.UserName);

            Assert.AreEqual(true, i2.SelectOneExecutedCalled);
            Assert.IsNotNull(i2.SelectOneExecutedCommand);
            Assert.IsNotNull(i2.SelectOneExecutedContext);
        }

        [TestMethod]
        public async Task Interceptor_AreNotified_OnScalarExecuting()
        {
            var service = GetService();

            var i1 = new MyInterceptorOne();
            var i2 = new MyInterceptorTwo();
            service.AddInterceptor(i1);
            service.AddInterceptor(i2);

            await service.CountAsync<User>(null);

            Assert.AreEqual(true, i1.ScalarCalled);
            Assert.IsNotNull(i1.ScalarCommand);

            Assert.AreEqual(true, i2.ScalarCalled);
            Assert.IsNotNull(i2.ScalarCommand);
        }

        [TestMethod]
        public async Task Interceptor_AreNotified_OnScalarExecuted()
        {
            var service = GetService();

            var i1 = new MyInterceptorOne();
            var i2 = new MyInterceptorTwo();
            service.AddInterceptor(i1);
            service.AddInterceptor(i2);

            await service.CountAsync<User>(null);

            Assert.AreEqual(true, i1.ScalarExecutedCalled);
            Assert.IsNotNull(i1.ScalarExecutedCommand);
            Assert.IsNotNull(i1.ScalarExecutedContext);
            Assert.AreEqual(4, ((DbInterceptionContext<object>)i1.ScalarExecutedContext).Result);

            Assert.AreEqual(true, i2.ScalarExecutedCalled);
            Assert.IsNotNull(i2.ScalarExecutedCommand);
            Assert.IsNotNull(i2.ScalarExecutedContext);
            Assert.AreEqual(4, ((DbInterceptionContext<object>)i2.ScalarExecutedContext).Result);
        }

        [TestMethod]
        public async Task Interceptor_AreNotified_OnNonQueryExecuting()
        {
            var service = GetService();

            var i1 = new MyInterceptorOne();
            var i2 = new MyInterceptorTwo();
            service.AddInterceptor(i1);
            service.AddInterceptor(i2);

            var model = new User { Id = 3, UserName = "Updated User" };
            await service.UpdateAsync<User>(model, Check.Op("Id", model.Id));

            Assert.AreEqual(true, i1.NonQueryCalled);
            Assert.IsNotNull(i1.NonQueryCommand);

            Assert.AreEqual(true, i2.NonQueryCalled);
            Assert.IsNotNull(i2.NonQueryCommand);
        }

        [TestMethod]
        public async Task Interceptor_AreNotified_OnNonQueryExecuted()
        {
            var service = GetService();

            var i1 = new MyInterceptorOne();
            var i2 = new MyInterceptorTwo();
            service.AddInterceptor(i1);
            service.AddInterceptor(i2);

            var model = new User { Id = 3, UserName = "Updated User" };
            await service.UpdateAsync<User>(model, Check.Op("Id", model.Id));

            Assert.AreEqual(true, i1.NonQueryExecutedCalled);
            Assert.IsNotNull(i1.NonQueryExecutedCommand);
            Assert.IsNotNull(i1.NonQueryExecutedContext);
            Assert.AreEqual(1,((DbInterceptionContext<int>)i1.NonQueryExecutedContext).Result);

            Assert.AreEqual(true, i2.NonQueryExecutedCalled);
            Assert.IsNotNull(i2.NonQueryExecutedCommand);
            Assert.IsNotNull(i2.NonQueryExecutedContext);
            Assert.AreEqual(1, ((DbInterceptionContext<int>)i2.NonQueryExecutedContext).Result);
        }

    }

}
