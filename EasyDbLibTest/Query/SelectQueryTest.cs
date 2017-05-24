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
        private static string sqlServerConnectionString =
                  @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\romag\Documents\Visual Studio 2017\Projects\EasyDbLib\EasyDbLibTest\SqlServerDb.mdf;Integrated Security=True;Connect Timeout=20"
              ;

        private static string sqlServerProviderName = "System.Data.SqlClient";

        // build query

        [TestMethod]
        public void TestBuildQuery()
        {
            EasyDb.Default.SetConnectionSettings(sqlServerConnectionString, sqlServerProviderName);

            Mapping.AddTable("Users");

            var result = EasyDb.Default
                    .Select<User>(Mapping.GetTable("Users"))
                    .GetQuery();

            Assert.AreEqual("select * from [Users]", result);
        }


        [TestMethod]
        public void TestBuildQuery_WithCondition()
        {
            EasyDb.Default.SetConnectionSettings(sqlServerConnectionString, sqlServerProviderName);

            Mapping.AddTable("Users");

            var result = EasyDb.Default
                    .Select<User>(Mapping.GetTable("Users"))
                    .Where(Condition.Op("id", 12))
                    .GetQuery();

            Assert.AreEqual("select * from [Users] where [id]=@id", result);
        }

        [TestMethod]
        public void TestBuildQuery_WithCondition_FormatColumns()
        {
            EasyDb.Default.SetConnectionSettings(sqlServerConnectionString, sqlServerProviderName);

            Mapping.AddTable("Users");

            var result = EasyDb.Default
                    .Select<User>(Mapping.GetTable("Users"))
                    .Where(Condition.Op("posts.id", 12))
                    .GetQuery();

            Assert.AreEqual("select * from [Users] where [posts].[id]=@id", result);
        }

        [TestMethod]
        public void TestBuildQuery_WithCondition_AreUniques()
        {
            EasyDb.Default.SetConnectionSettings(sqlServerConnectionString, sqlServerProviderName);

            Mapping.AddTable("Users");

            var result = EasyDb.Default
                    .Select<User>(Mapping.GetTable("Users"))
                    .Where(Condition.Op("id", 12).Or(Condition.Op("id",12)))
                    .GetQuery();

            Assert.AreEqual("select * from [Users] where [id]=@id or [id]=@id2", result);
        }

        [TestMethod]
        public void TestBuildQuery_WithColumns()
        {
            EasyDb.Default.SetConnectionSettings(sqlServerConnectionString, sqlServerProviderName);

            Mapping.AddTable("Users")
                .AddColumn("Name", "Name")
                 .AddColumn("Email", "Email");

            var result = EasyDb.Default
                    .Select<User>(Mapping.GetTable("Users"))
                    .Where(Condition.Op("id", 12))
                    .GetQuery();

            Assert.AreEqual("select [Name],[Email] from [Users] where [id]=@id", result);
        }

        [TestMethod]
        public void TestBuildQuery_WithPkAndColumns_ReturnsColumns()
        {
            EasyDb.Default.SetConnectionSettings(sqlServerConnectionString, sqlServerProviderName);

            Mapping.AddTable("Users")
                .AddPrimaryKeyColumn("Id", "Id")
                .AddColumn("Name", "Name");

            var result = EasyDb.Default
                    .Select<User>(Mapping.GetTable("Users"))
                    .Where(Condition.Op("id", 12))
                    .GetQuery();

            Assert.AreEqual("select [Id],[Name] from [Users] where [id]=@id", result);
        }

        [TestMethod]
        public void TestBuildQuery_WithOnlyPk_ReturnsAll()
        {
            EasyDb.Default.SetConnectionSettings(sqlServerConnectionString, sqlServerProviderName);

            Mapping.AddTable("Users")
                .AddPrimaryKeyColumn("Id", "Id");

            var result = EasyDb.Default
                    .Select<User>(Mapping.GetTable("Users"))
                    .Where(Condition.Op("id", 12))
                    .GetQuery();

            Assert.AreEqual("select * from [Users] where [id]=@id", result);
        }

        // read one 


        [TestMethod]
        public async Task TestSelectOne()
        {
            EasyDb.Default.SetConnectionSettings(sqlServerConnectionString, sqlServerProviderName);

            Mapping.AddTable("Users");


            var result = await EasyDb.Default
                   .Select<User>(Mapping.GetTable("Users"))
                   .Where(Condition.Op("id",12))
                   .ReadOneAsync();

            Assert.AreEqual(12, result.Id);
            Assert.AreEqual("Pat", result.Name);
            Assert.AreEqual(null, result.Email);
            Assert.AreEqual(20, result.Age);
        }


        [TestMethod]
        public async Task TestSelectOne_ReturnsTheFirstUserFound()
        {
            EasyDb.Default.SetConnectionSettings(sqlServerConnectionString, sqlServerProviderName);

            Mapping.AddTable("Users");


            var result = await EasyDb.Default
                   .Select<User>(Mapping.GetTable("Users"))
                   .ReadOneAsync();

            Assert.AreEqual(11, result.Id);
            Assert.AreEqual("Marie", result.Name);
            Assert.AreEqual("marie@mail.com", result.Email);
            Assert.AreEqual(null, result.Age);
        }

        // get relation one

        [TestMethod]
        public async Task TestRelationZeroOne()
        {
            EasyDb.Default.SetConnectionSettings(sqlServerConnectionString, sqlServerProviderName);

            Mapping.AddTable("categories")
                .AddPrimaryKeyColumn("id", "Id")
                .AddColumn("name","Name");

            Mapping.AddTable("posts")
                .AddPrimaryKeyColumn("Id", "Id")
                .AddColumn("title","Title")
                .AddColumn("content","Content")
                .AddForeignKeyColumn("category_id", "CategoryId", "categories", "id");

            var result = await EasyDb.Default
                   .Select<Post>(Mapping.GetTable("posts"))
                   .Where(Condition.Op("id", 1))
                   .HasOne<Category>("Category", Mapping.GetTable("categories"))
                   .ReadOneAsync();

            Assert.IsNotNull(result.Category);
            Assert.AreEqual(2,result.Category.Id);
            Assert.AreEqual("Mobile", result.Category.Name);
        }

        [TestMethod]
        public async Task TestRelationZeroOne_ReturnsNull()
        {
            EasyDb.Default.SetConnectionSettings(sqlServerConnectionString, sqlServerProviderName);

            Mapping.AddTable("categories").AddPrimaryKeyColumn("id", "id");

            Mapping.AddTable("posts")
                .AddPrimaryKeyColumn("Id", "Id")
                .AddForeignKeyColumn("category_id", "CategoryId", "categories", "id");

            var result = await EasyDb.Default
                   .Select<Post>(Mapping.GetTable("posts"))
                   .Where(Condition.Op("id", 2))
                   .HasOne<Category>("Category", Mapping.GetTable("categories"))
                   .ReadOneAsync();

            Assert.IsNull(result.Category);
        }

        [TestMethod]
        public async Task TestRelationOne_FailedIfNoForeignKeyDefinedInMapping()
        {
            bool failed = false;
            EasyDb.Default.SetConnectionSettings(sqlServerConnectionString, sqlServerProviderName);

            Mapping.AddTable("Users")
               .AddPrimaryKeyColumn("Id", "Id");

            Mapping.AddTable("categories")
                .AddPrimaryKeyColumn("id", "Id")
                .AddColumn("name", "Name");

            Mapping.AddTable("posts")
                .AddPrimaryKeyColumn("Id", "Id")
                .AddColumn("title", "Title")
                .AddColumn("content", "Content")
                .AddForeignKeyColumn("category_id", "CategoryId", "categories", "id");

            try
            {
                var result = await EasyDb.Default
                                 .Select<Post>(Mapping.GetTable("posts"))
                                 .Where(Condition.Op("id", 1))
                                 .HasOne<User>("User", Mapping.GetTable("Users"))
                                 .ReadOneAsync();
            }
            catch (Exception)
            {
                failed = true;
            }


            Assert.IsTrue(failed);
        }

        // get multiples relations

        [TestMethod]
        public async Task TestRelationOnes()
        {
            EasyDb.Default.SetConnectionSettings(sqlServerConnectionString, sqlServerProviderName);

            Mapping.AddTable("Users")
               .AddPrimaryKeyColumn("Id", "Id");

            Mapping.AddTable("categories")
                .AddPrimaryKeyColumn("id", "Id")
                .AddColumn("name", "Name");

            Mapping.AddTable("posts")
                .AddPrimaryKeyColumn("Id", "Id")
                .AddColumn("title", "Title")
                .AddColumn("content", "Content")
                .AddForeignKeyColumn("category_id", "CategoryId", "categories", "id")
                .AddForeignKeyColumn("user_id","UserId","Users","Id");

            var result = await EasyDb.Default
                   .Select<Post>(Mapping.GetTable("posts"))
                   .Where(Condition.Op("id", 1))
                   .HasOne<Category>("Category", Mapping.GetTable("categories"))
                   .HasOne<User>("User",Mapping.GetTable("Users"))
                   .ReadOneAsync();

            Assert.IsNotNull(result.Category);
            Assert.AreEqual(2, result.Category.Id);
            Assert.AreEqual("Mobile", result.Category.Name);

            Assert.IsNotNull(result.User);
            Assert.AreEqual(11, result.User.Id);
            Assert.AreEqual("Marie", result.User.Name);
            Assert.AreEqual("marie@mail.com", result.User.Email);
            Assert.AreEqual(null, result.User.Age);
        }

        [TestMethod]
        public async Task TestReadAllAsync()
        {
            EasyDb.Default.SetConnectionSettings(sqlServerConnectionString, sqlServerProviderName);

            Mapping.AddTable("Users")
               .AddPrimaryKeyColumn("Id", "Id");

            Mapping.AddTable("categories")
                .AddPrimaryKeyColumn("id", "Id")
                .AddColumn("name", "Name");

            Mapping.AddTable("posts")
                .AddPrimaryKeyColumn("Id", "Id")
                .AddColumn("title", "Title")
                .AddColumn("content", "Content")
                .AddForeignKeyColumn("category_id", "CategoryId", "categories", "id")
                .AddForeignKeyColumn("user_id", "UserId", "Users", "Id");

            var result = await EasyDb.Default
                   .Select<Post>(Mapping.GetTable("posts"))
                   .ReadAllAsync();

            Assert.AreEqual(1, result[0].Id);
            Assert.AreEqual("Post 1", result[0].Title);
            Assert.AreEqual("Content 1", result[0].Content);
            Assert.AreEqual(11, result[0].UserId);
            Assert.AreEqual(2, result[0].CategoryId);
            Assert.AreEqual(null, result[0].User);
            Assert.AreEqual(null, result[0].Category);

            Assert.AreEqual(2, result[1].Id);
            Assert.AreEqual("Post 2", result[1].Title);
            Assert.AreEqual("Content 2", result[1].Content);
            Assert.AreEqual(12, result[1].UserId);
            Assert.AreEqual(null, result[1].CategoryId);
            Assert.AreEqual(null, result[1].User);
            Assert.AreEqual(null, result[1].Category);
        }


        // relation one

        [TestMethod]
        public void TestAddOne_HasRelations()
        {
            EasyDb.Default.SetConnectionSettings(sqlServerConnectionString, sqlServerProviderName);

            Mapping.AddTable("Users")
                          .AddPrimaryKeyColumn("Id", "Id");

            Mapping.AddTable("categories")
                .AddPrimaryKeyColumn("id", "Id")
                .AddColumn("name", "Name");

            Mapping.AddTable("posts")
                .AddPrimaryKeyColumn("Id", "Id")
                .AddColumn("title", "Title")
                .AddColumn("content", "Content")
                .AddForeignKeyColumn("category_id", "CategoryId", "categories", "id")
                .AddForeignKeyColumn("user_id", "UserId", "Users", "Id");

            var command = EasyDb.Default
                   .Select<Post>(Mapping.GetTable("posts"))
                   .HasOne<Category>("Category", Mapping.GetTable("categories"))
                   .HasOne<User>("User", Mapping.GetTable("Users"));

            Assert.IsTrue(command.HasOneRelation<Category>());
            Assert.IsTrue(command.HasOneRelation<User>());
        }

        [TestMethod]
        public void TestOneForeignKeys()
        {
            EasyDb.Default.SetConnectionSettings(sqlServerConnectionString, sqlServerProviderName);

            Mapping.AddTable("Users")
                          .AddPrimaryKeyColumn("Id", "Id");

            Mapping.AddTable("categories")
                .AddPrimaryKeyColumn("id", "Id")
                .AddColumn("name", "Name");

            Mapping.AddTable("posts")
                .AddPrimaryKeyColumn("Id", "Id")
                .AddColumn("title", "Title")
                .AddColumn("content", "Content")
                .AddForeignKeyColumn("category_id", "CategoryId", "categories", "id")
                .AddForeignKeyColumn("user_id", "UserId", "Users", "Id");

            var command = EasyDb.Default
                   .Select<Post>(Mapping.GetTable("posts"))
                   .HasOne<Category>("Category", Mapping.GetTable("categories"))
                   .HasOne<User>("User", Mapping.GetTable("Users"));

            var relation = command.GetOneRelation<User>();
            var result = relation.GetForeignKeys();

            Assert.AreEqual(1, result.Length);
            Assert.AreEqual("user_id", result[0].ColumnName);
            Assert.AreEqual("Id", result[0].PrimaryKeyReferenced);
            Assert.AreEqual("UserId", result[0].PropertyName);
            Assert.AreEqual("Users", result[0].TableReferenced);
        }


        [TestMethod]
        public void TestGetOneQuery()
        {
            EasyDb.Default.SetConnectionSettings(sqlServerConnectionString, sqlServerProviderName);

            Mapping.AddTable("Users")
                          .AddPrimaryKeyColumn("Id", "Id");

            Mapping.AddTable("categories")
                .AddPrimaryKeyColumn("id", "Id")
                .AddColumn("name", "Name");

            Mapping.AddTable("posts")
                .AddPrimaryKeyColumn("Id", "Id")
                .AddColumn("title", "Title")
                .AddColumn("content", "Content")
                .AddForeignKeyColumn("category_id", "CategoryId", "categories", "id")
                .AddForeignKeyColumn("user_id", "UserId", "Users", "Id");

            var command = EasyDb.Default
                   .Select<Post>(Mapping.GetTable("posts"))
                   .HasOne<Category>("Category", Mapping.GetTable("categories"))
                   .HasOne<User>("User", Mapping.GetTable("Users"));

            var fks = command.GetOneRelation<User>().GetForeignKeys();

            var result = command.GetOneRelation<User>().GetQuery(fks);

            Assert.AreEqual("select * from [Users] where [Id]=@user_id", result);
        }


        [TestMethod]
        public async Task TestReadAllAsync_WithOneRelations()
        {
            EasyDb.Default.SetConnectionSettings(sqlServerConnectionString, sqlServerProviderName);

            Mapping.AddTable("Users")
               .AddPrimaryKeyColumn("Id", "Id");

            Mapping.AddTable("categories")
                .AddPrimaryKeyColumn("id", "Id")
                .AddColumn("name", "Name");

            Mapping.AddTable("posts")
                .AddPrimaryKeyColumn("Id", "Id")
                .AddColumn("title", "Title")
                .AddColumn("content", "Content")
                .AddForeignKeyColumn("category_id", "CategoryId", "categories", "id")
                .AddForeignKeyColumn("user_id", "UserId", "Users", "Id");

            var result = await EasyDb.Default
                   .Select<Post>(Mapping.GetTable("posts"))
                   .HasOne<Category>("Category", Mapping.GetTable("categories"))
                   .HasOne<User>("User", Mapping.GetTable("Users"))
                   .ReadAllAsync();

            Assert.IsNotNull(result[0].User);
            Assert.AreEqual(11, result[0].User.Id);
            Assert.AreEqual("Marie", result[0].User.Name);
            Assert.AreEqual("marie@mail.com", result[0].User.Email);
            Assert.AreEqual(null, result[0].User.Age);

            Assert.IsNotNull(result[0].Category);
            Assert.AreEqual(2, result[0].Category.Id);
            Assert.AreEqual("Mobile", result[0].Category.Name);

            Assert.IsNotNull(result[1].User);
            Assert.AreEqual(12, result[1].User.Id);
            Assert.AreEqual("Pat", result[1].User.Name);
            Assert.AreEqual(null, result[1].User.Email);
            Assert.AreEqual(20, result[1].User.Age);

            Assert.IsNull(result[1].Category);
        }


        [TestMethod]
        public async Task WithMoreResults()
        {
            EasyDb.Default.SetConnectionSettings(sqlServerConnectionString, sqlServerProviderName);

            Mapping.AddTable("Users")
               .AddPrimaryKeyColumn("Id", "Id");

            Mapping.AddTable("categories")
                .AddPrimaryKeyColumn("id", "Id")
                .AddColumn("name", "Name");

            Mapping.AddTable("posts")
                .AddPrimaryKeyColumn("Id", "Id")
                .AddColumn("title", "Title")
                .AddColumn("content", "Content")
                .AddForeignKeyColumn("category_id", "CategoryId", "categories", "id")
                .AddForeignKeyColumn("user_id", "UserId", "Users", "Id");

            var results = await EasyDb.Default
                 .Select<Post>(Mapping.GetTable("posts"))
                 .Where(Condition.Op("user_id",11))
                 .HasOne<Category>("Category", Mapping.GetTable("categories"))
                 .HasOne<User>("User", Mapping.GetTable("Users"))
                 .ReadAllAsync();

            foreach (var result in results)
            {
                Assert.IsNotNull(result.User);
                Assert.AreEqual(11, result.User.Id);
                Assert.AreEqual("Marie", result.User.Name);
                Assert.AreEqual("marie@mail.com", result.User.Email);
                Assert.AreEqual(null, result.User.Age);
            }
        }

        [TestMethod]
        public async Task WithMoreResultAndConditions()
        {
            EasyDb.Default.SetConnectionSettings(sqlServerConnectionString, sqlServerProviderName);

            Mapping.AddTable("Users")
               .AddPrimaryKeyColumn("Id", "Id");

            Mapping.AddTable("categories")
                .AddPrimaryKeyColumn("id", "Id")
                .AddColumn("name", "Name");

            Mapping.AddTable("posts")
                .AddPrimaryKeyColumn("Id", "Id")
                .AddColumn("title", "Title")
                .AddColumn("content", "Content")
                .AddForeignKeyColumn("category_id", "CategoryId", "categories", "id")
                .AddForeignKeyColumn("user_id", "UserId", "Users", "Id");

            var results = await EasyDb.Default
                 .Select<Post>(Mapping.GetTable("posts"))
                 .Where(Condition.Op("user_id", 11).And(Condition.IsNotNull("category_id")))
                 .HasOne<Category>("Category", Mapping.GetTable("categories"))
                 .HasOne<User>("User", Mapping.GetTable("Users"))
                 .ReadAllAsync();

            foreach (var result in results)
            {
                Assert.IsNotNull(result.User);
                Assert.AreEqual(11, result.User.Id);
                Assert.AreEqual("Marie", result.User.Name);
                Assert.AreEqual("marie@mail.com", result.User.Email);
                Assert.AreEqual(null, result.User.Age);

                Assert.IsNotNull(result.Category);
                Assert.IsTrue(result.Category.Id > 0);
                Assert.IsFalse(string.IsNullOrEmpty(result.Category.Name));
            }
        }

        [TestMethod]
        public async Task WithMoreResultAndConditionAndIsNull()
        {
            EasyDb.Default.SetConnectionSettings(sqlServerConnectionString, sqlServerProviderName);

            Mapping.AddTable("Users")
               .AddPrimaryKeyColumn("Id", "Id");

            Mapping.AddTable("categories")
                .AddPrimaryKeyColumn("id", "Id")
                .AddColumn("name", "Name");

            Mapping.AddTable("posts")
                .AddPrimaryKeyColumn("Id", "Id")
                .AddColumn("title", "Title")
                .AddColumn("content", "Content")
                .AddForeignKeyColumn("category_id", "CategoryId", "categories", "id")
                .AddForeignKeyColumn("user_id", "UserId", "Users", "Id");

            var results = await EasyDb.Default
                 .Select<Post>(Mapping.GetTable("posts"))
                 .Where(Condition.Op("user_id", 11).And(Condition.IsNull("category_id")))
                 .HasOne<Category>("Category", Mapping.GetTable("categories"))
                 .HasOne<User>("User", Mapping.GetTable("Users"))
                 .ReadAllAsync();

            foreach (var result in results)
            {
                Assert.IsNotNull(result.User);
                Assert.AreEqual(11, result.User.Id);
                Assert.AreEqual("Marie", result.User.Name);
                Assert.AreEqual("marie@mail.com", result.User.Email);
                Assert.AreEqual(null, result.User.Age);

                Assert.IsNull(result.CategoryId);
                Assert.IsNull(result.Category);
            }
        }

        // relations many

        [TestMethod]
        public void TestMany_HasRelations()
        {
            EasyDb.Default.SetConnectionSettings(sqlServerConnectionString, sqlServerProviderName);

            Mapping.AddTable("Users")
                .AddPrimaryKeyColumn("id", "Id");

            Mapping.AddTable("posts")
                  .AddPrimaryKeyColumn("Id", "Id")
                  .AddColumn("title", "Title")
                  .AddColumn("content", "Content")
                  .AddForeignKeyColumn("category_id", "CategoryId", "categories", "id")
                  .AddForeignKeyColumn("user_id", "UserId", "Users", "Id");


            var result = EasyDb.Default
                   .Select<User>(Mapping.GetTable("Users"))
                   .Where(Condition.Op("id", 11))
                   .HasMany<Post>("PostList", Mapping.GetTable("posts"));


            Assert.IsTrue(result.HasManyRelation<Post>());
        }

        [TestMethod]
        public void TestMany_ForeignKeys()
        {
            EasyDb.Default.SetConnectionSettings(sqlServerConnectionString, sqlServerProviderName);

            Mapping.AddTable("Users")
                .AddPrimaryKeyColumn("id", "Id");

            Mapping.AddTable("posts")
                  .AddPrimaryKeyColumn("Id", "Id")
                  .AddColumn("title", "Title")
                  .AddColumn("content", "Content")
                  .AddForeignKeyColumn("category_id", "CategoryId", "categories", "id")
                  .AddForeignKeyColumn("user_id", "UserId", "Users", "Id");


            var command = EasyDb.Default
                   .Select<User>(Mapping.GetTable("Users"))
                   .Where(Condition.Op("id", 11))
                   .HasMany<Post>("PostList", Mapping.GetTable("posts"));


            var relation = command.GetManyRelation<Post>();
            var result = relation.GetForeignKeys();

            Assert.AreEqual(1, result.Length);
            Assert.AreEqual("user_id", result[0].ColumnName);
            Assert.AreEqual("Id", result[0].PrimaryKeyReferenced);
            Assert.AreEqual("UserId", result[0].PropertyName);
            Assert.AreEqual("Users", result[0].TableReferenced);
        }

        [TestMethod]
        public void TestMany_GetQuery()
        {
            EasyDb.Default.SetConnectionSettings(sqlServerConnectionString, sqlServerProviderName);

            Mapping.AddTable("Users")
                .AddPrimaryKeyColumn("id", "Id");

            Mapping.AddTable("posts")
                  .AddPrimaryKeyColumn("id", "Id")
                  .AddColumn("title", "Title")
                  .AddColumn("content", "Content")
                  .AddForeignKeyColumn("category_id", "CategoryId", "categories", "id")
                  .AddForeignKeyColumn("user_id", "UserId", "Users", "Id");


            var command = EasyDb.Default
                   .Select<User>(Mapping.GetTable("Users"))
                   .Where(Condition.Op("id", 11))
                   .HasMany<Post>("PostList", Mapping.GetTable("posts"));


            var fks = command.GetManyRelation<Post>().GetForeignKeys();
            var result = command.GetManyRelation<Post>().GetQuery(fks);

            Assert.AreEqual("select [id],[title],[content],[category_id],[user_id] from [posts] where [user_id]=@user_id", result);
        }

        [TestMethod]
        public async Task TestRelationMany()
        {
            EasyDb.Default.SetConnectionSettings(sqlServerConnectionString, sqlServerProviderName);

            Mapping.AddTable("Users")
                .AddPrimaryKeyColumn("id", "Id");

            Mapping.AddTable("posts")
                  .AddPrimaryKeyColumn("id", "Id")
                  .AddColumn("title", "Title")
                  .AddColumn("content", "Content")
                  .AddForeignKeyColumn("category_id","CategoryId","categories","id")
                  .AddForeignKeyColumn("user_id", "UserId", "Users", "Id");


            var result = await EasyDb.Default
                   .Select<User>(Mapping.GetTable("Users"))
                   .Where(Condition.Op("id", 11))
                   .HasMany<Post>("PostList", Mapping.GetTable("posts"))
                   .ReadOneAsync();


            foreach (var item in result.PostList)
            {
                Assert.IsNotNull(item);
                Assert.IsTrue(item.Id > 0);
                Assert.IsFalse(string.IsNullOrEmpty(item.Title));
                Assert.IsFalse(string.IsNullOrEmpty(item.Content));
            }
        }

    }


    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int? Age { get; set; }

        public List<Post> PostList { get; set; }
    }

    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public int? CategoryId { get; set; }
        public Category Category { get; set; }
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
