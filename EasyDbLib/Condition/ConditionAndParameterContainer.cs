using System;
using System.Collections.Generic;
using System.Linq;

namespace EasyDbLib
{
    public class ConditionAndParameterContainer
    {
        protected IQueryService queryService;

        public ConditionAndParameter Main { get; }

        public List<ChainedConditionAndParameter> SubConditions { get; }

        protected List<string> uniqueParameterNames;

        public bool HasSubConditions => this.SubConditions.Count > 0;

        public ConditionAndParameterContainer(Check condition, IQueryService queryService)
        {
            this.queryService = queryService;

            this.SubConditions = new List<ChainedConditionAndParameter>();
            this.uniqueParameterNames = new List<string>();

            this.Main = this.CreateParameter(condition);

            if (condition.HasChainedConditions)
            {
                foreach (var chainedCondition in condition.ChainedConditions)
                {
                    this.AddParameter(this.CreateChainedParameter(chainedCondition.Condition, chainedCondition.Operator));
                }
            }
        }

        public void AddParameter(ChainedConditionAndParameter parameter)
        {
            this.SubConditions.Add(parameter);
        }

        public object GetValue(Check condition)
        {
            if (condition is CheckOp)
            {
                return ((CheckOp)condition).Value;
            }
            return null;
        }

        public bool IsCheckOp(Check condition)
        {
            return condition.GetType() == typeof(CheckOp);
        }

        public ChainedConditionAndParameter CreateChainedParameter(Check condition, string op)
        {
            var isConditionOp = this.IsCheckOp(condition);

            var parameterName = isConditionOp ? this.GetUniqueParameterName(condition.ColumnName)
                : this.queryService.GetParameterName(condition.ColumnName);

            var valueString = this.GetValueString(condition, parameterName);

            var parameterValue = this.GetValue(condition);

            return new ChainedConditionAndParameter(condition.ColumnName, parameterName, valueString, parameterValue, isConditionOp, op);
        }

        public ConditionAndParameter CreateParameter(Check condition)
        {
            var isConditionOp = this.IsCheckOp(condition);

            var parameterName = isConditionOp ? this.GetUniqueParameterName(condition.ColumnName)
                : this.queryService.GetParameterName(condition.ColumnName);

            var valueString = this.GetValueString(condition, parameterName);

            var parameterValue = this.GetValue(condition);

            return new ConditionAndParameter(condition.ColumnName, parameterName, valueString, parameterValue, isConditionOp);
        }

        public string GetUniqueParameterName(string columnName)
        {
            var parameter = this.queryService.GetParameterName(columnName);
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

        public string GetValueString(Check condition, string parameterName)
        {
            if (condition is CheckOp)
            {
                return this.queryService.GetConditionOp(((CheckOp)condition).Operator, parameterName);
            }
            else if (condition is CheckLike)
            {
                return this.queryService.GetLike(((CheckLike)condition).Value);
            }
            else if (condition is CheckBetween)
            {
                return this.queryService.GetBetween(((CheckBetween)condition).Value1, ((CheckBetween)condition).Value2);
            }
            else if (condition is CheckNull)
            {
                return this.queryService.GetIsNull(((CheckNull)condition).ValueIsNull);
            }
            throw new Exception("Condition not supported");
        }
    
    }

}
