using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EasyDbLib;
using System.Threading.Tasks;

namespace EasyDbLibTest.Query
{
    [TestClass]
    public class SelectManyRelationTest
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
        public void TestInvalidPropertyName()
        {
            bool failed = false;
            var db = this.GetDb();

            Mapping.AddTable("posts").
                AddForeignKeyColumn("user_id", "user_id", "users", "id");
            Mapping.AddTable("users")
                .AddPrimaryKeyColumn("id", "id");

            try
            {

                // select * from posts where user_id=id
                // pk_columns pk_table  fk_target = pk
                var main = db
                     .Select<UserLikeTable>(Mapping.GetTable("users"))
                     .Where(Condition.Op("id", 1))
                     .HasMany<PostLikeTable>("Po@List", Mapping.GetTable("posts"));
            }
            catch (Exception e)
            {
                failed = true;
            }

            Assert.IsTrue(failed);
        }

        [TestMethod]
        public void TestValidPropertyName()
        {
            bool failed = false;
            var db = this.GetDb();

            Mapping.AddTable("posts").
               AddForeignKeyColumn("user_id", "user_id", "users", "id");
            Mapping.AddTable("users")
                .AddPrimaryKeyColumn("id", "id");

            try
            {

                // select * from posts where user_id=id
                // pk_columns pk_table  fk_target = pk
                var main = db
                     .Select<UserLikeTable>(Mapping.GetTable("users"))
                     .Where(Condition.Op("id", 1))
                     .HasMany<PostLikeTable>("PostList", Mapping.GetTable("posts"));
            }
            catch (Exception e)
            {
                failed = true;
            }

            Assert.IsFalse(failed);
        }

        [TestMethod]
        public void TestQuery_WithoutForeignKey_Fail()
        {
            bool failed = false;
            var db = this.GetDb();

            Mapping.AddTable("posts");
            Mapping.AddTable("users");

            try
            {
                var main = db
                    .Select<UserLikeTable>(Mapping.GetTable("users"))
                    .Where(Condition.Op("id", 1))
                    .HasMany<PostLikeTable>("PostList", Mapping.GetTable("posts"));

                var result = main.GetManyRelation<PostLikeTable>().GetQuery();
            }
            catch (Exception e)
            {
                failed = true;
            }

            Assert.IsTrue(failed);
        }

        [TestMethod]
        public void TestQuery_WithoutPrimaryKey_Fail()
        {
            bool failed = false;
            var db = this.GetDb();

            Mapping.AddTable("posts").
                AddForeignKeyColumn("user_id", "user_id", "users", "id");
            Mapping.AddTable("users");

            try
            {
                var main = db
                    .Select<UserLikeTable>(Mapping.GetTable("users"))
                    .Where(Condition.Op("id", 1))
                    .HasMany<PostLikeTable>("PostList", Mapping.GetTable("posts"));

                var result = main.GetManyRelation<PostLikeTable>().GetQuery();
            }
            catch (Exception e)
            {
                failed = true;
            }

            Assert.IsTrue(failed);
        }

        [TestMethod]
        public void TestQuery_WithForeignKey_Success()
        {
            bool failed = false;
            var db = this.GetDb();

            Mapping.AddTable("posts").
                AddForeignKeyColumn("user_id", "user_id", "users", "id");
            Mapping.AddTable("users")
                .AddPrimaryKeyColumn("id","id");

            try
            {
                var main = db
                    .Select<UserLikeTable>(Mapping.GetTable("users"))
                    .Where(Condition.Op("id", 1))
                    .HasMany<PostLikeTable>("PostList", Mapping.GetTable("posts"));

                var result = main.GetManyRelation<PostLikeTable>().GetQuery();
            }
            catch (Exception e)
            {
                failed = true;
            }

            Assert.IsFalse(failed);
        }

        // query

        [TestMethod]
        public void TestQuery()
        {
            var db = this.GetDb();
            Mapping.AddTable("posts").
                AddForeignKeyColumn("user_id", "user_id", "users", "id");
            Mapping.AddTable("users")
                .AddPrimaryKeyColumn("id", "id");

            var main = db
                      .Select<UserLikeTable>(Mapping.GetTable("users"))
                      .Where(Condition.Op("id", 1))
                      .HasMany<PostLikeTable>("PostList", Mapping.GetTable("posts"));

            var result = main.GetManyRelation<PostLikeTable>().GetQuery();


            Assert.AreEqual("select * from [posts] where [user_id]=@id", result);
        }


        // create command 


