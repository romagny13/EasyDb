using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyDbLib
{
    public class InsertQuery
    {
        protected IQueryService queryService;
        protected EasyDb easyDbInstance;
        protected string table;
        protected Dictionary<string, object> columnValues;
        protected bool hasValues;

        public InsertQuery(IQueryService queryService, EasyDb easyDbInstance, string table)
        {
            this.columnValues = new Dictionary<string, object>();
            this.queryService = queryService;
            this.easyDbInstance = easyDbInstance;
            this.table = table;
        }

        public InsertQuery Values(string column, object value)
        {
            this.columnValues[column] = value;
            this.hasValues = true;
            return this;
        }

        public string[] GetColumns()
        {
           return this.columnValues.Keys.ToArray();
        }

        public string GetQuery(bool lastInsertedId = true)
        {
            if (!this.hasValues) { throw new Exception("No columns provided"); }

            return this.queryService.GetInsertInto(this.table, this.GetColumns(), lastInsertedId);
        }


        public EasyDbCommand CreateCommand()
        {
            var query = this.GetQuery();

            var command = this.easyDbInstance.CreateCommand(query);

            foreach (var columnValue in this.columnValues)
            {
                var parameterName = this.queryService.GetParameterName(columnValue.Key);
                command.AddParameter(parameterName, columnValue.Value);
            }

            return command;
        }

        public async Task<T> LastInsertedId<T>()
        {
            var command = this.CreateCommand();
            return (T)await command.ScalarAsync();
        }

        public async Task<int> NonQueryAsync()
        {
            var command = this.CreateCommand();
            return await command.NonQueryAsync();
        }

        public async Task<TModel> Fetch<TModel>(Table mapping) where TModel : new()
        {
            var command = this.CreateCommand();
            var lastInserted = await command.ScalarAsync();


            var primaryKeys = mapping.GetPrimaryKeys();
            if (primaryKeys.Length == 0) { throw new Exception("No primary keys provided in the mapping"); }
            if (primaryKeys.Length > 1) { throw new Exception("Multiple primary keys not supported for this method"); }

            return await this.easyDbInstance
                  .Select<TModel>(mapping)
                  .Where(Condition.Op(primaryKeys[0].ColumnName, lastInserted))
                  .ReadOneAsync();
        }
    }
}
