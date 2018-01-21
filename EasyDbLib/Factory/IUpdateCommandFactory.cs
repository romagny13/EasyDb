using System.Data.Common;

namespace EasyDbLib
{
    public interface IUpdateCommandFactory<TModel>
    {
        DbCommand CreateCommand(EasyDb db, TModel model);
    }
}
