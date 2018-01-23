using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace EasyDbLib
{
    public interface IEasyDb
    {
        DefaultMappingBehavior DefaultMappingBehavior { get; set; }
        bool HandleExecutionExceptions { get; set; }
        IReadOnlyList<IDbInterceptor> Interceptors { get; }
        IReadOnlyList<Func<Task>> PendingOperations { get; }
        ConnectionWrapper WrappedConnection { get; }

        void AddInterceptor(IDbInterceptor interceptor);
        EasyDb AddPendingOperation(Func<Task> pendingOperation);
        T CheckDBNullAndConvertTo<T>(object value);
        void ClearInterceptors();
        Task<int> CountAsync<TModel>(Check condition, DbTransaction transaction = null) where TModel : class, new();
        DbCommand CreateCommand(string commandText, CommandType commandType);
        DbCommand CreateSqlCommand(string sql);
        DbCommand CreateStoredProcedureCommand<TModel>(string storedProcedureName);
        T DeepClone<T>(T value);
        Task<int> DeleteAsync<TModel>(Check condition, DbTransaction transaction = null) where TModel : class, new();
        Task<int> DeleteAsync<TModel>(DbCommand command, DbTransaction transaction = null) where TModel : class, new();
        Task<int> DeleteAsync<TModel>(IDeleteCommandFactory<TModel> deleteCommandFactory, TModel model, DbTransaction transaction = null) where TModel : class, new();
        Table<TModel> DiscoverMappingFor<TModel>() where TModel : class, new();
        Task<int> ExecuteNonQueryAsync(DbCommand command, DbTransaction transaction = null);
        Task<bool> ExecutePendingOperationsAsync();
        Task<object> ExecuteScalarAsync(DbCommand command, DbTransaction transaction = null);
        Task<bool> ExecuteTransactionFactoryAsync(TransactionFactory transactionFactory);
        Table<TModel> GetTable<TModel>() where TModel : class, new();
        Task<object> InsertAsync<TModel>(DbCommand command, TModel model, Action<DbCommand, TModel, object> setNewId, DbTransaction transaction = null) where TModel : class, new();
        Task<object> InsertAsync<TModel>(IInsertCommandFactory<TModel> insertCommandFactory, TModel model, DbTransaction transaction = null) where TModel : class, new();
        Task<object> InsertAsync<TModel>(TModel model, DbTransaction transaction = null) where TModel : class, new();
        bool IsTableRegistered<TModel>() where TModel : class, new();
        bool RemoveInterceptor(IDbInterceptor interceptor);
        Task<List<TModel>> SelectAllAsync<TModel, TCriteria>(ISelectionAllCommandFactory<TModel, TCriteria> selectionAllCommandFactory, IModelFactory<TModel> modelFactory, TCriteria criteria) where TModel : class, new();
        Task<List<TModel>> SelectAllAsync<TModel>(int? limit = null, Check condition = null, string[] sorts = null) where TModel : class, new();
        Task<List<TModel>> SelectAllAsync<TModel>(DbCommand command) where TModel : class, new();
        Task<List<TModel>> SelectAllAsync<TModel>(DbCommand command, Func<IDataReader, EasyDb, TModel> modelFactory) where TModel : class, new();
        Task<List<TModel>> SelectAllAsync<TModel>(int? limit, Check condition, string[] sorts, Func<IDataReader, EasyDb, TModel> modelFactory) where TModel : class, new();
        Task<List<TModel>> SelectAllAsync<TModel>(ISelectionAllCommandFactory<TModel, NullCriteria> selectionAllCommandFactory, IModelFactory<TModel> modelFactory) where TModel : class, new();
        Task<TModel> SelectOneAsync<TModel, TCriteria>(ISelectionOneCommandFactory<TCriteria> selectionOneCommandFactory, IModelFactory<TModel> modelFactory, TCriteria criteria) where TModel : class, new();
        Task<TModel> SelectOneAsync<TModel>(Check condition) where TModel : class, new();
        Task<TModel> SelectOneAsync<TModel>(Check condition, Func<IDataReader, EasyDb, TModel> modelFactory) where TModel : class, new();
        Task<TModel> SelectOneAsync<TModel>(DbCommand command) where TModel : class, new();
        Task<TModel> SelectOneAsync<TModel>(DbCommand command, Func<IDataReader, EasyDb, TModel> modelFactory) where TModel : class, new();
        void SetConnectionStrategy(ConnectionStrategy connectionStrategy);
        void SetConnectionStringSettings(string connectionStringName = "DefaultConnection", ConnectionStrategy connectionStrategy = ConnectionStrategy.Auto);
        void SetConnectionStringSettings(string connectionString, string providerName, ConnectionStrategy connectionStrategy = ConnectionStrategy.Auto);
        void SetQueryService(string providerName, IQueryService queryService);
        Table<TModel> SetTable<TModel>(string tableName) where TModel : class, new();
        Table<TModel> TryGetTable<TModel>() where TModel : class, new();
        Task<int> UpdateAsync<TModel>(DbCommand command, DbTransaction transaction = null) where TModel : class, new();
        Task<int> UpdateAsync<TModel>(IUpdateCommandFactory<TModel> updateCommandFactory, TModel model, DbTransaction transaction = null) where TModel : class, new();
        Task<int> UpdateAsync<TModel>(TModel model, Check condition, DbTransaction transaction = null) where TModel : class, new();
    }
}