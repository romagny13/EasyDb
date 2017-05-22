namespace EasyDbLib
{
    public class Sort
    {
        public string Column { get; }
        public string Direction { get; }

        public Sort(string column, string direction = "")
        {
            this.Direction = direction;
            this.Column = column;
        }

        public static Sort Asc(string column)
        {
            return new Sort(column);
        }
        public static Sort Desc(string column)
        {
            return new Sort(column, "desc");
        }
    }
}
