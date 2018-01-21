using System;
using System.Collections.Generic;
using System.Data.Common;

namespace EasyDbLib
{

    public class InterceptionManager : IInterceptionManager
    {

        protected List<IDbInterceptor> interceptors;
        public IReadOnlyList<IDbInterceptor> Interceptors => this.interceptors;

        public InterceptionManager()
        {
            this.interceptors = new List<IDbInterceptor>();
        }

        public void AddInterceptor(IDbInterceptor interceptor)
        {
            this.interceptors.Add(interceptor);
        }

        public bool RemoveInterceptor(IDbInterceptor interceptor)
        {
            if (this.interceptors.Contains(interceptor))
            {
                this.interceptors.Remove(interceptor);
                return true;
            }
            return false;
        }

        public void Clear()
        {
            this.interceptors.Clear();
        }

        protected void Process(Action<IDbInterceptor> cb)
        {
            foreach (var interceptor in this.interceptors)
            {
                cb(interceptor);
            }
        }

        public void OnSelectAllExecuting(DbCommand command)
        {
            this.Process((interceptor) => interceptor.OnSelectAllExecuting(command));
        }

        public void OnSelectAllExecuted<TModel>(DbCommand command, DbInterceptionContext<List<TModel>> interceptionContext)
        {
            this.Process((interceptor) => interceptor.OnSelectAllExecuted(command, interceptionContext));
        }

        public void OnSelectOneExecuting(DbCommand command)
        {
            this.Process((interceptor) => interceptor.OnSelectOneExecuting(command));
        }

        public void OnSelectOneExecuted<TModel>(DbCommand command, DbInterceptionContext<TModel> interceptionContext)
        {
            this.Process((interceptor) => interceptor.OnSelectOneExecuted(command, interceptionContext));
        }

        public void OnScalarExecuting(DbCommand command)
        {
            this.Process((interceptor) => interceptor.OnScalarExecuting(command));
        }

        public void OnScalarExecuted(DbCommand command, DbInterceptionContext<object> interceptionContext)
        {
            this.Process((interceptor) => interceptor.OnScalarExecuted(command, interceptionContext));
        }

        public void OnNonQueryExecuting(DbCommand command)
        {
            this.Process((interceptor) => interceptor.OnNonQueryExecuting(command));
        }

        public void OnNonQueryExecuted(DbCommand command, DbInterceptionContext<int> interceptionContext)
        {
            this.Process((interceptor) => interceptor.OnNonQueryExecuted(command, interceptionContext));
        }

    }
}
