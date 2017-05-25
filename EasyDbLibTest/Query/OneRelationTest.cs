using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EasyDbLib;
using System.Threading.Tasks;

namespace EasyDbLibTest.Query
{
    [TestClass]
    public class OneRelationTest
    {

        [ClassInitialize()]
        public static void ClassInit(TestContext context)
        {
            InitDb.CreateDbTest();
            Mapping.Clear();
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

            Mapping.SetTable("posts").
              SetForeignKeyColumn("user_id", "user_id", "users", "id");
            Mapping.SetTable("users");

            try
            {
                var main = db
                     .Select<PostLikeTable>(Mapping.GetTable("posts"))
                     .Where(Check.Op("id", 1))
                     .SetZeroOne<UserLikeTable>("U@ser", Mapping.GetTable("users"));
            }
            catch (Exception)
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

            Mapping.SetTable("posts").
                 SetForeignKeyColumn("user_id", "user_id", "users", "id");
            Mapping.SetTable("users");

            try
            {
                var main = db
                      .Select<PostLikeTable>(Mapping.GetTable("posts"))
                      .Where(Check.Op("id", 1))
                      .SetZeroOne<UserLikeTable>("User", Mapping.GetTable("users"));
            }
            catch (Exception)
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

            Mapping.SetTable("posts");
            Mapping.SetTable("users");

            try
            {
                var main = db
                    .Select<PostLikeTable>(Mapping.GetTable("posts"))
                    .Where(Check.Op("id", 1))
                    .SetZeroOne<UserLikeTable>("User", Mapping.GetTable("users"));

                // wait for "user_id" in "posts"
                var result = main.GetOneRelation<UserLikeTable>().GetQuery();
            }
            catch (Exception e)
            {
                failed = true;
            }

            Assert.IsTrue(failed);
        }

        [TestMethod]
        public void TestQuery_WithForeign_Success()
        {
            bool failed = false;
            var db = this.GetDb();

            Mapping.SetTable("posts").
                SetForeignKeyColumn("user_id","user_id","users","id");
            Mapping.SetTable("users");

            try
            {
                var main = db
                    .Select<PostLikeTable>(Mapping.GetTable("posts"))
                    .Where(Check.Op("id", 1))
                    .SetZeroOne<UserLikeTable>("User", Mapping.GetTable("users"));

                var result = main.GetOneRelation<UserLikeTable>().GetQuery();
            }
            catch (Exception)
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
            Mapping.SetTable("posts").
                        SetForeignKeyColumn("user_id", "user_id", "users", "id");
            Mapping.SetTable("users");

            var main = db
                      .Select<PostLikeTable>(Mapping.GetTable("posts"))
                      .Where(Check.Op("id", 1))
                      .SetZeroOne<UserLikeTable>("User", Mapping.GetTable("users"));

            var result = main.GetOneRelation<UserLikeTable>().GetQuery();


            Assert.AreEqual("select * from [users] where [id]=@user_id", result);
        }

        // create command 


        [TestMethod]
        public void TestCreateCommand_WithInvalidModelType_Fail()
        {
            bool failed = false;
            var db = this.GetDb();
            Mapping.SetTable("posts").
                        SetForeignKeyColumn("user_id", "user_id", "users", "id");
            Mapping.SetTable("users");

            var main = db
                      .Select<PostLikeTable>(Mapping.GetTable("posts"))
                      .Where(Check.Op("id", 1))
                      .SetZeroOne<UserLikeTable>("User", Mapping.GetTable("users"));

            var model = new Category
            {
                Id = 1,
                Name ="Web"
            };

            try
            {
                var result = main.GetOneRelation<UserLikeTable>().CreateCommand(model);

            }
            catch (Exception e)
            {
                failed = true;
            }

            Assert.IsTrue(failed);
        }

