using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace EasyDbLib
{
    public class Table<TModel> : ITable where TModel : class, new()
    {
        public bool IgnoreCase { get; set; }

        public string TableName { get; }

        public Type ModelType => typeof(TModel);

        internal Dictionary<string, Column> mappingByColumnName;
        public IReadOnlyDictionary<string, Column> MappingByColumnName => mappingByColumnName;

        public PrimaryKeyColumn[] PrimaryKeys => (this.mappingByColumnName.Values
           .Where(p => p.GetType() == typeof(PrimaryKeyColumn))
           .Select(p => (PrimaryKeyColumn)p))
           ?.ToArray();

        public Table(string tableName)
        {
            this.mappingByColumnName = new Dictionary<string, Column>();
            this.TableName = tableName;
            this.IgnoreCase = true;
        }

        public Table<TModel> SetPrimaryKeyColumn<TPropertyType>(string columnName, Expression<Func<TModel, TPropertyType>> propertyExpression, bool isDatabaseGenerated = true, bool isIgnored = false)
        {
            Guard.IsNullOrEmpty(columnName);
            if (this.mappingByColumnName.ContainsKey(columnName)) { Guard.Throw("Column with column name " + columnName + " already registered"); }

            this.mappingByColumnName[columnName] = new PrimaryKeyColumn(TableName, typeof(TModel), columnName, ExpressionHelper.GetPropertyFromExpression(propertyExpression), isDatabaseGenerated, isIgnored);
            return this;
        }

        public Table<TModel> SetColumn<TPropertyType>(string columnName, Expression<Func<TModel, TPropertyType>> propertyExpression, bool isDatabaseGenerated = false, bool isIgnored = false)
        {
            Guard.IsNullOrEmpty(columnName);
            if (this.mappingByColumnName.ContainsKey(columnName)) { Guard.Throw("Column with column name " + columnName + " already registered"); }

            this.mappingByColumnName[columnName] = new Column(TableName, typeof(TModel), columnName, ExpressionHelper.GetPropertyFromExpression(propertyExpression), isDatabaseGenerated, isIgnored);
            return this;
        }

        public bool ContainsProperty(string propertyName)
        {
            return this.mappingByColumnName.Values.FirstOrDefault(p => p.PropertyName == propertyName) != null;
        }

        public Column GetColumnByPropertyName(string propertyName)
        {
            return this.mappingByColumnName.Values.FirstOrDefault(p => p.PropertyName == propertyName);
        }       
    }
}
