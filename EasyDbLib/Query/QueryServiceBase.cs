﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace EasyDbLib
{
    public class QueryServiceBase : IQueryService
    {
        protected char startQuote;
        protected char endQuote;

        public string DefaultPrimaryKeyName { get; protected set; }

        public QueryServiceBase(char startQuote = '[', char endQuote = ']', string defaultPrimaryKeyName = "Id")
        {
            this.startQuote = startQuote;
            this.endQuote = endQuote;
            this.DefaultPrimaryKeyName = defaultPrimaryKeyName;
        }

        public virtual string WrapWithQuotes(string value)
        {
            value = value.Trim();
            if (value[0] != startQuote)
            {
                value = startQuote + value;
            }
            if (value[value.Length - 1] != endQuote)
            {
                value = value + endQuote;
            }
            return value;
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

        public virtual string GetColumns(string[] columns, bool space = true)
        {
            var result = new List<string>();
            foreach (var column in columns)
            {
                result.Add(this.FormatTableAndColumn(column));
            }
            return space ? " " + string.Join(",", result) : string.Join(",", result);
        }

        public virtual string GetLimit(int? limit)
        {
            return limit.HasValue ? " top " + limit : "";
        }

        public virtual string GetConditionOp(CheckOp condition)
        {
            var left = this.FormatTableAndColumn(condition.ColumnName);
            // get unique parameter name
            var parameterName = this.GetParameterName(condition.ColumnName);
            if(condition.Rank > 1)
            {
                parameterName += condition.Rank.ToString();
            }
            return left + condition.Operator + parameterName;
        }

        public virtual string GetLike(CheckLike condition)
        {
            var left = condition.IgnoreCase ?
                   "lower(" + this.FormatTableAndColumn(condition.ColumnName) + ")"
                   : this.FormatTableAndColumn(condition.ColumnName);
            return left + " like '" + condition.Value + "'";
        }

        public virtual string GetBetween(CheckBetween condition)
        {
            return this.FormatTableAndColumn(condition.ColumnName) + " between " + condition.Value1 + " and " + condition.Value2; ;
        }

        public virtual string GetNull(CheckNull condition)
        {
            var left = this.FormatTableAndColumn(condition.ColumnName);
            return condition.ValueIsNull ? left + " is null" : left + " is not null";
        }

        protected virtual string CheckAndGetConditionString(Check condition)
        {
            if (condition.GetType() == typeof(CheckOp))
            {
                return this.GetConditionOp((CheckOp)condition);
            }
            else if (condition.GetType() == typeof(CheckLike))
            {
                return this.GetLike((CheckLike)condition);
            }
            else if (condition.GetType() == typeof(CheckBetween))
            {
                return this.GetBetween((CheckBetween)condition);
            }
            else if (condition.GetType() == typeof(CheckNull))
            {
                return this.GetNull((CheckNull)condition);
            }
            throw new Exception("Cannot resolve condition");
        }

        public virtual string GetConditionString(Check condition)
        {
            var result = "";
            if (condition != null)
            {
                // main
                result = this.CheckAndGetConditionString(condition);

                if (condition.HasChainedConditions)
                {
                    foreach (var subCondition in condition.ChainedConditions)
                    {
                        result += " "
                            + subCondition.Operator // and 
                            + " "
                            + this.CheckAndGetConditionString(subCondition.Condition);
                    }
                }
            }
            return result;
        }

        public virtual string RemoveQuotes(string value)
        {
            if (value != null && value.Length > 0)
            {
                if (value[0] == startQuote)
                {
                    value = value.Remove(0, 1); // remove [
                }
                if (value[value.Length - 1] == endQuote)
                {
                    value = value.Remove(value.Length - 1, 1); // remove ]
                }
            }
            return value;
        }

        public virtual string GetParameterName(string columnName)
        {
            // [dbo].[Role Id]
            var result = columnName.Split('.').Last();
            result = this.RemoveQuotes(result);
            return "@" + result.Replace(' ', '_').ToLower(); //@role_id
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

        public virtual bool IsValidSort(string value)
        {
            return value.ToLower() == "desc" || value.ToLower() == "asc";
        }

        public virtual string JoinOrderByStringValue(string sort)
        {
            var parts = sort.Split(' ');
            if (parts.Length > 2 || (parts.Length == 2 && !IsValidSort(parts[1]))) { Guard.Throw("Invalid sort"); }
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


        // select

        public virtual string GetSelect(int? limit, string[] columns, string tableName, Check condition, string[] sorts)
        {
            Guard.IsNullOrEmpty(tableName);

            return this.GetSelect(limit, columns)
              + this.GetFrom(tableName)
              + this.GetWhere(condition)
              + this.GetOrderBy(sorts);
        }

        public virtual string GetSelectCount(string tableName, Check condition)
        {
            Guard.IsNullOrEmpty(tableName);

            return "select count(*)"
              + this.GetFrom(tableName)
              + this.GetWhere(condition);
        }

        public virtual string GetSelect(int? limit, string[] columns)
        {
            return "select" + this.GetLimit(limit) +
               (columns != null && columns.Length > 0 ? this.GetColumns(columns) : " *");
        }

        public virtual string GetFrom(string tableName)
        {
            Guard.IsNullOrEmpty(tableName);

            return " from " + this.FormatTableAndColumn(tableName);
        }

        public virtual string GetWhere(Check condition)
        {
            if (condition != null)
            {
                return " where " + this.GetConditionString(condition);
            }
            return "";
        }

        public virtual string GetOrderBy(string[] sorts)
        {
            return sorts != null && sorts.Length > 0 ? " order by " + this.GetSorts(sorts) : "";
        }

        // insert

        public virtual string GetInsertInto(string tableName, string[] columns, bool lastInsertedId)
        {
            Guard.IsNullOrEmpty(tableName);
            if (columns.Length == 0) { Guard.Throw("No column provided"); }

            var result = "insert into " + this.FormatTableAndColumn(tableName) + " (" + this.GetColumns(columns, false) + ")";
            if (lastInsertedId) { result += " output inserted.id"; }
            result += " values (" + this.GetParameters(columns) + ")";
            return result;
        }

        // update

        public virtual string GetUpdate(string tableName, string[] columns, Check condition)
        {
            Guard.IsNullOrEmpty(tableName);
            if (columns.Length == 0) { Guard.Throw("No column provided"); }

            return "update " + this.FormatTableAndColumn(tableName) + " set " + this.GetSet(columns) + this.GetWhere(condition);
        }

        // delete

        public virtual string GetDelete(string tableName, Check condition)
        {
            Guard.IsNullOrEmpty(tableName);

            return "delete from " + this.FormatTableAndColumn(tableName) + this.GetWhere(condition);
        }
    }
}
