using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace EasyDbLib
{

    public class EasyDb : IEasyDb
    {
        protected IMappingContainer mappingContainer;
        protected IDefaultModelFactory defaultModelFactory;
        protected IDefaultSelectionAllCommandFactory defaultSelectionAllCommandFactory;
        protected IDefaultSelectionOneCommandFactory defaultSelectionOneCommandFactory;
        protected IDefaultInsertCommandFactory defaultInsertCommandFactory;
        protected IDefaultUpdateCommandFactory defaultUpdateCommandFactory;
        protected IDefaultDeleteCommandFactory defaultDeleteCommandFactory;
        protected IDefaultCountCommandFactory defaultCountCommandFactory;
        protected IPendingOperationManager pendingOperationManager;
        protected ICloneService cloneService;
        protected IInterceptionManager interceptionManager;

        internal IQueryService queryService;

        public ConnectionWrapper WrappedConnection { get; protected set; }

        public bool HandleExecutionExceptions { get; set; }

        public DefaultMappingBehavior DefaultMappingBehavior
        {
            get { return this.mappingContainer.DefaultMappingBehavior; }
            set { this.mappingContainer.DefaultMappingBehavior = value; }
        }

        public IReadOnlyList<IDbInterceptor> Interceptors => this.interceptionManager.Interceptors;

        public IReadOnlyList<Func<Task>> PendingOperations => pendingOperationManager.PendingOperations;

        public static EasyDb Default => Singleton<EasyDb>.Instance;

        public EasyDb() : this(
            new MappingContainer(),
            new DefaultModelFactory(),
            new DefaultSelectionAllCommandFactory(),
            new DefaultSelectionOneCommandFactory(),
            new DefaultInsertCommandFactory(),
            new DefaultUpdateCommandFactory(),
            new DefaultDeleteCommandFactory(),
            new DefaultCountCommandFactory(),
            new PendingOperationManager(),
            new CloneService(),
            new InterceptionManager())
        { }

        public EasyDb(
            IMappingContainer mappingContainer,
            IDefaultModelFactory defaultModelFactory,
            IDefaultSelectionAllCommandFactory defaultSelectionAllCommandFactory,
            IDefaultSelectionOneCommandFactory defaultSelectionOneCommandFactory,
            IDefaultInsertCommandFactory defaultInsertCommandFactory,
            IDefaultUpdateCommandFactory defaultUpdateCommandFactory,
            IDefaultDeleteCommandFactory defaultDeleteCommandFactory,
            IDefaultCountCommandFactory defaultCountCommandFactory,
            IPendingOperationManager pendingOperationManager,
            ICloneService cloneService,
            IInterceptionManager interceptionManager)
        {
            this.HandleExecutionExceptions = true;

            this.mappingContainer = mappingContainer;
            this.mappingContainer.SetDb(this);

            this.cloneService = cloneService;
            this.pendingOperationManager = pendingOperationManager;
            this.interceptionManager = interceptionManager;
            this.defaultModelFactory = defaultModelFactory;

            this.defaultSelectionAllCommandFactory = defaultSelectionAllCommandFactory;
            this.defaultSelectionAllCommandFactory.SetDb(this);

            this.defaultSelectionOneCommandFactory = defaultSelectionOneCommandFactory;
            this.defaultSelectionOneCommandFactory.SetDb(this);

            this.defaultInsertCommandFactory = defaultInsertCommandFactory;
            this.defaultInsertCommandFactory.SetDb(this);

            this.defaultUpdateCommandFactory = defaultUpdateCommandFactory;
            this.defaultUpdateCommandFactory.SetDb(this);

            this.defaultDeleteCommandFactory = defaultDeleteCommandFactory;
            this.defaultDeleteCommandFactory.SetDb(this);

            this.defaultCountCommandFactory = defaultCountCommandFactory;
            this.defaultCountCommandFactory.SetDb(this);
        }

        public void SetConnectionStringSettings(string connectionString, string providerName,
            ConnectionStrategy connectionStrategy = ConnectionStrategy.Auto)
        {
            Guard.IsNullOrEmpty(connectionString);
            Guard.IsNullOrEmpty(providerName);

            this.WrappedConnection = new ConnectionWrapper(connectionString, providerName, connectionStrategy);
            this.queryService = QueryServiceFactory.GetQueryService(providerName);
        }

        public void SetConnectionStringSettings(string connectionStringName = "DefaultConnection",
            ConnectionStrategy connectionStrategy = ConnectionStrategy.Auto)
        {
            Guard.IsNullOrEmpty(connectionStringName);

            var section = ConfigurationManager.ConnectionStrings[connectionStringName];
            if (section != null)
            {
                this.SetConnectionStringSettings(section.ConnectionString, section.ProviderName, connectionStrategy);
            }
            else
            {
                Guard.Throw("No connection string section found for the connection name " + connectionStringName);
            }
        }

        public void SetConnectionStrategy(ConnectionStrategy connectionStrategy)
        {
            Guard.IsNull(this.WrappedConnection, "Connection not initialized. No connection informations provided.");

            this.WrappedConnection.ConnectionStrategy = connectionStrategy;
        }

        public void SetQueryService(string providerName, IQueryService queryService)
        {
            Guard.IsNullOrEmpty(providerName);
            Guard.IsNull(queryService);

            QueryServiceFactory.RegisterQueryService(providerName, queryService);
            this.queryService = QueryServiceFactory.GetQueryService(providerName);
        }

        public Table<TModel> TryGetTable<TModel>() where TModel : class, new()
        {
            return this.mappingContainer.TryGetTable<TModel>();
        }

        public Table<TModel> DiscoverMappingFor<TModel>() where TModel : class, new()
        {
            return this.mappingContainer.DiscoverMappingFor<TModel>();
        }

        public Table<TModel> SetTable<TModel>(string tableName) where TModel : class, new()
        {
            Guard.IsNullOrEmpty(tableName);

            return this.mappingContainer.SetTable<TModel>(tableName);
        }

        public bool IsTableRegistered<TModel>() where TModel : class, new()
        {
            return this.mappingContainer.IsTableRegistered<TModel>();
        }

        public Table<TModel> GetTable<TModel>() where TModel : class, new()
        {
            return this.mappingContainer.GetTable<TModel>();
        }

        public void AddInterceptor(IDbInterceptor interceptor)
        {
            Guard.IsNull(interceptor);

            this.interceptionManager.AddInterceptor(interceptor);
        }

        public bool RemoveInterceptor(IDbInterceptor interceptor)
        {
            Guard.IsNull(interceptor);

            return this.interceptionManager.RemoveInterceptor(interceptor);
        }

        public void ClearInterceptors()
        {
            this.interceptionManager.Clear();
        }

        public T DeepClone<T>(T value)
        {
            return this.cloneService.DeepClone<T>(value);
        }

        public T CheckDBNullAndConvertTo<T>(object value)
        {
            return value == DBNull.Value ? default(T) : (T)value;
        }

        public DbCommand CreateCommand(string commandText, CommandType commandType)
        {
            Guard.IsNullOrEmpty(commandText);
            Guard.IsNull(this.WrappedConnection, "Connection not initialized. No connection informations provided.");

            var command = this.WrappedConnection.Connection.CreateCommand();
            command.CommandText = commandText;
            command.CommandType = commandType;
            return command;
        }

        public DbCommand CreateSqlCommand(string sql)
        {
            return this.CreateCommand(sql, CommandType.Text);
        }

        public DbCommand CreateStoredProcedureCommand<TModel>(string storedProcedureName)
        {
            return this.CreateCommand(storedProcedureName, CommandType.StoredProcedure);
        }

        public async Task<List<TModel>> SelectAllAsync<TModel>(DbCommand command, Func<IDataReader, EasyDb, TModel> modelFactory) where TModel : class, new()
        {
            Guard.IsNull(command);
            Guard.IsNull(modelFactory);

            var result = new List<TModel>();

            try
            {
                this.interceptionManager.OnSelectAllExecuting(command);

                await this.WrappedConnection.CheckStrategyAndOpenAsync();

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var model = modelFactory(reader, this);
                        result.Add(model);
                    }
                }

                this.interceptionManager.OnSelectAllExecuted(command, new DbInterceptionContext<List<TModel>>(result));
            }
            catch (Exception ex)
            {
                this.interceptionManager.OnSelectAllExecuted(command, new DbInterceptionContext<List<TModel>>(result, ex));
                if (this.HandleExecutionExceptions)
                {
                    throw ex;
                }
            }
            finally
            {
                this.WrappedConnection.CheckStrategyAndClose();
            }

            return result;
        }

        public async Task<List<TModel>> SelectAllAsync<TModel>(ISelectionAllCommandFactory<TModel> selectionAllCommandFactory,
            IModelFactory<TModel> modelFactory) where TModel : class, new()
        {
            Guard.IsNull(selectionAllCommandFactory);

            TryGetTable<TModel>();

            using (var command = selectionAllCommandFactory.CreateCommand(this))
            {
                return await this.SelectAllAsync<TModel>(command, modelFactory.CreateModel);
            }
        }

        public async Task<List<TModel>> SelectAllAsync<TModel>(DbCommand command) where TModel : class, new()
        {
            TryGetTable<TModel>();

            return await this.SelectAllAsync<TModel>(command, this.defaultModelFactory.CreateModel<TModel>);
        }

        public async Task<List<TModel>> SelectAllAsync<TModel>(int? limit, Check condition, string[] sorts, Func<IDataReader, EasyDb, TModel> modelFactory) where TModel : class, new()
        {
            using (var command = this.defaultSelectionAllCommandFactory.GetCommand<TModel>(limit, condition, sorts))
            {
                return await this.SelectAllAsync<TModel>(command, modelFactory);
            }
        }

        public async Task<List<TModel>> SelectAllAsync<TModel>(int? limit = null, Check condition = null, string[] sorts = null) where TModel : class, new()
        {
            using (var command = this.defaultSelectionAllCommandFactory.GetCommand<TModel>(limit, condition, sorts))
            {
                return await this.SelectAllAsync<TModel>(command, this.defaultModelFactory.CreateModel<TModel>);
            }
        }

        public async Task<TModel> SelectOneAsync<TModel>(ISelectionOneCommandFactory<TModel> selectionOneCommandFactory, TModel model,
            IModelFactory<TModel> modelFactory) where TModel : class, new()
        {
            Guard.IsNull(selectionOneCommandFactory);

            TryGetTable<TModel>();

            using (var command = selectionOneCommandFactory.CreateCommand(this, model))
            {
                return await this.SelectOneAsync<TModel>(command, modelFactory.CreateModel);
            }
        }

        public async Task<TModel> SelectOneAsync<TModel>(DbCommand command, Func<IDataReader, EasyDb, TModel> modelFactory) where TModel : class, new()
        {
            Guard.IsNull(command);
            Guard.IsNull(modelFactory);

            TModel model = default(TModel);

            try
            {
                this.interceptionManager.OnSelectOneExecuting(command);

                await this.WrappedConnection.CheckStrategyAndOpenAsync();

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        model = modelFactory(reader, this);
                    }
                }

                this.interceptionManager.OnSelectOneExecuted(command, new DbInterceptionContext<TModel>(model));
            }
            catch (Exception ex)
            {
                this.interceptionManager.OnSelectOneExecuted(command, new DbInterceptionContext<TModel>(model, ex));
                if (this.HandleExecutionExceptions)
                {
                    throw ex;
                }
            }
            finally
            {
                this.WrappedConnection.CheckStrategyAndClose();
            }

            return model;
        }

        public async Task<TModel> SelectOneAsync<TModel>(DbCommand command) where TModel : class, new()
        {
            TryGetTable<TModel>();

            return await this.SelectOneAsync<TModel>(command, this.defaultModelFactory.CreateModel<TModel>);
        }

        public async Task<TModel> SelectOneAsync<TModel>(Check condition, Func<IDataReader, EasyDb, TModel> modelFactory) where TModel : class, new()
        {
            using (var command = this.defaultSelectionOneCommandFactory.GetCommand<TModel>(condition))
            {
                return await this.SelectOneAsync<TModel>(command, modelFactory);
            }
        }

        public async Task<TModel> SelectOneAsync<TModel>(Check condition) where TModel : class, new()
        {
            using (var command = this.defaultSelectionOneCommandFactory.GetCommand<TModel>(condition))
            {
                return await this.SelectOneAsync<TModel>(command, this.defaultModelFactory.CreateModel<TModel>);
            }
        }

        public async Task<object> InsertAsync<TModel>(DbCommand command, TModel model, Action<DbCommand, TModel, object> setNewId, DbTransaction transaction = null) where TModel : class, new()
        {
            Guard.IsNull(model);

            var table = TryGetTable<TModel>();
            if (table != null
                && table.PrimaryKeys.Count() == 1
                && table.PrimaryKeys[0].IsDatabaseGenerated)
            {
                var lastInsertedId = await this.ExecuteScalarAsync(command, transaction);
                setNewId?.Invoke(command, model, lastInsertedId);
                return lastInsertedId;
            }

            return await this.ExecuteNonQueryAsync(command, transaction);
        }

        public async Task<object> InsertAsync<TModel>(IInsertCommandFactory<TModel> insertCommandFactory, TModel model,
            DbTransaction transaction = null) where TModel : class, new()
        {
            Guard.IsNull(insertCommandFactory);
            Guard.IsNull(model);

            TryGetTable<TModel>();

            using (var command = insertCommandFactory.CreateCommand(this, model))
            {
                return await this.InsertAsync<TModel>(command, model, insertCommandFactory.SetNewId);
            }
        }

        public async Task<object> InsertAsync<TModel>(TModel model, DbTransaction transaction = null) where TModel : class, new()
        {
            Guard.IsNull(model);

            using (var command = this.defaultInsertCommandFactory.GetCommand(model))
            {
                return await this.InsertAsync<TModel>(command, model, this.defaultInsertCommandFactory.SetNewId);
            }
        }

        public async Task<int> UpdateAsync<TModel>(DbCommand command, DbTransaction transaction = null) where TModel : class, new()
        {
            TryGetTable<TModel>();

            return await this.ExecuteNonQueryAsync(command, transaction);
        }

        public async Task<int> UpdateAsync<TModel>(IUpdateCommandFactory<TModel> updateCommandFactory,
           TModel model, DbTransaction transaction = null) where TModel : class, new()
        {
            Guard.IsNull(updateCommandFactory);
            Guard.IsNull(model);

            TryGetTable<TModel>();

            using (var command = updateCommandFactory.CreateCommand(this, model))
            {
                return await this.ExecuteNonQueryAsync(command, transaction);
            }
        }

        public async Task<int> UpdateAsync<TModel>(TModel model, Check condition, DbTransaction transaction = null) where TModel : class, new()
        {
            Guard.IsNull(model);

            using (var command = this.defaultUpdateCommandFactory.GetCommand(model, condition))
            {
                return await this.ExecuteNonQueryAsync(command, transaction);
            }
        }

        public async Task<int> DeleteAsync<TModel>(DbCommand command, DbTransaction transaction = null) where TModel : class, new()
        {
            TryGetTable<TModel>();

            return await this.ExecuteNonQueryAsync(command, transaction);
        }

        public async Task<int> DeleteAsync<TModel>(IDeleteCommandFactory<TModel> deleteCommandFactory, TModel model, DbTransaction transaction = null) where TModel : class, new()
        {
            Guard.IsNull(deleteCommandFactory);
            Guard.IsNull(model);

            TryGetTable<TModel>();

            using (var command = deleteCommandFactory.CreateCommand(this, model))
            {
                return await this.ExecuteNonQueryAsync(command, transaction);
            }
        }

        public async Task<int> DeleteAsync<TModel>(Check condition, DbTransaction transaction = null) where TModel : class, new()
        {
            using (var command = this.defaultDeleteCommandFactory.GetCommand<TModel>(condition))
            {
                return await this.ExecuteNonQueryAsync(command, transaction);
            }
        }

        public async Task<int> CountAsync<TModel>(Check condition, DbTransaction transaction = null) where TModel : class, new()
        {
            TryGetTable<TModel>();

            using (var command = this.defaultCountCommandFactory.GetCommand<TModel>(condition))
            {
                object result = await this.ExecuteScalarAsync(command, transaction);
                return Convert.ToInt32(result);
            }
        }

        public async Task<bool> ExecuteTransactionFactoryAsync(TransactionFactory transactionFactory)
        {
            Guard.IsNull(transactionFactory);

            return await transactionFactory.Execute(this);
        }

        public EasyDb AddPendingOperation(Func<Task> pendingOperation)
        {
            this.pendingOperationManager.AddPendingOperation(pendingOperation);
            return this;
        }

        public async Task<bool> ExecutePendingOperationsAsync()
        {
            return await this.pendingOperationManager.Execute(this);
        }

        public async Task<int> ExecuteNonQueryAsync(DbCommand command, DbTransaction transaction = null)
        {
            Guard.IsNull(command);

            int rowsAffected = 0;
            try
            {
                if (transaction != null)
                {
                    command.Transaction = transaction;
                }

                this.interceptionManager.OnNonQueryExecuting(command);

                await this.WrappedConnection.CheckStrategyAndOpenAsync();

                rowsAffected = await command.ExecuteNonQueryAsync();

                this.interceptionManager.OnNonQueryExecuted(command, new DbInterceptionContext<int>(rowsAffected));
            }
            catch (Exception ex)
            {
                this.interceptionManager.OnNonQueryExecuted(command, new DbInterceptionContext<int>(rowsAffected, ex));
                if (this.HandleExecutionExceptions)
                {
                    throw ex;
                }
            }
            finally
            {
                this.WrappedConnection.CheckStrategyAndClose();
            }
            return rowsAffected;
        }

        public async Task<object> ExecuteScalarAsync(DbCommand command, DbTransaction transaction = null)
        {
            Guard.IsNull(command);

            object result = null;
            try
            {
                var list = new List<string>();

                if (transaction != null)
                {
                    command.Transaction = transaction;
                }

                this.interceptionManager.OnScalarExecuting(command);

                await this.WrappedConnection.CheckStrategyAndOpenAsync();

                result = await command.ExecuteScalarAsync();

                this.interceptionManager.OnScalarExecuted(command, new DbInterceptionContext<object>(result));
            }
            catch (Exception ex)
            {
                this.interceptionManager.OnScalarExecuted(command, new DbInterceptionContext<object>(result, ex));
                if (this.HandleExecutionExceptions)
                {
                    throw ex;
                }
            }
            finally
            {
                this.WrappedConnection.CheckStrategyAndClose();
            }

            return result;
        }

    }

}