        [TestMethod]
        public void TestCreateCommand_WithInvalidModelType_Fail()
        {
            bool failed = false;
            var db = this.GetDb();
            Mapping.AddTable("posts").
               AddForeignKeyColumn("user_id", "user_id", "users", "id");
            Mapping.AddTable("users")
                .AddPrimaryKeyColumn("id", "id");

            var main = db
                      .Select<UserLikeTable>(Mapping.GetTable("users"))
                      .Where(Condition.Op("id", 1))
                      .HasMany<PostLikeTable>("PostList", Mapping.GetTable("posts"));

            var model = new Category
            {
                Id = 1,
                Name = "Web"
            };

            try
            {
                var result = main.GetManyRelation<PostLikeTable>().CreateCommand(model);
            }
            catch (Exception e)
            {
                failed = true;
            }

            Assert.IsTrue(failed);
        }

        [TestMethod]
        public void TestCreateCommand_WithNoPrimaryKey_Fail()
        {
            bool failed = false;
            var db = this.GetDb();
            Mapping.AddTable("posts").
                AddForeignKeyColumn("user_id", "user_id", "users", "id");
            Mapping.AddTable("users")
                .AddPrimaryKeyColumn("id", "id");

            var main = db
                      .Select<UserLikeTableWithoutPrimary>(Mapping.GetTable("users"))
                      .Where(Condition.Op("id", 1))
                      .HasMany<PostLikeTable>("PostList", Mapping.GetTable("posts"));


            var model = new UserLikeTableWithoutPrimary { };

            try
            {
                var result = main.GetManyRelation<PostLikeTable>().CreateCommand(model);

            }
            catch (Exception e)
            {
                failed = true;
            }

            Assert.IsTrue(failed);
        }

        [TestMethod]
        public void TestCreateCommand()
        {
            var db = this.GetDb();
            Mapping.AddTable("posts").
                AddForeignKeyColumn("user_id", "user_id", "users", "id");
            Mapping.AddTable("users")
                .AddPrimaryKeyColumn("id", "id");

            var main = db
                      .Select<UserLikeTable>(Mapping.GetTable("users"))
                      .Where(Condition.Op("id", 1))
                      .HasMany<PostLikeTable>("PostList", Mapping.GetTable("posts"));


            var model = new UserLikeTable
            {
                id = 10
            };

            var result = main.GetManyRelation<PostLikeTable>().CreateCommand(model);

            Assert.AreEqual("select * from [posts] where [user_id]=@id", result.Command.CommandText);

            Assert.AreEqual(1, result.Command.Parameters.Count);

            Assert.AreEqual("@id", result.Command.Parameters[0].ParameterName);
            Assert.AreEqual(10, result.Command.Parameters[0].Value);
        }

        // fetch

        [TestMethod]
        public async Task TestFetch_WithNoPropertyToFill_Fail()
        {
            bool failed = false;
            var db = this.GetDb();

            Mapping.AddTable("posts").
              AddForeignKeyColumn("user_id", "user_id", "users", "id");
            Mapping.AddTable("users")
                .AddPrimaryKeyColumn("id", "id");

            var main = db
                      .Select<UserLikeTableWithoutPropertyToFill>(Mapping.GetTable("users"))
                      .Where(Condition.Op("id", 1))
                      .HasMany<PostLikeTable>("PostList", Mapping.GetTable("posts"));


            var model = new UserLikeTableWithoutPropertyToFill
            {
                id = 10
            };

            try
            {
                // => fill post list without property to fill failed
                await main.GetManyRelation<PostLikeTable>().Fetch(model);
            }
            catch (Exception e)
            {
                failed = true;
            }

            Assert.IsTrue(failed);
        }


        [TestMethod]
        public async Task TestFetch()
        {
            var db = this.GetDb();

            Mapping.AddTable("posts").
              AddForeignKeyColumn("user_id", "user_id", "users", "id");
            Mapping.AddTable("users")
                .AddPrimaryKeyColumn("id", "id");

            var main = db
                      .Select<UserLikeTable>(Mapping.GetTable("users"))
                      .Where(Condition.Op("id", 1))
                      .HasMany<PostLikeTable>("PostList", Mapping.GetTable("posts"));


            var model = new UserLikeTable
            {
                id = 1
            };

            // => fill post list without property to fill failed
            await main.GetManyRelation<PostLikeTable>().Fetch(model);

            Assert.IsNotNull(model.PostList);
            Assert.AreEqual(3,model.PostList.Count);

            Assert.AreEqual(1, model.PostList[0].id);
            Assert.AreEqual("Post 1", model.PostList[0].title);
            Assert.AreEqual("Content 1", model.PostList[0].content);
            Assert.AreEqual(1, model.PostList[0].user_id);
            Assert.AreEqual(1, model.PostList[0].category_id);

            Assert.AreEqual(5, model.PostList[1].id);
            Assert.AreEqual("Post 5", model.PostList[1].title);
            Assert.AreEqual("Content 5", model.PostList[1].content);
            Assert.AreEqual(1, model.PostList[1].user_id);
            Assert.AreEqual(2, model.PostList[1].category_id);

            Assert.AreEqual(6, model.PostList[2].id);
            Assert.AreEqual("Post 6", model.PostList[2].title);
            Assert.AreEqual("Content 6", model.PostList[2].content);
            Assert.AreEqual(1, model.PostList[2].user_id);
            Assert.AreEqual(2, model.PostList[2].category_id);
        }


