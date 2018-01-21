namespace EasyDbLib
{
    public interface IQueryService
    {
        string DefaultPrimaryKeyName { get; }

        string FormatTableAndColumn(string columnName);
        string GetBetween(int value1, int value2);
        string GetColumns(string[] columns, bool space = true);
        string GetConditionOp(string op, string parameterName);
        string GetConditionString(ConditionAndParameterContainer condition);
        string GetDelete(string tableName, ConditionAndParameterContainer condition);
        string GetFrom(string tableName);
        string GetInsertInto(string tableName, string[] columns, bool lastInsertedId);
        string GetIsNull(bool isNull);
        string GetLike(string value);
        string GetLimit(int? limit);
        string GetOrderBy(string[] sorts);
        string GetParameterName(string columnName);
        string GetParameters(string[] columns);
        string GetSelect(int? limit, string[] columns);
        string GetSelect(int? limit, string[] columns, string tableName, ConditionAndParameterContainer condition, string[] sorts);
        string GetSelectCount(string tableName, ConditionAndParameterContainer condition);
        string GetSet(string[] columns);
        string GetSorts(string[] sorts);
        string GetUpdate(string tableName, string[] columns, ConditionAndParameterContainer condition);
        string GetWhere(ConditionAndParameterContainer condition);
        bool IsValidSort(string value);
        string JoinOrderByStringValue(string sort);
        string RemoveQuotes(string value);
        string WrapWithQuotes(string value);
    }
}