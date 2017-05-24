namespace EasyDbLib
{
    public class ConditionNull : Condition
    {
        public bool ValueIsNull { get; protected set; }

        public ConditionNull(string column, bool isNull)
            : base(column)
        {

            this.ValueIsNull = isNull;
        }
    }
}