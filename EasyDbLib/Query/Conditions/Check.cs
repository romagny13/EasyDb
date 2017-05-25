using System.Collections.Generic;

namespace EasyDbLib
{
    public class Check
    {
        public List<OrderedCondition> OrderedConditions { get; }
        public string Column { get; protected set; }

        public Check(string column)
        {
            this.Column = column;
            this.OrderedConditions = new List<OrderedCondition>();
        }

        public Check And(Check condition)
        {
            this.OrderedConditions.Add(new OrderedCondition("and", condition));
            return this;
        }

        public Check Or(Check condition)
        {
            this.OrderedConditions.Add(new OrderedCondition("or", condition));
            return this;
        }

        public bool HasConditions()
        {
            return this.OrderedConditions.Count > 0;
        }

        public static CheckOp Op(string column, string op, object value)
        {
            return new CheckOp(column, op, value);
        }

        public static CheckOp Op(string column, object value)
        {
            return new CheckOp(column, "=", value);
        }

        public static CheckLike Like(string column, string value)
        {
            return new CheckLike(column, value);
        }

        public static CheckBetween Between(string column, int value1, int value2)
        {
            return new CheckBetween(column, value1, value2);
        }

        public static CheckNull IsNotNull(string column)
        {
            return new CheckNull(column, false);
        }

        public static CheckNull IsNull(string column)
        {
            return new CheckNull(column, true);
        }
    }
}