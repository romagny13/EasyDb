using System;
using System.Data;

namespace EasyDbLib
{
    public class PrimaryKeyColumn : Column
    {
        public PrimaryKeyColumn(string columnName, string propertyName, DbType? dbType, bool ignore)
            : base(columnName, propertyName, dbType, ignore)
        { }
    }
}
