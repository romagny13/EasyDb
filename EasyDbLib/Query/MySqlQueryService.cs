namespace EasyDbLib
{
    public class MySqlQueryService : QueryServiceBase
    {
        public MySqlQueryService()
            : base('`', '`', "id")
        { }

        public override string GetInsertInto(string table, string[] columns, bool lastInsertedId)
        {
            var result = "insert into " + this.FormatTableAndColumn(table) + " (" + this.GetColumns(columns, false) + ")";
            result += " values (" + this.GetParameters(columns) + ");";
            if (lastInsertedId) { result += "select last_insert_id();"; }
            return result;
        }

        public override string GetLimit(int? limit)
        {
            return limit.HasValue ? " limit " + limit : "";
        }

        public override string GetSelect(int? limit, string[] columns, string tableName, Check condition, string[] sorts)
        {
            return this.GetSelect(null, columns)
              + this.GetFrom(tableName)
              + this.GetWhere(condition)
              + this.GetOrderBy(sorts)
              + this.GetLimit(limit);
        }

    }
}
