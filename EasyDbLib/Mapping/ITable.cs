using System;
using System.Collections.Generic;

namespace EasyDbLib
{
    public interface ITable
    {
        bool IgnoreCase { get; set; }
        IReadOnlyDictionary<string, Column> MappingByColumnName { get; }
        Type ModelType { get; }
        PrimaryKeyColumn[] PrimaryKeys { get; }
        string TableName { get; }

        bool ContainsProperty(string propertyName);
        Column GetColumnByPropertyName(string propertyName);
    }
}