using System.Data.Common;

namespace EasyDbLib
{
    public interface ISelectionAllCommandFactory<TModel>
    {
        DbCommand CreateCommand(EasyDb db);
    }
}
