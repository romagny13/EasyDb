using System.Data.Common;

namespace EasyDbLib
{
    public interface ISelectionOneCommandFactory<TModel>
    {
        DbCommand CreateCommand(EasyDb db, TModel model);
    }
}
