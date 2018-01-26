using System.Data.Common;

namespace EasyDbLib
{
    public class ConcurrencyInterceptor : DbInterceptor
    {
        public override void OnUpdated(DbCommand command, DbInterceptionContext interceptionContext)
        {
            if (int.TryParse(interceptionContext.Result.ToString(), out int rowsAffected))
            {
                if (rowsAffected == 0)
                {
                    throw new OptimisticConcurrencyException(string.Empty, null, interceptionContext);
                }
            }
        }
    }
}
