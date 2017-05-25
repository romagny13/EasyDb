using System.Collections.Generic;

namespace EasyDbLib
{
    public interface IQueryService
    {
        string GetFrom(string table);
        string GetFrom(params string[] tableNames);
        string GetOrderBy(string[] sorts);
        string GetParameterName(string columnName);
        string GetSelect(string[] statements, int? limit, Table mapping);
        string GetSelect(Table mapping);
        string GetSelectWithTableName(Table mapping);
        string GetWhere(ConditionAndParameterContainer condition);
        string GetWhereHasOne(ForeignKeyColumn[] foreignKeys);
        string GetWhereHasMany(ForeignKeyColumn[] foreignKeys);
        string GetWhereHasManyToMany(IntermediatePrimaryKeyColumn[] intermediateTablePrimaryKeys, IntermediatePrimaryKeyColumn[] intermediateTablePrimaryKeysForCheckValue);
        string GetInsertInto(string table, string[] columns, bool lastInsertedId);
        string GetUpdate(string table, string[] columns, ConditionAndParameterContainer condition);
        string GetDelete(string table, ConditionAndParameterContainer condition);
    }
}