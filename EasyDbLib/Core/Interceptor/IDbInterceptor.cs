using System.Collections.Generic;
using System.Data.Common;

namespace EasyDbLib
{
    public interface IDbInterceptor
    {
        void OnNonQueryExecuted(DbCommand command, DbInterceptionContext<int> interceptionContext);
        void OnNonQueryExecuting(DbCommand command);
        void OnScalarExecuted(DbCommand command, DbInterceptionContext<object> interceptionContext);
        void OnScalarExecuting(DbCommand command);
        void OnSelectAllExecuted<TModel>(DbCommand command, DbInterceptionContext<List<TModel>> interceptionContext);
        void OnSelectAllExecuting(DbCommand command);
        void OnSelectOneExecuted<TModel>(DbCommand command, DbInterceptionContext<TModel> interceptionContext);
        void OnSelectOneExecuting(DbCommand command);
    }
}