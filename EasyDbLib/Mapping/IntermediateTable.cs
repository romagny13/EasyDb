using System;
using System.Collections.Generic;

namespace EasyDbLib
{
    public class IntermediateTable
    {
        internal Dictionary<string, IntermediatePrimaryKeyColumn> columns;

        public string TableName { get; }

        public IntermediateTable(string tableName)
        {
            this.columns = new Dictionary<string, IntermediatePrimaryKeyColumn>();

            TableName = tableName;
        }

        protected void CheckRegistered(string columnName)
        {
            if (columns.ContainsKey(columnName))
            {
                throw new Exception("Column " + columnName + " already resgitered");
            }
        }

        public IntermediateTable SetPrimaryKeyColumn(string columnName, string targetTablePrimaryKey, string targetTableName)
        {
            this.CheckRegistered(columnName);
            this.columns[columnName] = new IntermediatePrimaryKeyColumn(columnName, this.TableName, targetTablePrimaryKey, targetTableName);
            return this;
        }

        public bool HasColumn(string columnName)
        {
            return this.columns.ContainsKey(columnName);
        }

        public IntermediatePrimaryKeyColumn GetColumn(string columnName)
        {
            if (!this.HasColumn(columnName)) { throw new Exception("No column registered for " + columnName + " in the mapping of the intermediate table " + this.TableName); }

            return this.columns[columnName];
        }

        public IntermediatePrimaryKeyColumn[] GetPrimaryKeys(string targetTableName)
        {
            var result = new List<IntermediatePrimaryKeyColumn>();
            foreach (var column in this.columns)
            {
                if (column.Value.TargetTableName == targetTableName)
                {
                    result.Add(column.Value);
                }
            }
            return result.ToArray();
        }
    }

}
