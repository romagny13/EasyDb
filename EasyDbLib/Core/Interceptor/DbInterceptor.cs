using System.Collections.Generic;
using System.Data.Common;

namespace EasyDbLib
{
    public class DbInterceptor : IDbInterceptor
    {
        public virtual void OnSelectAllExecuting(DbCommand command)
        { }

        public virtual void OnSelectAllExecuted<TModel>(DbCommand command, DbInterceptionContext<List<TModel>> interceptionContext)
        { }

        public virtual void OnSelectOneExecuting(DbCommand command)
        { }

        public virtual void OnSelectOneExecuted<TModel>(DbCommand command, DbInterceptionContext<TModel> interceptionContext)
        { }

        public virtual void OnScalarExecuting(DbCommand command)
        { }

        public virtual void OnScalarExecuted(DbCommand command, DbInterceptionContext<object> interceptionContext)
        { }

        public virtual void OnNonQueryExecuting(DbCommand command)
        { }

        public virtual void OnNonQueryExecuted(DbCommand command, DbInterceptionContext<int> interceptionContext)
        { }
    }
}
