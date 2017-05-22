using System.Collections.Generic;

namespace EasyDbLib
{
    public interface IQueryBuilderService
    {
        string GetOrderByString(string[] sorts);
        string GetSelectFromString(string[] statements, int? top, string[] columns, string[] tables);
        string GetWhereString(Condition condition);
        string GetInsertIntoString(string table, string[] columns);
        string GetUpdateString(string table, Dictionary<string, object> columnValues, Condition condition);
        string GetDeleteFromString(string table, Condition condition);
    }
}