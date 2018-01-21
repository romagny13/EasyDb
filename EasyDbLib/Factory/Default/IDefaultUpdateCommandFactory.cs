using System.Collections.Generic;
using System.Data.Common;

namespace EasyDbLib
{
    public interface IDefaultUpdateCommandFactory
    {
        DbCommand GetCommand<TModel>(TModel model, Check condition) where TModel : class, new();
        string GetQuery<TModel>(List<string> columns, ConditionAndParameterContainer conditionAndParameterContainer, Table<TModel> mapping = null) where TModel : class, new();
        void SetDb(EasyDb db);
    }
}