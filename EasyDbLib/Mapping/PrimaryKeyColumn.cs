using System;
using System.Reflection;

namespace EasyDbLib
{
    public class PrimaryKeyColumn : Column
    {
        public PrimaryKeyColumn(
            string tableName,
            Type modelType,
            string columnName,
            PropertyInfo property,
            bool isDatabaseGenerated,
            bool ignore)
            : base(tableName, modelType, columnName, property, isDatabaseGenerated, ignore)
        { }

    }
}
