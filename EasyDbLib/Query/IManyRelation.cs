namespace EasyDbLib
{
    public interface IManyRelation: IRelation
    {
        PrimaryKeyColumn[] PrimaryKeys { get; }
    }
}