        [TestMethod]
        public void TestCreateCommand_WithNoForeignKeyProperty_Fail()
        {
            bool failed = false;
            var db = this.GetDb();
            Mapping.SetTable("posts").
                        SetForeignKeyColumn("user_id", "user_id", "users", "id");
            Mapping.SetTable("users");

            var main = db
                      .Select<PostLikeTableWithoutForeignKeys>(Mapping.GetTable("posts"))
                      .Where(Check.Op("id", 1))
                      .SetZeroOne<UserLikeTable>("User", Mapping.GetTable("users"));

            var post = new PostLikeTableWithoutForeignKeys
            {
                id = 1,
                title = "Post 1",
                content = "Content 1"
            };


            try
            {
                var result = main.GetOneRelation<UserLikeTable>().CreateCommand(post);
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
            Mapping.SetTable("posts").
                        SetForeignKeyColumn("user_id", "user_id", "users", "id");
            Mapping.SetTable("users");

            var main = db
                      .Select<PostLikeTable>(Mapping.GetTable("posts"))
                      .Where(Check.Op("id", 1))
                      .SetZeroOne<UserLikeTable>("User", Mapping.GetTable("users"));

            var post = new PostLikeTable
            {
                id = 1,
                title = "Post 1",
                content = "Content 1",
                user_id = 10
            };

            var result = main.GetOneRelation<UserLikeTable>().CreateCommand(post);

            Assert.AreEqual("select * from [users] where [id]=@user_id", result.Command.CommandText);

            Assert.AreEqual(1, result.Command.Parameters.Count);

            Assert.AreEqual("@user_id", result.Command.Parameters[0].ParameterName);
            Assert.AreEqual(10, result.Command.Parameters[0].Value);
        }

        // fetch

        [TestMethod]
        public async Task TestFetch_WithNoPropertyToFill_Fail()
        {
            bool failed = false;
            var db = this.GetDb();
            Mapping.SetTable("posts").
                        SetForeignKeyColumn("user_id", "user_id", "users", "id");
            Mapping.SetTable("users");

            var main = db
                      .Select<PostLikeTableWithoutPropertyToFill>(Mapping.GetTable("posts"))
                      .Where(Check.Op("id", 1))
                      .SetZeroOne<UserLikeTable>("User", Mapping.GetTable("users"));

            var post = new PostLikeTableWithoutPropertyToFill
            {
                id = 1,
                title = "Post 1",
                content = "Content 1",
                user_id = 1
            };

            try
            {
                // => fill User property for the post model
                await main.GetOneRelation<UserLikeTable>().Fetch(post);
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
            Mapping.SetTable("posts").
                        SetForeignKeyColumn("user_id", "user_id", "users", "id");
            Mapping.SetTable("users");

            var main = db
                      .Select<PostLikeTable>(Mapping.GetTable("posts"))
                      .Where(Check.Op("id", 1))
                      .SetZeroOne<UserLikeTable>("User", Mapping.GetTable("users"));

            var post = new PostLikeTable
            {
                id = 1,
                title = "Post 1",
                content = "Content 1",
                user_id = 1
            };

            // => fill User property for the post model
            await main.GetOneRelation<UserLikeTable>().Fetch(post);

            Assert.IsNotNull(post.User);
            Assert.AreEqual(1, post.User.id);
            Assert.AreEqual("Marie", post.User.firstname);
            Assert.AreEqual("Bellin", post.User.lastname);
            Assert.AreEqual(null, post.User.age);
            Assert.AreEqual("marie@domain.com", post.User.email);
        }

        [TestMethod]
        public void TestFetch_HasMultipleRelationOnes()
        {
            var db = this.GetDb();
            Mapping.SetTable("posts")
                .SetForeignKeyColumn("user_id", "user_id", "users", "id")
                .SetForeignKeyColumn("category_id", "category_id", "categories", "id");
            Mapping.SetTable("categories").
                      SetForeignKeyColumn("category_id", "category_id", "categories", "id");
            Mapping.SetTable("users");

            var post = new PostLikeTable
            {
                id = 1,
                title = "Post 1",
                content = "Content 1",
                user_id = 1,
                category_id = 1
            };

            // => fill User property for the post model
            var result = db
                    .Select<PostLikeTable>(Mapping.GetTable("posts"))
                    .Where(Check.Op("id", 1))
                    .SetZeroOne<UserLikeTable>("User", Mapping.GetTable("users"))
                    .SetZeroOne<CategoryLikeTable>("Category", Mapping.GetTable("categories"));

            Assert.IsNotNull(result.HasOneRelation<UserLikeTable>());
            Assert.IsNotNull(result.HasOneRelation<CategoryLikeTable>());
        }


        [TestMethod]
        public async Task TestFetch_WithZeroOne_ReturnsNull()
        {
            var db = this.GetDb();

            Mapping.SetTable("posts")
                .SetForeignKeyColumn("category_id", "category_id", "categories", "id");

            Mapping.SetTable("categories");

            // => fill User property for the post model
            var result = await db
                    .Select<PostLikeTable>(Mapping.GetTable("posts"))
                    .Where(Check.Op("id", 2))
                    .SetZeroOne<CategoryLikeTable>("Category", Mapping.GetTable("categories"))
                    .ReadOneAsync();


            Assert.IsNull(result.Category);
        }


        [TestMethod]
        public async Task TestFetch_WithZeroOne_ReturnsResult()
        {
            var db = this.GetDb();
            Mapping.SetTable("posts")
                .SetForeignKeyColumn("category_id", "category_id", "categories", "id");
            Mapping.SetTable("categories");


            // => fill User property for the post model
            var result = await db
                    .Select<PostLikeTable>(Mapping.GetTable("posts"))
                    .Where(Check.Op("id", 1))
                    .SetZeroOne<CategoryLikeTable>("Category", Mapping.GetTable("categories"))
                    .ReadOneAsync();


            Assert.IsNotNull(result.Category);
            Assert.AreEqual(1, result.Category.id);
            Assert.AreEqual("Web", result.Category.name);
        }

        [TestMethod]
        public async Task TestFetch_WithMapping()
        {
            var db = this.GetDb();

            Mapping.SetTable("posts")
                .SetPrimaryKeyColumn("id","Id")
                .SetColumn("title","Title")
                .SetColumn("content","Content")
                .SetForeignKeyColumn("category_id", "CategoryId", "categories", "id");

            Mapping.SetTable("categories")
                .SetColumn("id","Id")
                .SetColumn("name","Name");

            Mapping.SetTable("categories")
                .SetColumn("id", "Id")
                .SetColumn("name", "Name");


            // => fill User property for the post model
            var result = await db
                    .Select<Post>(Mapping.GetTable("posts"))
                    .Where(Check.Op("id", 1))
                    .SetZeroOne<Category>("Category", Mapping.GetTable("categories"))
                    .ReadOneAsync();


            Assert.IsNotNull(result.Category);
            Assert.AreEqual(1, result.Category.Id);
            Assert.AreEqual("Web", result.Category.Name);
        }


        [TestMethod]
        public async Task TestFetch_MultipleRelationOnes()
        {
            var db = this.GetDb();

            Mapping.SetTable("posts")
                .SetForeignKeyColumn("user_id", "user_id", "users", "id")
                .SetForeignKeyColumn("category_id","category_id","categories","id");
            Mapping.SetTable("categories").
                      SetForeignKeyColumn("category_id", "category_id", "categories", "id");
            Mapping.SetTable("users");

            // => fill User property for the post model
            var result = await db
                    .Select<PostLikeTable>(Mapping.GetTable("posts"))
                    .Where(Check.Op("id", 1))
                    .SetZeroOne<UserLikeTable>("User", Mapping.GetTable("users"))
                    .SetZeroOne<CategoryLikeTable>("Category", Mapping.GetTable("categories"))
                    .ReadOneAsync();

            Assert.IsNotNull(result.User);
            Assert.AreEqual(1, result.User.id);
            Assert.AreEqual("Marie", result.User.firstname);
            Assert.AreEqual("Bellin", result.User.lastname);
            Assert.AreEqual(null, result.User.age);
            Assert.AreEqual("marie@domain.com", result.User.email);

            Assert.IsNotNull(result.Category);
            Assert.AreEqual(1, result.Category.id);
            Assert.AreEqual("Web", result.Category.name);
        }
    }

    public class PostLikeTableWithoutForeignKeys
    {
        public int id { get; set; }
        public string title { get; set; }
        public string content { get; set; }
    }

    public class PostLikeTableWithoutPropertyToFill
    {
        public int id { get; set; }
        public string title { get; set; }
        public string content { get; set; }
        public int user_id { get; set; }
    }


    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class CategoryLikeTable
    {
        public int id { get; set; }
        public string name { get; set; }
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


    public class PostLikeTable
    {
        public int id { get; set; }
        public string title { get; set; }
        public string content { get; set; }

        public int user_id { get; set; }
        public UserLikeTable User { get; set; }

        public int? category_id { get; set; }
        public CategoryLikeTable Category { get; set; }
    }
}
