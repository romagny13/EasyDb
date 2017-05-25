using System;
using System.Collections.Generic;
using EasyDbLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using System.Data;

namespace EasyDbLibTest
{
    [TestClass]
    public class SelectQueryTest
    {
        [ClassInitialize()]
        public static void ClassInit(TestContext context)
        {
            InitDb.CreateDbTest();
        }

        public EasyDb GetDb()
        {
            var db = new EasyDb();
            db.SetConnectionStringSettings(InitDb.SqlConnectionString, InitDb.SqlProviderName);
            return db;
        }

        // check

        [TestMethod]
        public void TestOneLimit()
        {
            bool failed = false;
            var db = this.GetDb();

            Mapping.AddTable("users");

            try
            {
                var result = db
                    .Select<UserLikeTable>(Mapping.GetTable("users"))
                    .Top(10)
                    .Top(20)
                    .GetQuery();
            }
            catch (Exception)
            {
                failed = true;
            }
            Assert.IsTrue(failed);
        }

        [TestMethod]
        public void TestOneStatements()
        {
            bool failed = false;
            var db = this.GetDb();

            Mapping.AddTable("users");

            try
            {
                var result = db
                    .Select<UserLikeTable>(Mapping.GetTable("users"))
                    .Statements("distinct")
                    .Statements("high_priority")
                    .GetQuery();
            }
            catch (Exception)
            {
                failed = true;
            }
            Assert.IsTrue(failed);
        }

        [TestMethod]
        public void TestOneClauseWhere()
        {
            bool failed = false;
            var db = this.GetDb();

            Mapping.AddTable("users");

            try
            {
                var result = db
                    .Select<UserLikeTable>(Mapping.GetTable("users"))
                    .Where(Condition.Op("id", 2))
                    .Where(Condition.Op("id", 3))
                    .GetQuery();
            }
            catch (Exception)
            {
                failed = true;
            }
            Assert.IsTrue(failed);
        }

        [TestMethod]
        public void TestOneOrderBy()
        {
            bool failed = false;
            var db = this.GetDb();

            Mapping.AddTable("users");

            try
            {
                var result = db
                    .Select<UserLikeTable>(Mapping.GetTable("users"))
                    .OrderBy("id")
                    .OrderBy("firstname")
                    .GetQuery();
            }
            catch (Exception)
            {
                failed = true;
            }
            Assert.IsTrue(failed);
        }

        // build query

        [TestMethod]
        public void TestQuery()
        {

            var db = this.GetDb();

            Mapping.AddTable("users");

            var result = db
                    .Select<UserLikeTable>(Mapping.GetTable("users"))
                    .GetQuery();

            Assert.AreEqual("select * from [users]", result);
        }

        // columns

        [TestMethod]
        public void TestQuery_WithPrimaryKey_ReturnsAllColumns()
        {

            var db = this.GetDb();

            Mapping.AddTable("users").AddPrimaryKeyColumn("id","id");

            var result = db
                    .Select<UserLikeTable>(Mapping.GetTable("users"))
                    .GetQuery();

            Assert.AreEqual("select * from [users]", result);
        }

        [TestMethod]
        public void TestQuery_WithForeignKey_ReturnsAllColumns()
        {
            var db = this.GetDb();

            Mapping
                .AddTable("posts")
                .AddForeignKeyColumn("user_id", "UserId","users","id");

            var result = db
                    .Select<UserLikeTable>(Mapping.GetTable("posts"))
                    .GetQuery();

            Assert.AreEqual("select * from [posts]", result);
        }

        [TestMethod]
        public void TestQuery_WithColumns_ReturnsColumns()
        {
            var db = this.GetDb();

            Mapping
                .AddTable("posts")
                .AddColumn("title","Title")
                .AddColumn("content","Content")
                .AddForeignKeyColumn("user_id", "UserId", "users", "id");

            var result = db
                    .Select<UserLikeTable>(Mapping.GetTable("posts"))
                    .GetQuery();

            Assert.AreEqual("select [title],[content],[user_id] from [posts]", result);
        }

