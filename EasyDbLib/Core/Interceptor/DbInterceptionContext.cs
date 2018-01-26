using System;

namespace EasyDbLib
{
    public class DbInterceptionContext
    {
        public object Model { get; }

        public object Result { get; }

        public Exception Exception { get; }

        public DbInterceptionContext(object result, Exception exception = null)
        {
            this.Result = result;
            this.Exception = exception;
        }

        public DbInterceptionContext(object model, object result, Exception exception = null)
            : this(result, exception)
        {
            this.Model = model;
        }

    }

}
