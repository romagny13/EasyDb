using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace EasyDbLib
{
    public class EasyDb : IEasyDb
    {
        protected readonly List<EventHandler<EasyDbErrorEventArgs>> onError;
        public event EventHandler<EasyDbErrorEventArgs> OnError
        {
            add { if (!onError.Contains(value)) onError.Add(value); }
            remove { if (onError.Contains(value)) onError.Remove(value); }
        }

        public static EasyDb Default => Singleton<EasyDb>.Instance;

        protected IQueryService queryService;

        protected DbConnection connection;
        public DbConnection Connection => this.connection;

        public ConnectionStrategy ConnectionStrategy { get; protected set; }
        public string ConnectionString { get; protected set; }
        public string ProviderName { get; protected set; }

        public EasyDb()
        {
            this.onError = new List<EventHandler<EasyDbErrorEventArgs>>();
        }

        public EasyDb SetQueryService(string providerName, IQueryService queryService)
        {
            QueryServiceFactory.Set(providerName, queryService);
            this.queryService = QueryServiceFactory.Get(providerName);
            return this;
        }

        public EasyDb SetConnectionStringSettings(string connectionString, string providerName, 
            ConnectionStrategy connectionStrategy = ConnectionStrategy.Default)
        {
            try
            {
                var connection = DbProviderFactories.GetFactory(providerName).CreateConnection();
                if (connection == null)
                {
                    this.HandleException("No connection found for \"" + connectionString + "\" with provider \"" + providerName + "\"", When.Config);
                }

                connection.ConnectionString = connectionString;

                this.ConnectionString = connectionString;
                this.ProviderName = providerName;
                this.connection = connection;
                this.ConnectionStrategy = connectionStrategy;

                this.queryService = QueryServiceFactory.Get(this.ProviderName);
            }
            catch (Exception e)
            {
                this.HandleException(e, When.Config);
            }

            return this;
        }

        public EasyDb SetConnectionStringSettings(string connectionStringName = "Default",
            ConnectionStrategy connectionStrategy = ConnectionStrategy.Default)
        {
            var section = ConfigurationManager.ConnectionStrings[connectionStringName];
            if (section != null)
            {
                this.SetConnectionStringSettings(section.ConnectionString, section.ProviderName);
            }
            else
            {
                this.HandleException("No section found for the connection string name " + connectionStringName + " in the configuration file.", When.Config);
            }
            return this;
        }

        public EasyDb SetConnectionStrategy(ConnectionStrategy connectionStrategy)
        {
            this.ConnectionStrategy = connectionStrategy;
            return this;
        }

        public bool IsInitialized()
        {
            return this.connection != null;
        }

        public bool IsOpen()
        {
            return this.connection.State == ConnectionState.Open;
        }

        public bool IsClosed()
        {
            return this.connection.State == ConnectionState.Closed;
        }

        public ConnectionState GetState()
        {
            return this.connection.State;
        }

        public async Task<EasyDb> OpenAsync()
        {
            try
            {
                if (!this.IsOpen())
                {
                    await this.connection.OpenAsync();
                }
            }
            catch (Exception e)
            {
                this.HandleNotInitilizedException(When.Open, e);
            }

            return this;
        }

        public void Close()
        {
            try
            {
                this.connection.Close();
            }
            catch (Exception e)
            {
                this.HandleNotInitilizedException(When.Close, e);
            }
        }

        internal bool IsInvalidCommandText(string commandText)
        {
            return string.IsNullOrWhiteSpace(commandText);
        }

        public EasyDbCommand CreateCommand(string commandText, CommandType commandType)
        {
            if (!this.IsInitialized())
            {
                this.HandleNotInitilizedException(When.CreateCommand);
                return null;
            }
            else if (this.IsInvalidCommandText(commandText))
            {
                this.HandleInvalidCommandTextException();
                return null;
            }
            else
            {
                var command = this.connection.CreateCommand();
                return new EasyDbCommand(this, command, commandText, commandType);
            }
        }

        public EasyDbCommand CreateCommand(string sql)
        {
            return this.CreateCommand(sql, CommandType.Text);
        }

        public EasyDbCommand CreateStoredProcedureCommand(string procedureName)
        {
            return this.CreateCommand(procedureName, CommandType.StoredProcedure);
        }

        public SelectQuery<T> Select<T>(Table mapping) where T:new()
        {
            return new SelectQuery<T>(this.queryService, this, typeof(T), mapping);
        }

        public InsertQuery InsertInto(string tableName)
        {
            return new InsertQuery(this.queryService, this, tableName);
        }

        public UpdateQuery Update(string tableName)
        {
            return new UpdateQuery(this.queryService, this, tableName);
        }

        public DeleteQuery DeleteFrom(string tableName) { 
            return new DeleteQuery(this.queryService, this, tableName);
        }

        internal void HandleException(Exception e, When when, DbCommand command = null)
        {
            if (this.onError.Count > 0)
            {
                var context = new EasyDbErrorEventArgs(e, when, this.connection, command);
                foreach (var handler in onError)
                {
                    handler(this, context);
                }
            }
            else
            {
                throw e;
            }
        }

        internal void HandleException(string message, When when)
        {
            this.HandleException(new Exception(message), when);
        }

        internal void HandleNotInitilizedException(When when)
        {
            this.HandleException("No connection string settings provided", when);
        }

        internal void HandleInvalidCommandTextException()
        {
            this.HandleException("Invalid command text", When.CreateCommand);
        }

        internal void HandleNotInitilizedException(When when, Exception e)
        {
            if (!this.IsInitialized())
            {
                this.HandleNotInitilizedException(when);
            }
            else
            {
                this.HandleException(e, when);
            }
        }
    }
}
