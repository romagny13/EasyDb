using System;
using EasyDbLib.Tests.Factory;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EasyDbLib.Tests
{
    [TestClass]
    public class EasyDbMappingTests
    {

        [TestMethod]
        public void SetTable_RegisterTable()
        {
            var db = new EasyDb();

            db.SetTable<User>("users");
            db.SetTable<Role>("roles");

            Assert.IsTrue(db.IsTableRegistered<User>());
            Assert.IsTrue(db.IsTableRegistered<Role>());
        }

        [TestMethod]
        public void SetTable_WithColumn()
        {
            var db = new EasyDb();

            db
                .SetTable<User>("users")
                .SetColumn("username", p => p.UserName);

            var table = db.GetTable<User>();
            var result = table.MappingByColumnName["username"] as Column;

            Assert.AreEqual(typeof(User), table.ModelType);
            Assert.AreEqual("users", table.TableName);
            Assert.AreEqual(typeof(User), result.ModelType);
            Assert.AreEqual("users", result.TableName);
            Assert.AreEqual(typeof(Column), result.GetType());
            Assert.AreEqual("username", result.ColumnName);
            Assert.AreEqual("UserName", result.Property.Name);
            Assert.AreEqual(typeof(User), result.ModelType);
            Assert.AreEqual(false, result.IsDatabaseGenerated);
            Assert.AreEqual(false, result.IsIgnored);
        }

        [TestMethod]
        public void TryGetTable()
        {
            var db = new EasyDb();

            db.DefaultMappingBehavior = DefaultMappingBehavior.None;

            var table = db.SetTable<User>("users");

            Assert.IsNotNull(db.TryGetTable<User>());
            Assert.AreEqual(table, db.TryGetTable<User>());
            Assert.IsNull(db.TryGetTable<Role>());
        }

        [TestMethod]
        public void DiscoverTable()
        {
            var db = new EasyDb();

            var result = db.DiscoverMappingFor<UserWithAttributes>();

            Assert.AreEqual("Users", result.TableName);
            Assert.AreEqual(typeof(UserWithAttributes), result.ModelType);

            Assert.AreEqual(5, result.MappingByColumnName.Count);

            // Id
            var idColumn = result.MappingByColumnName["Id"];
            Assert.AreEqual(typeof(PrimaryKeyColumn), idColumn.GetType());
            Assert.AreEqual("Id", idColumn.ColumnName);
            Assert.AreEqual("Id", idColumn.Property.Name);
            Assert.AreEqual(typeof(UserWithAttributes), idColumn.ModelType);
            Assert.AreEqual("Users", idColumn.TableName);
            Assert.AreEqual(true, idColumn.IsDatabaseGenerated);
            Assert.AreEqual(false, idColumn.IsIgnored);

            // UserName
            var userNameColumn = result.MappingByColumnName["UserName"];
            Assert.AreEqual(typeof(Column), userNameColumn.GetType());
            Assert.AreEqual("UserName", userNameColumn.ColumnName);
            Assert.AreEqual("UserName", userNameColumn.Property.Name);
            Assert.AreEqual(typeof(UserWithAttributes), userNameColumn.ModelType);
            Assert.AreEqual("Users", userNameColumn.TableName);
            Assert.AreEqual(false, userNameColumn.IsDatabaseGenerated);
            Assert.AreEqual(false, userNameColumn.IsIgnored);

            // RowVersion
            var rowVersionColumn = result.MappingByColumnName["RowVersion"];
            Assert.AreEqual(typeof(Column), rowVersionColumn.GetType());
            Assert.AreEqual("RowVersion", rowVersionColumn.ColumnName);
            Assert.AreEqual("RowVersion", rowVersionColumn.Property.Name);
            Assert.AreEqual(typeof(UserWithAttributes), rowVersionColumn.ModelType);
            Assert.AreEqual("Users", rowVersionColumn.TableName);
            Assert.AreEqual(true, rowVersionColumn.IsDatabaseGenerated);
            Assert.AreEqual(false, userNameColumn.IsIgnored);

            // role id
            var roleIdColumn = result.MappingByColumnName["role_id"];
            Assert.AreEqual(typeof(Column), roleIdColumn.GetType());
            Assert.AreEqual("role_id", roleIdColumn.ColumnName);
            Assert.AreEqual("RoleId", roleIdColumn.Property.Name);
            Assert.AreEqual(typeof(UserWithAttributes), roleIdColumn.ModelType);
            Assert.AreEqual("Users", roleIdColumn.TableName);
            Assert.AreEqual(false, roleIdColumn.IsDatabaseGenerated);
            Assert.AreEqual(false, roleIdColumn.IsIgnored);

            // Age
            var ageColumn = result.MappingByColumnName["Age"];
            Assert.AreEqual(typeof(Column), ageColumn.GetType());
            Assert.AreEqual("Age", ageColumn.ColumnName);
            Assert.AreEqual("Age", ageColumn.Property.Name);
            Assert.AreEqual(typeof(UserWithAttributes), ageColumn.ModelType);
            Assert.AreEqual("Users", ageColumn.TableName);
            Assert.AreEqual(false, ageColumn.IsDatabaseGenerated);
            Assert.AreEqual(true, ageColumn.IsIgnored);
        }
    }
}
