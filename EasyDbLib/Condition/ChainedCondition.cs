namespace EasyDbLib
{
    public class ChainedCondition
    {
        public string Operator { get; } // and | or

        public Check Condition { get; }

        public ChainedCondition(string op, Check condition)
        {
            this.Operator = op;
            this.Condition = condition;
        }
    }
}