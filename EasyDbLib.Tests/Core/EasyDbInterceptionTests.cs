using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Data.Common;
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
            db.SetConnectionStringSettings(DbConstants.SqlDb1, DbConstants.SqlProviderName);
            return db;
        }

        [TestMethod]
        public async Task RunTest()
        {
            await OnSelectAllExecuting();

            await OnSelectAllExecuted();

            await OnSelectOneExecuting();

            await OnSelectOneExecuted();

            await OnScalarExecuting();

            await OnScalarExecuted();

            await OnNonQueryExecuting();

            await OnNonQueryExecuted();

            await OnInserting();

            await OnInserted();

            await OnUpdating();

            await OnUpdated();

            await OnDeleting();

            await OnDeleted();
        }

        [TestMethod]
        public void Register_Unregister()
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

        public async Task OnSelectAllExecuting()
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

        public async Task OnSelectAllExecuted()
        {
            var service = GetService();

            var i1 = new MyInterceptorOne();
            var i2 = new MyInterceptorTwo();
            service.AddInterceptor(i1);
            service.AddInterceptor(i2);

            await service.SelectAllAsync<User>();

            var result = (List<User>)i1.SelectAllExecutedContext.Result;
            Assert.AreEqual(4, result.Count);

            Assert.AreEqual(true, i1.SelectAllExecutedCalled);
            Assert.AreEqual("select [Id],[UserName],[Age],[Email] from [User]", i1.SelectAllExecutedCommand.CommandText);
            Assert.IsNotNull(i1.SelectAllExecutedContext);

            Assert.AreEqual(true, i2.SelectAllExecutedCalled);
            Assert.AreEqual("select [Id],[UserName],[Age],[Email] from [User]", i2.SelectAllExecutedCommand.CommandText);
            Assert.IsNotNull(i2.SelectAllExecutedContext);
        }

        public async Task OnSelectOneExecuting()
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

        public async Task OnSelectOneExecuted()
        {
            var service = GetService();

            var i1 = new MyInterceptorOne();
            var i2 = new MyInterceptorTwo();
            service.AddInterceptor(i1);
            service.AddInterceptor(i2);

            await service.SelectOneAsync<User>(Check.Op("Id", 2));

            var result = (User)i1.SelectOneExecutedContext.Result;

            Assert.AreEqual(true, i1.SelectOneExecutedCalled);
            Assert.IsNotNull(i1.SelectOneExecutedCommand);
            Assert.IsNotNull(i1.SelectOneExecutedContext);
            Assert.AreEqual("Pat", result.UserName);

            Assert.AreEqual(true, i2.SelectOneExecutedCalled);
            Assert.IsNotNull(i2.SelectOneExecutedCommand);
            Assert.IsNotNull(i2.SelectOneExecutedContext);
        }

        public async Task OnScalarExecuting()
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

        public async Task OnScalarExecuted()
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
            Assert.AreEqual(4, i1.ScalarExecutedContext.Result);

            Assert.AreEqual(true, i2.ScalarExecutedCalled);
            Assert.IsNotNull(i2.ScalarExecutedCommand);
            Assert.IsNotNull(i2.ScalarExecutedContext);
            Assert.AreEqual(4, i2.ScalarExecutedContext.Result);
        }

        public async Task OnNonQueryExecuting()
        {
            var service = GetService();

            var i1 = new MyInterceptorOne();
            var i2 = new MyInterceptorTwo();
            service.AddInterceptor(i1);
            service.AddInterceptor(i2);

            var command = service.CreateSqlCommand("insert into [Category] (Name) values(@name)").AddInParameter("@name", "my category 1");
            var result = await service.ExecuteNonQueryAsync(command);

            Assert.AreEqual(true, i1.NonQueryCalled);
            Assert.IsNotNull(i1.NonQueryCommand);

            Assert.AreEqual(true, i2.NonQueryCalled);
            Assert.IsNotNull(i2.NonQueryCommand);
        }

        public async Task OnNonQueryExecuted()
        {
            var service = GetService();

            var i1 = new MyInterceptorOne();
            var i2 = new MyInterceptorTwo();
            service.AddInterceptor(i1);
            service.AddInterceptor(i2);

            var command = service.CreateSqlCommand("insert into [Category] (Name) values(@name)").AddInParameter("@name", "my category 2");
            var result = await service.ExecuteNonQueryAsync(command);

            Assert.AreEqual(true, i1.NonQueryExecutedCalled);
            Assert.IsNotNull(i1.NonQueryExecutedCommand);
            Assert.IsNotNull(i1.NonQueryExecutedContext);
            Assert.AreEqual(1, i1.NonQueryExecutedContext.Result);

            Assert.AreEqual(true, i2.NonQueryExecutedCalled);
            Assert.IsNotNull(i2.NonQueryExecutedCommand);
            Assert.IsNotNull(i2.NonQueryExecutedContext);
            Assert.AreEqual(1, i2.NonQueryExecutedContext.Result);
        }

        public async Task OnInserting()
        {
            var service = GetService();

            service.SetTable<User>("User")
              .SetPrimaryKeyColumn("Id", p => p.Id);

            var i1 = new MyInterceptorOne();
            var i2 = new MyInterceptorTwo();
            service.AddInterceptor(i1);
            service.AddInterceptor(i2);

            var model = new User
            {
                UserName = "Marie"
            };

            await service.InsertAsync(model);

            Assert.AreEqual(true, i1.InsertingCalled);
            Assert.IsNotNull(i1.InsertingCommand);
            Assert.AreEqual(model, i1.InsertingModel);

            Assert.AreEqual(true, i2.InsertingCalled);
            Assert.IsNotNull(i2.InsertingCommand);
            Assert.AreEqual(model, i2.InsertingModel);
        }

        public async Task OnInserted()
        {
            var service = GetService();

            service.SetTable<User>("User")
              .SetPrimaryKeyColumn("Id", p => p.Id);

            var i1 = new MyInterceptorOne();
            var i2 = new MyInterceptorTwo();
            service.AddInterceptor(i1);
            service.AddInterceptor(i2);

            var model = new User
            {
                UserName = "Marie"
            };

            await service.InsertAsync(model);

            Assert.AreEqual(true, i1.InsertedCalled);
            Assert.IsNotNull(i1.InsertedCommand);
            Assert.AreEqual(model.Id, ((User)i1.InsertedContext.Model).Id);
            Assert.AreEqual(model.Id, (int)i1.InsertedContext.Result);

            Assert.AreEqual(true, i2.InsertedCalled);
            Assert.IsNotNull(i2.InsertedCommand);
            Assert.AreEqual(model.Id, ((User)i2.InsertedContext.Model).Id);
            Assert.AreEqual(model.Id, (int)i2.InsertedContext.Result);

            Assert.IsFalse(i1.SelectAllCalled);
            Assert.IsFalse(i1.SelectOneCalled);
            Assert.IsFalse(i1.UpdatingCalled);
            Assert.IsFalse(i1.DeletingCalled);
            Assert.IsFalse(i1.SelectAllExecutedCalled);
            Assert.IsFalse(i1.SelectOneExecutedCalled);
            Assert.IsFalse(i1.UpdatedCalled);
            Assert.IsFalse(i1.DeletedCalled);
        }

        public async Task OnUpdating()
        {
            var service = GetService();

            service.SetTable<User>("User")
              .SetPrimaryKeyColumn("Id", p => p.Id);

            var i1 = new MyInterceptorOne();
            var i2 = new MyInterceptorTwo();
            service.AddInterceptor(i1);
            service.AddInterceptor(i2);

            var model = new User
            {
                Id = 2,
                UserName = "Marie"
            };

            await service.UpdateAsync(model, Check.Op("Id", model.Id));

            Assert.AreEqual(true, i1.UpdatingCalled);
            Assert.IsNotNull(i1.UpdatingCommand);
            Assert.AreEqual(model, i1.UpdatingModel);

            Assert.AreEqual(true, i2.UpdatingCalled);
            Assert.IsNotNull(i2.UpdatingCommand);
            Assert.AreEqual(model, i2.UpdatingModel);

            Assert.IsFalse(i1.SelectAllCalled);
            Assert.IsFalse(i1.SelectOneCalled);
            Assert.IsFalse(i1.InsertingCalled);
            Assert.IsFalse(i1.DeletingCalled);
            Assert.IsFalse(i1.SelectAllExecutedCalled);
            Assert.IsFalse(i1.SelectOneExecutedCalled);
            Assert.IsFalse(i1.InsertedCalled);
            Assert.IsFalse(i1.DeletedCalled);
        }

        public async Task OnUpdated()
        {
            var service = GetService();

            service.SetTable<User>("User")
              .SetPrimaryKeyColumn("Id", p => p.Id);

            var i1 = new MyInterceptorOne();
            var i2 = new MyInterceptorTwo();
            service.AddInterceptor(i1);
            service.AddInterceptor(i2);

            var model = new User
            {
                Id = 2,
                UserName = "Marie"
            };

            await service.UpdateAsync(model, Check.Op("Id", model.Id));

            Assert.AreEqual(true, i1.UpdatedCalled);
            Assert.IsNotNull(i1.UpdatedCommand);
            Assert.AreEqual(model, (User)i1.UpdatedContext.Model);
            Assert.AreEqual(1, (int)i1.UpdatedContext.Result);

            Assert.AreEqual(true, i2.UpdatedCalled);
            Assert.IsNotNull(i2.UpdatedCommand);
            Assert.AreEqual(model, (User)i2.UpdatedContext.Model);
            Assert.AreEqual(1, (int)i2.UpdatedContext.Result);

            Assert.IsFalse(i1.SelectAllCalled);
            Assert.IsFalse(i1.SelectOneCalled);
            Assert.IsFalse(i1.InsertingCalled);
            Assert.IsFalse(i1.DeletingCalled);
            Assert.IsFalse(i1.SelectAllExecutedCalled);
            Assert.IsFalse(i1.SelectOneExecutedCalled);
            Assert.IsFalse(i1.InsertedCalled);
            Assert.IsFalse(i1.DeletedCalled);
        }

        public async Task OnDeleting()
        {
            var service = GetService();

            service.SetTable<User>("User")
                .SetPrimaryKeyColumn("Id", p => p.Id);

            var i1 = new MyInterceptorOne();
            var i2 = new MyInterceptorTwo();
            service.AddInterceptor(i1);
            service.AddInterceptor(i2);

            var model = new User
            {
                Id = 5,
                UserName = "Marie"
            };

            await service.DeleteAsync(model, Check.Op("Id", model.Id));

            Assert.AreEqual(true, i1.DeletingCalled);
            Assert.IsNotNull(i1.DeletingCommand);
            Assert.AreEqual(model, i1.DeletingModel);

            Assert.AreEqual(true, i2.DeletingCalled);
            Assert.IsNotNull(i2.DeletingCommand);
            Assert.AreEqual(model, i2.DeletingModel);

            Assert.IsFalse(i1.SelectAllCalled);
            Assert.IsFalse(i1.SelectOneCalled);
            Assert.IsFalse(i1.InsertingCalled);
            Assert.IsFalse(i1.UpdatingCalled);
            Assert.IsFalse(i1.SelectAllExecutedCalled);
            Assert.IsFalse(i1.SelectOneExecutedCalled);
            Assert.IsFalse(i1.InsertedCalled);
            Assert.IsFalse(i1.UpdatedCalled);
        }

        public async Task OnDeleted()
        {
            var service = GetService();

            service.SetTable<User>("User")
              .SetPrimaryKeyColumn("Id", p => p.Id);

            var i1 = new MyInterceptorOne();
            var i2 = new MyInterceptorTwo();
            service.AddInterceptor(i1);
            service.AddInterceptor(i2);

            var model = new User
            {
                Id = 6,
                UserName = "Marie"
            };

            await service.DeleteAsync(model, Check.Op("Id", model.Id));

            Assert.AreEqual(true, i1.DeletedCalled);
            Assert.IsNotNull(i1.DeletedCommand);
            Assert.AreEqual(model, (User)i1.DeletedContext.Model);
            Assert.AreEqual(1, (int)i1.DeletedContext.Result);

            Assert.AreEqual(true, i2.DeletedCalled);
            Assert.IsNotNull(i2.DeletedCommand);
            Assert.AreEqual(model, (User)i1.DeletedContext.Model);
            Assert.AreEqual(1, (int)i1.DeletedContext.Result);
            Assert.AreEqual(model, (User)i2.DeletedContext.Model);
            Assert.AreEqual(1, (int)i2.DeletedContext.Result);

            Assert.IsFalse(i1.SelectAllCalled);
            Assert.IsFalse(i1.SelectOneCalled);
            Assert.IsFalse(i1.InsertingCalled);
            Assert.IsFalse(i1.UpdatingCalled);
            Assert.IsFalse(i1.SelectAllExecutedCalled);
            Assert.IsFalse(i1.SelectOneExecutedCalled);
            Assert.IsFalse(i1.InsertedCalled);
            Assert.IsFalse(i1.UpdatedCalled);
        }

        [TestMethod]
        public async Task ConcurrencyException()
        {
            var db = new EasyDb();
            db.SetConnectionStringSettings(@"Data Source=(LocalDb)\MSSQLLocalDB;Initial Catalog=MyDb;Integrated Security=True", DbConstants.SqlProviderName);

            db.AddInterceptor(new ConcurrencyInterceptor());

            db.SetTable<UserWithRowVersion>("User")
              .SetPrimaryKeyColumn("Id", p => p.Id)
              .SetColumn("Version", p => p.Version, true);

            var model = await db.SelectOneAsync<UserWithRowVersion>(Check.Op("Id", 2));
            model.UserName += " updated";
            model.Version[4] = 2;

            bool failed = false;

            try
            {
                int result = await db.UpdateAsync<UserWithRowVersion>(model, Check.Op("Id", model.Id).And(Check.Op("Version", model.Version)));
            }
            catch (OptimisticConcurrencyException ex)
            {
                failed = true;

            }

            Assert.IsTrue(failed);
        }

    }


    public class UserWithRowVersion
    {
        public int Id { get; set; }

        public string UserName { get; set; }

        public byte[] Version { get; set; }
    }


}
