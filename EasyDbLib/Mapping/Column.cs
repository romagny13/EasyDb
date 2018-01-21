using System;
using System.Reflection;

namespace EasyDbLib
{

    public class Column
    {
        public string TableName { get; }

        public Type ModelType { get; }

        public string ColumnName { get; }

        public bool IsDatabaseGenerated { get; }

        public PropertyInfo Property { get; }

        public string PropertyName => Property.Name;

        public bool IsIgnored { get; set; }

        public Column(
            string tableName,
            Type modelType,
            string columnName,
            PropertyInfo property,
            bool isDatabaseGenerated,
            bool isIgnored)
        {
            this.TableName = tableName;
            this.ModelType = modelType;
            this.ColumnName = columnName;
            this.Property = property;
            this.IsDatabaseGenerated = isDatabaseGenerated;
            this.IsIgnored = isIgnored;
        }
    }
}
