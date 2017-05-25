using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EasyDbLib;

namespace EasyDbLibTest
{
    [TestClass]
    public class IntermediateTableTest
    {
        [ClassInitialize()]
        public static void ClassInit(TestContext context)
        {
            Mapping.Clear();
        }

        // table

        [TestMethod]
        public void TestRegister()
        {
            Mapping.SetIntermediateTable("users_permissions");

            Assert.IsTrue(Mapping.HasIntermediateTable("users_permissions"));
        }


        [TestMethod]
        public void TestGetTable()
        {
            Mapping.SetIntermediateTable("users_permissions");

            var table = Mapping.GetIntermediateTable("users_permissions");

            Assert.AreEqual("users_permissions", table.TableName);

        }

        // set

        [TestMethod]
        public void TestSet()
        {
            Mapping.SetIntermediateTable("users_permissions");

            Mapping.SetIntermediateTable("users_permissions")
                .SetPrimaryKeyColumn("permission_id", "id", "permissions");

            Assert.IsTrue(Mapping.GetIntermediateTable("users_permissions").HasColumn("permission_id"));
        }

        [TestMethod]
        public void TestRemoveTable()
        {
            Mapping.SetIntermediateTable("users_permissions");

            Assert.IsTrue(Mapping.HasIntermediateTable("users_permissions"));

            Assert.IsTrue(Mapping.RemoveIntermediateTable("users_permissions"));
            Assert.IsFalse(Mapping.HasIntermediateTable("users_permissions"));
        }

        // column

        [TestMethod]
        public void TestAddColumn()
        {
            Mapping.SetIntermediateTable("users_permissions")
                .SetPrimaryKeyColumn("user_id", "id", "users");

            Assert.IsTrue(Mapping.GetIntermediateTable("users_permissions").HasColumn("user_id"));
        }

        [TestMethod]
        public void TestACannotAddColumns_WithSameName()
        {
            bool failed = false;
            try
            {
                Mapping.SetIntermediateTable("users_permissions")
                    .SetPrimaryKeyColumn("user_id", "id", "users")
                    .SetPrimaryKeyColumn("user_id","id2","users");
            }
            catch (Exception)
            {
                failed = true;
            }

            Assert.IsTrue(failed);
        }

        [TestMethod]
        public void TestGetColumn()
        {
            Mapping.SetIntermediateTable("users_permissions")
                .SetPrimaryKeyColumn("user_id", "id", "users")
                .SetPrimaryKeyColumn("permission_id", "id", "permissions");

            var column = Mapping.GetIntermediateTable("users_permissions").GetColumn("user_id");

            Assert.AreEqual("user_id", column.ColumnName);
            Assert.AreEqual("id", column.TargetPrimaryKey);
            Assert.AreEqual("users", column.TargetTableName);

            var column2 = Mapping.GetIntermediateTable("users_permissions").GetColumn("permission_id");

            Assert.AreEqual("permission_id", column2.ColumnName);
            Assert.AreEqual("id", column2.TargetPrimaryKey);
            Assert.AreEqual("permissions", column2.TargetTableName);

        }
    }
}
