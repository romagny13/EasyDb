using System;

namespace EasyDbLib
{
    public interface IModelResolver
    {
        object Resolve(Type modelType, IReaderContainer reader, Mapping mapping = null);
        void ResolveProperty(object model, string columnName, object columnValue, Mapping mapping = null);
    }
}