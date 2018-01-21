using System;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace EasyDbLib
{
    public class ConnectionWrapper : IDisposable
    {
        public string ConnectionString { get; }

        public string ProviderName { get; }

        internal DbConnection Connection { get; private set; }

        public ConnectionStrategy ConnectionStrategy { get; set; }

        public ConnectionWrapper(string connectionString, string providerName, ConnectionStrategy connectionStrategy)
        {
            this.Connection = DbProviderFactories.GetFactory(providerName).CreateConnection();
            Guard.IsNull(Connection, "Cannot resolve connection with informations provided.");

            this.Connection.ConnectionString = connectionString;

            this.ConnectionString = connectionString;
            this.ProviderName = providerName;

            this.ConnectionStrategy = connectionStrategy;
        }

        public bool IsOpen => this.Connection.State == ConnectionState.Open;

        public ConnectionState State => this.Connection.State;

        public DbTransaction BeginTransaction()
        {
            return this.Connection.BeginTransaction();
        }

        public async Task OpenAsync()
        {
            if (!this.IsOpen)
            {
                await this.Connection.OpenAsync();
            }
        }

        public void Close()
        {
            this.Connection.Close();
        }

        public async Task CheckStrategyAndOpenAsync()
        {
            if (this.ConnectionStrategy == ConnectionStrategy.Auto)
            {
                await this.OpenAsync();
            }
        }

        public void CheckStrategyAndClose()
        {
            if (this.ConnectionStrategy == ConnectionStrategy.Auto)
            {
                this.Close();
            }
        }

        public void Dispose()
        {
            this.Connection.Dispose();
            this.Connection = null;
            GC.SuppressFinalize(this);
        }
    }

    public enum ConnectionStrategy
    {
        Auto,
        Manual
    }
}
