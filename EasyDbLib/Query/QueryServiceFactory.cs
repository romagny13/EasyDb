using System;
using System.Collections.Generic;

namespace EasyDbLib
{
    public class QueryServiceFactory
    {
        protected static Dictionary<string, IQueryService> factories = new Dictionary<string, IQueryService>();

        static QueryServiceFactory()
        {
            RegisterDefaults();
        }

        protected static void RegisterDefaults()
        {
            Set("System.Data.SqlClient", new QueryService());
            Set("System.Data.OleDb", new QueryService());
        }

        public static void Set(string providerName, IQueryService queryService)
        {
            factories[providerName] = queryService;
        }

        public static bool Has(string providerName)
        {
            return factories.ContainsKey(providerName);
        }

        public static IQueryService Get(string providerName)
        {
            if (Has(providerName))
            {
                return factories[providerName];
            }
            else
            {
                throw new Exception("No query service found for " + providerName);
            }
        }
    }
}