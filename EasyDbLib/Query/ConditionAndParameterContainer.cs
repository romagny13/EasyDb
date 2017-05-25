using System;
using System.Collections.Generic;
using System.Linq;

namespace EasyDbLib
{
    public class ConditionAndParameterContainer
    {
        public ConditionAndParameter Main { get; }
        public List<OrderedConditionAndParameter> SubConditions { get; }
        protected List<string> uniqueParameterNames;

        public ConditionAndParameterContainer(Check condition)
        {
            this.SubConditions = new List<OrderedConditionAndParameter>();
            this.uniqueParameterNames = new List<string>();

            this.Main = this.CreateParameter(condition);

            if (condition.HasConditions())
            {
                foreach (var orderedCondition in condition.OrderedConditions)
                {
                    this.AddParameter(this.CreateOrderedParameter(orderedCondition.Condition, orderedCondition.Operator));
                }
            }
        }

        public bool HasConditions()
        {
            return this.SubConditions.Count > 0;
        }

        public void AddParameter(OrderedConditionAndParameter parameter)
        {
            this.SubConditions.Add(parameter);
        }

        public object GetValue(Check condition)
        {
            if (condition is CheckOp)
            {
                return ((CheckOp)condition).Value;
            }
            else if (condition is CheckLike)
            {
                return ((CheckLike)condition).Value;
            }
            return null;
        }

        public OrderedConditionAndParameter CreateOrderedParameter(Check condition, string op)
        {
            var isConditionOp = condition.GetType() == typeof(CheckOp);
            var parameterName = isConditionOp ? this.GetUniqueParameterName(condition.Column) : this.GetParameterName(condition.Column);
            var valueString = this.GetValueString(condition, parameterName);
            var parameterValue = this.GetValue(condition);
            return new OrderedConditionAndParameter(condition.Column, parameterName, valueString, parameterValue, isConditionOp,op);
        }

        public ConditionAndParameter CreateParameter(Check condition)
        {
            var isConditionOp = condition.GetType() == typeof(CheckOp);
            var parameterName = isConditionOp ? this.GetUniqueParameterName(condition.Column) : this.GetParameterName(condition.Column);
            var valueString = this.GetValueString(condition, parameterName);
            var parameterValue = this.GetValue(condition);
            return new ConditionAndParameter(condition.Column, parameterName, valueString, parameterValue, isConditionOp);
        }

        public string GetUniqueParameterName(string columnName)
        {
            var parameter = this.GetParameterName(columnName);
            if (this.uniqueParameterNames.Contains(parameter))
            {
                var count = this.uniqueParameterNames.Count(c => c == parameter);
                this.uniqueParameterNames.Add(parameter);
                return parameter + (count + 1).ToString();
            }
            else
            {
                this.uniqueParameterNames.Add(parameter);
                return parameter;
            }
        }

        public string GetValueString(Check condition,string parameterName)
        {
            if (condition is CheckOp)
            {
                return this.GetConditionOpValueString(((CheckOp)condition).Operator, parameterName);
            }
            else if (condition is CheckLike)
            {
                return this.GetConditionLikeValueString(((CheckLike)condition).Value);
            }
            else if (condition is CheckBetween)
            {
                return this.GetConditionBetweenValueString(((CheckBetween)condition).Value1, ((CheckBetween)condition).Value2);
            }
            else if (condition is CheckNull)
            {
                return this.GetConditionNullString(((CheckNull)condition).ValueIsNull);
            }
            throw new Exception("Condition not supported");
        }

        public string GetParameterName(string columnName)
        {
            var result = columnName.Split('.').Last();
            return "@" + result.Replace(' ', '_');
        }

        public string GetConditionOpValueString(string op, string parameterName)
        {
            return op + parameterName;
        }

        public string GetConditionLikeValueString(string value)
        {
            return " like '" + value + "'";
        }

        public string GetConditionBetweenValueString(int value1, int value2)
        {
            return " between " + value1 + " and " + value2;
        }

        public string GetConditionNullString(bool isNull)
        {
            return isNull ? " is null" : " is not null";
        }

    }

}
