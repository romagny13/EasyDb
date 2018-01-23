using System.Data.Common;

namespace EasyDbLib
{
    public interface ISelectionOneCommandFactory<TCriteria>
    {
        DbCommand CreateCommand(EasyDb db, TCriteria criteria);
    }
}
