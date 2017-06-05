namespace EasyDbLib
{
    public class MySQLQueryService : QueryService
    {
        public MySQLQueryService()
            : base("`", "`")
        { }

        public override string GetInsertInto(string table, string[] columns, bool lastInsertedId)
        {
            var result = "insert into " + this.FormatTableAndColumn(table) + " (" + this.GetColumns(columns, false) + ")";
            result += " values (" + this.GetParameters(columns) + ");";
            if (lastInsertedId) { result += "select last_insert_id();"; }
            return result;
        }

        public override string GetLimit(int? top)
        {
            return top.HasValue ? " limit " + top : "";
        }

        public override string GetSelectFromOrderBy(Table mapping, string[] statements, int? limit, ConditionAndParameterContainer condition, string[] sorts)
        {
            return this.GetSelect(statements, null, mapping)
                   + this.GetFrom(mapping.TableName)
                   + this.GetWhere(condition)
                   + this.GetOrderBy(sorts)
                   + this.GetLimit(limit);
        }
    }
}
