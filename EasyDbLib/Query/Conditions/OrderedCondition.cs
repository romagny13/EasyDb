namespace EasyDbLib
{
    public class OrderedCondition
    {
        public string Operator { get; } // and | or
        public Check Condition { get; }

        public OrderedCondition(string op, Check condition)
        {
            this.Operator = op;
            this.Condition = condition;
        }
    }
}