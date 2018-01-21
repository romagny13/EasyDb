namespace EasyDbLib
{
    public class ConditionAndParameter
    {
        public bool IsConditionOp { get; }

        public string ColumnName { get; }

        public string ValueString { get; }

        public string ParameterName { get; } // @id @id2

        public object ParameterValue { get; }

        public ConditionAndParameter(string columnName, string parameterName, string valueString, object parameterValue, bool isConditionOp)
        {
            this.ColumnName = columnName;
            this.ParameterName = parameterName;
            this.ValueString = valueString;
            this.ParameterValue = parameterValue;
            this.IsConditionOp = isConditionOp;
        }
    }
}
