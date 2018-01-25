namespace EasyDbLib
{

    public class CheckOp : Check
    {
        public string Operator { get; }

        public object Value { get;  }

        internal int Rank;

        public CheckOp(string column, string op, object value)
            :base(column)
        {
            this.Value = value;
            this.Operator = op;
        }
    }
}