namespace EasyDbLib
{
    public class ColumnMapping
    {
        public string ColumnName { get; }
        public string PropertyName { get; }
        public bool Ignore { get; set; }

        public ColumnMapping(string columnName, string propertyName, bool ignore = false)
        {
            this.ColumnName = columnName;
            this.PropertyName = propertyName;
            this.Ignore = ignore;
        }
    }
}