using System.Text;
using System.Threading.Tasks;

namespace EasyDbLib
{
    public class QueryBuilder
    {
        protected EasyDb easyDbInstance;

        public QueryBuilder(EasyDb easyDbInstance)
        {
            this.easyDbInstance = easyDbInstance;
        }

        public SelectQuery Select(params string[] columns)
        {
            if (columns.Length == 0)
            {
                columns = new[] { "*" };
            }
            // select distinct col from table where cond orderby col
            return new SelectQuery(columns, easyDbInstance);
        }

        public InsertQuery InsertInto(string table)
        {
            // insert into table_name (colum_name, col2) values (?,?)
            return new InsertQuery(table, easyDbInstance);
        }

        public UpdateQuery Update(string table)
        {
            // update table_name set column1 = value1, column2 = value2 where cond1 and ...
            return new UpdateQuery(table, easyDbInstance);
        }

        public DeleteQuery DeleteFrom(string table)
        {
            // delete from table_name where id=10 and ...
            return new DeleteQuery(table, easyDbInstance);
        }
    }
}
