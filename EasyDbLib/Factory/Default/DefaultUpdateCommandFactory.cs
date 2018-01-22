﻿using System.Collections.Generic;
using System.Data.Common;

namespace EasyDbLib
{
    public class DefaultUpdateCommandFactory : IDefaultUpdateCommandFactory
    {
        EasyDb db;

        public void SetDb(EasyDb db)
        {
            this.db = db;
        }

        public string GetQuery<TModel>(List<string> columns, ConditionAndParameterContainer conditionAndParameterContainer, Table<TModel> mapping = null) where TModel : class, new()
        {
            // example: update [User] set [UserName]=@username where where [Id]=@id and [Id2]=@id2 

            var tablename = mapping != null ? mapping.TableName : typeof(TModel).Name;

            // condition => where [Id]=@id and [Id2]=@id2 ...
            return this.db.queryService.GetUpdate(tablename, columns.ToArray(), conditionAndParameterContainer); ;
        }

        public DbCommand GetCommand<TModel>(TModel model, Check condition) where TModel : class, new()
        {
            var mapping = this.db.TryGetTable<TModel>();

            var conditionAndParameterContainer = condition != null ? new ConditionAndParameterContainer(condition, this.db.queryService) : null;

            // do not include database generated columns, and columns in condition
            var columnValues = DbHelper.GetUpdateColumnValues<TModel>(model, mapping, conditionAndParameterContainer);
            var columns = new List<string>(columnValues.Keys);
            if(columns.Count == 0) { Guard.Throw("No column for update command"); }

            var query = this.GetQuery(columns, conditionAndParameterContainer, mapping);
            var command = this.db.CreateSqlCommand(query);

            // column parameters => set [UserName]=@username ...
            DbHelper.AddParametersToCommand(command, columnValues, this.db.queryService);

            if (conditionAndParameterContainer != null)
            {
                // condition parameters => where [Id]=@id and [Id2]=@id2 ...
                DbHelper.AddConditionParametersToCommand(command, conditionAndParameterContainer);
            }

            return command;
        }
    }

}