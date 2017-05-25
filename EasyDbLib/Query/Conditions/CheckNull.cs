namespace EasyDbLib
{
    public class CheckNull : Check
    {
        public bool ValueIsNull { get; protected set; }

        public CheckNull(string column, bool isNull)
            : base(column)
        {

            this.ValueIsNull = isNull;
        }
    }
}