        [TestMethod]
        public void TestQuery_IgnoreColumns()
        {
            var db = this.GetDb();

            Mapping
                .AddTable("posts")
                .AddColumn("title", "Title",true)
                .AddColumn("content", "Content")
                .AddForeignKeyColumn("user_id", "UserId", "users", "id");

            var result = db
                    .Select<UserLikeTable>(Mapping.GetTable("posts"))
                    .GetQuery();

            Assert.AreEqual("select [content],[user_id] from [posts]", result);
        }

        [TestMethod]
        public void TestQuery_IfAllColumnsAreIgnore8returnsAll()
        {
            var db = this.GetDb();

            Mapping
                .AddTable("posts")
                .AddColumn("title", "Title", true)
                .AddColumn("content", "Content", true)
                .AddForeignKeyColumn("user_id", "UserId", "users", "id",true);

            var result = db
                    .Select<UserLikeTable>(Mapping.GetTable("posts"))
                    .GetQuery();

            Assert.AreEqual("select * from [posts]", result);
        }

        // top

        [TestMethod]
        public void TestQuery_WithTop()
        {

            var db = this.GetDb();

            Mapping.AddTable("users").AddPrimaryKeyColumn("id", "id");

            var result = db
                    .Select<UserLikeTable>(Mapping.GetTable("users"))
                    .Top(10)
                    .GetQuery();

            Assert.AreEqual("select top 10 * from [users]", result);
        }

        [TestMethod]
        public void TestQuery_WithTopAndColumns()
        {

            var db = this.GetDb();

            Mapping.AddTable("users")
                .AddPrimaryKeyColumn("id", "id")
                .AddColumn("firstname","FirstName")
                .AddColumn("lastname", "LastName");

            var result = db
                    .Select<UserLikeTable>(Mapping.GetTable("users"))
                    .Top(10)
                    .GetQuery();

            Assert.AreEqual("select top 10 [id],[firstname],[lastname] from [users]", result);
        }

        // where

        [TestMethod]
        public void TestQuery_WithWhere()
        {
            var db = this.GetDb();

            Mapping.AddTable("users");

            var result = db
                    .Select<UserLikeTable>(Mapping.GetTable("users"))
                    .Where(Condition.Op("id",1))
                    .GetQuery();

            Assert.AreEqual("select * from [users] where [id]=@id", result);
        }

        [TestMethod]
        public void TestQuery_WithWhereConditions()
        {
            var db = this.GetDb();

            Mapping.AddTable("users");

            var result = db
                    .Select<UserLikeTable>(Mapping.GetTable("users"))
                    .Where(Condition.Op("id", 1).Or(Condition.Op("id",2)))
                    .GetQuery();

            Assert.AreEqual("select * from [users] where [id]=@id or [id]=@id2", result);
        }

        // order by

        [TestMethod]
        public void TestQuery_WithOrderyBy()
        {
            var db = this.GetDb();

            Mapping.AddTable("users");

            var result = db
                    .Select<UserLikeTable>(Mapping.GetTable("users"))
                    .OrderBy("firstname")
                    .GetQuery();

            Assert.AreEqual("select * from [users] order by [firstname]", result);
        }

        [TestMethod]
        public void TestQuery_WitOrderByhMultiple()
        {
            var db = this.GetDb();

            Mapping.AddTable("users");

            var result = db
                    .Select<UserLikeTable>(Mapping.GetTable("users"))
                    .OrderBy("firstname desc", "lastname")
                    .GetQuery();

            Assert.AreEqual("select * from [users] order by [firstname] desc,[lastname]", result);
        }

        // create command

        [TestMethod]
        public void TestCreateCommand()
        {
            var db = this.GetDb();

            Mapping.AddTable("users");

            var result = db
                    .Select<UserLikeTable>(Mapping.GetTable("users"))
                      .Where(Condition.Op("id", 1).Or(Condition.Op("id", 2)))
                    .CreateCommand();

            Assert.AreEqual("select * from [users] where [id]=@id or [id]=@id2", result.Command.CommandText);

            Assert.AreEqual(2, result.Command.Parameters.Count);

            Assert.AreEqual("@id", result.Command.Parameters[0].ParameterName);
            Assert.AreEqual(1, result.Command.Parameters[0].Value);

            Assert.AreEqual("@id2", result.Command.Parameters[1].ParameterName);
            Assert.AreEqual(2, result.Command.Parameters[1].Value);
        }


