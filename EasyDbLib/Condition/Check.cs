using System.Collections.Generic;

namespace EasyDbLib
{
    public class Check
    {
        public List<ChainedCondition> ChainedConditions { get; }

        public string ColumnName { get; protected set; }

        public bool HasChainedConditions => this.ChainedConditions.Count > 0;

        public Check(string columnName)
        {
            this.ColumnName = columnName;
            this.ChainedConditions = new List<ChainedCondition>();
        }

        public static CheckOp Op(string columnName, string op, object value)
        {
            Guard.IsNullOrEmpty(columnName);
            Guard.IsNullOrEmpty(op);

            return new CheckOp(columnName, op, value);
        }

        public static CheckOp Op(string columnName, object value)
        {
            Guard.IsNullOrEmpty(columnName);

            return new CheckOp(columnName, "=", value);
        }

        public static CheckLike Like(string columnName, string value, bool ignoreCase = false)
        {
            Guard.IsNullOrEmpty(columnName);
            Guard.IsNullOrEmpty(value);

            return new CheckLike(columnName, value, ignoreCase);
        }

        public static CheckBetween Between(string columnName, int value1, int value2)
        {
            Guard.IsNullOrEmpty(columnName);

            return new CheckBetween(columnName, value1, value2);
        }

        public static CheckNull IsNotNull(string columnName)
        {
            Guard.IsNullOrEmpty(columnName);

            return new CheckNull(columnName, false);
        }

        public static CheckNull IsNull(string columnName)
        {
            Guard.IsNullOrEmpty(columnName);

            return new CheckNull(columnName, true);
        }

        private void SetRank(CheckOp condition)
        {
            var rank = 1;
            var columnName = condition.ColumnName;
            if (this is CheckOp && this.ColumnName == columnName)
            {
                rank += 1;
            }
            foreach (var subCondition in this.ChainedConditions)
            {
                if (subCondition.Condition is CheckOp
                    && subCondition.Condition.ColumnName == columnName)
                {
                    rank += 1;
                }
            }
            condition.Rank = rank;
        }

        public Check And(Check condition)
        {
            Guard.IsNull(condition);

            if (condition is CheckOp)
            {
                SetRank((CheckOp)condition);
            }

            this.ChainedConditions.Add(new ChainedCondition("and", condition));
            return this;
        }

        public Check Or(Check condition)
        {
            Guard.IsNull(condition);

            if (condition is CheckOp)
            {
                SetRank((CheckOp)condition);
            }

            this.ChainedConditions.Add(new ChainedCondition("or", condition));
            return this;
        }
    }
}