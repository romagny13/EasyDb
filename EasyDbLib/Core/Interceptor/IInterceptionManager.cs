using System.Collections.Generic;
using System.Data.Common;

namespace EasyDbLib
{
    public interface IInterceptionManager
    {
        IReadOnlyList<IDbInterceptor> Interceptors { get; }

        void AddInterceptor(IDbInterceptor interceptor);
        void Clear();
        void OnDeleted(DbCommand command, DbInterceptionContext interceptionContext);
        void OnDeleting(DbCommand command, object model);
        void OnInserted(DbCommand command, DbInterceptionContext interceptionContext);
        void OnInserting(DbCommand command, object model);
        void OnNonQueryExecuted(DbCommand command, DbInterceptionContext interceptionContext);
        void OnNonQueryExecuting(DbCommand command);
        void OnScalarExecuted(DbCommand command, DbInterceptionContext interceptionContext);
        void OnScalarExecuting(DbCommand command);
        void OnSelectAllExecuted(DbCommand command, DbInterceptionContext interceptionContext);
        void OnSelectAllExecuting(DbCommand command);
        void OnSelectOneExecuted(DbCommand command, DbInterceptionContext interceptionContext);
        void OnSelectOneExecuting(DbCommand command);
        void OnUpdated(DbCommand command, DbInterceptionContext interceptionContext);
        void OnUpdating(DbCommand command, object model);
        bool RemoveInterceptor(IDbInterceptor interceptor);
    }
}