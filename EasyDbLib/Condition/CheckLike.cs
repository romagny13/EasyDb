namespace EasyDbLib
{
    public class CheckLike : Check
    {
        public string Value { get; protected set; }

        public CheckLike(string column, string value)
            : base(column)
        {
            this.Value = value;
        }
    }
}