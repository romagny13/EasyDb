using System.Data.Common;

namespace EasyDbLib
{
    public class DbInterceptor : IDbInterceptor
    {
        public virtual void OnSelectAllExecuting(DbCommand command)
        { }

        public virtual void OnSelectAllExecuted(DbCommand command, DbInterceptionContext interceptionContext)
        { }

        public virtual void OnSelectOneExecuting(DbCommand command)
        { }

        public virtual void OnSelectOneExecuted(DbCommand command, DbInterceptionContext interceptionContext)
        { }

        public virtual void OnScalarExecuting(DbCommand command)
        { }

        public virtual void OnScalarExecuted(DbCommand command, DbInterceptionContext interceptionContext)
        { }

        public virtual void OnNonQueryExecuting(DbCommand command)
        { }

        public virtual void OnNonQueryExecuted(DbCommand command, DbInterceptionContext interceptionContext)
        { }

        public virtual void OnInserting(DbCommand command, object model)
        { }

        public virtual void OnInserted(DbCommand command, DbInterceptionContext interceptionContext)
        { }

        public virtual void OnUpdating(DbCommand command, object model)
        { }

        public virtual void OnUpdated(DbCommand command, DbInterceptionContext interceptionContext)
        { }

        public virtual void OnDeleting(DbCommand command, object model)
        { }

        public virtual void OnDeleted(DbCommand command, DbInterceptionContext interceptionContext)
        { }
    }
}
