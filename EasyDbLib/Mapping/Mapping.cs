using System;
using System.Collections.Generic;

namespace EasyDbLib
{
    public class Mapping
    {
        protected static Dictionary<string, Table> container;

        static Mapping()
        {
            container = new Dictionary<string, Table>();
        }

        public static Table AddTable(string tableName)
        {
            var result = new Table(tableName);
            container[tableName] = result;
            return result;
        }

        public static bool HasTable(string tableName)
        {
            return container.ContainsKey(tableName);
        }

        public static Table GetTable(string tableName)
        {
            if (!HasTable(tableName)) { throw new Exception("No table registered for " + tableName); }

            return container[tableName];
        }
    }
}
