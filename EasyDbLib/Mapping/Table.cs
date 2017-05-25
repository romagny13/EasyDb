using System;
using System.Collections.Generic;
using System.Data;

namespace EasyDbLib
{

    public class Table
    {
        internal Dictionary<string,Column> Columns { get; }

        public string TableName { get; }
        public bool HasNoColumnOrOnlyKeys { get; protected set; }

        public Table(string tableName)
        {
            this.TableName = tableName;
            this.HasNoColumnOrOnlyKeys = true;
            this.Columns = new Dictionary<string, Column>();
        }

        private static void CheckPropertyName(string propertyName)
        {
            if (!NameChecker.CheckPropertyName(propertyName))
            {
                throw new Exception("Invalid property name " + propertyName);
            }
        }

        public Table AddColumn(string columnName, string propertyName, DbType dbType, bool ignore = false)
        {
            CheckPropertyName(propertyName);
            this.Columns[columnName] = new Column(columnName, propertyName, dbType, ignore);
            this.HasNoColumnOrOnlyKeys = false;
            return this;
        }

        public Table AddColumn(string columnName, string propertyName, bool ignore = false)
        {
            CheckPropertyName(propertyName);
            this.Columns[columnName] = new Column(columnName, propertyName, null, ignore);
            this.HasNoColumnOrOnlyKeys = false;
            return this;
        }

        public Table AddPrimaryKeyColumn(string columnName, string propertyName, DbType dbType, bool ignore= false)
        {
            CheckPropertyName(propertyName);
            this.Columns[columnName] = new PrimaryKeyColumn(columnName, propertyName, dbType, ignore);
            return this;
        }

        public Table AddPrimaryKeyColumn(string columnName, string propertyName,  bool ignore = false)
        {
            CheckPropertyName(propertyName);
            this.Columns[columnName] = new PrimaryKeyColumn(columnName, propertyName, null, ignore);
            return this;
        }

        public Table AddForeignKeyColumn(string columnName, string propertyName, string tableReferenced, string primaryKeyReferenced, DbType dbType, bool ignore = false)
        {
            CheckPropertyName(propertyName);
            this.Columns[columnName] = new ForeignKeyColumn(columnName, tableReferenced, primaryKeyReferenced, propertyName, dbType, ignore);
            return this;
        }

        public Table AddForeignKeyColumn(string columnName, string propertyName, string tableReferenced, string primaryKeyReferenced, bool ignore = false)
        {
            CheckPropertyName(propertyName);
            this.Columns[columnName] = new ForeignKeyColumn(columnName, tableReferenced, primaryKeyReferenced, propertyName, null, ignore);
            return this;
        }

        public bool HasColumn(string columnName)
        {
            return this.Columns.ContainsKey(columnName);
        }

        public Column GetColumn(string columnName)
        {
            if (!this.HasColumn(columnName)) { throw new Exception("No column registered for " + columnName + " in the mapping of " + this.TableName); }

            return this.Columns[columnName];
        }

        public PrimaryKeyColumn[] GetPrimaryKeys()
        {
            var result = new List<PrimaryKeyColumn>();
            foreach (var column in this.Columns)
            {
                if (column.Value.GetType() == typeof(PrimaryKeyColumn))
                {
                    result.Add((PrimaryKeyColumn)column.Value);
                }
            }
            return result.ToArray();
        }

        public ForeignKeyColumn[] GetForeignKeys(string tableReferenced)
        {
            var result = new List<ForeignKeyColumn>();
            foreach (var column in this.Columns)
            {
                if (column.Value.GetType() == typeof(ForeignKeyColumn) && ((ForeignKeyColumn) column.Value).TableReferenced == tableReferenced)
                {
                    result.Add((ForeignKeyColumn)column.Value);
                }
            }
            return result.ToArray();
        }
    }
}
