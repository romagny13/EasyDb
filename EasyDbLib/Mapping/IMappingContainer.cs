using System;
using System.Collections.Generic;

namespace EasyDbLib
{
    public interface IMappingContainer
    {
        DefaultMappingBehavior DefaultMappingBehavior { get; set; }
        Dictionary<Type, ITable> TableByModelType { get; }

        Table<TModel> DiscoverMappingFor<TModel>() where TModel : class, new();
        Table<TModel> GetTable<TModel>() where TModel : class, new();
        bool IsTableRegistered<TModel>() where TModel : class, new();
        Table<TModel> SetTable<TModel>(string tableName) where TModel : class, new();
        void SetDb(EasyDb db);
        Table<TModel> TryGetTable<TModel>() where TModel : class, new();
    }
}