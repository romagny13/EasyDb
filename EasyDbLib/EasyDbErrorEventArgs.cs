using System;
using System.Data.Common;

namespace EasyDbLib
{
    public class EasyDbErrorEventArgs : EventArgs
    {
        public Exception Exception { get; }
        public When When { get; }
        public DbConnection Connection { get; }
        public DbCommand Command { get; }

        public string Message => this.Exception.Message;

        public EasyDbErrorEventArgs(Exception exception, When when, DbConnection connection, DbCommand command)
        {
            this.Exception = exception;
            this.When = when;
            this.Connection = connection;
            this.Command = command;
        }
    }

    public enum When
    {
        Open,
        Close,
        ReadAll,
        ReadOne,
        NonQuery,
        Scalar
    }
}