using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace EasyDbLib
{
    public class ModelResolver : IModelResolver
    {
        public PropertyInfo GetPropertyInfo(Type type, string name)
        {
            return type.GetProperty(name);
        }

        public PropertyInfo[] GetPropertyInfos(Type type)
        {
            return type.GetProperties();
        }

        public object CreateModelInstance(Type modelType)
        {
            return Activator.CreateInstance(modelType);
        }

        public Type ResolvePropertyType(PropertyInfo propertyInfo)
        {
            return Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;
        }

        public object GetConvertedValue(object columnValue,Type propertyType)
        {
            return columnValue == null ? null : Convert.ChangeType(columnValue, propertyType);
        }

        public bool CanTrimValue(Type propertyType, object value)
        {
            return propertyType == typeof(string) && value!=null;
        }

        public string TrimEndString(string value)
        {
            return value.TrimEnd();
        }

        public object ConvertColumnValueToPropertyValue(PropertyInfo propertyInfo, object columnValue)
        {
            var propertyType = this.ResolvePropertyType(propertyInfo);
            var propertyValue = this.GetConvertedValue(columnValue, propertyType);
            if (this.CanTrimValue(propertyInfo.PropertyType,propertyValue))
            {
                return this.TrimEndString((string)propertyValue);
            }
            return propertyValue;
        }

        public void SetValue(object model, PropertyInfo propertyInfo, object convertedValue)
        {
            propertyInfo.SetValue(model, convertedValue);
        }

        public object GetValue(object model, PropertyInfo propertyInfo)
        {
            return propertyInfo.GetValue(model);
        }

        public void CheckAndSetValue(object model, string expectedPropertyName, PropertyInfo propertyInfo, object columnValue)
        {
            if (propertyInfo != null)
            {
                var propertyValue = this.ConvertColumnValueToPropertyValue(propertyInfo, columnValue);
                this.SetValue(model, propertyInfo, propertyValue);
            }
            else
            {
                Debug.WriteLine("No property found for \"" + expectedPropertyName + "\" in \"" + model.GetType().Name + "\"");
            }
        }

        public void ResolveProperty(object model, string columnName, object columnValue, Table mapping = null)
        {
            if (mapping!= null && mapping.HasColumn(columnName))
            {
                var columnMapping = mapping.GetColumn(columnName);
                if (!columnMapping.Ignore)
                {
                    var propertyInfo = this.GetPropertyInfo(model.GetType(), columnMapping.PropertyName);
                    this.CheckAndSetValue(model, columnMapping.PropertyName, propertyInfo, columnValue);
                }
            }
            else
            {
                var propertyInfo = this.GetPropertyInfo(model.GetType(), columnName);
                this.CheckAndSetValue(model, columnName, propertyInfo, columnValue);
            }
        }

        public object Resolve(Type modelType, IReaderContainer reader, Table mapping = null)
        {
            var model = this.CreateModelInstance(modelType);
            for (int i = 0; i < reader.FieldCount; i++)
            {
                var columnName = reader.GetName(i);
                var columnValue = reader.IsDBNull(i) ? null : reader.GetValue(i);
                this.ResolveProperty(model, columnName, columnValue, mapping);
            }

            return model;
        }
    }
}