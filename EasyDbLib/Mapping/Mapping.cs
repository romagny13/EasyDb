using System;
using System.Collections.Generic;

namespace EasyDbLib
{
    public class Mapping
    {
        protected static Dictionary<string, Table> tables;
        protected static Dictionary<string, IntermediateTable> intermediateTables;

        static Mapping()
        {
            tables = new Dictionary<string, Table>();
            intermediateTables = new Dictionary<string, IntermediateTable>();
        }

        public static Table SetTable(string tableName)
        {
            var result = new Table(tableName);
            tables[tableName] = result;
            return result;
        }

        public static bool HasTable(string tableName)
        {
            return tables.ContainsKey(tableName);
        }

        public static Table GetTable(string tableName)
        {
            if (!HasTable(tableName)) { throw new Exception("No table registered for " + tableName); }

            return tables[tableName];
        }

        public static bool RemoveTable(string tableName)
        {
            if (HasTable(tableName))
            {
                tables.Remove(tableName);
                return true;
            }
            return false;
        }

        public static void Clear()
        {
            tables.Clear();
            intermediateTables.Clear();
        }

        // intermediate table

        public static IntermediateTable SetIntermediateTable(string tableName)
        {
            var result = new IntermediateTable(tableName);
            intermediateTables[tableName] = result;
            return result;
        }

        public static bool HasIntermediateTable(string tableName)
        {
            return intermediateTables.ContainsKey(tableName);
        }

        public static IntermediateTable GetIntermediateTable(string tableName)
        {
            if (!HasIntermediateTable(tableName)) { throw new Exception("No intermediate table registered for " + tableName); }

            return intermediateTables[tableName];
        }

        public static bool RemoveIntermediateTable(string tableName)
        {
            if (HasIntermediateTable(tableName))
            {
                intermediateTables.Remove(tableName);
                return true;
            }
            return false;
        }
    }

}
