using System.Threading.Tasks;

namespace EasyDbLib
{
    public interface IManyToManyRelation
    {
        IntermediateTable IntermediateTableMapping { get; }
        IntermediatePrimaryKeyColumn[] IntermediateTablePrimaryKeys { get; }
        IntermediatePrimaryKeyColumn[] IntermediateTablePrimaryKeysForCheckValue { get; }
        Table Mapping { get; }
        PrimaryKeyColumn[] PrimaryKeys { get; }
        string PropertyToFill { get; }

        EasyDbCommand CreateCommand(object model);
        Task Fetch(object model);
        string GetQuery();
    }
}