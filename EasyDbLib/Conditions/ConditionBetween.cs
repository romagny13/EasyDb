namespace EasyDbLib
{
    public class ConditionBetween : Condition
    {
        public int Value1 { get; protected set; }
        public int Value2 { get; protected set; }

        public ConditionBetween(string column, int value1, int value2)
            :base(column)
        {
            this.Value1 = value1;
            this.Value2 = value2;
        }
    }
}