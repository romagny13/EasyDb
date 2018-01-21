using System.Data.Common;

namespace EasyDbLib
{
    public interface IInsertCommandFactory<TModel>
    {
        DbCommand CreateCommand(EasyDb db, TModel model);

        void SetNewId(DbCommand command, TModel model, object result);
    }
}
