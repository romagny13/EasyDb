using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EasyDbLib
{
    public class CloneService : ICloneService
    {
        public object ToListOrCollection<T>(T values)
        {
            var type = values.GetType();
            var singleItemType = type.GenericTypeArguments[0];
            var listType = type.GetGenericTypeDefinition().MakeGenericType(singleItemType);

            var result = Activator.CreateInstance(listType) as IList;

            foreach (var value in (IList)values)
            {
                result.Add(DeepClone(value));
            }
            return result;
        }

        public object ToDictionary<T>(T values) where T : IDictionary
        {
            var type = values.GetType();
            var keyType = type.GenericTypeArguments[0];
            var valueType = type.GenericTypeArguments[1];

            var result = Activator.CreateInstance(type) as IDictionary;

            foreach (DictionaryEntry entry in values)
            {
                result.Add(entry.Key, entry.Value);
            }
            return result;
        }

        public object ToArray<T>(T value)
        {
            var array = value as Array;
            var singleType = value.GetType().GetTypeInfo().GetElementType();

            var result = Array.CreateInstance(singleType, array.Length);

            for (int i = 0; i < array.Length; i++)
            {
                result.SetValue(DeepClone(array.GetValue(i)), i);
            }
            return result;
        }

        public object ToObject<T>(T source)
        {
            var type = source.GetType();

            var result = Activator.CreateInstance(type);

            var propertyInfos = type.GetTypeInfo().DeclaredProperties.Where(p => p.CanRead && p.CanWrite);
            foreach (var propertyInfo in propertyInfos)
            {
                var sourceValue = propertyInfo.GetValue(source);
                var resultValue = DeepClone(sourceValue);
                propertyInfo.SetValue(result, resultValue);
            }
            return result;
        }

        public T DeepClone<T>(T value)
        {
            if (value == null) return default(T);

            var type = value.GetType();
            if (type.GetTypeInfo().IsValueType || type == typeof(string))
            {
                return value;
            }
            else if (typeof(IEnumerable).IsAssignableFrom(type))
            {
                if (type.GetTypeInfo().IsGenericType)
                {
                    if (type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                    {
                        return (T)ToDictionary((IDictionary)value);
                    }
                    else
                    {
                        return (T)ToListOrCollection(value);
                    }
                }
                else if (type.IsArray)
                {
                    return (T)ToArray(value);
                }
            }
            else if (type.GetTypeInfo().IsClass)
            {
                return (T)ToObject(value);
            }
            throw new Exception("cannot resolve");
        }
    }
}
