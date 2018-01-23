using System;
using System.Collections.Generic;
using System.Data.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EasyDbLib.Tests
{
    [TestClass]
    public class DbInterceptorTests
    {
        [TestMethod]
        public void Register_Unregister()
        {
            var service = new InterceptionManager();

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
        public void Interceptor_AreNotified_OnSelectAllExecuting()
        {
            var service = new InterceptionManager();

            var i1 = new MyInterceptorOne();
            var i2 = new MyInterceptorTwo();
            service.AddInterceptor(i1);
            service.AddInterceptor(i2);


            DbCommand command = DbProviderFactories.GetFactory("System.Data.SqlClient").CreateCommand();
            service.OnSelectAllExecuting(command);

            Assert.AreEqual(true, i1.SelectAllCalled);
            Assert.AreEqual(command, i1.SelectAllCommand);

            Assert.AreEqual(true, i2.SelectAllCalled);
            Assert.AreEqual(command, i2.SelectAllCommand);
        }

        [TestMethod]
        public void Interceptor_AreNotified_OnSelectAllExecuted()
        {
            var service = new InterceptionManager();

            var i1 = new MyInterceptorOne();
            var i2 = new MyInterceptorTwo();
            service.AddInterceptor(i1);
            service.AddInterceptor(i2);

            var result = new List<string> { "a", "b" };
            DbCommand command = DbProviderFactories.GetFactory("System.Data.SqlClient").CreateCommand();
            service.OnSelectAllExecuted(command, new DbInterceptionContext<List<string>>(result));

            Assert.AreEqual(true, i1.SelectAllExecutedCalled);
            Assert.AreEqual(command, i1.SelectAllExecutedCommand);
            Assert.IsNotNull(i1.SelectAllExecutedContext);
            Assert.AreEqual(result, ((DbInterceptionContext<List<string>>)i1.SelectAllExecutedContext).Result);

            Assert.AreEqual(true, i2.SelectAllExecutedCalled);
            Assert.AreEqual(command, i2.SelectAllExecutedCommand);
            Assert.IsNotNull(i2.SelectAllExecutedContext);
            Assert.AreEqual(result, ((DbInterceptionContext<List<string>>)i2.SelectAllExecutedContext).Result);
        }

        [TestMethod]
        public void Interceptor_AreNotified_OnSelectOneExecuting()
        {
            var service = new InterceptionManager();

            var i1 = new MyInterceptorOne();
            var i2 = new MyInterceptorTwo();
            service.AddInterceptor(i1);
            service.AddInterceptor(i2);


            DbCommand command = DbProviderFactories.GetFactory("System.Data.SqlClient").CreateCommand();
            service.OnSelectOneExecuting(command);

            Assert.AreEqual(true, i1.SelectOneCalled);
            Assert.AreEqual(command, i1.SelectOneCommand);

            Assert.AreEqual(true, i2.SelectOneCalled);
            Assert.AreEqual(command, i2.SelectOneCommand);
        }

        [TestMethod]
        public void Interceptor_AreNotified_OnSelectOneExecuted()
        {
            var service = new InterceptionManager();

            var i1 = new MyInterceptorOne();
            var i2 = new MyInterceptorTwo();
            service.AddInterceptor(i1);
            service.AddInterceptor(i2);

            var result = "a";
            DbCommand command = DbProviderFactories.GetFactory("System.Data.SqlClient").CreateCommand();
            service.OnSelectOneExecuted(command, new DbInterceptionContext<string>(result));

            Assert.AreEqual(true, i1.SelectOneExecutedCalled);
            Assert.AreEqual(command, i1.SelectOneExecutedCommand);
            Assert.IsNotNull(i1.SelectOneExecutedContext);
            Assert.AreEqual(result, ((DbInterceptionContext<string>)i1.SelectOneExecutedContext).Result);

            Assert.AreEqual(true, i2.SelectOneExecutedCalled);
            Assert.AreEqual(command, i2.SelectOneExecutedCommand);
            Assert.IsNotNull(i2.SelectOneExecutedContext);
            Assert.AreEqual(result, ((DbInterceptionContext<string>)i2.SelectOneExecutedContext).Result);
        }

        [TestMethod]
        public void Interceptor_AreNotified_OnScalarExecuting()
        {
            var service = new InterceptionManager();

            var i1 = new MyInterceptorOne();
            var i2 = new MyInterceptorTwo();
            service.AddInterceptor(i1);
            service.AddInterceptor(i2);


            DbCommand command = DbProviderFactories.GetFactory("System.Data.SqlClient").CreateCommand();
            service.OnScalarExecuting(command);

            Assert.AreEqual(true, i1.ScalarCalled);
            Assert.AreEqual(command, i1.ScalarCommand);

            Assert.AreEqual(true, i2.ScalarCalled);
            Assert.AreEqual(command, i2.ScalarCommand);
        }

        [TestMethod]
        public void Interceptor_AreNotified_OnScalarExecuted()
        {
            var service = new InterceptionManager();

            var i1 = new MyInterceptorOne();
            var i2 = new MyInterceptorTwo();
            service.AddInterceptor(i1);
            service.AddInterceptor(i2);

            var result = 10;
            DbCommand command = DbProviderFactories.GetFactory("System.Data.SqlClient").CreateCommand();
            service.OnScalarExecuted(command, new DbInterceptionContext<object>(result));

            Assert.AreEqual(true, i1.ScalarExecutedCalled);
            Assert.AreEqual(command, i1.ScalarExecutedCommand);
            Assert.IsNotNull(i1.ScalarExecutedContext);
            Assert.AreEqual(result, ((DbInterceptionContext<object>)i1.ScalarExecutedContext).Result);

            Assert.AreEqual(true, i2.ScalarExecutedCalled);
            Assert.AreEqual(command, i2.ScalarExecutedCommand);
            Assert.IsNotNull(i2.ScalarExecutedContext);
            Assert.AreEqual(result, ((DbInterceptionContext<object>)i2.ScalarExecutedContext).Result);
        }

        [TestMethod]
        public void Interceptor_AreNotified_OnNonQueryExecuting()
        {
            var service = new InterceptionManager();

            var i1 = new MyInterceptorOne();
            var i2 = new MyInterceptorTwo();
            service.AddInterceptor(i1);
            service.AddInterceptor(i2);


            DbCommand command = DbProviderFactories.GetFactory("System.Data.SqlClient").CreateCommand();
            service.OnNonQueryExecuting(command);

            Assert.AreEqual(true, i1.NonQueryCalled);
            Assert.AreEqual(command, i1.NonQueryCommand);

            Assert.AreEqual(true, i2.NonQueryCalled);
            Assert.AreEqual(command, i2.NonQueryCommand);
        }

        [TestMethod]
        public void Interceptor_AreNotified_OnNonQueryExecuted()
        {
            var service = new InterceptionManager();

            var i1 = new MyInterceptorOne();
            var i2 = new MyInterceptorTwo();
            service.AddInterceptor(i1);
            service.AddInterceptor(i2);

            var result = 10;
            DbCommand command = DbProviderFactories.GetFactory("System.Data.SqlClient").CreateCommand();
            service.OnNonQueryExecuted(command, new DbInterceptionContext<int>(result));

            Assert.AreEqual(true, i1.NonQueryExecutedCalled);
            Assert.AreEqual(command, i1.NonQueryExecutedCommand);
            Assert.IsNotNull(i1.NonQueryExecutedContext);
            Assert.AreEqual(result, ((DbInterceptionContext<int>)i1.NonQueryExecutedContext).Result);

            Assert.AreEqual(true, i2.NonQueryExecutedCalled);
            Assert.AreEqual(command, i2.NonQueryExecutedCommand);
            Assert.IsNotNull(i2.NonQueryExecutedContext);
            Assert.AreEqual(result, ((DbInterceptionContext<int>)i2.NonQueryExecutedContext).Result);
        }


    }


    public class MyInterceptorBase : DbInterceptor
    {
        public bool SelectAllCalled { get; set; }
        public DbCommand SelectAllCommand { get; set; }

        public bool SelectAllExecutedCalled { get; set; }
        public DbCommand SelectAllExecutedCommand { get; set; }
        public object SelectAllExecutedContext { get; set; }

        public bool SelectOneCalled { get; set; }
        public DbCommand SelectOneCommand { get; set; }

        public bool SelectOneExecutedCalled { get; set; }
        public DbCommand SelectOneExecutedCommand { get; set; }
        public object SelectOneExecutedContext { get; set; }

        public bool ScalarCalled { get; set; }
        public DbCommand ScalarCommand { get; set; }

        public bool ScalarExecutedCalled { get; set; }
        public DbCommand ScalarExecutedCommand { get; set; }
        public object ScalarExecutedContext { get; set; }

        public bool NonQueryCalled { get; set; }
        public DbCommand NonQueryCommand { get; set; }

        public bool NonQueryExecutedCalled { get; set; }
        public DbCommand NonQueryExecutedCommand { get; set; }
        public object NonQueryExecutedContext { get; set; }


        public override void OnSelectAllExecuting(DbCommand command)
        {
            SelectAllCalled = true;
            SelectAllCommand = command;
        }

        public override void OnSelectAllExecuted<TModel>(DbCommand command, DbInterceptionContext<List<TModel>> interceptionContext)
        {
            SelectAllExecutedCalled = true;
            SelectAllExecutedCommand = command;
            SelectAllExecutedContext = interceptionContext;
        }

        public override void OnSelectOneExecuting(DbCommand command)
        {
            SelectOneCalled = true;
            SelectOneCommand = command;
        }

        public override void OnSelectOneExecuted<TModel>(DbCommand command, DbInterceptionContext<TModel> interceptionContext)
        {
            SelectOneExecutedCalled = true;
            SelectOneExecutedCommand = command;
            SelectOneExecutedContext = interceptionContext;
        }

        public override void OnScalarExecuting(DbCommand command)
        {
            ScalarCalled = true;
            ScalarCommand = command;
        }

        public override void OnScalarExecuted(DbCommand command, DbInterceptionContext<object> interceptionContext)
        {
            ScalarExecutedCalled = true;
            ScalarExecutedCommand = command;
            ScalarExecutedContext = interceptionContext;
        }

        public override void OnNonQueryExecuting(DbCommand command)
        {
            NonQueryCalled = true;
            NonQueryCommand = command;
        }

        public override void OnNonQueryExecuted(DbCommand command, DbInterceptionContext<int> interceptionContext)
        {
            NonQueryExecutedCalled = true;
            NonQueryExecutedCommand = command;
            NonQueryExecutedContext = interceptionContext;
        }
    }

    public class MyInterceptorOne : MyInterceptorBase
    {

    }

    public class MyInterceptorTwo : MyInterceptorBase
    {

    }
}
