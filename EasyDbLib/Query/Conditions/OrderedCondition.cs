namespace EasyDbLib
{
    public class OrderedCondition
    {
        public string Operator { get; } // and | or
        public Condition Condition { get; }

        public OrderedCondition(string op, Condition condition)
        {
            this.Operator = op;
            this.Condition = condition;
        }
    }
}