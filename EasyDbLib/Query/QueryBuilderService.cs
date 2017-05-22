using System.Collections.Generic;
using System.Linq;

namespace EasyDbLib
{
    public class QueryBuilderService : IQueryBuilderService
    {
        protected string start = "[";
        protected string end = "]";
        protected List<string> parametersCache;

        public QueryBuilderService()
        {
                this.parametersCache = new List<string>();
        }

        public string JoinValue(string value)
        {
            // [posts] or [id]
            return this.start + value + this.end;
        }

        public string JoinTableColumn(string value, char splitter = '.')
        {
            // users.id  or dbo.users.id or id => [users].[id] or [dbo].[users].[id] or [id]
            var result = new List<string>();
            var parts = value.Split(splitter);

            foreach (var part in parts)
            {
                result.Add(this.JoinValue(part)); // [id]
            }
            return string.Join(".", result); // [users].[id]
        }

        public string JoinWith(string[] values, string separator = ",")
        {
            var result = new List<string>();
            foreach (var value in values)
            {
                result.Add(this.JoinTableColumn(value));
            }
            // [posts],[users] or [posts].[id] with separator
            return string.Join(separator, result);
        }

        //

        public string GetSelectColumnsString(string[] columns)
        {
            if (columns.Length == 1 && columns[0] == "*")
            {
                return " *";
            }
            else
            {
                return " " + this.JoinWith(columns);
            }
        }

        public string GetOptionString(string[] statements)
        {
            return statements.Length > 0 ? " " + string.Join(" ", statements) : "";
        }

        public string GetTopString(int? top)
        {
            return top.HasValue ? " TOP " + top : "";
        }

        // conditions

        public string GetParameterName(string columnName)
        {
            var result = columnName.Split('.').Last();

            var parameter = "@" + result.Replace(' ', '_');
            if (this.parametersCache.Contains(parameter))
            {
                var count = this.parametersCache.Count(c => c == parameter);
                this.parametersCache.Add(parameter);
                return parameter + (count + 1).ToString();
            }
            else
            {
                this.parametersCache.Add(parameter);
                return parameter;
            }
        }

        public string GetConditionOpString(ConditionOp condition)
        {
            var parameterName = this.GetParameterName(condition.Column);
            return this.JoinTableColumn(condition.Column) + condition.Operator + parameterName;
        }

        public string GetConditionLikeString(ConditionLike condition)
        {
            return this.JoinTableColumn(condition.Column) + " like '" + condition.Value + "'";
        }

        public string GetConditionBetweenString(ConditionBetween condition)
        {
            return this.JoinTableColumn(condition.Column) + " between " + condition.Value1 + " and " + condition.Value2;
        }

        public string GetConditionString(Condition condition)
        {
            var type = condition.GetType();
            if (type == typeof(ConditionOp))
            {
                return GetConditionOpString((ConditionOp)condition);
            }
            else if (type == typeof(ConditionLike))
            {
                return GetConditionLikeString((ConditionLike)condition);
            }
            else if (type == typeof(ConditionBetween))
            {
                return GetConditionBetweenString((ConditionBetween)condition);

            }
            return "";
        }

        public string GetConditionAndOrderedConditionString(Condition condition)
        {
            var result = this.GetConditionString(condition);
            foreach (var orderedCondition in condition.OrderedConditions)
            {
                result += " " + orderedCondition.Operator + " " + this.GetConditionString(orderedCondition.Condition);
            }
            return result;
        }


        //

        public string GetParametersString(string[] columns)
        {
            var result = new List<string>();
            foreach (var column in columns)
            {
              result.Add(this.GetParameterName(column));
            }
            return string.Join(",", result);
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

        public string GetSetString(Dictionary<string, object> columnValues)
        {
            var result = new List<string>();
            foreach (var columnValue in columnValues){
                result.Add(this.JoinTableColumn(columnValue.Key) + "=" + this.GetTypedValue(columnValue.Value) );
            }
            return string.Join(",", result);
        }

        public void ClearParametersCache()
        {
          this.parametersCache.Clear();
        }

        // insert

        public string GetInsertIntoString(string table, string[] columns)
        {
            this.ClearParametersCache();
            return "insert into " + this.JoinTableColumn(table) +  " ("  + this.JoinWith(columns) + ") values ("  + this.GetParametersString(columns)  + ")";
        }

        // update

        public string GetUpdateString(string table,Dictionary<string,object> columnValues, Condition condition)
        {
            this.ClearParametersCache();
            return "update " + this.JoinTableColumn(table) + " set " + this.GetSetString(columnValues) + this.GetWhereString(condition);
        }

        // delete

        public string GetDeleteFromString(string table, Condition condition)
        {
            this.ClearParametersCache();
            return "delete from " + this.JoinTableColumn(table) + this.GetWhereString(condition);
        }

        // select

        public string GetSelectFromString(string[] statements, int? top, string[] columns, string[] tables)
        {
            this.ClearParametersCache();
            return "select" + this.GetOptionString(statements) + this.GetTopString(top) + this.GetSelectColumnsString(columns) + " from " + this.JoinWith(tables);
        }


        // where

        public string GetWhereString(Condition condition)
        {
            if (condition != null)
            {
                return " where " + this.GetConditionAndOrderedConditionString(condition);
            }
            return "";
        }

        // order by

        public string JoinOrderByStringValue(string sort)
        {
            var parts = sort.Split(' ');
            return parts.Length == 2 ? this.JoinTableColumn(parts[0]) + " " + parts[1] : this.JoinTableColumn(parts[0]);
        }

        public string GetOrderByString(string[] sorts)
        {
            if (sorts.Length > 0)
            {
                var result = new List<string>();
                foreach (var sort in sorts)
                {
                    result.Add(this.JoinOrderByStringValue(sort));
                }
                return " order by " + string.Join(",", result);
            }
            return "";
        }
    }
}