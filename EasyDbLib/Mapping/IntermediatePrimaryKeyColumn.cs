namespace EasyDbLib
{
    public class IntermediatePrimaryKeyColumn
    {
        public string TableName { get; }
        public string ColumnName { get; }
        public string TargetPrimaryKey { get; }
        public string TargetTableName { get; set; }

        public IntermediatePrimaryKeyColumn(string columnName, string tableName, string targetPrimaryKey, string targetTableName) 
        {
            this.ColumnName = columnName;
            this.TableName = tableName;
            this.TargetPrimaryKey = targetPrimaryKey;
            this.TargetTableName = targetTableName;
        }
    }

}
