using System.Collections.Generic;

namespace EasyDbLib
{
    public class Condition
    {
        public List<OrderedCondition> OrderedConditions { get; }
        public string Column { get; protected set; }

        public Condition(string column)
        {
            this.Column = column;
            this.OrderedConditions = new List<OrderedCondition>();
        }

        public Condition And(Condition condition)
        {
            this.OrderedConditions.Add(new OrderedCondition("and", condition));
            return this;
        }

        public Condition Or(Condition condition)
        {
            this.OrderedConditions.Add(new OrderedCondition("or", condition));
            return this;
        }

        public static ConditionOp Op(string column, string op, object value)
        {
            return new ConditionOp(column, op, value);
        }

        public static ConditionOp Op(string column, object value)
        {
            return new ConditionOp(column, "=", value);
        }

        public static ConditionLike Like(string column, string value)
        {
            return new ConditionLike(column, value);
        }

        public static ConditionBetween Between(string column, int value1, int value2)
        {
            return new ConditionBetween(column, value1, value2);
        }
    }
}