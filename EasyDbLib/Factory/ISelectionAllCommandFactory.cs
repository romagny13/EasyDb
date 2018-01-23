using System.Data.Common;

namespace EasyDbLib
{
    public interface ISelectionAllCommandFactory<TModel, TCriteria> where TModel : class, new()
    {
        DbCommand CreateCommand(EasyDb db, TCriteria criteria);
    }
}
