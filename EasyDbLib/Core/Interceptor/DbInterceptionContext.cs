using System;

namespace EasyDbLib
{
    public class DbInterceptionContext<TResult> 
    {
        public TResult Result { get; }

        public Exception Exception { get; }

        public DbInterceptionContext(TResult result, Exception exception = null)
        {
            this.Result = result;
            this.Exception = exception;
        }
    }


}
