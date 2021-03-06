﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.Common;
using System.Reflection;

namespace EasyDbLib
{
    /// <summary>
    /// Object to SQL conversion helper
    /// </summary>
    public class DbHelper
    {
        public static bool IsAllowedType(Type type)
        {
            if (type == typeof(string) || type.IsValueType)
            {
                return true;
            }
            else
            {
                var allowedTypes = new Type[]
                {
                    typeof(Byte[]),
                    typeof(Char[])
                };
                foreach (var allowedType in allowedTypes)
                {
                    if (allowedType == type)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public static void AddParameter(DbCommand command, string name, object value, ParameterDirection parameterDirection, DbType? dbType = null)
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = name;

            if (dbType.HasValue)
            {
                parameter.DbType = dbType.Value;
            }

            parameter.Direction = parameterDirection;

            parameter.Value = value ?? DBNull.Value;

            command.Parameters.Add(parameter);
        }

        public static List<string> GetSelectColumns<TModel>(Table<TModel> mapping = null) where TModel : class, new()
        {
            // get all columns
            var properties = typeof(TModel).GetProperties();
            var columns = new List<string>();
            foreach (var property in properties)
            {
                var propertyType = property.PropertyType;
                if (IsAllowedType(propertyType))
                {
                    if (mapping != null)
                    {
                        var column = mapping.GetColumnByPropertyName(property.Name);
                        if (column != null)
                        {
                            // mapping found
                            if (!column.IsIgnored)
                            {
                                columns.Add(column.ColumnName);
                            }
                        }
                        else
                        {
                            columns.Add(property.Name);
                        }
                    }
                    else
                    {
                        columns.Add(property.Name);
                    }
                }
            }
            return columns;
        }

        public static bool IsInCondition(string columnName, Check condition = null)
        {
            if (condition != null)
            {
                if (condition.ColumnName == columnName)
                {
                    return true;
                }
                if (condition.HasChainedConditions)
                {
                    foreach (var subCondition in condition.ChainedConditions)
                    {
                        if (subCondition.Condition.ColumnName == columnName)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public static Dictionary<string, object> GetInsertColumnValues<TModel>(TModel model, Table<TModel> mapping = null) where TModel : class, new()
        {
            // all columns, do not include database generated columns and ignored 
            var properties = typeof(TModel).GetProperties();
            var columnValues = new Dictionary<string, object>();
            foreach (var property in properties)
            {
                var propertyType = property.PropertyType;
                if (IsAllowedType(propertyType))
                {
                    var value = property.GetValue(model);
                    if (mapping != null)
                    {
                        var column = mapping.GetColumnByPropertyName(property.Name);
                        if (column != null)
                        {
                            // mapping found
                            if (!column.IsIgnored && !column.IsDatabaseGenerated)
                            {
                                columnValues[column.ColumnName] = value;
                            }
                        }
                        else
                        {
                            columnValues[property.Name] = value;
                        }
                    }
                    else
                    {
                        columnValues[property.Name] = value;
                    }
                }
            }
            return columnValues;
        }

        public static Dictionary<string, object> GetUpdateColumnValues<TModel>(TModel model, Table<TModel> mapping = null,
            Check condition = null) where TModel : class, new()
        {
            // all columns, do not include database generated columns and ignored 
            var properties = typeof(TModel).GetProperties();
            var columnValues = new Dictionary<string, object>();

            foreach (var property in properties)
            {
                var propertyType = property.PropertyType;
                if (IsAllowedType(propertyType))
                {
                    var value = property.GetValue(model);
                    if (mapping != null)
                    {
                        var column = mapping.GetColumnByPropertyName(property.Name);
                        if (column != null)
                        {
                            // mapping found
                            if (!column.IsIgnored
                                && !column.IsDatabaseGenerated
                                && !IsInCondition(column.ColumnName, condition))
                            {
                                columnValues[column.ColumnName] = value;
                            }
                        }
                        else
                        {
                            if (!IsInCondition(property.Name, condition))
                            {
                                columnValues[property.Name] = value;
                            }
                        }
                    }
                    else
                    {
                        if (!IsInCondition(property.Name, condition))
                        {
                            columnValues[property.Name] = value;
                        }
                    }
                }
            }
            return columnValues;
        }

        public static void AddParameterToCommand(DbCommand command, string parameterName, object value,
            ParameterDirection direction = ParameterDirection.Input)
        {
            if (!command.Parameters.Contains(parameterName))
            {
                AddParameter(command, parameterName, value, direction);
            }
        }

        public static void AddConditionParametersToCommand(DbCommand command, Check condition, IQueryService queryService)
        {
            if (condition is CheckOp)
            {
                var parameterName = queryService.GetParameterName(condition.ColumnName);
                AddParameterToCommand(command, parameterName, ((CheckOp)condition).Value);
            }

            if (condition.HasChainedConditions)
            {
                foreach (var chainedCondition in condition.ChainedConditions)
                {
                    if (chainedCondition.Condition is CheckOp)
                    {
                        var asCheckOp = chainedCondition.Condition as CheckOp;

                        var subParameterName = queryService.GetParameterName(asCheckOp.ColumnName);
                        if (asCheckOp.Rank > 1)
                        {
                            subParameterName += asCheckOp.Rank.ToString();
                        }
                        AddParameterToCommand(command, subParameterName, asCheckOp.Value);
                    }
                }
            }
        }

        public static void AddParametersToCommand(DbCommand command, Dictionary<string, object> columnValues, IQueryService queryService)
        {
            foreach (var columnValue in columnValues)
            {
                var parameterName = queryService.GetParameterName(columnValue.Key);
                if (!command.Parameters.Contains(parameterName))
                {
                    DbHelper.AddParameter(command, parameterName, columnValue.Value, ParameterDirection.Input);
                }
            }
        }

        public static Table<TModel> TryCreateEmptyTable<TModel>(string defaultPrimaryKeyName) where TModel : class, new()
        {
            var modelType = typeof(TModel);
            var tableName = modelType.Name;

            var table = new Table<TModel>(tableName);

            var property = modelType.GetProperty(defaultPrimaryKeyName,
                    BindingFlags.Public
                    | BindingFlags.Static
                    | BindingFlags.Instance
                    | BindingFlags.IgnoreCase);
            if (property != null)
            {
                table.mappingByColumnName[defaultPrimaryKeyName]
                    = new PrimaryKeyColumn(tableName, modelType, defaultPrimaryKeyName, property, true, false);
                return table;
            }
            return null;
        }

        public static ITable DiscoverMappingFor<TModel>() where TModel : class, new()
        {
            var modelType = typeof(TModel);
            var tableAttribute = modelType.GetCustomAttribute(typeof(TableAttribute)) as TableAttribute;
            var tableName = tableAttribute != null ? tableAttribute.Name : modelType.Name;

            var tableType = typeof(Table<>).MakeGenericType(new Type[] { modelType });
            var table = Activator.CreateInstance(tableType, tableName) as ITable;

            var properties = modelType.GetProperties();
            foreach (var property in properties)
            {
                if (IsAllowedType(property.PropertyType))
                {
                    var keyAttribute = property.GetCustomAttribute(typeof(KeyAttribute));

                    var columnAttribute = property.GetCustomAttribute(typeof(ColumnAttribute)) as ColumnAttribute;
                    var databaseGeneratedAttribute = property.GetCustomAttribute(typeof(DatabaseGeneratedAttribute));
                    var notMappedAttribute = property.GetCustomAttribute(typeof(NotMappedAttribute));

                    var columnName = columnAttribute != null ? columnAttribute.Name : property.Name;
                    var columnType = keyAttribute != null ? typeof(PrimaryKeyColumn) : typeof(Column);
                    var isDatabaseGenerated = databaseGeneratedAttribute != null;
                    var isIgnored = notMappedAttribute != null;

                    var column = Activator.CreateInstance(columnType, tableName, modelType, columnName, property, isDatabaseGenerated, isIgnored) as Column;
                    ((Table<TModel>)table).mappingByColumnName[columnName] = column;
                }
            }
            return table;
        }
    }
}
