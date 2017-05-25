namespace EasyDbLib
{
    public class CheckOp : Check
    {
        public string Operator { get; protected set; }
        public object Value { get; protected set; }

        public CheckOp(string column, string op, object value)
            :base(column)
        {
            this.Value = value;
            this.Operator = op;
        }
    }
}