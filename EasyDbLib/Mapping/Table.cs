using System;
using System.Collections.Generic;
using System.Data;

namespace EasyDbLib
{

    public class Table
    {
        internal Dictionary<string, Column> columns;

        public string TableName { get; }
        public bool HasNoColumnOrOnlyKeys { get; protected set; }

        public Table(string tableName)
        {
            this.TableName = tableName;
            this.HasNoColumnOrOnlyKeys = true;
            this.columns = new Dictionary<string, Column>();
        }

        protected void CheckPropertyName(string propertyName)
        {
            if (!NameChecker.CheckPropertyName(propertyName))
            {
                throw new Exception("Invalid property name " + propertyName);
            }
        }

        public Table SetColumn(string columnName, string propertyName, DbType dbType, bool ignore = false)
        {
            this.CheckPropertyName(propertyName);
            this.columns[columnName] = new Column(columnName, propertyName, dbType, ignore);
            this.HasNoColumnOrOnlyKeys = false;
            return this;
        }

        public Table SetColumn(string columnName, string propertyName, bool ignore = false)
        {
            this.CheckPropertyName(propertyName);
            this.columns[columnName] = new Column(columnName, propertyName, null, ignore);
            this.HasNoColumnOrOnlyKeys = false;
            return this;
        }

        public Table SetPrimaryKeyColumn(string columnName, string propertyName, DbType dbType, bool ignore= false)
        {
            CheckPropertyName(propertyName);
            this.columns[columnName] = new PrimaryKeyColumn(columnName, propertyName, dbType, ignore);
            return this;
        }

        public Table SetPrimaryKeyColumn(string columnName, string propertyName,  bool ignore = false)
        {
            this.CheckPropertyName(propertyName);
            this.columns[columnName] = new PrimaryKeyColumn(columnName, propertyName, null, ignore);
            return this;
        }

        public Table SetForeignKeyColumn(string columnName, string propertyName, string tableReferenced, string primaryKeyReferenced, DbType dbType, bool ignore = false)
        {
            this.CheckPropertyName(propertyName);
            this.columns[columnName] = new ForeignKeyColumn(columnName, tableReferenced, primaryKeyReferenced, propertyName, dbType, ignore);
            return this;
        }

        public Table SetForeignKeyColumn(string columnName, string propertyName, string tableReferenced, string primaryKeyReferenced, bool ignore = false)
        {
            this.CheckPropertyName(propertyName);
            this.columns[columnName] = new ForeignKeyColumn(columnName, tableReferenced, primaryKeyReferenced, propertyName, null, ignore);
            return this;
        }

        public bool HasColumn(string columnName)
        {
            return this.columns.ContainsKey(columnName);
        }

        public Column GetColumn(string columnName)
        {
            if (!this.HasColumn(columnName)) { throw new Exception("No column registered for " + columnName + " in the mapping of " + this.TableName); }

            return this.columns[columnName];
        }

        public PrimaryKeyColumn[] GetPrimaryKeys()
        {
            var result = new List<PrimaryKeyColumn>();
            foreach (var column in this.columns)
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
            foreach (var column in this.columns)
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
