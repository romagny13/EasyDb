using System.Collections.Generic;
using System.Data.Common;

namespace EasyDbLib
{
    public interface IDefaultInsertCommandFactory
    {
        DbCommand GetCommand<TModel>(TModel model) where TModel : class, new();
        string GetQuery<TModel>(List<string> columns, Table<TModel> mapping = null) where TModel : class, new();
        void SetDb(EasyDb db);
        void SetNewId<TModel>(DbCommand command, TModel model, object result) where TModel : class, new();
    }
}