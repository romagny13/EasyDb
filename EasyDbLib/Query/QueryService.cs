using System;
using System.Collections.Generic;
using System.Linq;

namespace EasyDbLib
{
    public class QueryService : IQueryService
    {
        protected string startQuote;
        protected string endQuote;

        public QueryService(string startQuote = "[", string endQuote = "]")
        {
            this.startQuote = startQuote;
            this.endQuote = endQuote;
        }

        public virtual string WrapWithQuotes(string value)
        {
            return this.startQuote + value + this.endQuote;
        }

        public virtual string GetTypedValue(object value)
        {
            if (value.GetType() == typeof(string))
            {
                return "'" + value + "'";
            }
            else
            {
                return value.ToString();
            }
        }

        public virtual string GetStatements(string[] statements)
        {
            return statements.Length > 0 ? " " + string.Join(" ", statements) : "";
        }

        public virtual string GetLimit(int? top)
        {
            return top.HasValue ? " top " + top : "";
        }

        public virtual string FormatTableAndColumn(string columnName)
        {
            var result = new List<string>();
            var parts = columnName.Split('.');
            foreach (var part in parts)
            {
                result.Add(this.WrapWithQuotes(part));
            }
            return string.Join(".", result);
        }

        public virtual string GetColumns(Table mapping)
        {
            var result = new List<string>();

            if (!mapping.HasNoColumnOrOnlyKeys)
            {
                foreach (var column in mapping.columns)
                {
                    if (!column.Value.Ignore)
                    {
                        result.Add(this.FormatTableAndColumn(column.Value.ColumnName));
                    }
                }

                if (result.Count > 0)
                {
                    return " " + string.Join(",", result);
                }
            }
            return " *";
        }

        public virtual string GetColumnsWithTableName(Table mapping)
        {
            var result = new List<string>();

            if (!mapping.HasNoColumnOrOnlyKeys)
            {
                foreach (var column in mapping.columns)
                {
                    if (!column.Value.Ignore)
                    {
                        result.Add(this.WrapWithQuotes(mapping.TableName) + "." + this.FormatTableAndColumn(column.Value.ColumnName));
                    }
                }

                if (result.Count > 0)
                {
                    return " " + string.Join(",", result);
                }
            }
            return " " + this.WrapWithQuotes(mapping.TableName) + ".*";
        }

        public virtual string GetColumns(string[] columns, bool space = true)
        {
            var result = new List<string>();
            foreach (var column in columns)
            {
                result.Add(this.FormatTableAndColumn(column));
            }
            return space ? " " + string.Join(",", result) : string.Join(",", result);
        }

        public virtual bool IsValidSort(string value)
        {
            return value.ToLower() == "desc" || value.ToLower() == "asc";
        }

        public virtual string JoinOrderByStringValue(string sort)
        {
            var parts = sort.Split(' ');
            if(parts.Length > 2 || (parts.Length == 2 && !IsValidSort(parts[1]))) { throw new Exception("Invalid sort");}
            return parts.Length == 2 ? this.FormatTableAndColumn(parts[0]) + " " + parts[1] : this.FormatTableAndColumn(parts[0]);
        }

        public virtual string GetSorts(string[] sorts)
        {
            var result = new List<string>();
            foreach (var sort in sorts)
            {
                result.Add(this.JoinOrderByStringValue(sort));
            }
            return string.Join(",", result);
        }

        public virtual string GetConditionString(ConditionAndParameterContainer condition)
        {
            var result = this.FormatTableAndColumn(condition.Main.Column) + condition.Main.ValueString;

            if (condition.HasConditions())
            {
                foreach (var orderedCondition in condition.SubConditions)
                {
                    result += " " + orderedCondition.Op + " " + this.FormatTableAndColumn(orderedCondition.Column) + orderedCondition.ValueString;
                }
            }
            return result;
        }

        public virtual string GetParameterName(string columnName)
        {
            var result = columnName.Split('.').Last();
            return "@" + result.Replace(' ', '_');
        }

        public virtual string GetParameters(string[] columns)
        {
            var result = new List<string>();
            foreach (var column in columns)
            {
                result.Add(this.GetParameterName(column));
            }
            return string.Join(",", result);
        }

        public virtual string GetSet(string[] columns)
        {
            var result = new List<string>();
            foreach (var column in columns)
            {
                result.Add(this.FormatTableAndColumn(column) + "=" + this.GetParameterName(column));
            }
            return string.Join(",", result);
        }

        // select

