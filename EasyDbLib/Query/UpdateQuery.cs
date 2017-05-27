using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyDbLib
{

    public class UpdateQuery
    {
        protected IQueryService queryService;
        protected EasyDb easyDbInstance;
        protected string table;
        protected Dictionary<string, object> columnValues;
        protected ConditionAndParameterContainer condition;
        protected bool hasSet;
        protected bool hasCondition;

        public UpdateQuery(IQueryService queryService, EasyDb easyDbInstance, string table)
        {
            this.columnValues = new Dictionary<string, object>();
            this.queryService = queryService;
            this.easyDbInstance = easyDbInstance;
            this.table = table;
        }

        public string[] GetColumns()
        {
            return this.columnValues.Keys.ToArray();
        }

        public UpdateQuery Set(string column, object value)
        {
            this.columnValues[column] = value;
            this.hasSet = true;
            return this;
        }

        public UpdateQuery Where(Check condition)
        {
            if (this.hasCondition) { throw new Exception("One clause where"); }
            this.condition = new ConditionAndParameterContainer(condition);
            this.hasCondition = true;
            return this;
        }

        public string GetQuery()
        {
            if (!this.hasSet) { throw new Exception("No column and values provided"); }

            return this.queryService.GetUpdate(this.table, this.GetColumns(), this.condition);
        }

        public EasyDbCommand CreateCommand()
        {
            var query = this.GetQuery();

            var command = this.easyDbInstance.CreateCommand(query);

            var added = new List<string>();

            foreach (var columnValue in this.columnValues)
            {
                var parameterName = this.queryService.GetParameterName(columnValue.Key);
                if (!added.Contains(parameterName))
                {
                    command.AddParameter(parameterName, columnValue.Value);
                    added.Add(parameterName);
                }
            }

            if (this.hasCondition)
            {
                if (this.condition.Main.IsConditionOp && !added.Contains(this.condition.Main.ParameterName))
                {
                    command.AddParameter(this.condition.Main.ParameterName, this.condition.Main.ParameterValue);
                    added.Add(this.condition.Main.ParameterName);
                }

                if (this.condition.HasConditions())
                {
                    foreach (var subCondittion in this.condition.SubConditions)
                    {
                        if (subCondittion.IsConditionOp && !added.Contains(this.condition.Main.ParameterName))
                        {
                            command.AddParameter(subCondittion.ParameterName, subCondittion.ParameterValue);
                            added.Add(subCondittion.ParameterName);
                        }
                    }
                }
            }

            return command;
        }

        public async Task<int> NonQueryAsync()
        {
            var command = this.CreateCommand();
            return await command.NonQueryAsync();
        }
    }
}
