namespace EasyDbLib
{
    public class ConditionLike : Condition
    {
        public object Value { get; protected set; }

        public ConditionLike(string column, object value)
            : base(column)
        {
            this.Value = value;
        }
    }
}