        [TestMethod]
        public async Task TestFetch_WithMapping()
        {
            var db = this.GetDb();

            Mapping.AddTable("posts")
                .AddPrimaryKeyColumn("id", "Id")
                .AddColumn("title", "Title")
                .AddColumn("content", "Content")
                .AddForeignKeyColumn("user_id", "UserId","users","id")
                .AddForeignKeyColumn("category_id", "CategoryId", "categories", "id");

            Mapping.AddTable("users")
                .AddPrimaryKeyColumn("id", "Id")
                .AddColumn("firstname","FirstName")
                .AddColumn("lastname","LastName")
                .AddColumn("age","Age")
                .AddColumn("email","Email");

            var main = db
                      .Select<User>(Mapping.GetTable("users"))
                      .Where(Condition.Op("id", 1))
                      .HasMany<Post>("PostList", Mapping.GetTable("posts"));


            var model = new User
            {
                Id = 1
            };

            // => fill post list without property to fill failed
            await main.GetManyRelation<Post>().Fetch(model);

            Assert.IsNotNull(model.PostList);
            Assert.AreEqual(3, model.PostList.Count);

            Assert.AreEqual(1, model.PostList[0].Id);
            Assert.AreEqual("Post 1", model.PostList[0].Title);
            Assert.AreEqual("Content 1", model.PostList[0].Content);
            Assert.AreEqual(1, model.PostList[0].UserId);
            Assert.AreEqual(1, model.PostList[0].CategoryId);

            Assert.AreEqual(5, model.PostList[1].Id);
            Assert.AreEqual("Post 5", model.PostList[1].Title);
            Assert.AreEqual("Content 5", model.PostList[1].Content);
            Assert.AreEqual(1, model.PostList[1].UserId);
            Assert.AreEqual(2, model.PostList[1].CategoryId);

            Assert.AreEqual(6, model.PostList[2].Id);
            Assert.AreEqual("Post 6", model.PostList[2].Title);
            Assert.AreEqual("Content 6", model.PostList[2].Content);
            Assert.AreEqual(1, model.PostList[2].UserId);
            Assert.AreEqual(2, model.PostList[2].CategoryId);
        }

        [TestMethod]
        public async Task TestFetch_WithMain()
        {
            var db = this.GetDb();

            Mapping.AddTable("posts")
                .AddPrimaryKeyColumn("id", "Id")
                .AddColumn("title", "Title")
                .AddColumn("content", "Content")
                .AddForeignKeyColumn("user_id", "UserId", "users", "id")
                .AddForeignKeyColumn("category_id", "CategoryId", "categories", "id");

            Mapping.AddTable("users")
                .AddPrimaryKeyColumn("id", "Id")
                .AddColumn("firstname", "FirstName")
                .AddColumn("lastname", "LastName")
                .AddColumn("age", "Age")
                .AddColumn("email", "Email");

            var result = await db
                      .Select<User>(Mapping.GetTable("users"))
                      .Where(Condition.Op("id", 1))
                      .HasMany<Post>("PostList", Mapping.GetTable("posts"))
                      .ReadOneAsync();


            Assert.AreEqual(1, result.Id);
            Assert.AreEqual("Marie", result.FirstName);
            Assert.AreEqual("Bellin", result.LastName);
            Assert.AreEqual(null, result.Age);
            Assert.AreEqual("marie@domain.com", result.Email);

            Assert.IsNotNull(result.PostList);
            Assert.AreEqual(3, result.PostList.Count);

            Assert.AreEqual(1, result.PostList[0].Id);
            Assert.AreEqual("Post 1", result.PostList[0].Title);
            Assert.AreEqual("Content 1", result.PostList[0].Content);
            Assert.AreEqual(1, result.PostList[0].UserId);
            Assert.AreEqual(1, result.PostList[0].CategoryId);

            Assert.AreEqual(5, result.PostList[1].Id);
            Assert.AreEqual("Post 5", result.PostList[1].Title);
            Assert.AreEqual("Content 5", result.PostList[1].Content);
            Assert.AreEqual(1, result.PostList[1].UserId);
            Assert.AreEqual(2, result.PostList[1].CategoryId);

            Assert.AreEqual(6, result.PostList[2].Id);
            Assert.AreEqual("Post 6", result.PostList[2].Title);
            Assert.AreEqual("Content 6", result.PostList[2].Content);
            Assert.AreEqual(1, result.PostList[2].UserId);
            Assert.AreEqual(2, result.PostList[2].CategoryId);
        }

