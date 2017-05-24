using System;
using System.Data;

namespace EasyDbLib
{
    public class Column
    {
        public string ColumnName { get; }
        public string PropertyName { get; }
        public bool Ignore { get; set; }
        public DbType? DbType { get; }

        public Column(string columnName, string propertyName, DbType? dbType, bool ignore)
        {
            this.ColumnName = columnName;
            this.PropertyName = propertyName;
            this.Ignore = ignore;
            this.DbType = dbType;
        }

    }
}
