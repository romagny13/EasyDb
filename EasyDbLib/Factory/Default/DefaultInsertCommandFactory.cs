using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace EasyDbLib
{
    public class DefaultInsertCommandFactory : IDefaultInsertCommandFactory
    {
        EasyDb db;

        public void SetDb(EasyDb db)
        {
            this.db = db;
        }

        public string GetQuery<TModel>(List<string> columns, Table<TModel> mapping = null) where TModel : class, new()
        {
            // example: insert into [User] ([UserName],[RoleId]) output inserted.id values (@username,@roleid)

            // mapping + primary key auto incremented?
            var lastInsertedId = mapping != null
                && mapping.PrimaryKeys.Count() == 1
                && mapping.PrimaryKeys[0].IsDatabaseGenerated;

            var tablename = mapping != null ? mapping.TableName : typeof(TModel).Name;
            return this.db.queryService.GetInsertInto(tablename, columns.ToArray(), lastInsertedId);
        }

        public void SetNewId<TModel>(DbCommand command, TModel model, object result) where TModel : class, new()
        {
            var mapping = this.db.TryGetTable<TModel>();

            if (command.CommandType == CommandType.Text)
            {
                if (result != null
                    && mapping != null
                    && mapping.PrimaryKeys.Count() == 1
                    && mapping.PrimaryKeys[0].IsDatabaseGenerated)
                {
                    var property = mapping.PrimaryKeys[0].Property;
                    var convertedValue = Convert.ChangeType(result, property.PropertyType);
                    property.SetValue(model, convertedValue);
                }
            }
            else if (command.CommandType == CommandType.StoredProcedure)
            {
                // not implemented
            }
        }

        public DbCommand GetCommand<TModel>(TModel model) where TModel : class, new()
        {
            var mapping = this.db.TryGetTable<TModel>();

            // all columns, do not include database generated columns and ignored
            var columnValues = DbHelper.GetInsertColumnValues<TModel>(model, mapping);
            var columns = new List<string>(columnValues.Keys);

            var query = this.GetQuery(columns, mapping);
            var command = this.db.CreateSqlCommand(query);

            // column parameters => insert ... values (@username,@roleid, ...)
            DbHelper.AddParametersToCommand(command, columnValues, this.db.queryService);

            return command;
        }
    }

}
