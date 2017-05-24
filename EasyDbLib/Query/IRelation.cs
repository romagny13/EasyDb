using System.Threading.Tasks;

namespace EasyDbLib
{
    public interface IRelation
    {
        ForeignKeyColumn[] GetForeignKeys();
        string GetQuery(ForeignKeyColumn[] foreignKeys);
        EasyDbCommand CreateCommand(object model);
        Task Fetch(object model);
    }

}
