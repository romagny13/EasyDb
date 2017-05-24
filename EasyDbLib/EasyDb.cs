using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace EasyDbLib
{
    public class EasyDb
    {
        protected readonly List<EventHandler<EasyDbErrorEventArgs>> onError;
        public event EventHandler<EasyDbErrorEventArgs> OnError
        {
            add { if (!onError.Contains(value)) onError.Add(value); }
            remove { if (onError.Contains(value)) onError.Remove(value); }
        }

        public static EasyDb Default => Singleton<EasyDb>.Instance;

        protected DbConnection connection;

        public ConnectionStrategy ConnectionStrategy { get; protected set; }
        public IQueryService queryService;
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


        public EasyDb SetConnectionSettings(string connectionString, string providerName, 
            ConnectionStrategy connectionStrategy = ConnectionStrategy.Default)
        {
            var connection = DbProviderFactories.GetFactory(providerName).CreateConnection();
            if (connection == null) { throw new Exception("No connection found for \"" + connectionString + "\" with provider \"" + providerName + "\""); }

            connection.ConnectionString = connectionString;

            this.ConnectionString = connectionString;
            this.ProviderName = providerName;
            this.connection = connection;
            this.ConnectionStrategy = connectionStrategy;

            this.queryService = QueryServiceFactory.Get(this.ProviderName);

            return this;
        }

        public EasyDb SetConnectionSettings(string connectionStringName = "Default", 
            ConnectionStrategy connectionStrategy = ConnectionStrategy.Default)
        {
            var section = ConfigurationManager.ConnectionStrings[connectionStringName];
            if (section == null) { throw new Exception("No section found for the connection string name " + connectionStringName + " in the configuration file."); }

            return this.SetConnectionSettings(section.ConnectionString, section.ProviderName);
        }

        public EasyDb SetConnectionStrategy(ConnectionStrategy connectionStrategy)
        {
            this.ConnectionStrategy = connectionStrategy;
            return this;
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
               this.HandleException(e, When.Open);
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
                this.HandleException(e,When.Close);
            }
        }

        public EasyDbCommand CreateCommand(string commandText, CommandType commandType)
        {
            var command = this.connection.CreateCommand();
            return new EasyDbCommand(this, command, commandText, commandType);
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
    }
}