        public virtual string GetSelect(string[] statements, int? limit, Table mapping)
        {
            return "select" +  this.GetStatements(statements) + this.GetLimit(limit) + this.GetColumns(mapping);
        }

        public virtual string GetSelect(Table mapping)
        {
            return "select" + this.GetColumns(mapping);
        }

        public virtual string GetSelectWithTableName(Table mapping)
        {
            return "select" + this.GetColumnsWithTableName(mapping);
        }

        // from 

        public virtual string GetFrom(string tableName)
        {
            return " from " + this.FormatTableAndColumn(tableName) ;
        }

        public virtual string GetFrom(params string[] tableNames)
        {
            if (tableNames.Length > 0)
            {
                var result = new List<string>();
                foreach (var tableName in tableNames)
                {
                    result.Add(this.FormatTableAndColumn(tableName));
                }
                return " from " + string.Join(",",result);
            }
            return "";
        }

        // order by

        public virtual string GetOrderBy(string[] sorts)
        {
            return sorts != null && sorts.Length > 0 ? " order by " + this.GetSorts(sorts): "";
        }


        // where 

        public virtual string GetWhere(ConditionAndParameterContainer condition)
        {
            if (condition != null)
            {
                return " where " + this.GetConditionString(condition);
            }
            return "";
        }

        public virtual string GetWhereHasOne(ForeignKeyColumn[] foreignKeys)
        {
            // where pk = @fk and pk2=@fk2
            var result = new List<string>();
            foreach (var foreignKey in foreignKeys)
            {
                result.Add(this.FormatTableAndColumn(foreignKey.PrimaryKeyReferenced) + "=" + this.GetParameterName(foreignKey.ColumnName));
            }
            return " where " + string.Join(" and ", result);
        }

        public virtual string GetWhereHasMany(ForeignKeyColumn[] foreignKeys)
        {
            // where pk = @fk and pk2=@fk2
            var result = new List<string>();
            foreach (var foreignKey in foreignKeys)
            {
                result.Add(this.FormatTableAndColumn(foreignKey.ColumnName) + "=" + this.GetParameterName(foreignKey.PrimaryKeyReferenced));
            }
            return " where " + string.Join(" and ", result);
        }

        public virtual string GetWhereHasManyToMany(IntermediatePrimaryKeyColumn[] intermediateTablePrimaryKeys, IntermediatePrimaryKeyColumn[] intermediateTablePrimaryKeysForCheckValue)
        {
            if (intermediateTablePrimaryKeys.Length > 0 && intermediateTablePrimaryKeysForCheckValue.Length > 0)
            {
                var result = new List<string>();
                foreach (var intermediateTablePrimaryKey in intermediateTablePrimaryKeys)
                {
                    result.Add(this.WrapWithQuotes(intermediateTablePrimaryKey.TableName) + "." + this.WrapWithQuotes(intermediateTablePrimaryKey.ColumnName) + "=" + this.WrapWithQuotes(intermediateTablePrimaryKey.TargetTableName) + "." + this.WrapWithQuotes(intermediateTablePrimaryKey.TargetPrimaryKey));
                }
                foreach (var intermediateTablePrimaryKeyForCheckValue in intermediateTablePrimaryKeysForCheckValue)
                {
                    result.Add(this.WrapWithQuotes(intermediateTablePrimaryKeyForCheckValue.TableName) + "." + this.WrapWithQuotes(intermediateTablePrimaryKeyForCheckValue.ColumnName) + "=" + this.GetParameterName(intermediateTablePrimaryKeyForCheckValue.ColumnName));
                }
                return " where " + string.Join(" and ", result);
            }
            return "";
        }

        // insert

        public virtual string GetInsertInto(string table, string[] columns, bool lastInsertedId)
        {
            var result = "insert into " + this.FormatTableAndColumn(table) + " (" + this.GetColumns(columns, false) + ")";
            if (lastInsertedId) { result += " output inserted.id"; }
            result += " values (" + this.GetParameters(columns) + ")";
            return result;
        }

        // update

        public virtual string GetUpdate(string table, string[] columns, ConditionAndParameterContainer condition)
        {
            return "update " + this.FormatTableAndColumn(table) + " set " + this.GetSet(columns) + this.GetWhere(condition);
        }

        // delete

        public virtual string GetDelete(string table, ConditionAndParameterContainer condition)
        {
            return "delete from " + this.FormatTableAndColumn(table) + this.GetWhere(condition);
        }

    }
}
