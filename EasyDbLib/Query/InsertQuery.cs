using System;


namespace EasyDbLib
{ 
    public class InsertQuery
    {
        protected IQueryBuilderService queryBuilderService;

        protected string table;
        protected string[] columns;
        protected object[] values;
        protected bool hasColumns = false;
        protected bool hasValues = false;
        private EasyDb easyDbInstance;

        public InsertQuery(string table, EasyDb easyDbInstance)
            :this(new QueryBuilderService(), table, easyDbInstance)
        { }

        public InsertQuery(IQueryBuilderService queryBuilderService, string table, EasyDb easyDbInstance)
        {
            this.queryBuilderService = queryBuilderService;
            this.table = table;
            this.easyDbInstance = easyDbInstance;
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

        public string build()
        {
            if (!this.hasColumns) {  throw new Exception("No columns provided");}
            //if (!this.hasValues) { throw new Exception("No values provided"); }

            return this.queryBuilderService.GetInsertIntoString(this.table, this.columns);
        }


        //public InsertQuery execute(array values= null)
        //{
        //    if (!isset(values) && count(this.table_values) === 0) { throw new Exception("No values provided"); }

        //    // build la request
        //    queryString = this.build();

        //    return Db::getInstance().prepare(queryString).execute(values);
        //}
    }
}
