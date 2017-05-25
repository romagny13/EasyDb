using System.Threading.Tasks;

namespace EasyDbLib
{
    public interface IRelation
    {
        Table Mapping { get; }
        string PropertyToFill { get; }
        ForeignKeyColumn[] ForeignKeys { get; }

        EasyDbCommand CreateCommand(object model);
        Task Fetch(object model);
        string GetQuery();
    }

}
