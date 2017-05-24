namespace EasyDbLib
{
    public class ConditionLike : Condition
    {
        public string Value { get; protected set; }

        public ConditionLike(string column, string value)
            : base(column)
        {
            this.Value = value;
        }
    }
}