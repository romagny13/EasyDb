using System.Collections.Generic;

namespace EasyDbLib
{
    public class Mapping
    {
        protected Dictionary<string, ColumnMapping> columnMappings;

        public Mapping()
        {
            this.columnMappings = new Dictionary<string, ColumnMapping>();
        }

        public static Mapping Create()
        {
            return new Mapping();
        }

        public Mapping Add(string columnName, string propertyName, bool ignore = false)
        {
            this.columnMappings[columnName] = new ColumnMapping(columnName,propertyName, ignore);
            return this;
        }

        public bool Has(string columnName)
        {
            return this.columnMappings.ContainsKey(columnName);
        }

        public ColumnMapping Get(string columnName)
        {
            return this.columnMappings[columnName];
        }
    }
}