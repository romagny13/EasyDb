using System;
using System.Threading.Tasks;

namespace EasyDbLib
{
    public class DeleteQuery
    {
        protected IQueryBuilderService queryBuilderService;

        protected string table;
        protected Condition condition;
        protected bool hasWhere;
        protected EasyDb easyDbInstance;

        public DeleteQuery(string table, EasyDb easyDbInstance)
            :this(new QueryBuilderService(), table, easyDbInstance)
        { }

        public DeleteQuery(IQueryBuilderService queryBuilderService, string table, EasyDb easyDbInstance)
        {
            this.queryBuilderService = queryBuilderService;
            this.table = table;
            this.easyDbInstance = easyDbInstance;
        }

        public DeleteQuery Where(Condition condition)
        {
            if (this.hasWhere) { throw new Exception("One where clause"); }

            this.condition = condition;
            this.hasWhere = true;
            return this;
        }

        public string Build()
        {
            return this.queryBuilderService.GetDeleteFromString(this.table, this.condition);
        }

        public async Task<int> NonQueryAsync()
        {
            var sql = this.Build();
            return await this.easyDbInstance.CreateCommand(sql)
                .NonQueryAsync();
        }
    }
}
