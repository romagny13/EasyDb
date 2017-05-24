namespace EasyDbLib
{
    public class ConditionAndParameter
    {
        public bool IsConditionOp { get; }

        public string Column { get; }

        // only op require an unique  parameter name
        // "=@id" or "=@id2"
        // " like '" + condition.Value + "'" 
        // " between " + condition.Value1 + " and " + condition.Value2
        public string ValueString { get; }

        // @id @id2
        public string ParameterName { get; }
        
        // id => 10
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
