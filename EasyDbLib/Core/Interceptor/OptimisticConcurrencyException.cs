using System;

namespace EasyDbLib
{
    public class OptimisticConcurrencyException : Exception
    {
        public DbInterceptionContext InterceptionContext { get; }

        public OptimisticConcurrencyException(string message)
            : base(message)
        { }

        public OptimisticConcurrencyException(string message, Exception innerException)
           : base(message, innerException)
        { }

        public OptimisticConcurrencyException(string message, Exception innerException, DbInterceptionContext interceptionContext)
        {
            this.InterceptionContext = interceptionContext;
        }
    }
}