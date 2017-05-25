using System;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace EasyDbLib
{
    public interface IEasyDb
    {
        DbConnection Connection { get; }
        ConnectionStrategy ConnectionStrategy { get; }
        string ConnectionString { get; }
        string ProviderName { get; }

        event EventHandler<EasyDbErrorEventArgs> OnError;

        void Close();
        EasyDbCommand CreateCommand(string sql);
        EasyDbCommand CreateCommand(string commandText, CommandType commandType);
        EasyDbCommand CreateStoredProcedureCommand(string procedureName);
        DeleteQuery DeleteFrom(string tableName);
        ConnectionState GetState();
        InsertQuery InsertInto(string tableName);
        bool IsClosed();
        bool IsInitialized();
        bool IsOpen();
        Task<EasyDb> OpenAsync();
        SelectQuery<T> Select<T>(Table mapping) where T : new();
        EasyDb SetConnectionStrategy(ConnectionStrategy connectionStrategy);
        EasyDb SetConnectionStringSettings(string connectionStringName = "Default", ConnectionStrategy connectionStrategy = ConnectionStrategy.Default);
        EasyDb SetConnectionStringSettings(string connectionString, string providerName, ConnectionStrategy connectionStrategy = ConnectionStrategy.Default);
        EasyDb SetQueryService(string providerName, IQueryService queryService);
        UpdateQuery Update(string tableName);
    }
}