using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyDbLib
{

    public class UpdateQuery
    {
        protected IQueryBuilderService queryBuilderService;

        protected string table;
        protected string[] columns;
        protected Condition condition;
        protected Dictionary<string, object> columnValues;

        protected bool hasSet = false;
        protected bool hasWhere = false;
        protected EasyDb easyDbInstance;

        public UpdateQuery(string table, EasyDb easyDbInstance)
            :this(new QueryBuilderService(), table, easyDbInstance)
        { }

        public UpdateQuery(IQueryBuilderService queryBuilderService, string table, EasyDb easyDbInstance)
        {
            this.queryBuilderService = queryBuilderService;
            this.table = table;
            this.columnValues = new Dictionary<string, object>();
            this.easyDbInstance = easyDbInstance;
        }

        public UpdateQuery Set(string column, object value)
        {
            this.columnValues[column] = value;
            this.hasSet = true;
            return this;
        }


        public UpdateQuery Where(Condition condition)
        {
            if (this.hasWhere) { throw new Exception("One where clause"); }

            this.condition = condition;
            this.hasWhere = true;
            return this;
        }

        public string Build()
        {
            if(!this.hasSet) { throw new Exception("No column and values"); }

            return this.queryBuilderService.GetUpdateString(this.table, this.columnValues,this.condition);
        }

        //public function execute()
        //{
        //    $queryString = $this->build();

        //    return Db::getInstance()->query($queryString)->execute();
        //}
    }
}
