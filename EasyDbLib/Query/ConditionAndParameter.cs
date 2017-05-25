namespace EasyDbLib
{
    public class ConditionAndParameter
    {
        public bool IsConditionOp { get; }
        public string Column { get; }
        public string ValueString { get; }
        public string ParameterName { get; } // @id @id2
        public object ParameterValue { get; }

        public ConditionAndParameter(string column, string parameterName, string valueString, object parameterValue, bool isConditionOp)
        {
            this.Column = column;
            this.ParameterName = parameterName;
            this.ValueString = valueString;
            this.ParameterValue = parameterValue;
            this.IsConditionOp = isConditionOp;
        }
    }

    public class OrderedConditionAndParameter : ConditionAndParameter
    {
        public string Op { get; } // and | or

        public OrderedConditionAndParameter(string column, string parameterName, string valueString, object parameterValue, bool isConditionOp, string op)
            : base(column, parameterName, valueString, parameterValue, isConditionOp)
        {
            this.Op = op;
        }
    }
}