        // read one 

        [TestMethod]
        public async Task TestReadOneAsync()
        {
            var db = this.GetDb();

            Mapping.AddTable("users");

            var result = await db
                .Select<UserLikeTable>(Mapping.GetTable("users"))
                .Where(Condition.Op("id", 1))
                .ReadOneAsync();

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.id);
            Assert.AreEqual("Marie", result.firstname);
            Assert.AreEqual("Bellin", result.lastname);
            Assert.AreEqual(null, result.age);
            Assert.AreEqual("marie@domain.com", result.email);
        }

        [TestMethod]
        public async Task TestReadOneAsync_WithMapping()
        {
            var db = this.GetDb();

            Mapping.AddTable("users")
                .AddPrimaryKeyColumn("id", "Id")
                .AddColumn("firstname", "FirstName")
                .AddColumn("lastname", "LastName")
                .AddColumn("age", "Age")
                .AddColumn("email", "Email");

            var result = await db
                .Select<User>(Mapping.GetTable("users"))
                .Where(Condition.Op("id", 1))
                .ReadOneAsync();

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Id);
            Assert.AreEqual("Marie", result.FirstName);
            Assert.AreEqual("Bellin", result.LastName);
            Assert.AreEqual(null, result.Age);
            Assert.AreEqual("marie@domain.com", result.Email);
        }

        // read all

        [TestMethod]
        public async Task TestReadAllAsync()
        {
            var db = this.GetDb();

            Mapping.AddTable("users");

            var result = await db
                .Select<UserLikeTable>(Mapping.GetTable("users"))
                .ReadAllAsync();

            Assert.AreEqual(2, result.Count);

            Assert.AreEqual(1, result[0].id);
            Assert.AreEqual("Marie", result[0].firstname);
            Assert.AreEqual("Bellin", result[0].lastname);
            Assert.AreEqual(null, result[0].age);
            Assert.AreEqual("marie@domain.com", result[0].email);

            Assert.AreEqual(2, result[1].id);
            Assert.AreEqual("Pat", result[1].firstname);
            Assert.AreEqual("Prem", result[1].lastname);
            Assert.AreEqual(30, result[1].age);
            Assert.AreEqual(null, result[1].email);
        }

        [TestMethod]
        public async Task TestReadAllAsync_WithMapping()
        {
            var db = this.GetDb();

            Mapping.AddTable("users")
                .AddPrimaryKeyColumn("id", "Id")
                .AddColumn("firstname", "FirstName")
                .AddColumn("lastname", "LastName")
                .AddColumn("age", "Age")
                .AddColumn("email", "Email");

            var result = await db
                .Select<User>(Mapping.GetTable("users"))
                .ReadAllAsync();

            Assert.AreEqual(2,result.Count);

            Assert.AreEqual(1, result[0].Id);
            Assert.AreEqual("Marie", result[0].FirstName);
            Assert.AreEqual("Bellin", result[0].LastName);
            Assert.AreEqual(null, result[0].Age);
            Assert.AreEqual("marie@domain.com", result[0].Email);

            Assert.AreEqual(2, result[1].Id);
            Assert.AreEqual("Pat", result[1].FirstName);
            Assert.AreEqual("Prem", result[1].LastName);
            Assert.AreEqual(30, result[1].Age);
            Assert.AreEqual(null, result[1].Email);
        }        

    }


    public class BaseTable
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int? ZeroOneFKey { get; set; }
        public ZeroOne ZeroOne { get; set; }

        public int OneOneFKey { get; set; }
        public OneOne OneOne { get; set; }

        public List<ManyTable> ManyList { get; set; }
    }

    public class ZeroOne
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class OneOne
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class ManyMany
    {
        public int BaseTable_Id { get; set; }
        public int ManyTable_Id { get; set; }
    }
    public class ManyTable
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
