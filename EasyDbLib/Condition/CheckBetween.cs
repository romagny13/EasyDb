namespace EasyDbLib
{
    public class CheckBetween : Check
    {
        public int Value1 { get; protected set; }
        public int Value2 { get; protected set; }

        public CheckBetween(string column, int value1, int value2)
            :base(column)
        {
            this.Value1 = value1;
            this.Value2 = value2;
        }
    }
}