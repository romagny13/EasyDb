using System;
using System.Collections.Generic;
using System.Data.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EasyDbLib.Tests.Core
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
            service.OnSelectAllExecuted(command, new DbInterceptionContext(result));

            Assert.AreEqual(true, i1.SelectAllExecutedCalled);
            Assert.AreEqual(command, i1.SelectAllExecutedCommand);
            Assert.IsNotNull(i1.SelectAllExecutedContext);
            Assert.AreEqual(result, i1.SelectAllExecutedContext.Result);

            Assert.AreEqual(true, i2.SelectAllExecutedCalled);
            Assert.AreEqual(command, i2.SelectAllExecutedCommand);
            Assert.IsNotNull(i2.SelectAllExecutedContext);
            Assert.AreEqual(result, i2.SelectAllExecutedContext.Result);
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
            service.OnSelectOneExecuted(command, new DbInterceptionContext(result));

            Assert.AreEqual(true, i1.SelectOneExecutedCalled);
            Assert.AreEqual(command, i1.SelectOneExecutedCommand);
            Assert.IsNotNull(i1.SelectOneExecutedContext);
            Assert.AreEqual(result, i1.SelectOneExecutedContext.Result);

            Assert.AreEqual(true, i2.SelectOneExecutedCalled);
            Assert.AreEqual(command, i2.SelectOneExecutedCommand);
            Assert.IsNotNull(i2.SelectOneExecutedContext);
            Assert.AreEqual(result, i2.SelectOneExecutedContext.Result);
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
            service.OnScalarExecuted(command, new DbInterceptionContext(result));

            Assert.AreEqual(true, i1.ScalarExecutedCalled);
            Assert.AreEqual(command, i1.ScalarExecutedCommand);
            Assert.IsNotNull(i1.ScalarExecutedContext);
            Assert.AreEqual(result, i1.ScalarExecutedContext.Result);

            Assert.AreEqual(true, i2.ScalarExecutedCalled);
            Assert.AreEqual(command, i2.ScalarExecutedCommand);
            Assert.IsNotNull(i2.ScalarExecutedContext);
            Assert.AreEqual(result, i2.ScalarExecutedContext.Result);
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
            service.OnNonQueryExecuted(command, new DbInterceptionContext(result));

            Assert.AreEqual(true, i1.NonQueryExecutedCalled);
            Assert.AreEqual(command, i1.NonQueryExecutedCommand);
            Assert.IsNotNull(i1.NonQueryExecutedContext);
            Assert.AreEqual(result, i1.NonQueryExecutedContext.Result);

            Assert.AreEqual(true, i2.NonQueryExecutedCalled);
            Assert.AreEqual(command, i2.NonQueryExecutedCommand);
            Assert.IsNotNull(i2.NonQueryExecutedContext);
            Assert.AreEqual(result, i2.NonQueryExecutedContext.Result);
        }

        [TestMethod]
        public void Interceptor_AreNotified_OnInserting()
        {
            var service = new InterceptionManager();

            var i1 = new MyInterceptorOne();
            var i2 = new MyInterceptorTwo();
            service.AddInterceptor(i1);
            service.AddInterceptor(i2);

            var model = new User
            {
                UserName = "Marie"
            };

            DbCommand command = DbProviderFactories.GetFactory("System.Data.SqlClient").CreateCommand();
            service.OnInserting(command, model);

            Assert.AreEqual(true, i1.InsertingCalled);
            Assert.AreEqual(command, i1.InsertingCommand);
            Assert.AreEqual(model, i1.InsertingModel);

            Assert.AreEqual(true, i2.InsertingCalled);
            Assert.AreEqual(command, i2.InsertingCommand);
            Assert.AreEqual(model, i2.InsertingModel);
        }

        [TestMethod]
        public void Interceptor_AreNotified_OnInserted()
        {
            var service = new InterceptionManager();

            var i1 = new MyInterceptorOne();
            var i2 = new MyInterceptorTwo();
            service.AddInterceptor(i1);
            service.AddInterceptor(i2);

            var model = new User
            {
                UserName = "Marie"
            };

            var result = 10;
            DbCommand command = DbProviderFactories.GetFactory("System.Data.SqlClient").CreateCommand();
            service.OnInserted(command, new DbInterceptionContext(model, result));

            Assert.AreEqual(true, i1.InsertedCalled);
            Assert.AreEqual(command, i1.InsertedCommand);
            Assert.AreEqual(model, i1.InsertedContext.Model);
            Assert.AreEqual(result, i1.InsertedContext.Result);

            Assert.AreEqual(true, i2.InsertedCalled);
            Assert.AreEqual(command, i2.InsertedCommand);
            Assert.AreEqual(model, i2.InsertedContext.Model);
            Assert.AreEqual(result, i2.InsertedContext.Result);

            Assert.IsFalse(i1.SelectAllCalled);
            Assert.IsFalse(i1.SelectOneCalled);
            Assert.IsFalse(i1.InsertingCalled);
            Assert.IsFalse(i1.UpdatingCalled);
            Assert.IsFalse(i1.DeletingCalled);
            Assert.IsFalse(i1.SelectAllExecutedCalled);
            Assert.IsFalse(i1.SelectOneExecutedCalled);
            Assert.IsFalse(i1.UpdatedCalled);
            Assert.IsFalse(i1.DeletedCalled);
        }

        [TestMethod]
        public void Interceptor_AreNotified_OnUpdating()
        {
            var service = new InterceptionManager();

            var i1 = new MyInterceptorOne();
            var i2 = new MyInterceptorTwo();
            service.AddInterceptor(i1);
            service.AddInterceptor(i2);

            var model = new User
            {
                UserName = "Marie"
            };

            DbCommand command = DbProviderFactories.GetFactory("System.Data.SqlClient").CreateCommand();
            service.OnUpdating(command, model);

            Assert.AreEqual(true, i1.UpdatingCalled);
            Assert.AreEqual(command, i1.UpdatingCommand);
            Assert.AreEqual(model, i1.UpdatingModel);

            Assert.AreEqual(true, i2.UpdatingCalled);
            Assert.AreEqual(command, i2.UpdatingCommand);
            Assert.AreEqual(model, i2.UpdatingModel);

            Assert.IsFalse(i1.SelectAllCalled);
            Assert.IsFalse(i1.SelectOneCalled);
            Assert.IsFalse(i1.InsertingCalled);
            Assert.IsFalse(i1.DeletingCalled);
            Assert.IsFalse(i1.SelectAllExecutedCalled);
            Assert.IsFalse(i1.SelectOneExecutedCalled);
            Assert.IsFalse(i1.InsertedCalled);
            Assert.IsFalse(i1.UpdatedCalled);
            Assert.IsFalse(i1.DeletedCalled);
        }

        [TestMethod]
        public void Interceptor_AreNotified_OnUpdated()
        {
            var service = new InterceptionManager();

            var i1 = new MyInterceptorOne();
            var i2 = new MyInterceptorTwo();
            service.AddInterceptor(i1);
            service.AddInterceptor(i2);

            var model = new User
            {
                UserName = "Marie"
            };

            var result = 10;
            DbCommand command = DbProviderFactories.GetFactory("System.Data.SqlClient").CreateCommand();
            service.OnUpdated(command, new DbInterceptionContext(model, result));

            Assert.AreEqual(true, i1.UpdatedCalled);
            Assert.AreEqual(command, i1.UpdatedCommand);
            Assert.AreEqual(model, i1.UpdatedContext.Model);
            Assert.AreEqual(result, i1.UpdatedContext.Result);

            Assert.AreEqual(true, i2.UpdatedCalled);
            Assert.AreEqual(command, i2.UpdatedCommand);
            Assert.AreEqual(model, i2.UpdatedContext.Model);
            Assert.AreEqual(result, i2.UpdatedContext.Result);

            Assert.IsFalse(i1.SelectAllCalled);
            Assert.IsFalse(i1.SelectOneCalled);
            Assert.IsFalse(i1.InsertingCalled);
            Assert.IsFalse(i1.UpdatingCalled);
            Assert.IsFalse(i1.DeletingCalled);
            Assert.IsFalse(i1.SelectAllExecutedCalled);
            Assert.IsFalse(i1.SelectOneExecutedCalled);
            Assert.IsFalse(i1.InsertedCalled);
            Assert.IsFalse(i1.DeletedCalled);
        }


        [TestMethod]
        public void Interceptor_AreNotified_OnDeleting()
        {
            var service = new InterceptionManager();

            var i1 = new MyInterceptorOne();
            var i2 = new MyInterceptorTwo();
            service.AddInterceptor(i1);
            service.AddInterceptor(i2);

            var model = new User
            {
                UserName = "Marie"
            };

            DbCommand command = DbProviderFactories.GetFactory("System.Data.SqlClient").CreateCommand();
            service.OnDeleting(command, model);

            Assert.AreEqual(true, i1.DeletingCalled);
            Assert.AreEqual(command, i1.DeletingCommand);
            Assert.AreEqual(model, i1.DeletingModel);

            Assert.AreEqual(true, i2.DeletingCalled);
            Assert.AreEqual(command, i2.DeletingCommand);
            Assert.AreEqual(model, i2.DeletingModel);

            Assert.IsFalse(i1.SelectAllCalled);
            Assert.IsFalse(i1.SelectOneCalled);
            Assert.IsFalse(i1.InsertingCalled);
            Assert.IsFalse(i1.UpdatingCalled);
            Assert.IsFalse(i1.SelectAllExecutedCalled);
            Assert.IsFalse(i1.SelectOneExecutedCalled);
            Assert.IsFalse(i1.InsertedCalled);
            Assert.IsFalse(i1.UpdatedCalled);
            Assert.IsFalse(i1.DeletedCalled);
        }

        [TestMethod]
        public void Interceptor_AreNotified_OnDeleted()
        {
            var service = new InterceptionManager();

            var i1 = new MyInterceptorOne();
            var i2 = new MyInterceptorTwo();
            service.AddInterceptor(i1);
            service.AddInterceptor(i2);

            var model = new User
            {
                UserName = "Marie"
            };

            var result = 10;
            DbCommand command = DbProviderFactories.GetFactory("System.Data.SqlClient").CreateCommand();
            service.OnDeleted(command, new DbInterceptionContext(model, result));

            Assert.AreEqual(true, i1.DeletedCalled);
            Assert.AreEqual(command, i1.DeletedCommand);
            Assert.AreEqual(model, i1.DeletedContext.Model);
            Assert.AreEqual(result, i1.DeletedContext.Result);

            Assert.AreEqual(true, i2.DeletedCalled);
            Assert.AreEqual(command, i2.DeletedCommand);
            Assert.AreEqual(model, i2.DeletedContext.Model);
            Assert.AreEqual(result, i2.DeletedContext.Result);

            Assert.IsFalse(i1.SelectAllCalled);
            Assert.IsFalse(i1.SelectOneCalled);
            Assert.IsFalse(i1.InsertingCalled);
            Assert.IsFalse(i1.UpdatingCalled);
            Assert.IsFalse(i1.DeletingCalled);
            Assert.IsFalse(i1.SelectAllExecutedCalled);
            Assert.IsFalse(i1.SelectOneExecutedCalled);
            Assert.IsFalse(i1.InsertedCalled);
            Assert.IsFalse(i1.UpdatedCalled);
        }
    }

    public class MyInterceptorBase : DbInterceptor
    {
        public bool SelectAllCalled { get; set; }
        public DbCommand SelectAllCommand { get; set; }

        public bool SelectAllExecutedCalled { get; set; }
        public DbCommand SelectAllExecutedCommand { get; set; }
        public DbInterceptionContext SelectAllExecutedContext { get; set; }

        public bool SelectOneCalled { get; set; }
        public DbCommand SelectOneCommand { get; set; }

        public bool SelectOneExecutedCalled { get; set; }
        public DbCommand SelectOneExecutedCommand { get; set; }
        public DbInterceptionContext SelectOneExecutedContext { get; set; }

        public bool ScalarCalled { get; set; }
        public DbCommand ScalarCommand { get; set; }

        public bool ScalarExecutedCalled { get; set; }
        public DbCommand ScalarExecutedCommand { get; set; }
        public DbInterceptionContext ScalarExecutedContext { get; set; }

        public bool NonQueryCalled { get; set; }
        public DbCommand NonQueryCommand { get; set; }

        public bool NonQueryExecutedCalled { get; set; }
        public DbCommand NonQueryExecutedCommand { get; set; }
        public DbInterceptionContext NonQueryExecutedContext { get; set; }

        public bool InsertingCalled { get; set; }
        public DbCommand InsertingCommand { get; set; }
        public object InsertingModel { get; set; }

        public bool InsertedCalled { get; set; }
        public DbCommand InsertedCommand { get; set; }
        public DbInterceptionContext InsertedContext { get; set; }

        public bool UpdatingCalled { get; set; }
        public DbCommand UpdatingCommand { get; set; }
        public object UpdatingModel { get; set; }

        public bool UpdatedCalled { get; set; }
        public DbCommand UpdatedCommand { get; set; }
        public DbInterceptionContext UpdatedContext { get; set; }

        public bool DeletingCalled { get; set; }
        public DbCommand DeletingCommand { get; set; }
        public object DeletingModel { get; set; }

        public bool DeletedCalled { get; set; }
        public DbCommand DeletedCommand { get; set; }
        public DbInterceptionContext DeletedContext { get; set; }

        public override void OnSelectAllExecuting(DbCommand command)
        {
            SelectAllCalled = true;
            SelectAllCommand = command;
        }

        public override void OnSelectAllExecuted(DbCommand command, DbInterceptionContext interceptionContext)
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

        public override void OnSelectOneExecuted(DbCommand command, DbInterceptionContext interceptionContext)
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

        public override void OnScalarExecuted(DbCommand command, DbInterceptionContext interceptionContext)
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

        public override void OnNonQueryExecuted(DbCommand command, DbInterceptionContext interceptionContext)
        {
            NonQueryExecutedCalled = true;
            NonQueryExecutedCommand = command;
            NonQueryExecutedContext = interceptionContext;
        }

        public override void OnInserting(DbCommand command, object model)
        {
            InsertingCalled = true;
            InsertingCommand = command;
            InsertingModel = model;
        }

        public override void OnInserted(DbCommand command, DbInterceptionContext interceptionContext)
        {
            InsertedCalled = true;
            InsertedCommand = command;
            InsertedContext = interceptionContext;
        }

        public override void OnUpdating(DbCommand command, object model)
        {
            UpdatingCalled = true;
            UpdatingCommand = command;
            UpdatingModel = model;
        }

        public override void OnUpdated(DbCommand command, DbInterceptionContext interceptionContext)
        {
            UpdatedCalled = true;
            UpdatedCommand = command;
            UpdatedContext = interceptionContext;
        }

        public override void OnDeleting(DbCommand command, object model)
        {
            DeletingCalled = true;
            DeletingCommand = command;
            DeletingModel = model;
        }

        public override void OnDeleted(DbCommand command, DbInterceptionContext interceptionContext)
        {
            DeletedCalled = true;
            DeletedCommand = command;
            DeletedContext = interceptionContext;
        }
    }

    public class MyInterceptorOne : MyInterceptorBase
    {

    }

    public class MyInterceptorTwo : MyInterceptorBase
    {

    }
}
