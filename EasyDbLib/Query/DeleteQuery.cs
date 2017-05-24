using System;
using System.Threading.Tasks;

namespace EasyDbLib
{
    public class DeleteQuery
    {
        protected IQueryService queryService;
        protected EasyDb easyDbInstance;
        protected string tableName;
        protected ConditionAndParameterContainer condition;
        protected bool hasCondition;

        public DeleteQuery(IQueryService queryService, EasyDb easyDbInstance, string tableName)
        {
            this.queryService = queryService;
            this.easyDbInstance = easyDbInstance;
            this.tableName = tableName;
        }

        public DeleteQuery Where(Condition condition)
        {
            if (this.hasCondition) { throw new Exception("One clause where"); }
            this.condition = new ConditionAndParameterContainer(condition);
            this.hasCondition = true;
            return this;
        }

        public string GetQuery()
        {
            return this.queryService.GetDelete(this.tableName, this.condition);
        }

        public async Task<int> NonQueryAsync()
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

          return await command.NonQueryAsync();
        }

    }
}
