namespace EasyDbLib
{
    public interface IQueryService
    {
        string DefaultPrimaryKeyName { get; }

        string FormatTableAndColumn(string columnName);
        string GetBetween(CheckBetween condition);
        string GetColumns(string[] columns, bool space = true);
        string GetConditionOp(CheckOp condition);
        string GetConditionString(Check condition);
        string GetDelete(string tableName, Check condition);
        string GetFrom(string tableName);
        string GetInsertInto(string tableName, string[] columns, bool lastInsertedId);
        string GetLike(CheckLike condition);
        string GetLimit(int? limit);
        string GetNull(CheckNull condition);
        string GetOrderBy(string[] sorts);
        string GetParameterName(string columnName);
        string GetParameters(string[] columns);
        string GetSelect(int? limit, string[] columns);
        string GetSelect(int? limit, string[] columns, string tableName, Check condition, string[] sorts);
        string GetSelectCount(string tableName, Check condition);
        string GetSet(string[] columns);
        string GetSorts(string[] sorts);
        string GetUpdate(string tableName, string[] columns, Check condition);
        string GetWhere(Check condition);
        bool IsValidSort(string value);
        string JoinOrderByStringValue(string sort);
        string RemoveQuotes(string value);
        string WrapWithQuotes(string value);
    }
}