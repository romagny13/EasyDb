using System.Collections;

namespace EasyDbLib
{
    public interface ICloneService
    {
        T DeepClone<T>(T value);
        object ToArray<T>(T value);
        object ToDictionary<T>(T values) where T : IDictionary;
        object ToListOrCollection<T>(T values);
        object ToObject<T>(T source);
    }
}