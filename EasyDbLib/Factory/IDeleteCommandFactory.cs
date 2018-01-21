using System.Data.Common;

namespace EasyDbLib
{

    public interface IDeleteCommandFactory<TModel>
    {
        DbCommand CreateCommand(EasyDb db, TModel model);
    }
}
