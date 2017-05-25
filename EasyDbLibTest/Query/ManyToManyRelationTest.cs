using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EasyDbLib;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EasyDbLibTest.Query
{
    [TestClass]
    public class ManyToManyRelationTest
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

            Mapping.SetTable("users")
                .SetPrimaryKeyColumn("id", "id");

            Mapping.SetTable("permissions");

            Mapping.SetIntermediateTable("users_permissions")
                .SetPrimaryKeyColumn("user_id", "id", "users")
                .SetPrimaryKeyColumn("permission_id", "id", "permissions");

            try
            {
                var main = db
                     .Select<UserLikeTable>(Mapping.GetTable("users"))
                     .Where(Check.Op("id", 1))
                     .SetManyToMany<PermissionLikeTable>("Per@List", Mapping.GetTable("permissions"),Mapping.GetIntermediateTable("users_permissions"));
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

            Mapping.SetTable("users")
                .SetPrimaryKeyColumn("id", "id");

            Mapping.SetTable("permissions");

            Mapping.SetIntermediateTable("users_permissions")
                .SetPrimaryKeyColumn("user_id", "id", "users")
                .SetPrimaryKeyColumn("permission_id", "id", "permissions");

            try
            {
                var main = db
                     .Select<UserLikeTable>(Mapping.GetTable("users"))
                     .Where(Check.Op("id", 1))
                     .SetManyToMany<PermissionLikeTable>("PermissionList", Mapping.GetTable("permissions"), Mapping.GetIntermediateTable("users_permissions"));
            }
            catch (Exception e)
            {
                failed = true;
            }

            Assert.IsFalse(failed);
        }

        [TestMethod]
        public void TestQuery_WithoutIntermediatePrimaryKeys_Fail()
        {
            bool failed = false;
            var db = this.GetDb();

            Mapping.SetTable("users")
                .SetPrimaryKeyColumn("id", "id");

            Mapping.SetTable("permissions");

            Mapping.SetIntermediateTable("users_permissions")
                  .SetPrimaryKeyColumn("permission_id", "id", "permissions");

            try
            {
                var main = db
                     .Select<UserLikeTable>(Mapping.GetTable("users"))
                     .Where(Check.Op("id", 1))
                     .SetManyToMany<PermissionLikeTable>("PermissionList", Mapping.GetTable("permissions"), Mapping.GetIntermediateTable("users_permissions"));
            }
            catch (Exception e)
            {
                failed = true;
            }

            Assert.IsTrue(failed);
        }

        [TestMethod]
        public void TestQuery_WithoutIntermediatePrimaryKeysForCheckValue_Fail()
        {
            bool failed = false;
            var db = this.GetDb();

            Mapping.SetTable("users")
                .SetPrimaryKeyColumn("id", "id");

            Mapping.SetTable("permissions");

            Mapping.SetIntermediateTable("users_permissions")
                .SetPrimaryKeyColumn("user_id", "id", "users");

            try
            {
                var main = db
                     .Select<UserLikeTable>(Mapping.GetTable("users"))
                     .Where(Check.Op("id", 1))
                     .SetManyToMany<PermissionLikeTable>("PermissionList", Mapping.GetTable("permissions"), Mapping.GetIntermediateTable("users_permissions"));
            }
            catch (Exception e)
            {
                failed = true;
            }

            Assert.IsTrue(failed);
        }

        [TestMethod]
        public void TestQuery_WithoutNoPrimaryKeysInModel_Fail()
        {
            bool failed = false;
            var db = this.GetDb();

            Mapping.SetTable("users");

            Mapping.SetTable("permissions");

            Mapping.SetIntermediateTable("users_permissions")
                .SetPrimaryKeyColumn("user_id", "id", "users")
                .SetPrimaryKeyColumn("permission_id", "id", "permissions");

            try
            {
                var main = db
                     .Select<UserLikeTable>(Mapping.GetTable("users"))
                     .Where(Check.Op("id", 1))
                     .SetManyToMany<PermissionLikeTable>("PermissionList", Mapping.GetTable("permissions"), Mapping.GetIntermediateTable("users_permissions"));
            }
            catch (Exception e)
            {
                failed = true;
            }

            Assert.IsTrue(failed);
        }

        [TestMethod]
        public void TestQuery_WithKeys_Success()
        {
            bool failed = false;
            var db = this.GetDb();

            Mapping.SetTable("users")
                .SetPrimaryKeyColumn("id", "id");

            Mapping.SetTable("permissions");

            Mapping.SetIntermediateTable("users_permissions")
                .SetPrimaryKeyColumn("user_id", "id", "users")
                .SetPrimaryKeyColumn("permission_id", "id", "permissions");

            try
            {
                var main = db
                     .Select<UserLikeTable>(Mapping.GetTable("users"))
                     .Where(Check.Op("id", 1))
                     .SetManyToMany<PermissionLikeTable>("PermissionList", Mapping.GetTable("permissions"), Mapping.GetIntermediateTable("users_permissions"));
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

            Mapping.SetTable("users")
                .SetPrimaryKeyColumn("id", "id");

            Mapping.SetTable("permissions");

            Mapping.SetIntermediateTable("users_permissions")
                .SetPrimaryKeyColumn("user_id", "id", "users")
                .SetPrimaryKeyColumn("permission_id", "id", "permissions");

            var main = db
                 .Select<UserLikeTable>(Mapping.GetTable("users"))
                 .Where(Check.Op("id", 1))
                 .SetManyToMany<PermissionLikeTable>("PermissionList", Mapping.GetTable("permissions"), Mapping.GetIntermediateTable("users_permissions"));

            var result = main.GetManyToManyRelation<PermissionLikeTable>().GetQuery();

            Assert.AreEqual("select [permissions].* from [users_permissions],[permissions] where [users_permissions].[permission_id]=[permissions].[id] and [users_permissions].[user_id]=@user_id", result);
        }

        [TestMethod]
        public void TestQuery_WithColumns()
        {
            var db = this.GetDb();

            Mapping.SetTable("users")
                .SetPrimaryKeyColumn("id", "id");

            Mapping.SetTable("permissions")
                .SetPrimaryKeyColumn("id","Id")
                .SetColumn("name","Name")
                .SetColumn("description","Description");

            Mapping.SetIntermediateTable("users_permissions")
                .SetPrimaryKeyColumn("user_id", "id", "users")
                .SetPrimaryKeyColumn("permission_id", "id", "permissions");

            var main = db
                 .Select<UserLikeTable>(Mapping.GetTable("users"))
                 .Where(Check.Op("id", 1))
                 .SetManyToMany<PermissionLikeTable>("PermissionList", Mapping.GetTable("permissions"), Mapping.GetIntermediateTable("users_permissions"));

            var result = main.GetManyToManyRelation<PermissionLikeTable>().GetQuery();

            Assert.AreEqual("select [permissions].[id],[permissions].[name],[permissions].[description] from [users_permissions],[permissions] where [users_permissions].[permission_id]=[permissions].[id] and [users_permissions].[user_id]=@user_id", result);
        }

        // create command

        [TestMethod]
        public void TestCreateCommand_WithInvalidModelType_Fail()
        {
            bool failed = false;
            var db = this.GetDb();

            Mapping.SetTable("users")
                .SetPrimaryKeyColumn("id", "id");

            Mapping.SetTable("permissions");

            Mapping.SetIntermediateTable("users_permissions")
                .SetPrimaryKeyColumn("user_id", "id", "users")
                .SetPrimaryKeyColumn("permission_id", "id", "permissions");

            var main = db
                 .Select<UserLikeTable>(Mapping.GetTable("users"))
                 .Where(Check.Op("id", 1))
                 .SetManyToMany<PermissionLikeTable>("PermissionList", Mapping.GetTable("permissions"), Mapping.GetIntermediateTable("users_permissions"));

            var model = new Category
            {
                Id = 1
            };

            try
            {
                var result = main.GetManyToManyRelation<PermissionLikeTable>().CreateCommand(model);
            }
            catch (Exception)
            {
                failed = true;
            }

            Assert.IsTrue(failed);
        }


        [TestMethod]
        public void TestCreateCommand_WithNoPrimaryPropertyInModel_Fail()
        {
            bool failed = false;
            var db = this.GetDb();

            Mapping.SetTable("users")
                .SetPrimaryKeyColumn("id", "id");

            Mapping.SetTable("permissions");

            Mapping.SetIntermediateTable("users_permissions")
                .SetPrimaryKeyColumn("user_id", "id", "users")
                .SetPrimaryKeyColumn("permission_id", "id", "permissions");

            var main = db
                 .Select<UserLikeTableWithoutPrimary>(Mapping.GetTable("users"))
                 .Where(Check.Op("id", 1))
                 .SetManyToMany<PermissionLikeTable>("PermissionList", Mapping.GetTable("permissions"), Mapping.GetIntermediateTable("users_permissions"));

            var model = new UserLikeTableWithoutPrimary
            { };

            try
            {
                var result = main.GetManyToManyRelation<PermissionLikeTable>().CreateCommand(model);
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

            Mapping.SetTable("users")
                .SetPrimaryKeyColumn("id", "id");

            Mapping.SetTable("permissions");

            Mapping.SetIntermediateTable("users_permissions")
                .SetPrimaryKeyColumn("user_id", "id", "users")
                .SetPrimaryKeyColumn("permission_id", "id", "permissions");

            var main = db
                 .Select<UserLikeTable>(Mapping.GetTable("users"))
                 .Where(Check.Op("id", 1))
                 .SetManyToMany<PermissionLikeTable>("PermissionList", Mapping.GetTable("permissions"), Mapping.GetIntermediateTable("users_permissions"));

            var model = new UserLikeTable
            {
                id = 10
            };

            var result = main.GetManyToManyRelation<PermissionLikeTable>().CreateCommand(model);

            Assert.AreEqual("select [permissions].* from [users_permissions],[permissions] where [users_permissions].[permission_id]=[permissions].[id] and [users_permissions].[user_id]=@user_id", result.Command.CommandText);

            Assert.AreEqual(1, result.Command.Parameters.Count);

            Assert.AreEqual("@user_id", result.Command.Parameters[0].ParameterName);
            Assert.AreEqual(10, result.Command.Parameters[0].Value);
        }


        // fetch

        [TestMethod]
        public async Task TestFetch()
        {
            var db = this.GetDb();

            Mapping.SetTable("users")
                .SetPrimaryKeyColumn("id", "id");

            Mapping.SetTable("permissions");

            Mapping.SetIntermediateTable("users_permissions")
                .SetPrimaryKeyColumn("user_id", "id", "users")
                .SetPrimaryKeyColumn("permission_id", "id", "permissions");

            var main = db
                 .Select<UserLikeTable>(Mapping.GetTable("users"))
                 .Where(Check.Op("id", 1))
                 .SetManyToMany<PermissionLikeTable>("PermissionList", Mapping.GetTable("permissions"), Mapping.GetIntermediateTable("users_permissions"));

            var model = new UserLikeTable
            {
                id = 1
            };

            await main.GetManyToManyRelation<PermissionLikeTable>().Fetch(model);

            Assert.IsNotNull(model.PermissionList);
            Assert.AreEqual(4,model.PermissionList.Count);

            Assert.AreEqual(1, model.PermissionList[0].id);
            Assert.AreEqual("read posts", model.PermissionList[0].name);
            Assert.AreEqual("description read posts", model.PermissionList[0].description);

            Assert.AreEqual(2, model.PermissionList[1].id);
            Assert.AreEqual("create posts", model.PermissionList[1].name);
            Assert.AreEqual("description create posts", model.PermissionList[1].description);

            Assert.AreEqual(3, model.PermissionList[2].id);
            Assert.AreEqual("edit posts", model.PermissionList[2].name);
            Assert.AreEqual("description edit posts", model.PermissionList[2].description);

            Assert.AreEqual(4, model.PermissionList[3].id);
            Assert.AreEqual("delete posts", model.PermissionList[3].name);
            Assert.AreEqual("description delete posts", model.PermissionList[3].description);

        }

        [TestMethod]
        public async Task TestFetch_UseId()
        {
            var db = this.GetDb();

            Mapping.SetTable("users")
                .SetPrimaryKeyColumn("id", "id");

            Mapping.SetTable("permissions");

            Mapping.SetIntermediateTable("users_permissions")
                .SetPrimaryKeyColumn("user_id", "id", "users")
                .SetPrimaryKeyColumn("permission_id", "id", "permissions");

            var main = db
                 .Select<UserLikeTable>(Mapping.GetTable("users"))
                 .Where(Check.Op("id", 2))
                 .SetManyToMany<PermissionLikeTable>("PermissionList", Mapping.GetTable("permissions"), Mapping.GetIntermediateTable("users_permissions"));

            var model = new UserLikeTable
            {
                id = 2
            };

            await main.GetManyToManyRelation<PermissionLikeTable>().Fetch(model);

            Assert.IsNotNull(model.PermissionList);
            Assert.AreEqual(1, model.PermissionList.Count);

            Assert.AreEqual(1, model.PermissionList[0].id);
            Assert.AreEqual("read posts", model.PermissionList[0].name);
            Assert.AreEqual("description read posts", model.PermissionList[0].description);
        }

        [TestMethod]
        public async Task TestFetchFromModel()
        {
            var db = this.GetDb();

            Mapping.SetTable("users")
                .SetPrimaryKeyColumn("id", "id");

            Mapping.SetTable("permissions");

            Mapping.SetIntermediateTable("users_permissions")
                .SetPrimaryKeyColumn("user_id", "id", "users")
                .SetPrimaryKeyColumn("permission_id", "id", "permissions");

            var result = await db
                 .Select<UserLikeTable>(Mapping.GetTable("users"))
                 .Where(Check.Op("id", 2))
                 .SetManyToMany<PermissionLikeTable>("PermissionList", Mapping.GetTable("permissions"), Mapping.GetIntermediateTable("users_permissions"))
                 .ReadOneAsync();


            Assert.IsNotNull(result.PermissionList);
            Assert.AreEqual(1, result.PermissionList.Count);

            Assert.AreEqual(1, result.PermissionList[0].id);
            Assert.AreEqual("read posts", result.PermissionList[0].name);
            Assert.AreEqual("description read posts", result.PermissionList[0].description);
        }

        [TestMethod]
        public async Task TesReadAllAsync()
        {
            var db = this.GetDb();

            Mapping.SetTable("users")
                .SetPrimaryKeyColumn("id", "id");

            Mapping.SetTable("permissions");

            Mapping.SetIntermediateTable("users_permissions")
                .SetPrimaryKeyColumn("user_id", "id", "users")
                .SetPrimaryKeyColumn("permission_id", "id", "permissions");

            var results = await db
                 .Select<UserLikeTable>(Mapping.GetTable("users"))
                 .SetManyToMany<PermissionLikeTable>("PermissionList", Mapping.GetTable("permissions"), Mapping.GetIntermediateTable("users_permissions"))
                 .ReadAllAsync();

            var result = results[0];

            Assert.AreEqual(1, result.PermissionList[0].id);
            Assert.AreEqual("read posts", result.PermissionList[0].name);
            Assert.AreEqual("description read posts", result.PermissionList[0].description);

            Assert.AreEqual(2, result.PermissionList[1].id);
            Assert.AreEqual("create posts", result.PermissionList[1].name);
            Assert.AreEqual("description create posts", result.PermissionList[1].description);

            Assert.AreEqual(3, result.PermissionList[2].id);
            Assert.AreEqual("edit posts", result.PermissionList[2].name);
            Assert.AreEqual("description edit posts", result.PermissionList[2].description);

            Assert.AreEqual(4, result.PermissionList[3].id);
            Assert.AreEqual("delete posts", result.PermissionList[3].name);
            Assert.AreEqual("description delete posts", result.PermissionList[3].description);


            var result2 = results[1];

            Assert.IsNotNull(result2.PermissionList);
            Assert.AreEqual(1, result2.PermissionList.Count);

            Assert.AreEqual(1, result2.PermissionList[0].id);
            Assert.AreEqual("read posts", result2.PermissionList[0].name);
            Assert.AreEqual("description read posts", result2.PermissionList[0].description);
        }

    }

    public class PermissionLikeTable
    {
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
    }

}
