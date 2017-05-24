using System;
using System.Collections.Generic;
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

        public UpdateQuery Set(string column, object value)
        {
            this.columnValues[column] = value;
            this.hasSet = true;
            return this;
        }


        public UpdateQuery Where(Condition condition)
        {
            if (this.hasCondition) { throw new Exception("One clause where"); }
            this.condition = new ConditionAndParameterContainer(condition);
            this.hasCondition = true;
            return this;
        }

        public string GetQuery()
        {
            if (!this.hasSet) { throw new Exception("No column and values provided"); }

            return this.queryService.GetUpdate(this.table, this.columnValues, this.condition);
        }

        protected EasyDbCommand CreateCommand()
        {
            var query = this.GetQuery();

            var command = this.easyDbInstance.CreateCommand(query);

            if (this.hasCondition)
            {
                command.AddParameter(this.condition.Main.ParameterName, this.condition.Main.ParameterValue);

                if (this.condition.HasConditions())
                {
                    foreach (var subCondittion in this.condition.SubConditions)
                    {
                        if (subCondittion.IsConditionOp)
                        {
                            command.AddParameter(subCondittion.ParameterName, subCondittion.ParameterValue);
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
