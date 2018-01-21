using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace EasyDbLib
{
    public interface IDefaultModelFactory
    {
        Dictionary<Type, Dictionary<string, DefaultModelFactoryCacheEntry>> Cache { get; }

        void AddToCache(Type modelType, PropertyInfo property, bool ignoreCase);
        bool CanTrimValue(Type propertyType, object value);
        void ClearCache();
        object ConvertColumnValueToPropertyValue(PropertyInfo propertyInfo, object columnValue);
        TModel CreateModel<TModel>(IDataReader reader, EasyDb db) where TModel : class, new();
        object CreateModelInstance(Type modelType);
        object GetConvertedValue(object columnValue, Type propertyType);
        PropertyInfo GetProperty(Type modelType, string propertyName, bool ignoreCase);
        object GetValue(object model, PropertyInfo propertyInfo);
        bool IsInCache(Type modelType, string propertyName);
        void ResolveProperty<TModel>(TModel model, string columnName, object columnValue, Table<TModel> mapping = null) where TModel : class, new();
        Type ResolvePropertyType(PropertyInfo propertyInfo);
        void SetValue(object model, PropertyInfo propertyInfo, object convertedValue);
        string TrimEndString(string value);
        void TryConvertAndSetValue(object model, string expectedPropertyName, PropertyInfo property, object columnValue);
    }
}