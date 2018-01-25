using EasyDbLib.Tests.Factory;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace EasyDbLib.Tests
{
    [TestClass]
    public class DbHelperTests
    {
        // object to sql conversion
        // model type / name => (mapping) => table name (model type name or mapping table name)
        // properties => (mapping) => columns (mapping or property name)
        // properties => parameters 
        //          - column name (mapping or property name)
        //          - parameter name @id (@ + lower case column name)


        [TestMethod]
        public void AddParameter()
        {
            var factory = DbProviderFactories.GetFactory("System.Data.SqlClient");
            var command = factory.CreateCommand();

            //
            DbHelper.AddParameter(command, "@id", 10, ParameterDirection.Input);

            Assert.AreEqual(1, command.Parameters.Count);

            Assert.AreEqual("@id", command.Parameters[0].ParameterName);
            Assert.AreEqual(10, command.Parameters[0].Value);
            Assert.AreEqual(ParameterDirection.Input, command.Parameters[0].Direction);
            Assert.AreEqual(DbType.Int32, command.Parameters[0].DbType);

            //
            DbHelper.AddParameter(command, "@id2", 100, ParameterDirection.Output, DbType.Int16);

            Assert.AreEqual(2, command.Parameters.Count);

            Assert.AreEqual("@id2", command.Parameters[1].ParameterName);
            Assert.AreEqual(100, command.Parameters[1].Value);
            Assert.AreEqual(ParameterDirection.Output, command.Parameters[1].Direction);
            Assert.AreEqual(DbType.Int16, command.Parameters[1].DbType);

            //
            DbHelper.AddParameter(command, "@id3", null, ParameterDirection.ReturnValue);

            Assert.AreEqual(3, command.Parameters.Count);

            Assert.AreEqual("@id3", command.Parameters[2].ParameterName);
            Assert.AreEqual(DBNull.Value, command.Parameters[2].Value);
            Assert.AreEqual(ParameterDirection.ReturnValue, command.Parameters[2].Direction);
            Assert.AreEqual(DbType.String, command.Parameters[2].DbType);
        }

        [TestMethod]
        public void AddParameterToCommand_DoNotIncludeTwice()
        {
            var factory = DbProviderFactories.GetFactory("System.Data.SqlClient");
            var command = factory.CreateCommand();

            //
            DbHelper.AddParameterToCommand(command, "@id", 10, ParameterDirection.Input);

            Assert.AreEqual(1, command.Parameters.Count);

            Assert.AreEqual("@id", command.Parameters[0].ParameterName);
            Assert.AreEqual(10, command.Parameters[0].Value);
            Assert.AreEqual(ParameterDirection.Input, command.Parameters[0].Direction);
            Assert.AreEqual(DbType.Int32, command.Parameters[0].DbType);

            //
            DbHelper.AddParameterToCommand(command, "@id", 100, ParameterDirection.Output);

            Assert.AreEqual(1, command.Parameters.Count);

            Assert.AreEqual("@id", command.Parameters[0].ParameterName);
            Assert.AreEqual(10, command.Parameters[0].Value);
            Assert.AreEqual(ParameterDirection.Input, command.Parameters[0].Direction);
            Assert.AreEqual(DbType.Int32, command.Parameters[0].DbType);

            //
            DbHelper.AddParameterToCommand(command, "@id2", null, ParameterDirection.ReturnValue);

            Assert.AreEqual(2, command.Parameters.Count);

            Assert.AreEqual("@id2", command.Parameters[1].ParameterName);
            Assert.AreEqual(DBNull.Value, command.Parameters[1].Value);
            Assert.AreEqual(ParameterDirection.ReturnValue, command.Parameters[1].Direction);
            Assert.AreEqual(DbType.String, command.Parameters[1].DbType);
        }

        [TestMethod]
        public void AddParametersToCommand()
        {
            var factory = DbProviderFactories.GetFactory("System.Data.SqlClient");
            var command = factory.CreateCommand();

            //
            var condition = Check.Op("a", 10).Or(Check.Op("a", 20)).Or(Check.Op("b", 30)).Or(Check.Op("b", 40));

            DbHelper.AddConditionParametersToCommand(command, condition, new SqlQueryService());

            Assert.AreEqual(4, command.Parameters.Count);

            Assert.AreEqual("@a", command.Parameters[0].ParameterName);
            Assert.AreEqual(10, command.Parameters[0].Value);
            Assert.AreEqual(ParameterDirection.Input, command.Parameters[0].Direction);
            Assert.AreEqual(DbType.Int32, command.Parameters[0].DbType);

            Assert.AreEqual("@a2", command.Parameters[1].ParameterName);
            Assert.AreEqual(20, command.Parameters[1].Value);
            Assert.AreEqual(ParameterDirection.Input, command.Parameters[1].Direction);
            Assert.AreEqual(DbType.Int32, command.Parameters[1].DbType);

            Assert.AreEqual("@b", command.Parameters[2].ParameterName);
            Assert.AreEqual(30, command.Parameters[2].Value);
            Assert.AreEqual(ParameterDirection.Input, command.Parameters[2].Direction);
            Assert.AreEqual(DbType.Int32, command.Parameters[2].DbType);

            Assert.AreEqual("@b2", command.Parameters[3].ParameterName);
            Assert.AreEqual(40, command.Parameters[3].Value);
            Assert.AreEqual(ParameterDirection.Input, command.Parameters[3].Direction);
            Assert.AreEqual(DbType.Int32, command.Parameters[3].DbType);
        }

        [TestMethod]
        public void GetSelectColumns()
        {
            var result = DbHelper.GetSelectColumns<User>();

            Assert.AreEqual(3, result.Count);
            Assert.AreEqual("Id", result[0]);
            Assert.AreEqual("UserName", result[1]);
            Assert.AreEqual("RoleId", result[2]);
        }

        [TestMethod]
        public void GetSelectColumns_WithMapping()
        {
            var table = new Table<User>("users")
                .SetPrimaryKeyColumn("user_id", p => p.Id)
                .SetColumn("name", p => p.UserName);

            var result = DbHelper.GetSelectColumns<User>(table);

            Assert.AreEqual(3, result.Count);
            Assert.AreEqual("user_id", result[0]);
            Assert.AreEqual("name", result[1]);
            Assert.AreEqual("RoleId", result[2]);
        }

        [TestMethod]
        public void GetSelectColumns_WithMappingAndIgnore_IgnoreColumns()
        {
            var table = new Table<User>("users")
                .SetPrimaryKeyColumn("user_id", p => p.Id)
                .SetColumn("name", p => p.UserName, false, true);

            var result = DbHelper.GetSelectColumns<User>(table);

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("user_id", result[0]);
            Assert.AreEqual("RoleId", result[1]);
        }

        [TestMethod]
        public void IsInCondition()
        {
            var condition1 = Check.Op("Id", 1);
            var condition2 = Check.Op("RoleId", 5);
            var condition3 = Check.Op("RoleId", 5).And(Check.Op("Id", 1));

            Assert.IsFalse(DbHelper.IsInCondition("Id"));
            Assert.IsFalse(DbHelper.IsInCondition("Id", null));
            Assert.IsTrue(DbHelper.IsInCondition("Id", condition1));
            Assert.IsFalse(DbHelper.IsInCondition("Id", condition2));
            Assert.IsTrue(DbHelper.IsInCondition("Id", condition3));
        }

        [TestMethod]
        public void GetInsertColumns_WithNoMapping_ReturnsAll()
        {
            var result = DbHelper.GetInsertColumnValues<User>(new User());

            Assert.AreEqual(3, result.Count);
            Assert.AreEqual("Id", result.ElementAt(0).Key);
            Assert.AreEqual("UserName", result.ElementAt(1).Key);
            Assert.AreEqual("RoleId", result.ElementAt(2).Key);
        }

        [TestMethod]
        public void GetInsertColumns_WithMapping()
        {
            var table = new Table<User>("users")
                .SetPrimaryKeyColumn("user_id", p => p.Id, false)
                .SetColumn("name", p => p.UserName);

            var result = DbHelper.GetInsertColumnValues<User>(new User(), table);

            Assert.AreEqual(3, result.Count);
            Assert.AreEqual("user_id", result.ElementAt(0).Key);
            Assert.AreEqual("name", result.ElementAt(1).Key);
            Assert.AreEqual("RoleId", result.ElementAt(2).Key);
        }

        [TestMethod]
        public void GetInsertColumns_WithMapping_Ignore()
        {
            var table = new Table<User>("users")
                .SetPrimaryKeyColumn("user_id", p => p.Id, false)
                .SetColumn("name", p => p.UserName, false, true);

            var result = DbHelper.GetInsertColumnValues<User>(new User(), table);

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("user_id", result.ElementAt(0).Key);
            Assert.AreEqual("RoleId", result.ElementAt(1).Key);
        }

        [TestMethod]
        public void GetInsertColumns_WithMapping_DoNotIncludeDatabaseGeneratedColumns()
        {
            var table = new Table<User>("users")
                .SetPrimaryKeyColumn("user_id", p => p.Id)
                .SetColumn("name", p => p.UserName);

            var result = DbHelper.GetInsertColumnValues<User>(new User(), table);

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("name", result.ElementAt(0).Key);
            Assert.AreEqual("RoleId", result.ElementAt(1).Key);
        }

        [TestMethod]
        public void GetUpdateColumns_WithNoMappingAndNoCondition_ReturnsAll()
        {
            var result = DbHelper.GetUpdateColumnValues<User>(new User());

            Assert.AreEqual(3, result.Count);
            Assert.AreEqual("Id", result.ElementAt(0).Key);
            Assert.AreEqual("UserName", result.ElementAt(1).Key);
            Assert.AreEqual("RoleId", result.ElementAt(2).Key);
        }

        [TestMethod]
        public void GetUpdateColumns_WithNoMappingAndCondition_DoNotReturnColumnsInCondition()
        {
            var condition1 = Check.Op("Id", 1);

            var result = DbHelper.GetUpdateColumnValues<User>(new User(), null, condition1);

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("UserName", result.ElementAt(0).Key);
            Assert.AreEqual("RoleId", result.ElementAt(1).Key);
        }

        [TestMethod]
        public void GetUpdateColumns_WithNoMappingAndCondition_DoNotReturnColumnsInCondition2()
        {
            var condition3 = Check.Op("RoleId", 5).And(Check.Op("Id", 1));

            var result = DbHelper.GetUpdateColumnValues<User>(new User(), null, condition3);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("UserName", result.ElementAt(0).Key);
        }

        [TestMethod]
        public void GetUpdateColumns_WithMappingAndNoCondition_ReturnsAll()
        {
            var table = new Table<User>("users")
              .SetPrimaryKeyColumn("user_id", p => p.Id, false)
              .SetColumn("name", p => p.UserName);

            var result = DbHelper.GetUpdateColumnValues<User>(new User(), table);

            Assert.AreEqual(3, result.Count);
            Assert.AreEqual("user_id", result.ElementAt(0).Key);
            Assert.AreEqual("name", result.ElementAt(1).Key);
            Assert.AreEqual("RoleId", result.ElementAt(2).Key);
        }

        [TestMethod]
        public void GetUpdateColumns_WithMappingAndNoCondition_Ignore()
        {
            var table = new Table<User>("users")
              .SetPrimaryKeyColumn("user_id", p => p.Id, false)
              .SetColumn("name", p => p.UserName, false, true);

            var result = DbHelper.GetUpdateColumnValues<User>(new User(), table);

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("user_id", result.ElementAt(0).Key);
            Assert.AreEqual("RoleId", result.ElementAt(1).Key);
        }

        [TestMethod]
        public void GetUpdateColumns_WithMappingAndNoCondition_IgnoreDatabaseGeneratedColumns()
        {
            var table = new Table<User>("users")
              .SetPrimaryKeyColumn("user_id", p => p.Id)
              .SetColumn("name", p => p.UserName)
              .SetColumn("RoleId", p => p.RoleId, true);

            var result = DbHelper.GetUpdateColumnValues<User>(new User(), table);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("name", result.ElementAt(0).Key);
        }

        [TestMethod]
        public void AddConditionParametersToCommand()
        {
            var factory = DbProviderFactories.GetFactory("System.Data.SqlClient");
            var command = factory.CreateCommand();

            DbHelper.AddConditionParametersToCommand(command, Check.Op("Id", 1).And(Check.Op("RoleId", 5)), new SqlQueryService());

            Assert.AreEqual(2, command.Parameters.Count);

            Assert.AreEqual("@id", command.Parameters[0].ParameterName);
            Assert.AreEqual(1, command.Parameters[0].Value);
            Assert.AreEqual(ParameterDirection.Input, command.Parameters[0].Direction);
            Assert.AreEqual(DbType.Int32, command.Parameters[0].DbType);

            Assert.AreEqual("@roleid", command.Parameters[1].ParameterName);
            Assert.AreEqual(5, command.Parameters[1].Value);
            Assert.AreEqual(ParameterDirection.Input, command.Parameters[1].Direction);
            Assert.AreEqual(DbType.Int32, command.Parameters[1].DbType);
        }

        [TestMethod]
        public void CreateEmptyTable()
        {
            var service = new SqlQueryService();
            var result = DbHelper.TryCreateEmptyTable<User>(service.DefaultPrimaryKeyName);

            Assert.AreEqual("User", result.TableName);
            Assert.AreEqual(typeof(User), result.ModelType);

            Assert.AreEqual(1, result.MappingByColumnName.Count);
            Assert.AreEqual(1, result.PrimaryKeys.Count());

            Assert.AreEqual("Id", result.PrimaryKeys[0].ColumnName);
            Assert.AreEqual("User", result.PrimaryKeys[0].TableName);
            Assert.AreEqual(typeof(User), result.PrimaryKeys[0].ModelType);
            Assert.AreEqual("Id", result.PrimaryKeys[0].PropertyName);
            Assert.AreEqual(true, result.PrimaryKeys[0].IsDatabaseGenerated);
            Assert.AreEqual(false, result.PrimaryKeys[0].IsIgnored);
        }

        [TestMethod]
        public void CreateEmptyTable_WithMySql()
        {
            var service = new MySqlQueryService();
            var result = DbHelper.TryCreateEmptyTable<User>(service.DefaultPrimaryKeyName);

            Assert.AreEqual("User", result.TableName);
            Assert.AreEqual(typeof(User), result.ModelType);

            Assert.AreEqual(1, result.MappingByColumnName.Count);
            Assert.AreEqual(1, result.PrimaryKeys.Count());

            Assert.AreEqual("id", result.PrimaryKeys[0].ColumnName); // <=

            Assert.AreEqual("User", result.PrimaryKeys[0].TableName);
            Assert.AreEqual(typeof(User), result.PrimaryKeys[0].ModelType);
            Assert.AreEqual("Id", result.PrimaryKeys[0].PropertyName);
            Assert.AreEqual(true, result.PrimaryKeys[0].IsDatabaseGenerated);
            Assert.AreEqual(false, result.PrimaryKeys[0].IsIgnored);
        }

        [TestMethod]
        public void CreateEmptyTable_WithoutFoundKey_ReturnNull()
        {
            var result = DbHelper.TryCreateEmptyTable<UserWithUserId>("Id");

            Assert.IsNull(result);
        }


        [TestMethod]
        public void DiscoverTable()
        {
            var result = DbHelper.DiscoverMappingFor<UserWithAttributes>();

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


        [TestMethod]
        public void IsValueType()
        {
            // https://docs.microsoft.com/en-us/dotnet/framework/data/adonet/sql-server-data-type-mappings

            //bool
            //byte
            //char
            //decimal
            //double
            // enum
            //float
            //int
            //long
            //sbyte
            //short
            //struct
            //uint
            //ulong
            //ushort

            Assert.IsTrue(typeof(bool).IsValueType);
            Assert.IsTrue(typeof(byte).IsValueType);
            Assert.IsTrue(typeof(char).IsValueType);
            Assert.IsTrue(typeof(decimal).IsValueType);
            Assert.IsTrue(typeof(double).IsValueType);
            //Assert.IsTrue(typeof(enum).IsValueType);
            Assert.IsTrue(typeof(float).IsValueType);
            Assert.IsTrue(typeof(int).IsValueType);
            Assert.IsTrue(typeof(long).IsValueType);
            Assert.IsTrue(typeof(sbyte).IsValueType);
            Assert.IsTrue(typeof(short).IsValueType);
            //Assert.IsTrue(typeof(struct).IsValueType);
            Assert.IsTrue(typeof(uint).IsValueType);
            Assert.IsTrue(typeof(ulong).IsValueType);
            Assert.IsTrue(typeof(ushort).IsValueType);

            Assert.IsTrue(typeof(Int32).IsValueType);
            //Assert.IsTrue(typeof(String).IsValueType); // false
            Assert.IsTrue(typeof(Boolean).IsValueType);
            Assert.IsTrue(typeof(Double).IsValueType);
            Assert.IsTrue(typeof(Int16).IsValueType);
            Assert.IsTrue(typeof(Int64).IsValueType);
            Assert.IsTrue(typeof(Decimal).IsValueType);
            Assert.IsTrue(typeof(Single).IsValueType);
            Assert.IsTrue(typeof(TimeSpan).IsValueType);
            Assert.IsTrue(typeof(Byte).IsValueType);
            //Assert.IsTrue(typeof(Byte[]).IsValueType); // false
            Assert.IsTrue(typeof(Guid).IsValueType);
            //Assert.IsTrue(typeof(Char[]).IsValueType); // false
            Assert.IsTrue(typeof(DateTime).IsValueType);
            Assert.IsTrue(typeof(DateTimeOffset).IsValueType);
            Assert.IsTrue(typeof(SByte).IsValueType);
            Assert.IsTrue(typeof(UInt16).IsValueType);
            Assert.IsTrue(typeof(UInt32).IsValueType);
            Assert.IsTrue(typeof(UInt64).IsValueType);

            // nullable
            Assert.IsTrue(typeof(UInt64?).IsValueType);
        }


        [TestMethod]
        public void IsAllowedType()
        {
            Assert.IsTrue(DbHelper.IsAllowedType(typeof(bool)));
            Assert.IsTrue(DbHelper.IsAllowedType(typeof(byte)));
            Assert.IsTrue(DbHelper.IsAllowedType(typeof(char)));
            Assert.IsTrue(DbHelper.IsAllowedType(typeof(decimal)));
            Assert.IsTrue(DbHelper.IsAllowedType(typeof(double)));
            Assert.IsTrue(DbHelper.IsAllowedType(typeof(float)));
            Assert.IsTrue(DbHelper.IsAllowedType(typeof(int)));
            Assert.IsTrue(DbHelper.IsAllowedType(typeof(long)));
            Assert.IsTrue(DbHelper.IsAllowedType(typeof(sbyte)));
            Assert.IsTrue(DbHelper.IsAllowedType(typeof(short)));
            Assert.IsTrue(DbHelper.IsAllowedType(typeof(uint)));
            Assert.IsTrue(DbHelper.IsAllowedType(typeof(ulong)));
            Assert.IsTrue(DbHelper.IsAllowedType(typeof(ushort)));

            Assert.IsTrue(DbHelper.IsAllowedType(typeof(Int32)));
            Assert.IsTrue(DbHelper.IsAllowedType(typeof(String))); 
            Assert.IsTrue(DbHelper.IsAllowedType(typeof(Boolean)));
            Assert.IsTrue(DbHelper.IsAllowedType(typeof(Double)));
            Assert.IsTrue(DbHelper.IsAllowedType(typeof(Int16)));
            Assert.IsTrue(DbHelper.IsAllowedType(typeof(Int64)));
            Assert.IsTrue(DbHelper.IsAllowedType(typeof(Decimal)));
            Assert.IsTrue(DbHelper.IsAllowedType(typeof(Single)));
            Assert.IsTrue(DbHelper.IsAllowedType(typeof(TimeSpan)));
            Assert.IsTrue(DbHelper.IsAllowedType(typeof(Byte)));
            Assert.IsTrue(DbHelper.IsAllowedType(typeof(Byte[])));
            Assert.IsTrue(DbHelper.IsAllowedType(typeof(Guid)));
            Assert.IsTrue(DbHelper.IsAllowedType(typeof(Char[])));
            Assert.IsTrue(DbHelper.IsAllowedType(typeof(DateTime)));
            Assert.IsTrue(DbHelper.IsAllowedType(typeof(DateTimeOffset)));
            Assert.IsTrue(DbHelper.IsAllowedType(typeof(SByte)));
            Assert.IsTrue(DbHelper.IsAllowedType(typeof(UInt16)));
            Assert.IsTrue(DbHelper.IsAllowedType(typeof(UInt32)));
            Assert.IsTrue(DbHelper.IsAllowedType(typeof(UInt64)));

            // nullable
            Assert.IsTrue(DbHelper.IsAllowedType(typeof(UInt64?)));

            Assert.IsFalse(DbHelper.IsAllowedType(typeof(User)));

        }
    }

}
