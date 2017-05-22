using System;

namespace EasyDbLib
{
    public class SelectQuery
    {
        protected IQueryBuilderService queryBuilderService;

        protected string[] columns; // select
        protected string[] statements; // distinct
        protected string[] tables; // from
        protected Condition condition; // where
        protected int? top; // limit
        protected string[] sorts; // order by

        protected bool hasOptions = false;
        protected bool hasFrom = false;
        protected bool hasWhere = false;
        protected bool hasLimit = false;
        protected bool hasOrderBy = false;
        private EasyDb easyDbInstance;

        public SelectQuery(string[] columns, EasyDb easyDbInstance)
            : this(new QueryBuilderService(), columns, easyDbInstance)
        { }

        public SelectQuery(IQueryBuilderService queryBuilderService, string[] columns, EasyDb easyDbInstance)
        {
            this.columns = columns;
            this.queryBuilderService = queryBuilderService;
            this.easyDbInstance = easyDbInstance;
            this.statements = new string[] { };
            this.sorts = new string[] { };
        }


        public SelectQuery Statements(params string[] statements)
        {
            if (this.hasOptions) { throw new Exception("Options already defined"); }
            this.statements = statements;
            this.hasOptions = true;
            return this;
        }

        public SelectQuery Top(int value)
        {
            if (this.hasLimit) { throw new Exception("One limit clause"); }
            this.top = value;
            this.hasLimit = true;
            return this;
        }

        public SelectQuery From(params string[] tables)
        {
            if (this.hasFrom) { throw new Exception("One from clause"); }
            this.tables = tables;
            this.hasFrom = true;
            return this;
        }

        public SelectQuery Where(Condition condition)
        {
            if (this.hasWhere) { throw new Exception("One where clause"); }
            this.condition = condition;
            this.hasWhere = true;
            return this;
        }

        public SelectQuery OrderBy(params string[] sorts)
        {
            if (this.hasOrderBy) { throw new Exception("One order by clause"); }
            this.sorts = sorts;
            this.hasOrderBy = true;
            return this;
        }

        public SelectQuery OrderBy(params Sort[] sorts)
        {
            if (this.hasOrderBy) { throw new Exception("One order by clause"); }
            this.sorts = new string[sorts.Length];
            for (int i = 0; i < sorts.Length; i++)
            {
                var value = sorts[i].Direction == "" ? sorts[i].Column : sorts[i].Column + " " + sorts[i].Direction;
                this.sorts[i] = value;
            }
            this.hasOrderBy = true;
            return this;
        }

        public string Build()
        {
            if (!this.hasFrom) { throw new Exception("No table(s) provided"); }

            return this.queryBuilderService.GetSelectFromString(this.statements, this.top, this.columns, this.tables)
                   + this.queryBuilderService.GetWhereString(this.condition)
                   + this.queryBuilderService.GetOrderByString(this.sorts);
        }


    }
}