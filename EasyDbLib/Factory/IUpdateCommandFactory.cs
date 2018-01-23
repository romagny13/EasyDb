using System.Data.Common;

namespace EasyDbLib
{
    public interface IUpdateCommandFactory<TModel> where TModel : class, new()
    {
        DbCommand CreateCommand(EasyDb db, TModel model);
    }
}
