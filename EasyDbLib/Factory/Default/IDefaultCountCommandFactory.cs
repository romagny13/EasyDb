using System.Data.Common;

namespace EasyDbLib
{
    public interface IDefaultCountCommandFactory
    {
        DbCommand GetCommand<TModel>(Check condition) where TModel : class, new();
        string GetQuery<TModel>(ConditionAndParameterContainer condition, Table<TModel> mapping = null) where TModel : class, new();
        void SetDb(EasyDb db);
    }
}