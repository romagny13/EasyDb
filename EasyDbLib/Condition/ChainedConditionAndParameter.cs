namespace EasyDbLib
{
    public class ChainedConditionAndParameter : ConditionAndParameter
    {
        public string Op { get; } // and | or

        public ChainedConditionAndParameter(string columnName, string parameterName, string valueString, object parameterValue, bool isConditionOp, string op)
            : base(columnName, parameterName, valueString, parameterValue, isConditionOp)
        {
            this.Op = op;
        }
    }
}
