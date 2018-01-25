using System.Data.Common;

namespace EasyDbLib
{
    public interface IDefaultSelectionAllCommandFactory
    {
        DbCommand GetCommand<TModel>(int? limit, Check condition, string[] sorts) where TModel : class, new();
        string GetQuery<TModel>(int? limit, Check condition, string[] sorts, Table<TModel> mapping = null) where TModel : class, new();
        void SetDb(EasyDb db);
    }
}