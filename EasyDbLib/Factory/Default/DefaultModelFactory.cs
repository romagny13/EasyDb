using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace EasyDbLib
{
    public class DefaultModelFactory : IDefaultModelFactory
    {
        /// <summary>
        /// Cache for properties without mapping
        /// </summary>
        public Dictionary<Type, Dictionary<string, DefaultModelFactoryCacheEntry>> Cache { get; }

        public DefaultModelFactory()
        {
            this.Cache = new Dictionary<Type, Dictionary<string, DefaultModelFactoryCacheEntry>>();
        }

        public bool IsInCache(Type modelType, string propertyName)
        {
            return this.Cache.ContainsKey(modelType)
                && this.Cache[modelType].ContainsKey(propertyName);
        }

        public void ClearCache()
        {
            this.Cache.Clear();
        }

        public void AddToCache(Type modelType, PropertyInfo property, bool ignoreCase)
        {
            if (!this.Cache.ContainsKey(modelType))
            {
                this.Cache[modelType] = new Dictionary<string, DefaultModelFactoryCacheEntry>();
            }
            this.Cache[modelType][property.Name] = new DefaultModelFactoryCacheEntry(property, ignoreCase);
        }

        public PropertyInfo GetProperty(Type modelType, string propertyName, bool ignoreCase)
        {
            if (this.IsInCache(modelType, propertyName))
            {
                var cached = this.Cache[modelType][propertyName];
                if (cached.IgnoreCase == ignoreCase)
                {
                    return cached.Property;
                }
            }

            var property = ignoreCase ? modelType.GetProperty(propertyName,
                    BindingFlags.Public
                    | BindingFlags.Static
                    | BindingFlags.Instance
                    | BindingFlags.IgnoreCase) : modelType.GetProperty(propertyName);
            if (property != null)
            {
                this.AddToCache(modelType, property, ignoreCase);
            }
            return property;
        }

        public object CreateModelInstance(Type modelType)
        {
            return Activator.CreateInstance(modelType);
        }

        public Type ResolvePropertyType(PropertyInfo propertyInfo)
        {
            return Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;
        }

        public object GetConvertedValue(object columnValue, Type propertyType)
        {
            return columnValue == null ? null : Convert.ChangeType(columnValue, propertyType);
        }

        public bool CanTrimValue(Type propertyType, object value)
        {
            return propertyType == typeof(string) && value != null;
        }

        public string TrimEndString(string value)
        {
            return value.TrimEnd();
        }

        public object ConvertColumnValueToPropertyValue(PropertyInfo propertyInfo, object columnValue)
        {
            var propertyType = this.ResolvePropertyType(propertyInfo);
            var propertyValue = this.GetConvertedValue(columnValue, propertyType);
            if (this.CanTrimValue(propertyInfo.PropertyType, propertyValue))
            {
                return this.TrimEndString((string)propertyValue);
            }
            return propertyValue;
        }

        public object GetValue(object model, PropertyInfo propertyInfo)
        {
            return propertyInfo.GetValue(model);
        }

        public void SetValue(object model, PropertyInfo propertyInfo, object convertedValue)
        {
            propertyInfo.SetValue(model, convertedValue);
        }

        public void TryConvertAndSetValue(object model, string expectedPropertyName, PropertyInfo property, object columnValue)
        {
            var value = this.ConvertColumnValueToPropertyValue(property, columnValue);
            this.SetValue(model, property, value);
        }

        public virtual void ResolveProperty<TModel>(TModel model, string columnName, object columnValue, Table<TModel> mapping = null) where TModel : class, new()
        {
            var modelType = typeof(TModel);

            if (mapping != null)
            {
                if (mapping.MappingByColumnName.ContainsKey(columnName))
                {
                    var columnMapping = mapping.MappingByColumnName[columnName];
                    if (!columnMapping.IsIgnored)
                    {
                        this.TryConvertAndSetValue(model, columnMapping.Property.Name, columnMapping.Property, columnValue);
                    }
                }
                else
                {
                    var property = this.GetProperty(modelType, columnName, mapping.IgnoreCase);
                    if (property != null)
                    {
                        this.TryConvertAndSetValue(model, columnName, property, columnValue);
                    }
                }
            }
            else
            {
                var property = this.GetProperty(modelType, columnName, false);
                if (property != null)
                {
                    this.TryConvertAndSetValue(model, columnName, property, columnValue);
                }
            }
        }

        public virtual TModel CreateModel<TModel>(IDataReader reader, EasyDb db) where TModel : class, new()
        {
            var mapping = db.TryGetTable<TModel>();
            var modelType = typeof(TModel);

            var model = this.CreateModelInstance(modelType) as TModel;

            for (int i = 0; i < reader.FieldCount; i++)
            {
                var columnName = reader.GetName(i);
                var columnValue = reader.IsDBNull(i) ? null : reader.GetValue(i);

                this.ResolveProperty<TModel>(model, columnName, columnValue, mapping);
            }
            return model;
        }
    }

    public class DefaultModelFactoryCacheEntry
    {
        public PropertyInfo Property { get; }
        public bool IgnoreCase { get; }

        public DefaultModelFactoryCacheEntry(PropertyInfo property, bool ignoreCase)
        {
            Property = property;
            IgnoreCase = ignoreCase;
        }
    }

}