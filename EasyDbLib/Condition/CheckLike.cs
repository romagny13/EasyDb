namespace EasyDbLib
{
    public class CheckLike : Check
    {
        public string Value { get; protected set; }

        public bool IgnoreCase { get; }

        public CheckLike(string column, string value, bool ignoreCase)
            : base(column)
        {
            this.Value = value;
            this.IgnoreCase = ignoreCase;
        }
    }
}