        // read all

        [TestMethod]
        public async Task TesReadAllAsync()
        {
            var db = this.GetDb();

            Mapping.AddTable("posts")
                .AddPrimaryKeyColumn("id", "Id")
                .AddColumn("title", "Title")
                .AddColumn("content", "Content")
                .AddForeignKeyColumn("user_id", "UserId", "users", "id")
                .AddForeignKeyColumn("category_id", "CategoryId", "categories", "id");

            Mapping.AddTable("users")
                .AddPrimaryKeyColumn("id", "Id")
                .AddColumn("firstname", "FirstName")
                .AddColumn("lastname", "LastName")
                .AddColumn("age", "Age")
                .AddColumn("email", "Email");

            var results = await db
                      .Select<User>(Mapping.GetTable("users"))
                      .HasMany<Post>("PostList", Mapping.GetTable("posts"))
                      .ReadAllAsync();

            Assert.AreEqual(2, results.Count);

            var result = results[0];

            Assert.AreEqual(1, result.Id);
            Assert.AreEqual("Marie", result.FirstName);
            Assert.AreEqual("Bellin", result.LastName);
            Assert.AreEqual(null, result.Age);
            Assert.AreEqual("marie@domain.com", result.Email);

            Assert.IsNotNull(result.PostList);
            Assert.AreEqual(3, result.PostList.Count);

            Assert.AreEqual(1, result.PostList[0].Id);
            Assert.AreEqual("Post 1", result.PostList[0].Title);
            Assert.AreEqual("Content 1", result.PostList[0].Content);
            Assert.AreEqual(1, result.PostList[0].UserId);
            Assert.AreEqual(1, result.PostList[0].CategoryId);

            Assert.AreEqual(5, result.PostList[1].Id);
            Assert.AreEqual("Post 5", result.PostList[1].Title);
            Assert.AreEqual("Content 5", result.PostList[1].Content);
            Assert.AreEqual(1, result.PostList[1].UserId);
            Assert.AreEqual(2, result.PostList[1].CategoryId);

            Assert.AreEqual(6, result.PostList[2].Id);
            Assert.AreEqual("Post 6", result.PostList[2].Title);
            Assert.AreEqual("Content 6", result.PostList[2].Content);
            Assert.AreEqual(1, result.PostList[2].UserId);
            Assert.AreEqual(2, result.PostList[2].CategoryId);

            var result2 = results[1];

            Assert.AreEqual(2, result2.Id);
            Assert.AreEqual("Pat", result2.FirstName);
            Assert.AreEqual("Prem", result2.LastName);
            Assert.AreEqual(30, result2.Age);
            Assert.AreEqual(null, result2.Email);

            Assert.IsNotNull(result2.PostList);
            Assert.AreEqual(3, result2.PostList.Count);

            Assert.AreEqual(2, result2.PostList[0].Id);
            Assert.AreEqual("Post 2", result2.PostList[0].Title);
            Assert.AreEqual("Content 2", result2.PostList[0].Content);
            Assert.AreEqual(2, result2.PostList[0].UserId);
            Assert.AreEqual(null, result2.PostList[0].CategoryId);

            Assert.AreEqual(3, result2.PostList[1].Id);
            Assert.AreEqual("Post 3", result2.PostList[1].Title);
            Assert.AreEqual("Content 3", result2.PostList[1].Content);
            Assert.AreEqual(2, result2.PostList[1].UserId);
            Assert.AreEqual(2, result2.PostList[1].CategoryId);

            Assert.AreEqual(4, result2.PostList[2].Id);
            Assert.AreEqual("Post 4", result2.PostList[2].Title);
            Assert.AreEqual("Content 4", result2.PostList[2].Content);
            Assert.AreEqual(2, result2.PostList[2].UserId);
            Assert.AreEqual(null, result2.PostList[2].CategoryId);

        }

     
    }

    public class UserLikeTableWithoutPrimary
    {
        public string firstname { get; set; }
        public string lastname { get; set; }
        public int? age { get; set; }
        public string email { get; set; }
    }

    public class UserLikeTableWithoutPropertyToFill
    {
        public int id { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public int? age { get; set; }
        public string email { get; set; }
    }
}
