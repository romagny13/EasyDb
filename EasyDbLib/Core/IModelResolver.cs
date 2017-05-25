using System;
using System.Reflection;

namespace EasyDbLib
{
    public interface IModelResolver
    {
        bool CanTrimValue(Type propertyType, object value);
        void CheckAndSetValue(object model, string expectedPropertyName, PropertyInfo propertyInfo, object columnValue);
        object ConvertColumnValueToPropertyValue(PropertyInfo propertyInfo, object columnValue);
        object CreateModelInstance(Type modelType);
        object GetConvertedValue(object columnValue, Type propertyType);
        PropertyInfo GetPropertyInfo(Type type, string name);
        PropertyInfo[] GetPropertyInfos(Type type);
        object Resolve(Type modelType, IReaderContainer reader, Table mapping = null);
        void ResolveProperty(object model, string columnName, object columnValue, Table mapping = null);
        Type ResolvePropertyType(PropertyInfo propertyInfo);
        void SetValue(object model, PropertyInfo propertyInfo, object convertedValue);
        string TrimEndString(string value);
        object GetValue(object model, PropertyInfo propertyInfo);
    }
}