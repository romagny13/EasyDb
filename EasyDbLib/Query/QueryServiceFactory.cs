using System;
using System.Collections.Generic;

namespace EasyDbLib
{
    public class QueryServiceFactory
    {
        static Dictionary<string, IQueryService> factories = new Dictionary<string, IQueryService>();

        static string providerName { get; set; }

        public static IQueryService Current { get; private set; }

        static QueryServiceFactory()
        {
            Reset();
        }

        public static void Reset()
        {
            RegisterQueryService("System.Data.SqlClient", new SqlQueryService());
            RegisterQueryService("System.Data.OleDb", new OleDbQueryService());
            RegisterQueryService("MySql.Data.MySqlClient", new MySqlQueryService());

            Current = providerName != null && factories.ContainsKey(providerName) ?
                factories[providerName]
                : null;
        }

        public static void RegisterQueryService(string providerName, IQueryService queryService)
        {
            Guard.IsNullOrEmpty(providerName);

            factories[providerName] = queryService ?? throw new ArgumentNullException(nameof(queryService));
        }

        public static bool HasServiceFor(string providerName)
        {
            return factories.ContainsKey(providerName);
        }

        public static IQueryService GetQueryService(string providerName)
        {
            Guard.IsNullOrEmpty(providerName);

            if (HasServiceFor(providerName))
            {
                QueryServiceFactory.providerName = providerName;
                Current = factories[providerName];
                return Current;
            }
            else
            {
                throw new Exception("No query service registered for " + providerName);
            }
        }
    }
}