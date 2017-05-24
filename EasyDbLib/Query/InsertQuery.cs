using System;
using System.Threading.Tasks;

namespace EasyDbLib
{
    public class InsertQuery
    {
        protected IQueryService queryService;
        protected EasyDb easyDbInstance;
        protected string table;
        protected string[] columns;
        protected object[] values;
        protected bool hasColumns;
        protected bool hasValues;

        public InsertQuery(IQueryService queryService, EasyDb easyDbInstance, string table)
        {
            this.queryService = queryService;
            this.easyDbInstance = easyDbInstance;
            this.table = table;
        }

        public InsertQuery Columns(params string[] columns)
        {
            if (this.hasColumns) { throw new Exception("Columns already set"); }
            this.columns = columns;
            this.hasColumns = true;
            return this;
        }

        public InsertQuery Values(params object[] values)
        {
            if (this.hasValues) { throw new Exception("Values already set"); }
            this.values = values;
            this.hasValues = true;
            return this;
        }

        public string GetQuery(bool lastInsertedId = true)
        {
            if (!this.hasColumns) { throw new Exception("No columns provided"); }
            return this.queryService.GetInsertInto(this.table, this.columns, lastInsertedId);
        }


        protected EasyDbCommand CreateCommand()
        {
            var query = this.GetQuery();

            if (!this.hasValues) { throw new Exception("No values provided"); }
            if (this.columns.Length != this.values.Length) { throw new Exception("Not same number of columns and values"); }
            var command = this.easyDbInstance.CreateCommand(query);

            for (int i = 0; i < this.columns.Length; i++)
            {
                var parameterName = this.queryService.GetParameterName(this.columns[i]);
                command.AddParameter(parameterName, this.values[i]);
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
