using System.Data;

namespace EasyDbLib
{
    public class ForeignKeyColumn : Column
    {
        public string TableReferenced { get; }
        public string PrimaryKeyReferenced { get; }

        public ForeignKeyColumn(string columnName, string tableReferenced, string primaryKeyReferenced, string propertyName, DbType? dbType, bool ignore) 
            : base(columnName, propertyName, dbType, ignore)
        {
            this.TableReferenced = tableReferenced;
            this.PrimaryKeyReferenced = primaryKeyReferenced;
        }
    }
}
