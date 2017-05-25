namespace EasyDbLib
{
    public interface IReaderContainer
    {
        int FieldCount { get; }

        string GetName(int index);
        object GetValue(int index);
        bool IsDBNull(int index);
    }
}