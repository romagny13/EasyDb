namespace EasyDbLib
{
    public class ConditionOp : Condition
    {
        public string Operator { get; protected set; }
        public object Value { get; protected set; }

        public ConditionOp(string column, string op, object value)
            :base(column)
        {
            this.Value = value;
            this.Operator = op;
        }
    }
}