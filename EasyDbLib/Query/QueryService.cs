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

        public string WrapWithQuotes(string value)
        {
            return this.startQuote + value + this.endQuote;
        }

        public string GetTypedValue(object value)
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

        public string GetStatements(string[] statements)
        {
            return statements.Length > 0 ? " " + string.Join(" ", statements) : "";
        }

        public string GetLimit(int? top)
        {
            return top.HasValue ? " top " + top : "";
        }

        public string FormatTableAndColumn(string columnName)
        {
            var result = new List<string>();
            var parts = columnName.Split('.');
            foreach (var part in parts)
            {
                result.Add(this.WrapWithQuotes(part));
            }
            return string.Join(".", result);
        }

        public string GetColumns(Table mapping)
        {
            var result = new List<string>();

            if (!mapping.HasNoColumnOrOnlyKeys)
            {
                foreach (var column in mapping.Columns)
                {
                    if (!column.Value.Ignore)
                    {
                        result.Add(this.FormatTableAndColumn(column.Value.ColumnName));
                    }
                }

                if (mapping.Columns.Count > 0)
                {
                    return " " + string.Join(",", result);
                }
            }
            return " *";
        }

        public string GetColumns(string[] columns, bool space = true)
        {
            var result = new List<string>();
            foreach (var column in columns)
            {
                result.Add(this.FormatTableAndColumn(column));
            }
            return space ? " " + string.Join(",", result) : string.Join(",", result);
        }

        public bool IsValidSort(string value)
        {
            return value.ToLower() == "desc" || value.ToLower() == "asc";
        }

        public string JoinOrderByStringValue(string sort)
        {
            var parts = sort.Split(' ');
            if(parts.Length > 2 || (parts.Length == 2 && !IsValidSort(parts[1]))) { throw new Exception("Invalid sort");}
            return parts.Length == 2 ? this.FormatTableAndColumn(parts[0]) + " " + parts[1] : this.FormatTableAndColumn(parts[0]);
        }

        public string GetSorts(string[] sorts)
        {
            var result = new List<string>();
            foreach (var sort in sorts)
            {
                result.Add(this.JoinOrderByStringValue(sort));
            }
            return string.Join(",", result);
        }

        public string GetConditionString(ConditionAndParameterContainer condition)
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

        public string GetParameterName(string columnName)
        {
            var result = columnName.Split('.').Last();
            return "@" + result.Replace(' ', '_');
        }

        public string GetParameters(string[] columns)
        {
            var result = new List<string>();
            foreach (var column in columns)
            {
                result.Add(this.GetParameterName(column));
            }
            return string.Join(",", result);
        }

        public string GetSet(Dictionary<string, object> columnValues)
        {
            var result = new List<string>();
            foreach (var columnValue in columnValues)
            {
                result.Add(this.FormatTableAndColumn(columnValue.Key) + "=" + this.GetTypedValue(columnValue.Value));
            }
            return string.Join(",", result);
        }

        // select

        public string GetSelect(string[] statements, int? limit, Table mapping)
        {
            return "select" +  this.GetStatements(statements) + this.GetLimit(limit) + this.GetColumns(mapping);
        }

        public string GetSelect(Table mapping)
        {
            return "select" + this.GetColumns(mapping);
        }

        // from 

        public string GetFrom(string table)
        {
            return " from " + this.FormatTableAndColumn(table) ;
        }


        // order by

        public string GetOrderBy(string[] sorts)
        {
            return sorts != null && sorts.Length > 0 ? " order by " + this.GetSorts(sorts): "";
        }


        // where 

        public string GetWhere(ConditionAndParameterContainer condition)
        {
            if (condition != null)
            {
                return " where " + this.GetConditionString(condition);
            }
            return "";
        }

        public string GetWhereHasOne(ForeignKeyColumn[] foreignKeys)
        {
            // where pk = @fk and pk2=@fk2
            var result = new List<string>();
            foreach (var foreignKey in foreignKeys)
            {
                result.Add(this.FormatTableAndColumn(foreignKey.PrimaryKeyReferenced) + "=" + this.GetParameterName(foreignKey.ColumnName));
            }
            return " where " + string.Join(" and ", result);
        }

        public string GetWhereHasMany(ForeignKeyColumn[] foreignKeys)
        {
            // where pk = @fk and pk2=@fk2
            var result = new List<string>();
            foreach (var foreignKey in foreignKeys)
            {
                result.Add(this.FormatTableAndColumn(foreignKey.ColumnName) + "=" + this.GetParameterName(foreignKey.ColumnName));
            }
            return " where " + string.Join(" and ", result);
        }

        // insert

        public string GetInsertInto(string table, string[] columns, bool lastInsertedId)
        {
            var result = "insert into " + this.FormatTableAndColumn(table) + " (" + this.GetColumns(columns, false) + ")";
            if (lastInsertedId) { result += " output INSERTED.Id"; }
            result += " values (" + this.GetParameters(columns) + ")";
            return result;
        }

        // update

        public string GetUpdate(string table, Dictionary<string, object> columnValues, ConditionAndParameterContainer condition)
        {
            return "update " + this.FormatTableAndColumn(table) + " set " + this.GetSet(columnValues) + this.GetWhere(condition);
        }

        // delete

        public string GetDelete(string table, ConditionAndParameterContainer condition)
        {
            return "delete from " + this.FormatTableAndColumn(table) + this.GetWhere(condition);
        }

    }
}
